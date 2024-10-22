using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifEMailLogRepository : IRepository<TeklifEMailLog>
    { }

    public class TeklifEMailLogRepository : Repository<TeklifEMailLog>, ITeklifEMailLogRepository
    {
        public TeklifEMailLogRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
