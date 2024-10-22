using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracMarkaRepository : IRepository<AracMarka>
    { }

    public class AracMarkaRepository : Repository<AracMarka>, IAracMarkaRepository
    {
        public AracMarkaRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
