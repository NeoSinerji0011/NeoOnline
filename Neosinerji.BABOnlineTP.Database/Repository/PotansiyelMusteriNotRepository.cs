using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPotansiyelMusteriNotRepostory : IRepository<PotansiyelMusteriNot>
    { }

    public class PotansiyelMusteriNotRepository : Repository<PotansiyelMusteriNot>, IPotansiyelMusteriNotRepostory
    {
        public PotansiyelMusteriNotRepository(DbContext _dbContext)
            : base(_dbContext)
        {
        }
    }
}
