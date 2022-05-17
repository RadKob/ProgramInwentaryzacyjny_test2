using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ProgramInwentaryzacyjny.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;


namespace ProgramInwentaryzacyjny
{
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
        private void MagazynAud7_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StorageAud7Page();
        }
        private void MagazynUro_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StorageUroPage();
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
        private void ExportStorage_Click(object sender, RoutedEventArgs e)
        {
            string txtQuery = "Select Nazwa_produktu, Ilość from StanAud7 left join Products on Products.Symbol = StanAud7.Symbol";
            ConnectToDatabase();
            sql_cmd = new SQLiteCommand(txtQuery, sql_con);
            SQLiteDataReader dr = sql_cmd.ExecuteReader();
            List<ProductEx> products_list = new List<ProductEx>();
            while (dr.Read())
            {
                ProductEx product = new ProductEx(dr["Nazwa_produktu"].ToString(), Convert.ToInt32(dr["Ilość"]));
                products_list.Add(product);
            }
            CloseConnection();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create("Inwenta.xlsx", SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Raport" };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                foreach (var item in products_list)
                {
                    Row row = new Row();
                    Cell cell1 = new Cell() { CellValue = new CellValue(item.Nazwa), DataType = CellValues.String };
                    Cell cell2 = new Cell() { CellValue = new CellValue(item.Ilosc), DataType = CellValues.String };
                    row.Append(cell1);
                    row.Append(cell2);
                    sheetData.AppendChild(row);
                }
                worksheetPart.Worksheet.Save();
                MessageBox.Show("Raport utworzony");
            }
        }
    }
}
