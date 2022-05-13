using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProgramInwentaryzacyjny
{
    public partial class AdministratorPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public AdministratorPage()
        {
            InitializeComponent();
            LoadProducts();
        }
        private void ConnectToDatabase()
        {
            sql_con = new SQLiteConnection(connection_string);
            sql_con.Open();
        }
        private void CloseConnection() { sql_con.Close(); }
        private void AddProduct(object sender, RoutedEventArgs e)
        {
            if (txt_symbolAdd.Text == string.Empty || txt_nazwaAdd.Text == string.Empty || txt_jednAdd.Text == string.Empty) { MessageBox.Show("Parametry nie mogą być puste"); }
            else
            {
                if (CheckProductsTable() == true) { MessageBox.Show("Produkt już istnieje"); }
                else
                {
                    string txtQuery = @"Insert into Products (Symbol, Nazwa_produktu, Jedn_miary) values ('" + txt_symbolAdd.Text + "', '" + txt_nazwaAdd.Text + "', '" + txt_jednAdd.Text + "');" +
                                        "Insert into Stan (Symbol, Ilość) values ('" + txt_symbolAdd.Text + "', 0);";
                    ConnectToDatabase();
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = txtQuery;
                    sql_cmd.ExecuteNonQuery();
                    CloseConnection();
                    MessageBox.Show("Produkt dodany");
                    LoadProducts();
                    ClearTxtBoxs();
                }
            }
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
        private void EditProduct(object sender, RoutedEventArgs e)
        {
            if (txt_symbolAdd.Text == string.Empty || txt_nazwaAdd.Text == string.Empty || txt_jednAdd.Text == string.Empty) { MessageBox.Show("Parametry nie mogą być puste"); }
            else
            {
                string txtQuery = "Update Products set Nazwa_Produktu = '" + txt_nazwaAdd.Text + "', Jedn_miary = '" + txt_jednAdd.Text + "' where Symbol = '" + txt_symbolAdd.Text + "'";
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
        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Czy na pewno chcesz usunąć produkt? Zostanie on wymazany całkowicie.", "Usuwanie", MessageBoxButton.YesNo);
            if (m == MessageBoxResult.Yes)
            {
                string txtQuery = @"Delete from Products where Symbol = '" + txt_symbolAdd.Text + "';" +
                                "Delete from Stan where Symbol = '" + txt_symbolAdd.Text + "';";
                ConnectToDatabase();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                CloseConnection();
                MessageBox.Show("Produkt usunięty");
                LoadProducts();
                ClearTxtBoxs();
            }
        }
        private void SelectProcudct(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView dataRowView = dataGrid.SelectedItem as DataRowView;
            if (dataRowView != null)
            {
                txt_symbolAdd.Text = dataRowView["Symbol"].ToString();
                txt_nazwaAdd.Text = dataRowView["Nazwa_produktu"].ToString();
                txt_jednAdd.Text = dataRowView["Jedn_miary"].ToString();
            }
        }
        private void ClearTxtBoxs()
        {
            txt_symbolAdd.Clear();
            txt_nazwaAdd.Clear();
            txt_jednAdd.Clear();
        }
        private bool CheckProductsTable()
        {
            string txtQuery = "Select count(1) from Products where Symbol = '" + txt_symbolAdd.Text + "'";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            int check = Convert.ToInt32(sql_cmd.ExecuteScalar());
            if(check == 1) { return true; }
            else { return false; }
        }
        private void CopyDatabase(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wykonano ręczną kopie zapasową bazy danych");
            string srcPath = "BazaDoProgramu.db";
            string dstPath = "BazaDoProgramu_copy.db";

            File.Copy(srcPath, dstPath, true);
        }
    }
}
