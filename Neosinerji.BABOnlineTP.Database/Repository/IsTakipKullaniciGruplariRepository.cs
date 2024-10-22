
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipKullaniciGruplariRepository : IRepository<IsTakipKullaniciGruplari>
    { }
    public class IsTakipKullaniciGruplariRepository : Repository<IsTakipKullaniciGruplari>, IIsTakipKullaniciGruplariRepository
    {
        public IsTakipKullaniciGruplariRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}