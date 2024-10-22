using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPaylasimliPoliceUretimRepository : IRepository<PaylasimliPoliceUretim>
    { }

    public class PaylasimliPoliceUretimRepository : Repository<PaylasimliPoliceUretim>, IPaylasimliPoliceUretimRepository
    {
        public PaylasimliPoliceUretimRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
