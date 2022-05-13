using System;
using System.Collections.Generic;
using System.Text;

namespace ProgramInwentaryzacyjny.Data
{
    class ProductR
    {
        public string Nazwa { get; set; }
        public int Wydanie { get; set; }
        public string Data { get; set; }
        public ProductR(string nazwa, int wydanie, string data)
        {
            Nazwa = nazwa;
            Wydanie = wydanie;
            Data = data;
        }
    }
}
