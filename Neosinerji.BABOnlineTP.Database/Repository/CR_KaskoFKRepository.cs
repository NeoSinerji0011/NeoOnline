using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KaskoFKRepository : IRepository<CR_KaskoFK>
    { }
    public class CR_KaskoFKRepository : Repository<CR_KaskoFK>, ICR_KaskoFKRepository
    {
        public CR_KaskoFKRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
