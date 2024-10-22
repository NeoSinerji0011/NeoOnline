using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ISchengenUlkeOranlariRepository : IRepository<SchengenUlkeOranlari>
    { }
    public class SchengenUlkeOranlariRepository : Repository<SchengenUlkeOranlari>, ISchengenUlkeOranlariRepository
    {
        public SchengenUlkeOranlariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
