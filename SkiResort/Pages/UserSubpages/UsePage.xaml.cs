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
using System.Data;
using MySql.Data.MySqlClient;

namespace SkiResort.Pages.UserSubpages
{
    /// <summary>
    /// Interaction logic for UsePage.xaml
    /// </summary>
    public partial class UsePage : Page
    {
        DataTable dt;
        MySqlConnection connection;
        public UsePage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }

        private void UserIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                updateCombo();
            }

        }

        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            if (dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"].ToString() == "")
            {
                MySqlCommand cmd = new MySqlCommand("SELECT minutes FROM passtype INNER JOIN skipass where skipass.PassType_passTypeID = passtype.passTypeID AND skipass.skiPassID = @id", connection);
                cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                int minutes = (int)rdr[0];
                rdr.Close();
                cmd = new MySqlCommand("UPDATE `skipass` SET startDate = @now, expiryDate = @later WHERE skiPassID = @id", connection);
                cmd.Parameters.Add(new MySqlParameter("@now", DateTime.Now));
                cmd.Parameters.Add(new MySqlParameter("@later", DateTime.Now.AddMinutes(minutes)));
                cmd.Parameters.Add(new MySqlParameter("@id", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
                cmd.ExecuteNonQuery();
                dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"] = DateTime.Now;
                dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"] = DateTime.Now.AddMinutes(minutes);
                useLift();
            } else
            {
                DateTime date = Convert.ToDateTime(dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"].ToString());
                DateTime now = DateTime.Now;
                if (DateTime.Compare(date, now) > 0)
                {
                    useLift();
                }else
                {
                    MessageLabel.Content = "Sorry, already expired!";
                }
            }
            connection.Close();
            updateLabels();
            
        }

        private void updateCombo()
        {
            MySqlDataAdapter sda = new MySqlDataAdapter();

            MySqlCommand cmd = new MySqlCommand("SELECT * from skipass INNER JOIN passtype ON passtype.passTypeID=skipass.PassType_passTypeID where User_userID = @user", connection);

            cmd.Parameters.Add(new MySqlParameter("@user", UserIDTextBox.Text));

            sda.SelectCommand = cmd;

            dt = new DataTable();
            sda.Fill(dt);
            SkiPassComboBox.ItemsSource = dt.AsDataView();
            if (SkiPassComboBox.Items.Count > 0)
            {
                SkiPassComboBox.IsEnabled = true;
                NoPassTextBox.Visibility = Visibility.Hidden;
            }
            else
            {
                SkiPassComboBox.IsEnabled = false;
                NoPassTextBox.Visibility = Visibility.Visible;
                UseButton.IsEnabled = false;
            }
        }

        private void UserIDTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            updateCombo();
        }

        private void SkiPassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateLabels();
        }

        private void updateLabels()
        {
            UseButton.IsEnabled = true;
            BeginsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"].ToString();
            EndsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"].ToString();
        }

        private void UseButton_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageLabel.Content = "";
        }

        private void useLift()
        {
            MessageLabel.Content = "Beep!";
        }
    }
}
