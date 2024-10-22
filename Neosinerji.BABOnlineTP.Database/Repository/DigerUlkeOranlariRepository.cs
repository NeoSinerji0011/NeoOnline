using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDigerUlkeOranlariRepository : IRepository<DigerUlkeOranlari>
    { }
    public class DigerUlkeOranlariRepository : Repository<DigerUlkeOranlari>, IDigerUlkeOranlariRepository
    {
        public DigerUlkeOranlariRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
