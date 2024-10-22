using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IAutoPoliceTransferRepository : IRepository<AutoPoliceTransfer>
    { }

    public class AutoPoliceTransferRepository : Repository<AutoPoliceTransfer>, IAutoPoliceTransferRepository
    {
        public AutoPoliceTransferRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
