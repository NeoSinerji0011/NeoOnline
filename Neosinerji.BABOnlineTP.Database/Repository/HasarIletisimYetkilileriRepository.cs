using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarIletisimYetkilileriRepository : IRepository<HasarIletisimYetkilileri>
    {
    }
    public class HasarIletisimYetkilileriRepository : Repository<HasarIletisimYetkilileri>, IHasarIletisimYetkilileriRepository
    {
        public HasarIletisimYetkilileriRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
