using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
        public interface IAtananIsDokumanlarRepository : IRepository<AtananIsDokumanlar>
    { }

    public class AtananIsDokumanlarRepository : Repository<AtananIsDokumanlar>, IAtananIsDokumanlarRepository
    {
        public AtananIsDokumanlarRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
