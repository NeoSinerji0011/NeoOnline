using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KaskoAMSRepository : IRepository<CR_KaskoAMS>
    { }

    public class CR_KaskoAMSRepository : Repository<CR_KaskoAMS>, ICR_KaskoAMSRepository
    {
        public CR_KaskoAMSRepository(DbContext dbContext)
            : base(dbContext)
        {
            
        }
    }
}
