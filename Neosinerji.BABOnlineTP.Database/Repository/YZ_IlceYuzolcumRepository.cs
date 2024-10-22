using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlceYuzolcumRepository : IRepository<YZ_IlceYuzolcum>
    { }
    public class YZ_IlceYuzolcumRepository : Repository<YZ_IlceYuzolcum>, IYZ_IlceYuzolcumRepository
    {
        public YZ_IlceYuzolcumRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
 
}
