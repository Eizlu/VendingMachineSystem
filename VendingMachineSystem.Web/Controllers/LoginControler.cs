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

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Prihlasit(string login, string heslo)
        {
            var uzivatel = _service.OveritUzivatele(login, heslo);

            if (uzivatel != null)
            {
                HttpContext.Session.SetString("JmenoUzivatele", uzivatel.Jmeno);
                HttpContext.Session.SetString("Role", uzivatel.Role);

                return RedirectToAction("Index", "Home");
            }
            else
            {
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