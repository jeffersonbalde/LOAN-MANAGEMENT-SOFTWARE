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

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class frm_borrower_dashboard : Form
    {
        public frm_borrower_dashboard()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(135, 230, 133),  // Top color
                Color.FromArgb(101, 173, 139),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                //Color.FromArgb(231, 229, 251),  // Top color
                //Color.FromArgb(230, 187, 254),  // Bottom color

                Color.FromArgb(220, 106, 44),  // Top color
                Color.FromArgb(206, 37, 104),  // Bottom color
                LinearGradientMode.Vertical)) // You can try Horizontal, ForwardDiagonal, etc.
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
        }
    }
}
