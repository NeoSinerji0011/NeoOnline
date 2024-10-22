using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKaskoIMMRepository : IRepository<KaskoIMM>
    {

    }
    public class KaskoIMMRepository : Repository<KaskoIMM>, IKaskoIMMRepository
    {
        public KaskoIMMRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
