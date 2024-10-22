using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITCKNRepository : IRepository<TCKN>
    { }
    public class TCKNRepository : Repository<TCKN>, ITCKNRepository
    {
        public TCKNRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
