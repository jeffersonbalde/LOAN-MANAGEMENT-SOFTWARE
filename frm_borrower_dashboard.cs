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
    public partial class frm_borrower_dashboard : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_borrower_dashboard()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        public void LoadTotalApprovedLoanRequests()
        {
            try
            {
                cn.Open();
                string query = @"
        SELECT COUNT(*) AS total_approved_requests
        FROM tblLoanRequests
        WHERE status = 'Approved' AND borrower_id = @borrower_id";

                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());

                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    lblTotalApprovedApplication.Text = dr["total_approved_requests"].ToString();
                }
                else
                {
                    lblTotalApprovedApplication.Text = "0";
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


        public void LoadBorrowerTotalApprovedAmount()
        {
            try
            {
                cn.Open();
                string query = @"
            SELECT SUM(approved_amount) AS total_approved 
            FROM tblBorrowerLoanDetails 
            WHERE borrower_id = @borrower_id 
            GROUP BY borrower_id";

                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());

                dr = cm.ExecuteReader();

                if (dr.Read()) 
                {
                    lblTotalApprovedAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["total_approved"]));
                }
                else
                {
                    lblTotalApprovedAmount.Text = "₱0.00"; 
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading approved amount: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public void LoadOngoingBalance()
        {
            try
            {
                cn.Open();
                string query = @"
            SELECT SUM(ongoing_balance) AS total_balance 
            FROM tblBorrowerBalance 
            WHERE borrower_id = @borrower_id 
            GROUP BY borrower_id";

                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());

                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    lblBalance.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["total_balance"]));
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
                MessageBox.Show("Error loading approved amount: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
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

        private void LoadPaymentSchedule()
        {
            try
            {
                cmbRequestNumber.Items.Clear();

                string query = @"
            SELECT request_number
            FROM (
                SELECT request_number, MAX(due_date) AS latest_due_date
                FROM tblPaymentSchedule
                WHERE borrower_id = @borrower_id
                GROUP BY request_number
            ) AS sub
            ORDER BY latest_due_date DESC";

                using (SqlConnection conn = new SqlConnection(dbcon.MyConnection()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@borrower_id", txtID.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbRequestNumber.Items.Add(reader["request_number"].ToString());
                            }
                        }
                    }

                    if (cmbRequestNumber.Items.Count > 0)
                    {
                        cmbRequestNumber.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading request numbers: " + ex.Message);
            }
        }



        private void LoadPaymentSchedule(string requestNumber)
        {
            try
            {
                dataGridViewSchedule.Rows.Clear();

                string query = @"SELECT due_date, amount_to_pay, interest, total_payment, status
                         FROM tblPaymentSchedule
                         WHERE borrower_id = @borrower_id AND request_number = @request_number
                         ORDER BY due_date ASC";

                using (SqlConnection conn = new SqlConnection(dbcon.MyConnection()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@borrower_id", txtID.Text);
                        cmd.Parameters.AddWithValue("@request_number", requestNumber);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (reader.Read())
                            {
                                i++;
                                int rowIndex = dataGridViewSchedule.Rows.Add(
                                    i,
                                    DateTime.Parse(reader["due_date"].ToString()).ToString("MMMM dd, yyyy"),
                                    Convert.ToDecimal(reader["amount_to_pay"]).ToString("₱#,##0.00"),
                                    Convert.ToDecimal(reader["interest"]).ToString("₱#,##0.00"),
                                    Convert.ToDecimal(reader["total_payment"]).ToString("₱#,##0.00"),
                                    reader["status"].ToString()
                                );

                                // Color coding the status
                                string status = reader["status"].ToString();
                                if (status == "Pending")
                                {
                                    dataGridViewSchedule.Rows[rowIndex].Cells[5].Style.ForeColor = Color.Orange;
                                }
                                else if (status == "Paid")
                                {
                                    dataGridViewSchedule.Rows[rowIndex].Cells[5].Style.ForeColor = Color.Green;
                                }
                            }
                        }
                    }
                }

                lblNoLowStocks.Visible = dataGridViewSchedule.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payment schedule: " + ex.Message);
            }
        }


        private void frm_borrower_dashboard_Load(object sender, EventArgs e)
        {
            LoadBorrowerTotalApprovedAmount();
            LoadTotalApprovedLoanRequests();
            LoadPaymentSchedule();
            LoadOngoingBalance();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(237, 248, 100),  // Top color
                Color.FromArgb(131, 195, 100),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }   

        private void cmbRequestNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRequestNumber.SelectedItem != null)
            {
                LoadPaymentSchedule(cmbRequestNumber.SelectedItem.ToString());
            }
        }

        private void dataGridViewSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
