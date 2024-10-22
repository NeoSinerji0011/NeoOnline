using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_TescilIlIlceRepository : IRepository<CR_TescilIlIlce>
    { }

    public class CR_TescilIlIlceRepository : Repository<CR_TescilIlIlce>, ICR_TescilIlIlceRepository
    {
        public CR_TescilIlIlceRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
