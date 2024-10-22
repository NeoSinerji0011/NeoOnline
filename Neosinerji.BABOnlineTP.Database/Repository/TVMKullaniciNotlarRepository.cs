﻿using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface ITVMKullaniciNotlarRepository : IRepository<TVMKullaniciNotlar>
    { }
    public class TVMKullaniciNotlarRepository : Repository<TVMKullaniciNotlar>, ITVMKullaniciNotlarRepository
    {
        public TVMKullaniciNotlarRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}