using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMDurumTarihcesiRepository : IRepository<TUMDurumTarihcesi>
    { }

    public class TUMDurumTarihcesiRepository : Repository<TUMDurumTarihcesi>, ITUMDurumTarihcesiRepository
    {
        public TUMDurumTarihcesiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
