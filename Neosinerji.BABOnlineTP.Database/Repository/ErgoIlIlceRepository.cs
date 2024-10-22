using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IErgoIlIlceRepository : IRepository<ErgoIlIlce>
    { }

    public class ErgoIlIlceRepository : Repository<ErgoIlIlce>, IErgoIlIlceRepository
    {
        public ErgoIlIlceRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
