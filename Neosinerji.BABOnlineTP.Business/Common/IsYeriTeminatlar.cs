using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class IsYeriTeminatlar
    {
        //Ana Teminatlar  12 ana teminat var
        public const int BinaYangin = 68;
        public const int BinaDeprem = 69;
        public const int DemirbasYangin = 102;
        public const int DemirbasDeprem = 103;
        public const int EmteaYangin = 104;
        public const int EmteaDeprem = 105;
        public const int DekorasyonYangin = 106;
        public const int DekorasyonDeprem = 107;
        public const int MakinaVeTechizat = 113;//MAKİNA VE TECHİZAT (YANGIN)
        public const int KasaMuhteviyatYangin = 114;//KASA MUHTEVİYATI(Yangın)
        public const int SahisMallariYangin = 115;//3. ŞAHIS MALLARI (YANGIN)
        public const int TemellerYangin = 88;


        //Ek Temşnatlar

        //SOL
        public const int EkTeminatMuhteviyat = 73;
        public const int Hirsizlik = 72;
        public const int MakinaKirilmasi = 130; //MAKİNA KIRILMASI
        public const int Firtina = 76;
        public const int DepremYanardagPuskurmesiMuhteviyat = 108;
        public const int SelVeSuBaskini = 79;
        public const int CamKirilmasi = 80;
        public const int DahiliSu = 83;
        public const int KaraTasitlariCarpmasi = 84;
        public const int KarAgirligi = 86;
        public const int FerdiKaza = 94;
        public const int FerdiKazaOlum = 95;
        public const int MaliSorumlulukYangin = 70;
        public const int EnkazKaldirma = 111;
        public const int KiraKaybi = 93;
        public const int IsVerenMaliMesuliyetKazaBasinaBedeni = 126; //İŞVEREN MALİ MESULİYET Kaza Başına Bedeni
        public const int SahisMaliSorumlulukKisiBasinaBedeni3 = 127; //3.ŞAHIS MALİ SORUMLULUK Kişi Başına Bedeni
        public const int SahisMaliSorumlulukKazaBasinaMaddi3 = 129; //3.ŞAHIS MALİ SORUMLULUK Kaza Başına Maddi
        public const int KomsulukMaliSorumlulukYanginDahiliSuDuman = 116; //KOMŞULUK M.SORUM.(YANGIN,D.SU,DUMAN) --Komşuluk Mali Sorumluluk Yangın Dahıli Su Duman
        public const int KiraciMaliSorumlulukYanginDahiliSuDuman = 118; //KİRACI M.SORUM.(YANGIN,D.SU,DUMAN)  -- Kiracı Mali Sorumluluk Yangın Dahıli Su Duman


        //SAĞ
        public const int EkTeminatBina = 74;
        public const int KasaHirsizlik = 121;//KASA HIRSIZLIK
        public const int ElektronikCihazSigortasi = 131;//LEKTRONİK CİHAZ SİGORTASI
        public const int DepremYanardagPuskurmesi = 77;
        public const int DepremYanardagPuskurmesiBina = 97;
        public const int GLKHHKNHTeror = 82;
        public const int AsistanHizmeti = 81;
        public const int HukuksalKoruma = 110;
        public const int Duman = 112;
        public const int HavaTasitlariCarpmasi = 85;
        public const int YazDurmasi = 120;   //İŞ DURMASI
        public const int FerdiKazaSurekliSakatlik = 96;
        public const int MaliMesuliyetEkTeminat = 71;
        public const int EnkazKaldirmaBina = 91;
        public const int YerKaymasi = 75;
        public const int IsVerenMaliMEsuliyetKisiBasinaBedeni = 125; //İŞVEREN MALİ MESULİYET Kişi Başına Bedeni
        public const int SahisMaliSorumlulukKazaBasinaBedeni3 = 128; //3.ŞAHIS MALİ SORUMLULUK Kaza Başına Bedeni
        public const int KomsulukMaliSorumlulukTeror = 117; //KOMŞULUK M.SORUM.(TERÖR)
        public const int KiraciMaliSorumlulukTeror = 119; //KİRACI M.SORUM.( TERÖR)


        //Not: Ek Teminat Ek Prim ekrandan alınmayacak.
        public const int EkTeminatEkPrimi = 89;
        public const int Medline = 98;


        public const int DegerliEsyaYangin = 87;
        public const int IzolasOlayBsYil = 90;

        //public const int Kapkac = 99;
        //public const int AcilTibbiHastaneFerdiKaza = 100;
        //public const int Dolu = 109;
        public const int TasinanParaBeherSeferIcinLimit = 122; //TAŞINAN PARA Beher Sefer için Limit
        public const int TasinanParaYillikToplamLimit = 123; //TAŞINAN PARA Yıllık Toplam Limit
        public const int EmniyetiSuistimalKisiBasina = 124; //EMNİYETİ SUİSTİMAL Kişi Başına/Yıllık azami
    }
}
