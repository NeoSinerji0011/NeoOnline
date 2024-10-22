using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMIletisimYetkilileriRepository : IRepository<TVMIletisimYetkilileri> { }
    public class TVMIletisimYetkilileriRepository : Repository<TVMIletisimYetkilileri>, ITVMIletisimYetkilileriRepository
    {
        public TVMIletisimYetkilileriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
