using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class MenuIslem
    {
        public int IslemKodu { get; set; }
        public string IslemId { get; set; }
        public string URL { get; set; }
        public string Icon { get; set; }
    }
}
