using System;

namespace VendingMachineSystem.Core
{
    public class Objednavka
    {
        public int Id { get; set; }
        public string Dodavatel { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public string Stav { get; set; } // 'Nova', 'Odeslana'
    }
}