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
using MySql.Data.MySqlClient;

namespace SkiResort.Pages
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        MySqlConnection connection;

        public AdminPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }

        private void ManageListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new ManagePage(connection);
        }

        private void ReportListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new AdminReportPage(connection);
        }

        private void PriceListListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new PriceListPage(connection);
        }

        private void UsersListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            AdminFrame.Content = new UsersPage(connection);
        }
    }
}
