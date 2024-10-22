using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipDetayRepository : IRepository<IsTakipDetay>
    { }
    public class IsTakipDetayRepository : Repository<IsTakipDetay>, IIsTakipDetayRepository
    {
        public IsTakipDetayRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
