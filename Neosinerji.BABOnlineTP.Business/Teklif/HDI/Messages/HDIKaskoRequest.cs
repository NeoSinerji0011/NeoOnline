using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIKaskoRequest : HDIMessage
    {
        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string Refno { get; set; }
        public string IstekTip { get; set; }
        public string OrjinalRefNo { get; set; }
        public string HDIPoliceNumara { get; set; }
        public string HDIPoliceYenilemeNo { get; set; }
        public string IptalNedeni { get; set; }
        public string Tarih { get; set; }
        public string OzelTuzel { get; set; }
        public string MSCnsTp { get; set; }
        public string MSDogYL { get; set; }
        public string MSEgKd { get; set; }
        public string MSMesKd { get; set; }
        public string MSMedKd { get; set; }
        public string MSCocKd { get; set; }
        public string MSEhlYL { get; set; }
        public string MSSekKd { get; set; }
        public string TcKimlikNo { get; set; }
        public string VergiKimlikNo { get; set; }
        public string Uyruk { get; set; }
        public string YabanciKimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string SigortaEttirenAd { get; set; }
        public string SigortaEttirenSoyad { get; set; }
        public string IsTel1 { get; set; }
        public string IsTel2 { get; set; }
        public string EvTel1 { get; set; }
        public string EvTel2 { get; set; }
        //public HDIAdresBilgi AdresBilgi { get; set; }
        public string Adres { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Semt { get; set; }
        public string KoyMahalle { get; set; }
        public string BinaNo { get; set; }
        public string HanApartmanAd { get; set; }
        public string Daire { get; set; }
        public string Kat { get; set; }
        public string PostaKod { get; set; }
        public string Ilce { get; set; }
        public string IlKod { get; set; }
        public string SinifTarifeKod { get; set; }
        public string SinifTarifeKdEk { get; set; }
        public string AracListeKod { get; set; }
        public string Marka { get; set; }
        public string ModelYil { get; set; }
        public string AracTipiModeli { get; set; }
        public string Renk { get; set; }
        public string MotorNo { get; set; }
        public string SasiNo { get; set; }
        public string YakitTipi { get; set; }
        public string KoltukSayisi { get; set; }
        public string GarajTipi { get; set; }
        public string YillikKm { get; set; }
        public string SurucuAdetKd { get; set; }
        public string TrafigeCikisTarihi { get; set; }
        public string PlakailKod { get; set; }
        public string PlakaKod2 { get; set; }
        public string YabanciPlaka { get; set; }
        public string DainiMurtein { get; set; }
        public string DainMurteinTanim { get; set; }
        public string OdemeTipi { get; set; }
        public string TaksitSekli { get; set; }
        public string TckR { get; set; }
        public string ihtiyariKdVarmi { get; set; }
        public string IhtiyariKademe { get; set; }
        public string koltukferdiKdVarmi { get; set; }
        public string KoltukFerdiKademe { get; set; }
        public string HukuksalKorKdVarmi { get; set; }
        public string HkkslKorumaKademe { get; set; }
        public string YurtIciTasiyiciVarmi { get; set; }
        public string YutIciTasiyiciKademe { get; set; }
        public string WS31IA { get; set; }
        public string KisiSayi { get; set; }
        public string AracDegeri { get; set; }
        public string Deprem { get; set; }
        public string T2301Kdm { get; set; }
        public string SelSu { get; set; }
        public string T2303Kdm { get; set; }
        //public HDIEskiPoliceBilgileri EskiPoliceBilgileri { get; set; }
        public string EskiPoliceSirket { get; set; }
        public string EskiPoliceAcente { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
        public string TeklifNo { get; set; }
        public string SubeKod { get; set; }
        public string Reseller_user_ID { get; set; }
        public string Reseller_user_firstname { get; set; }
        public string Reseller_user_lastname { get; set; }
        public string Saglik { get; set; }
        public string TBSKd { get; set; }
        public string TBSNo { get; set; }
        public string WSSTC { get; set; }

        // === HASARSIZLIK KORUMA TEMİNATI VAR MI E/H === //
        public string T2345 { get; set; }

        // ==== Sonradan eklendi ==== // 
        public string WSHSST { get; set; }
        public string WSHSPT { get; set; }
        public string AnahtarKaybi { get; set; }
        public string Alarm { get; set; }
        public string KullanimKaybi { get; set; }


        // === MESLEK İNDİRİMİ E/H === //
        public string T2451 { get; set; }

        // === KASA BEDELİ 12,2 (Tarife Grubu 523,521 için) === //
        public string T2180 { get; set; }

        // === Taşınan Yük Teminatı === //
        public string TasinanYukKademe { get; set; }
        public string TasinanYukTeminatDegeri { get; set; }
        public string WS1MRKTSYACK { get; set; }


        public string KisiselEsya { get; set; }
        public string T2355 { get; set; }//RAYİÇ DEĞER KORUMA TEMİNATI (E/H)
        public string T2354 { get; set; }//Yanlış Akaryakıt Alımı Teminatı (E/H)
        public string T2322 { get; set; }//PATLAYICI PARLAYICI VE YANICI MADDE TASIMA E/H
        
        //
    }
}
