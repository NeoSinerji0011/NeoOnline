
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IReasurorGenelRepository : IRepository<ReasurorGenel>
    {

    }
    public class ReasurorGenelRepository : Repository<ReasurorGenel>, IReasurorGenelRepository
    {
        public ReasurorGenelRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
