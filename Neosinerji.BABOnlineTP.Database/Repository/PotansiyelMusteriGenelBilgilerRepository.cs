using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPotansiyelMusteriGenelBilgilerRepository : IRepository<PotansiyelMusteriGenelBilgiler>
    { }

    public class PotansiyelMusteriGenelBilgilerRepository : Repository<PotansiyelMusteriGenelBilgiler>, IPotansiyelMusteriGenelBilgilerRepository
    {
        public PotansiyelMusteriGenelBilgilerRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
