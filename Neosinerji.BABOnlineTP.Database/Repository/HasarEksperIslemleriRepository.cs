using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{   
    public interface IHasarEksperIslemleriRepository : IRepository<HasarEksperIslemleri>
    { }
    public class HasarEksperIslemleriRepository : Repository<HasarEksperIslemleri>, IHasarEksperIslemleriRepository
    {
        public HasarEksperIslemleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
