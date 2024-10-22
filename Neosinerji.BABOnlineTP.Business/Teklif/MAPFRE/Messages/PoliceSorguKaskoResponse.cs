using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    [XmlType(TypeName = "result")]
    public class PoliceSorguKaskoResponse : MapfreSorguResponse
    {
        public string hata { get; set; }
        public string durum { get; set; }
        public string durumAciklamasi { get; set; }

        public kaskoPoliceList yururlukPoliceList { get; set; }
        public kaskoPoliceList oncekiPoliceList { get; set; }
    }

    public class kaskoPoliceList
    {
        [XmlElement("TramerSorguPoliceValue")]
        public TramerSorguPoliceValue[] TramerSorguPolice { get; set; }
    }

    public class TramerSorguPoliceValue
    {
        public string sigortaliAdUnvan { get; set; }
        public string sigortaliTurKod { get; set; }
        public string sigortaliVatandaslikNo { get; set; }
        public string sigortaliVergiNo { get; set; }
        public string sigortaliPasaportNo { get; set; }
        public string sigortaliUyruk { get; set; }
        public string sigortaSirketKodu { get; set; }
        public string acenteNo { get; set; }
        public string policeNo { get; set; }
        public string yenilemeNo { get; set; }
        public string tanzimTarihi { get; set; }
        public string policeBaslamaTarihi { get; set; }
        public string policeBitisTarihi { get; set; }
        public string policeEkiTuru { get; set; }
        public string policeEkiBaslamaTarihi { get; set; }
        public string oncekiSirketKodu { get; set; }
        public string oncekiAcenteNo { get; set; }
        public string oncekiPoliceNo { get; set; }
        public string oncekiYenilemeNo { get; set; }
        public string oncekiTarifeBasamakKodu { get; set; }
        public string aracTarifeGrupKodu { get; set; }
        public string plakaIlKodu { get; set; }
        public string plakaNo { get; set; }
        public string aracMarkaKodu { get; set; }
        public string marka { get; set; }
        public string aracTipKodu { get; set; }
        public string aracinTipi { get; set; }
        public string modelYili { get; set; }
        public string motorNo { get; set; }
        public string sasiNo { get; set; }
        public string kullanimSekli { get; set; }
        public string trafikTescilTarihi { get; set; }
        public string aracRengi { get; set; }
        public string silindirHacmi { get; set; }
        public string motorGucu { get; set; }
        public string imalatYeri { get; set; }
        public string yolcuKapasitesi { get; set; }
        public string uygulanmisTarifeBasamak { get; set; }
        public string uygulanmisIndirimYuzde { get; set; }
        public string uygulanmisSurpirimYuzde { get; set; }
        public string ilPlakaIndirimYuzde { get; set; }
        public string tasimacilikIndirimYuzde { get; set; }
        public string tramerBelgeNo { get; set; }
        public string tramerBelgeTarih { get; set; }
        public string tasimacilikSigortaSirketKodu { get; set; }
        public string tasimacilikAcenteNo { get; set; }
        public string tasimacilikPoliceNo { get; set; }
        public string tasimacilikYenilemeNo { get; set; }
        public string ekBelgeAciklama { get; set; }
        public string iptalPolice { get; set; }
        public string iptalTarihi { get; set; }
        public string uygulananKademeNo { get; set; }
        public string iptalNedeni { get; set; }

        [XmlIgnore]
        public DateTime PoliceBitisTarihi
        {
            get
            {
                return MapfreSorguResponse.ToDateTime(policeBitisTarihi);
            }
        }
    }
}
