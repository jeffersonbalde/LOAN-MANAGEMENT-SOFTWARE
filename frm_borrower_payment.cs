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
    public partial class frm_borrower_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_borrower_payment()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(237, 248, 100),  // Top color
                Color.FromArgb(131, 195, 79),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(237, 248, 100),  // Top color
                Color.FromArgb(131, 195, 79),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string borrowerId = txtID.Text;

            try
            {
                //if (cn.State != ConnectionState.Open)
                //    cn.Open();

                //string query = @"
                //SELECT TOP 1 status, loan_status 
                //FROM tblLoanRequests 
                //WHERE borrower_id = @borrowerId 
                //ORDER BY id DESC";

                //SqlCommand cmd = new SqlCommand(query, cn);
                //cmd.Parameters.AddWithValue("@borrowerId", borrowerId);

                //SqlDataReader reader = cmd.ExecuteReader();

                //if (reader.Read())
                //{
                //    string status = reader["status"].ToString();
                //    string loanStatus = reader["loan_status"].ToString();

                //    if (status == "Pending")
                //    {
                //        MessageBox.Show("❌ You already have a pending loan request.", "Loan Request Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }
                //    else if (status == "Approved" && loanStatus != "Paid")
                //    {
                //        MessageBox.Show("❌ You already have an approved loan. Please complete payment first before requesting again.", "Loan Request Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }
                //}


                //reader.Close();

                frm_borrower_add_payment frm = new frm_borrower_add_payment(this);
                frm.txtID.Text = borrowerId;
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //finally
            //{
            //    if (cn.State == ConnectionState.Open)
            //        cn.Close();
            //}
        }
    }
}
