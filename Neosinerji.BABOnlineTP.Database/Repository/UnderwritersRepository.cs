
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUnderwritersRepository : IRepository<Underwriters>
    {

    }
    public class UnderwritersRepository : Repository<Underwriters>, IUnderwritersRepository
    {
        public UnderwritersRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
