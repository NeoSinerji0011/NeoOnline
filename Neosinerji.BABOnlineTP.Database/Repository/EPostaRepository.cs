using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IEPostaFormatlariRepository : IRepository<EPostaFormatlari>
    { }

    public class EPostaFormatlariRepository : Repository<EPostaFormatlari>, IEPostaFormatlariRepository
    {
        public EPostaFormatlariRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
