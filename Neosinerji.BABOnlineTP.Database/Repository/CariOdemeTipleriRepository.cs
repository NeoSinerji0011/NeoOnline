using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICariOdemeTipleriRepository : IRepository<CariOdemeTipleri>
    {
    }
    public class CariOdemeTipleriRepository : Repository<CariOdemeTipleri>, ICariOdemeTipleriRepository
    {
        public CariOdemeTipleriRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
