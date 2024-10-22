using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUlkeRepository : IRepository<Ulke>
    { }

    public class UlkeRepository : Repository<Ulke>, IUlkeRepository
    {
        public UlkeRepository(DbContext dbContext)
            : base(dbContext)
        {}
    }
}
