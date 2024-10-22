using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDepremMuafiyetRepository : IRepository<DepremMuafiyet>
    { }
    public class DepremMuafiyetRepository : Repository<DepremMuafiyet>, IDepremMuafiyetRepository
    {
        public DepremMuafiyetRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
