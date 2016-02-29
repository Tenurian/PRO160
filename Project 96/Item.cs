using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


            this.Name = ItemJSON.item.name;
            this.Description = ItemJSON.item.description;
            this.ImageURL = ItemJSON.item.icon_large;
            this.ID = id;
            Type t = (ItemJSON.item.current.price).GetType();
            this.Price = PriceConvert((ItemJSON.item.current.price).ToString());


            string historyInfo = GET(String.Format(@"{0}{1}.json", "http://services.runescape.com/m=itemdb_oldschool/api/graph/", id));
            dynamic historyJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(historyInfo);

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

        public Item(string name, int id, string description, string imageURL)
        {
            this.Name = name;
            this.ID = id;
            this.Description = description;
            this.ImageURL = imageURL;


            string ItemInfo = GET(String.Format(@"{0}{1}", "http://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item=", id));
            dynamic ItemJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(ItemInfo);

            this.Price = PriceConvert((ItemJSON.item.current.price).ToString());


            string historyInfo = GET(String.Format(@"{0}{1}.json", "http://services.runescape.com/m=itemdb_oldschool/api/graph/", id));
            dynamic historyJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(historyInfo);

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

        public int PriceConvert(string convert)
        {
            int p;
            if(Int32.TryParse(convert, out p))
             {
                return p;
            }
            else
            {
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
