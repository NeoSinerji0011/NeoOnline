﻿using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IDaskIlceRepository : IRepository<DaskIlce>
    { }
    public class DaskIlceRepository : Repository<DaskIlce>, IDaskIlceRepository
    {
        public DaskIlceRepository(DbContext _dbContext)
            : base(_dbContext)
        {

        }
    }
}