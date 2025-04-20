using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_view_borrower_informations : Form
    {

        public byte[] ProofOfIncomeBytes { get; set; }
        public string ProofOfIncomeFilename { get; set; }

        public frm_view_borrower_informations()
        {
            InitializeComponent();
        }

        private void DisplayProofOfIncome()
        {
            if (ProofOfIncomeBytes != null && !string.IsNullOrEmpty(ProofOfIncomeFilename))
            {
                string extension = Path.GetExtension(ProofOfIncomeFilename).ToLower();

                btnUploadProof.BackgroundImageLayout = ImageLayout.Stretch;
                btnUploadProof.Text = "";
                btnUploadProof.FillColor = Color.Transparent;
                btnUploadProof.BackColor = Color.Transparent;
                btnUploadProof.UseTransparentBackground = true;

                try
                {
                    using (MemoryStream ms = new MemoryStream(ProofOfIncomeBytes))
                    {
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            btnUploadProof.BackgroundImage = Image.FromStream(ms);
                        }
                        else if (extension == ".pdf")
                        {
                            using (var pdfDoc = PdfiumViewer.PdfDocument.Load(ms))
                            {
                                using (var img = pdfDoc.Render(0, 300, 300, true))
                                {
                                    btnUploadProof.BackgroundImage = new Bitmap(img);
                                    btnUploadProof.BackgroundImageLayout = ImageLayout.Zoom;
                                }
                            }
                        }
                        else
                        {
                            btnUploadProof.BackgroundImage = Properties.Resources.default_file_2; // optional
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error displaying proof of income: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                btnUploadProof.BackgroundImage = null;
                btnUploadProof.Text = "No file";
            }
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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void siticoneButton2_Click(object sender, EventArgs e)
        {
            if (ProofOfIncomeBytes != null && !string.IsNullOrEmpty(ProofOfIncomeFilename))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = ProofOfIncomeFilename;
                    saveFileDialog.Title = "Save Proof of Income";
                    saveFileDialog.Filter = "All Files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = saveFileDialog.FileName;

                        try
                        {
                            // Run the file writing in a background thread
                            await Task.Run(() => File.WriteAllBytes(path, ProofOfIncomeBytes));

                            MessageBox.Show("Proof of income downloaded successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving file: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void frm_view_borrower_informations_Load(object sender, EventArgs e)
        {
            DisplayProofOfIncome();
        }
    }
}
