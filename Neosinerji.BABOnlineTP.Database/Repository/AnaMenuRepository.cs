using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAnaMenuRepository : IRepository<AnaMenu>
    { }

    public class AnaMenuRepository : Repository<AnaMenu>, IAnaMenuRepository
    {
        public AnaMenuRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
