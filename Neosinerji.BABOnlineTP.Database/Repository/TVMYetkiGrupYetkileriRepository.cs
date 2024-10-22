using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMYetkiGrupYetkileriRepository : IRepository<TVMYetkiGrupYetkileri>
    { }
    public class TVMYetkiGrupYetkileriRepository : Repository<TVMYetkiGrupYetkileri>, ITVMYetkiGrupYetkileriRepository
    {
        public TVMYetkiGrupYetkileriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
