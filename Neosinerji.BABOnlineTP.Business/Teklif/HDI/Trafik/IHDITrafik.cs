using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public interface IHDITrafik : ITeklif
    {
        HDIPlakaSorgulamaResponse PlakaSorgula(string plakaKodu, string plakaNo, short musteriTipKodu, string kimlikNo);
    }
}
