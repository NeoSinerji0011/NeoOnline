using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.KesintiTransfer
{
    public interface IKesintiExcelReader
    {
        List<Kesintiler> getKesintiler();
        string getMessage();
    }
}
