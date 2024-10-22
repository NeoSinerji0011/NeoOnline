using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Robot.Models
{
    public class RobotModel
    {
        public string Token { get; set; }
        public int TvmKodu { get; set; }
        public int TvmKK { get; set; }
        public string Port { get; set; }

    }
}