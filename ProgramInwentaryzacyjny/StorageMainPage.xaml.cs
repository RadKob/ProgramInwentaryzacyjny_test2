using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Logika interakcji dla klasy StorageMainPage.xaml
    /// </summary>
    public partial class StorageMainPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public StorageMainPage()
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
        private void ExecuteQuery(string txtQuery)
        {
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            CloseConnection();
        }
        private void LoadProducts()
        {
            try
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
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
    }
}
