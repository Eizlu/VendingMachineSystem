using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core; // Tady říkáme, že chceme používat "slovník pojmů"

namespace VendingMachineSystem.DAL
{
    // Dědíme z BaseRepository, takže umíme používat metodu GetConnection()
    public class AutomatRepository : BaseRepository
    {
        // Metoda: "Jdi do skladu a přines všechny automaty"
        public List<Automat> GetAll()
        {
            var seznamAutomatu = new List<Automat>();

            // 1. Otevřeme spojení ("Vezmeme si klíč a odemkneme")
            using (SqlConnection connection = GetConnection())
            {
                // 2. Připravíme příkaz ("Co chceme udělat?")
                string sqlDotaz = "SELECT Id, Lokalita, Stav, Typ FROM Automat";

                using (SqlCommand command = new SqlCommand(sqlDotaz, connection))
                {
                    // Otevřít bránu
                    connection.Open();

                    // 3. Přečíst data ("Čteme řádek po řádku")
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 4. Převod z "tabulky" na "objekt"
                            Automat automat = new Automat();

                            // Pozor: Pořadí čísel (0, 1, 2...) musí sedět na pořadí v SQL dotazu (Id, Lokalita...)
                            automat.Id = reader.GetInt32(0);       // Sloupec Id
                            automat.Lokalita = reader.GetString(1); // Sloupec Lokalita
                            automat.Stav = reader.GetString(2);     // Sloupec Stav
                            automat.Typ = reader.GetString(3);      // Sloupec Typ

                            // Přidat do batohu
                            seznamAutomatu.Add(automat);
                        }
                    }
                } // Tady se automaticky zavře příkaz
            } // Tady se automaticky zavře spojení (zamknou dveře)

            return seznamAutomatu;
        }

        // Nová metoda: Vrátí seznam produktů v konkrétním automatu
        public List<ZasobaAutomatu> GetZasoby(int automatId)
        {
            var zasoby = new List<ZasobaAutomatu>();

            using (var connection = GetConnection())
            {
                // ZDE JE KOUZLO: Spojujeme (JOIN) tabulku zásob s tabulkou produktů,
                // abychom zjistili, jak se ten produkt jmenuje.
                string sql = @"
                    SELECT z.Id, z.AutomatId, z.ProduktId, z.Mnozstvi, z.MinimaleLimit, p.Nazev 
                    FROM ZasobaAutomatu z
                    JOIN Produkt p ON z.ProduktId = p.Id
                    WHERE z.AutomatId = @id";

                using (var command = new SqlCommand(sql, connection))
                {
                    // Ochrana proti hackerům (SQL Injection)
                    command.Parameters.AddWithValue("@id", automatId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var zasoba = new ZasobaAutomatu();
                            zasoba.Id = reader.GetInt32(0);
                            zasoba.AutomatId = reader.GetInt32(1);
                            zasoba.ProduktId = reader.GetInt32(2);
                            zasoba.Mnozstvi = reader.GetInt32(3);
                            zasoba.MinimaleLimit = reader.GetInt32(4);

                            // Tady načteme ten název z připojené tabulky
                            zasoba.NazevProduktu = reader.GetString(5);

                            zasoby.Add(zasoba);
                        }
                    }
                }
            }
            return zasoby;
        }
        // Metoda: Najdi jeden konkrétní automat podle ID
        public Automat GetById(int id)
        {
            Automat automat = null;

            using (var connection = GetConnection())
            {
                // SQL dotaz s podmínkou WHERE
                string sql = "SELECT Id, Lokalita, Stav, Typ FROM Automat WHERE Id = @id";

                using (var command = new SqlCommand(sql, connection))
                {
                    // Ochrana proti SQL Injection (vždy používej parametry!)
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Tady stačí "if", protože čekáme jen jeden výsledek
                        {
                            automat = new Automat();
                            automat.Id = reader.GetInt32(0);
                            automat.Lokalita = reader.GetString(1);
                            automat.Stav = reader.GetString(2);
                            automat.Typ = reader.GetString(3);
                        }
                    }
                }
            }
            return automat;
        }

        // Metoda: Doplní zboží (nastaví množství na bezpečnou hodnotu, např. 20)
        // Upravená metoda: Přijímá navíc parametr 'mnozstvi'
        public void DoplnitZasobu(int zasobaId, int mnozstvi)
        {
            using (var connection = GetConnection())
            {
                // SQL logika: Vezmi staré množství a přičti k němu to nové
                string sql = "UPDATE ZasobaAutomatu SET Mnozstvi = Mnozstvi + @mnozstvi, DatumPoslednihoDoplneni = GETDATE() WHERE Id = @id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", zasobaId);
                    command.Parameters.AddWithValue("@mnozstvi", mnozstvi);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}