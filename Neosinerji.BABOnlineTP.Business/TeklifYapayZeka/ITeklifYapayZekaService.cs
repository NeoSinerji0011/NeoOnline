using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.TeklifYapayZeka
{
    public interface ITeklifYapayZekaService
    {
        TeklifYapayZekaModel callAIService(TeklifYapayZekaData data);
        string buyukSehirMi(string il);
        Tuple<string, string> ilNufusVeYogunluk(string il);
        Tuple<string, string, string> ilceNufusVeYogunluk(string il, string ilce);
    }
}
