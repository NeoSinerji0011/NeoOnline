using Neosinerji.BABOnlineTP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UrunParametreleriService : IUrunParametreleriService
    {
        #region Fields

        private readonly IParametreContext _db;

        #endregion

        #region Constructors

        public UrunParametreleriService(IParametreContext db)
        {
            this._db = db;
        }

        #endregion

        #region Methods

        public UrunParametreleri GetUrunParametre(string kod)
        {
            return _db.UrunParametreleriRepository.FindById(kod);
        }

        public string GetUrunParametreValue(string kod)
        {
            string result = String.Empty;

            UrunParametreleri param = _db.UrunParametreleriRepository.FindById(kod);

            if (param != null)
            {
                result = param.Deger;
            }

            return result;
        }

        public UrunParamsTable GetListUrunParametre(string[] kodlar)
        {
            UrunParametreleri[] parametler = _db.UrunParametreleriRepository
                                                .Filter(s => kodlar.Contains(s.Kod))
                                                .ToArray<UrunParametreleri>();

            return new UrunParamsTable(parametler);
        }

        #endregion
    }
}
