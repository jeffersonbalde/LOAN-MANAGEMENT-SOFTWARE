using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_request_loan : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_request_history frm;

        public frm_borrower_request_loan(frm_request_history frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void frm_borrower_request_loan_Load(object sender, EventArgs e)
        {
            cmbLoanTerm.Items.Clear();
            cmbLoanTerm.Items.Add("Short Term (6 Months)");
            cmbLoanTerm.Items.Add("Long Term (1 Year)");
            cmbLoanTerm.SelectedIndex = 0;

            cmbPaymentSchedule.Items.Clear();
            cmbPaymentSchedule.Items.Add("Daily");
            cmbPaymentSchedule.Items.Add("Weekly");
            cmbPaymentSchedule.Items.Add("Monthly");
            cmbPaymentSchedule.SelectedIndex = 0;

            LoadBorrowerLoanDetails();
            GetRequestNumber();

            txtRequestDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
            txtRequestStatus.Text = "Pending";
            txtDateReviewed.Text = "Pending";
            txtRejectionReason.Text = "N/A";

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void txtLoanAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == 46)
            {
                if (txtLoanAmount.Text.Contains(".") || txtLoanAmount.SelectionStart == 0)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtLoanAmount_TextChanged(object sender, EventArgs e)
        {
            txtLoanAmount.TextChanged -= txtLoanAmount_TextChanged;

            try
            {
                if (!string.IsNullOrWhiteSpace(txtLoanAmount.Text))
                {
                    string rawText = txtLoanAmount.Text.Replace(",", "");

                    if (rawText.EndsWith("."))
                    {
                        txtLoanAmount.TextChanged += txtLoanAmount_TextChanged;
                        return;
                    }

                    double number;
                    if (double.TryParse(rawText, out number))
                    {
                        txtLoanAmount.Text = number.ToString("#,##0.###");

                        txtLoanAmount.SelectionStart = txtLoanAmount.Text.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                txtLoanAmount.Text = "";
            }

            txtLoanAmount.TextChanged += txtLoanAmount_TextChanged;

            decimal requestedAmount;
            decimal maxLoanAmount;

            string cleanedMaxLoan = txtMaxLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim();

            if (decimal.TryParse(txtLoanAmount.Text.Replace(",", ""), out requestedAmount) &&
                decimal.TryParse(cleanedMaxLoan, out maxLoanAmount))
            {
                if (requestedAmount > maxLoanAmount)
                {
                    MessageBox.Show("❌ Requested loan amount cannot be greater than the maximum allowed loan amount.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLoanAmount.Clear();
                    txtLoanAmount.Focus();

                    lblAmountToReceive.Text = "₱0.00";
                    lblInterestRate.Text = "0%";
                    lblMonthlyDues.Text = "₱0.00";
                    return;
                }

                CalculateLoanSummary(requestedAmount);
            }
            else
            {
                lblAmountToReceive.Text = "₱0.00";
                lblInterestRate.Text = "0%";
                lblMonthlyDues.Text = "₱0.00";
            }
        }


        private void CalculateLoanSummary(decimal principal)
        {
            if (!string.IsNullOrWhiteSpace(txtMonthlyIncome.Text) &&
                cmbLoanTerm.SelectedItem != null &&
                cmbPaymentSchedule.SelectedItem != null)
            {
                if (decimal.TryParse(txtMonthlyIncome.Text.Replace("₱", "").Replace(",", ""), out decimal income))
                {
                    string loanTermText = cmbLoanTerm.SelectedItem.ToString().Trim();
                    string schedule = cmbPaymentSchedule.SelectedItem.ToString().Trim().ToLower();

                    // Determine term duration and interest type
                    int months = loanTermText.Contains("6") ? 6 : 12;

                    decimal interestRate;
                    string rateType;

                    if (loanTermText.Contains("Short Term"))
                    {
                        interestRate = 0.05m;
                        rateType = "monthly";
                    }
                    else
                    {
                        interestRate = 0.10m;
                        rateType = "annually";
                    }

                    // Calculate interest
                    decimal totalInterest = rateType == "monthly"
                        ? principal * interestRate * months
                        : principal * interestRate * (months / 12m);

                    decimal totalPayable = principal + totalInterest;

                    // Determine number of payments
                    int payments = months;
                    if (schedule == "weekly") payments = months * 4;
                    else if (schedule == "daily") payments = months * 30;

                    decimal dues = payments > 0 ? totalPayable / payments : 0;

                    lblAmountToReceive.Text = $"₱{principal:N2}";
                    lblInterestRate.Text = $"{(int)(interestRate * 100)}% ({rateType})";
                    lblMonthlyDues.Text = $"₱{dues:N2}";
                    return;
                }
            }

            lblAmountToReceive.Text = "₱0.00";
            lblInterestRate.Text = "0%";
            lblMonthlyDues.Text = "₱0.00";
        }

        public void LoadBorrowerLoanDetails()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblBorrowerProfile WHERE id = @id";
                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@id", txtID.Text);

                dr = cm.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    txtMonthlyIncome.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_income"]));
                    txtMaxLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["maximum_loan"]));

                    string first_name = dr["first_name"].ToString();
                    string last_name = dr["last_name"].ToString();

                    txtName.Text = first_name + " " + last_name;

                    txtAddress.Text = dr["address"].ToString();
                    txtPhoneNumber.Text = dr["phone_number"].ToString();
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void cmbLoanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLoanAmount.Text) &&
                decimal.TryParse(txtLoanAmount.Text.Replace(",", ""), out decimal principal))
            {
                CalculateLoanSummary(principal);
            }

        }

        private void cmbLoanTerm_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPaymentSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentSchedule.SelectedItem != null)
            {
                string schedule = cmbPaymentSchedule.SelectedItem.ToString().ToLower();

                if (schedule == "daily")
                {
                    if (!lblDues.Text.Contains("Daily"))
                        lblDues.Text = lblDues.Text.Replace("Monthly", "Daily")
                                                   .Replace("Weekly", "Daily");
                }
                else if (schedule == "weekly")
                {
                    if (!lblDues.Text.Contains("Weekly"))
                        lblDues.Text = lblDues.Text.Replace("Monthly", "Weekly")
                                                   .Replace("Daily", "Weekly");
                }
                else if (schedule == "monthly")
                {
                    if (!lblDues.Text.Contains("Monthly"))
                        lblDues.Text = lblDues.Text.Replace("Daily", "Monthly")
                                                   .Replace("Weekly", "Monthly");
                }
            }

            if (!string.IsNullOrWhiteSpace(txtLoanAmount.Text) &&
            decimal.TryParse(txtLoanAmount.Text.Replace(",", ""), out decimal principal))
            {
                CalculateLoanSummary(principal);
            }
        }

        private void cmbLoanTerm_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLoanAmount.Text) &&
            decimal.TryParse(txtLoanAmount.Text.Replace(",", ""), out decimal principal))
            {
                CalculateLoanSummary(principal);
            }
        }

        public void GetRequestNumber()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                string transno;
                int count;

                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    string query = "SELECT MAX(request_number) FROM tblRequestNumber WHERE request_number LIKE @sdate + '%'";
                    cm = new SqlCommand(query, cn, transaction);
                    cm.Parameters.AddWithValue("@sdate", sdate);

                    object result = cm.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        transno = result.ToString();
                        count = int.Parse(transno.Substring(8)) + 1;
                    }
                    else
                    {
                        count = 100001; 
                    }

                    transno = sdate + count.ToString("D6"); 

                    string insertQuery = "INSERT INTO tblRequestNumber (request_number) VALUES (@request_number)";
                    cm = new SqlCommand(insertQuery, cn, transaction);
                    cm.Parameters.AddWithValue("@request_number", transno);
                    cm.ExecuteNonQuery();

                    transaction.Commit();
                    txtRequestNumber.Text = transno;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtLoanAmount.Text) ||
                    string.IsNullOrWhiteSpace(cmbLoanTerm.Text) ||
                    string.IsNullOrWhiteSpace(cmbPaymentSchedule.Text) ||
                    string.IsNullOrWhiteSpace(txtReasonForLoan.Text))
                {
                    MessageBox.Show("All fields are required.\n\nPlease complete the form before saving.",
                        "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal requested_loan_v = decimal.Parse(
                    txtLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim(),
                    System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
                    System.Globalization.CultureInfo.InvariantCulture
                );

                if (requested_loan_v == 0)
                {
                    MessageBox.Show("Loan amount cannot be zero. Please enter a valid amount.",
                        "Invalid Loan Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLoanAmount.Focus();
                    return;
                }

                if (requested_loan_v % 100 != 0)
                {
                    MessageBox.Show("The loan amount must be in clean hundreds only (e.g., ₱100, ₱200, ₱500, ₱1000).\n\nAvoid amounts like ₱523 or ₱990.",
                        "Invalid Loan Amount Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLoanAmount.Focus();
                    return;
                }

                if (MessageBox.Show("Are you sure you want to submit your loan request?",
                        "Confirm Loan Request",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                decimal monthly_income = decimal.Parse(txtMonthlyIncome.Text.Replace("₱", "").Replace(",", "").Trim());
                decimal maximum_loan = decimal.Parse(txtMaxLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim());
                decimal requested_loan = decimal.Parse(
                 txtLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim(),
                 System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
                 System.Globalization.CultureInfo.InvariantCulture
                );

                decimal amount_to_receive = decimal.Parse(lblAmountToReceive.Text.Replace("₱", "").Replace(",", "").Trim());

                //decimal interestRate = decimal.Parse(lblInterestRate.Text.Replace("%", "").Trim()) / 100;

                string interestText = lblInterestRate.Text.Split('%')[0].Trim(); // Extract number before '%'
                decimal interestRate = decimal.Parse(interestText) / 100;

                decimal monthlyDues = decimal.Parse(lblMonthlyDues.Text.Replace("₱", "").Replace(",", "").Trim());

                byte[] borrowerProfileData = null;

                cn.Open();

                using (SqlCommand selectCmd = new SqlCommand("SELECT borrower_profile FROM tblBorrowerProfile WHERE id = @borrower_id", cn))
                {
                    selectCmd.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());
                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            borrowerProfileData = (byte[])reader["borrower_profile"];
                        }
                        else
                        {
                            MessageBox.Show("Borrower profile not found.");
                            return;
                        }
                    }
                }

                cn.Close();

                cn.Open();

                using (SqlCommand insertCmd = new SqlCommand(@"
            INSERT INTO tblLoanRequests 
                (name, address, phone_number, monthly_income, maximum_loan, loan_term, 
                 payment_schedule, requested_loan, loan_purpose, amount_to_receive, interest_rate, 
                 monthly_dues, status, date_requested, borrower_profile, loan_status, borrower_id, request_number) 
            VALUES 
                (@name, @address, @phone_number, @monthly_income, @maximum_loan, @loan_term, 
                 @payment_schedule, @requested_loan, @loan_purpose, @amount_to_receive, @interest_rate, 
                 @monthly_dues, @status, @date_requested, @borrower_profile, @loan_status, @borrower_id, @request_number)", cn))
                {
                    insertCmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@phone_number", txtPhoneNumber.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@monthly_income", monthly_income);
                    insertCmd.Parameters.AddWithValue("@maximum_loan", maximum_loan);
                    insertCmd.Parameters.AddWithValue("@loan_term", cmbLoanTerm.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@payment_schedule", cmbPaymentSchedule.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@requested_loan", requested_loan);
                    insertCmd.Parameters.AddWithValue("@loan_purpose", txtReasonForLoan.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@amount_to_receive", amount_to_receive);
                    insertCmd.Parameters.AddWithValue("@interest_rate", interestRate);
                    insertCmd.Parameters.AddWithValue("@monthly_dues", monthlyDues);
                    insertCmd.Parameters.AddWithValue("@status", "Pending");
                    insertCmd.Parameters.AddWithValue("@date_requested", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@borrower_profile", borrowerProfileData);
                    insertCmd.Parameters.AddWithValue("@loan_status", "Pending");
                    insertCmd.Parameters.AddWithValue("@borrower_id", txtID.Text);
                    insertCmd.Parameters.AddWithValue("@request_number", txtRequestNumber.Text);

                    insertCmd.ExecuteNonQuery();

                    //using (SqlCommand loanDetailsCmd = new SqlCommand(@"
                    //INSERT INTO tblBorrowerLoanDetails (borrower_id, approved_amount)
                    //VALUES (@borrower_id, @approved_amount)", cn))
                    //{
                    //    loanDetailsCmd.Parameters.AddWithValue("@borrower_id", txtID.Text);
                    //    loanDetailsCmd.Parameters.AddWithValue("@approved_amount", 0.00m);
                    //    loanDetailsCmd.ExecuteNonQuery();
                    //}
                }

                cn.Close();

                //MessageBox.Show("Registration saved successfully!",
                //    "Registration Complete",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Information);

                frm.LoadRequest();
                frm.GetTotalLoanRequest();

                this.Dispose();
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
