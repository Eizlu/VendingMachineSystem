namespace VendingMachineSystem.Core
{
    public class ZasobaSkladu
    {
        public int Id { get; set; }
        public int ProduktId { get; set; }
        public int Mnozstvi { get; set; }
        public string NazevProduktu { get; set; }
        public int MnozstviNaCeste { get; set; }
        public bool JePotrebaObjednat => Mnozstvi < 10;
    }
}