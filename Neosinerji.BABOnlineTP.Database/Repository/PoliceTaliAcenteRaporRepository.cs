using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository

   
   {
    public interface IPoliceTaliAcenteRaporRepository : IRepository<PoliceTaliAcenteRapor>
    {

    }
    public class PoliceTaliAcenteRaporRepository : Repository<PoliceTaliAcenteRapor>, IPoliceTaliAcenteRaporRepository
    {
        public PoliceTaliAcenteRaporRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
