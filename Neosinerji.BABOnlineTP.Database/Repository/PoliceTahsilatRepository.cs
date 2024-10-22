using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceTahsilatRepository : IRepository<PoliceTahsilat>
    { }
    public class PoliceTahsilatRepository : Repository<PoliceTahsilat>, IPoliceTahsilatRepository
    {
        public PoliceTahsilatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
