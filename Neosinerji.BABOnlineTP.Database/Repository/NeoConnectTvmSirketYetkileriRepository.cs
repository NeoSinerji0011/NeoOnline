using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface INeoConnectTvmSirketYetkileriRepository : IRepository<NeoConnectTvmSirketYetkileri>
    { }

    public class NeoConnectTvmSirketYetkileriRepository : Repository<NeoConnectTvmSirketYetkileri>, INeoConnectTvmSirketYetkileriRepository
    {
        public NeoConnectTvmSirketYetkileriRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}



