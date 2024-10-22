using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarGenelBilgilerRepository : IRepository<HasarGenelBilgiler>
    { }
    public class HasarGenelBilgilerRepository : Repository<HasarGenelBilgiler>, IHasarGenelBilgilerRepository
    {
        public HasarGenelBilgilerRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
