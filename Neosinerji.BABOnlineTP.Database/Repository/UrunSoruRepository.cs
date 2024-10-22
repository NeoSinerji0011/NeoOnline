using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUrunSoruRepository : IRepository<UrunSoru>
    { }
    public class UrunSoruRepository : Repository<UrunSoru>, IUrunSoruRepository
    {
        public UrunSoruRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
