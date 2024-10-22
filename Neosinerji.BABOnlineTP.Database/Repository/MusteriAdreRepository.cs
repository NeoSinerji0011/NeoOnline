using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMusteriAdreRepository : IRepository<MusteriAdre>
    {
    }

    public class MusteriAdreRepository : Repository<MusteriAdre>, IMusteriAdreRepository
    {
        public MusteriAdreRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
