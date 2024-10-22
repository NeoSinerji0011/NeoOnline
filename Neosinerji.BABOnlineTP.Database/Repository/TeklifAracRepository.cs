using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifAracRepository : IRepository<TeklifArac>
    { }
    public class TeklifAracRepository : Repository<TeklifArac>, ITeklifAracRepository
    {
        public TeklifAracRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
