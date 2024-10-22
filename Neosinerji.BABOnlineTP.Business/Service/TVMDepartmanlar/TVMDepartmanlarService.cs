using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;


namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMDepartmanlarService : ITVMDepartmanlarService
    {

        ITVMContext _TVMContext;

        public TVMDepartmanlarService(ITVMContext tvmContext)
        {
            _TVMContext = tvmContext;
        }


        #region TVMDepartmanlarService Members

        public TVMDepartmanlar GetTVMDepartman(int departmanKodu)
        {
            TVMDepartmanlar departman = _TVMContext.TVMDepartmanlarRepository.Find(m => m.DepartmanKodu == departmanKodu);
            return departman;
        }

        public IQueryable<TVMDepartmanlar> GetList()
        {
            return _TVMContext.TVMDepartmanlarRepository.All();
        }

        public TVMDepartmanlar CreateItem(TVMDepartmanlar departman)
        {
            TVMDepartmanlar dprtmn = _TVMContext.TVMDepartmanlarRepository.Create(departman);
            _TVMContext.Commit();

            return dprtmn;
        }

        public bool UpdateItem(TVMDepartmanlar departman)
        {
            _TVMContext.TVMDepartmanlarRepository.Update(departman);
            _TVMContext.Commit();

            return true;
        }

        #endregion

    }
}
