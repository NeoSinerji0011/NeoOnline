using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliPolice
{
    public interface ITaliPoliceService
    {
        bool CreatePoliceTaliAcenteler(PoliceTaliAcenteler model);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteListe(int tvmKodu);
        PoliceTaliAcenteler GetPoliceTaliAcente(int id);
        PoliceTaliAcenteler GetPoliceTaliAcente(int tvmKodu, string policeNumarasi, string tumBirlikKodu);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteList(int tvmKodu, string policeNumarasi, string tumBirlikKodu);
        PoliceTaliAcenteRapor GetPoliceTaliAcenteRapor(int tvmkodu);
        PoliceTaliAcenteRapor GetPoliceTaliAcenteRapor(int tvmkodu, DateTime bordroKayitTarihi);
        bool CreatePoliceTaliAcenteRapor(PoliceTaliAcenteRapor model);
        bool UpdatePoliceTaliAcenteRapor(PoliceTaliAcenteRapor model);
        bool UpdatePoliceTaliAcenteler(PoliceTaliAcenteler model);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteGunlukListe(int tvmKodu);
        List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporGunlukListe(int tvmKodu);
        bool DeletePoliceTaliAcenteler(int id);
        PoliceTaliAcenteler TaliPoliceVarmi(string policeNo, int ekNo, string sigortaSirketi);
        int GetTVMKodTaliPoliceler(string tumBirlikKodu, string policeNo, int ekNo);
        int GetTVMKodTaliPoliceler(string tumBirlikKodu, string policeNo);
        List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporTarih(int tvmKodu, DateTime tarih);
        List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporStringTarih(int tvmKodu, string tarih);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteIslemTarih(int tvmKodu, DateTime basTarih);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteIslemStringTarih(int tvmKodu, string tarih);
        PoliceTaliAcenteRapor GetPoliceTaliAcenteRaporByDate(int tvmkodu, DateTime tarih);
        List<TVMDetay> GetYetkiliTVM(int tvmKodu);
        List<PoliceTaliAcenteler> GetPoliceTaliAcenteler(string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi);
        List<PoliceGenel> GetPoliceGenelEslesmeyen(int tvmKodu, DateTime baslangicTarihi, DateTime bitisTarihi);
        List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporTarihAraligi(string[] tvmList, DateTime baslangicTarihi, DateTime bitisTarihi);

        PaylasimliPoliceUretim PaylasimliPoliceUretimVarMi(int tvmkodu, int? taliTvmKodu, string policeNo, string yenilemeNo, string ekNo, string sigortaSirketi);
        bool CreatePaylasimliPoliceUretim(PaylasimliPoliceUretim model);

        List<PoliceTaliAcenteler> GetPoliceBordroList(string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi);
        List<PoliceGenel> GetPoliceGenelListesi(int merkezTVMKodu, int tvmKodu, int? disKaynakKodu, string sigortaSirketKodu, string policeNo, string tcVkn, string Plaka);
        List<PoliceGenel> GetHesaplanmisPoliceGenelListesi(int merkezTVMKodu, string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi);
    }
}
