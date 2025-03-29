using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_landing_page : Form
    {


        private Image animatedGif;
        private bool isPasswordVisible = false;


        public frm_landing_page()
        {
            InitializeComponent();
        }

        private void CenterPanel()
        {
            int centerX = (this.ClientSize.Width - panel1.Width) / 2;
            int centerY = (this.ClientSize.Height - panel1.Height) / 2;

            panel1.Location = new Point(centerX, centerY);
        }

        private void frm_dump_Load(object sender, EventArgs e)
        {
            CenterPanel();

            animatedGif = Properties.Resources.cover_gif2;
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

        private void frm_dump_Resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void frm_landing_page_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (animatedGif != null)
            {
                ImageAnimator.StopAnimate(animatedGif, OnFrameChanged);
                animatedGif.Dispose();
                animatedGif = null;
            }
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_create_account register_form = new frm_create_account();
            register_form.Show();
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
    }
}
