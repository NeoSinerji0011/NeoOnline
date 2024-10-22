using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IPoliceTaliAcentelerRepository : IRepository<PoliceTaliAcenteler>
    {

    }
    public class PoliceTaliAcentelerRepository : Repository<PoliceTaliAcenteler>, IPoliceTaliAcentelerRepository
    {
        public PoliceTaliAcentelerRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}