using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VendingMachineSystem.BLL; 
using VendingMachineSystem.Web.Models;

namespace VendingMachineSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private AutomatService _service;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _service = new AutomatService();
        }

        public IActionResult Index()
        {
            var vsechnyAutomaty = _service.GetVsechnyAutomaty();

            var dashboardData = new List<AutomatKartaViewModel>();

            foreach (var automat in vsechnyAutomaty)
            {
                var zasoby = _service.GetZasobyAutomatu(automat.Id);

                var karta = new AutomatKartaViewModel
                {
                    Info = automat,
                    Zasoby = zasoby
                };

                dashboardData.Add(karta);
            }

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

        public IActionResult Detail(int id)
        {
            var automat = _service.GetInfoOAutomatu(id);

            ViewBag.Automat = automat;

            var zasoby = _service.GetZasobyAutomatu(id);

            return View(zasoby);
        }

        [HttpPost]
        public IActionResult Doplnit(int zasobaId, int automatId, int mnozstvi)
        {
            try
            {
                _service.DoplnitZbozi(zasobaId, mnozstvi);

                TempData["Zprava"] = "Zboží bylo úspìšnì doplnìno.";
                TempData["TypZpravy"] = "success"; 
            }
            catch (ArgumentException ex)
            {
                TempData["Zprava"] = ex.Message; 
                TempData["TypZpravy"] = "danger"; 
            }

            return RedirectToAction("Detail", new { id = automatId });
        }
    }
}