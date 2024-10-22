using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_TUMMusteriRepository : IRepository<CR_TUMMusteri>
    { }

    public class CR_TUMMusteriRepository : Repository<CR_TUMMusteri>, ICR_TUMMusteriRepository
    {
        public CR_TUMMusteriRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
