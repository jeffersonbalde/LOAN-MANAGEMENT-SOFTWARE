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
    public partial class frm_database_checking : Form
    {

        DBConnection dbcon = new DBConnection();

        private int progress = 0;

        public frm_database_checking()
        {
            InitializeComponent();

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value < 100)
            {
                progress += 5;
                progressBar.Value = progress;
            }
            else
            {
                timer.Stop();
            }
        }

        private async Task UpdateProgressBar(int targetValue, int seconds)
        {
            int steps = seconds * 2; 
            int increment = Math.Max(1, (targetValue - progressBar.Value) / steps);

            for (int i = 0; i < steps; i++)
            {
                if (progressBar.Value + increment > progressBar.Maximum)
                {
                    progressBar.Value = progressBar.Maximum;
                    break;
                }

                progressBar.Value += increment;
                await Task.Delay(500);
            }
        }

        private bool TestDatabaseConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbcon.MyConnection()))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private async void frm_database_checking_Load(object sender, EventArgs e)   
        {
            timer.Start();

            lblStatus.Text = "WELCOME TO LOAN MANAGEMENT SYSTEM PREPARING YOUR WORKSPACE... 🚀";
            await UpdateProgressBar(50, 10);

            lblStatus.Text = "Checking database connection...";
            bool isConnected = await Task.Run(() => TestDatabaseConnection());

            if (isConnected)
            {
                await UpdateProgressBar(100, 3); 
                lblStatus.Text = "Database connected!";
                await Task.Delay(500);

                this.Hide();
                frm_preloader frm = new frm_preloader();
                frm.Show();
            }
            else
            {
                lblStatus.Text = "Database connection failed!";
                MessageBox.Show("Failed to connect to the database. Please check your software installation and try again.",
                                "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
