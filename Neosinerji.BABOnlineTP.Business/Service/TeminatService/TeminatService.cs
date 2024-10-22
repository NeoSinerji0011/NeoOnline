using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TeminatService:ITeminatService
    {
        IParametreContext _UnitOfWork;
        public TeminatService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region ITeminatService Members

        public DataTableList PagedList(DataTableParameters<Teminat> teminatList)
        {
            IQueryable<Teminat> query = _UnitOfWork.TeminatRepository.All();

            if (!String.IsNullOrEmpty(teminatList.SearchKeyword))
            {
                int numeric = int.MinValue;
                if (int.TryParse(teminatList.SearchKeyword, out numeric))
                {
                    query = query.Where(w => w.TeminatKodu == numeric ||
                                             w.TeminatAdi.StartsWith(teminatList.SearchKeyword));
                }
                else
                {
                    query = query.Where(w => w.TeminatAdi.StartsWith(teminatList.SearchKeyword));
                }
            }

            int totalRowCount = 0;
            query = _UnitOfWork.TeminatRepository.Page(query, teminatList.OrderByProperty, teminatList.IsAscendingOrder, teminatList.Page, teminatList.PageSize, out totalRowCount);

            return teminatList.Prepare(query, totalRowCount);
        }

        public Teminat GetTeminat(int teminatKodu)
        {
            Teminat teminat = _UnitOfWork.TeminatRepository.FindById(teminatKodu);
            return teminat;
        }

        public IQueryable<Teminat> GetList()
        {
            return _UnitOfWork.TeminatRepository.All();
        }

        public Teminat CreateItem(Teminat teminat)
        {
            Teminat tmnt = _UnitOfWork.TeminatRepository.Create(teminat);
            _UnitOfWork.Commit();
            return tmnt;
        }

        public bool UpdateItem(Teminat teminat)
        {
            _UnitOfWork.TeminatRepository.Update(teminat);
            _UnitOfWork.Commit();
            return true;
        }

        #endregion
        
    }
}
