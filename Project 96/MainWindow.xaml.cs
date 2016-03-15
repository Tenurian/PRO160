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
using System.Threading;
using System.Linq.Expressions;

namespace Project_96
{

    public partial class MainWindow : Window
    {
        static object locker = new object();
        public static int MIN = 1000, MAX = 10000, count = 5, days = 5; //these will be changed via bindings later
        public static Item currentItem;
        public List<ADVISRItem> RISEN, FALLEN;

        public bool JustLoaded = true;
        private int threshold = 10;
        public MainWindow()
        {
            InitializeComponent();
            currentItem = new Item(4151);
            RISEN = new List<ADVISRItem>();
            FALLEN = new List<ADVISRItem>();

            Thread t1 = new Thread(new ThreadStart(this.ADVISR1));
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(this.ADVISR2));
            t2.Start();

            ItemID.Content = currentItem.ID;
            updateImage();

            Dictionary<DateTime, int> priceHistory = new Dictionary<DateTime, int>();
            if (currentItem.DailyList != null)
            {
                foreach (Item.HistoryPrice hist in currentItem.DailyList)
                {
                    long hold;
                    if (Int64.TryParse(hist.date, out hold))
                    {
                        var d = FromUnixTime(hold);
                        priceHistory.Add(d, hist.prc);
                    }
                    else if (Convert.ToDateTime(hist.date.ToString()) != null)
                    {
                        var c = Convert.ToDateTime(hist.date.ToString());
                        priceHistory.Add(c, hist.prc);
                    }
                    else
                    {
                        priceHistory.Add(new DateTime(), hist.prc);
                    }
                }
            }

            PriceHistory.ItemsSource = priceHistory;


        }

        public class ADVISRItem
        {
            public readonly string name;
            public readonly int id, price, CURRENT_TREND_DIRECTION, PREVIOUS_TREND_DIRECTION, Direction_Change;
            public readonly decimal CHANGE_IN_DIFF;

            public ADVISRItem(string name, int id, decimal CHANGE_IN_DIFF, int Direction_Change) //for the advisr tab
            {
                this.name = name;
                this.id = id;
                this.CHANGE_IN_DIFF = CHANGE_IN_DIFF;
                this.Direction_Change = Direction_Change;
            }

            public ADVISRItem(string name, int id, decimal CHANGE_IN_DIFF, int CURRENT_TREND_DIRECTION, int PREVIOUS_TREND_DIRECTION, int Direction_Change) //for the advisr section in historics tab
            {
                this.name = name;
                this.id = id;
                this.CHANGE_IN_DIFF = CHANGE_IN_DIFF;
                this.CURRENT_TREND_DIRECTION = CURRENT_TREND_DIRECTION;
                this.PREVIOUS_TREND_DIRECTION = PREVIOUS_TREND_DIRECTION;
                this.Direction_Change = Direction_Change;
            }
        }

