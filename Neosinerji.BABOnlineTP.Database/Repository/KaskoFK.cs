using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKaskoFKRepository : IRepository<KaskoFK>
    {

    }
    public class KaskoFKRepository : Repository<KaskoFK>, IKaskoFKRepository
    {
        public KaskoFKRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
