using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMNotlarRepository : IRepository<TUMNotlar> { }
    public class TUMNotlarRepository : Repository<TUMNotlar>, ITUMNotlarRepository
    {
        public TUMNotlarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
