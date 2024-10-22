using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifEMailLog
    {
        public int LogId { get; set; }
        public int TeklifId { get; set; }
        public System.DateTime Tarih { get; set; }
        public string Email { get; set; }
        public string FormatAdi { get; set; }
        public Nullable<bool> Basarili { get; set; }
    }
}
