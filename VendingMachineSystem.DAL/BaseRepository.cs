using Microsoft.Data.SqlClient;

namespace VendingMachineSystem.DAL
{
    public abstract class BaseRepository
    {
        protected string _connectionString => DbConfig.Instance.ConnectionString;

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}