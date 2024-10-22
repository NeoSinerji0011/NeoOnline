using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{   
    public interface IAtananIslerRepository : IRepository<AtananIsler>
    { }

    public class AtananIslerRepository : Repository<AtananIsler>, IAtananIslerRepository
    {
        public AtananIslerRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
