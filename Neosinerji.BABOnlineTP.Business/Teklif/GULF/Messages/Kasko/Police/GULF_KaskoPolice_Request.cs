using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Police
{

    [XmlRootAttribute("REQUEST")]
    public class GULF_KaskoPolice_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }
        [XmlElement("POLICY")]
        public POLICY POLICY { get; set; }
    }  
    public class POLICY
    {
        [XmlElement("MASTER")]
        public MASTER MASTER { get; set; }

        [XmlElement("COLLECTION")]
        public COLLECTION COLLECTION { get; set; }
    }
    public class MASTER
    {
        public string FIRM_CODE;
        public string COMPANY_CODE;
        public string PRODUCT_NO;
        public string POLICY_NO;
        public string RENEWAL_NO;
        public string ENDORS_NO;
        public string CONFIRM_DATE;
    }
    public class COLLECTION
    {
        public DEBTOR[] DEBTORS { get; set; }
    }
    public class DEBTOR
    {
        public string DEBTOR_NO;
        public string NUMBER_OF_INSTALLMENT;
        public string ADVANCED_PAYMENT_AMOUNT;
        public string ADVANCE_PAYMENT_TOOL_TYPE;
        public string INSTALLMENTS_TOOL_TYPE;
        public BANK BANK;
        public CREDIT_CARD[] CREDIT_CARDS;
    }
    public class BANK
    {
        public string BANK_NO;
        public string CARD_CODE;
        public string BANK_SUB_BRANCH;
        public string ACCOUNT_NO;
        public string ACCOUNT_NAME;
        public string ACCOUNT_SURNAME;
        public string IBAN;
    }
    public class CREDIT_CARD
    {
        public string CARD_NO;
        public string NAME;
        public string SURNAME;
        public string VALID_DATE;
        public string CVV;
    }
}
