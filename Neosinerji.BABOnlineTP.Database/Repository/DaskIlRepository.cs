using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDaskRepository : IRepository<DaskIl>
    { }
    public class DaskIlRepository : Repository<DaskIl>, IDaskRepository
    {
        public DaskIlRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
