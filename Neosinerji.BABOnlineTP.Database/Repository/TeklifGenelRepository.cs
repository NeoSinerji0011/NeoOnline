using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifGenelRepository : IRepository<TeklifGenel>
    { }
    public class TeklifGenelRepository : Repository<TeklifGenel>, ITeklifGenelRepository
    {
        public TeklifGenelRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
