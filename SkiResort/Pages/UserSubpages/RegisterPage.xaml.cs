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

namespace SkiResort.Pages.UserSubpages
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        MySqlConnection connection;
        public RegisterPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO `user` (type, firstName, lastName, address, phoneNumber, email) values (@type, @first, @last, @adress, @phone, @email)";
            ComboBoxItem typeItem = (ComboBoxItem)RoleComboBox.SelectedItem;
            cmd.Parameters.Add(new MySqlParameter("type", typeItem.Content.ToString()));
            cmd.Parameters.Add(new MySqlParameter("first", FirstNameTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("last", LastNameTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("adress", AdressTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("phone", PhoneNumberTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("email", EmailTextBox.Text));

            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
