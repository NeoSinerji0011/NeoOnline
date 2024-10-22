using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeminatRepository : IRepository<Teminat>
    { }
    public class TeminatRepository : Repository<Teminat>, ITeminatRepository
    {
        public TeminatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
