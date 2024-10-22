using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA : ANADOLUMessage
    {
        public PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";

            //Tarih (YYYYAAGG)	Poliçe başlangıç tarihi
            this.BASLANGIC_TARIHI = TurkeyDateTime.Now.ToString("yyyyMMdd");

            //Tarih (YYYYAAGG)	Poliçe bitiş tarihi
            this.BITIS_TARIHI = TurkeyDateTime.Now.AddYears(1).ToString("yyyyMMdd");
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("DAIRE_YUZOLCUMU", this.DAIRE_YUZOLCUMU));
            sb.Append(MsgParameter("BELDE_KODU", this.BELDE_KODU));
            sb.Append(MsgParameter("BITIS_TARIHI", this.BITIS_TARIHI));
            sb.Append(MsgParameter("BINA_YAPI_TARZI", this.BINA_YAPI_TARZI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("BASLANGIC_TARIHI", this.BASLANGIC_TARIHI));
            sb.Append(MsgParameter("SESSION_ID", this.SESSION_ID));
            sb.Append(MsgParameter("RISK_ADRESI", this.RISK_ADRESI));
            sb.Append("</cms:PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string DAIRE_YUZOLCUMU { get; set; }
        public string BELDE_KODU { get; set; }
        public string BITIS_TARIHI { get; set; }
        public string BINA_YAPI_TARZI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string BASLANGIC_TARIHI { get; set; }
        public string SESSION_ID { get; set; }
        public string RISK_ADRESI { get; set; }
    }
}
