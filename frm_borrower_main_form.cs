using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_main_form : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        private Image animatedGif;

        public frm_borrower_main_form()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x200; // CS_NOCLOSE (Disables Close Button)
                return cp;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = "Today is " + DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");
        }

        private void frm_borrower_main_form_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; 
            timer1.Tick += timer1_Tick;
            timer1.Start();

            LoadBusinessLogo();

            animatedGif = Properties.Resources.under_maintenance2;
            pictureBox1.Image = animatedGif;

            ImageAnimator.Animate(animatedGif, OnFrameChanged);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {

            if (!this.IsDisposed && !pictureBox1.IsDisposed)
            {
                pictureBox1.Invalidate();
            }

        }

        public void LoadBusinessLogo()
        {
            try
            {
                cn.Open();
                string query = "SELECT * FROM tblBusinessProfile";
                cm = new SqlCommand(query, cn);
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {

                    if (dr["business_logo"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])dr["business_logo"]; 
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            using (Image tempImage = Image.FromStream(ms))
                            {
                                pictureBoxLogo.BackgroundImage = new Bitmap(tempImage);
                            }
                        }
                        pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                    {
                        pictureBoxLogo.BackgroundImage = null;
                    }

                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_landing_page frm = new frm_landing_page();
            frm.Show();
            this.Dispose();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Process.Start("calc.exe");
        }

        private void label8_Click(object sender, EventArgs e)
        {
                try
                {

                        frm_borrower_management frm = new frm_borrower_management();

                        cn.Open();
                        string query = "SELECT * FROM tblBorrowerProfile WHERE id LIKE '" + txtID.Text + "%'";
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
                                        frm.pictureBoxUserImage.BackgroundImage = new Bitmap(tempImage);
                                    }
                                }
                                frm.pictureBoxUserImage.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                            else
                            {
                                frm.pictureBoxUserImage.BackgroundImage = null; 
                            }

                            frm.txtFirstName.Text = dr["first_name"].ToString();
                            frm.txtLastName.Text = dr["last_name"].ToString();
                            frm.txtEmail.Text = dr["email_address"].ToString();
                            frm.txtPassword.Text = dr["password"].ToString();
                            frm.txtPhoneNumber.Text = dr["phone_number"].ToString();
                            frm.txtAddress.Text = dr["address"].ToString();
                            frm.txtZipCode.Text = dr["zip_code"].ToString();
                            //frm.txtM.Text = dr["zip_code"].ToString();

                            //string make = dr["make"].ToString();
                            //string model = dr["model"].ToString();
                            //frm.txtColor.Text = dr["color"].ToString();
                            //frm.txtPlate.Text = dr["plate"].ToString();

                            //// ✅ Store make and model in `Tag` as a Dictionary
                            //frm.Tag = new Dictionary<string, string>
                            //{
                            //    { "make", make },
                            //    { "model", model }
                            //};


                            //if (dr["car_image"] != DBNull.Value)
                            //{
                            //    byte[] imageBytes = (byte[])dr["car_image"]; // Assuming 'car_image' is the column name in the database
                            //    using (MemoryStream ms = new MemoryStream(imageBytes))
                            //    {
                            //        using (Image tempImage = Image.FromStream(ms))
                            //        {
                            //            // Clone the image to detach it from the stream
                            //            frm.pictureBoxProduct.BackgroundImage = new Bitmap(tempImage);
                            //        }
                            //    }
                            //    frm.pictureBoxProduct.SizeMode = PictureBoxSizeMode.StretchImage;
                            //}
                            //else
                            //{
                            //    frm.pictureBoxProduct.BackgroundImage = null; // Set to null if no image is found
                            //}



                            //frm.cboStatus.Text = dr["status"].ToString();
                            //frm.txtEmployee.Text = dr["employee"].ToString();

                            //frm.isInitializing = true;

                            //// Load the dates into the DateTimePickers
                            //if (dr["date_drop"] != DBNull.Value)
                            //{
                            //    frm.dtStartDate.Value = Convert.ToDateTime(dr["date_drop"]);
                            //}

                            //if (dr["date_released"] != DBNull.Value)
                            //{
                            //    frm.dtEndDate.Value = Convert.ToDateTime(dr["date_released"]);
                            //}

                            //frm.isInitializing = false;

                            //// Check if discount is NULL or empty before parsing
                            //object discountObj = dr["discount"]; // Get the object from DB
                            //string discountValue = discountObj != DBNull.Value ? discountObj.ToString() : "0"; // Convert safely

                            //if (!double.TryParse(discountValue, out double discountAmount))
                            //{
                            //    discountAmount = 0; // Default to 0 if parsing fails
                            //}

                            //frm.discount_amount = discountAmount;

                            //// Format the discount with peso sign and commas
                            //frm.txtDiscount.Text = $"₱{discountAmount:#,##0.00}";



                            //frm.txtGrossTotal.Text = "₱" + dr["total_cost"].ToString();
                            //frm.txtAmountPaid.Text = dr["paid_amount"].ToString();



                            //frm.txtTypeOfPurchased.Text = dr["mode_of_payment"].ToString();

                            //if (dr["payment_status"] != DBNull.Value)
                            //{
                            //    string paymentStatus = dr["payment_status"].ToString();

                            //    // Check or uncheck the checkbox based on the payment status
                            //    if (paymentStatus == "Down Payment")
                            //    {
                            //        frm.checkBoxDP.Checked = true; // Check the checkbox for Down Payment
                            //        frm.checkBoxUnpaid.Checked = false;
                            //    }
                            //    //else if(paymentStatus == )
                            //    else if (paymentStatus == "Unpaid")
                            //    {
                            //        frm.checkBoxDP.Checked = false; // Uncheck the checkbox for Paid
                            //        frm.checkBoxUnpaid.Checked = true;
                            //    }
                            //    else if (paymentStatus == "Fully Paid")
                            //    {
                            //        frm.checkBoxDP.Checked = false; // Uncheck the checkbox for Paid
                            //        frm.checkBoxUnpaid.Checked = false;
                            //    }
                            //}

                            //frm.txtNotes.Text = dr["notes"].ToString();

                        }
                        dr.Close();
                        cn.Close();


                        //frm.txtProjectCode.Text = transaction_code;
                        //frm.updateClick = true;
                        //frm.button2.Visible = false;
                        //frm.button3.Visible = false;
                        //frm.btnUpdate.Visible = true;
                        //frm.txtCustomer.Focus();

                        //frm.Text = "Edit Transaction";

                        ////frm.button4.Location = new Point(1046, 569);

                        //frm.txtUpdate.Text = "1";

                        frm.ShowDialog();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void frm_borrower_main_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (animatedGif != null)
            {
                ImageAnimator.StopAnimate(animatedGif, OnFrameChanged);
                animatedGif.Dispose();
                animatedGif = null;
            }
        }
    }
}
