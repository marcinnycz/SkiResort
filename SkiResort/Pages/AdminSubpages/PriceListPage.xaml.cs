﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace SkiResort.Pages.AdminSubpages
{
    /// <summary>
    /// Interaction logic for PriceListPage.xaml
    /// </summary>
    public partial class PriceListPage : Page
    {
        MySqlConnection connection;
        //Data table for the datagrid
        DataTable dt;
        //Data table to store a copy of the database table for comparison
        DataTable dt_db;

        //Main constructor
        public PriceListPage(MySqlConnection _connection)
        {
            connection = _connection;
            InitializeComponent();
            UpdateGridFromDB();
        }

        //Function to refresh the data grid from the database
        private void UpdateGridFromDB()
        {
            MySqlDataAdapter sda = new MySqlDataAdapter("SELECT * from passtype INNER JOIN pricelist ON passtype.passTypeID=pricelist.PassType_passTypeID where endDate IS NULL", connection);
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

            bool found = false;
            int foundID = -1;
            
            //Check for every row in the database
            for (int j = 0; j < dt_db.Rows.Count; j++)
            {
                //Check for every row in the datagrid
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Check if the row exists
                    if (dt.Rows[i]["PassType_passTypeID"].ToString() == dt_db.Rows[j]["PassType_passTypeID"].ToString())
                    {
                        found = true;
                        foundID = i;
                        break;
                    }
                }
                if (found)
                {
                    //The row exists
                    //If the price has changed
                    if(dt.Rows[foundID]["price"].ToString() != dt_db.Rows[j]["price"].ToString())
                    {
                        //Update the database entry
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = "UPDATE `pricelist` SET endDate = @now WHERE PassType_passTypeID = @id AND endDate IS NULL";
                        cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("id", dt.Rows[foundID]["PassType_passTypeID"].ToString()));
                        cmd.ExecuteNonQuery();

                        //Insert new row
                        cmd.CommandText = "INSERT INTO `pricelist` (price, startDate, PassType_passTypeID) values (@price, @start, @pass)";
                        cmd.Parameters.Add(new MySqlParameter("price", dt.Rows[foundID]["price"].ToString()));
                        cmd.Parameters.Add(new MySqlParameter("start", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("pass", dt.Rows[foundID]["PassType_passTypeID"].ToString()));
                        cmd.ExecuteNonQuery();
                    }
                    found = false;
                    foundID = -1;
                }
                else
                {
                    //The row was deleted
                    //Update the database entry
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "UPDATE `pricelist` SET endDate = @now WHERE PassType_passTypeID = @id AND endDate IS NULL";
                    cmd.Parameters.Add(new MySqlParameter("now", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("id", dt_db.Rows[j]["PassType_passTypeID"].ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            //Check for new rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if(dt.Rows[i]["priceListID"].ToString() == "")
                {
                    //Added new row
                    //Add to passtype
                    int passid;

                    //Get the ID
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT passTypeID from passtype where name = @name";
                    cmd.Parameters.Add(new MySqlParameter("name", dt.Rows[i]["name"]));
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if(!rdr.Read())
                    {
                        //Entry doesn't exist - insert
                        rdr.Close();
                        cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = "INSERT INTO passtype (name, minutes) values (@name, @minutes)";
                        cmd.Parameters.Add(new MySqlParameter("name", dt.Rows[i]["name"]));
                        cmd.Parameters.Add(new MySqlParameter("minutes", dt.Rows[i]["minutes"]));
                        cmd.ExecuteNonQuery();

                        //Get passtypeID
                        cmd = new MySqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT passTypeID from passtype where name = @name";
                        cmd.Parameters.Add(new MySqlParameter("name", dt.Rows[i]["name"]));
                        rdr = cmd.ExecuteReader();
                        rdr.Read();
                        passid = (int)rdr[0];
                        rdr.Close();
                    }
                    else
                    {
                        //Get passtypeID
                        passid = (int)rdr[0];
                        rdr.Close();  
                    }

                    //Add to pricelist
                    cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO pricelist (price, startDate, PassType_passTypeID) values (@price, @start, @pass)";
                    cmd.Parameters.Add(new MySqlParameter("price", dt.Rows[i]["price"]));
                    cmd.Parameters.Add(new MySqlParameter("start", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("pass", passid));
                    cmd.ExecuteNonQuery();
                    
                }
            }
            connection.Close();
            MessageLabel.Content = "Successfully saved to database!";
            UpdateGridFromDB();
        }

        //Function adds a row to the datagrid. Not yet saved to the database
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow workRow = dt.NewRow();
            bool exists = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if(dt.Rows[i]["name"].ToString() == NameTextBox.Text)
                {
                    exists = true;
                }
            }
            if(exists == false)
            {
                if(int.TryParse(PriceTextBox.Text, out int value) && int.TryParse(MinutesTextBox.Text, out int value2) && NameTextBox.Text != "")
                {
                    workRow["name"] = NameTextBox.Text;
                    workRow["price"] = PriceTextBox.Text;
                    workRow["minutes"] = MinutesTextBox.Text;
                    dt.Rows.Add(workRow);
                }
                else
                {
                    MessageLabel.Content = "Please provide viable data.";
                }

            }
            else
            {
                MessageLabel.Content = "Name already exists!";
            }
           
        }

        //Removes the selected row from the datagrid
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow selectedItem = (DataRow)((DataRowView)PriceListDataGrid.SelectedItem).Row;
            if (selectedItem != null)
            {
                dt.Rows.Remove(selectedItem);
            }
        }

        private void ClearMessage()
        {
            MessageLabel.Content = "";
        }

        private void Button_LostFocus(object sender, RoutedEventArgs e)
        {
            ClearMessage();
        }
    }    
}
