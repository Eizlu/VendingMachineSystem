using System.Diagnostics;

namespace VendingMachineSystem.BLL
{
    // Konkrétní posluchač - v reálu by posílal email, my píšeme do Debug okna
    public class DebugObserver : INotifikaceObserver
    {
        public void PrijmoutNotifikaci(string zprava)
        {
            // Vypíše se to do okna "Output" ve Visual Studiu
            Debug.WriteLine($"[NOTIFIKACE]: {zprava}");
        }
    }
}