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
using System.Xml.Linq;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_rejected_reason : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_lender_view_loan_request frm;

        public frm_rejected_reason(frm_lender_view_loan_request frm)
        {
            InitializeComponent(); 
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    MessageBox.Show("Reason for rejections field is required.\n\n" +
                    "Please complete the form before saving.",
                    "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to reject this loan request?",
                                                      "Confirm Rejection",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string query = @"
                UPDATE tblLoanRequests 
                SET status = @status, 
                    date_reviewed = @date_reviewed, 
                    loan_status = @loan_status, 
                    rejection_reason = @rejection_reason
                WHERE id LIKE @id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@rejection_reason", txtReason.Text);
                        cmd.Parameters.AddWithValue("@status", "Rejected");
                        cmd.Parameters.AddWithValue("@date_reviewed", DateTime.Now);
                        cmd.Parameters.AddWithValue("@loan_status", "Rejected");
                        cmd.Parameters.AddWithValue("@id", txtRequestID.Text);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }

                    //MessageBox.Show("Loan request has been rejected successfully.",
                    //                "Success",
                    //                MessageBoxButtons.OK,
                    //                MessageBoxIcon.Information);

                    frm.LoadRequest();
                    this.Dispose();
                }

            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
