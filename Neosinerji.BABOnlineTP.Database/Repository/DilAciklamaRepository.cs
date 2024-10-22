using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;


namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDilAciklamaRepository : IRepository<DilAciklama>
    {

    }
    public class DilAciklamaRepository : Repository<DilAciklama>, IDilAciklamaRepository
    {
        public DilAciklamaRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
