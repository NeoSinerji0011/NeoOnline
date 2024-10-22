using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipIsTipleriRepository : IRepository<IsTakipIsTipleri>
    { }
    public class IsTakipIsTipleriRepository : Repository<IsTakipIsTipleri>, IIsTakipIsTipleriRepository
    {
        public IsTakipIsTipleriRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
