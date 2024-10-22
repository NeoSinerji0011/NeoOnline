using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ILilyumKartTeminatlarRepository : IRepository<LilyumKartTeminatlar>
    { }

    public class LilyumKartTeminatlarRepository : Repository<LilyumKartTeminatlar>, ILilyumKartTeminatlarRepository
    {
        public LilyumKartTeminatlarRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
