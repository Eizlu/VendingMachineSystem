namespace VendingMachineSystem.Core
{
    public class Automat
    {
        public int Id { get; set; }
        public string Lokalita { get; set; }
        public string Stav { get; set; } // 'Online', 'Offline', 'Porucha'
        public string Typ { get; set; }  // 'Nápoje', 'Jídlo', 'Mix'
    }
}