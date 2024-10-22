using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMeslekIndirimiKaskoRepository : IRepository<MeslekIndirimiKasko>
    { }

    public class MeslekIndirimiKaskoRepository : Repository<MeslekIndirimiKasko>, IMeslekIndirimiKaskoRepository
    {
        public MeslekIndirimiKaskoRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }

}
