using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY.Message
{
    [XmlRootAttribute("POLICY", Namespace = "", IsNullable = false)]
    public class RAY_TrafikPolicelestirme_Response
    {
        public POLMAS POLMAS { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }
    }

    public class POLMAS
    {
        [XmlElement]
        public string PRODUCT_NO { get; set; }

        [XmlElement]
        public string PRODUCT_NAME { get; set; }

        [XmlElement]
        public string POLICY_NO { get; set; }

        [XmlElement]
        public string RENEWAL_NO { get; set; }

        [XmlElement]
        public string ENDORS_NO { get; set; }

        [XmlElement]
        public string ENDORS_TYPE_CODE { get; set; }

        [XmlElement]
        public string PROD_CANCEL { get; set; }

        [XmlElement]
        public string CANCEL_REASON_CODE { get; set; }

        [XmlElement]
        public string POLICY_STATUS { get; set; }

        [XmlElement]
        public string CHANNEL { get; set; }

        [XmlElement]
        public string CLIENT_NO { get; set; }

        [XmlElement]
        public string ISSUE_CHANNEL { get; set; }

        [XmlElement]
        public string SUB_CHANNEL_CODE { get; set; }

        [XmlElement]
        public string BEG_DATE { get; set; }

        [XmlElement]
        public string END_DATE { get; set; }

        [XmlElement]
        public string ISSUE_DATE { get; set; }

        [XmlElement]
        public string CONFIRM_DATE { get; set; }

        [XmlElement]
        public string CONFIRM_CANCEL_DATE { get; set; }

        [XmlElement]
        public string POL_BEG_DATE { get; set; }

        [XmlElement]
        public string POL_END_DATE { get; set; }

        [XmlElement]
        public string POL_ISSUE_DATE { get; set; }

        [XmlElement]
        public string PROPOSAL_DATE { get; set; }

        [XmlElement]
        public string SYSTEM_DATE { get; set; }

        [XmlElement]
        public string SYSTEM_TIME { get; set; }

        [XmlElement]
        public string SPECIAL_COMM_RATE { get; set; }

        [XmlElement]
        public string SPECIAL_COMM_AMOUNT { get; set; }

        [XmlElement]
        public string LC_SPECIAL_COMM_AMOUNT { get; set; }

        [XmlElement]
        public string NET_PREMIUM { get; set; }

        [XmlElement]
        public string LC_NET_PREMIUM { get; set; }

        [XmlElement]
        public string GROSS_PREMIUM { get; set; }

        [XmlElement]
        public string LC_GROSS_PREMIUM { get; set; }

        [XmlElement]
        public string CUR_TYPE { get; set; }

        [XmlElement]
        public string EXCHANGE_TYPE { get; set; }

        [XmlElement]
        public string BANK_NO { get; set; }

        [XmlElement]
        public string BANK_SUB_BRANCH { get; set; }

        [XmlElement]
        public string EXCHANGE_RATE { get; set; }

        [XmlElement]
        public string PAYMENT_TYPE { get; set; }

        [XmlElement]
        public string ADVANCED_PAYMENT_AMOUNT { get; set; }

        [XmlElement]
        public string REF_PRODUCT_NO { get; set; }

        [XmlElement]
        public string REF_POLICY_NO { get; set; }

        [XmlElement]
        public string REF_RENEWAL_NO { get; set; }

        [XmlElement]
        public string REF_ENDORS_NO { get; set; }

        [XmlElement]
        public string OLD_PRODUCT_NO { get; set; }

        [XmlElement]
        public string OLD_POLICY_NO { get; set; }

        [XmlElement]
        public string NEW_PRODUCT_NO { get; set; }

        [XmlElement]
        public string NEW_POLICY_NO { get; set; }

        [XmlElement]
        public string PROPOSAL_NO { get; set; }

        [XmlElement]
        public string PRINT_TOTAL { get; set; }

        [XmlElement]
        public string RENEWAL_DATE { get; set; }

        [XmlElement]
        public string MUN_COUNTRY_CODE { get; set; }

        [XmlElement]
        public string MUN_STATE_CODE { get; set; }

        [XmlElement]
        public string MUN_CITY_CODE { get; set; }

        [XmlElement]
        public string MUN_TOWN_CODE { get; set; }

        [XmlElement]
        public string MUN_CODE { get; set; }

        [XmlElement]
        public string PLATE { get; set; }

        [XmlElement]
        public string INSURED_NO { get; set; }

        [XmlElement]
        public string PERSONEL_COMMERCIAL { get; set; }

        [XmlElement]
        public string INSURED_NAME { get; set; }

        [XmlElement]
        public string INSURED_SURNAME { get; set; }

        [XmlElement]
        public string INSURED_FIRM_NAME { get; set; }

        [XmlElement]
        public string LAST_USER_NAME { get; set; }

        [XmlElement]
        public string ENTRY_DATE { get; set; }

        [XmlElement]
        public string ENTRY_TIME { get; set; }

        [XmlElement]
        public string POLICY_SERIAL_NO { get; set; }

        [XmlElement]
        public string ROUNDED_GROSS_PREMIUM { get; set; }

        [XmlElement]
        public string NUMBER_OF_INSURED { get; set; }

        [XmlElement]
        public string NUMBER_OF_INSTALLMENT { get; set; }

        [XmlElement]
        public string CANCELLATION_DATE { get; set; }

        [XmlElement]
        public string OLD_COMPANY_CODE { get; set; }

        [XmlElement]
        public string NEW_COMPANY_CODE { get; set; }

        [XmlElement]
        public string PAYMENT_NAME { get; set; }

        [XmlElement]
        public string EXTERNAL_POLICY_NO { get; set; }

        [XmlElement]
        public string FIRM_CODE { get; set; }

        [XmlElement]
        public string COMPANY_CODE { get; set; }

        [XmlElement]
        public string CLIENT_FIRM_CODE { get; set; }

        [XmlElement]
        public string CLIENT_UNIT_TYPE { get; set; }

        [XmlElement]
        public string PERSONAL_COMMERCIAL { get; set; }

        [XmlElement]
        public string PORTAL_PRODUCT_NO { get; set; }

        [XmlElement]
        public string POLICY_NAME { get; set; }


        private PSRCVP[] _PSRCVP;
        private POLTEM[] _POLTEM;
        private POLTAH[] _POLTAH;


        [XmlElement]
        public PSRCVP[] PSRCVP { get { return _PSRCVP; } set { _PSRCVP = value; } }

        [XmlElement]
        public POLTEM[] POLTEM { get { return _POLTEM; } set { _POLTEM = value; } }

        [XmlElement]
        public POLTAH[] POLTAH { get { return _POLTAH; } set { _POLTAH = value; } }
    }

    public class PSRCVP
    {
        private string _QUESTION_PLACE;
        private string _QUESTION_CODE;
        private string _QUESTION_EXPLANATION;
        private string _ANSWER;
        private string _ANSWER_EXPLANATION;

        [XmlElement]
        public string QUESTION_PLACE { get { return _QUESTION_PLACE; } set { _QUESTION_PLACE = value; } }

        [XmlElement]
        public string QUESTION_CODE { get { return _QUESTION_CODE; } set { _QUESTION_CODE = value; } }

        [XmlElement]
        public string QUESTION_EXPLANATION { get { return _QUESTION_EXPLANATION; } set { _QUESTION_EXPLANATION = value; } }

        [XmlElement]
        public string ANSWER { get { return _ANSWER; } set { _ANSWER = value; } }

        [XmlElement]
        public string ANSWER_EXPLANATION { get { return _ANSWER_EXPLANATION; } set { _ANSWER_EXPLANATION = value; } }

    }

    public class POLTEM
    {
        private string _COVER_CODE;
        private string _COVER_NAME;
        private string _COVER_AMOUNT;
        private string _PRICE;
        private string _NET_PREMIUM;
        private string _LC_COVER_AMOUNT;
        private string _LC_NET_PREMIUM;

        [XmlElement]
        public string COVER_CODE { get { return _COVER_CODE; } set { _COVER_CODE = value; } }

        [XmlElement]
        public string COVER_NAME { get { return _COVER_NAME; } set { _COVER_NAME = value; } }

        [XmlElement]
        public string COVER_AMOUNT { get { return _COVER_AMOUNT; } set { _COVER_AMOUNT = value; } }

        [XmlElement]
        public string PRICE { get { return _PRICE; } set { _PRICE = value; } }

        [XmlElement]
        public string NET_PREMIUM { get { return _NET_PREMIUM; } set { _NET_PREMIUM = value; } }

        [XmlElement]
        public string LC_COVER_AMOUNT { get { return _LC_COVER_AMOUNT; } set { _LC_COVER_AMOUNT = value; } }

        [XmlElement]
        public string LC_NET_PREMIUM { get { return _LC_NET_PREMIUM; } set { _LC_NET_PREMIUM = value; } }

    }

    public class POLTAH
    {
        private string _DEBTOR_UNIT_TYPE;
        private string _DEBTOR;
        private string _PROMISE_ORDER_NO;
        private string _PROMISE_DATE;
        private string _PROMISE_AMOUNT;
        private string _PAYMENT_TOOL_TYPE;


        [XmlElement]
        public string DEBTOR_UNIT_TYPE { get { return _DEBTOR_UNIT_TYPE; } set { _DEBTOR_UNIT_TYPE = value; } }

        [XmlElement]
        public string DEBTOR { get { return _DEBTOR; } set { _DEBTOR = value; } }

        [XmlElement]
        public string PROMISE_ORDER_NO { get { return _PROMISE_ORDER_NO; } set { _PROMISE_ORDER_NO = value; } }

        [XmlElement]
        public string PROMISE_DATE { get { return _PROMISE_DATE; } set { _PROMISE_DATE = value; } }

        [XmlElement]
        public string PROMISE_AMOUNT { get { return _PROMISE_AMOUNT; } set { _PROMISE_AMOUNT = value; } }

        [XmlElement]
        public string PAYMENT_TOOL_TYPE { get { return _PAYMENT_TOOL_TYPE; } set { _PAYMENT_TOOL_TYPE = value; } }
    }
}
