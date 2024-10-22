using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMDetayRepository : IRepository<TUMDetay>
    { }
    public class TUMDetayRepository : Repository<TUMDetay>, ITUMDetayRepository
    {
        public TUMDetayRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
