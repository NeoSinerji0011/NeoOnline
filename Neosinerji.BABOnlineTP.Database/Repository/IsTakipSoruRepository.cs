using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTakipSoruRepository : IRepository<IsTakipSoru>
    { }

    public class IsTakipSoruRepository : Repository<IsTakipSoru>, IIsTakipSoruRepository
    {
        public IsTakipSoruRepository(DbContext dbContext)
            : base(dbContext)
        {
            
        }
    }
}
