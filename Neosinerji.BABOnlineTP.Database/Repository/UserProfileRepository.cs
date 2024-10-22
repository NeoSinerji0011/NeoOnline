using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Database.Repository
{
    public interface IUserProfileRepository : IRepository<UserProfile> { }
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
