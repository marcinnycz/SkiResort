﻿using System;
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
        //Data table for the datagrid
        DataTable dt;
        //Data table to store a copy of the database table for comparison
        DataTable dt_db;
        MySqlConnection connection;

        //Main constructor
        public ManagePage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
            UpdateGridFromDB();
        }

        //Function to refresh the data grid from the database
        private void UpdateGridFromDB()
        {
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from skilift", connection);
            dt = new DataTable();
            dt_db = new DataTable();
            sda.Fill(dt);
            sda.Fill(dt_db);
            PriceListDataGrid.ItemsSource = dt.AsDataView();
        }

        //Function saves the data to the database based on the changes in the datagrid
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {            
            connection.Open();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                //Check if the availability changed
                if(dt.Rows[i]["availability"].ToString() != dt_db.Rows[i]["availability"].ToString())
                {
                    //Check if lift was opened
                    if (dt.Rows[i]["availability"].ToString() == "1")
                    {
                        //Lift was opened
                        //Add new entry to lifthistory
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO `lifthistory` (openingDate, timesUsed, SkiLift_skiLiftID) values (@now, 0, @id)", connection);
                        cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[i]["skiLiftID"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        //Lift was closed
                        //Update and finish entry in lifthistory
                        MySqlCommand cmd = new MySqlCommand("UPDATE `lifthistory` SET closingDate = @now WHERE SkiLift_skiLiftID = @id AND closingDate IS NULL", connection);
                        cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[i]["skiLiftID"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                }

                //Update the database
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
