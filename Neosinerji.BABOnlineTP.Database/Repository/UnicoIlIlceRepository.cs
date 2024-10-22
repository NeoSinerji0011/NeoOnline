using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUnicoIlIlceRepository : IRepository<UnicoIlIlce>
    { }

    public class UnicoIlIlceRepository : Repository<UnicoIlIlce>, IUnicoIlIlceRepository
    {
        public UnicoIlIlceRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
