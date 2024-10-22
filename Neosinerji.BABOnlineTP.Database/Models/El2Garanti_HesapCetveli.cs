using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class El2Garanti_HesapCetveli
    {
        public byte Periyod { get; set; }
        public string TeminatTuru { get; set; }
        public int TeminatTutari { get; set; }
        public byte MotorHacmi { get; set; }
        public int Optional_1 { get; set; }
        public int Optional_2 { get; set; }
        public int Mandatory_1 { get; set; }
        public int Mandatory_2 { get; set; }
    }
}
