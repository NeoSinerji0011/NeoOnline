using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceSigortaEttirenRepository : IRepository<PoliceSigortaEttiren>
    { }
    public class PoliceSigortaEttirenRepository : Repository<PoliceSigortaEttiren>, IPoliceSigortaEttirenRepository
    {
        public PoliceSigortaEttirenRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
