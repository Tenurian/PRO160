using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_96
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TesterItem testItem;
        public MainWindow()
        {
            InitializeComponent();
            testItem = new TesterItem();
            ItemID.Content = testItem.id;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(testItem.iconURL);
            bitmapImage.EndInit();
            icon.Source = bitmapImage;
            //we can deal with when the image is getting populated later
            /* Future code to try to bind it to the JSON object from the server */
            //            string GET(string url) 
            //{
            //                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //                try
            //                {
            //                    WebResponse response = request.GetResponse();
            //                    using (Stream responseStream = response.GetResponseStream())
            //                    {
            //                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            //                        return reader.ReadToEnd();
            //                    }
            //                }
            //                catch (WebException ex)
            //                {
            //                    WebResponse errorResponse = ex.Response;
            //                    using (Stream responseStream = errorResponse.GetResponseStream())
            //                    {
            //                        StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            //                        String errorText = reader.ReadToEnd();
            //                        // log errorText
            //                    }
            //                    throw;
            //                }
            //            }
            List<History> priceHistory = new List<History>();

            foreach(TesterItem.HistoryPrice hist in testItem.history)
            {
                long hold;
                if(Int64.TryParse(hist.date, out hold))
                {

                    //priceHistory.Add(new History() { date = String.Format("x{0}", hold), cost = hist.prc });
                    var d = FromUnixTime(hold);
                    priceHistory.Add(new History() { date = d.ToString(), cost = hist.prc });
                }
                else
                {
                    priceHistory.Add(new History() { date = hist.date, cost = hist.prc });
                }
            }

            PriceHistory.ItemsSource = priceHistory;
            

        }

        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime / 1000);
        }
      
    }

    public class History
    {
        public string date { get; set; }
        public int cost { get; set; }
    }

    public class ItemInfoBinder : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var self = values[1];

            var name = ((Label)self).Name;

            //we'd use the ItemID to get the info from the database maybe?
            TesterItem ti = new TesterItem();


            switch (name)
            {
                case "ItemName":
                    return ti.name;
                case "ItemDesc":
                    return ti.description;
                case "ItemPrice":
                    return ti.price;
                case "MembersOnly":
                    return ti.membersonly;
                default:
                    return String.Format("ERROR--x:Name={0} UNDEFINED", name);
            }
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemImageBinder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
