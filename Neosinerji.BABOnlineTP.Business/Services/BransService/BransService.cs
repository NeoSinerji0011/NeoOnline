using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business.Services
{
    public class BransService : IBransService
    {
        IUnitOfWork _UnitOfWork;
        public BransService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region IBransService Member

        public Bran GetBrans(int bransKodu)
        {
            Bran brans = _UnitOfWork.BranRepository.FindById(bransKodu);
            return brans;
        }

        public IQueryable<Bran> GetList()
        {
            return _UnitOfWork.BranRepository.All();
        }

        public Bran CreateItem(Bran brans)
        {
            Bran brn = _UnitOfWork.BranRepository.Create(brans);
            _UnitOfWork.Commit();
            return brn;
        }

        public bool UpdateItem(Bran brans)
        {
            _UnitOfWork.BranRepository.Update(brans);
            _UnitOfWork.Commit();
            return true;
        }

        #endregion


    }
}
