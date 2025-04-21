using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Cryptography;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using PdfiumViewer;



namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_create_account : Form
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

        public frm_create_account()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }


        private void CenterPanel()
        {
            int centerX = (this.ClientSize.Width - panel1.Width) / 2;
            int centerY = (this.ClientSize.Height - panel1.Height) / 2;

            panel1.Location = new Point(centerX, centerY);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_landing_page login_form = new frm_landing_page();
            login_form.Show();
        }

        private void frm_create_account_Load(object sender, EventArgs e)
        {
            CenterPanel();

            cmbLoanTerm.Items.Clear();
            cmbLoanTerm.Items.Add("Short Term (6 Months)");
            cmbLoanTerm.Items.Add("Long Term (1 Year)");
            cmbLoanTerm.SelectedIndex = 0;

            cmbPaymentSchedule.Items.Clear();
            cmbPaymentSchedule.Items.Add("Daily");
            cmbPaymentSchedule.Items.Add("Weekly");
            cmbPaymentSchedule.Items.Add("Monthly");
            cmbPaymentSchedule.SelectedIndex = 0;

            this.ActiveControl = txtFirstName;

            LoadBusinessLogo();

            txtPhoneNumber.Text = "+63 ";
            txtPhoneNumber.SelectionStart = txtPhoneNumber.Text.Length;

            panelValidation.Visible = false;
            panelValidation.BackColor = Color.FromArgb(120, Color.White);

        }

        public void LoadBusinessLogo()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblBusinessProfile";
                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    if (dr["business_logo"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])dr["business_logo"];
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            using (Image tempImage = Image.FromStream(ms))
                            {
                                pictureBoxLogo.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        pictureBoxLogo.BackgroundImage = null;
                    }

                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frm_create_account_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            frm_landing_page frm = new frm_landing_page();
            frm.Show();
        }

        private void frm_create_account_Resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void siticoneTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            Siticone.Desktop.UI.WinForms.SiticoneTextBox textBox = sender as Siticone.Desktop.UI.WinForms.SiticoneTextBox;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Only numbers are allowed.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (char.IsControl(e.KeyChar) && textBox.SelectionStart <= 4 && textBox.SelectionLength == 0)
            {
                e.Handled = true;
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
            panelValidation.Visible = true;

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
            int selStart = txtEmail.SelectionStart;
            txtEmail.Text = txtEmail.Text.ToUpper();
            txtEmail.SelectionStart = selStart;

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                lblEmailIcon.Text = "✅"; 
                lblEmailIcon.ForeColor = Color.Green;
            }
            else
            {
                lblEmailIcon.Text = "❌"; 
                lblEmailIcon.ForeColor = Color.Red;
            }
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtAddress.SelectionStart;
            txtAddress.Text = txtAddress.Text.ToUpper();
            txtAddress.SelectionStart = selStart;

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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtMiddleName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text) |
                    string.IsNullOrWhiteSpace(txtAddress.Text) ||
                    string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtMonthlyIncome.Text) ||
                    string.IsNullOrWhiteSpace(cmbLoanTerm.Text) ||
                    string.IsNullOrWhiteSpace(cmbPaymentSchedule.Text))
                    {
                        MessageBox.Show("All fields are required.\n\n" +
                        "Please complete the form before saving.",
                        "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //if (pictureBoxUserImagee.BackgroundImage == null)
                //{
                //    MessageBox.Show("No profile image has been uploaded.\n\n" +
                //                    "Please upload a profile image or set the default profile before saving.",
                //                    "Image Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                if (lblEmailIcon.Text == "❌")
                {
                    MessageBox.Show("The email address you entered is not valid. Please enter a valid email address.", "Invalid Email Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }


                cn.Open();

                string emailCheckQuery = "SELECT COUNT(*) FROM tblBorrowerProfile WHERE email_address = @email_address";
                using (SqlCommand checkCmd = new SqlCommand(emailCheckQuery, cn))
                {
                    checkCmd.Parameters.AddWithValue("@email_address", txtEmail.Text);
                    int emailCount = (int)checkCmd.ExecuteScalar();
                    if (emailCount > 0)
                    {
                        MessageBox.Show("The email address you entered already exists. Please log in instead.",
                                        "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cn.Close();
                        txtEmail.Focus();
                        return;
                    }
                }

                cn.Close();


                cn.Open();

                string usernameCheckQuery = "SELECT COUNT(*) FROM tblBorrowerProfile WHERE username = @username";
                using (SqlCommand checkCmd = new SqlCommand(usernameCheckQuery, cn))
                {
                    checkCmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    int emailCount = (int)checkCmd.ExecuteScalar();
                    if (emailCount > 0)
                    {
                        MessageBox.Show("The username you entered already exists. Please log in instead.",
                                        "Duplicate Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cn.Close();
                        txtUsername.Focus();
                        return;
                    }
                }

                cn.Close();


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

                string rawDigits = new string(txtPhoneNumber.Text.Where(char.IsDigit).ToArray());

                if (!txtPhoneNumber.Text.StartsWith("+63 ") || rawDigits.Length != 12 || !rawDigits.StartsWith("63"))
                {
                    MessageBox.Show("Phone number must be in the format: +63 XXX XXX XXXX", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                if (!chkTermsAndConditions.Checked)
                {
                    MessageBox.Show("Please agree to the Terms and Conditions before proceeding.",
                                    "Terms & Conditions",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    chkTermsAndConditions.Focus();
                    return;
                }


                if (MessageBox.Show("Are you sure you want to save your registration details?",
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

                    decimal amountToReceive = decimal.Parse(lblAmountToReceive.Text.Replace("₱", "").Replace(",", "").Trim());

                    //decimal interestRate = decimal.Parse(lblInterestRate.Text.Replace("%", "").Trim()) / 100;

                    string interestText = lblInterestRate.Text.Split('%')[0].Trim(); // Extract number before '%'
                    decimal interestRate = decimal.Parse(interestText) / 100;

                    decimal monthlyDues = decimal.Parse(lblMonthlyDues.Text.Replace("₱", "").Replace(",", "").Trim());

                    cn.Open();

                    string query = "INSERT INTO tblBorrowerProfile " +
                                   "(borrower_profile, first_name, last_name, email_address, password, phone_number, address, zip_code, monthly_income, proof_of_income, maximum_loan, loan_term, payment_schedule, date_registered, proof_of_income_filename, amount_to_receive, interest_rate, monthly_dues, middle_name, name_suffix, username) " +
                                   "VALUES (@borrower_profile, @first_name, @last_name, @email_address, @password, @phone_number, @address, @zip_code, @monthly_income, @proof_of_income, @maximum_loan, @loan_term, @payment_schedule, @date_registered, @proof_of_income_filename, @amount_to_receive, @interest_rate, @monthly_dues, @middle_name, @name_suffix, @username)";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@borrower_profile", profileImageData);
                        cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@last_name", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email_address", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", HashPassword(txtPassword.Text.Trim()));
                        cmd.Parameters.AddWithValue("@phone_number", txtPhoneNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@zip_code", txtZipCode.Text.Trim());
                        cmd.Parameters.AddWithValue("@monthly_income", monthlyIncome);
                        cmd.Parameters.AddWithValue("@proof_of_income", proofFileData);
                        cmd.Parameters.AddWithValue("@maximum_loan", maximumLoan);
                        cmd.Parameters.AddWithValue("@loan_term", cmbLoanTerm.Text.Trim());
                        cmd.Parameters.AddWithValue("@payment_schedule", cmbPaymentSchedule.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_registered", DateTime.Now);
                        cmd.Parameters.AddWithValue("@proof_of_income_filename", Path.GetFileName(proofOfIncomeFilePath));
                        cmd.Parameters.AddWithValue("@amount_to_receive", amountToReceive);
                        cmd.Parameters.AddWithValue("@interest_rate", interestRate);
                        cmd.Parameters.AddWithValue("@monthly_dues", monthlyDues);
                        cmd.Parameters.AddWithValue("@middle_name", txtMiddleName.Text.Trim());
                        cmd.Parameters.AddWithValue("@name_suffix", txtNameSuffix.Text.Trim());
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    cn.Close();

                    //MessageBox.Show("Registration saved successfully!",
                    //                "Registration Complete",
                    //                MessageBoxButtons.OK,
                    //                MessageBoxIcon.Information);

                    frm_borrower_main_form frm = new frm_borrower_main_form();
                    frm.lblUser.Text = txtFirstName.Text + " " + txtLastName.Text;

                    cn.Open();
                    string staffQuery = "SELECT * FROM tblBorrowerProfile WHERE username = @username";
                    cm = new SqlCommand(staffQuery, cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                    dr = cm.ExecuteReader();

                    byte[] userImage = null;
                    int userId = 0;
                    string storedHashedPassword = "";

                    if (dr.Read())
                    {
                        storedHashedPassword = dr["password"].ToString();

                        if (!VerifyPassword(txtPassword.Text.Trim(), storedHashedPassword))
                        {
                            MessageBox.Show("Password mismatch. Please try again.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dr.Close();
                            cn.Close();
                            return;
                        }

                        if (dr["borrower_profile"] != DBNull.Value)
                        {
                            userImage = (byte[])dr["borrower_profile"];
                            userId = Convert.ToInt32(dr["id"]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username not found.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dr.Close();
                        cn.Close();
                        return;
                    }

                    dr.Close();
                    cn.Close();


                    if (userImage != null)
                    {
                        using (MemoryStream ms = new MemoryStream(userImage))
                        {
                            frm.pictureBoxProfile.Image = Image.FromStream(ms);
                        }
                    }

                    frm.txtID.Text = userId.ToString();

                    this.Hide();
                    frm.Show();
                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in inputBytes)
                    builder.Append(b.ToString("x2"));
                string hashedInputPassword = builder.ToString();

                return hashedInputPassword == storedHashedPassword;
            }
        }


        private void siticoneButton3_Click(object sender, EventArgs e)
        {

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
                    pictureBoxUserImagee.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the image.\n\n" +
                                "Error Details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void siticoneTextBox5_KeyPress_1(object sender, KeyPressEventArgs e)
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


            if (string.IsNullOrWhiteSpace(txtMonthlyIncome.Text)) {

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

            CalculateLoanSummary();
        }

        private void ComputeMaxLoanAmount(decimal income)
        {
            try
            {
                if (income > 1000000000)
                {
                    MessageBox.Show("The income value is too large. Please enter a valid amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMonthlyIncome.Clear();
                    txtMonthlyIncome.Focus();
                    return;
                }

                // 30% of the monthly income
                decimal maxLoan = income * 0.30m;

                txtMaxLoanAmount.Text = "₱" + maxLoan.ToString("N0");
            }
            catch (OverflowException)
            {
                MessageBox.Show("The income value is too large and caused an overflow.", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMonthlyIncome.Clear();
                txtMonthlyIncome.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void siticoneButton4_Click(object sender, EventArgs e)
        {
            siticoneButton4_Click(sender, e, btnUploadProof);
        }

        private void siticoneButton4_Click(object sender, EventArgs e, Siticone.Desktop.UI.WinForms.SiticoneButton btnUploadProof)
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
                    string extension = Path.GetExtension(proofOfIncomeFilePath).ToLower();

                    btnUploadProof.BackgroundImageLayout = ImageLayout.Stretch;
                    btnUploadProof.Text = "";
                    btnUploadProof.FillColor = Color.Transparent;
                    btnUploadProof.BackColor = Color.Transparent;
                    btnUploadProof.UseTransparentBackground = true;

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        btnUploadProof.BackgroundImage = Image.FromFile(proofOfIncomeFilePath);
                    }
                    else if (extension == ".pdf")
                    {
                        using (var document = PdfiumViewer.PdfDocument.Load(proofOfIncomeFilePath))
                        {
                            using (var pdfImage = document.Render(0, 300, 300, true))
                            {
                                btnUploadProof.BackgroundImage = new Bitmap(pdfImage);
                                btnUploadProof.BackgroundImageLayout = ImageLayout.Zoom;
                            }
                        }
                    }
                    else
                    {
                        btnUploadProof.BackgroundImage = Properties.Resources.default_file_2;
                    }

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
                    lblFileName.Text = $"✅ File {fileName} uploaded successfully!";
                    lblFileName.ForeColor = Color.Green;
                    //btnUploadProof.Text = fileName;
                }
            };
            timer.Start();
        }

        private void siticonePanel1_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                                                //Color.FromArgb(231, 229, 251),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
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

        private void pictureBoxUserImage_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBoxUserImage_Click(object sender, EventArgs e)
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

        private void pictureBoxUserImage_Click_1(object sender, EventArgs e)
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

        private void siticonePanel2_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                                                //Color.FromArgb(231, 229, 251),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void siticonePanel3_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                //Color.FromArgb(231, 229, 251),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void CalculateLoanSummary()
        {
            if (decimal.TryParse(txtMonthlyIncome.Text, out decimal income) &&
                decimal.TryParse(txtMaxLoanAmount.Text.Replace("₱", "").Replace(",", ""), out decimal principal) &&
                cmbLoanTerm.SelectedItem != null &&
                cmbPaymentSchedule.SelectedItem != null)
            {
                string loanTermText = cmbLoanTerm.SelectedItem.ToString();
                string schedule = cmbPaymentSchedule.SelectedItem.ToString().ToLower();

                int months = loanTermText.Contains("6") ? 6 : 12;
                decimal interestRate = loanTermText.Contains("Short Term") ? 0.05m : 0.10m;
                string rateType = loanTermText.Contains("Short Term") ? "monthly" : "annually";

                // Calculate total interest depending on term type
                decimal totalInterest;
                if (rateType == "monthly")
                {
                    totalInterest = principal * interestRate * months; // 5% per month
                }
                else // annually
                {
                    totalInterest = principal * interestRate * (months / 12m); // 10% per year
                }

                decimal totalPayable = principal + totalInterest;

                int payments = months;
                if (schedule == "weekly") payments = months * 4;
                else if (schedule == "daily") payments = months * 30;

                decimal dues = payments > 0 ? totalPayable / payments : 0;

                lblAmountToReceive.Text = $"₱{principal:N2}";
                lblInterestRate.Text = $"{(int)(interestRate * 100)}% ({rateType})";
                lblMonthlyDues.Text = $"₱{dues:N2}";
            }
            else
            {
                lblAmountToReceive.Text = "₱0.00";
                lblInterestRate.Text = "0%";
                lblMonthlyDues.Text = "₱0.00";
            }
        }

        private void cmbLoanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateLoanSummary();
        }

        private void cmbLoanTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateLoanSummary();
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

            CalculateLoanSummary();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void siticonePanel4_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                //Color.FromArgb(231, 229, 251),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtFirstName.SelectionStart; 
            txtFirstName.Text = txtFirstName.Text.ToUpper();
            txtFirstName.SelectionStart = selStart; 
        }

        private void txtFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("First Name should not contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
            }
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Last Name should not contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus();
            }
        }

        private bool isFormatting = false;

        private void txtPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            if (isFormatting) return;
            isFormatting = true;

            Siticone.Desktop.UI.WinForms.SiticoneTextBox textBox = sender as Siticone.Desktop.UI.WinForms.SiticoneTextBox;

            if (textBox != null)
            {
                int cursorPosition = textBox.SelectionStart;

                if (!textBox.Text.StartsWith("+63 "))
                {
                    textBox.Text = "+63 ";
                    textBox.SelectionStart = textBox.Text.Length;
                    isFormatting = false;
                    return;
                }

                string digits = new string(textBox.Text.Substring(4).Where(char.IsDigit).ToArray());

                if (digits.Length > 10)
                    digits = digits.Substring(0, 10);

                string formatted = "";
                for (int i = 0; i < digits.Length; i++)
                {
                    formatted += digits[i];
                    if ((i == 2 || i == 5) && i != digits.Length - 1)
                        formatted += " ";
                }

                string newText = "+63 " + formatted;

                int numSpacesBefore = textBox.Text.Take(cursorPosition).Count(c => c == ' ');
                int newCursorPosition = cursorPosition;

                textBox.Text = newText;

                int numSpacesAfter = textBox.Text.Take(newCursorPosition).Count(c => c == ' ');

                int spaceDiff = numSpacesAfter - numSpacesBefore;
                newCursorPosition += spaceDiff;

                if (newCursorPosition < 4) newCursorPosition = 4;

                textBox.SelectionStart = newCursorPosition;
            }

            isFormatting = false;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal
                return builder.ToString();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                panelValidation.Visible = false;
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            panelValidation.Visible = false;
        }

        private void txtMiddleName_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtMiddleName.SelectionStart;
            txtMiddleName.Text = txtMiddleName.Text.ToUpper();
            txtMiddleName.SelectionStart = selStart;
        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtLastName.SelectionStart;
            txtLastName.Text = txtLastName.Text.ToUpper();
            txtLastName.SelectionStart = selStart;
        }

        private void txtNameSuffix_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtNameSuffix.SelectionStart;
            txtNameSuffix.Text = txtNameSuffix.Text.ToUpper();
            txtNameSuffix.SelectionStart = selStart;
        }

        private void txtMiddleName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Middle Name should not contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMiddleName.Focus();
            }
        }

        private void txtNameSuffix_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Suffix should not contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNameSuffix.Focus();
            }
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            int selStart = txtAddress.SelectionStart;
            txtAddress.Text = txtAddress.Text.ToUpper();
            txtAddress.SelectionStart = selStart;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            int selStart = txtUsername.SelectionStart;
            txtUsername.Text = txtUsername.Text.ToUpper();
            txtUsername.SelectionStart = selStart;
        }

        private void panelValidation_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblDues_Click(object sender, EventArgs e)
        {

        }

        private void lblInterest_Click(object sender, EventArgs e)
        {

        }
    }
}
