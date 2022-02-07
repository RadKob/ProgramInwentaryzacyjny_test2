using IronPdf;
using ProgramInwentaryzacyjny.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


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
            SetTimeAndBackup();
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
        // raport produktow zerowych
        private void Raport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new RaportPage();
        }
        private void Administrator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdministratorPage();
        }
        private void Wyjście_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // kopia pliku bazy sqlite 15-go dnia
        private void SetTimeAndBackup()
        {
            DateTime copydaytime = DateTime.Today;
            DateTime thisDay = DateTime.Today;
            txt_datetime.Text = thisDay.ToString("d");
            if(copydaytime == thisDay)
            {
                MessageBox.Show("Wykonano automatyczną kopie zapasową bazy danych");
                string srcPath = "BazaDoProgramu.db";
                string dstPath = "BazaDoProgramu_copy.db";

                File.Copy(srcPath, dstPath, true);
            }
        }
    }
}
