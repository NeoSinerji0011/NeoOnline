using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifWebServisCevapRepository : IRepository<TeklifWebServisCevap>
    { }

    public class TeklifWebServisCevapRepository : Repository<TeklifWebServisCevap>, ITeklifWebServisCevapRepository
    {
        public TeklifWebServisCevapRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
