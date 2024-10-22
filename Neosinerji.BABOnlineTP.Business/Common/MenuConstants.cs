using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class AnaMenuler
    {
        public const int Yonetim = 1;
        public const int Musteri = 2;
        public const int Teklif = 9;
        public const int Rapor = 10; //Üretim Raporları
        public const int Police = 11; // Poliçe/Teklif/Tahsilat
        public const int VeriTransferMerkezi = 12;
        public const int PoliceTransfer = 13;
        public const int Komisyon = 14;
        public const int SigortaUzmanlari = 15;
        public const int GorevAtamaSistemi = 16;
        public const int OnMuhasebe = 17;
        public const int Lilyum = 18;
        public const int RobotTeklif = 19;
    }

    public class AltMenuler
    {
        public const int SistemYonetimi = 39;
        public const int KullaniciYonetimi = 40;
        public const int UrunYonetimi = 41;
        public const int TVMYonetimi = 42;
        public const int TUMyonetimi = 43;
        public const int YetkiYonetimi = 44;
        public const int MusteriEkle = 52;
        public const int AraGuncelle = 53;
        public const int MusteriListesi = 116;
        public const int MusteriAdedi = 117;
        public const int TopluMusteriEkle = 54;
        public const int PoliceRaporu = 51; //Poliçe listesi online
        public const int PoliceAra = 48;
        public const int KrediliHayatPoliceAktar = 45;
        public const int TopluMusteriAktar = 46;
        public const int OfflinePoliceAktar = 47;
        public const int PotansiyelMusteri = 55;
        public const int Musterilerim = 56;      
        public const int PoliceTransfer = 60;
        public const int PoliceAra_Kimlik_Plaka = 61;
        public const int OranBelirleme = 63;
        public const int KomisyonHesaplama = 64;
        public const int TaliAcenteTransferi = 65;
        public const int UretimListesi = 66;
        public const int UretimKontrol = 67;       
        public const int UretimHedef = 68;       
        public const int PoliceAraOffLine  = 72;  //Police tahsilat girişi
        public const int PoliceTahsilatRaporu = 73;  //Police tahsilat girişi
        public const int PoliceEslesmeyenListesi = 74; //eşleşmeyen kayıtlar listesi
        public const int UretimBordrosuGoruntuleme = 75; //eşleşmeyen kayıtlar listesi
        public const int PoliceOnaylama = 76;
        public const int OfflineUretimPerformansi = 77;
        public const int KullaniciKokpit = 111;
        public const int OfflineRaporlar = 78;
        public const int OnlineRaporlar = 79;
        public const int PoliceTransferDosyaGoruntule = 80;
        public const int NeoConnectYonetim = 81;
        public const int MasrafGiderGirisi = 82;
        public const int OfflinePoliceGirisi = 83;
        public const int KesintiTransfer = 84;
        public const int OtomatikPoliceOnaylama = 85;
        public const int PoliceListesiOffline = 86;
        public const int GorevDagilimRaporu = 87;
        public const int GorevAtama = 88;
        public const int Islerim = 89;
        public const int PoliceTahsilatEkstresi = 90;
        public const int CariHareketGirisi= 91;
        public const int CariHesapGirisi = 92;
        public const int CariHesapEkstresi = 93;
        public const int CariHesapAraGuncelle = 94;
        public const int CariHesapHareketKontrolListesi = 95;
        public const int PoliceleriCHAktar = 97;
        public const int CariHesapMizani = 98;
        public const int TopluPoliceOdeme = 99;
        public const int PoliceleriCariyeAktar = 100;
        public const int MuhasebeAktarimListesi = 101;
        public const int LilyumKartSatinAl = 102;
        public const int LilyumKartKullanim = 103;
        public const int LilyumKartKullanimGuncelle = 104;
        public const int LilyumKartKartListesi = 105;
        public const int ManuelTeklifPoliceGirisi = 107;
        public const int PoliceTeklifListesi = 108;
        public const int RobotTeklifAl = 119;
        public const int PoliceYaslandirmaTablosu = 120;
        public const int PoliceTeklifListesiUWDetayli = 121;
        public const int BrokerCariHareketEkle = 122;
        
    }

    public class AltMenuSekmeler
    {
        public const int EPostaFormatlari = 1;
        public const int WebServisLoglari = 2;
        public const int DuyuruYayinlama = 3;
        public const int MenuYonetimi = 4;
        public const int KullaniciTanimlama = 5;
        public const int VergiTanimlama = 6;
        public const int SoruTanimlama = 7;
        public const int TeminatTanimlama = 8;
        public const int BransTanimlama = 9;
        public const int UrunTanimlama = 10;
        public const int TeklifVermeMerkeziTVM = 11;
        public const int TeklifUretmeMerkeziTUM = 12;
        public const int YetkiAyarlari = 13;
        public const int TVMUrunYetkileri = 14;
        public const int Konfigurasyon = 15;
        public const int Ekle = 16;
        public const int AraGuncelle = 17;
        public const int HataLog = 18;
        public const int MapfreKullaniciTanimlama = 19;
        public const int TVMDenemeSurumTanimi = 20;       
        public const int PoliceUretimRaporuListe = 21; //Üretim detay
        public const int PoliceUretimIcmali = 22; //Üretim icmal
        public const int GerceklesenUretimRapor = 23;

        public const int TeklifRaporu =24;
        public const int OzetRapor = 25;
        public const int AracPoliceIstatistik = 26; //araç bazında üretim
        public const int SubeSatisRaporu = 27;
        public const int MTSatisRaporu = 28;
        public const int TVMProjeKoduGenel = 29; //satış kanalı ve kullancı adetleri

        public const int VadeTakipRaporu = 31;
        public const int PoliceTahsilatRaporu = 32;
        public const int NeoConnectSirketAra = 33;
        public const int NeoConnectSifreGuncelle = 34;
        public const int SatısKanalıIpAraEkle = 35;
        public const int KullaniciYonetimi = 36;
        public const int KullaniciAraGuncelleSil = 37;
        public const int NeoConnectLogRapor = 38;  //Log Raporlama
        public const int NeoConnectSigortaSirketGrupTanimlama = 39;  
        public const int SatısKanalıIpListesi = 40;  
    }

    public class MenuPermission
    {
        public const int Gorme = 1;
        public const int Ekleme = 2;
        public const int Guncelleme = 3;
        public const int Silme = 4;
    }

    public class MenuPermissionType
    {
        public const int AnaMenu = 1;
        public const int AltMenu = 2;
        public const int ALtMenuSekme = 3;
    }
}
