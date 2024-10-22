
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifDokumanRepository : IRepository<TeklifDokuman>
    {

    }
    public class TeklifDokumanRepository : Repository<TeklifDokuman>, ITeklifDokumanRepository
    {
        public TeklifDokumanRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
