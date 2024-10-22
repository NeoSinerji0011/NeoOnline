using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KaskoDMRepository : IRepository<CR_KaskoDM>
    { }

    public class CR_KaskoDMRepository : Repository<CR_KaskoDM>, ICR_KaskoDMRepository
    {
        public CR_KaskoDMRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
