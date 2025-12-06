using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VendingMachineSystem.BLL; // Pøidat using pro Mozek
using VendingMachineSystem.Web.Models;

namespace VendingMachineSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // 1. Pøidáme si promìnnou pro naši službu
        private AutomatService _service;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            // 2. Vytvoøíme instanci služby
            _service = new AutomatService();
        }

        public IActionResult Index()
        {
            // 1. Získáme seznam všech automatù
            var vsechnyAutomaty = _service.GetVsechnyAutomaty();

            // 2. Pøipravíme si seznam kartièek pro View
            var dashboardData = new List<AutomatKartaViewModel>();

            foreach (var automat in vsechnyAutomaty)
            {
                // Pro každý automat naèteme jeho zásoby (aby byly vidìt rovnou na kartì)
                var zasoby = _service.GetZasobyAutomatu(automat.Id);

                // Zabalíme to do balíèku
                var karta = new AutomatKartaViewModel
                {
                    Info = automat,
                    Zasoby = zasoby
                };

                dashboardData.Add(karta);
            }

            // 3. Pošleme data do View
            return View(dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Akce pro zobrazení detailu (volá se napø. jako /Home/Detail/1)
        public IActionResult Detail(int id)
        {
            // 1. Získáme info o automatu (abychom dali do nadpisu "Lokalita: Nádraží")
            var automat = _service.GetInfoOAutomatu(id);

            // Uložíme si to do "batohu" ViewBag, abychom to mohli vypsat v nadpisu
            ViewBag.Automat = automat;

            // 2. Získáme seznam produktù
            var zasoby = _service.GetZasobyAutomatu(id);

            // 3. Pošleme seznam produktù na stránku
            return View(zasoby);
        }

        // Akce pro doplnìní (volá se po kliknutí na tlaèítko)
        [HttpPost]
        public IActionResult Doplnit(int zasobaId, int automatId, int mnozstvi)
        {
            try
            {
                // Zkusíme provést akci
                _service.DoplnitZbozi(zasobaId, mnozstvi);

                // Když se to povede, uložíme si zprávu o úspìchu
                TempData["Zprava"] = "Zboží bylo úspìšnì doplnìno.";
                TempData["TypZpravy"] = "success"; // Zelená barva
            }
            catch (ArgumentException ex)
            {
                // ALTERNATIVNÍ SCÉNÁØ:
                // Když Mozek vyhodí chybu (validace), chytíme ji tady.
                TempData["Zprava"] = ex.Message; // "Množství musí být kladné..."
                TempData["TypZpravy"] = "danger"; // Èervená barva
            }

            return RedirectToAction("Detail", new { id = automatId });
        }
    }
}