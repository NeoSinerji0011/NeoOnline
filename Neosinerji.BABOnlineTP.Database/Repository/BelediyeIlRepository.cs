using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IBelediyeIlRepository : IRepository<BelediyeIl>
    { }
    public class BelediyeIlRepository : Repository<BelediyeIl>, IBelediyeIlRepository
    {
        public BelediyeIlRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
