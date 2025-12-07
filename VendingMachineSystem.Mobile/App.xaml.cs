using VendingMachineSystem.Core;

namespace VendingMachineSystem.Mobile
{
    public partial class App : Application
    {
        public static Uzivatel PrihlasenyUzivatel { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new LoginPage();
        }
    }
}