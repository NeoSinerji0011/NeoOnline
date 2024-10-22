using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class BankaSubeleri
    {
        public int Id { get; set; }
        public string Banka { get; set; }
        public string Sube { get; set; }
        public string Adres { get; set; }
        public string Ilce { get; set; }
        public string Sehir { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Acilis { get; set; }
        public string Kapanis { get; set; }
    }
}
