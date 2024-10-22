using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IBranRepository : IRepository<Bran>
    { }
    public class BranRepository : Repository<Bran>, IBranRepository
    {
        public BranRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
