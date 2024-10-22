using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_MahalleKoordinatRepository : IRepository<YZ_MahalleKoordinat>
    { }
    public class YZ_MahalleKoordinatRepository : Repository<YZ_MahalleKoordinat>, IYZ_MahalleKoordinatRepository
    {
        public YZ_MahalleKoordinatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
