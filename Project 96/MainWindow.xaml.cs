using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Primitives;
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
        public static Item currentItem;
        public bool JustLoaded = true;
        private int threshold = 10;
        public MainWindow()
        {
            InitializeComponent();

            currentItem = new Item(2);

            ItemID.Content = currentItem.ID;

            updateImage();

            List<History> priceHistory = new List<History>();

            if(currentItem.DailyList != null)
            {
                foreach (Item.HistoryPrice hist in currentItem.DailyList)
                {
                    long hold;
                    if (Int64.TryParse(hist.date, out hold))
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
            }

            PriceHistory.ItemsSource = priceHistory;
            

        }

        public void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToString();
            if (query != null && query != "")
            {
                try
                {
                    SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
                    con.Open();

                    string searchString = String.Format("select top({0}) name from dbo.smartSearch('{1}');", threshold, query);

                    SqlCommand cmd = new SqlCommand(searchString, con);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    string str = "";
                    var list = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList();

                    foreach (string s in list)
                    {
                        str += s + "\n";
                    }

                    if (list.Count > 0)
                    {
                        autofill.ItemsSource = list;
                        autofill.Visibility = Visibility.Visible;
                    }
                }
                catch
                {
                    MessageBox.Show("db error");
                }
            }
            else
            {
                autofill.ItemsSource = null;
                autofill.Visibility = Visibility.Collapsed;
            }
        }

        public void updateImage()
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(currentItem.ImageURL);
            bitmapImage.EndInit();
            icon.Source = bitmapImage;
        }

        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime / 1000);
        }

        private void LoadLineChartData(double view,int series)
        {
            Dictionary<DateTime, int> priceHistory = new Dictionary<DateTime, int>();
            if(series == 0 && currentItem.DailyList != null)
            {
                int startIndex = (currentItem.DailyList.Count - 7) - (Int32)(((currentItem.DailyList.Count - 7) * (view / 100)));
                //daily
                for (int i = startIndex; i < currentItem.DailyList.Count; i++)
                {
                    var hist = currentItem.DailyList[i];
                    long hold;
                    if (Int64.TryParse(hist.date, out hold))
                    {
                        var d = FromUnixTime(hold);
                        priceHistory.Add(d, hist.prc);
                    }
                    else
                    {
                        priceHistory.Add(new DateTime(), hist.prc);
                    }
                }
            }
            else if(currentItem.AverageList != null)
            {
                int startIndex = (currentItem.AverageList.Count - 7) - (Int32)(((currentItem.AverageList.Count - 7) * (view / 100)));
                //average
                for (int i = startIndex; i < currentItem.AverageList.Count; i++)
                {
                    var hist = currentItem.AverageList[i];
                    long hold;
                    if (Int64.TryParse(hist.date, out hold))
                    {
                        var d = FromUnixTime(hold);
                        priceHistory.Add(d, hist.prc);
                    }
                    else
                    {
                        priceHistory.Add(new DateTime(), hist.prc);
                    }
                }
            }
            ((LineSeries)PriceHistoryChart.Series[series]).ItemsSource = priceHistory;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JustLoaded = false;
            LoadLineChartData(50, 0);
            LoadLineChartData(50, 1);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!JustLoaded)
            {
                LoadLineChartData(HistorySlider.Value, 0); //min of 7 days
                LoadLineChartData(HistorySlider.Value, 1); //min of 7 days
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
                con.Open();

                string searchString = String.Format("select name, id, description, imageURL, membersOnly from ItemInfo where name = '{0}'", SearchBox.Text);

                SqlCommand cmd = new SqlCommand(searchString, con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var name = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList().First();
                var id = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("id")).ToList().First();
                var description = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("description")).ToList().First();
                var imageurl = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("imageURL")).ToList().First();
                var membersOnly = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<bool>("membersOnly")).ToList().First();
                Console.WriteLine();
                //currentItem = new Item(name, id, description, imageurl, ((membersOnly == 1) ? true : false));
                currentItem = new Item(name, id, description, imageurl, membersOnly);

                ItemID.Content = currentItem.ID;

                updateImage();

                LoadLineChartData(HistorySlider.Value, 0); //min of 7 days
                LoadLineChartData(HistorySlider.Value, 1); //min of 7 days

            }
            catch
            {
                MessageBox.Show("db error");
                throw new Exception();
            }

            //ItemID.Content = SearchBox.Text;
        }

        private void autofill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(autofill.ItemsSource != null)
            {
                autofill.Visibility = Visibility.Collapsed;
                SearchBox.TextChanged -= new TextChangedEventHandler(SearchBox_TextChanged);
                if(autofill.SelectedIndex != -1)
                {
                    SearchBox.Text = autofill.SelectedItem.ToString();
                }
                SearchBox.TextChanged += new TextChangedEventHandler(SearchBox_TextChanged);
            }
        }
    }

    public class History
    {
        public string date { get; set; }
        public int cost { get; set; }
    }

    public class ItemIconBinder : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(new Item(Int32.Parse(values[0].ToString())).ImageURL);
            bitmapImage.EndInit();
            return bitmapImage;
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemInfoBinder : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var self = values[1];
            string name = null;
            try
            {
                name = ((Label)self).Name;
            }
            catch
            {
                try {
                    name = ((TextBlock)self).Name;
                }
                catch
                {
                    MessageBox.Show("Parsing Name error");
                    throw;
                }
            }


            if (MainWindow.currentItem != null)
            {
                switch (name)
                {
                    case "ItemName":
                        return MainWindow.currentItem.Name;
                    case "ItemDesc":
                        return MainWindow.currentItem.Description;
                    case "ItemPrice":
                        return MainWindow.currentItem.Price;
                    case "MembersOnly":
                        return MainWindow.currentItem.membersonly;
                    default:
                        return String.Format("ERROR--x:Name={0} UNDEFINED", name);
                }
                throw new NotImplementedException();
            } else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
