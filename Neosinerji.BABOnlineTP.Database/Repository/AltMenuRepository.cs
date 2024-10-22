using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAracDeger : IRepository<X_AracDegerListesi>
    { }

    public class AracDegerRepository : Repository<X_AracDegerListesi>, IAracDeger
    {
        public AracDegerRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
