using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_AracEkSoruRepository : IRepository<CR_AracEkSoru>
    { }

    public class CR_AracEkSoruRepository : Repository<CR_AracEkSoru>, ICR_AracEkSoruRepository
    {
        public CR_AracEkSoruRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
