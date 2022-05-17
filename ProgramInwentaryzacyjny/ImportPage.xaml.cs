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
                if (CheckProductInBase(pt.Symbol,pt.Magazyn) == true)
                {
                    if(pt.Magazyn == "aud7")
                    {
                        string txtQuery = "Update StanAud7 set Ilość = (Select Ilość from StanAud7 where Symbol = '" + pt.Symbol + "') + '" + pt.Ilość + "' where Symbol = '" + pt.Symbol + "';";
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                    }
                    else if (pt.Magazyn == "uro")
                    {
                        string txtQuery = "Update StanUro set Ilość = (Select Ilość from StanUro where Symbol = '" + pt.Symbol + "') + '" + pt.Ilość + "' where Symbol = '" + pt.Symbol + "';";
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                    }
                    
                }
                else if (CheckProductInBase(pt.Symbol,pt.Magazyn) == false)
                {
                    MessageBox.Show("Produktu " + pt.Nazwa + " nie było w bazie więc zostanie dodany do niej automatycznie.");
                    if (pt.Magazyn == "aud7")
                    {
                        string txtQuery = @"Insert into Products (Symbol, Nazwa_produktu, Jedn_miary, Id_magazynu) values ('" + pt.Symbol + "', '" + pt.Nazwa + "', '" + pt.Jedn + "', '" + pt.Magazyn + "');" +
                                        "Insert into StanAud7 (Symbol, Ilość) values ('" + pt.Symbol + "', '" + pt.Ilość + "');";
                        ConnectToDatabase();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                        CloseConnection();
                    }
                    else if(pt.Magazyn == "uro")
                    {
                        string txtQuery = @"Insert into Products (Symbol, Nazwa_produktu, Jedn_miary, Id_magazynu) values ('" + pt.Symbol + "', '" + pt.Nazwa + "', '" + pt.Jedn + "', '" + pt.Magazyn + "');" +
                                        "Insert into StanUro (Symbol, Ilość) values ('" + pt.Symbol + "', '" + pt.Ilość + "');";
                        ConnectToDatabase();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                        CloseConnection();
                    }
                }
            }
            CloseConnection();
            MessageBox.Show("Produkty zostały dodane");
        }
        private bool CheckProductInBase(string symbol,string magazyn)
        {
            if(magazyn == "aud7")
            {
                string txtQuery = "Select Symbol from StanAud7 where Symbol = '" + symbol + "';";
                ConnectToDatabase();
                SQLiteCommand sql_cmd = new SQLiteCommand(txtQuery, sql_con);
                sql_cmd.ExecuteNonQuery();
                SQLiteDataReader dataReader = sql_cmd.ExecuteReader();
                int count = 0;
                while (dataReader.Read()) { count++; }
                if (count == 1) { return true; }
                else { return false; }
            }
            else
            {
                string txtQuery = "Select Symbol from StanUro where Symbol = '" + symbol + "';";
                ConnectToDatabase();
                SQLiteCommand sql_cmd = new SQLiteCommand(txtQuery, sql_con);
                sql_cmd.ExecuteNonQuery();
                SQLiteDataReader dataReader = sql_cmd.ExecuteReader();
                int count = 0;
                while (dataReader.Read()) { count++; }
                if (count == 1) { return true; }
                else { return false; }
            }
        }
    }
}
