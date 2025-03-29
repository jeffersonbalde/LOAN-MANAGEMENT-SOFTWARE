using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_create_account_2 : Form
    {
        public frm_create_account_2()
        {
            InitializeComponent();
        }

        private void siticoneButton2_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_create_account register_form = new frm_create_account();
            register_form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    pictureBoxProduct.BackgroundImage = Properties.Resources.caravatar; // Replace 'DefaultImage' with your actual image name in Resources.
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error setting default image: " + ex.Message);
            //}
        }
    }
}
