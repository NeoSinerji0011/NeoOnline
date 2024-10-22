using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;


namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceUretimHedefGerceklesenRepository : IRepository<PoliceUretimHedefGerceklesen>
    {

    }
    public class PoliceUretimHedefGerceklesenRepository : Repository<PoliceUretimHedefGerceklesen>, IPoliceUretimHedefGerceklesenRepository
    {
        public PoliceUretimHedefGerceklesenRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
