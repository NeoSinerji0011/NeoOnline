
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceDokumanRepository : IRepository<PoliceDokuman>
    {

    }
    public class PoliceDokumanRepository : Repository<PoliceDokuman>, IPoliceDokumanRepository
    {
        public PoliceDokumanRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
