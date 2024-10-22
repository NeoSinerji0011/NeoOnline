using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIlRepository : IRepository<Il>
    { }

    public class IlRepository : Repository<Il>, IIlRepository
    {
        public IlRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
