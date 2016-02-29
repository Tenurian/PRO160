//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace Project_96
//{

//    public class TesterItem
//    {
//        public readonly String response;
//        public readonly String res;
//        public List<HistoryPrice> DailyList, AverageList;

//        public TesterItem()
//        {
//            response = GET(@"http://services.runescape.com/m=itemdb_oldschool/api/graph/2.json");
//            DailyList = new List<HistoryPrice>();
//            AverageList = new List<HistoryPrice>();

//            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

//            Dictionary<string, int> DailyValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(String.Format("{0}", json.daily));

//            foreach (KeyValuePair<string, int> x in DailyValues)
//            {
//                DailyList.Add(new HistoryPrice(x.Key, x.Value));
//            }
//            Dictionary<string, int> AverageValues = JsonConvert.DeserializeObject<Dictionary<string, int>>(String.Format("{0}", json.average));

//            foreach (KeyValuePair<string, int> x in AverageValues)
//            {
//                AverageList.Add(new HistoryPrice(x.Key, x.Value));
//            }
//        }



//        public class HistoryPrice
//        {
//            public string date { get; set; }
//            public int prc { get; set; }
//            public HistoryPrice(string epoch, int cost)
//            {
//                this.date = epoch;
//                this.prc = cost;
//            }

            
//        }

//        /* Future code to try to bind it to the JSON object from the server */
//        public string GET(string url)
//        {
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
//            try
//            {
//                WebResponse response = request.GetResponse();
//                using (Stream responseStream = response.GetResponseStream())
//                {
//                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
//                    return reader.ReadToEnd();
//                }
//            }
//            catch (WebException ex)
//            {
//                WebResponse errorResponse = ex.Response;
//                using (Stream responseStream = errorResponse.GetResponseStream())
//                {
//                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
//                    String errorText = reader.ReadToEnd();
//                    // log errorText
//                }
//                throw;
//            }
//        }

//        public readonly string
//            name = "Cannonball",
//            description = "Ammo for the Dwarf Cannon.",
//            iconURL = "http://services.runescape.com/m=itemdb_oldschool/5104_obj_sprite.gif?id=2";
//        public readonly int 
//            id = 2,
//            price = 205;
//        public readonly bool membersonly = true;

