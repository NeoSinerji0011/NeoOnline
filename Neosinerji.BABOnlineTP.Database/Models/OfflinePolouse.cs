using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class OfflinePolouse
    {
        public int Id { get; set; }
        public int UrunKodu { get; set; }
        public int TVMKodu { get; set; }
        public string SEKimlikNo { get; set; }
        public string SETVMMusteriKodu { get; set; }
        public string SKimlikNo { get; set; }
        public string STVMMusteriKodu { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyileNo { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> Komisyon { get; set; }
        public Nullable<System.DateTime> PoliceBaslangicTarihi { get; set; }
        public Nullable<System.DateTime> PoliceBitisTarihi { get; set; }
        public Nullable<System.DateTime> TanzimTarihi { get; set; }
        public Nullable<byte> TaksitAdedi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public string KrediKartiBankaAdi { get; set; }
        public Nullable<byte> SatisTuru { get; set; }
        public string SatisTemsilcisiTCKN { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
