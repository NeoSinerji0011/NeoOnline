using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_KoyNufusRepository : IRepository<YZ_KoyNufus>
    { }
    public class YZ_KoyNufusRepository : Repository<YZ_KoyNufus>, IYZ_KoyNufusRepository
    {
        public YZ_KoyNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
