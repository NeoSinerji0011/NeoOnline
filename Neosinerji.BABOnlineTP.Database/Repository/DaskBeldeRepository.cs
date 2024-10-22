using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDaskBeldeRepository : IRepository<DaskBelde>
    { }
    public class DaskBeldeRepository : Repository<DaskBelde>, IDaskBeldeRepository
    {
        public DaskBeldeRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
