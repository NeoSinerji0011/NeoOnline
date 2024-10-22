using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarNotlariRepository : IRepository<HasarNotlari>
    { }
    public class HasarNotlariRepository : Repository<HasarNotlari>, IHasarNotlariRepository
    {
        public HasarNotlariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
