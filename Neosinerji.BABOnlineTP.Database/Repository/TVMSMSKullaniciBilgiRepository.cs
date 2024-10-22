using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{

    public interface ITVMSMSKullaniciBilgiRepository : IRepository<TVMSMSKullaniciBilgi>
    { }
    public class TVMSMSKullaniciBilgiRepository : Repository<TVMSMSKullaniciBilgi>, ITVMSMSKullaniciBilgiRepository
    {
        public TVMSMSKullaniciBilgiRepository(DbContext dbContext)
            :base(dbContext)
        {
        }
    }

}
