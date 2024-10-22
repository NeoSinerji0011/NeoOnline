using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlNufusRepository : IRepository<YZ_IlNufus>
    { }
    public class YZ_IlNufusRepository : Repository<YZ_IlNufus>, IYZ_IlNufusRepository
    {
        public YZ_IlNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
  
}
