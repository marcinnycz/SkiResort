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

        //Main constructor
        public BuyPage(MySqlConnection _connection)
        {
            connection = _connection;       
            InitializeComponent();

            //Fill the combobox with pass types
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from passtype INNER JOIN pricelist ON passtype.passTypeID=pricelist.PassType_passTypeID where endDate IS NULL ORDER BY price", connection);
            dt = new DataTable();
            sda.Fill(dt);
            PassTypeComboBox.ItemsSource = dt.AsDataView();
            
        }

        //Function to facilitate buying a pass
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            //Insert a new pass
            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO `skipass` (startDate, expiryDate, User_userID, PassType_passTypeID, PassType_PriceList_priceListID) values (NULL, NULL, @user, @type, @price)";           
            cmd.Parameters.Add(new MySqlParameter("user", UserIDTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("type", dt.Rows[(int)PassTypeComboBox.SelectedIndex]["passTypeID"].ToString()));
            cmd.Parameters.Add(new MySqlParameter("price", dt.Rows[(int)PassTypeComboBox.SelectedIndex]["priceListID"].ToString()));
            cmd.ExecuteNonQuery();

            //Get the ID
            cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT skiPassID from skipass where User_userID = @user AND PassType_passTypeID = @type AND PassType_PriceList_priceListID = @price AND startDate IS NULL";
            cmd.Parameters.Add(new MySqlParameter("user", UserIDTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("type", dt.Rows[(int)PassTypeComboBox.SelectedIndex]["passTypeID"].ToString()));
            cmd.Parameters.Add(new MySqlParameter("price", dt.Rows[(int)PassTypeComboBox.SelectedIndex]["priceListID"].ToString()));
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int skipassid = (int)rdr[0];
            rdr.Close();

            //Insert a new payment
            cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO `payment` (date, paymentMethod, SkiPass_skiPassID) values (@date, @method, @pass)";
            cmd.Parameters.Add(new MySqlParameter("date", DateTime.Now));
            cmd.Parameters.Add(new MySqlParameter("method", PaymentMethodComboBox.Text));
            cmd.Parameters.Add(new MySqlParameter("pass", skipassid));
            cmd.ExecuteNonQuery();
            
            //Fill the message with a name
            cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT firstName, lastName from `user` where userID = @user ";
            cmd.Parameters.Add(new MySqlParameter("user", UserIDTextBox.Text));
       
            rdr = cmd.ExecuteReader();
            rdr.Read();
            string first = (string)rdr[0];
            string last = (string)rdr[1];
            rdr.Close();

            connection.Close();

            //Display the message
            MessageLabel.Content = "Successfully bought a " + PassTypeComboBox.Text + " pass for " + first + " " + last;
        }

        //Update the price if the combobox selection has changed
        private void PassTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PriceLabel.Content = dt.Rows[(int)PassTypeComboBox.SelectedIndex]["price"];
            PaymentMethodComboBox.IsEnabled = true;
        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuyButton.IsEnabled = true;
        }

        private void BuyButton_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageLabel.Content = "";
        }
    }
}
