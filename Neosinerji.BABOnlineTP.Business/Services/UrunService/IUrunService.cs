using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.Services
{
    public interface IUrunService
    {
        Urun GetUrun(int urunKodu);
        IQueryable<Urun> GetList();
        Urun CreateItem(Urun urun);
        bool UpdateItem(Urun urun);
    }
}
