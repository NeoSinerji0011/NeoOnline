using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IGenelTanimlarRepository : IRepository<GenelTanimlar>
    { }

    public class GenelTanimlarRepository : Repository<GenelTanimlar>, IGenelTanimlarRepository
    {
        public GenelTanimlarRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
