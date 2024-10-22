using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class BransService : IBransService
    {
        IParametreContext _UnitOfWork;
        ITVMContext _TVMContext;

        public BransService(IParametreContext unitOfWork, ITVMContext tvmContext)
        {
            _UnitOfWork = unitOfWork;
            _TVMContext = tvmContext;
        }


        #region IBransService Members

        public Bran GetBrans(int bransKodu)
        {
            Bran brn = _UnitOfWork.BranRepository.FindById(bransKodu);
            return brn;
        }
        public string GetBransUnvani(int bransKodu)
        {
            Bran brn = _UnitOfWork.BranRepository.FindById(bransKodu);
            return brn.BransAdi;
        }

        public List<Bran> GetList(string TvmTipi)
        {
            if (TvmTipi == "1")
            {
                return _UnitOfWork.BranRepository.All().ToList<Bran>();
            }
            else
            {
                return _UnitOfWork.BranRepository.All().Where(z => z.BransKodu < 49).ToList<Bran>();
            }
        }
        public List<Bran> GetListBroker()
        {
            return _UnitOfWork.BranRepository.All().Where(z => z.BransKodu > 49).ToList<Bran>();
        }

        public List<Bran> GetList(int tvmKodu)
        {
            var yetkiler = _TVMContext.TVMUrunYetkileriRepository.All().Where(w => w.TVMKodu == tvmKodu && w.Teklif == 1);
            var urunler = _UnitOfWork.UrunRepository.All();

            var urunbrans = (from y in yetkiler
                            join u in urunler on y.BABOnlineUrunKodu equals u.UrunKodu
                            select new { u.BransKodu })
                            .GroupBy(g => g.BransKodu)
                            .Select(s => new { BransKodu = s.Key });

            var branslar = _UnitOfWork.BranRepository.All();

            return (from b in branslar
                   join u in urunbrans on b.BransKodu equals u.BransKodu
                   select b).ToList();
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
