using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKesintilerRepository : IRepository<Kesintiler>
    { }

    public class KesintilerRepository : Repository<Kesintiler>, IKesintilerRepository
    {
        public KesintilerRepository(DbContext dbContext)
            : base(dbContext)
        {

        }

    }
}


