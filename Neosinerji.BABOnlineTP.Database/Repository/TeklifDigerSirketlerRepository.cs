using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifDigerSirketlerRepository : IRepository<TeklifDigerSirketler>
    { }
    public class TeklifDigerSirketlerRepository : Repository<TeklifDigerSirketler>, ITeklifDigerSirketlerRepository
    {
        public TeklifDigerSirketlerRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
