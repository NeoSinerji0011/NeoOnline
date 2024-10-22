using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarAnlasmaliServislerRepository : IRepository<HasarAnlasmaliServisler>
    { }
    public class HasarAnlasmaliServislerRepository : Repository<HasarAnlasmaliServisler>, IHasarAnlasmaliServislerRepository
    {
        public HasarAnlasmaliServislerRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
