using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IParaBirimleriRepository : IRepository<ParaBirimleri> { }
    public class ParaBirimleriRepository : Repository<ParaBirimleri>, IParaBirimleriRepository
    {
        public ParaBirimleriRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
 