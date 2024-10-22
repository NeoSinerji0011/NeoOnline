using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMDetayRepository : IRepository<TVMDetay> { }
    public class TVMDetayRepository : Repository<TVMDetay>, ITVMDetayRepository
    {
        public TVMDetayRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
