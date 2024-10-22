using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceOdemePlaniRepository : IRepository<PoliceOdemePlani>
    { }
    public class PoliceOdemePlaniRepository : Repository<PoliceOdemePlani>, IPoliceOdemePlaniRepository
    {
        public PoliceOdemePlaniRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
