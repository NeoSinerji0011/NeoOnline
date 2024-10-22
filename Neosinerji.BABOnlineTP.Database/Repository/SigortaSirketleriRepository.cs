using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ISigortaSirketleriRepository : IRepository<SigortaSirketleri>
    { }

    public class SigortaSirketleriRepository : Repository<SigortaSirketleri>, ISigortaSirketleriRepository
    {
        public SigortaSirketleriRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
