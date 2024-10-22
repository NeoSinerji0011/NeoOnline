using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUrunParametreleriRepository : IRepository<UrunParametreleri>
    { }

    public class UrunParametreleriRepository : Repository<UrunParametreleri>, IUrunParametreleriRepository
    {
        public UrunParametreleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
