using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKesintiTurleriRepository : IRepository<KesintiTurleri>
    { }

    public class KesintiTurleriRepository : Repository<KesintiTurleri>, IKesintiTurleriRepository
    {
        public KesintiTurleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }

    }
}
