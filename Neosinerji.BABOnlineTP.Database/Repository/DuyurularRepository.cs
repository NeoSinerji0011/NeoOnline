using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDuyurularRepository : IRepository<Duyurular>
    { }
    public class DuyurularRepository : Repository<Duyurular>, IDuyurularRepository
    {
        public DuyurularRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
