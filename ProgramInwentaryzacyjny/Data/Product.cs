using System;
using System.Collections.Generic;
using System.Text;

namespace ProgramInwentaryzacyjny.Data
{
    class Product
    {
        public string Symbol { get; set; }
        public string Nazwa { get; set; }
        public int Ilość { get; set; }
        public string Jedn { get; set; }
        public Product(string symbol, string nazwa, int ilość, string jedn)
        {
            Symbol = symbol;
            Nazwa = nazwa;
            Ilość = ilość;
            Jedn = jedn;
        }
    }
}
