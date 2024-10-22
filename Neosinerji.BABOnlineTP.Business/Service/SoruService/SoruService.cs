using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class SoruService : ISoruService
    {
        IParametreContext _UnitOfWork;
        public SoruService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region ISoruService Members

        public DataTableList PagedList(DataTableParameters<Soru> soruList)
        {
            IQueryable<Soru> query = _UnitOfWork.SoruRepository.All();

            if (!String.IsNullOrEmpty(soruList.SearchKeyword))
            {
                int numeric = int.MinValue;
                if (int.TryParse(soruList.SearchKeyword, out numeric))
                {
                    query = query.Where(w => w.SoruKodu == numeric ||
                                             w.SoruAdi.StartsWith(soruList.SearchKeyword));
                }
                else
                {
                    query = query.Where(w => w.SoruAdi.StartsWith(soruList.SearchKeyword));
                }
            }

            int totalRowCount = 0;
            query = _UnitOfWork.SoruRepository.Page(query, soruList.OrderByProperty, soruList.IsAscendingOrder, soruList.Page, soruList.PageSize, out totalRowCount);

            return soruList.Prepare(query, totalRowCount);
        }

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
