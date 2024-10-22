using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IWEBServisLogRepository : IRepository<WEBServisLog>
    { }

    public class WEBServisLogRepository : Repository<WEBServisLog>, IWEBServisLogRepository
    {
        public WEBServisLogRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
