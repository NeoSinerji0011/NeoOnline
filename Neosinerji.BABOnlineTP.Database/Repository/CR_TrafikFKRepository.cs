using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_TrafikFKRepository : IRepository<CR_TrafikFK>
    { }

    public class CR_TrafikFKRepository : Repository<CR_TrafikFK>, ICR_TrafikFKRepository
    {
        public CR_TrafikFKRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
