using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipKullaniciGrupKullanicilariRepository : IRepository<IsTakipKullaniciGrupKullanicilari>
    { }
    public class IsTakipKullaniciGrupKullanicilariRepository : Repository<IsTakipKullaniciGrupKullanicilari>, IIsTakipKullaniciGrupKullanicilariRepository
    {
        public IsTakipKullaniciGrupKullanicilariRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}