using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsDurumDetayRepository : IRepository<IsDurumDetay>
    {
    }

    public class IsDurumDetayRepository : Repository<IsDurumDetay>, IIsDurumDetayRepository
    {
        public IsDurumDetayRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
