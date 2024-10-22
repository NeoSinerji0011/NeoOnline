using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IUrunParametreleriService
    {
        UrunParametreleri GetUrunParametre(string kod);
        string GetUrunParametreValue(string kod);
        UrunParamsTable GetListUrunParametre(string[] kodlar);
    }
}
