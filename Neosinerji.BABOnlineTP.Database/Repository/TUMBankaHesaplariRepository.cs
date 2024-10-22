using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMBankaHesaplariRepository : IRepository<TUMBankaHesaplari>
    { }
    public class TUMBankaHesaplariRepository : Repository<TUMBankaHesaplari>, ITUMBankaHesaplariRepository
    {
        public TUMBankaHesaplariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
