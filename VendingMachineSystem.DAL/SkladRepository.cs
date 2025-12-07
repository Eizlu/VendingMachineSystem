using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class SkladRepository : BaseRepository
    {
        public List<ZasobaSkladu> GetSkladoveZasoby()
        {
            var seznam = new List<ZasobaSkladu>();

            using (var connection = GetConnection())
            {
                // Spojíme tabulku skladu s tabulkou produktů
                string sql = @"
                    SELECT 
                        z.Id, 
                        z.ProduktId, 
                        z.Mnozstvi, 
                        p.Nazev,
                        (
                            SELECT ISNULL(SUM(po.Mnozstvi), 0)
                            FROM PolozkaObjednavky po
                            JOIN Objednavka o ON po.ObjednavkaId = o.Id
                            WHERE po.ProduktId = z.ProduktId AND o.Stav = 'Nova'
                        ) AS NaCeste
                    FROM ZasobaSkladu z
                    JOIN Produkt p ON z.ProduktId = p.Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var zasoba = new ZasobaSkladu();
                            zasoba.Id = reader.GetInt32(0);
                            zasoba.ProduktId = reader.GetInt32(1);
                            zasoba.Mnozstvi = reader.GetInt32(2);
                            zasoba.NazevProduktu = reader.GetString(3);

                            zasoba.MnozstviNaCeste = reader.GetInt32(4);

                            seznam.Add(zasoba);
                        }
                    }
                }
            }
            return seznam;
        }
    }
}