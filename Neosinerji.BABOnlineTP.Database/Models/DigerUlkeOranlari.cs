using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DigerUlkeOranlari
    {
        public int Gun1 { get; set; }
        public int Gun2 { get; set; }
        public byte KisiTipi { get; set; }
        public byte PlanTipi { get; set; }
        public byte Extra { get; set; }
        public byte Yil_1 { get; set; }
        public int Oran { get; set; }
    }
}
