using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlceNufusRepository : IRepository<YZ_IlceNufus>
    { }
    public class YZ_IlceNufusRepository : Repository<YZ_IlceNufus>, IYZ_IlceNufusRepository
    {
        public YZ_IlceNufusRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
