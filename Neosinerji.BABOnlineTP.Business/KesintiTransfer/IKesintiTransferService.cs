using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IKesintiTransferService
    {
        List<Kesintiler> getKesintiler(string path, int tvmKodu, int ay, int yil);
        string getMessage();
    }
}
