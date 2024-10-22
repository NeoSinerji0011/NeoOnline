using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public interface IANADOLUKasko : ITeklif
    {
        AnadoluModel getAnadoluTeminatlar(string aracKodu, string kullanimSekliKodu, string kullanimTipKodu, int tvmKodu);
    }
}
