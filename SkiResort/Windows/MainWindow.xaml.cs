using SkiResort.Pages;
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

namespace SkiResort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string cs = @"server=localhost;userid=root;password=marcinnycz;database=skiliftdb";
        MySqlConnection con;
        
        public MainWindow()
        {
            InitializeComponent();
            con = new MySqlConnection(cs);
            con.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * from `user`";
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
               
                    Console.WriteLine(rdr[0] + " -- " + rdr[1]);
                
            }
            rdr.Close();
            Console.WriteLine($"MySQL version : {con.ServerVersion}");
            con.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //this.Owner.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Owner.Hide();
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdminPage(con);
            AdminButton.IsEnabled = false;
            UserButton.IsEnabled = true;
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new UserPage(con);
            AdminButton.IsEnabled = true;
            UserButton.IsEnabled = false;
        }

        private void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdminPage(con);
            AdminButton.IsEnabled = false;
        }
    }
}
