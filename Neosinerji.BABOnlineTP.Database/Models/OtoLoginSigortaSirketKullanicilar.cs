using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class OtoLoginSigortaSirketKullanicilar
    {
        public int TVMKodu { get; set; }
        public Nullable<int> AltTVMKodu { get; set; }
        public int TUMKodu { get; set; }
        public string SigortaSirketAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public string AcenteKodu { get; set; }
        public string Sifre { get; set; }
        public string InputTextKullaniciId { get; set; }
        public string InputTextAcenteKoduId { get; set; }
        public string InputTextSifreId { get; set; }
        public string InputTextGirisId { get; set; }
        public string LoginUrl { get; set; }
        public string ProxyIpPort { get; set; }
        public string ProxyKullaniciAdi { get; set; }
        public string ProxySifre { get; set; }
        public string SmsKodTelNo { get; set; }
        public string SmsKodSecretKey1 { get; set; }
        public string SmsKodSecretKey2 { get; set; }
        public int Id { get; set; }
        public Nullable<int> GrupKodu { get; set; }
    }
}
