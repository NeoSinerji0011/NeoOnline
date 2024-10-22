using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMDurumTarihcesiRepository : IRepository<TVMDurumTarihcesi> { }
    public class TVMDurumTarihcesiRepository : Repository<TVMDurumTarihcesi>, ITVMDurumTarihcesiRepository
    {
        public TVMDurumTarihcesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
