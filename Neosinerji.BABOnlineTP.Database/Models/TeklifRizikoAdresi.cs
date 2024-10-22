using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifRizikoAdresi
    {
        public int RizikoAdresiId { get; set; }
        public int TeklifId { get; set; }
        public int SiraNo { get; set; }
        public Nullable<int> IlKodu { get; set; }
        public string Il { get; set; }
        public Nullable<int> IlceKodu { get; set; }
        public string Ilce { get; set; }
        public string SemtBeldeKodu { get; set; }
        public string SemtBelde { get; set; }
        public string MahalleKodu { get; set; }
        public string Mahalle { get; set; }
        public string CaddeKodu { get; set; }
        public string Cadde { get; set; }
        public string SokakKodu { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string BinaKodu { get; set; }
        public string Bina { get; set; }
        public string DaireKodu { get; set; }
        public string Daire { get; set; }
        public string HanAptFab { get; set; }
        public Nullable<int> PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UAVTKodu { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
