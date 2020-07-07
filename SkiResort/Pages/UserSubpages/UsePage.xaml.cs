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
        DataTable dt_lift;
        MySqlConnection connection;
        public UsePage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();

            MySqlDataAdapter sda = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * from skilift", connection);
            dt_lift = new DataTable();
            sda.SelectCommand = cmd;
            sda.Fill(dt_lift);
            LiftComboBox.ItemsSource = dt_lift.AsDataView();
        }

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

        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkiPassComboBox.SelectedIndex < 0)
            {
                return;
            }
            connection.Open();
            if (dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"].ToString() == "")
            {
                //First time using the pass
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
            }
            connection.Close();
            useLift();    
            updateLabels(false);

            //User report update
            connection.Open();
            MySqlCommand cmd1 = new MySqlCommand("SELECT * FROM skiliftusage INNER JOIN skilift WHERE skiliftusage.SkiLift_skiLiftID = skilift.skiLiftID AND SkiPass_skiPassID = @id AND skilift.liftName = @name", connection);
            cmd1.Parameters.Add(new MySqlParameter("id", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
            cmd1.Parameters.Add(new MySqlParameter("name", LiftComboBox.Text));
            MySqlDataReader rdr1 = cmd1.ExecuteReader();
            int skiliftid;
            if(rdr1.Read())
            {
                //Used the lift before
                int used = (int)rdr1["timesUsed"];
                skiliftid = (int)rdr1["skiLiftID"];
                cmd1 = new MySqlCommand("UPDATE `skiliftusage` set lastTimeUsed = @now, timesUsed = @times WHERE SkiPass_skiPassID = @passid AND SkiLift_skiLiftID = @liftid", connection);
                cmd1.Parameters.Add(new MySqlParameter("@now", DateTime.Now));
                cmd1.Parameters.Add(new MySqlParameter("@times", ++used));
                cmd1.Parameters.Add(new MySqlParameter("@liftid", skiliftid));
                cmd1.Parameters.Add(new MySqlParameter("@passid", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
                rdr1.Close();
                cmd1.ExecuteNonQuery();
            }
            else
            {
                //Didnt use the lift before
                rdr1.Close();
                cmd1.CommandText = "SELECT skiLiftID FROM skilift WHERE liftName = @name";
                rdr1 = cmd1.ExecuteReader();
                rdr1.Read();
                
                skiliftid = (int)rdr1["skiLiftID"];
                cmd1 = new MySqlCommand("INSERT INTO `skiliftusage` (timesUsed, firstTimeUsed, lastTimeUsed, SkiLift_skiliftID, SkiPass_skiPassID) values (1, @now, @now, @liftid, @passid)", connection);
                cmd1.Parameters.Add(new MySqlParameter("@now", DateTime.Now));
                cmd1.Parameters.Add(new MySqlParameter("@liftid", skiliftid));
                cmd1.Parameters.Add(new MySqlParameter("@passid", dt.Rows[SkiPassComboBox.SelectedIndex]["skiPassID"].ToString()));
                rdr1.Close();
                cmd1.ExecuteNonQuery();
                        
            }
            connection.Close();


            //Admin report update
            connection.Open();
            MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM lifthistory INNER JOIN skilift WHERE lifthistory.SkiLift_skiLiftID = skilift.skiLiftID AND skilift.liftName = @name AND liftHistory.closingDate IS NULL", connection);
            cmd2.Parameters.Add(new MySqlParameter("name", LiftComboBox.Text));
            MySqlDataReader rdr2 = cmd2.ExecuteReader();
            rdr2.Read();
            
            int used2 = (int)rdr2["timesUsed"];
            cmd2 = new MySqlCommand("UPDATE `lifthistory` set timesUsed = @times WHERE SkiLift_skiLiftID = @liftid AND lifthistory.closingDate IS NULL", connection);
            cmd2.Parameters.Add(new MySqlParameter("@times", ++used2));
            cmd2.Parameters.Add(new MySqlParameter("@liftid", skiliftid));
            rdr2.Close();
            cmd2.ExecuteNonQuery();
            
            
            connection.Close();
        }

        private void updateCombo()
        {
            UseButton.IsEnabled = false;
            LiftComboBox.IsEnabled = false;

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

        private void SkiPassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UseButton.IsEnabled = false;
            if(SkiPassComboBox.SelectedIndex < 0)
            {
                return;
            }
            bool first = false;
            DateTime date;
            try
            {
                date = Convert.ToDateTime(dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"].ToString());
            }catch(Exception exception)
            {
                date = DateTime.Now;
                first = true;
            }
            
            DateTime now = DateTime.Now;
            if(!first)
            {
                if (DateTime.Compare(date, now) > 0)
                {
                    MessageLabel.Content = "";                    
                    LiftComboBox.IsEnabled = true;
                }
                else
                {
                    MessageLabel.Content = "Sorry, pass already expired!";                    
                    LiftComboBox.IsEnabled = false;
                }
            }else
            {
                MessageLabel.Content = "";                
                LiftComboBox.IsEnabled = true;
            }
            LiftComboBox.SelectedIndex = -1;
            updateLabels(first);         
        }

        private void updateLabels(bool first)
        {          
            if(!first)
            {
                BeginsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["startDate"].ToString();
                EndsLabel.Content = dt.Rows[SkiPassComboBox.SelectedIndex]["expiryDate"].ToString();
            }
            else
            {
                BeginsLabel.Content = "N/A";
                EndsLabel.Content = "N/A";
            }
            
        }

        private void UseButton_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageLabel.Content = "";
        }

        private void useLift()
        {
            MessageLabel.Content = "Beep!";
        }

        private void LiftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LiftComboBox.SelectedIndex < 0)
            {
                return;
            }
            if(dt_lift.Rows[LiftComboBox.SelectedIndex]["availability"].ToString() == "1")
            {
                MessageLabel.Content = "";
                UseButton.IsEnabled = true;
            }
            else
            {
                MessageLabel.Content = "Sorry, lift unavailable!";
                UseButton.IsEnabled = false;
            }
            
        }
    }
}
