using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYZ_KoyKoordinatRepository : IRepository<YZ_KoyKoordinat>
    { }
    public class YZ_KoyKoordinatRepository : Repository<YZ_KoyKoordinat>, IYZ_KoyKoordinatRepository
    {
        public YZ_KoyKoordinatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
