using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    public partial class Form1 : Form
    {

        private Image animatedGif; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            animatedGif = Properties.Resources.gif_login;
            pictureBox1.Image = animatedGif; 

            ImageAnimator.Animate(animatedGif, OnFrameChanged);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            ImageAnimator.UpdateFrames(); 
            pictureBox1.Invalidate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ImageAnimator.StopAnimate(animatedGif, OnFrameChanged);
        }
    }
}
