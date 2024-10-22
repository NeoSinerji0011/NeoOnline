using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class UlkeKodlari
    {
        public string UlkeKodu { get; set; }
        public string UlkeAdi { get; set; }
        public Nullable<byte> Garanti { get; set; }
        public Nullable<byte> SuprimOrani { get; set; }
        public byte SchengenMi { get; set; }
    }
}
