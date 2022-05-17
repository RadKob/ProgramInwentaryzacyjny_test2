using System.Data;
using System.Data.SQLite;
using System.Windows.Controls;

namespace ProgramInwentaryzacyjny
{
    public partial class StorageAud7Page : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public StorageAud7Page()
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
            string txtQuery = "Select Nazwa_produktu, Ilość from StanAud7 left join Products on Products.Symbol = StanAud7.Symbol";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
            dt = new DataTable("Products");
            dataAdapter.Fill(dt);
            ProduktDataGrid.ItemsSource = dt.DefaultView;
            CloseConnection();
        }
    }
}
