using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY.Messages.Kasko
{
    [XmlRootAttribute("POLICY")]
    public class RAY_KaskoTeklifKayit_Request
    {
        public string USER_NAME;
        public string PASSWORD;
        public string PRODUCT_NO;
        public string CLIENT_IP;
        public string MUN_CITY_CODE;
        public string MUN_TOWN_CODE;
        public string MUN_CODE;
        public string MUN_COUNTRY_CODE;
        public string PROCESS_ID;
        public string POLICY_NO;
        public string RENEWAL_NO;
        public string ENDORS_NO;
        public string ENDORS_TYPE_CODE;
        public string PROD_CANCEL;
        public string CANCEL_REASON_CODE;
        public string CHANNEL;

        public string ISSUE_CHANNEL;
        public string BEG_DATE;
        public string END_DATE;
        public string ISSUE_DATE;
        public string CONFIRM_DATE;
        public string TRANSFER_DATE;
        public string PROPOSAL_DATE;
        public string CUR_TYPE;
        public string CUR_ACCOUNT_TYPE;
        public string EXCHANGE_RATE;
        public string PAYMENT_TYPE;
        public string QUERY_METHOD;
        public string DESCRIPTION;
        public string FIRM_CODE;
        public string COMPANY_CODE;
        public string CLIENT_NO;
        public string INSURED_NO;
        public string CATEGORY1;
        public string CATEGORY2;
        public string PRINTOUT_TYPE;
        public string PLATE;

        [XmlElement("QUESTION")]
        public QUESTION[] QUESTION;
    }

    public class QUESTION
    {
        public string QUESTION_CODE;
        public string ANSWER;
    }
}
