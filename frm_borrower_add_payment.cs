using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_add_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        frm_borrower_payment frm;

        public frm_borrower_add_payment(frm_borrower_payment frm)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
            this.frm = frm;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
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
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(231, 229, 251),  // Top color
                Color.FromArgb(230, 187, 254),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        public void LoadBorrowerDetails()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblBorrowerProfile WHERE id = @id";
                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@id", txtID.Text);

                dr = cm.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();

                    string first_name = dr["first_name"].ToString();
                    string last_name = dr["last_name"].ToString();

                    txtName.Text = first_name + " " + last_name;

                    txtAddress.Text = dr["address"].ToString();
                    txtPhoneNumber.Text = dr["phone_number"].ToString();
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public void LoadReferenceNumber()
        {
            try
            {
                cn.Open();
                string query = @"
            SELECT TOP 1 request_number 
            FROM tblLoanRequests 
            WHERE borrower_id = @id AND loan_status = 'Ongoing'";

                cm = new SqlCommand(query, cn);
                cm.Parameters.AddWithValue("@id", txtID.Text.Trim());

                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    txtReferenceNumber.Text = dr["request_number"].ToString();
                }
                else
                {
                    txtReferenceNumber.Text = ""; // No ongoing loan found
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading latest ongoing request: " + ex.Message);
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }


        private string proofOfIncomeFilePath = string.Empty;

        private void siticoneButton4_Click(object sender, EventArgs e, Siticone.Desktop.UI.WinForms.SiticoneButton btnUploadProof)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select Proof of Income",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Filter = "Valid Files (*.pdf;*.jpg;*.jpeg;*.png)|*.pdf;*.jpg;*.jpeg;*.png|PDF Files (*.pdf)|*.pdf|Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    proofOfIncomeFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(proofOfIncomeFilePath);
                    string extension = Path.GetExtension(proofOfIncomeFilePath).ToLower();

                    btnUploadProof.BackgroundImageLayout = ImageLayout.Stretch;
                    btnUploadProof.Text = "";
                    btnUploadProof.FillColor = Color.Transparent;
                    btnUploadProof.BackColor = Color.Transparent;
                    btnUploadProof.UseTransparentBackground = true;

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        btnUploadProof.BackgroundImage = Image.FromFile(proofOfIncomeFilePath);
                    }
                    else if (extension == ".pdf")
                    {
                        using (var document = PdfiumViewer.PdfDocument.Load(proofOfIncomeFilePath))
                        {
                            using (var pdfImage = document.Render(0, 300, 300, true))
                            {
                                btnUploadProof.BackgroundImage = new Bitmap(pdfImage);
                                btnUploadProof.BackgroundImageLayout = ImageLayout.Zoom;
                            }
                        }
                    }
                    else
                    {
                        btnUploadProof.BackgroundImage = Properties.Resources.default_file_2;
                    }

                    StartUploadAnimation(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while selecting the file:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartUploadAnimation(string fileName)
        {
            siticoneProgressBar1.Value = 0;
            lblFileName.Text = "Uploading...";
            lblFileName.ForeColor = Color.Orange;

            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) =>
            {
                if (siticoneProgressBar1.Value < 100)
                {
                    siticoneProgressBar1.Value += 10;
                }
                else
                {
                    timer.Stop();
                    lblFileName.Text = $"✅ File {fileName} uploaded successfully!";
                    lblFileName.ForeColor = Color.Green;
                    //btnUploadProof.Text = fileName;
                }
            };
            timer.Start();
        }

        public void LoadCurrentBalance()
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
                    lblCurrentBalance.Text = string.Format("₱{0:N2}", Convert.ToDecimal(dr["total_balance"]));
                }
                else
                {
                    lblCurrentBalance.Text = "₱0.00";
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

        private void D(object sender, EventArgs e)
        {
            LoadBorrowerDetails(); 
            LoadReferenceNumber();
            LoadCurrentBalance();
            GetRequestNumber();

            cmMOP.Items.Clear();
            cmMOP.Items.Add("Cash");
            cmMOP.Items.Add("Cheque");
            cmMOP.Items.Add("Bank Transfer");
            cmMOP.Items.Add("GCash");
            cmMOP.Items.Add("Others");
            cmMOP.SelectedIndex = 0;
        }

        public void GetRequestNumber()
        {
            try
            {
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                string transno;
                int count;

                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    string query = "SELECT MAX(request_number) FROM tblRequestNumber WHERE request_number LIKE @sdate + '%'";
                    cm = new SqlCommand(query, cn, transaction);
                    cm.Parameters.AddWithValue("@sdate", sdate);

                    object result = cm.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        transno = result.ToString();
                        count = int.Parse(transno.Substring(8)) + 1;
                    }
                    else
                    {
                        count = 100001;
                    }

                    transno = sdate + count.ToString("D6");

                    string insertQuery = "INSERT INTO tblRequestNumber (request_number) VALUES (@request_number)";
                    cm = new SqlCommand(insertQuery, cn, transaction);
                    cm.Parameters.AddWithValue("@request_number", transno);
                    cm.ExecuteNonQuery();

                    transaction.Commit();
                    txtPaymentNumber.Text = transno;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnUploadProof_Click(object sender, EventArgs e)
        {
            siticoneButton4_Click(sender, e, btnUploadProof);
        }

        private void siticoneTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == 8)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == 46)
            {
                if (txtAmountPaid.Text.Contains(".") || txtAmountPaid.SelectionStart == 0)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            txtAmountPaid.TextChanged -= txtPaidAmount_TextChanged;

            try
            {
                if (!string.IsNullOrWhiteSpace(txtAmountPaid.Text))
                {
                    string rawText = txtAmountPaid.Text.Replace(",", "");

                    if (rawText.EndsWith("."))
                    {
                        txtAmountPaid.TextChanged += txtPaidAmount_TextChanged;
                        return;
                    }

                    double number;
                    if (double.TryParse(rawText, out number))
                    {
                        txtAmountPaid.Text = number.ToString("#,##0.###");

                        txtAmountPaid.SelectionStart = txtAmountPaid.Text.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                txtAmountPaid.Text = "";
            }

            txtAmountPaid.TextChanged += txtPaidAmount_TextChanged;

            CalculateUpdatedBalance();
        }

        public void CalculateUpdatedBalance()
        {
            try
            {
                double current_balance = 0;
                if (!string.IsNullOrEmpty(lblCurrentBalance.Text))
                {
                    double.TryParse(lblCurrentBalance.Text.Replace("₱", "").Replace(",", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out current_balance);
                }

                double paidAmount = 0;
                bool isValidPaidAmount = !string.IsNullOrWhiteSpace(txtAmountPaid.Text) &&
                                         double.TryParse(txtAmountPaid.Text.Replace(",", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out paidAmount);

                // If empty or 0, revert updated balance to current balance
                if (!isValidPaidAmount || paidAmount == 0)
                {
                    lblUpdatedBalance.Text = "₱" + current_balance.ToString("N2");
                    return;
                }

                // Validate: Paid amount should not exceed current balance
                if (paidAmount > current_balance)
                {
                    MessageBox.Show("Paid amount cannot be greater than the current balance.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAmountPaid.Text = "";
                    lblUpdatedBalance.Text = "₱" + current_balance.ToString("N2");
                    return;
                }

                double updatedBalance = current_balance - paidAmount;
                lblUpdatedBalance.Text = "₱" + updatedBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the balance: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtAmountPaid.Text) ||
                    string.IsNullOrWhiteSpace(cmMOP.Text))
                {
                    MessageBox.Show("All fields are required.\n\nPlease complete the form before sending.",
                        "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (lblFileName.ForeColor != Color.Green)
                {
                    MessageBox.Show("Please upload a valid proof of payment file.",
                                    "Invalid File",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    btnUploadProof.Focus();
                    return;
                }

                byte[] borrowerProfileData = null;

                cn.Open();

                using (SqlCommand selectCmd = new SqlCommand("SELECT borrower_profile FROM tblBorrowerProfile WHERE id = @borrower_id", cn))
                {
                    selectCmd.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());
                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            borrowerProfileData = (byte[])reader["borrower_profile"];
                        }
                        else
                        {
                            MessageBox.Show("Borrower profile not found.");
                            return;
                        }
                    }
                }

                cn.Close();

                if (MessageBox.Show("Are you sure you want to submit your loan payment?",
                        "Confirm Loan Payment",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                decimal current_balance = decimal.Parse(
                    lblCurrentBalance.Text.Replace("₱", "").Replace(",", "").Trim(),
                    System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
                    System.Globalization.CultureInfo.InvariantCulture
                );

                decimal paid_amount = decimal.Parse(
                    txtAmountPaid.Text.Replace("₱", "").Replace(",", "").Trim(),
                    System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
                    System.Globalization.CultureInfo.InvariantCulture
                );

                decimal updated_balance = decimal.Parse(
                    lblUpdatedBalance.Text.Replace("₱", "").Replace(",", "").Trim(),
                    System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands,
                    System.Globalization.CultureInfo.InvariantCulture
                );

                string proofOfPaymentFilePath = this.proofOfIncomeFilePath;
                byte[] proofPayment = System.IO.File.ReadAllBytes(proofOfPaymentFilePath);

                cn.Open();

                string query = "INSERT INTO tblLoanPayment " +
                                "(borrower_id, name, address, phone_number, reference_number, payment_date, proof_of_payment, proof_of_payment_filename, current_balance, paid_amount, mode_of_payment, gcash_reference, notes, updated_balance, status, loan_status, payment_number, borrower_profile) " +
                                "VALUES (@borrower_id, @name, @address, @phone_number, @reference_number, @payment_date, @proof_of_payment, @proof_of_payment_filename, @current_balance, @paid_amount, @mode_of_payment, @gcash_reference, @notes, @updated_balance, @status, @loan_status, @payment_number, @borrower_profile)";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@borrower_id", txtID.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone_number", txtPhoneNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@reference_number", txtReferenceNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@payment_date", dtPaymentDate.Value);
                    cmd.Parameters.AddWithValue("@proof_of_payment", proofPayment);
                    cmd.Parameters.AddWithValue("@proof_of_payment_filename", Path.GetFileName(proofOfPaymentFilePath));
                    cmd.Parameters.AddWithValue("@current_balance", current_balance);
                    cmd.Parameters.AddWithValue("@paid_amount", paid_amount);
                    cmd.Parameters.AddWithValue("@mode_of_payment", cmMOP.Text);
                    cmd.Parameters.AddWithValue("@gcash_reference", txtGcash.Text);
                    cmd.Parameters.AddWithValue("@notes",txtNotes.Text.Trim());
                    cmd.Parameters.AddWithValue("@updated_balance", updated_balance);
                    cmd.Parameters.AddWithValue("@status", "Pending");
                    cmd.Parameters.AddWithValue("@loan_status", "Pending");
                    cmd.Parameters.AddWithValue("@payment_number", txtPaymentNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@borrower_profile", borrowerProfileData);
                    cmd.ExecuteNonQuery();
                }

                cn.Close();

                frm.LoadPayment();
                this.Dispose();

            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void frm_borrower_add_payment_Load(object sender, EventArgs e)
        {
            TimeZoneInfo phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            DateTime phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, phTimeZone);

            dtPaymentDate.Value = phTime;

            LoadBorrowerDetails();
            LoadReferenceNumber();
            LoadCurrentBalance();
            GetRequestNumber();

            cmMOP.Items.Clear();
            cmMOP.Items.Add("Cash");
            cmMOP.Items.Add("Cheque");
            cmMOP.Items.Add("Bank Transfer");
            cmMOP.Items.Add("GCash");
            cmMOP.Items.Add("Others");
            cmMOP.SelectedIndex = 0;
        }
    }
}
