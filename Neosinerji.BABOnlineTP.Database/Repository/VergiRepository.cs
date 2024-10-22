using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IVergiRepository : IRepository<Vergi> { }
    public class VergiRepository : Repository<Vergi>, IVergiRepository
    {
        public VergiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
