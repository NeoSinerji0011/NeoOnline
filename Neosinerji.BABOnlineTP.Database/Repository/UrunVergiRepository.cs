using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUrunVergiRepository : IRepository<UrunVergi> { }
    public class UrunVergiRepository : Repository<UrunVergi>, IUrunVergiRepository
    {
        public UrunVergiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
