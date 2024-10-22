using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMIletisimYetkilileri
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public string GorusulenKisi { get; set; }
        public string Gorevi { get; set; }
        public byte TelefonTipi { get; set; }
        public string TelefonNo { get; set; }
        public string Email { get; set; }
    }
}
