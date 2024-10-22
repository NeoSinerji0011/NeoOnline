using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIstigalRepository : IRepository<Istigal>
    { }

    public class IstigalRepository : Repository<Istigal>, IIstigalRepository
    {
        public IstigalRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
