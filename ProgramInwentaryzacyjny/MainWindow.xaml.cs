using IronPdf;
using ProgramInwentaryzacyjny.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace ProgramInwentaryzacyjny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
        // raport produktow zerowych
        private void Raport_Click(object sender, RoutedEventArgs e)
        {
            string txtQuery = "Select Nazwa_Produktu, Sum(Wydanie) as Wydanie, Data from Products left join Zuzycie on Products.Symbol = Zuzycie.Symbol where Wydanie < 0 Group by Nazwa_produktu";
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
        private void Administrator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AdministratorPage();
        }
        private void Wyjście_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // kopia pliku bazy sqlite 15-go dnia
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
