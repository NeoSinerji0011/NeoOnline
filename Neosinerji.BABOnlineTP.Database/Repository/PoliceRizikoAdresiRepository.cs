using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceRizikoAdresiRepository : IRepository<PoliceRizikoAdresi>
    { }
    public class PoliceRizikoAdresiRepository : Repository<PoliceRizikoAdresi>, IPoliceRizikoAdresiRepository
    {
        public PoliceRizikoAdresiRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
