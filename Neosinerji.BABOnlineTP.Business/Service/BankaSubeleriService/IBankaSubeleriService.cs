using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IBankaSubeleriService
    {
        List<Bankalar> GetListBanka();
        List<BankaSubeleri> GetListBankaSubeleri(string BankaKodu);
    }
}
