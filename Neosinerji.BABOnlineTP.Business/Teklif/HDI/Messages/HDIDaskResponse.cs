using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIDaskResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public Uyarilar Uyarilar { get; set; }
        public PoliceBilgileri PoliceBilgileri { get; set; }
    }

    public class PoliceBilgileri
    {
        public string DASKRefNo { get; set; }
        public string HDIPoliceNo { get; set; }
        public string HDIYenilemeNo { get; set; }
        public string HDIZeyilNo { get; set; }

        public string DASKPoliceNo { get; set; }
        public string DASKYenilemeNo { get; set; }
        public string DASKZeyilNo { get; set; }

        public string PoliceBedeli { get; set; }
        public string Fiyat { get; set; }
        public string OdenecekPrim { get; set; }
        public string UygulananTaksitSayisi { get; set; }
        public string SigortaliSayisi { get; set; }
        public string SEUnvan { get; set; }
    }

    public class Uyari
    {
        public string UyariAciklama { get; set; }
    }

    public class Uyarilar
    {
        public List<Uyari> Uyari { get; set; }
    }


    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIIllerResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string Aciklama { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIIlcelerResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string Aciklama { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIBeldelerResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string Aciklama { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIMahallelerResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string Aciklama { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDICaddeSokakBulvarMeydanResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string Aciklama { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDICaddeSokakBulvarMeydanBinaAdResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string BinaAd { get; set; }
            public string BinaNo { get; set; }
            public string Ada { get; set; }
            public string Pafta { get; set; }
            public string Parsel { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIDairelerResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }

        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string Kod { get; set; }
            public string UAVT { get; set; }
            public string DaireNo { get; set; }
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIUAVTAdresResponse
    {
        public string Durum { get; set; }
        public string DurumAciklama { get; set; }


        public List<KAYIT> KAYITLAR { get; set; }

        public class KAYIT
        {
            public string IlKod { get; set; }
            public string IlAd { get; set; }
            public string IlceKod { get; set; }
            public string IlceAd { get; set; }
            public string BeldeKod { get; set; }
            public string BeldeAd { get; set; }
            public string Mahalle { get; set; }
            public string CSBMAd { get; set; }
            public string BinaAd { get; set; }
            public string BinaNo { get; set; }
            public string DaireNo { get; set; }
            public string Ada { get; set; }
            public string Pafta { get; set; }
            public string Parsel { get; set; }
        }
    }
}
