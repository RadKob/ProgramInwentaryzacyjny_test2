using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;


namespace ProgramInwentaryzacyjny
{
    public partial class MainWindow : Window
    {
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
