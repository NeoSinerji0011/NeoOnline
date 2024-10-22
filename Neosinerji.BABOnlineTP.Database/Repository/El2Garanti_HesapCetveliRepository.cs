using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IEl2Garanti_HesapCetveliRepository : IRepository<El2Garanti_HesapCetveli>
    {
    }

    public class El2Garanti_HesapCetveliRepository : Repository<El2Garanti_HesapCetveli>, IEl2Garanti_HesapCetveliRepository
    {
        public El2Garanti_HesapCetveliRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
