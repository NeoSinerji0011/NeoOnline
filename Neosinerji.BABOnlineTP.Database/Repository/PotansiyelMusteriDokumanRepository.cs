using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPotansiyelMusteriDokumanRepository:IRepository<PotansiyelMusteriDokuman>
    {}

    public class PotansiyelMusteriDokumanRepository : Repository<PotansiyelMusteriDokuman>, IPotansiyelMusteriDokumanRepository
    {
         public PotansiyelMusteriDokumanRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
