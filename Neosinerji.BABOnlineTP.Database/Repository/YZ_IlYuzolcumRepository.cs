using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlYuzolcumRepository : IRepository<YZ_IlYuzolcum>
    { }
    public class YZ_IlYuzolcumRepository : Repository<YZ_IlYuzolcum>, IYZ_IlYuzolcumRepository
    {
        public YZ_IlYuzolcumRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
