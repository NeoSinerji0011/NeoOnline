using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUlkeKodlariRepository : IRepository<UlkeKodlari>
    {

    }
    public class UlkeKodlariRepository : Repository<UlkeKodlari>, IUlkeKodlariRepository
    {
        public UlkeKodlariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
