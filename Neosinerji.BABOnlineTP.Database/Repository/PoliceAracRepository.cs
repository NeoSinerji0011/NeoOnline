using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceAracRepository : IRepository<PoliceArac>
    { }
    public class PoliceAracRepository : Repository<PoliceArac>, IPoliceAracRepository
    {
        public PoliceAracRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
