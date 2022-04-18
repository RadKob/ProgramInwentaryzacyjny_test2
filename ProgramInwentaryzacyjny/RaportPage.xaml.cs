using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ProgramInwentaryzacyjny.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace ProgramInwentaryzacyjny
{
    public partial class RaportPage : System.Windows.Controls.Page
    {
        readonly string connection_string = "Data Source=BazaDoProgramu.db;Version=3;New=false;Compress=True;";
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter dataAdapter;
        DataTable dt = new DataTable();
        public RaportPage()
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
        private void RaportExcel(object sender, RoutedEventArgs e)
        {
            string dataA = data_A.Text;
            string dataB = data_B.Text;
            string txtQuery = "Select Nazwa_Produktu, Sum(Wydanie) as Wydanie, Data from Products left join Zuzycie on Products.Symbol = Zuzycie.Symbol where Wydanie < 0 and Data between '" + dataA + "' and '" + dataB + "' Group by Nazwa_produktu";
            ConnectToDatabase();
            sql_cmd = new SQLiteCommand(txtQuery, sql_con);
            SQLiteDataReader dr = sql_cmd.ExecuteReader();
            List<ProductR> products_list = new List<ProductR>();
            while (dr.Read())
            {
                ProductR product = new ProductR(dr["Nazwa_produktu"].ToString(), Convert.ToInt32(dr["Wydanie"]), dr["Data"].ToString());
                products_list.Add(product);
            }
            CloseConnection();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create("raport.xlsx", SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Arkusz 1" };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                foreach (var item in products_list)
                {
                    Row row = new Row();
                    Cell cell1 = new Cell() { CellValue = new CellValue(item.Nazwa), DataType = CellValues.String };
                    Cell cell2 = new Cell() { CellValue = new CellValue(item.Wydanie), DataType = CellValues.String };
                    Cell cell3 = new Cell() { CellValue = new CellValue(item.Data), DataType = CellValues.String };
                    row.Append(cell1);
                    row.Append(cell2);
                    row.Append(cell3);
                    sheetData.AppendChild(row);
                }
                worksheetPart.Worksheet.Save();
            }
        }
        private void LoadProducts()
        {
            DateTime copydaytime = DateTime.Today;
            string a = copydaytime.ToString("d");
            string txtQuery = "Select Nazwa_produktu, Sum(Wydanie) as Wydanie from Zuzycie left join Products on Products.Symbol = Zuzycie.Symbol where Wydanie < 0 and Data ='" + a + "' Group by Nazwa_produktu";
            ConnectToDatabase();
            sql_cmd = sql_con.CreateCommand();
            dataAdapter = new SQLiteDataAdapter(txtQuery, sql_con);
            dt = new DataTable("Products");
            dataAdapter.Fill(dt);
            ZuzycieDataGrid.ItemsSource = dt.DefaultView;
            CloseConnection();
        }
    }
}
