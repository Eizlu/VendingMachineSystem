using Microsoft.AspNetCore.Mvc;
using VendingMachineSystem.BLL;

namespace VendingMachineSystem.Web.Controllers
{
    public class SkladController : Controller
    {
        private SkladService _service;

        public SkladController()
        {
            _service = new SkladService();
        }

        public IActionResult Index()
        {
            var zasoby = _service.GetPrehledSkladu();
            return View(zasoby);
        }

        [HttpPost]
        public IActionResult VytvoritObjednavku(int produktId, int mnozstvi)
        {
            try
            {
                // Předáme zadané množství do BLL
                _service.ObjednatZbozi(produktId, mnozstvi);

                TempData["Zprava"] = "Objednávka byla úspěšně vytvořena a odeslána dodavateli.";
                TempData["TypZpravy"] = "success";
            }
            catch (ArgumentException ex)
            {
                // Chyba validace (např. záporné číslo)
                TempData["Zprava"] = ex.Message;
                TempData["TypZpravy"] = "danger";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Exportovat()
        {
            _service.VygenerovatAExportovatReport();

            // Vrátíme se na seznam a zobrazíme zprávu (přes TempData)
            TempData["Zprava"] = "Report byl uložen do XML souboru v kořenové složce projektu.";
            return RedirectToAction("Index");
        }
    }
}