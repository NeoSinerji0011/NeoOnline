using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKimlikNoUretRepository : IRepository<KimlikNoUret>
    {
    }
    public class KimlikNoUretRepository : Repository<KimlikNoUret>, IKimlikNoUretRepository
    {
        public KimlikNoUretRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
