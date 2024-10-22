using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMYetkiGruplariRepository : IRepository<TVMYetkiGruplari>
    { }
    public class TVMYetkiGruplariRepository : Repository<TVMYetkiGruplari>, ITVMYetkiGruplariRepository
    {
        public TVMYetkiGruplariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
