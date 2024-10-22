using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_IlKoordinatRepository : IRepository<YZ_IlKoordinat>
    { }
    public class YZ_IlKoordinatRepository : Repository<YZ_IlKoordinat>, IYZ_IlKoordinatRepository
    {
        public YZ_IlKoordinatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
