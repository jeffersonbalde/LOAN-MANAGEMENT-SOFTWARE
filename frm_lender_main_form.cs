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
    public partial class frm_lender_main_form : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        private Image animatedGif;
        private Button currentButton;

        public frm_lender_main_form()
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
                    currentButton.BackColor = Color.FromArgb(109, 207, 246);

                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panel1.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    //previousBtn.BackColor = Color.FromArgb(239, 251, 255);
                    previousBtn.BackColor = Color.FromArgb(225, 188, 255);
                }
            }
        }

        private void frm_lender_main_form_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            LoadBusinessLogo();

            animatedGif = Properties.Resources.under_maintenance2;
            pictureBox1.Image = animatedGif;

            ImageAnimator.Animate(animatedGif, OnFrameChanged);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {

            if (!this.IsDisposed && !pictureBox1.IsDisposed)
            {
                pictureBox1.Invalidate();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = "Today is " + DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
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

        private void label11_Click(object sender, EventArgs e)
        {
            frm_check_database_path frm = new frm_check_database_path();

            try
            {
                string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString();
                if (!string.IsNullOrEmpty(dataDirectory))
                {
                    string dbPath = System.IO.Path.Combine(dataDirectory, "LOAN_DB.mdf");

                    if (System.IO.File.Exists(dbPath))
                    {
                        frm.txtDBPath.Text = dbPath;
                        frm.ShowDialog();
                        return;
                        //MessageBox.Show("Your database file is located at:\n" + dbPath,
                        //                "Database Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Database file (.mdf) not found at:\n" + dbPath,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    frm.txtDBPath.Text = "DataDirectory is not set. Unable to locate the database file.";
                    frm.ShowDialog();
                    return;
                    //MessageBox.Show("DataDirectory is not set. Unable to locate the database file.",
                    //                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving database path:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            frm_owner_management frm = new frm_owner_management();
            frm.ShowDialog();
        }

        public void LoadOwnerName()
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
                    lblUser.Text = "Owner: " + dr["proprietor"].ToString();

                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frm_lender_main_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (animatedGif != null)
            {
                ImageAnimator.StopAnimate(animatedGif, OnFrameChanged);
                animatedGif.Dispose();
                animatedGif = null;
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
            main_panel.Controls.Clear();
            frm_lender_view_loan_request frm = new frm_lender_view_loan_request();
            frm.lblTitle.Text = "BORROWER LOAN APPLICATION";
            frm.TopLevel = false;
            main_panel.Controls.Add(frm);
            frm.BringToFront();
            frm.Show();
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
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

        private void btnDownPayment_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            ActivateButton(sender);
        }

        private void btnExpense_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            frm_landing_page frm = new frm_landing_page();
            frm.Show();
            this.Dispose();
        }
    }
}
