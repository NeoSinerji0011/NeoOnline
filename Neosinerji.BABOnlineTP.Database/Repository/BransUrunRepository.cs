using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    
    public interface IBransUrunRepository : IRepository<BransUrun>
    {

    }
    public class BransUrunRepository : Repository<BransUrun>, IBransUrunRepository
    {
        public BransUrunRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
