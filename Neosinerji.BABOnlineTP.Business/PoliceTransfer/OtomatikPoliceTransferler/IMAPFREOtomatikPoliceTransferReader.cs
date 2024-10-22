using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IMAPFREOtomatikPoliceTransferReader
    {
        List<Police> GetMAPFREAutoPoliceTransfer();
    }
}
