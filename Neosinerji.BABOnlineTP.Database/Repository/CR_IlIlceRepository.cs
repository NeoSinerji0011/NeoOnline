using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_IlIlceRepository : IRepository<CR_IlIlce>
    { }

    public class CR_IlIlceRepository : Repository<CR_IlIlce>, ICR_IlIlceRepository
    {
        public CR_IlIlceRepository(DbContext dbContext)
            : base(dbContext)
        {
            
        }
    }
}
