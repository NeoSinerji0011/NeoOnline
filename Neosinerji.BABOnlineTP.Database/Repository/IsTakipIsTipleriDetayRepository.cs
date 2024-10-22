using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipIsTipleriDetayRepository : IRepository<IsTakipIsTipleriDetay>
    { }
    public class IsTakipIsTipleriDetayRepository : Repository<IsTakipIsTipleriDetay>, IIsTakipIsTipleriDetayRepository
    {
        public IsTakipIsTipleriDetayRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}