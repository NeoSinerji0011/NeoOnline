using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICariHesapBorcAlacakRepository : IRepository<CariHesapBorcAlacak>
    {
    }
    public class CariHesapBorcAlacakRepository : Repository<CariHesapBorcAlacak>, ICariHesapBorcAlacakRepository
    {
        public CariHesapBorcAlacakRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
