using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DepremMuafiyet
    {
        public int TeminatKodu { get; set; }
        public byte YazlikKislik { get; set; }
        public byte Kademe { get; set; }
        public decimal Fiyat { get; set; }
        public string Aciklama { get; set; }
    }
}
