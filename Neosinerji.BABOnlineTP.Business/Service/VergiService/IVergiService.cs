using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public  interface IVergiService
    {
        DataTableList PagedList(DataTableParameters<Vergi> vergiList);
        Vergi GetVergi(int vergiKodu);
        IQueryable<Vergi> GetList();
        Vergi CreateItem(Vergi vergi);
        bool UpdateItem(Vergi vergi);
    }
}
