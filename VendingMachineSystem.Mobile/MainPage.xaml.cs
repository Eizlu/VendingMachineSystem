using VendingMachineSystem.BLL;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.Mobile
{
    public partial class MainPage : ContentPage
    {
        private AutomatService _service;

        public MainPage()
        {
            InitializeComponent();
            _service = new AutomatService();

            // Zobrazíme, kdo je přihlášen
            if (App.PrihlasenyUzivatel != null)
            {
                LblUzivatel.Text = App.PrihlasenyUzivatel.Jmeno;
                LblRole.Text = App.PrihlasenyUzivatel.Role;
            }
        }

        // TATO METODA SE SPUSTÍ AUTOMATICKY, KDYŽ SE STRÁNKA UKÁŽE
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NačístAutomaty();
        }

        private void NačístAutomaty()
        {
            try
            {
                SeznamAutomatuLayout.Children.Clear();
                var automaty = _service.GetVsechnyAutomaty();

                if (automaty.Count == 0)
                {
                    SeznamAutomatuLayout.Children.Add(new Label { Text = "Žádné automaty k zobrazení.", TextColor = Colors.Black });
                    return;
                }

                foreach (var automat in automaty)
                {
                    // Tady taky načteme zásoby pro náhled
                    var zasoby = _service.GetZasobyAutomatu(automat.Id);
                    var karta = VytvoritKartu(automat, zasoby);
                    SeznamAutomatuLayout.Children.Add(karta);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Chyba", "Data nelze načíst: " + ex.Message, "OK");
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // 1. Vymazat uživatele
            App.PrihlasenyUzivatel = null;

            // 2. Přepnout zpět na Login (reset aplikace)
            Application.Current.MainPage = new LoginPage();
        }

        private View VytvoritKartu(Automat automat, List<ZasobaAutomatu> zasoby)
        {
            // ... kód z minulé zprávy ...
            // (Pokud ho nemáš, řekni, pošlu ho znovu)
            var frame = new Frame
            {
                BorderColor = Colors.LightGray,
                CornerRadius = 12,
                Padding = 0,
                BackgroundColor = Colors.White,
                HasShadow = true,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var stack = new VerticalStackLayout();

            // Hlavička karty
            var header = new VerticalStackLayout { BackgroundColor = Color.FromArgb("#f8f9fa"), Padding = 15 };
            header.Children.Add(new Label { Text = automat.Lokalita, FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });

            var lblStav = new Label { Text = automat.Stav, FontSize = 12, FontAttributes = FontAttributes.Bold };
            lblStav.TextColor = automat.Stav == "Online" ? Colors.Green : Colors.Red;
            header.Children.Add(lblStav);

            stack.Children.Add(header);

            // Tělo karty - Seznam produktů (jako na webu)
            var body = new VerticalStackLayout { Padding = 15, Spacing = 5 };

            // Vypíšeme prvních 5 produktů
            foreach (var z in zasoby.Take(5))
            {
                var radek = new HorizontalStackLayout { Spacing = 10 };
                radek.Children.Add(new Label { Text = z.NazevProduktu, WidthRequest = 180, LineBreakMode = LineBreakMode.TailTruncation, TextColor = Colors.Gray });

                if (z.Mnozstvi < z.MinimaleLimit)
                {
                    radek.Children.Add(new Label { Text = $"{z.Mnozstvi} ks (!)", TextColor = Colors.Red, FontAttributes = FontAttributes.Bold });
                }
                else
                {
                    radek.Children.Add(new Label { Text = $"{z.Mnozstvi} ks", TextColor = Colors.Green });
                }
                body.Children.Add(radek);
            }
            stack.Children.Add(body);

            // Patička - Tlačítko Detail
            var btnDetail = new Button
            {
                Text = "Spravovat zásoby",
                Margin = 15,
                BackgroundColor = Color.FromArgb("#0d6efd"),
                TextColor = Colors.White
            };

            // Kliknutí otevře Detail stránku
            btnDetail.Clicked += async (s, e) => {
                await Navigation.PushModalAsync(new DetailAutomatuPage(automat.Id));
            };

            stack.Children.Add(btnDetail);
            frame.Content = stack;

            return frame;
        }
    }
}