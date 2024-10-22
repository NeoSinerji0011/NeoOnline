using Neosinerji.BABOnlineTP.Business.Komisyon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models
{
    public class TaliKomisyonOranModel
    {
        public TVMModel Tali { get; set; }
        public SigortaSirketiModel SigortaSirketi { get; set; }
        public BransModel Brans { get; set; }
        public DateTime Tarih { get; set; }
        public string policeNo { get; set; }
    }
}