using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarIletisimYetkilileri
    {
        public int IletisimYetkiliId { get; set; }
        public int HasarId { get; set; }
        public string GorusulenKisi { get; set; }
        public string Gorevi { get; set; }
        public byte TelefonTipi { get; set; }
        public string TelefonNo { get; set; }
        public string Email { get; set; }
        public virtual HasarGenelBilgiler HasarGenelBilgiler { get; set; }
    }
}
