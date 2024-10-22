using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class KaskoSorular
    {
        // ==== Trafik ve kasko ortak soruları ==== //
        /// <summary>
        /// Arac Kullanim sekli
        /// </summary>
        public const int Arac_Kullanim_Sekli = 1;

        /// <summary>
        /// Arac Kullanim Tarzı
        /// </summary>
        public const int Arac_Kullanim_Tarzı = 22;

        /// <summary>
        /// Arac Marka
        /// </summary>
        public const int Arac_Marka = 2;

        /// <summary>
        /// Arac Model Yili
        /// </summary>
        public const int Arac_Model_Yili = 3;

        /// <summary>
        /// Arac Tipi
        /// </summary>
        public const int Arac_Tipi = 4;

        /// <summary>
        /// Tescil İl Kodu
        /// </summary>
        public const int Tescil_Il_Kodu = 5;

        /// <summary>
        /// Tescil İlce Adi
        /// </summary>
        public const int Tescil_Ilce_Kodu = 6;

        /// <summary>
        /// Trafik Tescil Tarihi
        /// </summary>
        public const int Trafik_Tescil_Tarihi = 7;

        /// <summary>
        /// Trafiğe Çikis Tarihi
        /// </summary>
        public const int Trafige_Cikis_Tarihi = 8;

        /// <summary>
        /// Arac Motor No
        /// </summary>
        public const int Arac_Motor_No = 9;

        /// <summary>
        /// Arac Şase No
        /// </summary>
        public const int Arac_Sase_No = 10;

        /// <summary>
        /// Police Baslangic Tarihi
        /// </summary>
        public const int Police_Baslangic_Tarihi = 11;

        /// <summary>
        /// Eski Police Var/Yok
        /// </summary>
        public const int Eski_Police_VarYok = 12;

        /// <summary>
        /// Eski Sigorta Sirketi Kodu
        /// </summary>
        public const int Eski_Police_Sigorta_Sirketi = 23;

        /// <summary>
        /// Eski Police Acente No
        /// </summary>
        public const int Eski_Police_Acente_No = 13;

        /// <summary>
        /// Eski Police No
        /// </summary>
        public const int Eski_Police_No = 14;

        /// <summary>
        /// Eski Police Yenileme No
        /// </summary>
        public const int Eski_Police_Yenileme_No = 15;

        /// <summary>
        /// Tasima Yetki Belgesi Var/Yok
        /// </summary>
        public const int Tasima_Yetki_Belgesi_VarYok = 16;

        /// <summary>
        /// Tasiyici Sorumluluk Var/Yok
        /// </summary>
        public const int Tasiyici_Sorumluluk_VarYok = 17;

        /// <summary>
        /// Tasiyici Sorumluluk Sigorta sirketi
        /// </summary>
        public const int Tasiyici_Sorumluluk_Sigorta_Sirketi = 18;

        /// <summary>
        /// Tasiyici Sorumluluk Acente No
        /// </summary>
        public const int Tasiyici_Sorumluluk_Acente_No = 19;

        /// <summary>
        /// Tasiyici Sorumluluk Police No
        /// </summary>
        public const int Tasiyici_Sorumluluk_Police_No = 20;

        /// <summary>
        /// Tasiyici Sorumluluk Yenileme No
        /// </summary>
        public const int Tasiyici_Sorumluluk_Yenileme_No = 21;

        /// <summary>
        /// I.M.M. Kademe
        /// </summary>
        public const int Teminat_IMM_Kademe = 24;

        /// <summary>
        /// Ferdi Kaza Kademe
        /// </summary>
        public const int Teminat_FK_Kademe = 25;

        /// <summary>
        /// Asistans Hizmeti
        /// </summary>
        public const int Teminat_Asistans = 26;

        // ==== Kasko ürünü için eklenmiş olan sorular ==== //
        /// <summary>
        /// Hukuksal Koruma Var/Yok
        /// </summary>
        public const int Hukuksal_Koruma_VarYok = 27;

        /// <summary>
        /// Hukuksal Koruma Kademesi
        /// </summary>
        public const int Hukuksal_Koruma_Kademesi = 28;

        /// <summary>
        /// Yurt içi Taşiyicı Var/Yok
        /// </summary>
        public const int Yurt_ici_Tasiyici_VarYok = 29;

        /// <summary>
        ///Yurt içi Taşiyıcı Kademesi 
        /// </summary>
        public const int Yurt_ici_Tasiyici_Kademesi = 30;

        /// <summary>
        ///Sağlık Var/Yok
        /// </summary>
        public const int Saglik_VarYok = 31;

        /// <summary>
        /// Sağlık Kişi Sayısı
        /// </summary>
        public const int Saglik_Kisi_Sayisi = 32;

        /// <summary>
        /// Deprem Var/Yok
        /// </summary>
        public const int Deprem_VarYok = 33;

        /// <summary>
        /// Deprem Kademesi
        /// </summary>
        public const int Deprem_Kademesi = 34;

        /// <summary>
        /// Sel Su Var/Yok
        /// </summary>
        public const int Sel_Su_VarYok = 35;

        /// <summary>
        /// Sel Su Var/Yok
        /// </summary>
        public const int Sel_Su_Kademesi = 36;

        /// <summary>
        /// İkame Araç Teminatı Var/Yok
        /// </summary>
        public const int Ikame_Arac_Teminati_VarYok = 37;

        /// <summary>
        /// Personel İndirimi Var/Yok
        /// </summary>
        public const int Personel_Indirimi_VarYok = 38;

        /// <summary>
        /// GLKHHT
        /// </summary>
        public const int GLKHHT = 39;

        /// <summary>
        /// Hasarsizlik Koruma Var/Yok
        /// </summary>
        public const int Hasarsizlik_Koruma_VarYok = 40;

        /// <summary>
        /// Yurt Dışı Teminatı Var/Yok
        /// </summary>
        public const int Yurt_Disi_Teminati_VarYok = 41;

        /// <summary>
        /// Yurt Dışı Teminatı Süresi
        /// </summary>
        public const int Yurt_Disi_Teminati_Suresi = 42;

        /// <summary>
        /// Alarm Var/Yok
        /// </summary>
        public const int Alarm_VarYok = 43;

        /// <summary>
        /// Anahtar Kaybı Var/Yok
        /// </summary>
        public const int Anahtar_Kaybi_VarYok = 44;

        /// <summary>
        /// Yangın Var/Yok
        /// </summary>
        public const int Yangin_VarYok = 45;

        /// <summary>
        /// Eskime Var/Yok
        /// </summary>
        public const int Eskime_VarYok = 46;

        /// <summary>
        /// Hayvanların Vereçeği Zarar Var/Yok
        /// </summary>
        public const int Hayvanlarin_Verecegi_Zarar_ZarYok = 47;

        /// <summary>
        /// LPG Var/Yok
        /// </summary>
        public const int LPG_VarYok = 48;

        /// <summary>
        /// Kasko Türü
        /// </summary>
        public const int Kasko_Turu = 49;

        /// <summary>
        /// Servis Türü
        /// </summary>
        public const int Servis_Turu = 50;

        /// <summary>
        /// Yedek Parça Türü
        /// </summary>
        public const int Yedek_Parca_Turu = 51;

        /// <summary>
        /// Çalınma
        /// </summary>
        public const int Calinma_VarYok = 52;

        /// <summary>
        /// A.M.S. Kademe
        /// </summary>
        public const int Teminat_AMS_Kodu = 230;

        /// <summary>
        /// Ölüm ve Sakatlık Teminatı
        /// </summary>
        public const int Teminat_Olum_Sakatlik = 231;

        /// <summary>
        /// Tedavi Teminatı
        /// </summary>
        public const int Teminat_Tedavi_VarYok = 232;

        /// <summary>
        /// Tedavi Teminat Tutarı
        /// </summary>
        public const int Teminat_Tedavi_Tutar = 233;

        /// <summary>
        /// Deprem Muafiyet Kodu
        /// </summary>
        public const int Teminat_Deprem_Muafiyet_Kodu = 234;

        /// <summary>
        /// Özel Eşya Teminatı Var/Yok
        /// </summary>
        public const int Teminat_Ozel_Esya_VarYok = 235;

        /// <summary>
        /// Anahtarla Çalınma Var/Yok
        /// </summary>
        public const int Teminat_Anahtarla_Calinma_VarYok = 236;

        /// <summary>
        /// Ekstra Aksesuar
        /// </summary>
        public const int Teminat_Ekstra_Aksesuar_VarYok = 237;

        /// <summary>
        /// Elektronik Cihaz Var/Yok
        /// </summary>
        public const int Teminat_ElektronikCihaz_VarYok = 238;

        /// <summary>
        /// Taşınan Yük Var/Yok
        /// </summary>
        public const int Teminat_TasinanYuk_VarYok = 239;

        /// <summary>
        /// Daini Mürtehin Var/Yok
        /// </summary>
        public const int DainiMurtein_VarYok = 240;

        /// <summary>
        /// Daini Mürtehin Kimlik Tipi
        /// </summary>
        public const int DainiMurtein_KimlikTipi = 241;

        /// <summary>
        /// Daini Mürtehin Kimlik No
        /// </summary>
        public const int DainiMurtein_KimlikNo = 242;

        /// <summary>
        /// Daini Mürtehin Unvan
        /// </summary>
        public const int DainiMurtein_Unvan = 243;

        /// <summary>
        /// Yurtdışı teminatı seyahat ulke
        /// </summary>
        public const int Yurt_Disi_Teminati_Ulke = 245;

        /// <summary>
        /// Araç Kullanıcı Teminatı
        /// </summary>
        public const int Arac_Kullanici_Teminat = 247;

        /// <summary>
        /// Araç Kullanıcı TCKN
        /// </summary>
        public const int Arac_Kullanici_TCKN = 248;

        /// <summary>
        /// Araç Kullanıcı Adı
        /// </summary>
        public const int Arac_Kullanici_Adi = 249;

        /// <summary>
        /// İkame türü
        /// </summary>
        public const int Ikame_Turu = 251;

        /// <summary>
        /// Indirim/Surprim Tipi
        /// </summary>
        public const int IndirimSurprimTipi = 253;

        /// <summary>
        /// Indirim/Surprim Oranı
        /// </summary>
        public const int IndirimSurprimOrani = 254;

        /// <summary>
        /// Onarım Yeri
        /// </summary>
        public const int OnarimYeri = 255;

        /// <summary>
        /// Filo poliçe
        /// </summary>
        public const int FiloPolice = 275;

        /// <summary>
        /// Hasarsızlık İndirim
        /// </summary>
        public const int HasarsizlikIndirim = 276;

        /// <summary>
        /// Hasar Surprim
        /// </summary>
        public const int HasarSurprim = 277;

        /// <summary>
        /// Uygulanan Kademe
        /// </summary>
        public const int UygulananKademe = 278;

        /// <summary>
        /// Belediye / Halk Otobüsü mü?
        /// </summary>
        public const int BelediyeHalkOtobusu = 279;

        /// <summary>
        /// Daini Mürtehin Kurum Tipi
        /// </summary>
        public const int DainiMurtein_KurumTipi = 296;

        /// <summary>
        /// Daini Mürtehin Kurum Kodu
        /// </summary>
        public const int DainiMurtein_KurumKodu = 297;

        /// <summary>
        /// Daini Mürtehin Şube Kodu
        /// </summary>
        public const int DainiMurtein_SubeKodu = 298;

        /// <summary>
        /// Meslek Kodu
        /// </summary>
        public const int Meslek = 300;


        /// <summary>
        /// Acente Yeni İş Mi?
        /// </summary>
        public const int YeniIsMi = 301;

        /// <summary>
        /// Anadolu Kullanım Tipi
        /// </summary>
        public const int AnadoluKullanimTipi = 302;

        /// <summary>
        /// Anadolu Kullanım Şekli
        /// </summary>
        public const int AnadoluKullanimSekli = 303;

        /// <summary>
        /// Anadolu İkame Turu
        /// </summary>
        public const int AnadoluIkameTuru = 304;


        /// <summary>
        /// Sigorta Ettiren Numara Tipi
        /// </summary>
        public const int SENumaratipi = 305;

    
        /// </summary>
        public const int AnadoluMarkaKodu = 306;
      
        /// </summary>
        public const int NipponYetkiliInidirimi = 307;

        public const int SompoJapanKaskoTuru = 308;
        public const int SompoJapanFaaliyetKodu = 309;
        public const int ManeviDahilMi = 310;

        //Groupama Özel Sorular
        public const int KazaDestekVarmi = 311;
        public const int PrimKoruma = 312;
        public const int AsistansPlusPaketi = 313;
        public const int AcenteOzelIndirimi = 314;
        public const int TeminatLimiti = 315;
        public const int Kolonlar = 316;
        public const int PesinIndirimi = 317;
        public const int YurtdisiKasko = 318;
        public const int GroupamaMeslekKodu = 319;
        public const int GroupamaElitKaskomu = 320;
        
        public const int LPG_Arac_Orjinalmi = 321;
        public const int LPG_Markasi = 322;
        public const int LPG_Bedel = 323;
        public const int GroupamaYakinlikDerecesi = 324;
        public const int GroupamaRizikoFiyati = 325;
        public const int KolonBedel = 326;
        public const int KolonMarka = 327;
        public const int GroupamaYHIMSKodu = 328;
        public const int GroupamaYHIMSBasamakKodu = 329;
        public const int GroupamaYHIMSSerbestLimit = 330;
        public const int GroupamaAracHukuksalKoruma = 331;
        public const int GroupamaSurucuHukuksalKoruma = 332;
        public const int GroupamaManeviDahilMi = 333;
        public const int GroupamaManeviDahilBedeli = 334;
        public const int GulfIkameTuru = 345;
        public const int GulfYakitTuru = 346;
        public const int GulfHukuksalKorumaBedeli = 347;
        public const int GulfKimlikNo = 348;
        public const int GulfMeslekKodu = 349;
        public const int ErgoMeslekKodu = 350;

        public const int UnicoGenisletilmisCam = 352;
        public const int UnicoManeviTazminat = 353;
        public const int UnicokilitBedeli = 354;
        public const int EngelliAracimi = 355;
        public const int UnicoHasarsizlikKorumaKlozu = 356;
        public const int UnicoKaskoMuafiyeti = 357;
        public const int UnicoKiralikAraciMi = 358;
        public const int UnicoDepremSelMuafiyeti = 359;
        public const int UnicoSurucuKursuAraciMi = 360;
        public const int UnicoSurucuSayisi = 361;
        public const int UnicoTekSurucuMu = 362;
        public const int UnicoTEBUyesiMi = 363;
        public const int UnicoYeniDegerklozu = 364;
        public const int UnicoIkameSecenegi = 365;
        public const int UnicoAksesuarTuru = 366;
        public const int AxaHayatTeminatLimiti = 367;
        public const int ElektrikliArac = 368;
        public const int ElektrikliAracBedeli = 369;
        public const int ElektrikliAracPilId = 370;
        public const int AxaAracaBagliKaravanVarMi = 371;
        public const int AxaAracaBagliKaravanBedeli = 372;
        public const int KullanimGelirKaybiVarMi = 373;
        public const int KullanimGelirKaybiBedel = 374;
        public const int AxaAsistansHizmeti = 375;
        public const int AxaIkameSecimi = 376;
        //Sadece 0-1 yaş için bu soru gelmektedir
        public const int AxaOnarimSecimi = 377;
        public const int AxaPlakaYeniKayitMi = 378;
        public const int AxaYeniDegerKlozu = 379;
        public const int AxaDepremSelKoasuransi = 380;
        public const int AxaMuafiyetTutari = 381;
        public const int AxaCamFilmiLogo = 382;
        public const int AxaSorumlulukLimiti = 383;
        //---------------

        public const int ErgoServisSecenegi = 384;
        public const int KiymetKazanma = 385;
        public const int SigaraYanigi = 386;
        public const int YetkiliOlmayanCekilme = 387;
        public const int Seylap = 388;
        public const int AnahtarliCalinma = 389;
        public const int CamMuafiyetiKaldirilsinMi = 390;
        public const int SompoArtiTeminatPlani = 394;
        public const int SompoArtiTeminatPlanDegeri = 395;

        public const int HDIRayicBedelKoruma = 398;
        public const int HataliAkaryakitAlimi = 399;
        public const int HDIPatlayiciParlayici = 400;
        public const int UNICOMeslekKodu = 401;
        public const int TurkNipponServisTuru = 403;
        public const int TurkNipponMuafiyetTutari = 404;
        public const int TurkNipponBitisTarihi = 405;

    }
}

