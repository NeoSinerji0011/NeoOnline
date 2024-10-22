using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMeslekRepository : IRepository<Meslek>
    { }
    public class MeslekRepository : Repository<Meslek>, IMeslekRepository
    {
        public MeslekRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
