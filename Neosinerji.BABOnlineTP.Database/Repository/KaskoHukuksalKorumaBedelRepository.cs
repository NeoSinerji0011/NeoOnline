using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IKaskoHukuksalKorumaBedelRepository : IRepository<KaskoHukuksalKorumaBedel>
    { }

    public class KaskoHukuksalKorumaBedelRepository : Repository<KaskoHukuksalKorumaBedel>, IKaskoHukuksalKorumaBedelRepository
    {
        public KaskoHukuksalKorumaBedelRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
