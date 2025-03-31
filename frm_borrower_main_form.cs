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
            frm_borrower_management frm = new frm_borrower_management();
            frm.ShowDialog();
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
    }
}
