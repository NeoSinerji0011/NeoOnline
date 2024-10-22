using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMWebServisKullanicilariRepository : IRepository<TVMWebServisKullanicilari>
    { }

    public class TVMWebServisKullanicilariRepository : Repository<TVMWebServisKullanicilari>, ITVMWebServisKullanicilariRepository
    {
        public TVMWebServisKullanicilariRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
