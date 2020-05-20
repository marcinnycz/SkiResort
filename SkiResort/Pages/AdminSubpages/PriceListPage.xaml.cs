using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SkiResort.Pages.AdminSubpages
{
    /// <summary>
    /// Interaction logic for PriceListPage.xaml
    /// </summary>
    public partial class PriceListPage : Page
    {
        public PriceListPage()
        {
            InitializeComponent();
            PriceListDataGrid.ItemsSource = PriceListEntry.GetEmployees();
        }
    }
    public class PriceListEntry
    {
        private string name;
        private string price;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;            
            }
        }

        public string Price
        {
            get { return price; }
            set
            {
                price = value;
            }
        }

        public static ObservableCollection<PriceListEntry> GetEmployees()
        {
            var priceList = new ObservableCollection<PriceListEntry>();

            priceList.Add(new PriceListEntry()
            {
                Name = "100 points",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "200 points",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "500 points",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "1000 points",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "1 hour",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "2 hours",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "4 hours",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "1 day",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "3 days",

            });

            priceList.Add(new PriceListEntry()
            {
                Name = "1 week",

            });

            return priceList;
        }

    }
        
}
