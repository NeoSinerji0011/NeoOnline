using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMUrunYetkileriRepository : IRepository<TVMUrunYetkileri>
    { }
    public class TVMUrunYetkileriRepository : Repository<TVMUrunYetkileri>, ITVMUrunYetkileriRepository
    {
        public TVMUrunYetkileriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
