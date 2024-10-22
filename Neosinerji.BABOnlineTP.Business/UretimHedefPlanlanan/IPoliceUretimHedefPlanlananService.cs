using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.UretimHedefPlanlanan
{
    public interface IPoliceUretimHedefPlanlananService
    {
        List<PoliceUretimHedefPlanlanan> GetPoliceUretimHedefPlanlananListe(int acenteKodu, int Yil);
        bool CreatePoliceUretimHedefPlanlanan(PoliceUretimHedefPlanlanan model);
        PoliceUretimHedefPlanlanan GetPoliceUretimHedefPlan(int id);
        bool UpdatePoliceUretimHedefPlanlanan(PoliceUretimHedefPlanlanan model);
        PoliceUretimHedefPlanlanan PoliceUretimHedefPlaniVarmi(int yil, int acenteTVMKodu);
        List<PoliceUretimHedefPlanlanan> GetPoliceUretimHedefPlaniAramaEkrani(int tvmKodu, DateTime basTarih);
    }
}
