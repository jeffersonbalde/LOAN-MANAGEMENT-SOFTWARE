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

            //for local sql server
            con = @"Data Source=LAPTOP-DMAMJ5FJ,1433;Initial Catalog=SUBLIMATION_DB;User Id=SuperAdmin;Password=SuperAdmin;";

            return con;

            //for deploy offline database
            //con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\OOP.mdf;Integrated Security=True;";
            //return con;
        }

    }
}
