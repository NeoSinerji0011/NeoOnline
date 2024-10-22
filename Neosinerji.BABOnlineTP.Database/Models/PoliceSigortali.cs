using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceSigortali
    {
        public int PoliceId { get; set; }
        public string KimlikNo { get; set; }
        public string VergiKimlikNo { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public Nullable<short> TipKodu { get; set; }
        public string Cinsiyet { get; set; }
        public Nullable<System.DateTime> DogumTarihi { get; set; }
        public string MobilTelefonNo { get; set; }
        public string TelefonNo { get; set; }
        public string UlkeKodu { get; set; }
        public string UlkeAdi { get; set; }
        public string IlKodu { get; set; }
        public string IlAdi { get; set; }
        public Nullable<int> IlceKodu { get; set; }
        public string IlceAdi { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public string HanAptFab { get; set; }
        public Nullable<int> PostaKodu { get; set; }
        public string Adres { get; set; }
        public string MusteriTemsilciKodu { get; set; }
        public string TahsilatSorumlusuKodu { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string MuhasebeEntegrasyonHesapNo { get; set; }
        public Nullable<bool> DainiMurtehin { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EMail { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
       
    }
}
