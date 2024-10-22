using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_BelediyeNufusRepository : IRepository<YZ_BelediyeNufus>
    { }
    public class YZ_BelediyeNufusRepository : Repository<YZ_BelediyeNufus>, IYZ_BelediyeNufusRepository
    {
        public YZ_BelediyeNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
