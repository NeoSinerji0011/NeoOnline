using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_TrafikIMMRepository : IRepository<CR_TrafikIMM>
    { }

    public class CR_TrafikIMMRepository : Repository<CR_TrafikIMM>, ICR_TrafikIMMRepository
    {
        public CR_TrafikIMMRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
