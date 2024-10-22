using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_MahalleNufusRepository : IRepository<YZ_MahalleNufus>
    { }
    public class YZ_MahalleNufusRepository : Repository<YZ_MahalleNufus>, IYZ_MahalleNufusRepository
    {
        public YZ_MahalleNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
