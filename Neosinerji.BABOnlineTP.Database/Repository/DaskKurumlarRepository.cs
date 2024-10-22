﻿using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDaskKurumlarRepository : IRepository<DaskKurumlar>
    {

    }
    public class DaskKurumlarRepository : Repository<DaskKurumlar>, IDaskKurumlarRepository
    {
        public DaskKurumlarRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}
