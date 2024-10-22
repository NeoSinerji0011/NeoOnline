using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IIsTipleriRepository : IRepository<IsTipleri>
    { }

    public class IsTipleriRepository : Repository<IsTipleri>, IIsTipleriRepository
    {
        public IsTipleriRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
