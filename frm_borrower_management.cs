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

        frm_borrower_main_form frm;

        public frm_borrower_management(frm_borrower_main_form frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void frm_borrower_management_Load(object sender, EventArgs e)
        {
            RetrieveProofOfIncome(int.Parse(txtID.Text));

            lblFileName.Text = "✅ File uploaded successfully!";
            lblFileName.ForeColor = Color.Green;
            siticoneProgressBar1.Value = 100;

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

            if (this.Tag is Dictionary<string, string> tagData)
            {

                if (tagData.ContainsKey("loan_term"))
                {
                    string make = tagData["loan_term"];

                    int index = cmbLoanTerm.FindStringExact(make);
                    if (index != -1)
                    {
                        cmbLoanTerm.SelectedIndex = index;
                    }
                    else
                    {
                        cmbLoanTerm.Items.Add(make);
                        cmbLoanTerm.SelectedItem = make;
                    }
                }

                if (tagData.ContainsKey("loan_type"))
                {
                    string model = tagData["loan_type"];

                    int index = cmbLoanType.FindStringExact(model);
                    if (index != -1)
                    {
                        cmbLoanType.SelectedIndex = index;
                    }
                    else
                    {
                        cmbLoanType.Items.Add(model);
                        cmbLoanType.SelectedItem = model;
                    }
                }

                if (tagData.ContainsKey("loan_type"))
                {
                    string model = tagData["loan_type"];

                    int index = cmbLoanType.FindStringExact(model);
                    if (index != -1)
                    {
                        cmbLoanType.SelectedIndex = index;
                    }
                    else
                    {
                        cmbLoanType.Items.Add(model);
                        cmbLoanType.SelectedItem = model;
                    }
                }

                if (tagData.ContainsKey("payment_schedule"))
                {
                    string model = tagData["payment_schedule"];

                    int index = cmbLoanType.FindStringExact(model);
                    if (index != -1)
                    {
                        cmbPaymentSchedule.SelectedIndex = index;
                    }
                    else
                    {
                        cmbPaymentSchedule.Items.Add(model);
                        cmbPaymentSchedule.SelectedItem = model;
                    }
                }
            }
        }

        private void RetrieveProofOfIncome(int borrowerId)
        {
            try
            {
                cn.Open();
                string query = "SELECT proof_of_income, proof_of_income_filename FROM tblBorrowerProfile WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@id", borrowerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                byte[] fileData = (byte[])reader["proof_of_income"];
                                string fileName = reader["proof_of_income_filename"].ToString();

                                string tempFolder = Path.Combine(Path.GetTempPath(), "LoanAppFiles");
                                Directory.CreateDirectory(tempFolder); 

                                proofOfIncomeFilePath = Path.Combine(tempFolder, fileName); 

                                File.WriteAllBytes(proofOfIncomeFilePath, fileData);

                                btnUploadProof.Text = fileName;
                            }
                            else
                            {
                                MessageBox.Show("No proof of income file found.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cn.Close();
            }
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

                    if (rawText.EndsWith("."))
                    {
                        txtMonthlyIncome.TextChanged += txtMonthlyIncome_TextChanged;
                        return;
                    }

                    double number;
                    if (double.TryParse(rawText, out number))
                    {
                        txtMonthlyIncome.Text = number.ToString("#,##0.###");

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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) |
                    string.IsNullOrWhiteSpace(txtAddress.Text) ||
                    string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtMonthlyIncome.Text) ||
                    string.IsNullOrWhiteSpace(cmbLoanTerm.Text) ||
                    string.IsNullOrWhiteSpace(cmbLoanType.Text) ||
                    string.IsNullOrWhiteSpace(cmbPaymentSchedule.Text))
                {
                    MessageBox.Show("All fields are required.\n\n" +
                        "Please complete the form before saving.",
                        "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pictureBoxUserImage.BackgroundImage == null)
                {
                    MessageBox.Show("No profile image has been uploaded.\n\n" +
                                    "Please upload a profile image or set the default profile before saving.",
                                    "Image Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (lblEmailValidation.Text == "❌ Invalid Email Format")
                {
                    MessageBox.Show("The email address you entered is not valid. Please enter a valid email address.", "Invalid Email Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }


                if (lblUppercase.ForeColor != Color.Green ||
                    lblLowercase.ForeColor != Color.Green ||
                    lblSpecialChar.ForeColor != Color.Green)
                {
                    MessageBox.Show("Your password must contain at least \n\n" +
                        "one uppercase letter, " +
                        "one lowercase letter, " +
                        "and one special character. \n\n" +
                        "Please adjust your password accordingly.",
                                    "Invalid Password",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (txtPhoneNumber.Text.Length != 11 || !long.TryParse(txtPhoneNumber.Text, out _))
                {
                    MessageBox.Show("Contact number must be an 11-digit number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhoneNumber.Focus();
                    return;
                }

                if (lblLoanEligibility.ForeColor != Color.Green)
                {
                    MessageBox.Show("Your monthly income does not meet our loan eligibility criteria. Please verify your income and try again.",
                                    "Invalid Monthly Income",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    txtMonthlyIncome.Focus();
                    return;
                }


                if (lblFileName.ForeColor != Color.Green)
                {
                    MessageBox.Show("Please upload a valid proof of payment file.",
                                    "Invalid File",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    btnUploadProof.Focus();
                    return;
                }

                if (MessageBox.Show("Are you sure you want to update your registration details?",
                            "Confirm Registration",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] profileImageData;
                    using (MemoryStream msProfile = new MemoryStream())
                    {
                        pictureBoxUserImage.BackgroundImage.Save(msProfile, System.Drawing.Imaging.ImageFormat.Jpeg);
                        profileImageData = msProfile.ToArray();
                    }

                    string proofOfIncomeFilePath = this.proofOfIncomeFilePath;
                    byte[] proofFileData = System.IO.File.ReadAllBytes(proofOfIncomeFilePath);

                    decimal monthlyIncome = decimal.Parse(txtMonthlyIncome.Text, System.Globalization.NumberStyles.AllowThousands);

                    string maxLoanText = txtMaxLoanAmount.Text.Replace("₱", "").Replace(",", "").Trim();
                    decimal maximumLoan = decimal.Parse(maxLoanText);

                    cn.Open();

                    string query = "UPDATE tblBorrowerProfile " +
                                   "SET borrower_profile = @borrower_profile, first_name = @first_name, last_name = @last_name, email_address = @email_address, password = @password, phone_number = @phone_number, address = @address, zip_code = @zip_code, monthly_income = @monthly_income, proof_of_income = @proof_of_income, maximum_loan = @maximum_loan, loan_type = @loan_type, loan_term = @loan_term, payment_schedule = @payment_schedule, date_registered = @date_registered, proof_of_income_filename = @proof_of_income_filename " +
                                   "WHERE id LIKE @id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@borrower_profile", profileImageData);
                        cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@last_name", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email_address", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone_number", txtPhoneNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@zip_code", txtZipCode.Text.Trim());
                        cmd.Parameters.AddWithValue("@monthly_income", monthlyIncome);
                        cmd.Parameters.AddWithValue("@proof_of_income", proofFileData);
                        cmd.Parameters.AddWithValue("@maximum_loan", maximumLoan);
                        cmd.Parameters.AddWithValue("@loan_type", cmbLoanType.Text.Trim());
                        cmd.Parameters.AddWithValue("@loan_term", cmbLoanTerm.Text.Trim());
                        cmd.Parameters.AddWithValue("@payment_schedule", cmbPaymentSchedule.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_registered", DateTime.Now);
                        cmd.Parameters.AddWithValue("@proof_of_income_filename", Path.GetFileName(proofOfIncomeFilePath));
                        cmd.Parameters.AddWithValue("@id", txtID.Text);

                        cmd.ExecuteNonQuery();
                    }

                    cn.Close();

                    MessageBox.Show("Profile updated successfully!",
                                    "Profile Updated",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);


                    if (Application.OpenForms["frm_borrower_main_form"] is frm_borrower_main_form borrowerForm)
                    {
                        borrowerForm.LoadBorrowerProfile();
                        borrowerForm.LoadBorrowerName();
                    }

                    this.Dispose();
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
