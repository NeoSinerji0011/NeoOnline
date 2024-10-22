using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITeminatService
    {
        DataTableList PagedList(DataTableParameters<Teminat> teminatList);
        Teminat GetTeminat(int teminatKodu);
        IQueryable<Teminat> GetList();
        Teminat CreateItem(Teminat teminat);
        bool UpdateItem(Teminat teminat);
    }
}
