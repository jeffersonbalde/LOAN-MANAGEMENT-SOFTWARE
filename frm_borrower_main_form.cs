using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_main_form : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        private Image animatedGif;
        private Button currentButton;

        public frm_borrower_main_form()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x200; // CS_NOCLOSE (Disables Close Button)
                return cp;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = "Today is " + DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
        }

        private void frm_borrower_main_form_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; 
            timer1.Tick += timer1_Tick;
            timer1.Start();

            LoadBusinessLogo();
            LoadBusinessName();
            LoadBorrowerProfile();

            animatedGif = Properties.Resources.under_maintenance2;
            pictureBox1.Image = animatedGif;

            ImageAnimator.Animate(animatedGif, OnFrameChanged);

            GetDashboard();
        }

        public void GetDashboard()
        {
            ActivateButton(btnDashboard);
            main_panel.Controls.Clear();
            frm_borrower_dashboard frm = new frm_borrower_dashboard();
            frm.lblTitle.Text = "Dashboard";
            frm.TopLevel = false;
            main_panel.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {

            if (!this.IsDisposed && !pictureBox1.IsDisposed)
            {
                pictureBox1.Invalidate();
            }

        }

        public void LoadBorrowerName()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblBorrowerProfile WHERE id = @id";
                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@id", txtID.Text);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    lblUser.Text = dr["first_name"].ToString() + " " + dr["last_name"].ToString();

                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadBorrowerProfile()
        {
            try
            {
                cn.Open();
                string query = "SELECT borrower_profile FROM tblBorrowerProfile WHERE id = @id";

                using (SqlCommand cm = new SqlCommand(query, cn))
                {
                    cm.Parameters.AddWithValue("@id", txtID.Text);

                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["borrower_profile"] != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])dr["borrower_profile"];
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    if (pictureBoxProfile.BackgroundImage != null)
                                    {
                                        pictureBoxProfile.BackgroundImage.Dispose(); 
                                    }

                                    pictureBoxProfile.BackgroundImage = new Bitmap(Image.FromStream(ms));
                                }
                                pictureBoxProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBoxProfile.Refresh(); 
                            }
                            else
                            {
                                pictureBoxProfile.BackgroundImage = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading borrower profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cn.Close();
            }
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

        public void LoadBusinessName()
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
                    lblBusinessName.Text = dr["business_name"].ToString();

                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_landing_page frm = new frm_landing_page();
            frm.Show();
            this.Dispose();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Process.Start("calc.exe");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            try
            {

                frm_borrower_management frm = new frm_borrower_management(this);

                cn.Open();
                string query = "SELECT * FROM tblBorrowerProfile WHERE id LIKE '" + txtID.Text + "%'";
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
                                frm.pictureBoxUserImage.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        frm.pictureBoxUserImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        frm.pictureBoxUserImage.BackgroundImage = null; 
                    }

                    frm.txtID.Text = txtID.Text;
                    frm.txtFirstName.Text = dr["first_name"].ToString();
                    frm.txtLastName.Text = dr["last_name"].ToString();
                    frm.txtEmail.Text = dr["email_address"].ToString();
                    frm.txtPassword.Text = dr["password"].ToString();
                    frm.txtPhoneNumber.Text = dr["phone_number"].ToString();
                    frm.txtAddress.Text = dr["address"].ToString();
                    frm.txtZipCode.Text = dr["zip_code"].ToString();
                    frm.txtMonthlyIncome.Text = dr["monthly_income"].ToString();

                    string loan_term = dr["loan_term"].ToString();
                    string loan_type = dr["loan_type"].ToString();
                    string payment_schedule = dr["loan_type"].ToString();

                    frm.Tag = new Dictionary<string, string>
                            {
                                { "loan_term", loan_term },
                                { "loan_type", loan_type },
                                { "payment_schedule", payment_schedule }
                            };
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

        private void frm_borrower_main_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (animatedGif != null)
            {
                ImageAnimator.StopAnimate(animatedGif, OnFrameChanged);
                animatedGif.Dispose();
                animatedGif = null;
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        public void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    string colorr = "#cdfec2";
                    Color color = ColorTranslator.FromHtml(colorr);
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = Color.FromArgb(137, 85, 229);
                    //currentButton.BackColor = Color.FromArgb(250, 187, 255);

                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panel1.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    //previousBtn.BackColor = Color.FromArgb(250, 187, 255);
                    previousBtn.BackColor = Color.FromArgb(219, 231, 237);
                }
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            main_panel.Controls.Clear();
            frm_borrower_dashboard frm = new frm_borrower_dashboard();
            frm.lblTitle.Text = "Dashboard";
            frm.TopLevel = false;
            main_panel.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            main_panel.Controls.Clear();
            frm_request_loan frm = new frm_request_loan();
            frm.txtID.Text = txtID.Text;
            frm.lblTitle.Text = "Request Loan";
            frm.TopLevel = false;
            main_panel.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            //main_panel.Controls.Clear();
            //frm_payment_scheduling frm = new frm_payment_scheduling();
            //frm.lblTitle.Text = "Payment Scheduling";
            //frm.TopLevel = false;
            //main_panel.Controls.Add(frm);
            //frm.BringToFront();
            //frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnCustomerInfo_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnExpense_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnPersonalExpense_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }
    }
}
