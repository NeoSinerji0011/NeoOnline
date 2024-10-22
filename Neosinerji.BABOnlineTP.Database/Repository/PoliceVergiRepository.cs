using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceVergiRepository : IRepository<PoliceVergi>
    { }
    public class PoliceVergiRepository : Repository<PoliceVergi>, IPoliceVergiRepository
    {
        public PoliceVergiRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
