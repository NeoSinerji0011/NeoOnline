using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AnadoluKullanimTipSekil
    {
        public int Id { get; set; }
        public string NeoOnlineKullanimTarzi { get; set; }
        public string KullanimTipi { get; set; }
        public string TipAdi { get; set; }
        public string KullanimSekli { get; set; }
        public string SekilAdi { get; set; }
        public short Durum { get; set; }
    }
}
