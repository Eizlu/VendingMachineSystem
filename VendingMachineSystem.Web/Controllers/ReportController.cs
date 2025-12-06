using Microsoft.AspNetCore.Mvc;
using VendingMachineSystem.BLL;

namespace VendingMachineSystem.Web.Controllers
{
    public class ReportController : Controller
    {
        private ReportService _service;

        public ReportController()
        {
            _service = new ReportService();
        }

        // Pomocná metoda pro kontrolu role (Validace oprávnění)
        private bool JeUzivatelOpravnen()
        {
            var role = HttpContext.Session.GetString("Role");
            // Pustíme jen Admina a Provozovatele
            return role == "Administrator" || role == "Provozovatel";
        }

        public IActionResult Index()
        {
            // 1. KONTROLA OPRÁVNĚNÍ
            if (!JeUzivatelOpravnen())
            {
                // Alternativní průběh: Neoprávněný přístup
                TempData["Zprava"] = "Nemáte oprávnění prohlížet finanční reporty!";
                TempData["TypZpravy"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            // Výchozí stav: Prázdný report, nastavíme datum na poslední měsíc
            ViewBag.DatumOd = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.DatumDo = DateTime.Today.ToString("yyyy-MM-dd");

            return View(new List<VendingMachineSystem.Core.StatistikaProdeje>());
        }

        [HttpPost]
        public IActionResult Generovat(DateTime datumOd, DateTime datumDo)
        {
            // 1. KONTROLA OPRÁVNĚNÍ (i u POST metody!)
            if (!JeUzivatelOpravnen())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var data = _service.GenerovatReport(datumOd, datumDo);

                // Aby formulář zůstal vyplněný
                ViewBag.DatumOd = datumOd.ToString("yyyy-MM-dd");
                ViewBag.DatumDo = datumDo.ToString("yyyy-MM-dd");

                return View("Index", data);
            }
            catch (ArgumentException ex)
            {
                // 2. VALIDACE DATA (Datum Od > Do)
                TempData["Zprava"] = ex.Message;
                TempData["TypZpravy"] = "danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ExportovatXml(DateTime datumOd, DateTime datumDo)
        {
            // Kontrola role (pro jistotu)
            if (!JeUzivatelOpravnen()) return RedirectToAction("Index", "Home");

            try
            {
                _service.ExportovatReportDoXml(datumOd, datumDo);
                TempData["Zprava"] = "Report byl úspěšně vyexportován do XML souboru.";
                TempData["TypZpravy"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Zprava"] = "Chyba při exportu: " + ex.Message;
                TempData["TypZpravy"] = "danger";
            }

            // Vrátíme uživatele zpět na report se stejným datem
            ViewBag.DatumOd = datumOd.ToString("yyyy-MM-dd");
            ViewBag.DatumDo = datumDo.ToString("yyyy-MM-dd");

            // Znovu načteme data pro zobrazení
            var data = _service.GenerovatReport(datumOd, datumDo);
            return View("Index", data);
        }
    }
}