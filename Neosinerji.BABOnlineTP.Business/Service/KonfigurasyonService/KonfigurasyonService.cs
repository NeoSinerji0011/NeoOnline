using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KonfigurasyonService : IKonfigurasyonService
    {
        IParametreContext _UnitOfWork;

        public KonfigurasyonService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        #region IKonfigurasyonService Members

        public string GetKonfigDeger(string Kod)
        {
            Konfigurasyon konfig = _UnitOfWork.KonfigurasyonRepository.FindById(Kod);
            if (konfig != null)
                return konfig.Deger;

            return String.Empty;
        }

        public KonfigTable GetKonfig(string[] Kodlar)
        {
            Konfigurasyon[] konfig = _UnitOfWork.KonfigurasyonRepository
                                                .Filter(f => Kodlar.Contains(f.Kod))
                                                .ToArray<Konfigurasyon>();

            return new KonfigTable(konfig);
        }

        public Konfigurasyon GetKonfig(string Kod)
        {
            return _UnitOfWork.KonfigurasyonRepository.FindById(Kod);
        }

        public List<Konfigurasyon> GetList()
        {
            return _UnitOfWork.KonfigurasyonRepository.All().ToList<Konfigurasyon>();
        }

        public Konfigurasyon CreateItem(Konfigurasyon konfigurasyon)
        {
            Konfigurasyon knfg = _UnitOfWork.KonfigurasyonRepository.Create(konfigurasyon);
            _UnitOfWork.Commit();
            return knfg;
        }

        public bool UpdateItem(Konfigurasyon konfigurasyon)
        {
            _UnitOfWork.KonfigurasyonRepository.Update(konfigurasyon);
            _UnitOfWork.Commit();
            return true;
        }

        public void DeleteKonfig(string Kod)
        {
            _UnitOfWork.KonfigurasyonRepository.Delete(m => m.Kod == Kod);
            _UnitOfWork.Commit();
        }
        #endregion

    }
}
