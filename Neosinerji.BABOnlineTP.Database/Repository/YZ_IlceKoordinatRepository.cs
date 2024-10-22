using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlceKoordinatRepository : IRepository<YZ_IlceKoordinat>
    { }
    public class YZ_IlceKoordinatRepository : Repository<YZ_IlceKoordinat>, IYZ_IlceKoordinatRepository
    {
        public YZ_IlceKoordinatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
