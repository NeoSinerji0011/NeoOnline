using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;


namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMapfreKullaniciRepository : IRepository<MapfreKullanici>
    { }

    public class MapfreKullaniciRepository : Repository<MapfreKullanici>, IMapfreKullaniciRepository
    {
        public MapfreKullaniciRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
  
}
