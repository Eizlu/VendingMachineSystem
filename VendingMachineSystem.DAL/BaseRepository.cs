using Microsoft.Data.SqlClient;

namespace VendingMachineSystem.DAL
{
    public abstract class BaseRepository
    {
        // ---------------------------------------------------------
        // KROK PRO TEBE:
        // Místo textu "ZDE_VLOZ_SVUJ_STRING" vlož to, co jsi zkopírovala (Ctrl+V).
        // Nech tam ten zavináč @ a ty uvozovky!
        // Vypadá to třeba takto: @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=..."
        // ---------------------------------------------------------

        protected readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=VendingMachineDB;Integrated Security=True;TrustServerCertificate=True";

        // Tato metoda slouží jako "výrobna" připojení
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}