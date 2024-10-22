using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifProvizyonRepository : IRepository<TeklifProvizyon>
    {

    }

    public class TeklifProvizyonRepository : Repository<TeklifProvizyon>, ITeklifProvizyonRepository
    {
        public TeklifProvizyonRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
