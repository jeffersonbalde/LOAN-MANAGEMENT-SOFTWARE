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
    public partial class frm_edit_business_owner_profile : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_owner_management frm;


        public frm_edit_business_owner_profile(frm_owner_management frm)
        {
            InitializeComponent();
            this.frm = frm;
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_edit_business_owner_profile_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtBusinessName;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBusinessName.Text) ||
                    string.IsNullOrWhiteSpace(txtBarangay.Text) ||
                    string.IsNullOrWhiteSpace(txtStreetName.Text) ||
                    string.IsNullOrWhiteSpace(txtContactNo.Text) ||
                    string.IsNullOrWhiteSpace(txtBusinessTIN.Text) ||
                    string.IsNullOrWhiteSpace(txtPro.Text)
                    )
                {
                    MessageBox.Show("All fields are required.\n\n" +
                        "Please complete the form before saving.",
                        "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (pictureBoxLogo.BackgroundImage == null)
                {
                    MessageBox.Show("No image has been uploaded.\n\n" +
                                    "Please upload a business logo or set the default logo before saving.",
                                    "Logo Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (MessageBox.Show("Do you want to confirm updating this business profile?", "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MemoryStream ms = new MemoryStream();
                    pictureBoxLogo.BackgroundImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arrImage = ms.GetBuffer();

                    cn.Open();
                    string query2 = "UPDATE tblBusinessProfile SET business_logo = @business_logo, business_name=@business_name, barangay=@barangay, street_name=@street_name, contact_no=@contact_no, business_tin=@business_tin, proprietor=@proprietor WHERE id LIKE @id";
                    cm = new SqlCommand(query2, cn);
                    cm.Parameters.AddWithValue("@business_logo", arrImage);
                    cm.Parameters.AddWithValue("@business_name", txtBusinessName.Text.Trim());
                    cm.Parameters.AddWithValue("@barangay", txtBarangay.Text.Trim());
                    cm.Parameters.AddWithValue("@street_name", txtStreetName.Text.Trim());
                    cm.Parameters.AddWithValue("@contact_no", txtContactNo.Text.Trim());
                    cm.Parameters.AddWithValue("@business_tin", txtBusinessTIN.Text.Trim());
                    cm.Parameters.AddWithValue("@proprietor", txtPro.Text.Trim());
                    cm.Parameters.AddWithValue("@id", txtID.Text.Trim());
                    cm.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("Business Profile has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frm.LoadBusinessProfile();

                    if (Application.OpenForms["frm_lender_main_form"] is frm_lender_main_form lenderForm)
                    {
                        lenderForm.LoadBusinessLogo();
                        lenderForm.LoadOwnerName();
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxLogo.BackgroundImage = Properties.Resources.loan_business_logo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting default image: " + ex.Message);
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = string.Empty; 
                openFileDialog1.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxLogo.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the image.\n\n" +
                                "Error Details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
