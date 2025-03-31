using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_management : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        private string proofOfIncomeFilePath = string.Empty;

        private Dictionary<string, string> zipCodeData = new Dictionary<string, string>()
        {
            // Metro Manila
            { "Manila", "1000" },
            { "Makati", "1200" },
            { "Quezon City", "1100" },
            { "Pasig", "1600" },
            { "Taguig", "1630" },
            { "Mandaluyong", "1550" },
            { "Caloocan", "1400" },
            { "Las Piñas", "1740" },
            { "Parañaque", "1700" },
            { "Pasay", "1300" },

            // Luzon
            { "Baguio City", "2600" },
            { "Dagupan City", "2400" },
            { "San Fernando, Pampanga", "2000" },
            { "Batangas City", "4200" },
            { "Lucena City", "4301" },
            { "Legazpi City", "4500" },

            // Visayas
            { "Cebu City", "6000" },
            { "Lapu-Lapu City", "6015" },
            { "Mandaue City", "6014" },
            { "Iloilo City", "5000" },
            { "Bacolod City", "6100" },
            { "Dumaguete City", "6200" },
            { "Tacloban City", "6500" },

            // Mindanao
            { "Davao City", "8000" },
            { "Cagayan de Oro", "9000" },
            { "Zamboanga City", "7000" },
            { "General Santos City", "9500" },
            { "Pagadian City", "7016" },
            { "Butuan City", "8600" },
        };

        private Image animatedGif;
        private bool isPasswordVisible = false;

        public frm_borrower_management()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_borrower_management_Load(object sender, EventArgs e)
        {
            cmbLoanTerm.Items.Add("6 Months");
            cmbLoanTerm.Items.Add("12 Months");
            cmbLoanTerm.Items.Add("24 Months");
            cmbLoanTerm.Items.Add("36 Months");

            cmbLoanType.Items.Add("Housing Loan");
            cmbLoanType.Items.Add("Business Loan");
            cmbLoanType.Items.Add("Personal Loan");

            cmbPaymentSchedule.Items.Add("Daily");
            cmbPaymentSchedule.Items.Add("Weekly");
            cmbPaymentSchedule.Items.Add("Monthly");

            this.ActiveControl = txtFirstName;
        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Invalid input! Only numbers are allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (txtPhoneNumber.Text.Length >= 11 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Phone number cannot exceed 11 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '\0';
                btnShowHide.Image = Properties.Resources.eye_open;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                btnShowHide.Image = Properties.Resources.eye_close;
            }
        }


        private void UpdateValidationLabel(Label lbl, bool isValid, string validText, string invalidText)
        {
            lbl.Text = isValid ? validText : invalidText;
            lbl.ForeColor = isValid ? Color.Green : Color.Red;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            string password = txtPassword.Text;

            bool hasUpperCase = Regex.IsMatch(password, "[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, "[a-z]");
            bool hasSpecialChar = Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]");

            UpdateValidationLabel(lblUppercase, hasUpperCase, "✔ Must contain an uppercase letter", "✖ Must contain an uppercase letter");
            UpdateValidationLabel(lblLowercase, hasLowerCase, "✔ Must contain a lowercase letter", "✖ Must contain a lowercase letter");
            UpdateValidationLabel(lblSpecialChar, hasSpecialChar, "✔ Must contain a special character", "✖ Must contain a special character");
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                lblEmailValidation.Text = "✅ Valid Email";
                lblEmailValidation.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblEmailValidation.Text = "❌ Invalid Email Format";
                lblEmailValidation.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            string input = txtAddress.Text.Trim().ToLower();
            string matchedZip = "";

            foreach (var entry in zipCodeData)
            {
                if (input.Contains(entry.Key.ToLower()))
                {
                    matchedZip = entry.Value;
                    break;
                }
            }

            txtZipCode.Text = matchedZip;
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxUserImage.BackgroundImage = Properties.Resources.default_user1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting default image: " + ex.Message);
            }
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = string.Empty;
                openFileDialog1.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxUserImage.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the image.\n\n" +
                                "Error Details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtMonthlyIncome_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == 46)
            {
                if (txtMonthlyIncome.Text.Contains(".") || txtMonthlyIncome.SelectionStart == 0)
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

        private void txtMonthlyIncome_TextChanged(object sender, EventArgs e)
        {
            txtMonthlyIncome.TextChanged -= txtMonthlyIncome_TextChanged;

            try
            {
                if (!string.IsNullOrWhiteSpace(txtMonthlyIncome.Text))
                {
                    string rawText = txtMonthlyIncome.Text.Replace(",", "");

                    // Skip formatting if the last character is a dot (allow intermediate input)
                    if (rawText.EndsWith("."))
                    {
                        txtMonthlyIncome.TextChanged += txtMonthlyIncome_TextChanged;
                        return;
                    }

                    // Parse the input as a double if valid (without commas)
                    double number;
                    if (double.TryParse(rawText, out number))
                    {
                        // Format the number with commas and preserve decimals
                        txtMonthlyIncome.Text = number.ToString("#,##0.###");

                        // Preserve the cursor position at the end
                        txtMonthlyIncome.SelectionStart = txtMonthlyIncome.Text.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                txtMonthlyIncome.Text = "";
            }

            txtMonthlyIncome.TextChanged += txtMonthlyIncome_TextChanged;


            if (string.IsNullOrWhiteSpace(txtMonthlyIncome.Text))
            {

                lblLoanEligibility.Text = "❌ Not Eligible ";
                lblLoanEligibility.ForeColor = Color.Red;
                txtMaxLoanAmount.Text = "₱0";
            }

            if (decimal.TryParse(txtMonthlyIncome.Text.Replace(",", ""), out decimal income))
            {

                if (income >= 15000)
                {
                    lblLoanEligibility.Text = "✅ Eligible";
                    lblLoanEligibility.ForeColor = Color.Green;
                    ComputeMaxLoanAmount(income);
                }
                else
                {
                    lblLoanEligibility.Text = "❌ Not Eligible";
                    lblLoanEligibility.ForeColor = Color.Red;
                    txtMaxLoanAmount.Text = "₱0";
                }
            }
        }

        private void ComputeMaxLoanAmount(decimal income)
        {
            decimal multiplier;

            if (income >= 50000) multiplier = 4;
            else if (income >= 30001) multiplier = 3;
            else if (income >= 15000) multiplier = 2;
            else multiplier = 0;

            decimal maxLoan = income * multiplier;
            txtMaxLoanAmount.Text = "₱" + maxLoan.ToString("N0");
        }

        private void btnUploadProof_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select Proof of Income",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Filter = "Valid Files (*.pdf;*.jpg;*.jpeg;*.png)|*.pdf;*.jpg;*.jpeg;*.png|PDF Files (*.pdf)|*.pdf|Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    proofOfIncomeFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(proofOfIncomeFilePath);

                    StartUploadAnimation(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while selecting the file:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartUploadAnimation(string fileName)
        {
            siticoneProgressBar1.Value = 0;
            lblFileName.Text = "Uploading...";
            lblFileName.ForeColor = Color.Orange;

            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) =>
            {
                if (siticoneProgressBar1.Value < 100)
                {
                    siticoneProgressBar1.Value += 10;
                }
                else
                {
                    timer.Stop();
                    lblFileName.Text = "✅ File uploaded successfully!";
                    lblFileName.ForeColor = Color.Green;
                    btnUploadProof.Text = fileName;
                }
            };
            timer.Start();
        }
    }
}
