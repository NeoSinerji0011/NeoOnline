using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifOdemePlaniRepository : IRepository<TeklifOdemePlani>
    { }
    public class TeklifOdemePlaniRepository : Repository<TeklifOdemePlani>, ITeklifOdemePlaniRepository
    {
        public TeklifOdemePlaniRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
