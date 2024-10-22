using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarEksperListesiRepository : IRepository<HasarEksperListesi>
    { }
    public class HasarEksperListesiRepository : Repository<HasarEksperListesi>, IHasarEksperListesiRepository
    {
        public HasarEksperListesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
