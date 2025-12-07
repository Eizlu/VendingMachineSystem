namespace VendingMachineSystem.BLL
{
    // NÁVRHOVÝ VZOR: Observer (Rozhraní)
    public interface INotifikaceObserver
    {
        void PrijmoutNotifikaci(string zprava);
    }
}