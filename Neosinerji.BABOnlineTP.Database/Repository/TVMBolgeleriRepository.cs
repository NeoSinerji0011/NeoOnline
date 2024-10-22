using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMBolgeleriRepository : IRepository<TVMBolgeleri> { }
    public class TVMBolgeleriRepository : Repository<TVMBolgeleri>, ITVMBolgeleriRepository
    {
        public TVMBolgeleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
