using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Data.Entity;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICR_KaskoIkameTuruRepository : IRepository<CR_KaskoIkameTuru>
    { }
    public class CR_KaskoIkameTuruRepository : Repository<CR_KaskoIkameTuru>, ICR_KaskoIkameTuruRepository
    {
        public CR_KaskoIkameTuruRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
