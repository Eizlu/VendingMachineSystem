using System.Collections.Generic;
using VendingMachineSystem.Core;
using VendingMachineSystem.DAL;

namespace VendingMachineSystem.BLL
{
    public class AutomatService
    {
        private AutomatRepository _repository;

        public AutomatService()
        {
            // Tady Mozek říká: "Budu potřebovat Skladníka"
            _repository = new AutomatRepository();
        }

        // Metoda, kterou zavolá Webová stránka: "Dej mi seznam automatů"
        public List<Automat> GetVsechnyAutomaty()
        {
            // Mozek řekne Skladníkovi: "Skoč pro to"
            return _repository.GetAll();
        }

        // Metoda: Dej mi informace o konkrétním automatu (lokalita, stav...)
        public Automat GetInfoOAutomatu(int id)
        {
            return _repository.GetById(id);
        }

        // Metoda: Dej mi seznam zboží v tom automatu
        public List<ZasobaAutomatu> GetZasobyAutomatu(int id)
        {
            return _repository.GetZasoby(id);
        }

        // Příkaz: Doplň tuto konkrétní zásobu
        public void DoplnitZbozi(int zasobaId, int mnozstvi)
        {
            // VALIDACE (Alternativní průběh):
            // Pokud je číslo 0 nebo menší, systém akci odmítne a vyhodí chybu.
            if (mnozstvi <= 0)
            {
                throw new ArgumentException("Množství pro doplnění musí být kladné číslo!");
            }

            // Můžeme přidat i horní limit (např. automat neunese víc než 50 ks najednou)
            if (mnozstvi > 50)
            {
                throw new ArgumentException("Nelze doplnit více než 50 kusů najednou.");
            }

            // Pokud je vše OK, zavoláme skladníka
            _repository.DoplnitZasobu(zasobaId, mnozstvi);
        }
    }
}