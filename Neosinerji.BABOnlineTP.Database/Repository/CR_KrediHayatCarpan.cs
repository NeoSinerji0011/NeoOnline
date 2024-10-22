using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KrediHayatCarpanRepository : IRepository<CR_KrediHayatCarpan>
    { }

    public class CR_KrediHayatCarpanRepository : Repository<CR_KrediHayatCarpan>, ICR_KrediHayatCarpanRepository
    {
        public CR_KrediHayatCarpanRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
