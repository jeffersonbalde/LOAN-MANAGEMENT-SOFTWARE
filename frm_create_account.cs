using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_create_account : Form
    {

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
        }

        private void frm_create_account_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void frm_create_account_Resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void siticoneTextBox5_KeyPress(object sender, KeyPressEventArgs e)
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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_create_account_2 register_form2 = new frm_create_account_2();
            register_form2.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void siticonePanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxUserImage.BackgroundImage = Properties.Resources.default_user1; // Replace 'DefaultImage' with your actual image name in Resources.
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
                openFileDialog1.FileName = string.Empty; // Clear any previously selected file
                openFileDialog1.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";

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
    }
}
