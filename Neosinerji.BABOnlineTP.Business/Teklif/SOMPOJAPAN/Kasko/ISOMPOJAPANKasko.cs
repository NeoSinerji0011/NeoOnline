using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN
{
    public interface ISOMPOJAPANKasko : ITeklif
    {
        string IpGetir(int tvmKodu);
    }
}
