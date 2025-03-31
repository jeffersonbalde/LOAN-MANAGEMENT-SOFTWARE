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
    public partial class frm_edit_owner_image : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_owner_management frm;

        public frm_edit_owner_image(frm_owner_management frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = string.Empty; 
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                openFileDialog1.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxImage.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the image.\n\n" +
                                "Error Details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBoxImage.BackgroundImage = Properties.Resources.owner_imge_default;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting default image: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("Do you want to confirm updating this owner image?",
                                    "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    MemoryStream ms = new MemoryStream();
                    pictureBoxImage.BackgroundImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] arrImage = ms.GetBuffer();

                    cn.Open();
                    string updateQuery = "UPDATE tblOwnerAccount SET owner_image = @owner_image WHERE id = @id";
                    cm = new SqlCommand(updateQuery, cn);
                    cm.Parameters.AddWithValue("@owner_image", arrImage);
                    cm.Parameters.AddWithValue("@id", txtID.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("Owner Image has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frm.LoadOwnerAccount();

                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
