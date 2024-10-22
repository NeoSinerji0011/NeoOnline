using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifRizikoAdresiRepository : IRepository<TeklifRizikoAdresi>
    { }
    public class TeklifRizikoAdresiRepository : Repository<TeklifRizikoAdresi>, ITeklifRizikoAdresiRepository
    {
        public TeklifRizikoAdresiRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
