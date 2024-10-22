using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;
namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceSigortaliRepository : IRepository<PoliceSigortali>
    { }
    public class PoliceSigortaliRepository : Repository<PoliceSigortali>, IPoliceSigortaliRepository
    {
        public PoliceSigortaliRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
