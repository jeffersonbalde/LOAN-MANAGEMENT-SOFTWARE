using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_lender_approved_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_lender_approved_payment()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(196, 75, 128),  // Top color
                Color.FromArgb(103, 71, 219),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private void frm_lender_approved_payment_Load(object sender, EventArgs e)
        {
            dtFrom.ShowCheckBox = true;
            dtTo.ShowCheckBox = true;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            this.ActiveControl = txtSearchBorrower;

            cmbRequestStatus.Items.Clear();
            cmbRequestStatus.Items.Add("All");
            cmbRequestStatus.Items.Add("Pending");
            cmbRequestStatus.Items.Add("Approved");
            cmbRequestStatus.Items.Add("Rejected");
            cmbRequestStatus.Items.Add("Cancelled");
            cmbRequestStatus.SelectedIndex = 0;

            cmbMOP.Items.Clear();
            cmbMOP.Items.Add("All");
            cmbMOP.Items.Add("Cash");
            cmbMOP.Items.Add("Cheque");
            cmbMOP.Items.Add("Bank Transfer");
            cmbMOP.Items.Add("GCash");
            cmbMOP.Items.Add("Others");
            cmbMOP.SelectedIndex = 0;

            cmbLoanStatus.Items.Clear();
            cmbLoanStatus.Items.Add("All");
            cmbLoanStatus.Items.Add("Ongoing");
            cmbLoanStatus.Items.Add("Completed");
            cmbLoanStatus.SelectedIndex = 0;

            LoadPayment();
            GetTotalPayment();
        }

        public void LoadPayment()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();

                string query = @"
            SELECT * FROM tblLoanPayment WHERE 1 = 1";

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    query += " AND name LIKE @name";
                }

                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    query += " AND status LIKE @status";
                }

                if (cmbLoanStatus.SelectedIndex != -1 && cmbLoanStatus.Text != "All")
                {
                    query += " AND loan_status LIKE @loan_status";
                }

                if (cmbMOP.SelectedIndex != -1 && cmbMOP.Text != "All")
                {
                    query += " AND mode_of_payment LIKE @mode_of_payment";
                }

                if (dtFrom.Checked && dtTo.Checked)
                {
                    query += " AND payment_date >= @start_date AND payment_date < @end_date";
                }
                else if (dtFrom.Checked)
                {
                    query += " AND payment_date >= @start_date";
                }
                else if (dtTo.Checked)
                {
                    query += " AND payment_date < @end_date";
                }

                query += " ORDER BY payment_date DESC";

                using (SqlConnection connection = new SqlConnection(dbcon.MyConnection()))
                {
                    connection.Open();
                    using (SqlCommand cm = new SqlCommand(query, connection))
                    {

                        if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                        {
                            cm.Parameters.AddWithValue("@name", txtSearchBorrower.Text + "%");
                        }

                        if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@status", cmbRequestStatus.SelectedItem.ToString() + "%");
                        }

                        if (cmbLoanStatus.SelectedIndex != -1 && cmbLoanStatus.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@loan_status", cmbLoanStatus.SelectedItem.ToString() + "%");
                        }

                        if (cmbMOP.SelectedIndex != -1 && cmbMOP.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@mode_of_payment", cmbMOP.SelectedItem.ToString() + "%");
                        }

                        if (dtFrom.Checked)
                        {
                            cm.Parameters.AddWithValue("@start_date", dtFrom.Value.Date);
                        }
                        if (dtTo.Checked)
                        {
                            cm.Parameters.AddWithValue("@end_date", dtTo.Value.Date.AddDays(1));
                        }

                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                i++;

                                string paid_amount = dr["paid_amount"] != DBNull.Value
                                ? "₱" + Double.Parse(dr["paid_amount"].ToString()).ToString("#,##0.00")
                                : "₱0.00";

                                string date_reviewed = dr["date_reviewed"] != DBNull.Value
                                    ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                                    : "Pending";

                                string payment_date = dr["payment_date"] != DBNull.Value
                                    ? DateTime.Parse(dr["payment_date"].ToString()).ToString("MMMM d, yyyy")
                                    : "N/A";

                                int rowIndex = dataGridView1.Rows.Add(
                                    i,
                                    null,
                                    null,
                                    null,
                                    null,
                                    dr["borrower_profile"],
                                    dr["name"].ToString(),
                                    payment_date,   
                                    paid_amount,
                                    dr["status"].ToString(),
                                    date_reviewed,
                                    dr["loan_status"].ToString(),
                                    dr["mode_of_payment"].ToString(),
                                    dr["payment_number"].ToString(),
                                    dr["id"].ToString(),
                                    dr["borrower_id"].ToString()
                                );

                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["approve"].Value = Properties.Resources.approve;
                                row.Cells["reject"].Value = Properties.Resources.reject;
                                row.Cells["view_details"].Value = Properties.Resources.view_details;
                                row.Cells["borrower_information"].Value = Properties.Resources.borrower_information;

                                string requestStatus = dr["status"].ToString();
                                string loanStatus = dr["loan_status"].ToString();

                                Font boldFont = new Font(dataGridView1.Font, FontStyle.Bold);

                                if (requestStatus == "Rejected" || requestStatus == "Cancelled")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["approval_status"].Style.ForeColor = Color.Red;
                                }
                                else if (requestStatus == "Approved")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["approval_status"].Style.ForeColor = Color.Green;
                                }
                                else if (requestStatus == "Pending")
                                {
                                    row.Cells["approval_status"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["approval_status"].Style.Font = boldFont;

                                if (loanStatus == "Rejected" || loanStatus == "Cancelled")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Red;
                                }
                                else if (loanStatus == "Completed")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Green;
                                }
                                else if (loanStatus == "Ongoing" || loanStatus == "Pending")
                                {
                                    row.Cells["loan_status"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["loan_status"].Style.Font = boldFont;
                            }
                        }
                    }
                }

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["borrower_profile"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        public void GetTotalPayment()
        {

            try
            {
                int total_request = dataGridView1.Rows.Count;

                lblTotalTransactions.Text = total_request.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while counting total payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbRequestStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private void cmbLoanStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private void cmbMOP_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private void txtSearchBorrower_TextChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            cmbRequestStatus.SelectedIndex = 0;
            cmbMOP.SelectedIndex = 0;
            cmbLoanStatus.SelectedIndex = 0;

            txtSearchBorrower.Clear();

            await SafeLoadRequestAsync();
        }

        private async Task SafeLoadRequestAsync()
        {
            try
            {

                button1.Enabled = false;

                Cursor.Current = Cursors.WaitCursor;

                await Task.Delay(100);

                LoadPayment();
                GetTotalPayment();
            }
            finally
            {
                button1.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "approve")
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                        return;

                    var row = dataGridView1.SelectedRows[0];
                    string request_id = row.Cells["payment_id"].Value.ToString();
                    string status = row.Cells["approval_status"].Value?.ToString();

                    string borrower_id = row.Cells["borrower_id"].Value?.ToString();

                    string paid_amount = row.Cells["paid_amount"].Value.ToString();

                    //string frequency = row.Cells["payment_schedule"].Value?.ToString() ?? "Daily";
                    //string rawTerm = row.Cells["loan_term"].Value?.ToString() ?? "Short Term (6 Months)";
                    //string rawInterestRate = row.Cells["interest_rate"].Value?.ToString() ?? "0.10";
                    //string request_number = row.Cells["request_number"].Value?.ToString() ?? "0";

                    if (status == "Approved" || status == "Rejected" || status == "Cancelled")
                    {
                        MessageBox.Show($"❌ This loan request has already been {status.ToLower()} and cannot be approved.",
                                        "Invalid Approval", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show("Are you sure you want to approve this loan request?",
                                        "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    cn.Open();

                    // Update approval
                    using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE tblLoanPayment 
                    SET status = @status, date_reviewed = @date_reviewed
                    WHERE id = @id", cn))
                    {
                        cmd.Parameters.AddWithValue("@status", "Approved");
                        cmd.Parameters.AddWithValue("@date_reviewed", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", request_id);
                        cmd.ExecuteNonQuery();
                    }

                    // Parse loan & interest
                    string cleanedLoan = paid_amount.Replace("₱", "").Replace(",", "").Trim();
                    if (!decimal.TryParse(cleanedLoan, out decimal paid_amount_V))
                    {
                        MessageBox.Show("Invalid loan amount format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cn.Close();
                        return;
                    }
                    // Insert borrower amount paid
                    using (SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO tblBorrowerAmountPaid (borrower_id, amount_paid)
                    VALUES (@borrower_id, @amount_paid)", cn))
                    {
                        insertCmd.Parameters.AddWithValue("@borrower_id", borrower_id);
                        insertCmd.Parameters.AddWithValue("@amount_paid", paid_amount_V);
                        insertCmd.ExecuteNonQuery();
                    }

                    // Deduct Balance by updating existing balance
                    using (SqlCommand checkCmd = new SqlCommand(@"
                    SELECT ongoing_balance FROM tblBorrowerBalance WHERE borrower_id = @borrower_id", cn))
                    {
                        checkCmd.Parameters.AddWithValue("@borrower_id", borrower_id);

                        object result = checkCmd.ExecuteScalar();

                        if (result != null && decimal.TryParse(result.ToString(), out decimal currentBalance))
                        {
                            decimal newBalance = currentBalance - paid_amount_V;

                            using (SqlCommand updateCmd = new SqlCommand(@"
                            UPDATE tblBorrowerBalance 
                            SET ongoing_balance = @newBalance 
                            WHERE borrower_id = @borrower_id", cn))
                            {
                                updateCmd.Parameters.AddWithValue("@newBalance", newBalance);
                                updateCmd.Parameters.AddWithValue("@borrower_id", borrower_id);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }


                    cn.Close();
                    LoadPayment();
   
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (colName == "reject")
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                        return;

                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                    var row = dataGridView1.SelectedRows[0];

                    string request_id = row.Cells["payment_id"].Value.ToString();
                    string status = row.Cells["approval_status"].Value?.ToString();

                    if (status == "Rejected")
                    {
                        MessageBox.Show("❌ This loan request has already been rejected.",
                                        "Already Rejected",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    if (status == "Approved")
                    {
                        MessageBox.Show("❌ This loan request has already been approved and cannot be rejected.",
                                        "Cannot Reject Approved Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (status == "Cancelled")
                    {
                        MessageBox.Show("❌ This loan request has already been cancelled and cannot be reject.",
                                        "Cannot Reject Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    frm_reject_reason_payment frm = new frm_reject_reason_payment(this);
                    frm.txtRequestID.Text = request_id;
                    frm.ShowDialog();

                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (colName == "view_details")
            {
                try
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                    frm_view_payment_details frm = new frm_view_payment_details();

                    string payment_id = dataGridView1.Rows[selectedRowIndex].Cells["payment_id"].Value.ToString();


                    cn.Open();
                    string query = "SELECT * FROM tblLoanPayment WHERE id LIKE '" + payment_id + "%'";
                    cm = new SqlCommand(query, cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        frm.txtName.Text = dr["name"].ToString();
                        frm.txtAddress.Text = dr["address"].ToString();
                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();

                        if (dr["proof_of_payment"] != DBNull.Value && dr["proof_of_payment_filename"] != DBNull.Value)
                        {
                            frm.ProofOfIncomeBytes = (byte[])dr["proof_of_payment"];
                            frm.ProofOfIncomeFilename = dr["proof_of_payment_filename"].ToString();
                        }
                        else
                        {
                            frm.ProofOfIncomeBytes = null;
                            frm.ProofOfIncomeFilename = "";
                        }

                        frm.txtRequestStatus.Text = dr["status"].ToString();

                        string date_reviewed = dr["date_reviewed"] != DBNull.Value
                        ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                        : "Pending";

                        frm.txtDateReviewed.Text = date_reviewed;

                        frm.txtRejectionReason.Text = dr["rejection_reason"] != DBNull.Value ? dr["rejection_reason"].ToString() : "N/A";

                        frm.txtCancelReason.Text = dr["cancel_reason"] != DBNull.Value ? dr["cancel_reason"].ToString() : "N/A";

                        string date_cancelled = dr["date_cancelled"] != DBNull.Value
                        ? DateTime.Parse(dr["date_cancelled"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtDateCancelled.Text = date_cancelled;

                        frm.txtReferenceNumber.Text = dr["reference_number"].ToString().Trim();
                        frm.txtPaymentNumber.Text = dr["payment_number"].ToString().Trim();

                        string payment_date = dr["payment_date"] != DBNull.Value
                        ? DateTime.Parse(dr["payment_date"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtPaymentDate.Text = payment_date;


                        frm.lblCurrentBalance.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["current_balance"]));
                        frm.txtAmountPaid.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["paid_amount"]));
                        frm.txtMOP.Text = dr["mode_of_payment"].ToString();
                        frm.txtGcash.Text = dr["gcash_reference"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["gcash_reference"].ToString())
                            ? dr["gcash_reference"].ToString()
                            : "N/A";

                        frm.txtNotes.Text = dr["notes"] != DBNull.Value && !string.IsNullOrWhiteSpace(dr["notes"].ToString())
                            ? dr["notes"].ToString()
                            : "N/A";
                        frm.lblUpdatedBalance.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["updated_balance"]));
                    }

                    dr.Close();
                    cn.Close();

                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (colName == "borrower_information")
            {
                try
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                    frm_view_borrower_informations frm = new frm_view_borrower_informations();

                    //string request_id = dataGridView1.Rows[selectedRowIndex].Cells["request_id"].Value.ToString();
                    string borrower_id = dataGridView1.Rows[selectedRowIndex].Cells["borrower_id"].Value.ToString();


                    cn.Open();
                    string query = "SELECT * FROM tblBorrowerProfile WHERE id LIKE '" + borrower_id + "%'";
                    cm = new SqlCommand(query, cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {

                        if (dr["borrower_profile"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])dr["borrower_profile"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                using (Image tempImage = Image.FromStream(ms))
                                {
                                    frm.pictureBoxProfile.BackgroundImage = new Bitmap(tempImage);
                                }
                            }
                            frm.pictureBoxProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            frm.pictureBoxProfile.BackgroundImage = null;
                        }


                        frm.txtFirstName.Text = dr["first_name"].ToString();
                        frm.txtMiddleName.Text = dr["middle_name"].ToString();
                        frm.txtLastName.Text = dr["last_name"].ToString();
                        frm.txtNameSuffix.Text = dr["name_suffix"].ToString();
                        frm.txtEmail.Text = dr["email_address"].ToString();
                        frm.txtUsername.Text = dr["username"].ToString();
                        //frm.txtPassword.Text = dr["password"].ToString();  
                        frm.txtPassword.Text = "●●●●●●";
                        frm.txtAddress.Text = dr["address"].ToString();

                        string zipCode = dr["zip_code"] != DBNull.Value ? dr["zip_code"].ToString().Trim() : "";
                        frm.txtZIPCode.Text = !string.IsNullOrEmpty(zipCode) ? zipCode : "N/A";

                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();


                        if (dr["proof_of_income"] != DBNull.Value && dr["proof_of_income_filename"] != DBNull.Value)
                        {
                            frm.ProofOfIncomeBytes = (byte[])dr["proof_of_income"];
                            frm.ProofOfIncomeFilename = dr["proof_of_income_filename"].ToString();
                            frm.lblProof.Text += "  " + dr["proof_of_income_filename"].ToString();
                            frm.btnUploadProof.Text = dr["proof_of_income_filename"].ToString();
                        }
                        else
                        {
                            frm.ProofOfIncomeBytes = null;
                            frm.ProofOfIncomeFilename = "";
                        }

                        frm.txtMonthlyIncome.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_income"]));
                        frm.txtMaximumLoan.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["maximum_loan"]));

                    }
                    dr.Close();
                    cn.Close();

                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
