using VendingMachineSystem.Core;

namespace VendingMachineSystem.Web.Models
{
    // Tato třída představuje jednu "kartičku" na nástěnce
    public class AutomatKartaViewModel
    {
        public Automat Info { get; set; }           // Informace o automatu (Lokalita, Stav)
        public List<ZasobaAutomatu> Zasoby { get; set; } // Co je uvnitř

        // Pomocná vlastnost: Má automat nějaký problém (málo zboží)?
        public bool VyzadujePozornost => Zasoby.Any(z => z.Mnozstvi < z.MinimaleLimit);
    }
}