using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceArac
    {
        public int PoliceId { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public string Marka { get; set; }
        public string MarkaAciklama { get; set; }
        public string AracinTipiKodu { get; set; }
        public string AracinTipiAciklama { get; set; }
        public string AracinTipiKodu2 { get; set; }
        public string AracinTipiAciklama2 { get; set; }
        public Nullable<int> Model { get; set; }
        public string MotorNo { get; set; }
        public string SasiNo { get; set; }
        public string Cinsi { get; set; }
        public string TescilSeriKod { get; set; }
        public string TescilSeriNo { get; set; }
        public string AsbisNo { get; set; }
        public string YakitCinsi { get; set; }
        public string Renk { get; set; }
        public string KullanimSekli { get; set; }
        public string KullanimTarzi { get; set; }
        public string AracId { get; set; }
        public string SilindirHacmi { get; set; }
        public string MotorGucu { get; set; }
        public Nullable<int> KoltukSayisi { get; set; }
        public Nullable<int> YukKapasitesiKg { get; set; }
        public Nullable<byte> ImalatYeri { get; set; }
        public Nullable<System.DateTime> TrafikCikisTarihi { get; set; }
        public Nullable<System.DateTime> TrafikTescilTarihi { get; set; }
        public string TescilIlKodu { get; set; }
        public string TescilIlceKodu { get; set; }
        public string TramerBelgeNo { get; set; }
        public Nullable<System.DateTime> TramerBelgeTarihi { get; set; }
        public Nullable<decimal> AracDeger { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
    }
}
