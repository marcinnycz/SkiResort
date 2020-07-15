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
using System.Data;

namespace SkiResort.Pages.UserSubpages
{
    /// <summary>
    /// Interaction logic for UserReportPage.xaml
    /// </summary>
    public partial class UserReportPage : Page
    {
        MySqlConnection connection;
        //Data table for the data grid
        DataTable dt;

        //Main constructor
        public UserReportPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }

        //Check if the user pressed enter
        private void UserIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                updateCombo();
            }
        }

        private void UserIDTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            updateCombo();
        }

        //Function that fills the datagrid with data from the selected skipass
        private void SkiPassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SkiPassComboBox.SelectedIndex < 0)
            {
                return;
            }
            MySqlDataAdapter sda = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * from skiliftusage INNER JOIN skipass ON skiliftusage.SkiPass_skiPassID=skipass.skiPassID INNER JOIN skilift ON skilift.SkiLiftID = skiliftusage.SkiLift_skiLiftID where User_userID = @user AND skipass.skiPassID = @passid", connection);
            cmd.Parameters.Add(new MySqlParameter("@user", UserIDTextBox.Text));
            cmd.Parameters.Add(new MySqlParameter("@passid", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
            sda.SelectCommand = cmd;

            DataTable result_dt = new DataTable();
            sda.Fill(result_dt);
            ResultDataGrid.ItemsSource = result_dt.AsDataView();
            BeginsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"];
            EndsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"];
        }

        //Update the skipass combobox based on the given user ID
        private void updateCombo()
        {
            BeginsLabel.Content = "";
            EndsLabel.Content = "";

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
            }
        }
    }
}
