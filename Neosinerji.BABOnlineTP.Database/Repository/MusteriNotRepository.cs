using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMusteriNotRepository : IRepository<MusteriNot>
    { }

    public class MusteriNotRepository : Repository<MusteriNot>, IMusteriNotRepository
    {
        public MusteriNotRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
