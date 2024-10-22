using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.Services
{
    public interface ISoruService
    {
        Soru GetSoru(int soruKodu);
        IQueryable<Soru> GetList();
        Soru CreateItem(Soru soru);
        bool UpdateItem(Soru soru);
    }
}
