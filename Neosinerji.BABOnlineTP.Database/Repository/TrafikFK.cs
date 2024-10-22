using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITrafikFKRepository : IRepository<TrafikFK>
    {

    }
    public class TrafikFKRepository : Repository<TrafikFK>, ITrafikFKRepository
    {
        public TrafikFKRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
