using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifTeminatRepository : IRepository<TeklifTeminat>
    { }

    public class TeklifTeminatRepository : Repository<TeklifTeminat>, ITeklifTeminatRepository
    {
        public TeklifTeminatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
