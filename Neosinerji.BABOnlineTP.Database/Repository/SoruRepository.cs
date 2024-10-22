using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ISoruRepository : IRepository<Soru>
    { }

    public class SoruRepository : Repository<Soru>, ISoruRepository
    {
        public SoruRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
