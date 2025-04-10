using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_lender_view_loan_request : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_lender_view_loan_request()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_lender_view_loan_request_Load(object sender, EventArgs e)
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

            cmbLoanType.Items.Clear();
            cmbLoanType.Items.Add("All");
            cmbLoanType.Items.Add("Housing Loan");
            cmbLoanType.Items.Add("Business Loan");
            cmbLoanType.Items.Add("Personal Loan");
            cmbLoanType.SelectedIndex = 0;

            cmbLoanTerm.Items.Clear();
            cmbLoanTerm.Items.Add("All");
            cmbLoanTerm.Items.Add("6 Months");
            cmbLoanTerm.Items.Add("12 Months");
            cmbLoanTerm.Items.Add("24 Months");
            cmbLoanTerm.Items.Add("36 Months");
            cmbLoanTerm.SelectedIndex = 0;

            LoadRequest();
            GetTotalLoanRequest();
        }

        public void LoadRequest()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();
                string query = @"
                    SELECT 
                        *
                    FROM tblLoanRequests
                    WHERE 1=1"; 

                cn.Open();


                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    query += " AND name LIKE @name";
                }

                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    query += " AND status LIKE @status";
                }

                if (cmbLoanType.SelectedIndex != -1 && cmbLoanType.Text != "All")
                {
                    query += " AND loan_type LIKE @loan_type";
                }


                if (cmbLoanTerm.SelectedIndex != -1 && cmbLoanTerm.Text != "All")
                {
                    query += " AND loan_term LIKE @loan_term";
                }

                if (dtFrom.Checked && dtTo.Checked)
                {
                    query += " AND date_requested >= @start_date AND date_requested < @end_date";
                }
                else if (dtFrom.Checked) 
                {
                    query += " AND date_requested >= @start_date";
                }
                else if (dtTo.Checked) 
                {
                    query += " AND date_requested < @end_date";
                }

                query += @"
                ORDER BY 
                    date_requested"; 

                cm = new SqlCommand(query, cn);

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    cm.Parameters.AddWithValue("@name", txtSearchBorrower.Text + "%");
                }


                if (cmbRequestStatus.SelectedIndex != -1 && cmbRequestStatus.Text != "All")
                {
                    cm.Parameters.AddWithValue("@status", cmbRequestStatus.SelectedItem.ToString() + "%");
                }

                if (cmbLoanType.SelectedIndex != -1 && cmbLoanType.Text != "All")
                {
                    cm.Parameters.AddWithValue("@loan_type", cmbLoanType.SelectedItem.ToString() + "%");
                }

                if (cmbLoanTerm.SelectedIndex != -1 && cmbLoanTerm.Text != "All")
                {
                    cm.Parameters.AddWithValue("@loan_term", cmbLoanTerm.SelectedItem.ToString() + "%");
                }

                if (dtFrom.Checked)
                {
                    cm.Parameters.AddWithValue("@start_date", dtFrom.Value.Date);
                }
                if (dtTo.Checked)
                {
                    cm.Parameters.AddWithValue("@end_date", dtTo.Value.Date.AddDays(1));
                }

                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;

                    string monthly_income = dr["monthly_income"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["monthly_income"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string maximum_loan = dr["maximum_loan"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["maximum_loan"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string requested_loan = dr["requested_loan"] != DBNull.Value
                        ? "₱" + Double.Parse(dr["requested_loan"].ToString()).ToString("#,##0.00")
                        : "₱0.00";

                    string date_requested = dr["date_requested"] != DBNull.Value
                        ? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                    int rowIndex = dataGridView1.Rows.Add(
                        i,
                        null,
                        null,
                        null,
                        dr["borrower_profile"], 
                        date_requested,
                        dr["status"].ToString(),
                        dr["name"].ToString(),
                        monthly_income,
                        requested_loan,
                        dr["loan_type"].ToString(),
                        dr["loan_term"].ToString()
                    );

                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["approve"].Value = Properties.Resources.approve;
                    row.Cells["reject"].Value = Properties.Resources.reject;
                    row.Cells["view_details"].Value = Properties.Resources.view_details;
                }

                dr.Close();
                cn.Close();

                if (dataGridView1.Rows.Count == 0)
                {
                    lblNoLowStocks.Visible = true;
                }
                else
                {
                    lblNoLowStocks.Visible = false;
                }

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["borrower_profile"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }

        }

        private void txtSearchBorrower_TextChanged(object sender, EventArgs e)
        {
            LoadRequest();
        }

        public void GetTotalLoanRequest()
        {

            try
            {
                int total_request = dataGridView1.Rows.Count;

                lblTotalTransactions.Text = total_request.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while counting total request: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtFrom_ValueChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void dtTo_ValueChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbRequestStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbLoanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }

        private void cmbLoanTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequest();
            GetTotalLoanRequest();
        }
    }
}
