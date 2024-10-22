using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMIPBaglantiRepository : IRepository<TUMIPBaglanti>
    { }
    public class TUMIPBaglantiRepository : Repository<TUMIPBaglanti>, ITUMIPBaglantiRepository
    {
        public TUMIPBaglantiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
