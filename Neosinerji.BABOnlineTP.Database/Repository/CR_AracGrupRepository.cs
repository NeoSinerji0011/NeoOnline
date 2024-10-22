using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_AracGrupRepository : IRepository<CR_AracGrup>
    { }

    public class CR_AracGrupRepository : Repository<CR_AracGrup>, ICR_AracGrupRepository
    {
        public CR_AracGrupRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
