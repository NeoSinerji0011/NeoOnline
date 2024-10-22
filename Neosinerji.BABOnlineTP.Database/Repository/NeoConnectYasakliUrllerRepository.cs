using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface INeoConnectYasakliUrllerRepository : IRepository<NeoConnectYasakliUrller>
    { }

    public class NeoConnectYasakliUrllerRepository : Repository<NeoConnectYasakliUrller>, INeoConnectYasakliUrllerRepository
    {
        public NeoConnectYasakliUrllerRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
