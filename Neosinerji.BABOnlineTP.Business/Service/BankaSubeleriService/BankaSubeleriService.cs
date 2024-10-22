using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class BankaSubeleriService : IBankaSubeleriService
    {
        IParametreContext _UnitOfWork;

        public BankaSubeleriService()
        {
            _UnitOfWork = DependencyResolver.Current.GetService<IParametreContext>();
        }

        #region BankaSubeleri

        public List<Bankalar> GetListBanka()
        {
            List<Bankalar> banks = new List<Bankalar>();

            IQueryable<BankaSubeleri> bankalar = _UnitOfWork.BankaSubeleriRepository.All();

            banks = (from b in bankalar
                     select new Bankalar
                     {
                         BankaAdi = b.Banka,
                         BankaKodu = b.Banka
                     }).Distinct().OrderBy(m => m.BankaAdi).ToList<Bankalar>();

            return banks;
        }

        public List<BankaSubeleri> GetListBankaSubeleri(string BankaKodu)
        {
            return _UnitOfWork.BankaSubeleriRepository.Filter(s => s.Banka == BankaKodu).OrderBy(s => s.Sube).ToList<BankaSubeleri>();
        }

        #endregion
    }
}
