using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDuyuruTVMRepository : IRepository<DuyuruTVM>
    { }
    public class DuyuruTVMRepository : Repository<DuyuruTVM>, IDuyuruTVMRepository
    {
        public DuyuruTVMRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
