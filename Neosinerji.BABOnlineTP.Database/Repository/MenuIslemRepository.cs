using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMenuIslemRepository : IRepository<MenuIslem>
    {
    }

    public class MenuIslemRepository : Repository<MenuIslem>, IMenuIslemRepository
    {
        public MenuIslemRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
