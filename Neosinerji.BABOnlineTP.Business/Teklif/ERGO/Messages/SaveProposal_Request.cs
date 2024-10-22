using Neosinerji.BABOnlineTP.Business.ergoturkiye.police.uretim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ERGO.Messages
{
    [Serializable]
    public class SaveProposal_Request : ERGOMessage
    {
        //public SaveProposal_Request()
        //{
        //    this.CORE_CLIENT = "ERE";
        //    this.CORE_DOMAIN = "ANADOLUSIGORTA";
        //    this.CORE_LANGUAGE = "tr";
        //    this.ZKTMS_SIRKET_KODU = String.Empty;
        //    this.ZKTMS_ACENTE_KODU = String.Empty;
        //    this.ZKTMS_ESKI_POLICE_NO = String.Empty;
        //    this.ZKTMS_YENILEME_NO = String.Empty;
        //    this.ONCEKI_ACENTE_KODU = String.Empty;
        //    this.ONCEKI_POLICE_NO = String.Empty;
        //    this.ONCEKI_SIRKET_KODU = String.Empty;
        //    this.ONCEKI_YENILEME_NO = String.Empty;
        //    this.TASIMA_YETKI_BELGE_NO = String.Empty;
        //    this.TASIMA_YETKI_BELGE_SERI_NO = String.Empty;
        //    this.TCKN = String.Empty;
        //    this.VKN = String.Empty;
        //}

        //public override string GetSoapAction()
        //{
        //    return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1";
        //}

        //public override string GetMessageBody()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<pol:saveProposal>");
        //    sb.Append("<proposal>");
        //    sb.Append(MsgParameter("agencyNumber", this.CORE_CLIENT));
        //    sb.Append(MsgParameter("agencySellerNumber", this.CORE_DOMAIN));
        //    sb.Append(MsgParameter("agencySubNumber", this.CORE_LANGUAGE));
        //    sb.Append(MsgParameter("authorizationCode", this.ARAC_KODU));
        //    sb.Append(MsgParameter("beginDate", this.ARAC_TESCIL_KODU));

        //    //sb.Append(MsgParameter("benefitList", this.ZKTMS_YENILEME_NO));
        //    sb.Append(MsgParameter("currencyCode", this.ZKTMS_SIRKET_KODU));
        //    sb.Append(MsgParameter("endDate", this.ZKTMS_ESKI_POLICE_NO));
        //    sb.Append(MsgParameter("issueDate", this.ZKTMS_ACENTE_KODU));
        //    sb.Append(MsgParameter("paymentPlanId", this.WEB_KULLANICI_ADI));
        //    sb.Append(MsgParameter("productNumber", this.WEB_GIRIS));
        //    sb.Append(MsgParameter("VKN", this.VKN));
        //    sb.Append(MsgParameter("TRAMER_TARIFE_KODU", this.TRAMER_TARIFE_KODU));
        //    sb.Append(MsgParameter("TRAFIK_TESCIL_TARIHI", this.TRAFIK_TESCIL_TARIHI));
        //    sb.Append(MsgParameter("TCKN", this.TCKN));
        //    sb.Append(MsgParameter("TASIMA_YETKI_BELGE_SERI_NO", this.TASIMA_YETKI_BELGE_SERI_NO));
        //    sb.Append(MsgParameter("ARAC_TESCIL_SERI_NO", this.ARAC_TESCIL_SERI_NO));
        //    sb.Append(MsgParameter("BITIS_TARIHI", this.BITIS_TARIHI));
        //    sb.Append(MsgParameter("DOGUM_TARIHI", this.DOGUM_TARIHI));
        //    sb.Append(MsgParameter("CINSIYET", this.CINSIYET));
        //    sb.Append(MsgParameter("MUSTERI_ILCE_KODU", this.MUSTERI_ILCE_KODU));
        //    sb.Append(MsgParameter("MUSTERI_IL_KODU", this.MUSTERI_IL_KODU));
        //    sb.Append(MsgParameter("MUSTERI_TIPI", this.MUSTERI_TIPI));
        //    sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
        //    sb.Append(MsgParameter("ONCEKI_ACENTE_KODU", this.ONCEKI_ACENTE_KODU));
        //    sb.Append(MsgParameter("ONCEKI_POLICE_NO", this.ONCEKI_POLICE_NO));
        //    sb.Append(MsgParameter("ONCEKI_SIRKET_KODU", this.ONCEKI_SIRKET_KODU));
        //    sb.Append(MsgParameter("ONCEKI_YENILEME_NO", this.ONCEKI_YENILEME_NO));
        //    sb.Append(MsgParameter("PLAKA", this.PLAKA));
        //    sb.Append(MsgParameter("PLAKA_IL_KODU", this.PLAKA_IL_KODU));
        //    sb.Append(MsgParameter("PLAKA_TIPI", this.PLAKA_TIPI));
        //    sb.Append(MsgParameter("TASIMA_YETKI_BELGE_NO", this.TASIMA_YETKI_BELGE_NO));
        //    sb.Append(MsgParameter("SESSION_ID", String.Empty));
        //    sb.Append("</proposal>");
        //    sb.Append("</pol:saveProposal>");
        //    return sb.ToString();
        //}



        public string agencyNumber { get; set; }
        public string agencySellerNumber { get; set; }
        public string agencySubNumber { get; set; }
        public string authorizationCode { get; set; }
        public string beginDate { get; set; }
        public benefitWsDto[] benefitList { get; set; }
        public coverageWsDto[] coverageList { get; set; }
        public string currencyCode { get; set; }
        public customerWsDto[] customerList { get; set; }
        public discountSurchargeWsDto[] discountSurchargeList { get; set; }
        public string endDate { get; set; }
        public string issueDate { get; set; }
        public lossPayeeWsDto lossPayee { get; set; }
        public pPropertyWsDto[] pPropertyList { get; set; }
        public string paymentPlanId { get; set; }
        public string productNumber { get; set; }
        public System.Nullable<int>[] profileIdList  { get; set; }
      
    }

}