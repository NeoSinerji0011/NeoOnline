using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public interface IMAPFREProjeKasko : IMAPFREKasko
    {
        OtorizasyonResponse OtorizasyonOnay();
    }
}
