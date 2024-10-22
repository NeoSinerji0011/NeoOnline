using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifAracEkSoruRepository : IRepository<TeklifAracEkSoru>
    { }
    public class TeklifAracEkSoruRepository : Repository<TeklifAracEkSoru>, ITeklifAracEkSoruRepository
    {
        public TeklifAracEkSoruRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
