using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAltMenuRepository : IRepository<AltMenu>
    { }

    public class AltMenuRepository : Repository<AltMenu>, IAltMenuRepository
    {
        public AltMenuRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
