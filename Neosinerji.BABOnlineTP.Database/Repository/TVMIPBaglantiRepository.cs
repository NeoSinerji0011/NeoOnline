using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMIPBaglantiRepository : IRepository<TVMIPBaglanti> { }
    public class TVMIPBaglantiRepository : Repository<TVMIPBaglanti>, ITVMIPBaglantiRepository
    {
        public TVMIPBaglantiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
