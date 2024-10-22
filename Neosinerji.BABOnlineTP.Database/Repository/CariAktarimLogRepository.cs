using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICariAktarimLogRepository : IRepository<CariAktarimLog>
    {
    }
    public class CariAktarimLogRepository : Repository<CariAktarimLog>, ICariAktarimLogRepository
    {
        public CariAktarimLogRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
