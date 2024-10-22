using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IBankaSubeleriRepository : IRepository<BankaSubeleri>
    { }
    public class BankaSubeleriRepository : Repository<BankaSubeleri>, IBankaSubeleriRepository
    {
        public BankaSubeleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
