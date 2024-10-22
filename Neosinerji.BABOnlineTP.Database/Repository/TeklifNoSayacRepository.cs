using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifNoSayacRepository : IRepository<TeklifNoSayac>
    { }
    public class TeklifNoSayacRepository : Repository<TeklifNoSayac>, ITeklifNoSayacRepository
    {
        public TeklifNoSayacRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
