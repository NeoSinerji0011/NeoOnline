using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.KORU.Messages
{
   public class KoruLilyumYardimTekUrunHesaplaRequest
    {
        public string UrunKodu { get; set; }
        public string AcenteNo { get; set; }
        public string TaliNo { get; set; }
        public string KullaniciAdi { get; set; }
        public string Parola { get; set; }
        public string MusteriTcKimlikNo { get; set; }
        public string MusteriVergiNo { get; set; }
        public string MusteriUyruk { get; set; }
        public int BeldeKodu { get; set; }
        public string MusteriUavtAdresKodu { get; set; }
        public string SigortaliTcKimlikNo { get; set; }
        public string SigortaliVergiNo { get; set; }
        public string SigortaliUyruk { get; set; }
        public int SigortaliBeldeKodu { get; set; }
        public string SigortaliUavtAdresKodu { get; set; }
        public string Komisyoner { get; set; }
        public int UretimAraci { get; set; }
        public DateTime VadeBaslangic { get; set; }
        public DateTime VadeBitis { get; set; }
        public int DainiMurtehinTip { get; set; }
        public string DainiMurtehinKurumKod { get; set; }
        public string DainiMurtehinBankaSubeKod { get; set; }
        public string DainiMurtehinKimlikNo { get; set; }
        public string PoliceHesapLogEkle { get; set; }
        public string _MeslekAçıklaması { get; set; }
        public string _Motosiklet { get; set; }
        public string _Avcilik { get; set; }
        public string _TehlikeliSporlar { get; set; }
        public string _SporMusabakalari { get; set; }
        public string _SporAciklama { get; set; }
        public string _Ucus { get; set; }
        public string _DogalAfet { get; set; }
        public string _Teror { get; set; }
        public string _AracVarmi { get; set; }
        public string _AracPlaka { get; set; }
        public string UrunGrubu { get; set; }
        public string MeslekSinifi { get; set; }
        public string Menfaattar { get; set; }
        public string IstenilenTeminatMiktari { get; set; }
        public string Sertifika { get; set; }
        public string SporlaUgrasiyor { get; set; }
        public string MotosikletKullaniyor { get; set; }
        public string AcilSaglikKapsamTipi { get; set; }
        public string DogalAfet { get; set; }
        public string Teror { get; set; }
    }
}
