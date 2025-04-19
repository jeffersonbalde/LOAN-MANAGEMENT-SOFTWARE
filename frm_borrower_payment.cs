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
    public partial class frm_borrower_payment : Form
    {

        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnection dbcon = new DBConnection();
        SqlDataReader dr;

        public frm_borrower_payment()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConnection());
        }
    }
}
