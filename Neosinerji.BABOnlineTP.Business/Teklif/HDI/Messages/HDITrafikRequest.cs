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
    public class HDITrafikRequest : HDIMessage
    {
        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string Refno { get; set; }
        public string IstekTip { get; set; }
        public string OrjinalRefNo { get; set; }
        public string HDIPoliceNumara { get; set; }
        public string HDIPoliceYNo { get; set; }
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
        public string PasAdi { get; set; }
        public string PasSoyad { get; set; }
        public string PasBabad { get; set; }
        public string PasDogYR { get; set; }
        public string PasDogTR { get; set; }
        public string LMUSNO { get; set; }
        public string LRIMNO { get; set; }
        public string IsTel1 { get; set; }
        public string IsTel2 { get; set; }
        public string EvTel1 { get; set; }
        public string EvTel2 { get; set; }
        public HDIAdresBilgi AdresBilgi { get; set; }
        public string SinifTarifeKod { get; set; }
        public string STRFKd2 { get; set; }
        public HDIEskiPoliceBilgileri EskiPoliceBilgileri { get; set; }
        public string TescilTarihi { get; set; }
        public string ModelYili { get; set; }
        public string AracListeKod { get; set; }
        public string Marka { get; set; }
        public string AracTipiModeli { get; set; }
        public string RnkKd { get; set; }
        public string MotorNo { get; set; }
        public string SasiNo { get; set; }
        public string YakitTipi { get; set; }
        public string GarajTipi { get; set; }
        public string YillikKm { get; set; }
        public string SurucuAdetKd { get; set; }
        public string PlakailKod { get; set; }
        public string PlakaKod2 { get; set; }
        public string YabanciPlaka { get; set; }
        public string PulNo1 { get; set; }
        public string PulNo2 { get; set; }
        public string IMMTeminati { get; set; }
        public string WSIMMK { get; set; }
        public string WSKOLS { get; set; }
        public string WSKOLK { get; set; }
        public string WSKOSY { get; set; }
        public string WSHUKS { get; set; }
        public string WSHUKK { get; set; }
        public string AsistanHizmeti { get; set; }
        public string TrafigeCikisTarihi { get; set; }
        public string YetkiBelgesiVarmi { get; set; }
        public string TasiyiciSorumluluk { get; set; }
        public string TasSorSigSirKod { get; set; }
        public string TasSorAcenteKod { get; set; }
        public string TasSorPoliceNo { get; set; }
        public string TasSorYenilemeNo { get; set; }
        public string OdemeSekli { get; set; }
        public string TaksitSayisi { get; set; }
        public string TckR { get; set; }
        public string MerkezKod { get; set; }
        public string SubeKod { get; set; }
        public string Reseller_user_ID { get; set; }
        public string Reseller_user_firstname { get; set; }
        public string Reseller_user_lastname { get; set; }
        public string TBSKd { get; set; }
        public string TBSNo { get; set; }
    }
}
