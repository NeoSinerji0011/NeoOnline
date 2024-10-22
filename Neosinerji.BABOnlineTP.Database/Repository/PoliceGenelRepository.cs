using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceGenelRepository : IRepository<PoliceGenel>
    { 
        
    }
    public class PoliceGenelRepository : Repository<PoliceGenel>, IPoliceGenelRepository
    {
        public PoliceGenelRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
