using System;
using Microsoft.Data.SqlClient;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.DAL
{
    public class ObjednavkaRepository : BaseRepository
    {
        // Metoda vytvoří objednávku a rovnou do ní vloži jednu položku
        public void VytvoritObjednavku(int produktId, int mnozstvi)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // 1. KROK: Vložíme hlavičku objednávky a získáme její nové ID
                // "OUTPUT INSERTED.ID" je SQL trik, jak hned zjistit číslo nové objednávky
                string sqlObjednavka = @"
                    INSERT INTO Objednavka (Dodavatel, DatumVytvoreni, Stav) 
                    OUTPUT INSERTED.ID
                    VALUES ('Externí Dodavatel', GETDATE(), 'Nova')";

                int noveIdObjednavky;

                using (var command = new SqlCommand(sqlObjednavka, connection))
                {
                    // ExecuteScalar vrátí tu první buňku (naše nové ID)
                    noveIdObjednavky = (int)command.ExecuteScalar();
                }

                // 2. KROK: Vložíme položku do této nové objednávky
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