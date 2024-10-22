using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class IsYeriSorular
    {
        public const int Yururlukte_dask_policesi_VarYok = 57;
        public const int Dask_Police_Vade_Tarihi = 58;
        public const int Dask_Police_Sigorta_Sirketi = 59;
        public const int Dask_Police_Numarasi = 112;


        public const int RA_Dain_i_Muhtehin_VarYok = 63;//Rehinli Alacaklı (Dain-i Mürtehin) Var / Yok 
        public const int RA_Tipi_Banka_Finansal_Kurum = 64;//Rehinli Alacaklı Tipi (Banka / Finansal Kurum)
        public const int RA_Kurum_Banka = 65;//Rehinli Alacaklı Kurum / Banka
        public const int RA_Sube = 66;//Rehinli Alacaklı Şube
        public const int RA_Kredi_Referans_No_Hesap_Sozlesme_No = 67;//Rehinli Alacaklı Kredi Referans No / Hesap Sözleşme No
        public const int RA_Kredi_Bitis_Tarihi = 68;//Rehin Alacakli Kredi Bitiş Tarihi
        public const int RA_Kredi_Tutari = 69;//Rehinli Alacaklı  Kredi Tutarı
        public const int RA_Doviz_Kodu = 70;//Rehinli Alacaklı Döviz Kodu

        public const int Daire_Brut_Yuzolcumu_M2 = 75;//Brüt Alan (m2) (Daire Brüt Yüzölçümü)
        public const int Belediye_Kodu = 113;
        public const int Muafiyet_Deprem = 114; //Muafiyet  Deprem (%)
        public const int Muafiyet_Bina = 115; //Muafiyet  Bina (%)
        public const int MuafiyetEsya = 116; //Muafiyet  Eşya (%)
        public const int Diger_Indirim_EH = 121; //Diğer İndirim(E/H)
        public const int Fiyat_Basimi_EH = 122;//Fiyat Basımı(E/H)
        public const int BosKalmaSuresi = 123;//Boş kalma süresi
        public const int YillikEnflasyonundan_Koruma_Orani = 125;//Yıllık Enflasyondan Koruma Oranı (%)
        public const int Celik_Kapı_VarMi_EH = 117; //Çelik Kapı var mı?(E/H)
        public const int DemirPArmaklik_VarMi_EH = 118; //Demir Parmaklık var mı?(E/H)
        public const int OzelGuvenlik_Alarm_VarMi_EH = 119; //Özel Guvenlik/Alarm var mı?(E/H)
        public const int DegerliEsyaYangin = 127;

        //Diğer bilgiler
        public const int Yapi_Tarzi = 71;
        public const int CatiTipi = 171;
        public const int KatTipi = 172;
        public const int PaletVarMi = 173;
        public const int DepremMusterekOraniBinaMuhteviyat = 174;
        public const int DepremMusterekOraniMuhteviyat = 175;
        public const int Kamera = 182;
        public const int TemperliCam = 183;
        public const int IstigalKonusu = 184; //(E/H)
        public const int IsYerindeCalisanKisiSayisi = 185;
        public const int Bekci = 176;
        public const int KepenkVeVeyaDemir = 180;
        public const int PasajIciUstKatlar = 181;
        public const int AsansorSayisi = 189;
        public const int EnflasyonOrani = 161;


        //Ana teminat bedelleri
        public const int DemirbasBedeli = 168;
        public const int BinaBedeli = 128;
        public const int DekorasyonBedeli = 170;
        public const int EmteaBedeli = 169;


        // ANA EK Teminatlar
        public const int DemirbasYangin = 190;
        public const int DemirbasDeprem = 191;
        public const int BinaYangin = 132;
        public const int BinaDeprem = 133;
        public const int EmteaYangin = 192;
        public const int EmteaDeprem = 193;
        public const int DekorasyonYangin = 194;
        public const int DekorasyonDeprem = 195;
        public const int MakinaTechizat = 196; //Makina Techizat
        public const int SahisMallariYangin3 = 197; //3.Şahıs Malları Yangın
        public const int KasaMuhteviyatYangin = 198; //Kasa Muhteviyat Yangın
        public const int TemellerYangin = 155;


        //Ek Teminatlar
        //SOL
        public const int EkTeminatMuhteviyat = 137;
        public const int Hirsizlik = 136;
        public const int MakinaKirilmasi = 199;
        public const int Firtina = 140;
        public const int DepremYanardagPuskurmesiMuhteviyat = 142;
        public const int SelVeSuBaskini = 143;
        public const int CamKirilmasi = 144;
        public const int DahiliSu = 147;
        public const int KaraTasitlariCarpmasi = 149;
        public const int KarAgirligi = 151;
        public const int FerdiKaza = 152;
        public const int FerdiKazaOlum = 153;
        public const int MaliSorumlulukYangin = 134;
        public const int EnkazKaldirma = 159;
        public const int KiraKaybi = 160;
        public const int IsverenMaliMesuliyetKazaBasinaBedeni = 200;
        public const int SahisMaliSorumlulukKisiBasinaBedeni3 = 201;
        public const int SahisMaliSorumlulukKazaBasinaMaddi3 = 202;
        public const int KomsulukMaliSorumlulukYanginDahiliSuDuman = 203;
        public const int KiraciMaliSorumlulukYanginDahiliSuDuman = 204;

        //SAĞ
        public const int EkTeminatBina = 138;
        public const int KasaHirsizlik = 205;
        public const int ElektronikCihaz = 206;
        public const int DepremYanardagPuskurmesi = 141;
        public const int DepremYanardagPuskurmesiBina = 124;
        public const int GLKHHKNHTeror = 146;
        public const int AsistanHizmeti = 145;
        public const int HukuksalKoruma = 148;
        public const int Duman = 163;
        public const int HavaTasitlariCarpmasi = 150;
        public const int YazDurmasi = 207;
        public const int FerdiKazaSurekliSakatlik = 154;
        public const int MaliSorumlulukEkTeminat = 135;
        public const int EnkazKaldirmaBina = 158;
        public const int YerKaymasi = 139;
        public const int IsverenMaliMesuliyetKisiBasinaBedeni = 208;
        public const int SahisMaliSorumlulukKazaBasinaBedeni3 = 209;
        public const int KomsulukMaliSorumlulukTeror = 210;
        public const int KiraciMaliSorumlulukTeror = 211;


        //public const int EkTeminatEkPrimi = 156;
        public const int IzolasOlayBsYil = 157;
        public const int Dask_Sigorta_Bedeli = 162;
        public const int Medline = 164;
        public const int Kapkac = 165;
        public const int AcilTibbiHastaneFerdiKaza = 166;
        //public const int KislikMi = 167;

        //Ekrandan sorulmayan sorular
        //public const int Fronting = 188;
        //public const int SaglikTeminati = 187;
        //public const int YanginMaliSorumlulukLimiti = 186;
        //public const int HirsizlikMuafNotu = 179;
        //public const int DahiliSuMuafKlozu = 178;
    }
}
