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

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_owner_management : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_owner_management()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_owner_management_Load(object sender, EventArgs e)
        {
            LoadBusinessProfile();
            LoadOwnerAccount();
        }

        public void LoadBusinessProfile()
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

                    txtBusinessName.Text = dr["business_name"].ToString();
                    txtBarangay.Text = dr["barangay"].ToString();
                    txtStreetName.Text = dr["street_name"].ToString();
                    txtContactNo.Text = dr["contact_no"].ToString();
                    txtBusinessTIN.Text = dr["business_tin"].ToString();
                    txtPro.Text = dr["proprietor"].ToString();
                    txtID.Text = dr["id"].ToString();
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadOwnerAccount()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblOwnerAccount";
                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    if (dr["owner_image"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])dr["owner_image"]; 
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            using (Image tempImage = Image.FromStream(ms))
                            {
                                pictureBoxOwner.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        pictureBoxOwner.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        pictureBoxOwner.BackgroundImage = null;
                    }

                    txtEmail.Text = dr["username"].ToString();
                    txtPassword.Text = dr["password"].ToString();
                    txtIDOwner.Text = dr["id"].ToString();
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frm_edit_business_owner_profile frm = new frm_edit_business_owner_profile(this);

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
                                frm.pictureBoxLogo.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        frm.pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        frm.pictureBoxLogo.BackgroundImage = null; 
                    }

                    frm.txtBusinessName.Text = dr["business_name"].ToString();
                    frm.txtBarangay.Text = dr["barangay"].ToString();
                    frm.txtStreetName.Text = dr["street_name"].ToString();
                    frm.txtContactNo.Text = dr["contact_no"].ToString();
                    frm.txtBusinessTIN.Text = dr["business_tin"].ToString();
                    frm.txtPro.Text = dr["proprietor"].ToString();
                    frm.txtID.Text = dr["id"].ToString();
                }

                dr.Close();
                cn.Close();

                frm.txtBusinessName.Focus();
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frm_edit_owner_image frm = new frm_edit_owner_image(this);

            try
            {
                cn.Open();
                string query = "SELECT * FROM tblOwnerAccount";
                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    if (dr["owner_image"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])dr["owner_image"];
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            using (Image tempImage = Image.FromStream(ms))
                            {
                                frm.pictureBoxImage.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        frm.pictureBoxImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        frm.pictureBoxImage.BackgroundImage = null; 
                    }

                    frm.txtID.Text = dr["id"].ToString();
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

        private void button4_Click(object sender, EventArgs e)
        {
            frm_edit_owner_account frm = new frm_edit_owner_account(this);

            try
            {
                cn.Open();
                string query = "SELECT * FROM tblOwnerAccount";
                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    frm.txtID.Text = dr["id"].ToString();
                }

                dr.Close();
                cn.Close();

                frm.txtPassword.Focus();
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
