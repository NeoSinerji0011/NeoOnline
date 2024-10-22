using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_UlkeRepository : IRepository<CR_Ulke>
    { }

    public class CR_UlkeRepository : Repository<CR_Ulke>, ICR_UlkeRepository
    {
        public CR_UlkeRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
