namespace VendingMachineSystem.Core
{
    public class Produkt
    {
        public int Id { get; set; }
        public string Nazev { get; set; }
        public decimal Cena { get; set; }
        public string Kategorie { get; set; }
        public string Ean { get; set; }
    }
}