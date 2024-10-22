using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectSirketDetay
    {
        public int Id { get; set; }
        public int TUMKodu { get; set; }
        public string SigortaSirketAdi { get; set; }
        public string InputTextKullaniciId { get; set; }
        public string InputTextAcenteKoduId { get; set; }
        public string InputTextSifreId { get; set; }
        public string InputTextGirisId { get; set; }
        public string LoginUrl { get; set; }
    }
}
