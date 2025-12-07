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
            if (od > @do)
            {
                throw new ArgumentException("Datum 'Od' nesmí být novější než datum 'Do'.");
            }

            return _repository.GetProdejePodleProduktu(od, @do);
        }

        public void ExportovatReportDoXml(DateTime od, DateTime @do)
        {
            var data = _repository.GetProdejePodleProduktu(od, @do);

            var xmlRepo = new XmlRepository();

            string nazevSouboru = $"Report_{od:yyyy-MM-dd}_{@do:yyyy-MM-dd}.xml";

            xmlRepo.UlozitDoXml(data, nazevSouboru);
        }
    }
}