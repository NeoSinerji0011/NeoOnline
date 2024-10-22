using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common.TURKNIPPON
{
    public class TurkNippon_KaskoAracTarifeKodlari
    {
        public const int Otomobil = 1;
        public const int AraziTasiti = 3;
        public const int Kamyonet = 1;
        public const int ZirhliArac = 3;
    }

    public class TurkNippon_KaskoAracKullanimSekilleri
    {
        public const int Ozel = 1;
        public const int Resmi = 2;
        public const int Ticari = 3;

    }
    public class TurkNippon_KaskoAracYakitTipleri
    {
        public const int Benzin = 1;
        public const int Dizel = 2;
        public const int BenzinLPG = 3;

    }
    public class TurkNippon_KaskoAracRenkTipleri
    {
        public const int Beyaz = 1;
        public const int Mavi = 2;
        public const int Sari = 3;
        public const int Yesil = 4;
        public const int Kirmizi = 5;
        public const int Siyah = 6;
        public const int Lacivert = 7;
        public const int Kahverengi = 8;
        public const int Bordo = 9;
        public const int Pembe = 10;
        public const int Gri = 11;
        public const int Somon = 12;
        public const int Mor = 13;
        public const int Efaltun = 14;
        public const int Bej = 15;
        public const int Turuncu = 16;
    }
    public class TurkNippon_KaskoDainiMurteinDurumlari
    {
        public const string Yok = "Y";
        public const string Banka = "B";
        public const string FinansKurumu = "F";

    }
    public class TurkNippon_KaskoOnarimServisTipleri
    {
        //public const int AntlasmaliYS = 1;
        //public const int AntlasmasizYS = 2;
        //public const int AnlasmaliOS = 3;
        //public const int AntlasmasizOS = 4;

        public const int TNSAnlasmaliServis = 1;
        public const int TUMServisler = 2;
    }
    public class TurkNippon_KaskoYedekParcaTipleri
    {
        public const int orjinal = 1;
        public const int Esdeger = 2;
    }
    public class TurkNippon_KaskoAracBagajTipleri
    {
        public const int Yok = 1;
        public const int Acik = 2;
        public const int Kapali = 3;
    }

    public class TurkNippon_KaskoHukuksalKorumaLimitleri
    {
        public const int Yok = 0;
        public const int BesBin = 5000;
        public const int OnBin = 10000;
        public const int OnBesBin = 15000;
        public const int YirmiBin = 20000;
        public const int YirmiBesBin = 25000;
        public const int OtuzBin = 30000;
    }

    public class TurkNippon_KaskoMuafiyetLimitleri
    {
        public const int Yok = 0;
        public const int İkiYüzElli = 1;
        public const int BesYuz = 2;
        public const int YediYuzElli = 3;
        public const int On = 4;
        public const int OnBes = 6;
    }

    public class TurkNippon_KaskoKullanimTarzi
    {
        public const int Binek = 1;
        public const int Kamyonet = 6;
    }

    public class TurkNippon_KaskoKullanimSekilleri
    {
        public const int Ozel = 1;
        public const int Resmi = 2;
        public const int Ticari = 3;

    }

    public class TurkNippon_BaskiTipleri
    {
        public const int TeklifPolice = 1;
        public const int KrediKartiOdemeFormu =2;
        public const int BilgilendirmeFormu = 7;
    }
   
}
