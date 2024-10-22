using System.Data.Entity;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracModelRepository : IRepository<AracModel>
    { }

    public class AracModelRepository : Repository<AracModel>, IAracModelRepository
    {
        public AracModelRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
