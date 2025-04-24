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

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_user_management : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_user_management()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        public void LoadApplicant()
        {
            try
            {
                int i = 0;
                dataGridView1.Rows.Clear();

                string query = @"
            SELECT * FROM tblBorrowerProfile WHERE 1 = 1";

                if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                {
                    query += " AND first_name LIKE @name";
                }

                query += " ORDER BY date_registered DESC";

                using (SqlConnection connection = new SqlConnection(dbcon.MyConnection()))
                {
                    connection.Open();
                    using (SqlCommand cm = new SqlCommand(query, connection))
                    {

                        if (!string.IsNullOrEmpty(txtSearchBorrower.Text))
                        {
                            cm.Parameters.AddWithValue("@name", txtSearchBorrower.Text + "%");
                        }

                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                i++;

                                string date_registered = dr["date_registered"] != DBNull.Value
                                    ? DateTime.Parse(dr["date_registered"].ToString()).ToString("MMMM d, yyyy")
                                    : "N/A";


                                int rowIndex = dataGridView1.Rows.Add(
                                    i,
                                    null,
                                    null,
                                    dr["borrower_profile"],
                                    date_registered,
                                    dr["first_name"].ToString(),
                                    dr["last_name"].ToString(),
                                    dr["address"].ToString(),
                                    dr["phone_number"].ToString(),
                                    dr["id"].ToString()
                                );

                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["borrower_information"].Value = Properties.Resources.borrower_information;
                                row.Cells["delete_user"].Value = Properties.Resources.delete_user;
                            }
                        }
                    }
                }

                lblNoLowStocks.Visible = dataGridView1.Rows.Count == 0;

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["borrower_image"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void GetTotalApplicant()
        {

            try
            {
                int total_request = dataGridView1.Rows.Count;

                lblTotalTransactions.Text = total_request.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while counting total payment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frm_user_management_Load(object sender, EventArgs e)
        {
            LoadApplicant();
            GetTotalApplicant();

            this.ActiveControl = txtSearchBorrower;
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

        private void txtSearchBorrower_TextChanged(object sender, EventArgs e)
        {
            LoadApplicant();
            GetTotalApplicant();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (colName == "borrower_information")
            {
                try
                {
                    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                    frm_view_borrower_informations frm = new frm_view_borrower_informations();

                    string borrower_id = dataGridView1.Rows[selectedRowIndex].Cells["borrower_id2"].Value.ToString();


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
                        frm.txtMiddleName.Text = dr["middle_name"].ToString();
                        frm.txtLastName.Text = dr["last_name"].ToString();
                        frm.txtNameSuffix.Text = dr["name_suffix"].ToString();
                        frm.txtEmail.Text = dr["email_address"].ToString();
                        frm.txtUsername.Text = dr["username"].ToString();
                        frm.txtPassword.Text = "●●●●●●";
                        frm.txtAddress.Text = dr["address"].ToString();

                        string zipCode = dr["zip_code"] != DBNull.Value ? dr["zip_code"].ToString().Trim() : "";
                        frm.txtZIPCode.Text = !string.IsNullOrEmpty(zipCode) ? zipCode : "N/A";

                        frm.txtPhoneNumber.Text = dr["phone_number"].ToString();


                        if (dr["proof_of_income"] != DBNull.Value && dr["proof_of_income_filename"] != DBNull.Value)
                        {
                            frm.ProofOfIncomeBytes = (byte[])dr["proof_of_income"];
                            frm.ProofOfIncomeFilename = dr["proof_of_income_filename"].ToString();
                            frm.lblProof.Text += "  " + dr["proof_of_income_filename"].ToString();
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

            if (colName == "delete_user")
            {
                try
                {
                    int selectedRowIndex = e.RowIndex;
                    string borrower_id = dataGridView1.Rows[selectedRowIndex].Cells["borrower_id2"].Value.ToString();

                    DialogResult result = MessageBox.Show("Are you sure you want to delete this applicant?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        cn.Open();
                        string deleteQuery = "DELETE FROM tblBorrowerProfile WHERE id = @borrower_id";
                        cm = new SqlCommand(deleteQuery, cn);
                        cm.Parameters.AddWithValue("@borrower_id", borrower_id);
                        cm.ExecuteNonQuery();
                        cn.Close();

                        LoadApplicant();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting the applicant: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
        }
    }
}
