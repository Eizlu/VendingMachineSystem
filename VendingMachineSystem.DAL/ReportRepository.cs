using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class ReportRepository : BaseRepository
    {
        public List<StatistikaProdeje> GetProdejePodleProduktu(DateTime datumOd, DateTime datumDo)
        {
            var seznam = new List<StatistikaProdeje>();

            using (var connection = GetConnection())
            {
                string sql = @"
                    SELECT p.Nazev, COUNT(*) as Kusy, SUM(pr.Cena) as Trzba
                    FROM Prodej pr
                    JOIN Produkt p ON pr.ProduktId = p.Id
                    WHERE pr.DatumProdeje >= @od AND pr.DatumProdeje <= @do
                    GROUP BY p.Nazev
                    ORDER BY Trzba DESC"; 

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@od", datumOd);
                    command.Parameters.AddWithValue("@do", datumDo);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            seznam.Add(new StatistikaProdeje
                            {
                                NazevProduktu = reader.GetString(0),
                                ProdaneKusy = reader.GetInt32(1),
                                CelkovaTrzba = reader.GetDecimal(2)
                            });
                        }
                    }
                }
            }
            return seznam;
        }
    }
}