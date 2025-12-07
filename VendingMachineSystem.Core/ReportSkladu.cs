using System;
using System.Collections.Generic;

namespace VendingMachineSystem.Core
{
    public class ReportSkladu
    {
        public DateTime DatumGenerovani { get; set; }
        public decimal CelkovaHodnotaZasob { get; set; }
        public List<PolozkaReportu> Polozky { get; set; }

        public ReportSkladu()
        {
            Polozky = new List<PolozkaReportu>();
        }
    }

    public class PolozkaReportu
    {
        public string NazevProduktu { get; set; }
        public int Mnozstvi { get; set; }
        public decimal CenaZaKus { get; set; }
        public decimal CelkovaCena { get; set; }
    }
}