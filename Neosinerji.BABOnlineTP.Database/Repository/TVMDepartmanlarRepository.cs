using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMDepartmanlarRepository : IRepository<TVMDepartmanlar> { }
    public class TVMDepartmanlarRepository : Repository<TVMDepartmanlar>, ITVMDepartmanlarRepository
    {
        public TVMDepartmanlarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
