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
using System.Xml.Linq;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_lender_view_loan_request : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_lender_view_loan_request()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_lender_view_loan_request_Load(object sender, EventArgs e)
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

            cmbLoanTerm.Items.Clear();
            cmbLoanTerm.Items.Add("All");
            cmbLoanTerm.Items.Add("Short Term (6 Months)");
            cmbLoanTerm.Items.Add("Long Term (1 Year)");
            cmbLoanTerm.SelectedIndex = 0;

            cmbPaymentSchedule.Items.Clear();
            cmbPaymentSchedule.Items.Add("All");
            cmbPaymentSchedule.Items.Add("Daily");
            cmbPaymentSchedule.Items.Add("Weekly");
            cmbPaymentSchedule.Items.Add("Monthly");
            cmbPaymentSchedule.SelectedIndex = 0;

            LoadRequest();
            GetTotalLoanRequest();
        }

        public void LoadRequest()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();
                string query = @"
            SELECT 
                *
            FROM tblLoanRequests
            WHERE 1=1";

                // ✅ Fix: Ensure connection is closed before opening
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                cn.Open();

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    query += " AND name LIKE @name";
                }

                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    query += " AND status LIKE @status";
                }

                if (cmbPaymentSchedule.SelectedIndex != -1 && cmbPaymentSchedule.Text != "All")
                {
                    query += " AND payment_schedule LIKE @payment_schedule";
                }

                if (cmbLoanTerm.SelectedIndex != -1 && cmbLoanTerm.Text != "All")
                {
                    query += " AND loan_term LIKE @loan_term";
                }

                if (dtFrom.Checked && dtTo.Checked)
                {
                    query += " AND date_requested >= @start_date AND date_requested < @end_date";
                }
                else if (dtFrom.Checked)
                {
                    query += " AND date_requested >= @start_date";
                }
                else if (dtTo.Checked)
                {
                    query += " AND date_requested < @end_date";
                }

                query += @"
        ORDER BY 
            date_requested DESC";

                cm = new SqlCommand(query, cn);

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    cm.Parameters.AddWithValue("@name", txtSearchBorrower.Text + "%");
                }

                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    cm.Parameters.AddWithValue("@status", cmbRequestStatus.SelectedItem.ToString() + "%");
                }

                if (cmbPaymentSchedule.SelectedIndex != -1 && cmbPaymentSchedule.Text != "All")
                {
                    cm.Parameters.AddWithValue("@payment_schedule", cmbPaymentSchedule.SelectedItem.ToString() + "%");
                }

                if (cmbLoanTerm.SelectedIndex != -1 && cmbLoanTerm.Text != "All")
                {
                    cm.Parameters.AddWithValue("@loan_term", cmbLoanTerm.SelectedItem.ToString() + "%");
                }

                if (dtFrom.Checked)
                {
                    cm.Parameters.AddWithValue("@start_date", dtFrom.Value.Date);
                }
                if (dtTo.Checked)
                {
                    cm.Parameters.AddWithValue("@end_date", dtTo.Value.Date.AddDays(1));
                }

                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;

                    string monthly_income = dr["monthly_income"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["monthly_income"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string maximum_loan = dr["maximum_loan"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["maximum_loan"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string requested_loan = dr["requested_loan"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["requested_loan"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string date_requested = dr["date_requested"] != DBNull.Value
                        ? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                    int rowIndex = dataGridView1.Rows.Add(
                        i,
                        null,
                        null,
                        null,
                        null,
                        dr["borrower_profile"],
                        date_requested,
                        dr["status"].ToString(),
                        dr["name"].ToString(),
                        requested_loan,
                        monthly_income,
                        dr["loan_term"].ToString(),
                        dr["payment_schedule"].ToString(),
                        dr["id"].ToString(),
                        dr["borrower_id"].ToString(),
                        dr["interest_rate"].ToString(),
                        dr["request_number"].ToString()
                    );

                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["approve"].Value = Properties.Resources.approve;
                    row.Cells["reject"].Value = Properties.Resources.reject;
                    row.Cells["view_details"].Value = Properties.Resources.view_details;
                    row.Cells["borrower_information"].Value = Properties.Resources.borrower_information;


                    string requestStatus = dr["status"].ToString();

                    Font boldFont = new Font(dataGridView1.Font, FontStyle.Bold);

                    if (requestStatus == "Rejected" || requestStatus == "Cancelled")
                    {
                        row.Cells["request_status2"].Style.ForeColor = Color.Red;
                    }
                    else if (requestStatus == "Approved")
                    {
                        row.Cells["request_status2"].Style.ForeColor = Color.Green;
                    }
                    else if (requestStatus == "Pending")
                    {
                        row.Cells["request_status2"].Style.ForeColor = Color.Orange;
                    }
                    row.Cells["request_status2"].Style.Font = boldFont;
                }

                dr.Close();
                cn.Close();

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["borrower_profile"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSearchBorrower_TextChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        public void GetTotalLoanRequest()
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
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbRequestStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbLoanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbLoanTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
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
                    string request_id = row.Cells["request_id"].Value.ToString();
                    string status = row.Cells["request_status2"].Value?.ToString();
                    string borrower_id = row.Cells["borrower_id"].Value?.ToString();
                    string rawLoan = row.Cells["requested_loan2"].Value.ToString();
                    string frequency = row.Cells["payment_schedule"].Value?.ToString() ?? "Daily";
                    string rawTerm = row.Cells["loan_term"].Value?.ToString() ?? "Short Term (6 Months)";
                    string rawInterestRate = row.Cells["interest_rate"].Value?.ToString() ?? "0.10";
                    string request_number = row.Cells["request_number"].Value?.ToString() ?? "0";

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
                    UPDATE tblLoanRequests 
                    SET status = @status, date_reviewed = @date_reviewed, loan_status = @loan_status 
                    WHERE id = @id", cn))
                    {
                        cmd.Parameters.AddWithValue("@status", "Approved");
                        cmd.Parameters.AddWithValue("@date_reviewed", DateTime.Now);
                        cmd.Parameters.AddWithValue("@loan_status", "Ongoing");
                        cmd.Parameters.AddWithValue("@id", request_id);
                        cmd.ExecuteNonQuery();
                    }

                    // Parse loan & interest
                    string cleanedLoan = rawLoan.Replace("₱", "").Replace(",", "").Trim();
                    if (!decimal.TryParse(cleanedLoan, out decimal approvedAmount))
                    {
                        MessageBox.Show("Invalid loan amount format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cn.Close();
                        return;
                    }

                    decimal interestRate = decimal.TryParse(rawInterestRate, out var parsedRate) ? parsedRate : 0.05m;
                    int termInDays = 30;

                    if (rawTerm.Contains("Short"))
                        termInDays = 180;
                    else if (rawTerm.Contains("Long"))
                        termInDays = 360;

                    int months = termInDays / 30;
                    decimal totalInterest = approvedAmount * interestRate * months;
                    decimal totalPayable = approvedAmount + totalInterest;

                    int numberOfPayments = termInDays; // default to daily
                    if (frequency == "Weekly")
                        numberOfPayments = termInDays / 7;
                    else if (frequency == "Monthly")
                        numberOfPayments = months;

                    decimal perInstallment = Math.Round(totalPayable / numberOfPayments, 2);
                    decimal perPrincipal = Math.Round(approvedAmount / numberOfPayments, 2);
                    decimal perInterest = Math.Round(totalInterest / numberOfPayments, 2);

                    // Insert borrower loan record
                    using (SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO tblBorrowerLoanDetails (borrower_id, approved_amount)
                    VALUES (@borrower_id, @approved_amount)", cn))
                    {
                        insertCmd.Parameters.AddWithValue("@borrower_id", borrower_id);
                        insertCmd.Parameters.AddWithValue("@approved_amount", approvedAmount);
                        insertCmd.ExecuteNonQuery();
                    }

                    // Insert borrower loan record
                    using (SqlCommand insertCmdBalance = new SqlCommand(@"
                    INSERT INTO tblBorrowerBalance (borrower_id, ongoing_balance)
                    VALUES (@borrower_id, @ongoing_balance)", cn))
                    {
                        insertCmdBalance.Parameters.AddWithValue("@borrower_id", borrower_id);
                        insertCmdBalance.Parameters.AddWithValue("@ongoing_balance", approvedAmount);
                        insertCmdBalance.ExecuteNonQuery();
                    }

                    // Determine start date
                    DateTime startDate = DateTime.Today.AddDays(1);
                    if (frequency == "Weekly")
                        startDate = DateTime.Today.AddDays(7);
                    else if (frequency == "Monthly")
                        startDate = DateTime.Today.AddMonths(1);

                    // Generate payment schedule
                    for (int i = 0; i < numberOfPayments; i++)
                    {
                        DateTime dueDate = startDate.AddDays(i);
                        if (frequency == "Weekly")
                            dueDate = startDate.AddDays(i * 7);
                        else if (frequency == "Monthly")
                            dueDate = startDate.AddMonths(i);

                        using (SqlCommand scheduleCmd = new SqlCommand(@"
                        INSERT INTO tblPaymentSchedule 
                            (borrower_id, due_date, amount_to_pay, interest, total_payment, status, request_number, payment_status)
                        VALUES 
                            (@borrower_id, @due_date, @amount_to_pay, @interest, @total_payment, 'Pending', @request_number, 'Ongoing')", cn))
                        {
                            scheduleCmd.Parameters.AddWithValue("@borrower_id", borrower_id);
                            scheduleCmd.Parameters.AddWithValue("@due_date", dueDate);
                            scheduleCmd.Parameters.AddWithValue("@amount_to_pay", perPrincipal);
                            scheduleCmd.Parameters.AddWithValue("@interest", perInterest);
                            scheduleCmd.Parameters.AddWithValue("@total_payment", perInstallment);
                            scheduleCmd.Parameters.AddWithValue("@request_number", request_number);
                            scheduleCmd.ExecuteNonQuery();
                        }
                    }

                    cn.Close();
                    LoadRequest();
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

                    string request_id = selectedRow.Cells["request_id"].Value.ToString();
                    string status = selectedRow.Cells["request_status2"].Value?.ToString();

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

                    frm_rejected_reason frm = new frm_rejected_reason(this);
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

                    frm_view_request_datails frm = new frm_view_request_datails();

                    string request_id = dataGridView1.Rows[selectedRowIndex].Cells["request_id"].Value.ToString();


                    cn.Open();
                    string query = "SELECT * FROM tblLoanRequests WHERE id LIKE '" + request_id + "%'";
                    cm = new SqlCommand(query, cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        frm.txtName.Text = dr["name"].ToString();
                        frm.txtAddress.Text = dr["address"].ToString();
                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();

                        frm.txtMonthlyIncome.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_income"]));
                        frm.txtMaxLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["maximum_loan"]));

                        frm.txtLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["requested_loan"]));

                        frm.txtLoanTerm.Text = dr["loan_term"].ToString();
                        frm.txtPaymentSchedule.Text = dr["payment_schedule"].ToString();

                        frm.txtReasonForLoan.Text = dr["loan_purpose"].ToString();

                        frm.txtRequestNumber.Text = dr["request_number"].ToString();

                        string date_requested = dr["date_requested"] != DBNull.Value
                        ? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtRequestDate.Text = date_requested;

                        frm.txtRequestStatus.Text = dr["status"].ToString();

                        string date_reviewed = dr["date_reviewed"] != DBNull.Value
                        ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                        : "Pending";

                        frm.txtDateReviewed.Text = date_reviewed;

                        frm.txtRejectionReason.Text = dr["rejection_reason"] != DBNull.Value ? dr["rejection_reason"].ToString() : "N/A";

                        frm.lblAmountToReceive.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["amount_to_receive"]));

                        decimal interestRate = dr["interest_rate"] != DBNull.Value ? Convert.ToDecimal(dr["interest_rate"]) : 0;
                        frm.lblInterestRate.Text = $"{(int)(interestRate * 100)}%";

                        frm.lblMonthlyDues.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_dues"]));

                        frm.txtCancelReason.Text = dr["cancel_reason"] != DBNull.Value ? dr["cancel_reason"].ToString() : "N/A";

                        string date_cancelled = dr["date_cancelled"] != DBNull.Value
                        ? DateTime.Parse(dr["date_cancelled"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtDateCancelled.Text = date_cancelled;

                        string loanTerm = dr["loan_term"].ToString();
                        string paymentSchedule = dr["payment_schedule"].ToString();

                        if (loanTerm == "Short Term (6 Months)")
                        {
                            frm.lblInterestRate.Text = "5% (monthly)";
                        }
                        else if (loanTerm == "Long Term (1 Year)")
                        {
                            frm.lblInterestRate.Text = "10% (annually)";
                        }

                        if (paymentSchedule == "Daily")
                        {
                            frm.lblDues.Text = "Daily Dues: ";
                        }
                        else if (paymentSchedule == "Weekly")
                        {
                            frm.lblDues.Text = "Weekly Dues: ";
                        }
                        else if (paymentSchedule == "Monthly")
                        {
                            frm.lblDues.Text = "Monthly Dues: ";
                        }
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

                    string request_id = dataGridView1.Rows[selectedRowIndex].Cells["request_id"].Value.ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            cmbRequestStatus.SelectedIndex = 0;
            cmbLoanTerm.SelectedIndex = 0;
            cmbPaymentSchedule.SelectedIndex = 0;

            LoadRequest();
            GetTotalLoanRequest();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
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

        private void cmbPaymentSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
