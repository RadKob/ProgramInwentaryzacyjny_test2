using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Logika interakcji dla klasy StorageEditPage.xaml
    /// </summary>
    public partial class StorageEditPage : Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public StorageEditPage()
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
            string txtQuery = "Select * from Products";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
            dt = new DataTable("Products");
            dataAdapter.Fill(dt);
            ProduktDataGrid.ItemsSource = dt.DefaultView;
            CloseConnection();
        }
        private void txt_wyszukiwarka_KeyPress(object sender, KeyEventArgs e)
        {
            if (txt_wyszukiwarka.Text == string.Empty)
            {
                LoadProducts();
            }
            else
            {
                string txtQuery = string.Format("Select * from Products where Symbol like '%{0}%' or Nazwa_produktu like '%{0}%'", txt_wyszukiwarka.Text);
                sql_cmd = sql_con.CreateCommand();
                dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
                dt = new DataTable("Products");
                dataAdapter.Fill(dt);
                ProduktDataGrid.ItemsSource = dt.DefaultView;
                CloseConnection();
            }
        }
        // aktualizacja texblockow po wybraniu produktu z siatki
        private void SelectProcudct(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView dataRowView = dataGrid.SelectedItem as DataRowView;
            if (dataRowView != null)
            {
                txt_symbolEdit.Text = dataRowView["Symbol"].ToString();
                txt_nazwaEdit.Text = dataRowView["Nazwa_produktu"].ToString();
            }
        }
        // czyszczenie texboxa
        private void ClearTxtBoxs()
        {
            txt_iloscEdit.Clear();
        }
        // ręczna aktualizacja stanu produktu
        private void EditProduct(object sender, RoutedEventArgs e)
        {
            DateTime localDate = DateTime.Now;
            int number;
            if (txt_iloscEdit.Text != string.Empty)
            {
                if (Int32.TryParse(txt_iloscEdit.Text, out number)==false)
                {
                    MessageBox.Show("Uwaga - niedozwolona operacja, wpisałeś literę");
                }
                else
                {
                    if (Math.Abs(Convert.ToInt32(txt_iloscEdit.Text)) <= NotZero() && Convert.ToInt32(txt_iloscEdit.Text) < 0)
                    {
                        string txtQuery = @"Update Stan set Ilość = (Select Ilość from Stan where Symbol ='" + txt_symbolEdit.Text + "') + '" + txt_iloscEdit.Text + "' where Symbol = '" + txt_symbolEdit.Text + "';" +
                                            "Insert into Zuzycie (Symbol, Wydanie, Data) values ('" + txt_symbolEdit.Text + "', '" + txt_iloscEdit.Text + "', '" + localDate.ToString() + "');";
                        ConnectToDatabase();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                        CloseConnection();
                        MessageBox.Show("Produkt " + txt_nazwaEdit.Text + " został wydany w liczbie sztuk " + txt_iloscEdit.Text);
                        LoadProducts();
                        ClearTxtBoxs();
                    }
                    else if(Convert.ToInt32(txt_iloscEdit.Text) > 0)
                    {
                        string txtQuery = @"Update Stan set Ilość = (Select Ilość from Stan where Symbol ='" + txt_symbolEdit.Text + "') + '" + txt_iloscEdit.Text + "' where Symbol = '" + txt_symbolEdit.Text + "';" +
                                            "Insert into Zuzycie (Symbol, Dodanie, Data) values ('" + txt_symbolEdit.Text + "', '" + txt_iloscEdit.Text + "', '" + localDate.ToString() + "');";
                        ConnectToDatabase();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = txtQuery;
                        sql_cmd.ExecuteNonQuery();
                        CloseConnection();
                        MessageBox.Show("Produkt " + txt_nazwaEdit.Text + " został dodany w liczbie sztuk " + txt_iloscEdit.Text);
                        LoadProducts();
                        ClearTxtBoxs();
                    }
                    else
                    {
                        MessageBox.Show("Uwaga - próbujesz zrobić stan ujemny");
                        ClearTxtBoxs();
                    }
                }
            }
            else
            {
                MessageBox.Show("Parametr nie może być pusty");
            }
        }
        // iloscproduktu w chwili aktualizacji jego stanu
        private int NotZero()
        {
            string txtQuery = "Select Ilość from Stan where Symbol ='" + txt_symbolEdit.Text + "'";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            int notzero = Convert.ToInt32(sql_cmd.ExecuteScalar());
            CloseConnection();
            return notzero;
        }
    }
}
