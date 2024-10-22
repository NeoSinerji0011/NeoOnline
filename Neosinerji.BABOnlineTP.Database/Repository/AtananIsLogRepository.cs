using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAtananIsLogRepository : IRepository<AtananIsLog>
    {
    }
    public class AtananIsLogRepository : Repository<AtananIsLog>, IAtananIsLogRepository
    {
        public AtananIsLogRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
