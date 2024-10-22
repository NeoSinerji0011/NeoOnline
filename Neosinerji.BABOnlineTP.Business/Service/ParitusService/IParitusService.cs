using Neosinerji.BABOnlineTP.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IParitusService
    {
        ParitusAdresModel GetParitusAdres(ParitusAdresSorgulamaRequest adresModel);
    }
}
