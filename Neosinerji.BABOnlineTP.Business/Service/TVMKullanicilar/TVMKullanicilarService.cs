using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;


namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMKullanicilarService : ITVMKullanicilarService
    {

        ITVMContext _TVMContext;

        public TVMKullanicilarService(ITVMContext tvmContext)
        {
            _TVMContext = tvmContext;
        }


        #region ITVMKullanicilarService Members

        public TVMKullanicilar GetTVMKullanici(int kullaniciKodu)
        {
            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.FindById(kullaniciKodu);
            return kullanici;
        }

        public IQueryable<TVMKullanicilar> GetList()
        {
            return _TVMContext.TVMKullanicilarRepository.All();
        }

        public TVMKullanicilar CreateItem(TVMKullanicilar kullanici)
        {
            TVMKullanicilar kllnc = _TVMContext.TVMKullanicilarRepository.Create(kullanici);
            _TVMContext.Commit();
            return kllnc;
        }

        public bool UpdateItem(TVMKullanicilar kullanici)
        {
            _TVMContext.TVMKullanicilarRepository.Update(kullanici);
            _TVMContext.Commit();
            return true;
        }

        public List<TVMKullanicilar> GetListTVMKullanicilarByTVMKodu(int tvmKodu)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvmKodu).ToList();
        }

        #endregion

    }
}