        public void UpdateADVISRTab(int list, string list1)
        {
            lock (locker)
            {
                if (list == 0)
                {

                    Action l1 = () => ADVISR_LIST1.Content = list1;
                    Dispatcher.Invoke(l1);
                }
                else
                {
                    Action l1 = () => ADVISR_LIST2.Content = list1;
                    Dispatcher.Invoke(l1);
                }
            }
        }
        public void UpdateADVISRTab(int list, List<ADVISRItem> list1)
        {
            lock (locker)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    List<Label> lizst = new List<Label>();
                    foreach (ADVISRItem i in list1)
                    {
                        Label l = new Label();
                        l.Content = String.Format("ID: {0} :: Name: {1} :: CID: {2} :: DCW: {3} :: Price: {4}\n", i.id, i.name, i.CHANGE_IN_DIFF, i.Direction_Change, new Item(i.id).Price);
                        lizst.Add(l);
                    }
                    Console.WriteLine();
                    foreach(Label l in lizst)
                    {
                        if (list == 0)
                        {
                            RISEN_STACK.Children.Add(l);
                            //Action l1 = () => ADVISR_LIST1.Content = list1;
                            //Dispatcher.Invoke(l1);
                        }
                        else
                        {
                            FALLEN_STACK.Children.Add(l);
                            //Action l1 = () => ADVISR_LIST2.Content = list1;
                            //Dispatcher.Invoke(l1);
                        }
                        var x = FALLEN_STACK.Children;
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                });
            }
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

        public void ADVISR1()
        {
            RISEN = new List<ADVISRItem>();
            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();
            string Risen_Query = String.Format("SELECT TOP ({2}) A.* FROM ADVISR_FULL({0}, {1}) AS A ORDER BY A.DIRECTION_CHANGE_WEIGHT DESC, A.CHANGE_IN_DIFF DESC", MIN, MAX, count);
            SqlCommand risen_cmd = new SqlCommand(Risen_Query, con);
            SqlDataAdapter risen_adapter = new SqlDataAdapter(risen_cmd);
            DataTable risen_dt = new DataTable();
            risen_adapter.Fill(risen_dt);
            List<Int32> Risen_IDs = risen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("ID")).ToList();
            List<string> Risen_NAMES = risen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("NAME")).ToList();
            List<decimal> Risen_CIDs = risen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<decimal>("CHANGE_IN_DIFF")).ToList();
            List<Int32> Risen_DCWs = risen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("DIRECTION_CHANGE_WEIGHT")).ToList();
            string risen_stats = "";
            for (int i = 0; i < count; i++)
            {
                RISEN.Add(
                    new ADVISRItem(
                        Risen_NAMES[i],
                        Risen_IDs[i],
                        Risen_CIDs[i],
                        Risen_DCWs[i]
                        )
                    );
                var r = RISEN[i];
                risen_stats += String.Format("ID: {0} :: Name: {1} :: CID: {2} :: DCW: {3} :: Price: {4}\n", r.id, r.name, r.CHANGE_IN_DIFF, r.Direction_Change, new Item(r.id).Price);
            }
            UpdateADVISRTab(0, risen_stats);
            UpdateADVISRTab(0, RISEN);
        }

        public void ADVISR2()
        {
            FALLEN = new List<ADVISRItem>();
            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();
            string Fallen_Query = String.Format("SELECT TOP ({2}) A.* FROM ADVISR_FULL({0}, {1}) AS A ORDER BY A.DIRECTION_CHANGE_WEIGHT ASC, A.CHANGE_IN_DIFF DESC", MIN, MAX, count);
            SqlCommand fallen_cmd = new SqlCommand(Fallen_Query, con);
            SqlDataAdapter fallen_adapter = new SqlDataAdapter(fallen_cmd);
            DataTable fallen_dt = new DataTable();
            fallen_adapter.Fill(fallen_dt);
            List<Int32> Fallen_IDs = fallen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("ID")).ToList();
            List<string> Fallen_NAMES = fallen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("NAME")).ToList();
            List<decimal> Fallen_CIDs = fallen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<decimal>("CHANGE_IN_DIFF")).ToList();
            List<Int32> Fallen_DCWs = fallen_dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("DIRECTION_CHANGE_WEIGHT")).ToList();
            string fallen_stats = "";
            for (int i = 0; i < count; i++)
            {
                FALLEN.Add(
                    new ADVISRItem(
                        Fallen_NAMES[i],
                        Fallen_IDs[i],
                        Fallen_CIDs[i],
                        Fallen_DCWs[i]
                        )
                    );
                var f = FALLEN[i];
                fallen_stats += String.Format("ID: {0} :: Name: {1} :: CID: {2} :: DCW: {3} :: Price: {4}\n", f.id, f.name, f.CHANGE_IN_DIFF, f.Direction_Change, new Item(f.id).Price);
            }
            UpdateADVISRTab(1, fallen_stats);
            UpdateADVISRTab(1, FALLEN);
        }

        public void updateImage()
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(currentItem.ImageURL);
            bitmapImage.EndInit();
            icon.Source = bitmapImage;
            Console.WriteLine();
            SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
            con.Open();
            Console.WriteLine();
            string searchString = String.Format("SELECT name, id, CHANGE_IN_DIFF, CURRENT_TREND_DIRECTION, PREVIOUS_TREND_DIRECTION, Direction_Change FROM ADVISR3({0}, {1}, {2})", currentItem.ID, days, currentItem.Price);
            SqlCommand cmd = new SqlCommand(searchString, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Console.WriteLine();
            var advsirItem = new ADVISRItem(dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList().First(),
                dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("id")).ToList().First(),
                dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Decimal>("CHANGE_IN_DIFF")).ToList().First(),
                dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("CURRENT_TREND_DIRECTION")).ToList().First(),
                dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("PREVIOUS_TREND_DIRECTION")).ToList().First(),
                dt.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("Direction_Change")).ToList().First());
            Console.WriteLine();
            string info = String.Format("Name: {0}\nID: {1}\nCHANGE_IN_DIFF: {2}\nCURRENT_TREND_DIRECTION: {3}\nPREVIOUS_TREND_DIRECTION: {4}\nDirection_Change: {5}",
                advsirItem.name,
                advsirItem.id,
                advsirItem.CHANGE_IN_DIFF,
                advsirItem.CURRENT_TREND_DIRECTION,
                advsirItem.PREVIOUS_TREND_DIRECTION,
                advsirItem.Direction_Change
                );
            Console.WriteLine();
            ADVISR_CONTENT.Content = info;
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
                    else if(Convert.ToDateTime(hist.date.ToString()) != null)
                    {
                        var c = Convert.ToDateTime(hist.date.ToString());
                        priceHistory.Add(c, hist.prc);
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
                    else if (Convert.ToDateTime(hist.date.ToString()) != null)
                    {
                        var c = Convert.ToDateTime(hist.date.ToString());
                        priceHistory.Add(c, hist.prc);
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
            if(SearchBox.Text != null && SearchBox.Text != "" && SearchBox.Text != currentItem.Name)
            {
                try
                {
                    SqlConnection con = new SqlConnection(@"Data Source=TENURIANS_ROG;Initial Catalog=Osiris;User ID=osiris_user;Password=3p8%7r7k9#2i");
                    con.Open();

                    string hold = SearchBox.Text;
                    hold = hold.Replace("'", "''");

                    string bortString = String.Format("select * from ItemInfo where name = '{0}'", hold);
                    SqlCommand bort = new SqlCommand(bortString, con);
                    SqlDataAdapter bortDA = new SqlDataAdapter(bort);
                    DataTable bortDT = new DataTable();
                    bortDA.Fill(bortDT);


                    Console.WriteLine();
                    var bortlist = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList();


                    if (bortlist.Count != 0)
                    {
                        var name = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList().First();
                        var id = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<Int32>("id")).ToList().First();
                        var description = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("description")).ToList().First();
                        var imageurl = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("imageURL")).ToList().First();
                        var membersOnly = bortDT.Rows.OfType<DataRow>().Select(dr => dr.Field<bool>("membersOnly")).ToList().First();
                        Console.WriteLine();
                        //currentItem = new Item(name, id, description, imageurl, ((membersOnly == 1) ? true : false));
                        currentItem = new Item(name, id, description, imageurl, membersOnly);

                        ItemID.Content = currentItem.ID;

                        updateImage();

                        LoadLineChartData(HistorySlider.Value, 0); //min of 7 days
                        LoadLineChartData(HistorySlider.Value, 1); //min of 7 days
                    }
                    else
                    {

                        string preSearchString = String.Format("select top(1) name from dbo.smartSearch('{0}');", hold);

                        SqlCommand pcmd = new SqlCommand(preSearchString, con);

                        SqlDataAdapter pda = new SqlDataAdapter(pcmd);
                        DataTable pdt = new DataTable();
                        pda.Fill(pdt);
                        var list = pdt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("name")).ToList();

                        var xt = list.First().Replace("'", "''");
                        string searchString = String.Format("select name, id, description, imageURL, membersOnly from ItemInfo where name = '{0}'", list.First().Replace("'", "''"));

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
                }
                catch
                {
                    MessageBox.Show("db error");
                    throw new Exception();
                }

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
