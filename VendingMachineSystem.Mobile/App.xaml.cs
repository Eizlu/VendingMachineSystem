using VendingMachineSystem.Core;

namespace VendingMachineSystem.Mobile
{
    public partial class App : Application
    {
        // Globální proměnná pro přihlášeného uživatele
        public static Uzivatel PrihlasenyUzivatel { get; set; }

        public App()
        {
            InitializeComponent();

            // Tady nastavujeme, že aplikace startuje Login stránkou
            MainPage = new LoginPage();
        }
    }
}