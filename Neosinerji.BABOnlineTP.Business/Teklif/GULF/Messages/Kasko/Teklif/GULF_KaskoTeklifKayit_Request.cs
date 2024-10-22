using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_KaskoTeklifKayit_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }

        [XmlElement("POLICY")]
        public POLICY POLICY { get; set; }
    }
    public class AUTH
    {
        public string USER_NAME;
        public string PASSWORD;
        public string EXT_CLIENT_IP;
        public string CALLER_GUID;
    }
    public class POLICY
    {
        public MASTER MASTER { get; set; }
        public string RISK_ADDRESS { get; set; }
        public QUESTION[] QUESTIONS { get; set; }
        public string COVERAGES { get; set; }
        public string COVER_DEDUCTIONS { get; set; }
        public string EXPLANATION { get; set; }
        public string NOTES { get; set; }
        public INSURED[] INSUREDS { get; set; }
        public COLLECTION COLLECTION { get; set; }
    }
    public class MASTER
    {
        public string PRODUCT_NO { get; set; }
        public string CLIENT_NO { get; set; }
        public string CHANNEL { get; set; }
        public string BEG_DATE { get; set; }
        public string END_DATE { get; set; }
        public string ISSUE_DATE { get; set; }
        public string CONFIRM_DATE { get; set; }
        public string INSURED_NAME { get; set; }
        public string INSURED_MIDDLE_NAME { get; set; }
        public string INSURED_SURNAME { get; set; }
        public string PLATE { get; set; }
        public string CATEGORY1 { get; set; }
        public string EXCHANGE_RATE { get; set; }
        public string EXTERNAL_POLICY_NO { get; set; }
        public string HAS_INST_CARD { get; set; }
    }

    public class COLLECTION
    {
        public DEBTOR[] DEBTORS { get; set; }
    }
    public class QUESTION
    {
        public string QUESTION_CODE { get; set; }
        public string ANSWER { get; set; }
    }
    public class INSURED
    {
        public string INSURED_NO { get; set; }
        public string MAIN_INSURED { get; set; }
        public string INFO1 { get; set; }
        public string INFO2 { get; set; }
        public string INFO3 { get; set; }
        public string INFO4 { get; set; }
        public string INFO5 { get; set; }
        public string INFO6 { get; set; }
    }
    
    public class DEBTOR
    {
        public string ADVANCE_PAYMENT_TOOL_TYPE { get; set; }
        public string INSTALLMENTS_PAYMENT_TYPE { get; set; }
        public string INSTALLMENTS_TOOL_TYPE { get; set; }
    }
}
