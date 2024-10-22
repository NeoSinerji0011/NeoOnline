using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMBankaHesaplariRepository : IRepository<TVMBankaHesaplari> { }
    public class TVMBankaHesaplariRepository : Repository<TVMBankaHesaplari>, ITVMBankaHesaplariRepository
    {
        public TVMBankaHesaplariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
