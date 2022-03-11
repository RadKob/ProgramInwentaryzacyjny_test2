using Ganss.Excel;
using ProgramInwentaryzacyjny.Data;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Logika interakcji dla klasy ImportPage.xaml
    /// </summary>
    public partial class ImportPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        DataTable dt = new DataTable();

        // konstruktor strony
        public ImportPage()
        {
            InitializeComponent();
        }
        // polaczenie z baza
        private void ConnectToDatabase()
        {
            sql_con = new SQLiteConnection(connection_string);
            sql_con.Open();
        }
        // zamkniecie polaczenia
        private void CloseConnection() { sql_con.Close(); }
        // import dostawy z pliku excel
        private void ImportCSV(object sender, RoutedEventArgs e)
        {
            ConnectToDatabase();
            string file = @"test.xlsx";
            var product = new ExcelMapper(file).Fetch<Product>();
            foreach(var pt in product)
            {
                if (CheckProductInBase(pt.Symbol) == true)
                {
                    string txtQuery = "Update Stan set Ilość = (Select Ilość from Stan where Symbol = '" + pt.Symbol + "') + '" + pt.Ilość + "' where Symbol = '" + pt.Symbol + "';";
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = txtQuery;
                    sql_cmd.ExecuteNonQuery();
                }
                else MessageBox.Show("Takiego produktu nie ma w bazie");
            }
            CloseConnection();
            MessageBox.Show("Produkty zostały dodane");
        }
        // sprawdzenie czy produkt z dostawy jest w magazynie
        private bool CheckProductInBase(string symbol)
        {
            string txtQuery = "Select count(1) from Stan where Symbol = '" + symbol + "';";
            ConnectToDatabase();
            SQLiteCommand sql_cmd = new SQLiteCommand(txtQuery, sql_con);
            sql_cmd.ExecuteNonQuery();
            SQLiteDataReader dataReader = sql_cmd.ExecuteReader();
            int count = 0;
            while (dataReader.Read()) { count++; }
            if (count == 1){ return true; }
            else { return false; }
        }
    }
}
