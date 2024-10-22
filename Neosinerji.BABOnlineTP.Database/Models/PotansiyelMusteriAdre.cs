using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PotansiyelMusteriAdre
    {
        public int PotansiyelMusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public Nullable<int> AdresTipi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public Nullable<int> IlceKodu { get; set; }
        public string Adres { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public string HanAptFab { get; set; }
        public Nullable<int> PostaKodu { get; set; }
        public string Diger { get; set; }
        public Nullable<bool> Varsayilan { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public virtual PotansiyelMusteriGenelBilgiler PotansiyelMusteriGenelBilgiler { get; set; }
    }
}
