using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKaskoTasinanYukKademeleriRepository : IRepository<KaskoTasinanYukKademeleri>
    { }

    public class KaskoTasinanYukKademeleriRepository : Repository<KaskoTasinanYukKademeleri>, IKaskoTasinanYukKademeleriRepository
    {
        public KaskoTasinanYukKademeleriRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}