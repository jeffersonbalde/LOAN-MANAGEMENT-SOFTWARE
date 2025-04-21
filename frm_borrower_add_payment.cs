using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
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
                    txtRequestNumber.Text = dr["request_number"].ToString();
                }
                else
                {
                    txtRequestNumber.Text = ""; // No ongoing loan found
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


        private void frm_borrower_add_payment_Load(object sender, EventArgs e)
        {
            LoadBorrowerDetails(); 
            LoadReferenceNumber();

            cmMOP.Items.Clear();
            cmMOP.Items.Add("Cash");
            cmMOP.Items.Add("Cheque");
            cmMOP.Items.Add("Bank Transfer");
            cmMOP.Items.Add("GCash");
            cmMOP.SelectedIndex = 0;
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
                if (txtPaidAmount.Text.Contains(".") || txtPaidAmount.SelectionStart == 0)
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
            txtPaidAmount.TextChanged -= txtPaidAmount_TextChanged;

            try
            {
                if (!string.IsNullOrWhiteSpace(txtPaidAmount.Text))
                {
                    string rawText = txtPaidAmount.Text.Replace(",", "");

                    if (rawText.EndsWith("."))
                    {
                        txtPaidAmount.TextChanged += txtPaidAmount_TextChanged;
                        return;
                    }

                    double number;
                    if (double.TryParse(rawText, out number))
                    {
                        txtPaidAmount.Text = number.ToString("#,##0.###");

                        txtPaidAmount.SelectionStart = txtPaidAmount.Text.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                txtPaidAmount.Text = "";
            }

            txtPaidAmount.TextChanged += txtPaidAmount_TextChanged;
        }
    }
}
