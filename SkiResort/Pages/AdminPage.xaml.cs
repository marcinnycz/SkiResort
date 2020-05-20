using SkiResort.Pages.AdminSubpages;
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

namespace SkiResort.Pages
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void ManageListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new ManagePage();
        }

        private void ReportListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new AdminReportPage();
        }

        private void PriceListListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new PriceListPage();
        }
    }
}
