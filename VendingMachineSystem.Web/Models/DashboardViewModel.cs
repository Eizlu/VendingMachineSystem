using VendingMachineSystem.Core;

namespace VendingMachineSystem.Web.Models
{
    public class AutomatKartaViewModel
    {
        public Automat Info { get; set; } 
        public List<ZasobaAutomatu> Zasoby { get; set; }
        public bool VyzadujePozornost => Zasoby.Any(z => z.Mnozstvi < z.MinimaleLimit);
    }
}