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

namespace SkiResort.Pages.AdminSubpages
{
    /// <summary>
    /// Interaction logic for AdminReportPage.xaml
    /// </summary>
    public partial class AdminReportPage : Page
    {
        DataTable dt;
        DataTable result_dt;
        MySqlConnection connection;

        public AdminReportPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();

            MySqlDataAdapter sda = new MySqlDataAdapter();

            MySqlCommand cmd = new MySqlCommand("SELECT * from skilift", connection);


            sda.SelectCommand = cmd;

            dt = new DataTable();
            sda.Fill(dt);
            DataRow row = dt.NewRow();
            row["liftName"] = "Any";
            dt.Rows.Add(row);

            LiftComboBox.ItemsSource = dt.AsDataView();
            LiftComboBox.SelectedIndex = dt.Rows.Count - 1;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
            MySqlDataAdapter sda = new MySqlDataAdapter();

            string command = "SELECT * from skilift INNER JOIN lifthistory ON skilift.skiLiftID = lifthistory.SkiLift_skiLiftID";

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;


            bool added = false;
            string append = "";
            
            if(LiftComboBox.SelectedIndex >= 0 && LiftComboBox.Text != "Any")
            {
                append += "liftName = @name AND ";
                cmd.Parameters.Add(new MySqlParameter("@name", LiftComboBox.Text));
                added = true;
            }
            if (OpeningFromDatePicker.SelectedDate != null)
            {
                append += "openingDate >= @from AND ";
                cmd.Parameters.Add(new MySqlParameter("@from", OpeningFromDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd")));
                added = true;
            }
            if (OpeningToDatePicker.SelectedDate != null)
            {
                append += "openingDate <= @to AND ";
                cmd.Parameters.Add(new MySqlParameter("@to", OpeningToDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd") + " 23:59:59"));
                added = true;
            }
            if (ClosingFromDatePicker.SelectedDate != null)
            {
                append += "closingDate >= @from AND ";
                cmd.Parameters.Add(new MySqlParameter("@from", ClosingFromDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd")));
                added = true;
            }
            if (ClosingToDatePicker.SelectedDate != null)
            {
                append += "closingDate <= @to AND ";
                cmd.Parameters.Add(new MySqlParameter("@to", ClosingToDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd") + " 23:59:59"));
                added = true;
            }
            if (TimesFromTextBox.Text != "")
            {
                if (int.TryParse(TimesFromTextBox.Text, out int value))
                {
                    append += "timesUsed >= @from AND ";
                    cmd.Parameters.Add(new MySqlParameter("@from", TimesFromTextBox.Text));
                    added = true;
                }
                
            }
            if (TimesToTextBox.Text != "")
            {
                if (int.TryParse(TimesFromTextBox.Text, out int value))
                {
                    append += "timesUsed <= @to AND ";
                    cmd.Parameters.Add(new MySqlParameter("@to", TimesToTextBox.Text));
                    added = true;
                }               
            }


            if (added)
            {
                command += " WHERE " + append;
                command = command.Substring(0, command.Length - 5);
            }



            cmd.CommandText = command;
            sda.SelectCommand = cmd;
            result_dt = new DataTable();
            sda.Fill(result_dt);

            LiftDataGrid.ItemsSource = result_dt.AsDataView();
        }
    }
}
