using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Models
{
    public class ErrorModel
    {
        public int HataKodu { get; set; }
        public string Mesaj { get; set; }
        public string ReturnURL { get; set; }
    }
}