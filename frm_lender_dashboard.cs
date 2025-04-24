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
    public partial class frm_lender_dashboard : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_lender_dashboard()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        public void LoadTotalAmountLent()
        {
            try
            {
                cn.Open();
                string query = @"
        SELECT SUM(requested_loan) AS total_lent
        FROM tblLoanRequests
        WHERE status = 'Approved'";

                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();

                if (dr.Read() && dr["total_lent"] != DBNull.Value)
                {
                    decimal totalLent = Convert.ToDecimal(dr["total_lent"]);
                    lblTotalAmountLent.Text = totalLent.ToString("₱#,##0.00");
                }
                else
                {
                    lblTotalAmountLent.Text = "₱0.00";
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total lent amount: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        public void LoadTotalPaymentRecieved()
        {
            try
            {
                cn.Open();
                string query = @"
        SELECT SUM(paid_amount) AS total_lent
        FROM tblLoanPayment
        WHERE status = 'Approved'";

                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();

                if (dr.Read() && dr["total_lent"] != DBNull.Value)
                {
                    decimal totalLent = Convert.ToDecimal(dr["total_lent"]);
                    lblAmountPaid.Text = totalLent.ToString("₱#,##0.00");
                }
                else
                {
                    lblAmountPaid.Text = "₱0.00";
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total lent amount: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public void LoadOutstandingBalance()
        {
            try
            {
                cn.Open();
                string query = @"
        SELECT SUM(ongoing_balance) AS total_lent
        FROM tblBorrowerBalance";

                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();

                if (dr.Read() && dr["total_lent"] != DBNull.Value)
                {
                    decimal totalLent = Convert.ToDecimal(dr["total_lent"]);
                    lblBalance.Text = totalLent.ToString("₱#,##0.00");
                }
                else
                {
                    lblBalance.Text = "₱0.00";
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total lent amount: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        public void LoadActiveLoans()
        {
            try
            {
                cn.Open();
                string query = @"
                SELECT COUNT(*) AS total_approved_requests
                FROM tblLoanRequests
                WHERE loan_status = 'Ongoing'";

                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    lblActiveLoans.Text = dr["total_approved_requests"].ToString();
                }
                else
                {
                    lblActiveLoans.Text = "0";
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading total approved loan requests: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        //public void LoadBorrowerTotalApprovedAmount()
        //{
        //    try
        //    {
        //        cn.Open();
        //        string query = @"
        //    SELECT SUM(approved_amount) AS total_approved 
        //    FROM tblBorrowerLoanDetails 
        //    WHERE borrower_id = @borrower_id 
        //    GROUP BY borrower_id";

        //        cm = new SqlCommand(query, cn);
        //        cm.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());

        //        dr = cm.ExecuteReader();

        //        if (dr.Read())
        //        {
        //            lblTotalApprovedAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["total_approved"]));
        //        }
        //        else
        //        {
        //            lblTotalApprovedAmount.Text = "₱0.00";
        //        }

        //        dr.Close();
        //        cn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error loading approved amount: " + ex.Message);
        //        if (cn.State == ConnectionState.Open) cn.Close();
        //    }
        //}

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(196, 75, 128),  // Top color
                Color.FromArgb(103, 71, 219),  // Bottom color
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
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(196, 75, 128),  // Top color
                Color.FromArgb(103, 71, 219),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(135, 230, 133),  // Top color
                Color.FromArgb(101, 173, 139),  // Bottom color
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

                Color.FromArgb(220, 106, 44),  // Top color
                Color.FromArgb(206, 37, 104),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(196, 75, 128),  // Top color
                Color.FromArgb(103, 71, 219),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(135, 230, 133),  // Top color
                Color.FromArgb(101, 173, 139),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(220, 106, 44),  // Top color
                Color.FromArgb(206, 37, 104),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(196, 75, 128),  // Top color
                Color.FromArgb(103, 71, 219),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void frm_lender_dashboard_Load(object sender, EventArgs e)
        {
            LoadActiveLoans();
            LoadTotalAmountLent();
            LoadTotalPaymentRecieved();
            LoadOutstandingBalance();
        }
    }
}
