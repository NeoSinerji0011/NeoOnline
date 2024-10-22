using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAltMenuSekmeRepository : IRepository<AltMenuSekme>
    { }

    public class AltMenuSekmeRepository : Repository<AltMenuSekme>, IAltMenuSekmeRepository
    {
        public AltMenuSekmeRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
