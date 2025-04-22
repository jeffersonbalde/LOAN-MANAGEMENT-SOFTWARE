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
    public partial class frm_lender_approved_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_lender_approved_payment()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
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

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {

        }

        private void frm_lender_approved_payment_Load(object sender, EventArgs e)
        {
            dtFrom.ShowCheckBox = true;
            dtTo.ShowCheckBox = true;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            this.ActiveControl = txtSearchBorrower;

            cmbRequestStatus.Items.Clear();
            cmbRequestStatus.Items.Add("All");
            cmbRequestStatus.Items.Add("Pending");
            cmbRequestStatus.Items.Add("Approved");
            cmbRequestStatus.Items.Add("Rejected");
            cmbRequestStatus.Items.Add("Cancelled");
            cmbRequestStatus.SelectedIndex = 0;

            cmbMOP.Items.Clear();
            cmbMOP.Items.Add("All");
            cmbMOP.Items.Add("Cash");
            cmbMOP.Items.Add("Cheque");
            cmbMOP.Items.Add("Bank Transfer");
            cmbMOP.Items.Add("GCash");
            cmbMOP.Items.Add("Others");
            cmbMOP.SelectedIndex = 0;

            cmbLoanStatus.Items.Clear();
            cmbLoanStatus.Items.Add("All");
            cmbLoanStatus.Items.Add("Pending");
            cmbLoanStatus.Items.Add("Rejected");
            cmbLoanStatus.Items.Add("Cancelled");
            cmbLoanStatus.Items.Add("Ongoing");
            cmbLoanStatus.Items.Add("Completed");
            cmbLoanStatus.SelectedIndex = 0;

            LoadPayment();
            //GetTotalLoanRequest();
        }

        public void LoadPayment()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();

                string query = @"
            SELECT * FROM tblLoanPayment WHERE 1 = 1";

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    query += " AND name LIKE @name";
                }

                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    query += " AND status LIKE @status";
                }

                if (cmbLoanStatus.SelectedIndex != -1 && cmbLoanStatus.Text != "All")
                {
                    query += " AND loan_status LIKE @loan_status";
                }

                if (cmbMOP.SelectedIndex != -1 && cmbMOP.Text != "All")
                {
                    query += " AND mode_of_payment LIKE @mode_of_payment";
                }

                if (dtFrom.Checked && dtTo.Checked)
                {
                    query += " AND payment_date >= @start_date AND payment_date < @end_date";
                }
                else if (dtFrom.Checked)
                {
                    query += " AND payment_date >= @start_date";
                }
                else if (dtTo.Checked)
                {
                    query += " AND payment_date < @end_date";
                }

                query += " ORDER BY payment_date DESC";

                using (SqlConnection connection = new SqlConnection(dbcon.MyConnection()))
                {
                    connection.Open();
                    using (SqlCommand cm = new SqlCommand(query, connection))
                    {

                        if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                        {
                            cm.Parameters.AddWithValue("@name", txtSearchBorrower.Text + "%");
                        }

                        if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@status", cmbRequestStatus.SelectedItem.ToString() + "%");
                        }

                        if (cmbLoanStatus.SelectedIndex != -1 && cmbLoanStatus.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@loan_status", cmbLoanStatus.SelectedItem.ToString() + "%");
                        }

                        if (cmbMOP.SelectedIndex != -1 && cmbMOP.Text != "All")
                        {
                            cm.Parameters.AddWithValue("@mode_of_payment", cmbMOP.SelectedItem.ToString() + "%");
                        }

                        if (dtFrom.Checked)
                        {
                            cm.Parameters.AddWithValue("@start_date", dtFrom.Value.Date);
                        }
                        if (dtTo.Checked)
                        {
                            cm.Parameters.AddWithValue("@end_date", dtTo.Value.Date.AddDays(1));
                        }

                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                i++;

                                //string monthly_income = dr["monthly_income"] != DBNull.Value
                                //    ? "₱" + Double.Parse(dr["monthly_income"].ToString()).ToString("#,##0.00")
                                //    : "₱0.00";

                                //string maximum_loan = dr["maximum_loan"] != DBNull.Value
                                //    ? "₱" + Double.Parse(dr["maximum_loan"].ToString()).ToString("#,##0.00")
                                //    : "₱0.00";

                                //string requested_loan = dr["requested_loan"] != DBNull.Value
                                //    ? "₱" + Double.Parse(dr["requested_loan"].ToString()).ToString("#,##0.00")
                                //    : "₱0.00";

                                //string payment_date = dr["payment_date"] != DBNull.Value
                                //    ? DateTime.Parse(dr["payment_date"].ToString()).ToString("MMMM d, yyyy")
                                //    : "N/A";

                                //int rowIndex = dataGridView1.Rows.Add(
                                //    i,
                                //    null,
                                //    null,
                                //    null,
                                //    null,
                                //    dr["borrower_profile"],
                                //    payment_date,
                                //    dr["status"].ToString(),
                                //    dr["name"].ToString(),
                                //    requested_loan,
                                //    monthly_income,
                                //    dr["loan_term"].ToString(),
                                //    dr["payment_schedule"].ToString(),
                                //    dr["id"].ToString(),
                                //    dr["borrower_id"].ToString(),
                                //    dr["interest_rate"].ToString(),
                                //    dr["request_number"].ToString()
                                //);

                                string paid_amount = dr["paid_amount"] != DBNull.Value
                                ? "₱" + Double.Parse(dr["paid_amount"].ToString()).ToString("#,##0.00")
                                : "₱0.00";

                                string date_reviewed = dr["date_reviewed"] != DBNull.Value
                                    ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                                    : "Pending";

                                string payment_date = dr["payment_date"] != DBNull.Value
                                    ? DateTime.Parse(dr["payment_date"].ToString()).ToString("MMMM d, yyyy")
                                    : "N/A";

                                int rowIndex = dataGridView1.Rows.Add(
                                    i,
                                    null,
                                    null,
                                    null,
                                    null,
                                    dr["borrower_profile"],
                                    dr["payment_number"].ToString(),
                                    payment_date,
                                    date_reviewed,
                                    paid_amount,
                                    dr["status"].ToString(),
                                    dr["loan_status"].ToString(),
                                    dr["mode_of_payment"].ToString(),
                                    dr["id"].ToString(),
                                    dr["borrower_id"].ToString()
                                );

                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["approve"].Value = Properties.Resources.approve;
                                row.Cells["reject"].Value = Properties.Resources.reject;
                                row.Cells["view_details"].Value = Properties.Resources.view_details;
                                row.Cells["borrower_information"].Value = Properties.Resources.borrower_information;

                                string requestStatus = dr["status"].ToString();
                                string loanStatus = dr["loan_status"].ToString();

                                Font boldFont = new Font(dataGridView1.Font, FontStyle.Bold);

                                if (requestStatus == "Rejected" || requestStatus == "Cancelled")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["approval_status"].Style.ForeColor = Color.Red;
                                }
                                else if (requestStatus == "Approved")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["approval_status"].Style.ForeColor = Color.Green;
                                }
                                else if (requestStatus == "Pending")
                                {
                                    row.Cells["approval_status"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["approval_status"].Style.Font = boldFont;

                                if (loanStatus == "Rejected" || loanStatus == "Cancelled")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Red;
                                }
                                else if (loanStatus == "Completed")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Green;
                                }
                                else if (loanStatus == "Ongoing" || loanStatus == "Pending")
                                {
                                    row.Cells["loan_status"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["loan_status"].Style.Font = boldFont;
                            }
                        }
                    }
                }

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["borrower_profile"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
