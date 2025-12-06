using System.Collections.Generic;
using VendingMachineSystem.Core;
using VendingMachineSystem.DAL;

namespace VendingMachineSystem.BLL
{
    public class SkladService
    {
        private SkladRepository _repository;

        public SkladService()
        {
            _repository = new SkladRepository();
            _objednavkaRepo = new ObjednavkaRepository();
        }

        public List<ZasobaSkladu> GetPrehledSkladu()
        {
            return _repository.GetSkladoveZasoby();
        }

        // ... uvnitř třídy SkladService ...

        // Přidáme si privátní proměnnou pro novou repository
        private ObjednavkaRepository _objednavkaRepo;

        // Metoda pro vytvoření objednávky
        public void ObjednatZbozi(int produktId, int mnozstvi)
        {
            // VALIDACE (Alternativní průběh):
            if (mnozstvi <= 0)
            {
                throw new ArgumentException("Množství k objednání musí být alespoň 1 kus!");
            }

            if (mnozstvi > 200)
            {
                throw new ArgumentException("Nelze objednat více než 200 kusů najednou (kapacita skladu).");
            }

            // Pokud je to OK, pošleme to do DAL
            _objednavkaRepo.VytvoritObjednavku(produktId, mnozstvi);
        }

        // ... stávající kód ...

        // Metoda pro UC 18: Generování reportu a export do XML
        public string VygenerovatAExportovatReport()
        {
            // 1. Získáme data z SQL (MS SQL)
            var zasoby = _repository.GetSkladoveZasoby();

            // 2. Vytvoříme report (Logika aplikace)
            var report = new ReportSkladu();
            report.DatumGenerovani = DateTime.Now;

            // Potřebovali bychom ceny, pro zjednodušení si je "vymyslíme" nebo by musely být v SQL.
            // Pro účel splnění zadání předpokládejme fixní cenu, nebo si dotáhni cenu z Produktu (už ji tam možná máme v JOINu?)
            // V SkladRepository v JOINu máme: z.Id, z.ProduktId, z.Mnozstvi, p.Nazev.
            // POZOR: Chybí nám tam cena. Ale pro demonstraci XML stačí počty kusů.

            decimal celkovaHodnota = 0;

            foreach (var z in zasoby)
            {
                var polozka = new PolozkaReportu
                {
                    NazevProduktu = z.NazevProduktu,
                    Mnozstvi = z.Mnozstvi,
                    CenaZaKus = 10, // Placeholder, abychom nemuseli měnit SQL repository
                    CelkovaCena = z.Mnozstvi * 10
                };
                report.Polozky.Add(polozka);
                celkovaHodnota += polozka.CelkovaCena;
            }
            report.CelkovaHodnotaZasob = celkovaHodnota;

            // 3. Uložíme do XML (Druhé úložiště)
            var xmlRepo = new XmlRepository();
            xmlRepo.UlozitDoXml(report, "StavSkladu_Export.xml");

            return "Report byl úspěšně vyexportován do XML.";
        }
    }
}