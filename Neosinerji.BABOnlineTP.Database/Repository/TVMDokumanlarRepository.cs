using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMDokumanlarRepository : IRepository<TVMDokumanlar> { }
    public class TVMDokumanlarRepository : Repository<TVMDokumanlar>, ITVMDokumanlarRepository
    {
        public TVMDokumanlarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
