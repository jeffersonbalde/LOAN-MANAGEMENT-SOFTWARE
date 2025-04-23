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
    public partial class frm_reject_reason_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_lender_approved_payment frm;

        public frm_reject_reason_payment(frm_lender_approved_payment frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void frm_reject_reason_payment_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtReason;
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    MessageBox.Show("Reason for rejections field is required.\n\n" +
                    "Please complete the form before submitting.",
                    "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to reject this loan payment?",
                                                      "Confirm Rejection",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string query = @"
                UPDATE tblLoanPayment 
                SET status = @status, 
                    date_reviewed = @date_reviewed, 
                    rejection_reason = @rejection_reason
                WHERE id LIKE @id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@rejection_reason", txtReason.Text);
                        cmd.Parameters.AddWithValue("@status", "Rejected");
                        cmd.Parameters.AddWithValue("@date_reviewed", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", txtRequestID.Text);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }

                    //MessageBox.Show("Loan request has been rejected successfully.",
                    //                "Success",
                    //                MessageBoxButtons.OK,
                    //                MessageBoxIcon.Information);

                    frm.LoadPayment();
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
