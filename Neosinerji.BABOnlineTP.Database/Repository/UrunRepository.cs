using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUrunRepository : IRepository<Urun>
    { }
    public class UrunRepository : Repository<Urun>, IUrunRepository
    {
        public UrunRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
