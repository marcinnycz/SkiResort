using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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

namespace SkiResort.Pages.UserSubpages
{
    /// <summary>
    /// Interaction logic for BuyPage.xaml
    /// </summary>
    public partial class BuyPage : Page
    {
        DataTable dt;
        MySqlConnection connection;
        public BuyPage(MySqlConnection _connection)
        {
            connection = _connection;       
            InitializeComponent();
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from passtype INNER JOIN pricelist ON passtype.passTypeID=pricelist.PassType_passTypeID where endDate IS NULL", connection);
            dt = new DataTable();
            sda.Fill(dt);
            PassTypeComboBox.ItemsSource = dt.AsDataView();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PassTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PriceLabel.Content = dt.Rows[(int)PassTypeComboBox.SelectedValue]["price"];
            PaymentMethodComboBox.IsEnabled = true;
            BuyButton.IsEnabled = true;
        }
    }
}
