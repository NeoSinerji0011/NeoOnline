using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICariHareketleriRepository : IRepository<CariHareketleri>
    {
    }
    public class CariHareketleriRepository : Repository<CariHareketleri>, ICariHareketleriRepository
    {
        public CariHareketleriRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
