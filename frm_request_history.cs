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
    public partial class frm_request_history : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_request_history()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        private void frm_request_history_Load(object sender, EventArgs e)
        {
            dtFrom.ShowCheckBox = true;
            dtTo.ShowCheckBox = true;

            dtFrom.Checked = false;
            dtTo.Checked = false;

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

        public void LoadRequest()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();

                string query = @"
            SELECT * FROM tblLoanRequests WHERE borrower_id = @borrower_id";

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

                query += " ORDER BY date_requested";

                using (SqlConnection connection = new SqlConnection(dbcon.MyConnection()))
                {
                    connection.Open();
                    using (SqlCommand cm = new SqlCommand(query, connection))
                    {
                        cm.Parameters.AddWithValue("@borrower_id", txtID.Text);

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

                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                i++;

                                string requested_loan = dr["requested_loan"] != DBNull.Value
                                    ? "₱" + Double.Parse(dr["requested_loan"].ToString()).ToString("#,##0.00")
                                    : "₱0.00";

                                string date_requested = dr["date_requested"] != DBNull.Value
                                    ? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                                    : "N/A";

                                string date_reviewed = dr["date_reviewed"] != DBNull.Value
                                    ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                                    : "N/A";

                                int rowIndex = dataGridView1.Rows.Add(
                                    i,
                                    null,
                                    null,
                                    dr["request_number"].ToString(),
                                    date_requested,
                                    date_reviewed,
                                    requested_loan,
                                    dr["status"].ToString(),
                                    dr["loan_status"].ToString(),
                                    dr["loan_type"].ToString(),
                                    dr["loan_term"].ToString(),
                                    dr["id"].ToString(),
                                    dr["borrower_id"].ToString()
                                );

                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["view_request"].Value = Properties.Resources.view_request;
                                row.Cells["cancel_request"].Value = Properties.Resources.cancel_request;



                                string requestStatus = dr["status"].ToString();
                                string loanStatus = dr["loan_status"].ToString();

                                Font boldFont = new Font(dataGridView1.Font, FontStyle.Bold);

                                if (requestStatus == "Rejected" || requestStatus == "Cancelled")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["request_status2"].Style.ForeColor = Color.Red;
                                }
                                else if (requestStatus == "Approved")
                                {
                                    //row.Cells["request_status2"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["request_status2"].Style.ForeColor = Color.Green;
                                }
                                else if (requestStatus == "Pending")
                                {
                                    row.Cells["request_status2"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["request_status2"].Style.Font = boldFont;

                                if (loanStatus == "Rejected" || loanStatus == "Cancelled")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Red;
                                }
                                else if (loanStatus == "Ongoing" || loanStatus == "Paid")
                                {
                                    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                                    row.Cells["loan_status"].Style.ForeColor = Color.Green;
                                }
                                else if (loanStatus == "Pending")
                                {
                                    row.Cells["loan_status"].Style.ForeColor = Color.Orange;
                                }
                                row.Cells["loan_status"].Style.Font = boldFont;

                            }
                        }
                    }
                }

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string borrowerId = txtID.Text;

            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                string query = @"
            SELECT TOP 1 status, loan_status 
            FROM tblLoanRequests 
            WHERE borrower_id = @borrowerId 
            ORDER BY id DESC";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@borrowerId", borrowerId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string status = reader["status"].ToString();
                    string loanStatus = reader["loan_status"].ToString();

                    if (status == "Pending")
                    {
                        MessageBox.Show("❌ You already have a pending loan request.", "Loan Request Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (status == "Approved" && loanStatus != "Paid")
                    {
                        MessageBox.Show("❌ You already have an approved loan. Please complete payment first before requesting again.", "Loan Request Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }


                reader.Close();

                frm_borrower_request_loan frm = new frm_borrower_request_loan(this);
                frm.txtID.Text = borrowerId;
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "view_request")
            {
                try
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                    frm_view_request_datails frm = new frm_view_request_datails();

                    string request_id = dataGridView1.Rows[selectedRowIndex].Cells["request_id"].Value.ToString();


                    cn.Open();
                    string query = "SELECT * FROM tblLoanRequests WHERE id LIKE '" + request_id + "%'";
                    cm = new SqlCommand(query, cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        frm.txtName.Text = dr["name"].ToString();
                        frm.txtAddress.Text = dr["address"].ToString();
                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();

                        frm.txtMonthlyIncome.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_income"]));
                        frm.txtMaxLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["maximum_loan"]));

                        frm.txtLoanAmount.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["requested_loan"]));

                        frm.txtLoanType.Text = dr["loan_type"].ToString();
                        frm.txtLoanTerm.Text = dr["loan_term"].ToString();
                        frm.txtPaymentSchedule.Text = dr["payment_schedule"].ToString();

                        frm.txtReasonForLoan.Text = dr["loan_purpose"].ToString();

                        frm.txtRequestNumber.Text = dr["request_number"].ToString();

                        string date_requested = dr["date_requested"] != DBNull.Value
                        ? DateTime.Parse(dr["date_requested"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtRequestDate.Text = date_requested;

                        frm.txtRequestStatus.Text = dr["status"].ToString();

                        string date_reviewed = dr["date_reviewed"] != DBNull.Value
                        ? DateTime.Parse(dr["date_reviewed"].ToString()).ToString("MMMM d, yyyy")
                        : "Pending";

                        frm.txtDateReviewed.Text = date_reviewed;

                        frm.txtRejectionReason.Text = dr["rejection_reason"] != DBNull.Value ? dr["rejection_reason"].ToString() : "N/A";

                        frm.lblAmountToReceive.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["amount_to_receive"]));

                        decimal interestRate = dr["interest_rate"] != DBNull.Value ? Convert.ToDecimal(dr["interest_rate"]) : 0;
                        frm.lblInterestRate.Text = $"{(int)(interestRate * 100)}%";

                        frm.lblMonthlyDues.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_dues"]));

                        frm.txtCancelReason.Text = dr["cancel_reason"] != DBNull.Value ? dr["cancel_reason"].ToString() : "N/A";

                        string date_cancelled = dr["date_cancelled"] != DBNull.Value
                        ? DateTime.Parse(dr["date_cancelled"].ToString()).ToString("MMMM d, yyyy")
                        : "N/A";

                        frm.txtDateCancelled.Text = date_cancelled;

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

            if (colName == "cancel_request")
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Please select a request to cancel.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

                    string request_id = selectedRow.Cells["request_id"].Value.ToString();
                    string status = selectedRow.Cells["request_status2"].Value?.ToString();

                    if (status == "Rejected")
                    {
                        MessageBox.Show("This request has already been rejected and cannot be cancelled.",
                                        "Request Already Rejected",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    if (status == "Approved")
                    {
                        MessageBox.Show("This request has already been approved and cannot be cancelled.",
                                        "Request Already Approved",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (status == "Cancelled")
                    {
                        MessageBox.Show("This request has already been cancelled.",
                                        "Request Already Cancelled",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    frm_cancel_reason frm = new frm_cancel_reason(this);
                    frm.txtRequestID.Text = request_id;
                    frm.ShowDialog();
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("An error occurred while attempting to cancel the request:\n\n" + ex.Message,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
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

        private void button7_Click(object sender, EventArgs e)
        {
            LoadRequest();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;

            dtFrom.Checked = false;
            dtTo.Checked = false;

            cmbRequestStatus.SelectedIndex = -1;
            cmbLoanType.SelectedIndex = -1;
            cmbLoanTerm.SelectedIndex = -1;

            LoadRequest();
        }
    }
}
