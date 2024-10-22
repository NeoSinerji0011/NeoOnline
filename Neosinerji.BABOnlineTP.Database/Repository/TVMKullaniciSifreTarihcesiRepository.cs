using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMKullaniciSifreTarihcesiRepository : IRepository<TVMKullaniciSifreTarihcesi>
    { }
    public class TVMKullaniciSifreTarihcesiRepository : Repository<TVMKullaniciSifreTarihcesi>, ITVMKullaniciSifreTarihcesiRepository
    {
        public TVMKullaniciSifreTarihcesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
