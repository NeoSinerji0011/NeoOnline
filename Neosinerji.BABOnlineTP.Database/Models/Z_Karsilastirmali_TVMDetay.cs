using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_Karsilastirmali_TVMDetay
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public short Tipi { get; set; }
        public string KayitNo { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string TCKN { get; set; }
        public byte Profili { get; set; }
        public int BagliOlduguTVMKodu { get; set; }
        public byte AcentSuvbeVar { get; set; }
        public byte Durum { get; set; }
        public Nullable<System.DateTime> DurumGuncallemeTarihi { get; set; }
        public Nullable<System.DateTime> SozlesmeBaslamaTarihi { get; set; }
        public Nullable<System.DateTime> SozlesmeDondurmaTarihi { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebAdresi { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public Nullable<short> IlceKodu { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }
        public int BolgeKodu { get; set; }
        public short SifreKontralSayisi { get; set; }
        public short SifreDegistirmeGunu { get; set; }
        public short SifreIkazGunu { get; set; }
        public Nullable<int> GrupKodu { get; set; }
        public byte BaglantiSiniri { get; set; }
        public Nullable<short> UcretlendirmeKodu { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<bool> MuhasebeEntegrasyon { get; set; }
        public string ProjeKodu { get; set; }
    }
}
