using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KaskoIMMREpository : IRepository<CR_KaskoIMM>
    {
    }
    public class CR_KaskoIMMRepository : Repository<CR_KaskoIMM>, ICR_KaskoIMMREpository
    {
        public CR_KaskoIMMRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
