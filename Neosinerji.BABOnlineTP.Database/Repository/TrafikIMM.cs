using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    
    public interface ITrafikIMMRepository : IRepository<TrafikIMM>
    {

    }
    public class TrafikIMMRepository : Repository<TrafikIMM>, ITrafikIMMRepository
    {
        public TrafikIMMRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
