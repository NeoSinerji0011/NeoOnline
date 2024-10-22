using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMKullaniciSifremiUnuttumRepository : IRepository<TVMKullaniciSifremiUnuttum>
    { }

    public class TVMKullaniciSifremiUnuttumRepository : Repository<TVMKullaniciSifremiUnuttum>, ITVMKullaniciSifremiUnuttumRepository
    {
        public TVMKullaniciSifremiUnuttumRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
