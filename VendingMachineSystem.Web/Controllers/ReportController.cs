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

        private bool JeUzivatelOpravnen()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Administrator" || role == "Provozovatel";
        }

        public IActionResult Index()
        {
            if (!JeUzivatelOpravnen())
            {
                TempData["Zprava"] = "Nemáte oprávnění prohlížet finanční reporty!";
                TempData["TypZpravy"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DatumOd = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.DatumDo = DateTime.Today.ToString("yyyy-MM-dd");

            return View(new List<VendingMachineSystem.Core.StatistikaProdeje>());
        }

        [HttpPost]
        public IActionResult Generovat(DateTime datumOd, DateTime datumDo)
        {
            if (!JeUzivatelOpravnen())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var data = _service.GenerovatReport(datumOd, datumDo);

                ViewBag.DatumOd = datumOd.ToString("yyyy-MM-dd");
                ViewBag.DatumDo = datumDo.ToString("yyyy-MM-dd");

                return View("Index", data);
            }
            catch (ArgumentException ex)
            {
                TempData["Zprava"] = ex.Message;
                TempData["TypZpravy"] = "danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult ExportovatXml(DateTime datumOd, DateTime datumDo)
        {
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

            ViewBag.DatumOd = datumOd.ToString("yyyy-MM-dd");
            ViewBag.DatumDo = datumDo.ToString("yyyy-MM-dd");

            var data = _service.GenerovatReport(datumOd, datumDo);
            return View("Index", data);
        }
    }
}