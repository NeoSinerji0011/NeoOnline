using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface INeoConnectSirketGrupKullaniciDetayRepository : IRepository<NeoConnectSirketGrupKullaniciDetay>
    { }

    public class NeoConnectSirketGrupKullaniciDetayRepository : Repository<NeoConnectSirketGrupKullaniciDetay>, INeoConnectSirketGrupKullaniciDetayRepository
    {
        public NeoConnectSirketGrupKullaniciDetayRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
