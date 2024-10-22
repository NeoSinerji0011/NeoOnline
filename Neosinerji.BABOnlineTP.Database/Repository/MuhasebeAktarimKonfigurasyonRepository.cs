using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IMuhasebeAktarimKonfigurasyonRepository : IRepository<MuhasebeAktarimKonfigurasyon>
    {
    }

    public class MuhasebeAktarimKonfigurasyonRepository : Repository<MuhasebeAktarimKonfigurasyon>, IMuhasebeAktarimKonfigurasyonRepository
    {
        public MuhasebeAktarimKonfigurasyonRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
