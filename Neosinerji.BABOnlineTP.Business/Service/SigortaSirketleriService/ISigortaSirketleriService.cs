using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ISigortaSirketleriService
    {
        SigortaSirketleri GetSirket(string sirketKodu);
        List<SigortaSirketleri> GetList(bool isUnderWriter=false);
        SigortaSirketleri CreateItem(SigortaSirketleri sirket);
        bool UpdateItem(SigortaSirketleri sirket);
        void DeleteSirket(string sirketKodu);
        string GetSigortaSirketleriUnvan(string kodu);
        SigortaSirketleri GetSigortaBilgileri(string SirketKodu);

        List<SigortaSirketleri> GetNeoConnectSirketListesi();
    }
}
