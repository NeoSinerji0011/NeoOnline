using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsDurumRepository : IRepository<IsDurum>
    {
    }

    public class IsDurumRepository : Repository<IsDurum>, IIsDurumRepository
    {
        public IsDurumRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
