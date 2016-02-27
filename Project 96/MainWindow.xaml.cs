using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
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
        public bool JustLoaded = true;
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

        private void LoadLineChartData(int startIndex)
        {
            Dictionary<DateTime, int> priceHistory = new Dictionary<DateTime, int>();

            //foreach (TesterItem.HistoryPrice hist in testItem.history)
            for(int i = startIndex; i < testItem.history.Count; i++)
            {
                var hist = testItem.history[i];
                long hold;
                if (Int64.TryParse(hist.date, out hold))
                {
                    var d = FromUnixTime(hold);
                    priceHistory.Add(d,hist.prc);
                }
                else
                {
                    priceHistory.Add(new DateTime(),hist.prc );
                }
            }

            ((LineSeries)PriceHistoryChart.Series[0]).ItemsSource = priceHistory;


            //((LineSeries)PriceHistoryChart.Series[0]).ItemsSource =
            //    new KeyValuePair<DateTime, int>[]{
            //new KeyValuePair<DateTime, int>(DateTime.Now, 100),
            //new KeyValuePair<DateTime, int>(DateTime.Now.AddMonths(1), 130),
            //new KeyValuePair<DateTime, int>(DateTime.Now.AddMonths(2), 150),
            //new KeyValuePair<DateTime, int>(DateTime.Now.AddMonths(3), 125),
            //new KeyValuePair<DateTime, int>(DateTime.Now.AddMonths(4),155) };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JustLoaded = false;
            LoadLineChartData((testItem.history.Count - 7) - (Int32)(((testItem.history.Count - 7) * (.5))));
            //const double margin = 10;
            //double xmin = margin;
            //double xmax = myCanvas.Width - margin;
            //double ymin = margin;
            //double ymax = myCanvas.Height - margin;
            //const double step = 10;

            //// Make the X axis.
            //GeometryGroup xaxis_geom = new GeometryGroup();
            //xaxis_geom.Children.Add(new LineGeometry(
            //    new Point(0, ymax), new Point(myCanvas.Width, ymax)));
            //for (double x = xmin + step;
            //    x <= myCanvas.Width - step; x += step)
            //{
            //    xaxis_geom.Children.Add(new LineGeometry(
            //        new Point(x, ymax - margin / 2),
            //        new Point(x, ymax + margin / 2)));
            //}

            //Path xaxis_path = new Path();
            //xaxis_path.StrokeThickness = 1;
            //xaxis_path.Stroke = Brushes.Black;
            //xaxis_path.Data = xaxis_geom;

            //myCanvas.Children.Add(xaxis_path);

            //// Make the Y ayis.
            //GeometryGroup yaxis_geom = new GeometryGroup();
            //yaxis_geom.Children.Add(new LineGeometry(
            //    new Point(xmin, 0), new Point(xmin, myCanvas.Height)));
            //for (double y = step; y <= myCanvas.Height - step; y += step)
            //{
            //    yaxis_geom.Children.Add(new LineGeometry(
            //        new Point(xmin - margin / 2, y),
            //        new Point(xmin + margin / 2, y)));
            //}

            //Path yaxis_path = new Path();
            //yaxis_path.StrokeThickness = 1;
            //yaxis_path.Stroke = Brushes.Black;
            //yaxis_path.Data = yaxis_geom;

            //myCanvas.Children.Add(yaxis_path);

            //// Make some data sets.
            //Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            //Random rand = new Random();
            //for (int data_set = 0; data_set < 2; data_set++)
            //{
            //    int last_y = rand.Next((int)ymin, (int)ymax);

            //    PointCollection points = new PointCollection();
            //    for (double x = xmin; x <= xmax; x += step)
            //    {
            //        //last_y = rand.Next(last_y - 10, last_y + 10);
            //        last_y = testItem.history[].prc;
            //        if (last_y < ymin) last_y = (int)ymin;
            //        if (last_y > ymax) last_y = (int)ymax;
            //        points.Add(new Point(x, last_y));
            //    }

            //    Polyline polyline = new Polyline();
            //    polyline.StrokeThickness = 1;
            //    polyline.Stroke = brushes[data_set];
            //    polyline.Points = points;

            //    myCanvas.Children.Add(polyline);
            //}
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!JustLoaded) {
                LoadLineChartData((testItem.history.Count - 7) - (Int32)(((testItem.history.Count - 7) * (HistorySlider.Value / 100)))); //min of 7 days
            }
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
