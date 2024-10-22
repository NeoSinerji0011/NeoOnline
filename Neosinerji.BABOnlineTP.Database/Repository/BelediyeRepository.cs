using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IBelediyeRepository : IRepository<Belediye>
    { }
    public class BelediyeRepository : Repository<Belediye>, IBelediyeRepository
    {
        public BelediyeRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
