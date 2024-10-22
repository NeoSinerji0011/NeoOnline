using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    /// <summary>
    /// Creates instance of specific DbContext
    /// </summary>
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
        DbContext CreateNewContext();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly DbContext _context;

        public DbContextFactory()
        {
            _context = new Neosinerji.BABOnlineTP.Database.Models.BABOnlineContext();
        }

        public DbContext GetDbContext()
        {
            return _context;
        }

        public DbContext CreateNewContext()
        {
            return new Neosinerji.BABOnlineTP.Database.Models.BABOnlineContext();
        }
    }
}
