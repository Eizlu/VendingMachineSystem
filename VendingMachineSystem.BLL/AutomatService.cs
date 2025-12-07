using System;
using System.Collections.Generic;
using VendingMachineSystem.Core;
using VendingMachineSystem.DAL;

namespace VendingMachineSystem.BLL
{
    public class AutomatService
    {
        private AutomatRepository _repository;
        private List<INotifikaceObserver> _observers = new List<INotifikaceObserver>();

        public AutomatService()
        {
            _repository = new AutomatRepository();
            PripojitObserver(new DebugObserver());
        }

        public void PripojitObserver(INotifikaceObserver observer)
        {
            _observers.Add(observer);
        }

        public void OdpojitObserver(INotifikaceObserver observer)
        {
            _observers.Remove(observer);
        }

        private void Notifikovat(string zprava)
        {
            foreach (var observer in _observers)
            {
                observer.PrijmoutNotifikaci(zprava);
            }
        }

        public List<Automat> GetVsechnyAutomaty()
        {
            return _repository.GetAll();
        }

        public Automat GetInfoOAutomatu(int id)
        {
            return _repository.GetById(id);
        }

        public List<ZasobaAutomatu> GetZasobyAutomatu(int id)
        {
            var zasoby = _repository.GetZasoby(id);

            foreach (var z in zasoby)
            {
                if (z.Mnozstvi < z.MinimaleLimit)
                {
                    Notifikovat($"POZOR: V automatu ID {id} dochází produkt {z.NazevProduktu} (Zbývá {z.Mnozstvi} ks).");
                }
            }

            return _repository.GetZasoby(id);
        }

        public void DoplnitZbozi(int zasobaId, int mnozstvi)
        {
            if (mnozstvi <= 0)
            {
                throw new ArgumentException("Množství pro doplnění musí být kladné číslo!");
            }

            if (mnozstvi > 50)
            {
                throw new ArgumentException("Nelze doplnit více než 50 kusů najednou.");
            }

            _repository.DoplnitZasobu(zasobaId, mnozstvi);

            Notifikovat($"Zásoba ID {zasobaId} byla doplněna o {mnozstvi} ks.");
        }
    }
}