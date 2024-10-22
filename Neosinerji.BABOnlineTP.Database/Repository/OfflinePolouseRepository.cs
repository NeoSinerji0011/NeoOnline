using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IOfflinePolouseRepository : IRepository<OfflinePolouse>
    {

    }
    public class OfflinePolouseRepository : Repository<OfflinePolouse>, IOfflinePolouseRepository
    {
        public OfflinePolouseRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
