using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDokumanTurleriRepository : IRepository<DokumanTurleri>
    { }
    public class DokumanTurleriRepository : Repository<DokumanTurleri>, IDokumanTurleriRepository
    {
        public DokumanTurleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }

    }
}
