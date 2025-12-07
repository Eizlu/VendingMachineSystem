using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core; 

namespace VendingMachineSystem.DAL
{
    public class AutomatRepository : BaseRepository
    {
        public List<Automat> GetAll()
        {
            var seznamAutomatu = new List<Automat>();

            using (SqlConnection connection = GetConnection())
            {
                string sqlDotaz = "SELECT Id, Lokalita, Stav, Typ FROM Automat";

                using (SqlCommand command = new SqlCommand(sqlDotaz, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Automat automat = new Automat();

                            automat.Id = reader.GetInt32(0);       
                            automat.Lokalita = reader.GetString(1); 
                            automat.Stav = reader.GetString(2);     
                            automat.Typ = reader.GetString(3);      

                            seznamAutomatu.Add(automat);
                        }
                    }
                } 
            } 

            return seznamAutomatu;
        }

        public List<ZasobaAutomatu> GetZasoby(int automatId)
        {
            var zasoby = new List<ZasobaAutomatu>();

            using (var connection = GetConnection())
            {
                string sql = @"
                    SELECT z.Id, z.AutomatId, z.ProduktId, z.Mnozstvi, z.MinimaleLimit, p.Nazev 
                    FROM ZasobaAutomatu z
                    JOIN Produkt p ON z.ProduktId = p.Id
                    WHERE z.AutomatId = @id";

                using (var command = new SqlCommand(sql, connection))
                {
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

                            zasoba.NazevProduktu = reader.GetString(5);

                            zasoby.Add(zasoba);
                        }
                    }
                }
            }
            return zasoby;
        }
        public Automat GetById(int id)
        {
            Automat automat = null;

            using (var connection = GetConnection())
            {
                string sql = "SELECT Id, Lokalita, Stav, Typ FROM Automat WHERE Id = @id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

        public void DoplnitZasobu(int zasobaId, int mnozstvi)
        {
            using (var connection = GetConnection())
            {
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