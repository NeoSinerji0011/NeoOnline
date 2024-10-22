using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMDokumanlarRepository : IRepository<TUMDokumanlar>
    { }

    public class TUMDokumanlarRepository : Repository<TUMDokumanlar>, ITUMDokumanlarRepository
    {
        public TUMDokumanlarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
