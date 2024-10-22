using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifNotRepository : IRepository<TeklifNot>
    { }
    public class TeklifNotRepository : Repository<TeklifNot>, ITeklifNotRepository
    {
        public TeklifNotRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
