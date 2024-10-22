using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ILilyumKartTeminatKullanimRepository : IRepository<LilyumKartTeminatKullanim>
    { }

    public class LilyumKartTeminatKullanimRepository : Repository<LilyumKartTeminatKullanim>, ILilyumKartTeminatKullanimRepository
    {
        public LilyumKartTeminatKullanimRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
