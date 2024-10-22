using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMusteriTelefonRepository : IRepository<MusteriTelefon>
    { }

    public class MusteriTelefonRepository : Repository<MusteriTelefon>,IMusteriTelefonRepository
    {
        public MusteriTelefonRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
