using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IBransService
    {
        Bran GetBrans(int bransKodu);
        string GetBransUnvani(int bransKodu);

        List<Bran> GetList(string TvmTipi);
        List<Bran> GetListBroker();
        List<Bran> GetList(int tvmKodu);
        Bran CreateItem(Bran brans);
        bool UpdateItem(Bran brans);
    }
}
