using Microsoft.Data.SqlClient;

namespace VendingMachineSystem.DAL
{
    public abstract class BaseRepository
    {
        // ZMĚNA: Už tu není řetězec natvrdo, ale odkazujeme se na Singleton------------------------------------------------------

        protected string _connectionString => DbConfig.Instance.ConnectionString;

        // Tato metoda slouží jako "výrobna" připojení
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}