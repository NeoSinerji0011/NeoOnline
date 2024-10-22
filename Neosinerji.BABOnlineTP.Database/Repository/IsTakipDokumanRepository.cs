
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipDokumanRepository : IRepository<IsTakipDokuman>
    { }
    public class IsTakipDokumanRepository : Repository<IsTakipDokuman>, IIsTakipDokumanRepository
    {
        public IsTakipDokumanRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
