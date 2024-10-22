using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMNotlarRepository : IRepository<TVMNotlar>
    { }
    public class TVMNotlarRepository:Repository<TVMNotlar>,ITVMNotlarRepository
    {
        public TVMNotlarRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
