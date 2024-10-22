using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHasarBankaHesaplariRepository : IRepository<HasarBankaHesaplari>
    {

    }
    public class HasarBankaHesaplariRepository : Repository<HasarBankaHesaplari>, IHasarBankaHesaplariRepository
    {
        public HasarBankaHesaplariRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
