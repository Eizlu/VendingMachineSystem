namespace VendingMachineSystem.Core
{
    public class ZasobaSkladu
    {
        public int Id { get; set; }
        public int ProduktId { get; set; }
        public int Mnozstvi { get; set; }

        // Pomocná vlastnost pro název (doplníme přes JOIN)
        public string NazevProduktu { get; set; }

        // Pomocná vlastnost: Je potřeba objednat? (Pokud je méně než 10 ks)
        public bool JePotrebaObjednat => Mnozstvi < 10;
    }
}