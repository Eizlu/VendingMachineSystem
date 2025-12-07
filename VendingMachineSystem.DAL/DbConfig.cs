namespace VendingMachineSystem.DAL
{
    // NÁVRHOVÝ VZOR: Singleton
    // Zajišťuje, že konfigurace existuje v paměti jen jednou.
    public class DbConfig
    {
        private static DbConfig _instance;

        // Vlastnost pro uložení řetězce
        public string ConnectionString { get; private set; }

        // Privátní konstruktor = nikdo jiný nemůže vyrobit instanci "new DbConfig()"
        private DbConfig()
        {
            // ZDE JE TVŮJ CONNECTION STRING (přesunutý z BaseRepository)
            ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=VendingMachineDB;Integrated Security=True;TrustServerCertificate=True";
        }

        // Veřejný přístup k jediné instanci
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