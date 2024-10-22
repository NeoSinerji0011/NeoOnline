using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceUretimHedefPlanlananRepository : IRepository<PoliceUretimHedefPlanlanan>
    {

    }
    public class PoliceUretimHedefPlanlananRepository : Repository<PoliceUretimHedefPlanlanan>, IPoliceUretimHedefPlanlananRepository
    {
        public PoliceUretimHedefPlanlananRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}




