using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business.Services
{
    public class SoruService : ISoruService
    {
        IUnitOfWork _UnitOfWork;

        public SoruService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region ISoruService Member

        public Soru GetSoru(int soruKodu)
        {
            Soru soru = _UnitOfWork.SoruRepository.FindById(soruKodu);
            return soru;
        }

        public IQueryable<Soru> GetList()
        {
            return _UnitOfWork.SoruRepository.All();
        }

        public Soru CreateItem(Soru soru)
        {
            Soru sr = _UnitOfWork.SoruRepository.Create(soru);
            _UnitOfWork.Commit();
            return sr;
        }

        public bool UpdateItem(Soru soru)
        {
            _UnitOfWork.SoruRepository.Update(soru);
            _UnitOfWork.Commit();
            return true;
        }

        #endregion

    }
}
