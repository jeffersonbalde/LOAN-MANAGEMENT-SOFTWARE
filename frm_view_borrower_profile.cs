using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters;

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

                // ✅ Fix: Ensure connection is closed before opening
                if (cn.State == ConnectionState.Open)
                    cn.Close();

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
                        null,
                        dr["borrower_profile"],
                        date_requested,
                        dr["status"].ToString(),
                        dr["name"].ToString(),
                        requested_loan,
                        monthly_income,
                        dr["loan_type"].ToString(),
                        dr["loan_term"].ToString(),
                        dr["id"].ToString(),
                        dr["borrower_id"].ToString()
                    );

                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["approve"].Value = Properties.Resources.approve;
                    row.Cells["reject"].Value = Properties.Resources.reject;
                    row.Cells["view_details"].Value = Properties.Resources.view_details;
                    row.Cells["borrower_information"].Value = Properties.Resources.borrower_information;


                    string requestStatus = dr["status"].ToString();
                    //string loanStatus = dr["loan_status"].ToString();

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

                    //if (loanStatus == "Rejected" || loanStatus == "Cancelled")
                    //{
                    //    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(122, 50, 0);
                    //    row.Cells["loan_status"].Style.ForeColor = Color.Red;
                    //}
                    //else if (loanStatus == "Ongoing" || loanStatus == "Paid")
                    //{
                    //    //row.Cells["loan_status"].Style.ForeColor = Color.FromArgb(0, 67, 50);
                    //    row.Cells["loan_status"].Style.ForeColor = Color.Green;
                    //}
                    //else if (loanStatus == "Pending")
                    //{
                    //    row.Cells["loan_status"].Style.ForeColor = Color.Orange;
                    //}
                    //row.Cells["loan_status"].Style.Font = boldFont;
                }

                dr.Close();
                cn.Close();

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "approve")
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                        return;

                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

                    string request_id = selectedRow.Cells["request_id"].Value.ToString();
                    string status = selectedRow.Cells["request_status2"].Value?.ToString();

                    if (status == "Approved")
                    {
                        MessageBox.Show("❌ This loan request has already been approved.",
                                        "Already Approved",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    if (status == "Rejected")
                    {
                        MessageBox.Show("❌ This loan request has already been rejected and cannot be approve.",
                                        "Cannot Approve Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (status == "Cancelled")
                    {
                        MessageBox.Show("❌ This loan request has already been cancelled and cannot be approve.",
                                        "Cannot Approve Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    DialogResult result = MessageBox.Show("Are you sure you want to approve this loan request?",
                                                          "Confirm Approval",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        string query = @"
                    UPDATE tblLoanRequests 
                    SET status = @status, 
                        date_reviewed = @date_reviewed, 
                        loan_status = @loan_status 
                    WHERE id LIKE @id";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@status", "Approved");
                            cmd.Parameters.AddWithValue("@date_reviewed", DateTime.Now);
                            cmd.Parameters.AddWithValue("@loan_status", "Ongoing");
                            cmd.Parameters.AddWithValue("@id", request_id);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                        }

                        MessageBox.Show("Loan request has been approved successfully.",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        LoadRequest();
                    }
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (colName == "reject")
            {
                try
                {
                    if (dataGridView1.SelectedRows.Count == 0)
                        return;

                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

                    string request_id = selectedRow.Cells["request_id"].Value.ToString();
                    string status = selectedRow.Cells["request_status2"].Value?.ToString();

                    if (status == "Rejected")
                    {
                        MessageBox.Show("❌ This loan request has already been rejected.",
                                        "Already Rejected",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    if (status == "Approved")
                    {
                        MessageBox.Show("❌ This loan request has already been approved and cannot be rejected.",
                                        "Cannot Reject Approved Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (status == "Cancelled")
                    {
                        MessageBox.Show("❌ This loan request has already been cancelled and cannot be reject.",
                                        "Cannot Reject Request",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    frm_rejected_reason frm = new frm_rejected_reason(this);
                    frm.txtRequestID.Text = request_id;
                    frm.ShowDialog();

                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();

                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (colName == "view_details")
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

            if (colName == "borrower_information")
            {
                try
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                    frm_view_borrower_informations frm = new frm_view_borrower_informations();

                    string request_id = dataGridView1.Rows[selectedRowIndex].Cells["request_id"].Value.ToString();
                    string borrower_id = dataGridView1.Rows[selectedRowIndex].Cells["borrower_id"].Value.ToString();


                    cn.Open();
                    string query = "SELECT * FROM tblBorrowerProfile WHERE id LIKE '" + borrower_id + "%'";
                    cm = new SqlCommand(query, cn);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {

                        if (dr["borrower_profile"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])dr["borrower_profile"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                using (Image tempImage = Image.FromStream(ms))
                                {
                                    frm.pictureBoxProfile.BackgroundImage = new Bitmap(tempImage);
                                }
                            }
                            frm.pictureBoxProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            frm.pictureBoxProfile.BackgroundImage = null; 
                        }


                        frm.txtFirstName.Text = dr["first_name"].ToString();
                        frm.txtLastName.Text = dr["last_name"].ToString();
                        frm.txtAddress.Text = dr["address"].ToString();

                        string zipCode = dr["zip_code"] != DBNull.Value ? dr["zip_code"].ToString().Trim() : "";
                        frm.txtZIPCode.Text = !string.IsNullOrEmpty(zipCode) ? zipCode : "N/A";

                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();


                        if (dr["proof_of_income"] != DBNull.Value && dr["proof_of_income_filename"] != DBNull.Value)
                        {
                            frm.ProofOfIncomeBytes = (byte[])dr["proof_of_income"];
                            frm.ProofOfIncomeFilename = dr["proof_of_income_filename"].ToString();
                            frm.btnUploadProof.Text = dr["proof_of_income_filename"].ToString();
                        }
                        else
                        {
                            frm.ProofOfIncomeBytes = null;
                            frm.ProofOfIncomeFilename = "";
                        }

                        frm.txtMonthlyIncome.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["monthly_income"]));
                        frm.txtMaximumLoan.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["maximum_loan"]));

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

        private void button7_Click(object sender, EventArgs e)
        {
            LoadRequest();
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
    }
}
