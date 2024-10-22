using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceTransferReaderKullanicilariRepository : IRepository<PoliceTransferReaderKullanicilari>
    {
    }

    public class PoliceTransferReaderKullanicilariRepository : Repository<PoliceTransferReaderKullanicilari>, IPoliceTransferReaderKullanicilariRepository
    {
        public PoliceTransferReaderKullanicilariRepository(DbContext dbContext)
             : base(dbContext)
        {
        }
    }

}
