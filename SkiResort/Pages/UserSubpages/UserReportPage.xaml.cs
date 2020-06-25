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
    /// Interaction logic for UserReportPage.xaml
    /// </summary>
    public partial class UserReportPage : Page
    {
        MySqlConnection connection;
        public UserReportPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
        }
    }
}
