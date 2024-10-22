using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KullanimTarziRepository : IRepository<CR_KullanimTarzi>
    { }

    public class CR_KullanimTarziRepository : Repository<CR_KullanimTarzi>, ICR_KullanimTarziRepository
    {
        public CR_KullanimTarziRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
