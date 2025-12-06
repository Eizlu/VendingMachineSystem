using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class UzivatelRepository : BaseRepository
    {
        // Metoda ověří uživatele a vrátí ho, pokud existuje
        public Uzivatel Login(string login, string heslo)
        {
            Uzivatel uzivatel = null;

            using (var connection = GetConnection())
            {
                // SQL dotaz: Hledáme shodu loginu a hesla
                string sql = "SELECT Id, Jmeno, Login, Role FROM Uzivatel WHERE Login = @login AND Heslo = @heslo";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@heslo", heslo); // Poznámka: V reálu by heslo mělo být hashované

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uzivatel = new Uzivatel
                            {
                                Id = reader.GetInt32(0),
                                Jmeno = reader.GetString(1),
                                Login = reader.GetString(2),
                                Role = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return uzivatel; // Vrátí null, pokud se přihlášení nepovedlo
        }
    }
}