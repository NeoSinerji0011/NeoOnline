using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.AK
{
    public interface IAKKasko : ITeklif
    {
        string IpGetir(int tvmKodu);
    }
}
