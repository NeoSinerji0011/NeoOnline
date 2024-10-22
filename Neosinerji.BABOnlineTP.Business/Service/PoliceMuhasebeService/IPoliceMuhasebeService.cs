using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.PoliceMuhasebe;

namespace Neosinerji.BABOnlineTP.Business.Service.PoliceMuhasebeService
{
    public interface IPoliceMuhasebeService
    {
        PoliceGenel getPoliceByIdHash(Int32 id, String hash);
        PoliceGenel getPoliceByKey(String SigortaSirketKodu, String PoliceNo, int YenilemeNo, int EkNo);
        List<PoliceGenel> getPoliceMuhasebeList();
        List<PoliceGenel> getPoliceMuhasebeList(Int32 acenteKodu, DateTime baslangicTarihi, DateTime bitisTarihi);
        PoliceEntity createEntityFromModel(PoliceGenel PoliceGenel, TVMDetay tvmDetay);
        List<PoliceEntity> createEntityListFromModelList(List<PoliceGenel> PoliceGenelList, TVMDetay tvmDetay);
        PoliceKeyEntity createKeyEntityFromModel(PoliceGenel PoliceGenel);
        List<PoliceKeyEntity> createKeyEntityListFromModelList(List<PoliceGenel> PoliceGenelList);
    }
}
