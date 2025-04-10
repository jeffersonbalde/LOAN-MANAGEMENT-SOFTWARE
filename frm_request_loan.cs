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
    public partial class frm_request_loan : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_request_loan()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
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

        private void panel3_Paint(object sender, PaintEventArgs e)
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

        private void frm_request_loan_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtLoanAmount;

            LoadBorrowerLoanDetails();
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

                    cmbLoanType.Text = dr["loan_type"].ToString();
                    cmbLoanTerm.Text = dr["loan_term"].ToString();
                    cmbPaymentSchedule.Text = dr["payment_schedule"].ToString();

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
        }

        private void CalculateLoanSummary(decimal principal)
        {
            Dictionary<string, decimal> interestRates = new Dictionary<string, decimal>()
            {
                { "Housing Loan", 0.07m },
                { "Business Loan", 0.10m },
                { "Personal Loan", 0.12m }
            };

            if (decimal.TryParse(txtMonthlyIncome.Text.Replace("₱", "").Replace(",", ""), out decimal income) &&
                cmbLoanType.Text != null &&
                cmbLoanTerm.Text != null &&
                cmbPaymentSchedule.Text != null)
            {
                string loanType = cmbLoanType.Text.ToString();
                string loanTermText = cmbLoanTerm.Text.ToString();
                int months = int.Parse(loanTermText.Split(' ')[0]);

                decimal interestRate = interestRates.ContainsKey(loanType) ? interestRates[loanType] : 0.12m;

                decimal totalInterest = principal * interestRate * (months / 12m);
                decimal totalPayable = principal + totalInterest;

                int payments = months;
                string schedule = cmbPaymentSchedule.Text.ToString().ToLower();

                if (schedule == "weekly") payments = months * 4;
                else if (schedule == "daily") payments = months * 30;

                decimal dues = payments > 0 ? totalPayable / payments : 0;

                lblAmountToReceive.Text = $"₱{principal:N2}";
                lblInterestRate.Text = $"{interestRate * 100}%";
                lblMonthlyDues.Text = $"₱{dues:N2}";
            }
            else
            {
                lblAmountToReceive.Text = "₱0.00";
                lblInterestRate.Text = "0%";
                lblMonthlyDues.Text = "₱0.00";
            }
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtLoanAmount.Text) ||
                    string.IsNullOrWhiteSpace(txtLoanPurpose.Text))
                {
                    MessageBox.Show("All fields are required.\n\n" +
                    "Please complete the form before saving.",
                    "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to submit your loan request?",
                            "Confirm Loan Request",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    decimal monthly_income = decimal.Parse(txtMonthlyIncome.Text.Replace("₱", "").Replace(",", "").Trim());
                    decimal maximum_loan = decimal.Parse(txtMaxLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim());
                    decimal requested_loan = decimal.Parse(txtLoanAmount.Text, System.Globalization.NumberStyles.AllowThousands);

                    decimal amount_to_receive = decimal.Parse(lblAmountToReceive.Text.Replace("₱", "").Replace(",", "").Trim());
                    decimal interestRate = decimal.Parse(lblInterestRate.Text.Replace("%", "").Trim()) / 100;
                    decimal monthlyDues = decimal.Parse(lblMonthlyDues.Text.Replace("₱", "").Replace(",", "").Trim());


                    cn.Open();

                    byte[] borrowerProfileData = null;
                    string selectQuery = "SELECT borrower_profile FROM tblBorrowerProfile WHERE id = @borrower_id";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, cn))
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
                                cn.Close();
                                return;
                            }
                        }
                    }

                    string insertQuery = "INSERT INTO tblLoanRequests " +
                        "(name, address, phone_number, monthly_income, maximum_loan, loan_type, loan_term, payment_schedule, requested_loan, loan_purpose, amount_to_receive, interest_rate, monthly_dues, status, date_requested, borrower_profile) " +
                        "VALUES (@name, @address, @phone_number, @monthly_income, @maximum_loan, @loan_type, @loan_term, @payment_schedule, @requested_loan, @loan_purpose, @amount_to_receive, @interest_rate, @monthly_dues, @status, @date_requested, @borrower_profile)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, cn))
                    {
                        insertCmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@phone_number", txtPhoneNumber.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@monthly_income", monthly_income);
                        insertCmd.Parameters.AddWithValue("@maximum_loan", maximum_loan);
                        insertCmd.Parameters.AddWithValue("@loan_type", cmbLoanType.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@loan_term", cmbLoanTerm.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@payment_schedule", cmbPaymentSchedule.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@requested_loan", requested_loan);
                        insertCmd.Parameters.AddWithValue("@loan_purpose", txtLoanPurpose.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@amount_to_receive", amount_to_receive);
                        insertCmd.Parameters.AddWithValue("@interest_rate", interestRate);
                        insertCmd.Parameters.AddWithValue("@monthly_dues", monthlyDues);
                        insertCmd.Parameters.AddWithValue("@status", "Pending");
                        insertCmd.Parameters.AddWithValue("@date_requested", DateTime.Now);
                        insertCmd.Parameters.AddWithValue("@borrower_profile", borrowerProfileData);

                        insertCmd.ExecuteNonQuery();
                    }

                    cn.Close();


                    MessageBox.Show("Registration saved successfully!",
                                    "Registration Complete",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
