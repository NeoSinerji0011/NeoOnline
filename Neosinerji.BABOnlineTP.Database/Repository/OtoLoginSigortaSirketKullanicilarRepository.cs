using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IOtoLoginSigortaSirketKullanicilarRepository : IRepository<OtoLoginSigortaSirketKullanicilar>
    {    }
   
    public class OtoLoginSigortaSirketKullanicilarRepository : Repository<OtoLoginSigortaSirketKullanicilar>, IOtoLoginSigortaSirketKullanicilarRepository
    {
        public OtoLoginSigortaSirketKullanicilarRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
