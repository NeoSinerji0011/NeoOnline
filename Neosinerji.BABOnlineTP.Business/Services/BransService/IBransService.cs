using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.Services
{
    public interface IBransService
    {
        Bran GetBrans(int bransKodu);
        IQueryable<Bran> GetList();
        Bran CreateItem(Bran bran);
        bool UpdateItem(Bran bran);
    }
}
