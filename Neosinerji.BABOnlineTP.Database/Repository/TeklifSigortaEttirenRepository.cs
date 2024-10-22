using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITeklifSigortaEttirenRepository : IRepository<TeklifSigortaEttiren>
    { }
    public class TeklifSigortaEttirenRepository : Repository<TeklifSigortaEttiren>, ITeklifSigortaEttirenRepository
    {
        public TeklifSigortaEttirenRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
