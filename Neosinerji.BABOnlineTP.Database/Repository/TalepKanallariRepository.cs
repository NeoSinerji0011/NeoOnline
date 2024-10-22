using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITalepKanallariRepository : IRepository<TalepKanallari>
    { }

    public class TalepKanallariRepository : Repository<TalepKanallari>, ITalepKanallariRepository
    {
        public TalepKanallariRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
