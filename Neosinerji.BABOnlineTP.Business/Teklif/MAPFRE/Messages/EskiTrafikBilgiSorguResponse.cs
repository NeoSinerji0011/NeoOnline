using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class EskiTrafikBilgiSorguResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public TrafikAracTemelBilgileri aracTemelBilgileri { get; set; }
        public BelgeBilgileri belgeBilgileri { get; set; }
        public MapfreTarih fesihTarihi { get; set; }
        public string hasarAdet { get; set; }
        public string hasarVar { get; set; }
        public string zkytmsIndirimYuzde { get; set; }
        public PrimBilgileri primBilgileri { get; set; }
        public SigortaliBilgileri sigortali { get; set; }
        public TarihBilgileri tarihBilgileri { get; set; }
    }

    public class TrafikAracTemelBilgileri
    {
        public string aracRengi { get; set; }
        public string aracTarifeGrupKod { get; set; }
        public string imalatYeri { get; set; }
        public string kullanimSekli { get; set; }
        public TrafikMarka marka { get; set; }
        public TrafikTip tip { get; set; }
        public string modelYili { get; set; }
        public string motorGucu { get; set; }
        public string motorNo { get; set; }
        public string sasiNo { get; set; }
        public string silindirHacmi { get; set; }
        public TrafikPlakaBilgi plaka { get; set; }

        public MapfreTarih tescilTarihi { get; set; }
        public MapfreTarih trafigeCikisTarihi { get; set; }
        public string yolcuKapasitesi { get; set; }
        public string yukKapasitesi { get; set; }
    }

    public class TrafikMarka
    {
        public string kod { get; set; }
    }

    public class TrafikTip
    {
        public string aciklama { get; set; }
        public string kod { get; set; }
    }

    public class TrafikPlakaBilgi
    {
        public string ilKodu { get; set; }
        public string no { get; set; }
    }

    public class BelgeBilgileri
    {
        public string belgeNo { get; set; }
        public string belgeSiraNo { get; set; }
        public MapfreTarih belgeTarih { get; set; }
        public OncekiPoliceAnahtari oncekiPoliceAnahtari { get; set; }
        public string uygulanmasiGerekenGecikmeSurprimYuzde { get; set; }
        public string uygulanmasiGerekenIndirimYuzde { get; set; }
        public string uygulanmasiGerekenSurprimYuzde { get; set; }
        public string uygulanmasiGerekenTarifeBasamakKodu { get; set; }
        public string uygulanmisGecikmeSurprimYuzde { get; set; }
        public string uygulanmisIlPlakaIndirimYuzde { get; set; }
        public string uygulanmisIndirimYuzde { get; set; }
        public string uygulanmisSurprimYuzde { get; set; }
        public string uygulanmisTarifeBasamakKodu { get; set; }
    }

    public class OncekiPoliceAnahtari
    {
        public string acenteKod { get; set; }
        public string policeNo { get; set; }
        public string sirketKodu { get; set; }
        public string yenilemeNo { get; set; }
    }

    public class PrimBilgileri
    {
        public string altLimitPrim { get; set; }
        public string garantiFonu { get; set; }
        public string giderVergisi { get; set; }
        public string netPrim { get; set; }
        public string temelTarifePrim { get; set; }
        public string trafikHizmetGelistirmeFonu { get; set; }
        public string ustLimitPrim { get; set; }
    }

    public class SigortaliBilgileri
    {
        public string adUnvan { get; set; }
        public string soyad { get; set; }
        public string tckimlikNo { get; set; }
        public string turKodu { get; set; }
        public string uyruk { get; set; }
        public string cinsiyet { get; set; }
        public string egitimDurumKodu { get; set; }
        public AdresBilgi adres { get; set; }

    }

    public class AdresBilgi
    {
        public string acikAdres { get; set; }
        public string ilKodu { get; set; }
        public string ilceKodu { get; set; }
    }

    public class TarihBilgileri
    {
        public MapfreTarih baslangicTarihi { get; set; }
        public MapfreTarih bitisTarihi { get; set; }
        public MapfreTarih ekBaslangicTarihi { get; set; }
        public MapfreTarih tanzimTarihi { get; set; }
    }
}
