using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IYKNRepository : IRepository<YKN> { }

    public class YKNRepository : Repository<YKN>, IYKNRepository
    {
        public YKNRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
