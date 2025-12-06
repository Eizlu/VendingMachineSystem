using System; // Nutné pro DateTime

namespace VendingMachineSystem.Core
{
    public class ZasobaAutomatu
    {
        public int Id { get; set; }
        public int AutomatId { get; set; }
        public int ProduktId { get; set; }
        public int Mnozstvi { get; set; }
        public int MinimaleLimit { get; set; }
        public DateTime DatumPoslednihoDoplneni { get; set; }
        public string NazevProduktu { get; set; }
    }
}