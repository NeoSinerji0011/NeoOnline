using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class VergiService : IVergiService
    {
        IParametreContext _UnitOfWork;

        public VergiService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        #region IVergiService Members

        public DataTableList PagedList(DataTableParameters<Vergi> vergiList)
        {
            IQueryable<Vergi> query = _UnitOfWork.VergiRepository.All();

            if (!String.IsNullOrEmpty(vergiList.SearchKeyword))
            {
                int numeric = int.MinValue;
                if (int.TryParse(vergiList.SearchKeyword, out numeric))
                {
                    query = query.Where(w => w.VergiKodu == numeric ||
                                             w.VergiAdi.StartsWith(vergiList.SearchKeyword));
                }
                else
                {
                    query = query.Where(w => w.VergiAdi.StartsWith(vergiList.SearchKeyword));
                }
            }

            int totalRowCount = 0;
            query = _UnitOfWork.VergiRepository.Page(query, vergiList.OrderByProperty, vergiList.IsAscendingOrder, vergiList.Page, vergiList.PageSize, out totalRowCount);

            return vergiList.Prepare(query, totalRowCount);
        }

        public Vergi GetVergi(int vergiKodu)
        {
            Vergi vergi = _UnitOfWork.VergiRepository.FindById(vergiKodu);
            return vergi;
        }

        public IQueryable<Vergi> GetList()
        {
            return _UnitOfWork.VergiRepository.All();
        }

        public Vergi CreateItem(Vergi Vergi)
        {
            Vergi vrg = _UnitOfWork.VergiRepository.Create(Vergi);
            _UnitOfWork.Commit();
            return vrg;
        }

        public bool UpdateItem(Vergi vergi)
        {
            _UnitOfWork.VergiRepository.Update(vergi);
            _UnitOfWork.Commit();
            return true;
        }

        #endregion

    }
}
