﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITUMUrunleriRepository : IRepository<TUMUrunleri> { }
    public class TUMUrunleriRepository : Repository<TUMUrunleri>, ITUMUrunleriRepository
    {
        public TUMUrunleriRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}