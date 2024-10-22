using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracKullanimTarziRepository : IRepository<AracKullanimTarzi>
    { }

    public class AracKullanimTarziRepository : Repository<AracKullanimTarzi>, IAracKullanimTarziRepository
    {
        public AracKullanimTarziRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
