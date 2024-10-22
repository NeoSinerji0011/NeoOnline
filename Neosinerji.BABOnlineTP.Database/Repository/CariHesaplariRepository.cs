using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICariHesaplariRepository : IRepository<CariHesaplari>
    {
    }
    public class CariHesaplariRepository : Repository<CariHesaplari>, ICariHesaplariRepository
    {
        public CariHesaplariRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }

        public object Filter(Func<CariHesaplari, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
