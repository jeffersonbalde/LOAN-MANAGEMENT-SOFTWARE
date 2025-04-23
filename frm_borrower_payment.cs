using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_borrower_payment()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            string borrowerId = txtID.Text;

            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                string statusQuery = @"
                SELECT TOP 1 loan_status 
                FROM tblLoanRequests 
                WHERE borrower_id = @borrowerId 
                ORDER BY id DESC";

                SqlCommand statusCmd = new SqlCommand(statusQuery, cn);
                statusCmd.Parameters.AddWithValue("@borrowerId", borrowerId);
                object result = statusCmd.ExecuteScalar();

                if (result != null)
                {
                    string loanStatus = result.ToString();

                    if (loanStatus == "Rejected" || loanStatus == "Cancelled")
                    {
                        MessageBox.Show("❌ You have no valid loan record. Please request a loan first before adding a payment.",
                            "No Active Loan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (loanStatus == "Pending")
                    {
                        MessageBox.Show("⏳ Please wait for your lender's approval on your loan request before adding a payment.",
                            "Loan Request Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("⚠️ No loan requests found. Please request a loan first before adding a payment.",
                        "No Loan Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string latestQuery = @"
                SELECT TOP 1 status, loan_status 
                FROM tblLoanPayment 
                WHERE borrower_id = @borrowerId 
                ORDER BY id DESC";
                SqlCommand latestCmd = new SqlCommand(latestQuery, cn);
                latestCmd.Parameters.AddWithValue("@borrowerId", borrowerId);

                SqlDataReader reader = latestCmd.ExecuteReader();

                if (reader.Read())
                {
                    string status = reader["status"].ToString();
                    string loanStatus = reader["loan_status"].ToString();

                    if (status == "Pending")
                    {
                        reader.Close();
                        MessageBox.Show("⚠️ You already have a pending loan payment. Please wait for lender approval before adding a new payment.",
                            "Pending Loan Payment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                reader.Close();

                frm_borrower_add_payment frm = new frm_borrower_add_payment(this);
                frm.txtID.Text = borrowerId;
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "view_request")
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

                        //frm.txtLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["requested_loan"]));

                        //frm.txtLoanTerm.Text = dr["loan_term"].ToString();


                        //frm.txtReasonForLoan.Text = dr["loan_purpose"].ToString();

                        //frm.txtRequestNumber.Text = dr["request_number"].ToString();

                        //string date_requested = dr["date_requested"] != DBNull.Value
                        //? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                        //: "N/A";

                        //frm.txtRequestDate.Text = date_requested;









                        //frm.lblAmountToReceive.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["amount_to_receive"]));

                        ////decimal interestRate = dr["interest_rate"] != DBNull.Value ? Convert.ToDecimal(dr["interest_rate"]) : 0;
                        ////frm.lblInterestRate.Text = $"{(int)(interestRate * 100)}%";

                        //frm.lblMonthlyDues.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_dues"]));






                        //string loanTerm = dr["loan_term"].ToString();
                        //string paymentSchedule = dr["payment_schedule"].ToString();

                        //if (loanTerm == "Short Term (6 Months)")
                        //{
                        //    frm.lblInterestRate.Text = "5% (monthly)";
                        //}
                        //else if (loanTerm == "Long Term (1 Year)")
                        //{
                        //    frm.lblInterestRate.Text = "10% (annually)";
                        //}

                        //if (paymentSchedule == "Daily")
                        //{
                        //    frm.lblDues.Text = "Daily Dues: ";
                        //}
                        //else if (paymentSchedule == "Weekly")
                        //{
                        //    frm.lblDues.Text = "Weekly Dues: ";
                        //}
                        //else if (paymentSchedule == "Monthly")
                        //{
                        //    frm.lblDues.Text = "Monthly Dues: ";
                        //}
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

            if (colName == "cancel_request")
            {
                try
                {

                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

                    string request_id = selectedRow.Cells["payment_id"].Value.ToString();
                    string status = selectedRow.Cells["approval_status"].Value?.ToString();

                    if (status == "Rejected")
                    {
                        MessageBox.Show("This payment has already been rejected and cannot be cancelled.",
                                        "Payment Already Rejected",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    if (status == "Approved")
                    {
                        MessageBox.Show("This payment has already been approved and cannot be cancelled.",
                                        "Payment Already Approved",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (status == "Cancelled")
                    {
                        MessageBox.Show("This payment has already been cancelled.",
                                        "Payment Already Cancelled",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    frm_cancel_payment frm = new frm_cancel_payment(this);
                    frm.txtRequestID.Text = request_id;
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("An error occurred while attempting to cancel the payment:\n\n" + ex.Message,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        public void LoadPayment()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();

                string query = @"
            SELECT * FROM tblLoanPayment WHERE borrower_id = @borrower_id";

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
                        cm.Parameters.AddWithValue("@borrower_id", txtID.Text);

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
                                    dr["payment_number"].ToString(),
                                    payment_date,
                                    date_reviewed,
                                    paid_amount,
                                    dr["status"].ToString(),
                                    dr["loan_status"].ToString(),
                                    dr["mode_of_payment"].ToString(),
                                    dr["id"].ToString(),
                                    dr["borrower_id"].ToString()
                                );

                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["view_request"].Value = Properties.Resources.view_payment;
                                row.Cells["cancel_request"].Value = Properties.Resources.cancel_request;



                                string requestStatus = dr["status"].ToString();
                                string loanStatus = dr["loan_status"].ToString();
                                //string c_date_reviewed = dr["date_reviewed"].ToString();

                                Font boldFont = new Font(dataGridView1.Font, FontStyle.Bold);

                                //if (c_date_reviewed == "Pending")
                                //{
                                //    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                //    row.Cells["date_reviewed"].Style.ForeColor = Color.Orange;
                                //}

                                //row.Cells["date_reviewed"].Style.Font = boldFont;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void frm_borrower_payment_Load(object sender, EventArgs e)
        {
            dtFrom.ShowCheckBox = true;
            dtTo.ShowCheckBox = true;

            dtFrom.Checked = false;
            dtTo.Checked = false;

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

        public void GetTotalPayment()
        {

            try
            {
                int total_request = dataGridView1.Rows.Count;

                lblTotalTransactions.Text = total_request.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while counting total request: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadPayment();
            GetTotalPayment();
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

        private async void button1_Click(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            cmbRequestStatus.SelectedIndex = 0;
            cmbMOP.SelectedIndex = 0;
            cmbLoanStatus.SelectedIndex = 0;

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
    }
}
