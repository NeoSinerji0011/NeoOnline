using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITaliAcenteKomisyonOraniRepository : IRepository<TaliAcenteKomisyonOrani>
    { }

    public class TaliAcenteKomisyonOraniRepository : Repository<TaliAcenteKomisyonOrani>, ITaliAcenteKomisyonOraniRepository
    {
        public TaliAcenteKomisyonOraniRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
