using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.HDI.DASK
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIDASKEskiPoliceResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public PoliceBilgileri PoliceBilgileri { get; set; }
    }

        public class PoliceBilgileri
        {
            public string TanzimTarihi { get; set; }
            public string PoliceBasTarihi { get; set; }
            public string PoliceBitTarihi { get; set; }
            public string AdresDurum { get; set; }
            public EskiPoliceBilgileri EskiPoliceBilgileri { get; set; }
            public SigortaEttiren SigortaEttiren { get; set; }
        public List<Sigortali> Sigortalilar { get; set; }
            public RizikoBilgileri RizikoBilgileri { get; set; }
            public IletisimBilgileri IletisimBilgileri { get; set; }
            public BinaBilgileri BinaBilgileri { get; set; }
            public RehinAlacakBilgileri RehinAlacakBilgileri { get; set; }
        public int SigortaliSayisi { get; set; }
    }


    public class EskiPoliceBilgileri
    {
        public string EskiPoliceSirketKod { get; set; }
        public string EskiPoliceSirket { get; set; }
        public string EskiPoliceAcenteKod { get; set; }
        public string EskiPoliceAcente { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
    }
    public class SigortaEttiren
    {
        public string MusteriNumarasi { get; set; }
        public string Tipi { get; set; }
        public string Uyruk { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public string TCKimlikNo { get; set; }
        public string YabanciKimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string EvTelefonu { get; set; }
        public string CepTelefonu { get; set; }
        public string EPosta { get; set; }
        public string SigortaEttirenSifati { get; set; }
        public string SigortaEttirenSifatiAciklama { get; set; }

    }
    public class Sigortali
    {
        public string MusteriNumarasi { get; set; }
        public string Tipi { get; set; }
        public string Uyruk { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public string TCKimlikNo { get; set; }
        public string YabanciKimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string CepTelefonu { get; set; }
        public string EPosta { get; set; }

    }
    public class RizikoBilgileri
    {
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string SiteApartmanAd { get; set; }
        public string BinaNo { get; set; }
        public string Kat { get; set; }
        public string Daire { get; set; }
        public string PostaKod { get; set; }
        public string IlKod { get; set; }
        public string Ilce { get; set; }
        public string Belde { get; set; }
        public string IlAciklama { get; set; }
        public string IlceAciklama { get; set; }
        public string BeldeAciklama { get; set; }

        public List<SelectListItem> Ilceler { get; set; }
        public List<SelectListItem> Beldeler { get; set; }
    }
    public class IletisimBilgileri
    {
        public string Adres { get; set; }
        public string IlKod { get; set; }
        public string Ilce { get; set; }
        public string Belde { get; set; }
        public string IlAciklama { get; set; }
        public string IlceAciklama { get; set; }
        public string BeldeAciklama { get; set; }

    }
    public class BinaBilgileri
    {
        public string DaireYuzOlcumu { get; set; }
        public string BinaYapiTarzi { get; set; }
        public string BinaYapiTarziAciklama { get; set; }
        public string BinaInsaatYili { get; set; }
        public string BinaInsaatYiliAciklama { get; set; }
        public string ToplamKatSayisi { get; set; }
        public string ToplamKatSayisiAciklama { get; set; }
        public string DaireKullanimSekli { get; set; }
        public string DaireKullanimSekliAciklama { get; set; }
        public string BinaHasar { get; set; }
        public string BinaHasarAciklama { get; set; }
        public string EvrakTarihSayi { get; set; }
        public string Ada { get; set; }
        public string Pafta { get; set; }
        public string Parsel { get; set; }
        public string Sayfa { get; set; }

    }
    public class RehinAlacakBilgileri
    {
        public string RehinAlacak { get; set; }
        public string Kurum { get; set; }
        public string KurumID { get; set; }
        public string KurumAciklama { get; set; }
        public string SubeID { get; set; }
        public List<SelectListItem> Subeler { get; set; }
        public string SubeAciklama { get; set; }
        public string HesapSozlesmeNo { get; set; }
        public string KrediBitisTarih { get; set; }
        public string KrediTutari { get; set; }
        public string DovizKodu { get; set; }

    }
}