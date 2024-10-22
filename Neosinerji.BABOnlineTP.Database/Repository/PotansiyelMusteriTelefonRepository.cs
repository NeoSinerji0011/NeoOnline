using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPotansiyelMusteriTelefonRepository : IRepository<PotansiyelMusteriTelefon>
    { }

    public class PotansiyelMusteriTelefonRepository : Repository<PotansiyelMusteriTelefon>, IPotansiyelMusteriTelefonRepository
    {
        public PotansiyelMusteriTelefonRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
