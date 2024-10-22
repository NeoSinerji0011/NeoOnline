using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracTrafikTeminatRepository : IRepository<AracTrafikTeminat>
    { }

    public class AracTrafikTeminatRepository : Repository<AracTrafikTeminat>, IAracTrafikTeminatRepository
    {
        public AracTrafikTeminatRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
