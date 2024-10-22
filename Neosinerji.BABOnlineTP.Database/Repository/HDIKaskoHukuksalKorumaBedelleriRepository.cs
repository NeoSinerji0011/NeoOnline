using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IHDIKaskoHukuksalKorumaBedelleriRepository : IRepository<HDIKaskoHukuksalKorumaBedelleri>
    { }

    public class HDIKaskoHukuksalKorumaBedelleriRepository : Repository<HDIKaskoHukuksalKorumaBedelleri>, IHDIKaskoHukuksalKorumaBedelleriRepository
    {
        public HDIKaskoHukuksalKorumaBedelleriRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}
