namespace VendingMachineSystem.DAL
{
    public class DbConfig
    {
        private static DbConfig _instance;

        public string ConnectionString { get; private set; }

        private DbConfig()
        {
            ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=VendingMachineDB;Integrated Security=True;TrustServerCertificate=True";
        }

        public static DbConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbConfig();
                }
                return _instance;
            }
        }
    }
}