using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public interface IHDIDask : ITeklif
    {
        HDIIllerResponse GetUAVTIllerList();
        HDIIlcelerResponse GetUAVTIlcelerList(int ilKodu);
        HDIBeldelerResponse GetUAVTBeldelerList(int ilceKodu);
        HDIMahallelerResponse GetUAVTMahallelerList(int beldeKodu);
        HDICaddeSokakBulvarMeydanResponse GetUAVTCadSkBlvMeydanList(int mahalleKodu, string aciklama);
        HDICaddeSokakBulvarMeydanBinaAdResponse GetUAVTCadSkBlvMeydan_BinaAdList(int cadSkBulMeyKodu, string aciklama);
        HDIDairelerResponse GetUAVTDairelerList(int binaKodu);
        HDIUAVTAdresResponse GetUAVTAdres(string uavtKodu);
    }
}
