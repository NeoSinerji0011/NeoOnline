using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MapfreBelgeSorguRequest
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string brbBransKodu { get; set; }
        public string p_num_poliza { get; set; }
        public decimal p_num_spto { get; set; }
        public decimal p_num_apli { get; set; }
        public decimal p_num_spto_apli { get; set; }
        public string p_tip_emision { get; set; }

        public MapfreBelgeSorguRequest CloneForLog()
        {
            MapfreBelgeSorguRequest log = new MapfreBelgeSorguRequest();

            string user = String.Empty;
            string pass = String.Empty;

            if (!String.IsNullOrEmpty(this.userName) && this.userName.Length > 0)
            {
                user = this.userName[0] + "".PadLeft(this.userName.Length - 2, 'x') + this.userName[this.userName.Length - 1];
            }
            if (!String.IsNullOrEmpty(this.passWord) && this.passWord.Length > 0)
            {
                pass = this.passWord[0] + "".PadLeft(this.passWord.Length - 2, 'x') + this.passWord[this.passWord.Length - 1];
            }

            log.userName = user;
            log.passWord = pass;
            log.brbBransKodu = this.brbBransKodu;
            log.p_num_poliza = this.p_num_poliza;
            log.p_num_spto = this.p_num_spto;
            log.p_num_apli = this.p_num_apli;
            log.p_num_spto_apli = this.p_num_spto_apli;
            log.p_tip_emision = this.p_tip_emision;

            return log;
        }
    }
}
