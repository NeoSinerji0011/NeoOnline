using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;


namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMBolgeleriService : ITVMBolgeleriService
    {

        ITVMContext _TVMContext;

        public TVMBolgeleriService(ITVMContext tvmContext)
        {
           // _TVMContext = tvmContext;
        }


        #region TVMBolgeleriService Members

        public TVMBolgeleri GetTVMBolge(int bolgeKodu)
        {
            TVMBolgeleri bolge = _TVMContext.TVMBolgeleriRepository.Find(m => m.TVMBolgeKodu == bolgeKodu);
            return bolge;
        }

        public IQueryable<TVMBolgeleri> GetList()
        {
            return _TVMContext.TVMBolgeleriRepository.All();
        }

        public IQueryable<TVMBolgeleri> GetListByTVM(int tvmKodu)
        {
            return _TVMContext.TVMBolgeleriRepository.Filter(m => m.TVMKodu == tvmKodu);
        }

        public TVMBolgeleri CreateItem(TVMBolgeleri bolge)
        {
            TVMBolgeleri blg = _TVMContext.TVMBolgeleriRepository.Create(bolge);
            _TVMContext.Commit();

            return blg;
        }

        public bool UpdateItem(TVMBolgeleri bolge)
        {
            _TVMContext.TVMBolgeleriRepository.Update(bolge);
            _TVMContext.Commit();

            return true;
        }

        #endregion

    }
}
