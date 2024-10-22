using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracTipRepository : IRepository<AracTip>
    { }

    public class AracTipRepository : Repository<AracTip>, IAracTipRepository
    {
        public AracTipRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
