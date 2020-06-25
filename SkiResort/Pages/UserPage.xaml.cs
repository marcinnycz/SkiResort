using SkiResort.Pages.UserSubpages;
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
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        MySqlConnection connection;
        public UserPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }

        private void ReportListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            UserFrame.Content = new UserReportPage(connection);
        }

        private void UseListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            UserFrame.Content = new UsePage(connection);
        }

        private void BuyListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            UserFrame.Content = new BuyPage(connection);
        }

        private void RegisterListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            UserFrame.Content = new RegisterPage(connection);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UserListBox.SelectedItem = 0;
        }

        
    }
}
