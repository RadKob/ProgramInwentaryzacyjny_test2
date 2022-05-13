using System;
using System.Collections.Generic;
using System.Text;

namespace ProgramInwentaryzacyjny.Data
{
    class ProductEx
    {
        public string Nazwa { get; set; }
        public int Ilosc { get; set; }
        public ProductEx(string nazwa, int ilosc)
        {
            Nazwa = nazwa;
            Ilosc = ilosc;
        }
    }
}
