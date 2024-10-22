using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MapfrePrintRequest
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string p_num_poliza { get; set; }
        public decimal p_num_spto { get; set; }
        public decimal p_num_apli { get; set; }
        public decimal p_num_spto_apli { get; set; }
        public decimal p_num_riesgos { get; set; }
        public string p_tip_emision { get; set; }
    }
}
