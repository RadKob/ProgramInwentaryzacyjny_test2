using Ganss.Excel;
using ProgramInwentaryzacyjny.Data;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace ProgramInwentaryzacyjny
{
    public partial class ImportPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        DataTable dt = new DataTable();
        public ImportPage()
        {
            InitializeComponent();
        }
        private void ConnectToDatabase()
        {
            sql_con = new SQLiteConnection(connection_string);
            sql_con.Open();
        }
        private void CloseConnection() { sql_con.Close(); }
        private void ImportCSV(object sender, RoutedEventArgs e)
        {
            ConnectToDatabase();
            string file = @"dostawa.xlsm";
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
                else if (CheckProductInBase(pt.Symbol) == false)
                {
                    MessageBox.Show("Produktu " + pt.Nazwa + " nie było w bazie więc zostanie dodany do niej automatycznie.");

                    string txtQuery = @"Insert into Products (Symbol, Nazwa_produktu, Jedn_miary) values ('" + pt.Symbol + "', '" + pt.Nazwa + "', '" + pt.Jedn + "');" +
                                        "Insert into Stan (Symbol, Ilość) values ('" + pt.Symbol + "', '" + pt.Ilość + "');";
                    ConnectToDatabase();
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = txtQuery;
                    sql_cmd.ExecuteNonQuery();
                    CloseConnection();
                }
            }
            CloseConnection();
            MessageBox.Show("Produkty zostały dodane");
        }
        private bool CheckProductInBase(string symbol)
        {
            string txtQuery = "Select Symbol from Stan where Symbol = '" + symbol + "';";
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
