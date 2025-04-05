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
    public partial class frm_preloader : Form
    {
        public frm_preloader()
        {
            InitializeComponent();
        }

        private void frm_preloader_Load(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void CenterPanel()
        {
            int centerX = (this.ClientSize.Width - panel1.Width) / 2;
            int centerY = (this.ClientSize.Height - panel1.Height) / 2;

            panel1.Location = new Point(centerX, centerY);
        }

        private void frm_preloader_Resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm_landing_page frm = new frm_landing_page();
            frm.Show();
        }
    }
}
