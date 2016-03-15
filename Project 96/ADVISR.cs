using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_96
{
    public class ADVISR
    {
        private List<Item> Risers;
        private List<Item> Fallen;
        private int MIN, MAX, count;
        public ADVISR(int MIN, int MAX, int count)
        {
            this.MIN = MIN;
            this.MAX = MAX;
            this.count = count;

            Thread t = new Thread(new ThreadStart(this.loadResults));
            t.Start();

            Console.WriteLine();
        }

        private void loadResults()
        {
            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();
            string searchString = String.Format("SELECT * FROM ADVISR_FULL({0}, {1}, {2}) ORDER BY RN ASC", MIN, MAX, count);
            SqlCommand cmd = new SqlCommand(searchString, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Int32> UP_IDs = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("UP_ID")).ToList();
            List<Int32> DN_IDs = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("DN_ID")).ToList();
            Console.WriteLine("DONE!");
        }
    }
}