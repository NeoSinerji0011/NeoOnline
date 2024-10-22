using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY.Messages.Kasko
{

    [XmlRootAttribute("POLICY")]
    public class RAY_KaskoPolicelestirme_Request
    {
        public string USER_NAME;
        public string PASSWORD;
        public string PRODUCT_NO;
        public string PROCESS_ID;
        public string FIRM_CODE;
        public string COMPANY_CODE;
        public string POLICY_NO;
        public string RENEWAL_NO;
        public string ENDORS_NO;
        public string CHANNEL;
        public string ISSUE_CHANNEL;
        public string CONFIRM_DATE;
        public string QUERY_METHOD;
        public string CLIENT_IP;
        public string ENDORS_TYPE_CODE;
        public string PROD_CANCEL;
        public string CANCEL_REASON_CODE;
        public string BEG_DATE;
        public string END_DATE;
        public string CUR_TYPE;
        public string EXCHANGE_RATE;
        public string PAYMENT_TYPE;
        public string DESCRIPTION;
        public string CLIENT_NO;
        public string INSURED_NO;
        public string PRINTOUT_TYPE;
        public string PLATE;
        public string HAS_INST_CARD;

       // public PAYMENTS PAYMENTS = new PAYMENTS();

    }

    public class PAYMENTS
    {
        public string CREDIT_CARD_NO;
        public string CREDIT_CARD_NAME;
        public string CREDIT_CARD_SURNAME;
        public string CREDIT_CARD_VALID_MONTH;
        public string CREDIT_CARD_VALID_YEAR;
        public string CREDIT_CARD_CVV;
        public string INSTALLMENT_NUMBER;
        public string CC_PREFIX;
        public string CREDIT_CARD_TYPE;
    }
}
