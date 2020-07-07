using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


namespace SkiResort.Pages.AdminSubpages
{
    /// <summary>
    /// Interaction logic for ManagePage.xaml
    /// </summary>
    public partial class ManagePage : Page
    {
        DataTable dt;
        DataTable dt_db;
        MySqlConnection connection;
        public ManagePage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
            UpdateGridFromDB();
        }

        private void UpdateGridFromDB()
        {
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from skilift", connection);
            dt = new DataTable();
            dt_db = new DataTable();
            sda.Fill(dt);
            sda.Fill(dt_db);
            PriceListDataGrid.ItemsSource = dt.AsDataView();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                if(dt.Rows[i]["availability"].ToString() != dt_db.Rows[i]["availability"].ToString())
                {
                    if (dt.Rows[i]["availability"].ToString() == "1")
                    {
                        //Add new entry to lifthistory
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `lifthistory` (openingDate, timesUsed, SkiLift_skiLiftID) values (@now, 0, @id)", connection);
                        cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[i]["skiLiftID"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        //Update and finish entry in lifthistory
                        MySqlCommand cmd = new MySqlCommand("UPDATE `lifthistory` SET closingDate = @now WHERE SkiLift_skiLiftID = @id AND closingDate IS NULL", connection);
                        cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[i]["skiLiftID"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }

                MySqlCommand cmd1 = new MySqlCommand("UPDATE `skilift` SET availability = @availability WHERE skiLiftID = @id", connection);
                cmd1.Parameters.Add(new MySqlParameter("availability", dt.Rows[i]["availability"].ToString()));
                cmd1.Parameters.Add(new MySqlParameter("id", dt.Rows[i]["skiLiftID"].ToString()));
                cmd1.ExecuteNonQuery();
            }
            connection.Close();
            MessageLabel.Content = "Succesfully saved to database.";
            UpdateGridFromDB();
        }

        private void SaveButton_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageLabel.Content = "";
        }
    }   
}