//        public readonly List<HistoryPrice> average = new List<HistoryPrice>()
//        {
//            { new HistoryPrice(
//              "1440288000000",206
//              ) },{ new HistoryPrice(
//              "1440374400000",207
//              ) },{ new HistoryPrice(
//              "1440460800000",208
//              ) },{ new HistoryPrice(
//              "1440547200000",210
//              ) },{ new HistoryPrice(
//              "1440633600000",212
//              ) },{ new HistoryPrice(
//              "1440720000000",214
//              ) },{ new HistoryPrice(
//              "1440806400000",215
//              ) },{ new HistoryPrice(
//              "1440892800000",216
//              ) },{ new HistoryPrice(
//              "1440979200000",217
//              ) },{ new HistoryPrice(
//              "1441065600000",217
//              ) },{ new HistoryPrice(
//              "1441152000000",217
//              ) },{ new HistoryPrice(
//              "1441238400000",218
//              ) },{ new HistoryPrice(
//              "1441324800000",218
//              ) },{ new HistoryPrice(
//              "1441411200000",219
//              ) },{ new HistoryPrice(
//              "1441497600000",220
//              ) },{ new HistoryPrice(
//              "1441584000000",221
//              ) },{ new HistoryPrice(
//              "1441670400000",221
//              ) },{ new HistoryPrice(
//              "1441756800000",221
//              ) },{ new HistoryPrice(
//              "1441843200000",221
//              ) },{ new HistoryPrice(
//              "1441929600000",222
//              ) },{ new HistoryPrice(
//              "1442016000000",223
//              ) },{ new HistoryPrice(
//              "1442102400000",223
//              ) },{ new HistoryPrice(
//              "1442188800000",225
//              ) },{ new HistoryPrice(
//              "1442275200000",226
//              ) },{ new HistoryPrice(
//              "1442361600000",227
//              ) },{ new HistoryPrice(
//              "1442448000000",228
//              ) },{ new HistoryPrice(
//              "1442534400000",229
//              ) },{ new HistoryPrice(
//              "1442620800000",230
//              ) },{ new HistoryPrice(
//              "1442707200000",231
//              ) },{ new HistoryPrice(
//              "1442793600000",231
//              ) },{ new HistoryPrice(
//              "1442880000000",231
//              ) },{ new HistoryPrice(
//              "1442966400000",231
//              ) },{ new HistoryPrice(
//              "1443052800000",231
//              ) },{ new HistoryPrice(
//              "1443139200000",231
//              ) },{ new HistoryPrice(
//              "1443225600000",231
//              ) },{ new HistoryPrice(
//              "1443312000000",231
//              ) },{ new HistoryPrice(
//              "1443398400000",231
//              ) },{ new HistoryPrice(
//              "1443484800000",231
//              ) },{ new HistoryPrice(
//              "1443571200000",231
//              ) },{ new HistoryPrice(
//              "1443657600000",231
//              ) },{ new HistoryPrice(
//              "1443744000000",232
//              ) },{ new HistoryPrice(
//              "1443830400000",233
//              ) },{ new HistoryPrice(
//              "1443916800000",233
//              ) },{ new HistoryPrice(
//              "1444003200000",233
//              ) },{ new HistoryPrice(
//              "1444089600000",233
//              ) },{ new HistoryPrice(
//              "1444176000000",232
//              ) },{ new HistoryPrice(
//              "1444262400000",232
//              ) },{ new HistoryPrice(
//              "1444348800000",232
//              ) },{ new HistoryPrice(
//              "1444435200000",232
//              ) },{ new HistoryPrice(
//              "1444521600000",232
//              ) },{ new HistoryPrice(
//              "1444608000000",232
//              ) },{ new HistoryPrice(
//              "1444694400000",231
//              ) },{ new HistoryPrice(
//              "1444780800000",231
//              ) },{ new HistoryPrice(
//              "1444867200000",231
//              ) },{ new HistoryPrice(
//              "1444953600000",230
//              ) },{ new HistoryPrice(
//              "1445040000000",230
//              ) },{ new HistoryPrice(
//              "1445126400000",230
//              ) },{ new HistoryPrice(
//              "1445212800000",230
//              ) },{ new HistoryPrice(
//              "1445299200000",230
//              ) },{ new HistoryPrice(
//              "1445385600000",230
//              ) },{ new HistoryPrice(
//              "1445472000000",231
//              ) },{ new HistoryPrice(
//              "1445558400000",231
//              ) },{ new HistoryPrice(
//              "1445644800000",231
//              ) },{ new HistoryPrice(
//              "1445731200000",231
//              ) },{ new HistoryPrice(
//              "1445817600000",231
//              ) },{ new HistoryPrice(
//              "1445904000000",230
//              ) },{ new HistoryPrice(
//              "1445990400000",231
//              ) },{ new HistoryPrice(
//              "1446076800000",231
//              ) },{ new HistoryPrice(
//              "1446163200000",231
//              ) },{ new HistoryPrice(
//              "1446249600000",232
//              ) },{ new HistoryPrice(
//              "1446336000000",232
//              ) },{ new HistoryPrice(
//              "1446422400000",232
//              ) },{ new HistoryPrice(
//              "1446508800000",232
//              ) },{ new HistoryPrice(
//              "1446595200000",232
//              ) },{ new HistoryPrice(
//              "1446681600000",232
//              ) },{ new HistoryPrice(
//              "1446768000000",232
//              ) },{ new HistoryPrice(
//              "1446854400000",233
//              ) },{ new HistoryPrice(
//              "1446940800000",234
//              ) },{ new HistoryPrice(
//              "1447027200000",234
//              ) },{ new HistoryPrice(
//              "1447113600000",235
//              ) },{ new HistoryPrice(
//              "1447200000000",236
//              ) },{ new HistoryPrice(
//              "1447286400000",238
//              ) },{ new HistoryPrice(
//              "1447372800000",239
//              ) },{ new HistoryPrice(
//              "1447459200000",240
//              ) },{ new HistoryPrice(
//              "1447545600000",241
//              ) },{ new HistoryPrice(
//              "1447632000000",242
//              ) },{ new HistoryPrice(
//              "1447718400000",243
//              ) },{ new HistoryPrice(
//              "1447804800000",243
//              ) },{ new HistoryPrice(
//              "1447891200000",244
//              ) },{ new HistoryPrice(
//              "1447977600000",244
//              ) },{ new HistoryPrice(
//              "1448064000000",244
//              ) },{ new HistoryPrice(
//              "1448150400000",244
//              ) },{ new HistoryPrice(
//              "1448236800000",244
//              ) },{ new HistoryPrice(
//              "1448323200000",244
//              ) },{ new HistoryPrice(
//              "1448409600000",245
//              ) },{ new HistoryPrice(
//              "1448496000000",245
//              ) },{ new HistoryPrice(
//              "1448582400000",245
//              ) },{ new HistoryPrice(
//              "1448668800000",245
//              ) },{ new HistoryPrice(
//              "1448755200000",245
//              ) },{ new HistoryPrice(
//              "1448841600000",245
//              ) },{ new HistoryPrice(
//              "1448928000000",245
//              ) },{ new HistoryPrice(
//              "1449014400000",245
//              ) },{ new HistoryPrice(
//              "1449100800000",244
//              ) },{ new HistoryPrice(
//              "1449187200000",245
//              ) },{ new HistoryPrice(
//              "1449273600000",245
//              ) },{ new HistoryPrice(
//              "1449360000000",245
//              ) },{ new HistoryPrice(
//              "1449446400000",244
//              ) },{ new HistoryPrice(
//              "1449532800000",244
//              ) },{ new HistoryPrice(
//              "1449619200000",244
//              ) },{ new HistoryPrice(
//              "1449705600000",244
//              ) },{ new HistoryPrice(
//              "1449792000000",244
//              ) },{ new HistoryPrice(
//              "1449878400000",243
//              ) },{ new HistoryPrice(
//              "1449964800000",243
//              ) },{ new HistoryPrice(
//              "1450051200000",243
//              ) },{ new HistoryPrice(
//              "1450137600000",242
//              ) },{ new HistoryPrice(
//              "1450224000000",242
//              ) },{ new HistoryPrice(
//              "1450310400000",243
//              ) },{ new HistoryPrice(
//              "1450396800000",243
//              ) },{ new HistoryPrice(
//              "1450483200000",243
//              ) },{ new HistoryPrice(
//              "1450569600000",243
//              ) },{ new HistoryPrice(
//              "1450656000000",243
//              ) },{ new HistoryPrice(
//              "1450742400000",243
//              ) },{ new HistoryPrice(
//              "1450828800000",243
//              ) },{ new HistoryPrice(
//              "1450915200000",243
//              ) },{ new HistoryPrice(
//              "1451001600000",242
//              ) },{ new HistoryPrice(
//              "1451088000000",241
//              ) },{ new HistoryPrice(
//              "1451174400000",240
//              ) },{ new HistoryPrice(
//              "1451260800000",239
//              ) },{ new HistoryPrice(
//              "1451347200000",238
//              ) },{ new HistoryPrice(
//              "1451433600000",238
//              ) },{ new HistoryPrice(
//              "1451520000000",237
//              ) },{ new HistoryPrice(
//              "1451606400000",236
//              ) },{ new HistoryPrice(
//              "1451692800000",235
//              ) },{ new HistoryPrice(
//              "1451779200000",234
//              ) },{ new HistoryPrice(
//              "1451865600000",232
//              ) },{ new HistoryPrice(
//              "1451952000000",231
//              ) },{ new HistoryPrice(
//              "1452038400000",230
//              ) },{ new HistoryPrice(
//              "1452124800000",230
//              ) },{ new HistoryPrice(
//              "1452211200000",230
//              ) },{ new HistoryPrice(
//              "1452297600000",229
//              ) },{ new HistoryPrice(
//              "1452384000000",229
//              ) },{ new HistoryPrice(
//              "1452470400000",229
//              ) },{ new HistoryPrice(
//              "1452556800000",228
//              ) },{ new HistoryPrice(
//              "1452643200000",228
//              ) },{ new HistoryPrice(
//              "1452729600000",227
//              ) },{ new HistoryPrice(
//              "1452816000000",227
//              ) },{ new HistoryPrice(
//              "1452902400000",227
//              ) },{ new HistoryPrice(
//              "1452988800000",226
//              ) },{ new HistoryPrice(
//              "1453075200000",225
//              ) },{ new HistoryPrice(
//              "1453161600000",225
//              ) },{ new HistoryPrice(
//              "1453248000000",224
//              ) },{ new HistoryPrice(
//              "1453334400000",224
//              ) },{ new HistoryPrice(
//              "1453420800000",224
//              ) },{ new HistoryPrice(
//              "1453507200000",225
//              ) },{ new HistoryPrice(
//              "1453593600000",226
//              ) },{ new HistoryPrice(
//              "1453680000000",227
//              ) },{ new HistoryPrice(
//              "1453766400000",228
//              ) },{ new HistoryPrice(
//              "1453852800000",228
//              ) },{ new HistoryPrice(
//              "1453939200000",229
//              ) },{ new HistoryPrice(
//              "1454025600000",230
//              ) },{ new HistoryPrice(
//              "1454112000000",231
//              ) },{ new HistoryPrice(
//              "1454198400000",232
//              ) },{ new HistoryPrice(
//              "1454284800000",233
//              ) },{ new HistoryPrice(
//              "1454371200000",234
//              ) },{ new HistoryPrice(
//              "1454457600000",235
//              ) },{ new HistoryPrice(
//              "1454544000000",236
//              ) },{ new HistoryPrice(
//              "1454630400000",237
//              ) },{ new HistoryPrice(
//              "1454716800000",238
//              ) },{ new HistoryPrice(
//              "1454803200000",238
//              ) },{ new HistoryPrice(
//              "1454889600000",239
//              ) },{ new HistoryPrice(
//              "1454976000000",240
//              ) },{ new HistoryPrice(
//              "1455062400000",241
//              ) },{ new HistoryPrice(
//              "1455148800000",243
//              ) },{ new HistoryPrice(
//              "1455235200000",245
//              ) },{ new HistoryPrice(
//              "1455321600000",247
//              ) },{ new HistoryPrice(
//              "1455408000000",248
//              ) },{ new HistoryPrice(
//              "1455494400000",250
//              ) },{ new HistoryPrice(
//              "1455580800000",251
//              ) },{ new HistoryPrice(
//              "1455667200000",253
//              ) },{ new HistoryPrice(
//              "1455753600000",254)
//              }
//        };
//    }
//}
