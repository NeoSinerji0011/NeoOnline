using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarZorunluEvraklariRepository : IRepository<HasarZorunluEvraklari>
    { }
    public class HasarZorunluEvraklariRepository : Repository<HasarZorunluEvraklari>, IHasarZorunluEvraklariRepository
    {
        public HasarZorunluEvraklariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
