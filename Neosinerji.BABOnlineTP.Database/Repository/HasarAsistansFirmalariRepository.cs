using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{  
    public interface IHasarAsistansFirmalariRepository : IRepository<HasarAsistansFirmalari>
    { }
    public class HasarAsistansFirmalariRepository : Repository<HasarAsistansFirmalari>, IHasarAsistansFirmalariRepository
    {
        public HasarAsistansFirmalariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
