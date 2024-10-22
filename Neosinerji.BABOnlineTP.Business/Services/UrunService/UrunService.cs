using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;


namespace Neosinerji.BABOnlineTP.Business.Services
{
    public class UrunService : IUrunService
    {
        IUnitOfWork _UnitOfWork;
        public UrunService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region IUrunService Member

        public Urun GetUrun(int urunKodu)
        {
            Urun urun = _UnitOfWork.UrunRepository.FindById(urunKodu);
            return urun;
        }

        public IQueryable<Urun> GetList()
        {
            return _UnitOfWork.UrunRepository.All();
        }

        public Urun CreateItem(Urun urun)
        {
            Urun urn = _UnitOfWork.UrunRepository.Create(urun);
            _UnitOfWork.Commit();
            return urn;
        }

        public bool UpdateItem(Urun urun)
        {
            _UnitOfWork.UrunRepository.Update(urun);
            _UnitOfWork.Commit();
            return true;
        }
        #endregion
    }
}
