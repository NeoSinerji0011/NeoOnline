using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMIletisimYetkilileriRepository : IRepository<TUMIletisimYetkilileri>
    { }
    public class TUMIletisimYetkilileriRepository : Repository<TUMIletisimYetkilileri>, ITUMIletisimYetkilileriRepository
    {
        public TUMIletisimYetkilileriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
