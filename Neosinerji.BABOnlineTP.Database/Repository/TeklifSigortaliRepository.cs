using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifSigortaliRepository : IRepository<TeklifSigortali>
    { }

    public class TeklifSigortaliRepository : Repository<TeklifSigortali>, ITeklifSigortaliRepository
    {
        public TeklifSigortaliRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
