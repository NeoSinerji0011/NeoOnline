using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracKullanimSekliRepository : IRepository<AracKullanimSekli>
    { }

    public class AracKullanimSekliRepository : Repository<AracKullanimSekli>, IAracKullanimSekliRepository
    {
        public AracKullanimSekliRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
