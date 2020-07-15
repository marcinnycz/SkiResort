using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
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
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        MySqlConnection connection;

        //Main constructor
        public UsersPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
            UpdateGridFromDB();
        }

        //Function to refresh the data grid from the database
        private void UpdateGridFromDB()
        {
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from user", connection);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            UsersDataGrid.ItemsSource = dt.AsDataView();
        }
    }
}
