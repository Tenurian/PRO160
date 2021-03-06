﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project_96
{
    public class Item
    {
        public readonly string Name, Description, ImageURL;
        public readonly int ID, Price;
        public readonly bool membersonly;
        public List<HistoryPrice> DailyList, AverageList;

        public Item(int id)
        {
            //all of these assignments will be changed to the database later, but for now they are done from the website
            string ItemInfo = GET(String.Format(@"{0}{1}", "http://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item=", id));
            dynamic ItemJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(ItemInfo);

            if (ItemJSON != null && ItemJSON.ToString() != "")
            {
                this.Name = ItemJSON.item.name;
                this.Description = ItemJSON.item.description;
                this.ImageURL = ItemJSON.item.icon_large;
                this.ID = id;
                //this.Price = PriceConvert((ItemJSON.item.current.price).ToString());
                this.Price = TruePriceConvert((ItemJSON.item.today.price).ToString());

                this.membersonly = System.Text.RegularExpressions.Regex.IsMatch(ItemJSON.item.members.ToString(), "true", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            else
            {
                SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
                con.Open();

                string searchString = String.Format("select name, description, imageURL, membersOnly from ItemInfo where id = {0}", id);

                SqlCommand cmd = new SqlCommand(searchString, con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                this.ID = id;
                this.Name = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList().First();
                this.Description = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("description")).ToList().First();
                this.ImageURL = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("imageURL")).ToList().First();
                this.Price = -1;
                this.membersonly = (dt.Rows.OfType<DataRow>().Select(dr => dr.Field<bool>("membersOnly")).ToList().First());
            }




            string historyInfo = GET(String.Format(@"{0}{1}.json", "http://services.runescape.com/m=itemdb_oldschool/api/graph/", id));
            dynamic historyJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(historyInfo);
            if (historyJSON != null && historyJSON.ToString() != "") { 
                Dictionary<string, int> DailyValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(String.Format("{0}", historyJSON.daily));
                DailyList = new List<HistoryPrice>();
                foreach (KeyValuePair<string, int> x in DailyValues)
                {
                    DailyList.Add(new HistoryPrice(x.Key, x.Value));
                }

                Dictionary<string, int> AverageValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(String.Format("{0}", historyJSON.average));
                AverageList = new List<HistoryPrice>();
                foreach (KeyValuePair<string, int> x in AverageValues)
                {
                    AverageList.Add(new HistoryPrice(x.Key, x.Value));
                }
            }
            else
            {
            }
        }
        
        public Item(string name, int id, string description, string imageURL, bool members)
        {
            this.Name = name;
            this.ID = id;
            this.Description = description;
            this.ImageURL = imageURL;
            this.membersonly = members;
            
            string ItemInfo = GET(String.Format(@"{0}{1}", "http://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item=", id));
            dynamic T = Newtonsoft.Json.JsonConvert.DeserializeObject(ItemInfo);
            this.Price = TruePriceConvert((T.item.today.price).ToString());
            var x = TruePriceConvert((T.item.today.price).ToString());


            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();

            string searchString = String.Format("select h_date, daily, average from PriceHistory where id = {0}", id);

            SqlCommand cmd = new SqlCommand(searchString, con);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<DateTime> dates = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<DateTime>("h_date")).ToList();
            List<int> daily = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<int>("daily")).ToList();
            List<int> average = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<int>("average")).ToList();
            DailyList = new List<HistoryPrice>();
            AverageList = new List<HistoryPrice>();
            for (int i = 0; i < dates.Count; i++)
            {
                DailyList.Add(new HistoryPrice(dates.ElementAt(i).ToString(), daily.ElementAt(i)));
                AverageList.Add(new HistoryPrice(dates.ElementAt(i).ToString(), average.ElementAt(i)));
            }
        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public int TruePriceConvert(String priceChange)
        {
            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();

            string searchString = String.Format("SELECT TOP(1) daily FROM PriceHistory WHERE id = {0} ORDER BY h_date DESC", this.ID);

            int? z = null;
            int v;
            var s = priceChange.Substring(1);
            if (Int32.TryParse(GetNumbers(priceChange), out v))
            {
                var y = (priceChange.Substring(0,1) == "+") ? 1 : -1;
                z = y * v;
            }
            else
            {
                z = null;
            }

            SqlCommand cmd = new SqlCommand(searchString, con);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int? x = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("daily")).ToList().First();

            if (x.HasValue && z.HasValue)
            {
                return x.Value + z.Value;
            }


            return -1;
        }

        public int PriceConvert(string convert)
        {

            int p;
            double d;
            if(Int32.TryParse(convert, out p))
            {
                return p;
            }
            else
            {
                string k = "k", m = "m";

                if(System.Text.RegularExpressions.Regex.IsMatch(convert, k, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    if (Double.TryParse(convert.Replace(k, ""), out d))
                    {
                        return Convert.ToInt32((d * 1000));
                    }
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(convert, m, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    if (Double.TryParse(convert.Replace("m", ""), out d))
                    {
                        return Convert.ToInt32((d * 1000000)); ;
                    }
                }

                return -1;
            }
        }

        public string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse historyInfo = request.GetResponse();
                using (Stream historyInfoStream = historyInfo.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(historyInfoStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream historyInfoStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(historyInfoStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public class HistoryPrice
        {
            public string date;
            public int prc;

            public HistoryPrice(string key, int value)
            {
                this.date = key;
                this.prc = value;
            }
        }
    }
}
