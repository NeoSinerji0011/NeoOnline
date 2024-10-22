using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.Common.KORU
{
    public class KORUCommon
    {
        public const string FerdiKaza = "240";
        public class YardimTekUrunHesaplaSorular
        {
            public const string UrunKodu = "UrunKodu";
            public const string AcenteNo = "AcenteNo";
            public const string TaliNo = "TaliNo";
            public const string KullaniciAdi = "KullaniciAdi";
            public const string Parola = "Parola";
            public const string UrunGrubu = "UrunGrubu";
            public const string BeldeKodu = "BeldeKodu";
            public const string MusteriSoyadi = "MusteriSoyadi";
            public const string MusteriAdi = "MusteriAdi";
            public const string TanzimTarihi = "TanzimTarihi";
            public const string VadeBaslangic = "VadeBaslangic";
            public const string MusteriTcKimlikNo = "MusteriTcKimlikNo";
            public const string SigortaliAdi = "SigortaliAdi";
            public const string SigortaliSoyadi = "SigortaliSoyadi";
            public const string SigortaliTcKimlikNo = "SigortaliTcKimlikNo";
            public const string SigortaliSbmIlAdi = "SigortaliSbmIlAdi";
            public const string SigortaliSbmIlceAdi = "SigortaliSbmIlceAdi";
            public const string SigortaliSbmIlKodu = "SigortaliSbmIlKodu";
            public const string SigortaliSbmIlceKodu = "SigortaliSbmIlceKodu";
            public const string MusteriVergiNo = "MusteriVergiNo";
            public const string MusteriUyruk = "MusteriUyruk";
            public const string MusteriUavtAdresKodu = "MusteriUavtAdresKodu";
            public const string SigortaliVergiNo = "SigortaliVergiNo";
            public const string SigortaliUyruk = "SigortaliUyruk";
            public const string SigortaliBeldeKodu = "SigortaliBeldeKodu";
            public const string SigortaliUavtAdresKodu = "SigortaliUavtAdresKodu";
            public const string Komisyoner = "Komisyoner";
            public const string UretimAraci = "UretimAraci";
            public const string VadeBitis = "VadeBitis";
            public const string DainiMurtehinTip = "DainiMurtehinTip";
            public const string DainiMurtehinKurumKod = "DainiMurtehinKurumKod";
            public const string DainiMurtehinBankaSubeKod = "DainiMurtehinBankaSubeKod";
            public const string DainiMurtehinKimlikNo = "DainiMurtehinKimlikNo";
            public const string PoliceHesapLogEkle = "PoliceHesapLogEkle";
            public const string _MeslekAçıklaması = "_MeslekAçıklaması";
            public const string _Motosiklet = "_Motosiklet";
            public const string _Avcilik = "_Avcilik";
            public const string _TehlikeliSporlar = "_TehlikeliSporlar";
            public const string _SporMusabakalari = "_SporMusabakalari";
            public const string _Ucus = "_Ucus";
            public const string _DogalAfet = "_DogalAfet";
            public const string _Teror = "_Teror";
            public const string _AracVarmi = "_AracVarmi";
            public const string _AracPlaka = "_AracPlaka";
            
            public const string MeslekSinifi = "MeslekSinifi";
            public const string Menfaattar = "Menfaattar";
            public const string IstenilenTeminatMiktari = "IstenilenTeminatMiktari";
            public const string Sertifika = "Sertifika";
            public const string SporlaUgrasiyor = "SporlaUgrasiyor";
            public const string MotosikletKullaniyor = "MotosikletKullaniyor";
            public const string AcilSaglikKapsamTipi = "AcilSaglikKapsamTipi";
            public const string DogalAfet = "DogalAfet";
            public const string Teror = "Teror";
        }
        public class YardimTekUrunHesaplaDonenSoruCevaplar
        {
            public const string UrunKodu = "UrunKodu";
            public const string UrunAdi = "UrunAdi";
            public const string AcenteNo = "AcenteNo";
            public const string TaliNo = "TaliNo";
            public const string KullaniciAdi = "KullaniciAdi";
            public const string Parola = "Parola";
            public const string UrunGrubu = "UrunGrubu";
            public const string BeldeKodu = "BeldeKodu";
            public const string MusteriSoyadi = "MusteriSoyadi";
            public const string MusteriAdi = "MusteriAdi";
            public const string TanzimTarihi = "TanzimTarihi";
            public const string VadeBaslangic = "VadeBaslangic";
            public const string MusteriTcKimlikNo = "MusteriTcKimlikNo";
            public const string SigortaliAdi = "SigortaliAdi";
            public const string SigortaliSoyadi = "SigortaliSoyadi";
            public const string SigortaliTcKimlikNo = "SigortaliTcKimlikNo";
            public const string SigortaliSbmIlAdi = "SigortaliSbmIlAdi";
            public const string SigortaliSbmIlceAdi = "SigortaliSbmIlceAdi";
            public const string SigortaliSbmIlKodu = "SigortaliSbmIlKodu";
            public const string SigortaliSbmIlceKodu = "SigortaliSbmIlceKodu";
            public const string MusteriVergiNo = "MusteriVergiNo";
            public const string SigortaliVergiNo = "SigortaliVergiNo";
            public const string SigortaliBeldeKodu = "SigortaliBeldeKodu";
            public const string VadeBitis = "VadeBitis";
            public const string TeklifHash = "TeklifHash";
        }
        public class YardimTekUrunHesaplamaSonuclari
        {
            public const string EsitTaksitAdet = "EsitTaksitAdet";
            public const string NetPrim = "NetPrim";
            public const string BrutPrim = "BrutPrim";
            public const string Komisyon = "Komisyon";
            public const string OrtakKatkiPayi = "OrtakKatkiPayi";
            public const string THGF = "THGF";
            public const string GiderVergi = "GiderVergi";
            public const string GarantiFonu = "GarantiFonu";
            public const string YSV = "YSV";
            public const string KuralHatalari = "KuralHatalari";
            public const string IndirimArttirimlar = "IndirimArttirimlar";
            public const string DovizAdi = "DovizAdi";
            public const string DovizKodu = "DovizKodu";
            public const string DovizKuru = "DovizKuru";
            public const string TaksitTipiStr = "TaksitTipiStr";
            public PoliceKayitOdemePlani[] OdemePlani { get; set; }
        }

        public class PoliKayitTeklifSorular
        {
            public const string TeklifHash = "TeklifHash";
            public const string SporlaUgrasiyor = "SporlaUgrasiyor";
            public const string MotosikletKullaniyor = "MotosikletKullaniyor";
            public const string MeslekSinifi = "MeslekSinifi";
            public const string KayitTuru = "KayitTuru";

            public const string UrunKodu = "UrunKodu";
            public const string KullaniciAdi = "KullaniciAdi";
            public const string Parola = "Parola";
            public const string UrunGrubu = "UrunGrubu";
            public const string BeldeKodu = "BeldeKodu";
            public const string MusteriSoyadi = "MusteriSoyadi";
            public const string MusteriAdi = "MusteriAdi";
            public const string TanzimTarihi = "TanzimTarihi";
            public const string VadeBaslangic = "VadeBaslangic";
            public const string VadeBitis = "VadeBitis";

            public const string MusteriTcKimlikNo = "MusteriTcKimlikNo";
            public const string SigortaliAdi = "SigortaliAdi";
            public const string SigortaliSoyadi = "SigortaliSoyadi";
            public const string SigortaliTcKimlikNo = "SigortaliTcKimlikNo";
            public const string AcenteNo = "AcenteNo";
            public const string SigortaliSbmIlAdi = "SigortaliSbmIlAdi";
            public const string SigortaliSbmIlceAdi = "SigortaliSbmIlceAdi";
            public const string SigortaliSbmIlKodu = "SigortaliSbmIlKodu";
            public const string SigortaliSbmIlceKodu = "SigortaliSbmIlceKodu";
            public const string PoliceNo = "PoliceNo";
            public const string TecditNo = "TecditNo";
            public const string OrtakNo = "OrtakNo";
            public const string PoliceBasimiPDF = "PoliceBasimiPDF";
            public const string MusteriVergiNo = "MusteriVergiNo";
            public const string SigortaliVergiNo = "SigortaliVergiNo";

            public const string TaksitTipiStr = "TaksitTipiStr";
            public const string EsitTaksitAdet = "EsitTaksitAdet";
            public const string NetPrim = "NetPrim";
            public const string BrutPrim = "BrutPrim";
            public const string Komisyon = "Komisyon";
            public const string OrtakKatkiPayi = "OrtakKatkiPayi";
            public const string THGF = "THGF";
            public const string GiderVergi = "GiderVergi";
            public const string GarantiFonu = "GarantiFonu";
            public const string YSV = "YSV";
            public const string KuralHatalari = "KuralHatalari";
            public const string IndirimArttirimlar = "IndirimArttirimlar";

            public const string KrediKartiNo = "KrediKartiNo";
            public const string KrediKartiSonKullanmaAy = "KrediKartiSonKullanmaAy";
            public const string KrediKartiSonKullanmaYil = "KrediKartiSonKullanmaYil";
            public const string KrediKartiUstundekiIsim = "KrediKartiUstundekiIsim";
            public const string KrediKartiGuvenlikNo = "KrediKartiGuvenlikNo";


            public const string SigortaliCepTelefonNo = "SigortaliCepTelefonNo";
            public const string SigortaliIsTelefonNo = "SigortaliIsTelefonNo";
            public const string SigortaliEPosta = "SigortaliEPosta";
            public const string SigortaliEvTelefonNo = "SigortaliEvTelefonNo";
            
            public const string MusteriCepTelefonNo = "MusteriCepTelefonNo";
            public const string MusteriEPosta = "MusteriEPosta";

            public const string PoliceMakbuzPDF = "PoliceMakbuzPDF";


            public HesaplamaSonuclari[] HesaplamaSonuclari { get; set; }
            public Teminatlar[] Teminatlar { get; set; }
        }
        public class HesaplamaSonuclari
        {
            public string TaksitTipiStr { get; set; }
            public int EsitTaksitAdet { get; set; }
            public decimal NetPrim { get; set; }
            public decimal BrutPrim { get; set; }
            public decimal Komisyon { get; set; }
            public decimal OrtakKatkiPayi { get; set; }

            public decimal THGF { get; set; }
            public decimal GiderVergi { get; set; }
            public decimal GarantiFonu { get; set; }
            public decimal YSV { get; set; }
            public string KuralHatalari { get; set; }
            public string IndirimArttirimlar { get; set; }
        }
        public class Teminatlar
        {
            public int Kod { get; set; }
            public int SiraNo { get; set; }
            public decimal Bedel { get; set; }
            public decimal Fiyat { get; set; }
            public decimal Prim { get; set; }
            public decimal HesaplananPrim { get; set; }
            public decimal KullaniciPrimi { get; set; }
            public string Ad { get; set; }
        }
        public class PoliceKayitOdemePlani
        {
            public const string VadeTarihiStr = "VadeTarihiStr";
            public const string Tutar = "Tutar";
        }
        public class LilyumKoruFerdiKazaTeminatlar
        {
            public const string FERDIKAZAVEFAT = "FERDİ KAZA VEFAT";
            public const string FERDIKAZASUREKLISAKATLIK = "FERDİ KAZA SÜREKLİ SAKATLIK";
            public const string KORUYARDIM = "KORU YARDIM";
            public const string KOMBIKLIMAKURUTEMIZLEMEVEHALIYIKAMAHIZMETLERI = "KOMBİ, KLİMA, KURU TEMİZLEME VE HALI YIKAMA HİZMETLERİ";
            public const string SAGLIKASISTANS = "SAĞLIK ASİSTANS";

        }
        public class LilyumTeminatKodlari
        {
            public const int FERDIKAZAVEFAT = 263;
            public const int FERDIKAZASUREKLISAKATLIK = 261;
            public const int KORUYARDIM = 154;
            public const int KOMBIKLIMAKURUTEMIZLEMEVEHALIYIKAMAHIZMETLERI = 194;
            public const int SAGLIKASISTANS = 209;
        }

        public class LilyumKullaniciTanimlari
        {
            public const int LilyumInternetTvmKodu = 153001;
            public const int LilyumInternetDepartmanKodu = 0;
            public const int LilyumInternetYetkiKodu = 1186;
        }
    }
}
