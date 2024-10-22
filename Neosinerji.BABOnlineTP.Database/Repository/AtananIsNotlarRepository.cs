using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAtananIsNotlarRepository : IRepository<AtananIsNotlar>
    {
    }
    public class AtananIsNotlarRepository : Repository<AtananIsNotlar>, IAtananIsNotlarRepository
    {
        public AtananIsNotlarRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
