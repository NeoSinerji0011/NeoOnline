using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.GULF
{
    public interface IGULFKasko : ITeklif
    {
        GULFAracBilgileri GetGULFAracBilgiSorgu(string kimlikNo, string PlakaKodu, string PlakaNo);
        GULFMusteriBigileri GetGULFMusteriNo(string kimlikNo);
    }

    public class GULFAracBilgileri
    {
        public string AracTarifeGrupKodu { get; set; }
        public string KullanimSekli { get; set; }
        public string ModelYili { get; set; }
        public string MotorGucu { get; set; }
        public string MotorNo { get; set; }
        public string SasiNo { get; set; }
        public string PoliceBaslangicTarihi { get; set; }
        public string PoliceBitisTarihi { get; set; }
        public string TanzimTarihi { get; set; }
        public string UygulananKademe { get; set; }
        public string TescilTarihi { get; set; }
        public string KoltukSayisi { get; set; }
        public string MarkaKodu { get; set; }
        public string AracTipKodu { get; set; }
        public string SBMTramerNo { get; set; }
        public string Renk { get; set; }
        public string SilindirHacmi { get; set; }
        public string YakitTipi { get; set; }
        public PoliceBilgi policeBilgi { get; set; }
        public string HataMesaji { get; set; }
    }

    public class PoliceBilgi
    {
        public string AcenteKod { get; set; }
        public string PoliceNo { get; set; }
        public string SirketKodu { get; set; }
        public string YenilemeNo { get; set; }
    }
    public class GULFMusteriBigileri
    {
        public string MusteriNo { get; set; }
        public string Adi { get; set; }
        public string FirmaAdi { get; set; }
        public string Soyadi { get; set; }
        public string AnneAdi { get; set; }
        public string BabaAdi { get; set; }
        public string DogumTarihi { get; set; }
        public string DogumYeri { get; set; }
        public string Cinsiyeti { get; set; }
        public string IlKodu { get; set; }
        public string IlceKodu { get; set; }
        public string HataMesaji { get; set; }
    }

}
