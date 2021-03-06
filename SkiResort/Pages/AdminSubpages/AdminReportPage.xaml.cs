﻿using System;
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
        //Data table for the ComboBox
        DataTable dt;
        //Data table for the datagrid
        DataTable result_dt;
        MySqlConnection connection;

        //Main constructor
        public AdminReportPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();

            //Create select query
            MySqlDataAdapter sda = new MySqlDataAdapter();
            MySqlCommand cmd = new MySqlCommand("SELECT * from skilift", connection);

            //Fill data table for LiftComboBox
            sda.SelectCommand = cmd;
            dt = new DataTable();
            sda.Fill(dt);
            DataRow row = dt.NewRow();
            row["liftName"] = "Any";
            dt.Rows.Add(row);

            //Assign the data table to the combobox
            LiftComboBox.ItemsSource = dt.AsDataView();
            LiftComboBox.SelectedIndex = dt.Rows.Count - 1;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //Create select command
            MySqlDataAdapter sda = new MySqlDataAdapter();

            string command = "SELECT * from skilift INNER JOIN lifthistory ON skilift.skiLiftID = lifthistory.SkiLift_skiLiftID";

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;


            bool added = false;
            string append = "";
            
            //Add additional WHERE clauses based on UI inputs
            if(LiftComboBox.SelectedIndex >= 0 && LiftComboBox.Text != "Any")
            {
                append += "liftName = @name AND ";
                cmd.Parameters.Add(new MySqlParameter("@name", LiftComboBox.Text));
                added = true;
            }
            if (OpeningFromDatePicker.SelectedDate != null)
            {
                append += "openingDate >= @ofrom AND ";
                cmd.Parameters.Add(new MySqlParameter("@ofrom", OpeningFromDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd")));
                added = true;
            }
            if (OpeningToDatePicker.SelectedDate != null)
            {
                append += "openingDate <= @oto AND ";
                cmd.Parameters.Add(new MySqlParameter("@oto", OpeningToDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd") + " 23:59:59"));
                added = true;
            }
            if (ClosingFromDatePicker.SelectedDate != null)
            {
                append += "closingDate >= @cfrom AND ";
                cmd.Parameters.Add(new MySqlParameter("@cfrom", ClosingFromDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd")));
                added = true;
            }
            if (ClosingToDatePicker.SelectedDate != null)
            {
                append += "closingDate <= @cto AND ";
                cmd.Parameters.Add(new MySqlParameter("@cto", ClosingToDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd") + " 23:59:59"));
                added = true;
            }
            if (TimesFromTextBox.Text != "")
            {
                if (int.TryParse(TimesFromTextBox.Text, out int value))
                {
                    append += "timesUsed >= @tfrom AND ";
                    cmd.Parameters.Add(new MySqlParameter("@tfrom", TimesFromTextBox.Text));
                    added = true;
                }
                
            }
            if (TimesToTextBox.Text != "")
            {
                if (int.TryParse(TimesFromTextBox.Text, out int value))
                {
                    append += "timesUsed <= @tto AND ";
                    cmd.Parameters.Add(new MySqlParameter("@tto", TimesToTextBox.Text));
                    added = true;
                }               
            }


            if (added)
            {
                command += " WHERE " + append;
                command = command.Substring(0, command.Length - 5);
            }

            //Fill the data table
            cmd.CommandText = command;
            sda.SelectCommand = cmd;
            result_dt = new DataTable();
            sda.Fill(result_dt);

            //Fill the data grid
            LiftDataGrid.ItemsSource = result_dt.AsDataView();
        }
    }
}
