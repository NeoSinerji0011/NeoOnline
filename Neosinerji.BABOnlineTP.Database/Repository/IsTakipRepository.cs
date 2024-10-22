using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipRepository : IRepository<IsTakip>
    { }
    public class IsTakipRepository : Repository<IsTakip>, IIsTakipRepository
    {
        public IsTakipRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
