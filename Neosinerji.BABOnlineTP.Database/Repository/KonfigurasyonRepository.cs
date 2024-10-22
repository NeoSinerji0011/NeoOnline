using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKonfigurasyonRepository : IRepository<Konfigurasyon>
    { }

    public class KonfigurasyonRepository : Repository<Konfigurasyon>, IKonfigurasyonRepository
    {
        public KonfigurasyonRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
