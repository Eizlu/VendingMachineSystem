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

            var uzivatel = _service.OveritUzivatele(login, heslo);

            if (uzivatel != null)
            {
                App.PrihlasenyUzivatel = uzivatel;

                Application.Current.MainPage = new MainPage();
            }
            else
            {
                DisplayAlert("Chyba", "Špatné jméno nebo heslo", "OK");
            }
        }
    }
}