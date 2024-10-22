using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IAXAOtomatikPoliceTransfer
    {
        List<Police> GetAXAAutoPoliceTransfer();
        List<Police> GetAXAHayatAutoPoliceTransfer();
    }
}
