using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    [Serializable]
    public class KullaniciYetkiModel
    {
        public int MenuKodu { get; set; }
        public int AnaMenu { get; set; }
        public int SekmeKodu { get; set; }
        public string Aciklama { get; set; }

        public bool Active { get; set; }
        public bool HasChild { get; set; }

        //TEst
        public byte UrunYetki { get; set; }

        public string Yardim { get; set; }
        public int Seviye { get; set; }
        public short SiraNumarasi { get; set; }
        public string URL { get; set; }
        public string IslemId { get; set; }
        public string Icon { get; set; }
        public byte Gorme { get; set; }
        public byte YeniKayit { get; set; }
        public byte Degistirme { get; set; }
        public byte Silme { get; set; }
    }


}
