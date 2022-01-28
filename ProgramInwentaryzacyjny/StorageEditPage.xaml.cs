﻿using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Logika interakcji dla klasy StorageEditPage.xaml
    /// </summary>
    public partial class StorageEditPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public StorageEditPage()
        {
            InitializeComponent();
            LoadProducts();
        }
        private void ConnectToDatabase()
        {
            sql_con = new SQLiteConnection(connection_string);
            sql_con.Open();
        }
        private void CloseConnection()
        {
            sql_con.Close();
        }
        private void LoadProducts()
        {
            string txtQuery = "Select * from Products";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
            dt = new DataTable("Products");
            dataAdapter.Fill(dt);
            ProduktDataGrid.ItemsSource = dt.DefaultView;
            CloseConnection();
        }
        private void txt_wyszukiwarka_KeyPress(object sender, KeyEventArgs e)
        {
            if (txt_wyszukiwarka.Text == string.Empty)
            {
                LoadProducts();
            }
            else
            {
                string txtQuery = string.Format("Select * from Products where Symbol like '%{0}%' or Nazwa_produktu like '%{0}%'", txt_wyszukiwarka.Text);
                sql_cmd = sql_con.CreateCommand();
                dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
                dt = new DataTable("Products");
                dataAdapter.Fill(dt);
                ProduktDataGrid.ItemsSource = dt.DefaultView;
                CloseConnection();
            }
        }
        private void SelectProcudct(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView dataRowView = dataGrid.SelectedItem as DataRowView;
            if (dataRowView != null)
            {
                txt_symbolEdit.Text = dataRowView["Symbol"].ToString();
                txt_nazwaEdit.Text = dataRowView["Nazwa_produktu"].ToString();
                txt_ilośćEdit.Text = dataRowView["Ilość"].ToString();
                txt_jednostkaEdit.Text = dataRowView["Jedn_miary"].ToString();
            }
        }
        private void ClearTxtBoxs()
        {
            txt_symbolEdit.Clear();
            txt_nazwaEdit.Clear();
            txt_ilośćEdit.Clear();
            txt_jednostkaEdit.Clear();
        }
        private void EditProduct(object sender, RoutedEventArgs e)
        {
            if (txt_symbolEdit.Text == string.Empty || txt_nazwaEdit.Text == string.Empty || txt_ilośćEdit.Text == string.Empty || txt_jednostkaEdit.Text == string.Empty)
            {
                MessageBox.Show("Parametry nie mogą być puste");
            }
            else
            {
                string txtQuery = "Update Products set Nazwa_produktu='" + txt_nazwaEdit.Text + "', Ilość='" + txt_ilośćEdit.Text + "', Jedn_miary='" + txt_jednostkaEdit.Text + "' where Symbol='" + txt_symbolEdit.Text + "'";
                ConnectToDatabase();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                CloseConnection();
                MessageBox.Show("Produkt zaaktualizowany");
                LoadProducts();
                ClearTxtBoxs();
            }
        }
    }
}
