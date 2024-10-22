using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAnadoluKullanimTipSekilRepository : IRepository<AnadoluKullanimTipSekil>
    {

    }
    public class AnadoluKullanimTipSekilRepository : Repository<AnadoluKullanimTipSekil>, IAnadoluKullanimTipSekilRepository
    {
        public AnadoluKullanimTipSekilRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
