using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ISoruService
    {
        DataTableList PagedList(DataTableParameters<Soru> soruList);
        Soru GetSoru(int soruKodu);
        IQueryable<Soru> GetList();
        Soru CreateItem(Soru soru);
        bool UpdateItem(Soru soru);
    }
}
