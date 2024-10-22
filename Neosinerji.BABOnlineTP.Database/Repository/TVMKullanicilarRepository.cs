using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMKullanicilarRepository : IRepository<TVMKullanicilar>
    { }
    public class TVMKullanicilarRepository : Repository<TVMKullanicilar>, ITVMKullanicilarRepository
    {
        public TVMKullanicilarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
