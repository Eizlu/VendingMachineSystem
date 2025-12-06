using VendingMachineSystem.BLL;

namespace VendingMachineSystem.Mobile
{
    public partial class LoginPage : ContentPage
    {
        private UzivatelService _service;

        public LoginPage()
        {
            InitializeComponent();
            _service = new UzivatelService();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            string login = EntLogin.Text;
            string heslo = EntHeslo.Text;

            // 1. Ovìøení pøes BLL
            var uzivatel = _service.OveritUzivatele(login, heslo);

            if (uzivatel != null)
            {
                // 2. Uložení do "Session" (naše statická promìnná)
                App.PrihlasenyUzivatel = uzivatel;

                // 3. Pøepnutí na Hlavní stránku (Natvrdo vymìníme koøen aplikace)
                // Tím zmizí tlaèítko "Zpìt" na login, což je správnì.
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                DisplayAlert("Chyba", "Špatné jméno nebo heslo", "OK");
            }
        }
    }
}