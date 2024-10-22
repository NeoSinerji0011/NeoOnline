using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public interface IPoliceTransferReader
    {
        List<Police> getPoliceler();
        string getMessage();
        bool getTahsilatMi();
    }
}
