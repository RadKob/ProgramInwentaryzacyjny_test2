using ProgramInwentaryzacyjny.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;


namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private void ConnectToDatabase()
        {
            sql_con = new SQLiteConnection(connection_string);
            sql_con.Open();
        }
        private void CloseConnection()
        {
            sql_con.Close();
        }
        public MainWindow()
        {
            InitializeComponent();
            SetTimwAndBackup();
        }
        private void Magazyn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StorageMainPage();
        }
        private void Aktualizacja_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StorageEditPage();
        }
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ImportPage();
        }
        private void Raport_Click(object sender, RoutedEventArgs e)
        {
            string txtQuery = "Select * from Products where Ilość != 0";
            ConnectToDatabase();
            sql_cmd = new SQLiteCommand(txtQuery, sql_con);
            SQLiteDataReader dr = sql_cmd.ExecuteReader();
            List<Product> products_list = new List<Product>();
            while (dr.Read())
            {
                Product product = new Product(dr["Symbol"].ToString(),dr["Nazwa_produktu"].ToString(),Convert.ToInt32(dr["Ilość"]),dr["Jedn_miary"].ToString());
                products_list.Add(product);
            }
            CloseConnection();
        }
        private void Administrator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdministratorPage();
        }
        private void Wyjście_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SetTimwAndBackup()
        {
            DateTime thisDay = DateTime.Today;
            txt_datetime.Text = thisDay.ToString("d");
            if(thisDay.Day == 15)
            {
                string srcPath = "BazaDoProgramu.db";
                string dstPath = "BazaDoProgramu_copy.db";

                File.Copy(srcPath, dstPath, true);
            }
        }
    }
}
