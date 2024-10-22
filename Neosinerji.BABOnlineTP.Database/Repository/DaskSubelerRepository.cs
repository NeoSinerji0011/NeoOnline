using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDaskSubelerRepository : IRepository<DaskSubeler>
    { }
    public class DaskSubelerRepository : Repository<DaskSubeler>, IDaskSubelerRepository
    {
        public DaskSubelerRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
