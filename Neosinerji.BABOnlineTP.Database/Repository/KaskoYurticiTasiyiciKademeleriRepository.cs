using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKaskoYurticiTasiyiciKademeleriRepository : IRepository<KaskoYurticiTasiyiciKademeleri>
    { }

    public class KaskoYurticiTasiyiciKademeleriRepository : Repository<KaskoYurticiTasiyiciKademeleri>, IKaskoYurticiTasiyiciKademeleriRepository
    {
        public KaskoYurticiTasiyiciKademeleriRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
