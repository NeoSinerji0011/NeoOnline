using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface INeoConnectLogRepository : IRepository<NeoConnectLog>
    { }
    public class NeoConnectLogRepository : Repository<NeoConnectLog>, INeoConnectLogRepository
    {

        public NeoConnectLogRepository(DbContext dbContext)
            : base(dbContext)
        {

        }

    }
}
