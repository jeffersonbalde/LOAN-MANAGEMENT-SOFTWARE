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
    public partial class frm_cancel_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;


        frm_borrower_payment frm;


        public frm_cancel_payment(frm_borrower_payment frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void frm_cancel_payment_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtReason;
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
                    MessageBox.Show("Reason for cancellation field is required.\n\n" +
                    "Please complete the form before saving.",
                    "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to cancel this loan payment?",
                                                      "Confirm Cancellation",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string query = @"
                UPDATE tblLoanPayment 
                SET status = @status, 
                    loan_status = @loan_status, 
                    cancel_reason = @cancel_reason,
                    date_cancelled = @date_cancelled
                WHERE id LIKE @id";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@status", "Cancelled");
                        cmd.Parameters.AddWithValue("@loan_status", "Cancelled");
                        cmd.Parameters.AddWithValue("@cancel_reason", txtReason.Text);
                        cmd.Parameters.AddWithValue("@date_cancelled", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id", txtRequestID.Text);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }

                    //MessageBox.Show("Loan request has been cancelled successfully.",
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
