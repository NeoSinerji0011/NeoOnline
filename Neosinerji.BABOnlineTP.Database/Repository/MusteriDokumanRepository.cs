using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;


namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMusteriDokumanRepository : IRepository<MusteriDokuman>
    { }

    public class MusteriDokumanRepository : Repository<MusteriDokuman>, IMusteriDokumanRepository
    {
        public MusteriDokumanRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
