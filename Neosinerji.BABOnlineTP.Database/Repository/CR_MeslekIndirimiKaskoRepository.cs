using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_MeslekIndirimiKaskoRepository : IRepository<CR_MeslekIndirimiKasko>
    { }

    public class CR_MeslekIndirimiKaskoRepository : Repository<CR_MeslekIndirimiKasko>, ICR_MeslekIndirimiKaskoRepository
    {
        public CR_MeslekIndirimiKaskoRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
