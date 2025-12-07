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

        private ObjednavkaRepository _objednavkaRepo;

        public void ObjednatZbozi(int produktId, int mnozstvi)
        {
            if (mnozstvi <= 0)
            {
                throw new ArgumentException("Množství k objednání musí být alespoň 1 kus!");
            }

            if (mnozstvi > 200)
            {
                throw new ArgumentException("Nelze objednat více než 200 kusů najednou (kapacita skladu).");
            }

            _objednavkaRepo.VytvoritObjednavku(produktId, mnozstvi);
        }

        public string VygenerovatAExportovatReport()
        {
            var zasoby = _repository.GetSkladoveZasoby();

            var report = new ReportSkladu();
            report.DatumGenerovani = DateTime.Now;

            decimal celkovaHodnota = 0;

            foreach (var z in zasoby)
            {
                var polozka = new PolozkaReportu
                {
                    NazevProduktu = z.NazevProduktu,
                    Mnozstvi = z.Mnozstvi,
                    CenaZaKus = 10, 
                    CelkovaCena = z.Mnozstvi * 10
                };
                report.Polozky.Add(polozka);
                celkovaHodnota += polozka.CelkovaCena;
            }
            report.CelkovaHodnotaZasob = celkovaHodnota;

            var xmlRepo = new XmlRepository();
            xmlRepo.UlozitDoXml(report, "StavSkladu_Export.xml");

            return "Report byl úspěšně vyexportován do XML.";
        }
    }
}