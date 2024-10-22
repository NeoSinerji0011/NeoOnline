using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{

    public interface IYZ_BuyuksehirNufusRepository : IRepository<YZ_BuyuksehirNufus>
    { }
    public class YZ_BuyuksehirNufusRepository : Repository<YZ_BuyuksehirNufus>, IYZ_BuyuksehirNufusRepository
    {
        public YZ_BuyuksehirNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
