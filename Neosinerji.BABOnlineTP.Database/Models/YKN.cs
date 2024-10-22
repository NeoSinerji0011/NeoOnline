using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class YKN
    {
        public Nullable<long> KayitNo { get; set; }
        public string BabaAdi { get; set; }
        public string Ad { get; set; }
        public string VerilisNedeni { get; set; }
        public Nullable<int> AileSiraNo { get; set; }
        public Nullable<int> CiltKod { get; set; }
        public Nullable<int> VerildigiIlce { get; set; }
        public string IlceAd { get; set; }
        public Nullable<int> IlceKod { get; set; }
        public Nullable<System.DateTime> VerilisTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public Nullable<int> BireySiraNo { get; set; }
        public Nullable<int> Num { get; set; }
        public string AnaAdi { get; set; }
        public string Durum { get; set; }
        public Nullable<System.DateTime> DogumTarihi { get; set; }
        public Nullable<System.DateTime> OlumTarihi { get; set; }
        public string Seri { get; set; }
        public string DogumYeri { get; set; }
        public Nullable<int> IlKodu { get; set; }
        public string KimlikNo { get; set; }
        public string VerildigiIlceAdi { get; set; }
        public string CiltAd { get; set; }
        public string Soyad { get; set; }
        public string SorgulamaYeri { get; set; }
        public string IkametTezkereNo { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
    }
}
