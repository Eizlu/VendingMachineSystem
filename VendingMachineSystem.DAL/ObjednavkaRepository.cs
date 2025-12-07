using System;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class ObjednavkaRepository : BaseRepository
    {
        public void VytvoritObjednavku(int produktId, int mnozstvi)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                string sqlObjednavka = @"
                    INSERT INTO Objednavka (Dodavatel, DatumVytvoreni, Stav) 
                    OUTPUT INSERTED.ID
                    VALUES ('Externí Dodavatel', GETDATE(), 'Nova')";

                int noveIdObjednavky;

                using (var command = new SqlCommand(sqlObjednavka, connection))
                {
                    noveIdObjednavky = (int)command.ExecuteScalar();
                }

                string sqlPolozka = @"
                    INSERT INTO PolozkaObjednavky (ObjednavkaId, ProduktId, Mnozstvi)
                    VALUES (@objednavkaId, @produktId, @mnozstvi)";

                using (var command = new SqlCommand(sqlPolozka, connection))
                {
                    command.Parameters.AddWithValue("@objednavkaId", noveIdObjednavky);
                    command.Parameters.AddWithValue("@produktId", produktId);
                    command.Parameters.AddWithValue("@mnozstvi", mnozstvi);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}