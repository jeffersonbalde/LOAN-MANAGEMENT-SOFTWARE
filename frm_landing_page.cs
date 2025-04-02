using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_landing_page : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        private Image animatedGif;
        private bool isPasswordVisible = false;


        public frm_landing_page()
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

        private void frm_dump_Load(object sender, EventArgs e)
        {
            CenterPanel();

            LoadBusinessLogo();

            animatedGif = Properties.Resources.cover_gif2;
            pictureBox1.Image = animatedGif;

            ImageAnimator.Animate(animatedGif, OnFrameChanged);
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

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();
                bool userFound = false;
                bool isOwner = false;
                string user_name = "";
                byte[] userImage = null;
                string pro = "";
                int userId = 0;
                string first_name = "";
                string last_name = "";

                cn.Open();
                string pQuery = "SELECT proprietor FROM tblBusinessProfile;";
                cm = new SqlCommand(pQuery, cn);
                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    pro = dr["proprietor"].ToString();
                }
                dr.Close();
                cn.Close();

                cn.Open();
                string ownerQuery = "SELECT id, owner_image, username FROM tblOwnerAccount WHERE username = @email AND password = @password";
                cm = new SqlCommand(ownerQuery, cn);
                cm.Parameters.AddWithValue("@email", email);
                cm.Parameters.AddWithValue("@password", password);
                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    userFound = true;
                    isOwner = true;
                    user_name = pro;
                    if (dr["owner_image"] != DBNull.Value)
                    {
                        userImage = (byte[])dr["owner_image"];
                    }
                }
                dr.Close();
                cn.Close();


                if (!userFound)
                {
                    cn.Open();
                    string staffQuery = "SELECT * FROM tblBorrowerProfile WHERE email_address = @email AND password = @password";
                    cm = new SqlCommand(staffQuery, cn);
                    cm.Parameters.AddWithValue("@email", email);
                    cm.Parameters.AddWithValue("@password", password);
                    dr = cm.ExecuteReader();

                    if (dr.Read())
                    {
                        userFound = true;
                        userId = Convert.ToInt32(dr["id"]);
                        first_name = dr["first_name"].ToString();
                        last_name = dr["last_name"].ToString();
                        if (dr["borrower_profile"] != DBNull.Value)
                        {
                            userImage = (byte[])dr["borrower_profile"];
                        }
                    }
                    dr.Close();
                    cn.Close();
                }

                if (!userFound)
                {
                    MessageBox.Show("Invalid email or password. Please try again.",
                                    "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEmail.Focus();
                    return;
                }

                if (!isOwner)
                {
                    cn.Open();
                    string sessionQuery = "INSERT INTO tblUserSession (user_id, login_date, login_time) VALUES (@user_id, @login_date, GETDATE())";
                    cm = new SqlCommand(sessionQuery, cn);
                    cm.Parameters.AddWithValue("@user_id", userId);
                    cm.Parameters.AddWithValue("@login_date", DateTime.Now);
                    cm.ExecuteNonQuery();
                    cn.Close();
                }

                
                MessageBox.Show("You have successfully logged in.",  
                                "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);


                if (isOwner)
                {
                    frm_lender_main_form ownerForm = new frm_lender_main_form();

                    ownerForm.lblUser.Text = "Owner: " + user_name;

                    if (userImage != null)
                    {
                        using (MemoryStream ms = new MemoryStream(userImage))
                        {
                            ownerForm.pictureBoxProfile.Image = Image.FromStream(ms);
                        }
                    }

                    ownerForm.Show();
                }
                else
                {
                    frm_borrower_main_form borrowerForm = new frm_borrower_main_form();

                    borrowerForm.lblUser.Text = first_name + " " + last_name;

                    borrowerForm.txtID.Text = userId.ToString();

                    if (userImage != null)
                    {
                        using (MemoryStream ms = new MemoryStream(userImage))
                        {
                            borrowerForm.pictureBoxProfile.BackgroundImage = Image.FromStream(ms);
                        }
                    }

                    borrowerForm.Show();
                }

                this.Hide();
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
