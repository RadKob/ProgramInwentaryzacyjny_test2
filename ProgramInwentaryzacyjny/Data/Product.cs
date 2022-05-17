namespace ProgramInwentaryzacyjny.Data
{
    class Product
    {
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public int Ilość { get; set; }
        public string Jedn { get; set; }
        public string Magazyn { get; set; }
        public Product(string symbol, string nazwa, int ilość, string jedn, string magazyn)
        {
            Symbol = symbol;
            Nazwa = nazwa;
            Ilość = ilość;
            Jedn = jedn;
            Magazyn = magazyn;
        }
    }
}
