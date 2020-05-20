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

namespace SkiResort.Pages.AdminSubpages
{
    /// <summary>
    /// Interaction logic for ManagePage.xaml
    /// </summary>
    public partial class ManagePage : Page
    {
        public ManagePage()
        {
            InitializeComponent();
            LiftDataGrid.ItemsSource = Lift.GetEmployees();
        }
    }

    public class Lift
    {
        private string name;
        private bool operating;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public bool Operating
        {
            get { return operating; }
            set
            {
                operating = value;
            }
        }

        public static ObservableCollection<Lift> GetEmployees()
        {
            var lifts = new ObservableCollection<Lift>();

            lifts.Add(new Lift()
            {
                Name = "Green 1",
                Operating = true
            });

            lifts.Add(new Lift()
            {
                Name = "Blue 1",
                Operating = true
            });

            lifts.Add(new Lift()
            {
                Name = "Blue 2",
                Operating = true
            });

            lifts.Add(new Lift()
            {
                Name = "Red 1",
                Operating = false
            });

            lifts.Add(new Lift()
            {
                Name = "Black 1",
                Operating = true
            });
       
            return lifts;
        }

    }
}
