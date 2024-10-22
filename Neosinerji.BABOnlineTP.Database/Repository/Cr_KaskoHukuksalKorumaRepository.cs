using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ICr_KaskoHukuksalKorumaRepository : IRepository<Cr_KaskoHukuksalKoruma>
    { }

    public class Cr_KaskoHukuksalKorumaRepository : Repository<Cr_KaskoHukuksalKoruma>, ICr_KaskoHukuksalKorumaRepository
    {
        public Cr_KaskoHukuksalKorumaRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
