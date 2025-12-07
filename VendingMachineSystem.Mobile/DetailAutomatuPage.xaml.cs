using VendingMachineSystem.BLL;
using VendingMachineSystem.Core;

namespace VendingMachineSystem.Mobile
{
    public partial class DetailAutomatuPage : ContentPage
    {
        private AutomatService _service;
        private int _automatId;

        public DetailAutomatuPage(int automatId)
        {
            InitializeComponent();
            _automatId = automatId;
            _service = new AutomatService();

            NaèístData();
        }

        private void NaèístData()
        {
            LayoutZasoby.Children.Clear();

            var automat = _service.GetInfoOAutomatu(_automatId);
            LblNadpis.Text = automat.Lokalita;
            LblInfo.Text = $"ID: {automat.Id} | Typ: {automat.Typ} | Stav: {automat.Stav}";

            var zasoby = _service.GetZasobyAutomatu(_automatId);

            foreach (var z in zasoby)
            {
                var kartaProduktu = VytvoritRadekProduktu(z);
                LayoutZasoby.Children.Add(kartaProduktu);
            }
        }

        private View VytvoritRadekProduktu(ZasobaAutomatu z)
        {
            bool jeMalo = z.Mnozstvi < z.MinimaleLimit;

            var frame = new Frame
            {
                Padding = 15,
                CornerRadius = 12,
                BackgroundColor = Colors.White,
                HasShadow = true,
                BorderColor = jeMalo ? Colors.Red : Colors.Transparent 
            };

            var grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = GridLength.Auto }, new RowDefinition { Height = GridLength.Auto } },
                ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } },
                RowSpacing = 10
            };

            var lblNazev = new Label
            {
                Text = z.NazevProduktu,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center
            };
            grid.Add(lblNazev, 0, 0); 

            var lblMnozstvi = new Label
            {
                Text = $"{z.Mnozstvi} ks",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = jeMalo ? Colors.Red : Colors.Green,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End
            };
            grid.Add(lblMnozstvi, 1, 0); 


            if (jeMalo)
            {
                var gridForm = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Auto } },
                    ColumnSpacing = 10
                };

                var vstup = new Entry
                {
                    Placeholder = "Poèet ks",
                    Keyboard = Keyboard.Numeric,
                    BackgroundColor = Color.FromArgb("#F0F0F0"),
                    TextColor = Colors.Black,
                    HeightRequest = 40
                };

                var btn = new Button
                {
                    Text = "Doplnit",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 40,
                    BackgroundColor = Color.FromArgb("#0d6efd"),
                    TextColor = Colors.White,
                    CornerRadius = 8
                };

                btn.Clicked += async (s, e) =>
                {
                    if (int.TryParse(vstup.Text, out int mnozstvi))
                    {
                        try
                        {
                            _service.DoplnitZbozi(z.Id, mnozstvi);
                            await DisplayAlert("Hotovo", $"Doplnìno {mnozstvi} ks.", "OK");
                            NaèístData(); 
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Chyba", ex.Message, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Chyba", "Zadejte platné èíslo.", "OK");
                    }
                };

                gridForm.Add(vstup, 0, 0); 
                gridForm.Add(btn, 1, 0);  

                grid.Add(gridForm, 0, 1);
                Grid.SetColumnSpan(gridForm, 2);
            }

            frame.Content = grid;
            return frame;
        }

        private async void OnZpetClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}