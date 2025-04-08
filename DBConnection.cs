using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOAN_MANAGEMENT_SOFTWARE
{
    internal class DBConnection
    {

        private string con;

        public string MyConnection()
        {
            //for checking
            //con = @"Data Source=.;Initial Catalog=LOAN_DB;Integrated Security=True;";
            //return con

            //for local sql server
            con = @"Data Source=LAPTOP-DMAMJ5FJ,1433;Initial Catalog=LOAN_DB;User Id=SuperAdmin;Password=SuperAdmin;";
            return con;

            ////for deploy offline database
            //con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LOAN_DB.mdf;Integrated Security=True;";
            //return con;
        }

    }
}
