using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifVergiRepository : IRepository<TeklifVergi>
    { }

    public class TeklifVergiRepository : Repository<TeklifVergi>, ITeklifVergiRepository
    {
        public TeklifVergiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
