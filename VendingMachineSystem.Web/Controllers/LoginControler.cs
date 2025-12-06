using Microsoft.AspNetCore.Mvc;
using VendingMachineSystem.BLL;

namespace VendingMachineSystem.Web.Controllers
{
    public class LoginController : Controller
    {
        private UzivatelService _service;

        public LoginController()
        {
            _service = new UzivatelService();
        }

        // Zobrazí formulář (GET)
        public IActionResult Index()
        {
            return View();
        }

        // Zpracuje odeslaný formulář (POST)
        [HttpPost]
        public IActionResult Prihlasit(string login, string heslo)
        {
            var uzivatel = _service.OveritUzivatele(login, heslo);

            if (uzivatel != null)
            {
                // Uložíme uživatele do "paměti" prohlížeče (Session)
                HttpContext.Session.SetString("JmenoUzivatele", uzivatel.Jmeno);
                HttpContext.Session.SetString("Role", uzivatel.Role);

                // Přesměrujeme na hlavní dashboard
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Chyba - vrátíme ho zpátky na login
                ViewBag.Chyba = "Špatné jméno nebo heslo!";
                return View("Index");
            }
        }

        public IActionResult Odhlasit()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}