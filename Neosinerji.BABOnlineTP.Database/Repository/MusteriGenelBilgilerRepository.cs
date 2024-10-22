using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    
    public interface IMusteriGenelBilgilerRepository : IRepository<MusteriGenelBilgiler>
    {
    }

    public class MusteriGenelBilgilerRepository : Repository<MusteriGenelBilgiler>, IMusteriGenelBilgilerRepository
    {
        public MusteriGenelBilgilerRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

    }
}

