using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IVKNRepository : IRepository<VKN> { }
    public class VKNRepository : Repository<VKN>, IVKNRepository
    {
        public VKNRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
