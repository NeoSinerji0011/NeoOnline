using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIlceRepository : IRepository<Ilce>
    { }

    public class IlceRepository : Repository<Ilce>, IIlceRepository
    {
        public IlceRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
