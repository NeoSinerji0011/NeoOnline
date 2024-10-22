using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMKullaniciDurumTarihcesiRepository : IRepository<TVMKullaniciDurumTarihcesi>
    { }
    public class TVMKullaniciDurumTarihcesiRepository : Repository<TVMKullaniciDurumTarihcesi>, ITVMKullaniciDurumTarihcesiRepository
    {
        public TVMKullaniciDurumTarihcesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
