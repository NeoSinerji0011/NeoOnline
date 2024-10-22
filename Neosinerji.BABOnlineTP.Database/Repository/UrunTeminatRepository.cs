using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUrunTeminatRepository : IRepository<UrunTeminat> { }
    public class UrunTeminatRepository : Repository<UrunTeminat>, IUrunTeminatRepository
    {
        public UrunTeminatRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
