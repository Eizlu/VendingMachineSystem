using System;
using System.Collections.Generic;
using VendingMachineSystem.Core;
using VendingMachineSystem.DAL;

namespace VendingMachineSystem.BLL
{
    public class ReportService
    {
        private ReportRepository _repository;

        public ReportService()
        {
            _repository = new ReportRepository();
        }

        public List<StatistikaProdeje> GenerovatReport(DateTime od, DateTime @do)
        {
            // VALIDACE (Alternativní průběh - chyba)
            if (od > @do)
            {
                throw new ArgumentException("Datum 'Od' nesmí být novější než datum 'Do'.");
            }

            return _repository.GetProdejePodleProduktu(od, @do);
        }

        // ... stávající kód ...

        public void ExportovatReportDoXml(DateTime od, DateTime @do)
        {
            // 1. Získáme data (z SQL)
            var data = _repository.GetProdejePodleProduktu(od, @do);

            // 2. Uložíme je do XML (pomocí XML Repository)
            var xmlRepo = new XmlRepository();

            // Soubor se bude jmenovat třeba "Report_2023-10-01_2023-11-01.xml"
            string nazevSouboru = $"Report_{od:yyyy-MM-dd}_{@do:yyyy-MM-dd}.xml";

            xmlRepo.UlozitDoXml(data, nazevSouboru);
        }
    }
}