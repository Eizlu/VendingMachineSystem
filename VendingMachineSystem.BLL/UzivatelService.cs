using VendingMachineSystem.Core;
using VendingMachineSystem.DAL;

namespace VendingMachineSystem.BLL
{
    public class UzivatelService
    {
        private UzivatelRepository _repository;

        public UzivatelService()
        {
            _repository = new UzivatelRepository();
        }

        public Uzivatel OveritUzivatele(string login, string heslo)
        {
            return _repository.Login(login, heslo);
        }
    }
}