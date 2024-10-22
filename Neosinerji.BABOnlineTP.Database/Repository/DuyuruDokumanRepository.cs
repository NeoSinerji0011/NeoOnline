using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDuyuruDokumanRepository : IRepository<DuyuruDokuman>
    { }
    public class DuyuruDokumanRepository : Repository<DuyuruDokuman>, IDuyuruDokumanRepository
    {
        public DuyuruDokumanRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
