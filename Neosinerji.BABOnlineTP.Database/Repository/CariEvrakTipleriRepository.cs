using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{    
    public interface ICariEvrakTipleriRepository : IRepository<CariEvrakTipleri>
    {
    }
    public class CariEvrakTipleriRepository : Repository<CariEvrakTipleri>, ICariEvrakTipleriRepository
    {
        public CariEvrakTipleriRepository(DbContext _dbContext)
            : base(_dbContext)
        {
        }
    }
}
