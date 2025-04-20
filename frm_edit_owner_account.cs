using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_edit_owner_account : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_owner_management frm;

        public frm_edit_owner_account(frm_owner_management frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void frm_edit_owner_account_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtPassword;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                {
                    MessageBox.Show("All fields are required.\n\n" +
                                    "Please complete the form before saving.",
                                    "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                cn.Open();
                string query = "SELECT password FROM tblOwnerAccount WHERE id = @id";
                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@id", txtID.Text);
                object result = cm.ExecuteScalar();
                cn.Close();

                if (result == null || result.ToString() != txtPassword.Text)
                {
                    MessageBox.Show("The old password you entered is incorrect.\n\n" +
                                    "Please try again.",
                                    "Incorrect Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Focus();
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("New password and confirmation do not match.\n\n" +
                                    "Please re-enter your new password correctly.",
                                    "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewPassword.Focus();
                    return;
                }


                if (MessageBox.Show("Do you want to confirm updating this owner account?",
                                    "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    cn.Open();
                    string updateQuery = "UPDATE tblOwnerAccount SET password = @password WHERE id = @id";
                    cm = new SqlCommand(updateQuery, cn);
                    cm.Parameters.AddWithValue("@password", txtNewPassword.Text);
                    cm.Parameters.AddWithValue("@id", txtID.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("Password has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void panel3_Paint(object sender, PaintEventArgs e)
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

        private void panel1_Paint(object sender, PaintEventArgs e)
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
    }
}
