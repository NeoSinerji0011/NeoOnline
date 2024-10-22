using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{

    public interface IHasarZorunluEvrakListesiRepository : IRepository<HasarZorunluEvrakListesi>
    { }
    public class HasarZorunluEvrakListesiRepository : Repository<HasarZorunluEvrakListesi>, IHasarZorunluEvrakListesiRepository
    {
        public HasarZorunluEvrakListesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
