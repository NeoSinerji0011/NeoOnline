using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMAcentelikleriRepository : IRepository<TVMAcentelikleri> { }
    public class TVMAcentelikleriRepository : Repository<TVMAcentelikleri>, ITVMAcentelikleriRepository
    {
        public TVMAcentelikleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
