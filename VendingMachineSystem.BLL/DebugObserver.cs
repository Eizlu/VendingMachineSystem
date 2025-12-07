using System.Diagnostics;

namespace VendingMachineSystem.BLL
{
    public class DebugObserver : INotifikaceObserver
    {
        public void PrijmoutNotifikaci(string zprava)
        {
            Debug.WriteLine($"[NOTIFIKACE]: {zprava}");
        }
    }
}