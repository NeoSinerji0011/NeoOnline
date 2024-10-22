using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifSoruRepository : IRepository<TeklifSoru>
    { }

    public class TeklifSoruRepository : Repository<TeklifSoru>, ITeklifSoruRepository
    {
        public TeklifSoruRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
