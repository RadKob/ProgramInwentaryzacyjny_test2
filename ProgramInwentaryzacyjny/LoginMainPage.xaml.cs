using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Logika interakcji dla klasy LoginMainPage.xaml
    /// </summary>
    public partial class LoginMainPage : Window
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public LoginMainPage()
        {
            InitializeComponent();
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
        private void btn_Exit_Action(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_Login_Action(object sender, RoutedEventArgs e)
        {
            try
            {
                string txtQuery = "Select * from Users where UserLogin= '" + txt_użytkownik.Text + "' and Password= '" + txt_hasło.Password + "'";
                ConnectToDatabase();
                SQLiteCommand sql_cmd = new SQLiteCommand(txtQuery, sql_con);
                sql_cmd.ExecuteNonQuery();
                SQLiteDataReader dataReader = sql_cmd.ExecuteReader();
                int count = 0;
                while (dataReader.Read()) { count++; }
                if(count == 1)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nazwa użytkownika lub hasło jest błędne");                }
                CloseConnection();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
    }
}
