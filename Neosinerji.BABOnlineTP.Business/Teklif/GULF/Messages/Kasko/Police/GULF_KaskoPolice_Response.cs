using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Police
{
    //[XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    //public class GULF_KaskoPolice_Response
    //{
    //    public string OPERATION_ID { get; set; }
    //    public string RESULT { get; set; }
    //    public string ERROR { get; set; }
    //    public DATA DATA { get; set; }
    //}

    //public class DATA
    //{
    //    public SFSProductionSchema SFSProductionSchema { get; set; }
    //}

    //public class SFSProductionSchema
    //{
    //    public annotation annotation { get; set; }

    //    [XmlElement("MASTER")]
    //    public MASTER MASTER { get; set; }
    //    public RISK_ADDRESS RISK_ADDRESS { get; set; }
    //    public QUESTION[] QUESTIONS { get; set; }
    //    public COVERAGE[] COVERAGES { get; set; }
    //    public COVER_DEDUCTION[] COVER_DEDUCTIONS { get; set; }

    //    public string EXPLANATION { get; set; }
    //    public string ADDITIONAL_INFOS { get; set; }

    //    public NOTE[] NOTES { get; set; }
    //    public INSURED[] INSUREDS { get; set; }
    //    public string CLAUSES { get; set; }
    //    public COLLECTION COLLECTION { get; set; }
    //}

    //public class annotation
    //{
    //    public string documentation { get; set; }
    //}

    //public class MASTER
    //{
    //    public string FIRM_CODE { get; set; }
    //    public string COMPANY_CODE { get; set; }
    //    public string PRODUCT_NO { get; set; }
    //    public string POLICY_NO { get; set; }
    //    public string RENEWAL_NO { get; set; }
    //    public string ENDORS_NO { get; set; }
    //    public string VERSION_NO { get; set; }
    //    public string SUB_ENDORS_NO { get; set; }
    //    public string POLICY_STATUS { get; set; }
    //    public string ENDORS_TYPE_CODE { get; set; }
    //    public string PROD_CANCEL { get; set; }
    //    public string CANCEL_REASON_CODE { get; set; }
    //    public string CHANNEL_FIRM_CODE { get; set; }
    //    public string CHANNEL_UNIT_TYPE { get; set; }
    //    public string CHANNEL { get; set; }
    //    public string CLIENT_FIRM_CODE { get; set; }
    //    public string CLIENT_UNIT_TYPE { get; set; }
    //    public string CLIENT_NO { get; set; }
    //    public string INSURED_FIRM_CODE { get; set; }
    //    public string INSURED_UNIT_TYPE { get; set; }
    //    public string INSURED_NO { get; set; }
    //    public string BEG_DATE { get; set; }
    //    public string END_DATE { get; set; }
    //    public string ISSUE_DATE { get; set; }
    //    public string CONFIRM_DATE { get; set; }
    //    public string PRINT_DATE { get; set; }
    //    public string CONFIRM_CANCEL_DATE { get; set; }
    //    public string POL_BEG_DATE { get; set; }
    //    public string POL_ISSUE_DATE { get; set; }
    //    public string PROPOSAL_DATE { get; set; }
    //    public string SYSTEM_DATE { get; set; }
    //    public string SYSTEM_TIME { get; set; }
    //    public string COMMR_COMM_MULT { get; set; }
    //    public string SPECIAL_COMM_RATE { get; set; }
    //    public string SPECIAL_COMM_AMOUNT { get; set; }
    //    public string LC_SPECIAL_COMM_AMOUNT { get; set; }
    //    public string NET_PREMIUM { get; set; }
    //    public string LC_NET_PREMIUM { get; set; }
    //    public string GROSS_PREMIUM { get; set; }
    //    public string LC_GROSS_PREMIUM { get; set; }
    //    public string CUR_TYPE { get; set; }
    //    public string CUR_ACCOUNT_TYPE { get; set; }
    //    public string EXCHANGE_RATE { get; set; }
    //    public string PAYMENT_TYPE { get; set; }
    //    public string REF_PRODUCT_NO { get; set; }
    //    public string REF_POLICY_NO { get; set; }
    //    public string REF_RENEWAL_NO { get; set; }
    //    public string REF_ENDORS_NO { get; set; }
    //    public string OLD_PRODUCT_NO { get; set; }
    //    public string OLD_POLICY_NO { get; set; }
    //    public string NEW_PRODUCT_NO { get; set; }
    //    public string NEW_POLICY_NO { get; set; }
    //    public string PRINT_TOTAL { get; set; }
    //    public string ISSUE_TYPE { get; set; }
    //    public string ISSUE_PLACE_FIRM_CODE { get; set; }
    //    public string ISSUE_PLACE_UNIT_TYPE { get; set; }
    //    public string ISSUE_CHANNEL { get; set; }
    //    public string ISSUE_TYPE_OF_POLICY { get; set; }
    //    public string AUTHORIZE_CODE { get; set; }
    //    public string HAS_CUR_ACCOUNT { get; set; }
    //    public string DEDUCTION_CHANGED { get; set; }
    //    public string PROP_TAKER_USER_NAME { get; set; }
    //    public string CHAN_COM_MULTIPLIER { get; set; }
    //    public string MUN_COUNTRY_CODE { get; set; }
    //    public string MUN_STATE_CODE { get; set; }
    //    public string MUN_CITY_CODE { get; set; }
    //    public string MUN_TOWN_CODE { get; set; }
    //    public string MUN_CODE { get; set; }
    //    public string PLATE { get; set; }
    //    public string IS_CERTIFICATE { get; set; }
    //    public string LAST_USER_NAME { get; set; }
    //    public string ENTRY_DATE { get; set; }
    //    public string ENTRY_TIME { get; set; }
    //    public string INSURED_NAME { get; set; }
    //    public string INSURED_MIDDLE_NAME { get; set; }
    //    public string INSURED_SURNAME { get; set; }
    //    public string EXTERNAL_POLICY_NO { get; set; }
    //    public string BANK_NO { get; set; }
    //    public string CATEGORY1 { get; set; }
    //    public string CATEGORY2 { get; set; }
    //    public string COMPANY_REF_ENDORS_NO { get; set; }
    //    public string COMPANY_REF_POLICY_NO { get; set; }
    //    public string OLD_COMPANY_CODE { get; set; }
    //    public string OPER_SFS_PRODUCT_NO { get; set; }
    //    public string OPER_NO { get; set; }
    //    public string NEW_COMPANY_CODE { get; set; }
    //    public string HAS_INST_CARD { get; set; }
    //    public string CARD_CODE { get; set; }
    //    public string NUMBER_OF_INSTALLMENT { get; set; }
    //    public string CANCELLATION_DATE { get; set; }
    //}
    //public class RISK_ADDRESS
    //{
    //    public string SPLITED { get; set; }
    //    public CONCANATED CONCANATED { get; set; }
    //}
    //public class CONCANATED
    //{
    //    public string ADR_DATA { get; set; }
    //}

    //public class QUESTION
    //{
    //    public string QUESTION_PLACE { get; set; }
    //    public string QUESTION_CODE { get; set; }
    //    public string VALID_DATE { get; set; }
    //    public string ANSWER { get; set; }
    //}

    //public class COVERAGE
    //{
    //    public string COVER_CODE { get; set; }
    //    public string COVER_VALID_DATE { get; set; }
    //    public string COVER_AMOUNT { get; set; }
    //    public string PRICE { get; set; }
    //    public string NET_PREMIUM { get; set; }
    //    public string EXEMP_RATE { get; set; }
    //    public string EXEMP_AMOUNT { get; set; }
    //    public string INF_RATE { get; set; }
    //    public string END_YEAR_AMOUNT { get; set; }
    //    public string MAX_EXC_RATE { get; set; }
    //    public string BEG_DATE { get; set; }
    //    public string END_DATE { get; set; }
    //    public string CUR_TYPE { get; set; }
    //    public string EXCHANGE_TYPE { get; set; }
    //    public string BANK_NO { get; set; }
    //    public string EXCHANGE_RATE { get; set; }
    //    public string LC_COVER_AMOUNT { get; set; }
    //    public string LC_NET_PREMIUM { get; set; }
    //    public string EXEMP_AMOUNT_CUR_TYPE { get; set; }
    //    public string LC_EXEMP_AMOUNT { get; set; }
    //    public string LC_END_YEAR_AMOUNT { get; set; }
    //    public string SECOND_SUB_BRN { get; set; }
    //    public string USUB_BRN { get; set; }
    //    public string MSUB_BRN { get; set; }
    //    public string HSUB_BRN { get; set; }
    //    public string AFFECTS_SUM_INSURED { get; set; }
    //    public string IS_SELECTED { get; set; }
    //}

    //public class COVER_DEDUCTION
    //{
    //    public string COVER_CODE { get; set; }
    //    public string DEDUCTION_TYPE_CODE { get; set; }
    //    public string DEDUCTION_CODE { get; set; }
    //    public string DEDUCTION_AMOUNT { get; set; }
    //    public string LC_DEDUCTION_AMOUNT { get; set; }

    //}

    //public class NOTE
    //{
    //    public string NOTE_ORDER_NO { get; set; }
    //    public string NOTE_ENTRY_DATE { get; set; }
    //    public string NOTES { get; set; }
    //}

    //public class COLLECTION
    //{
    //    public DEBTOR[] DEBTORS { get; set; }
    //}
    //public class DEBTOR
    //{
    //    public string DEBTOR_FIRM_CODE { get; set; }
    //    public string DEBTOR_UNIT_TYPE { get; set; }
    //    [XmlElement("DEBTOR")]
    //    public string debtor { get; set; }
    //    public string DEBTOR_SHARE { get; set; }
    //    public string TOTAL_PAYMENT { get; set; }
    //    public string ADVANCED_PAYMENT_RATE { get; set; }
    //    public string NUMBER_OF_INSTALLMENT { get; set; }
    //    public string ADVANCED_PAYMENT_AMOUNT { get; set; }
    //    public string ADVANCE_PAYMENT_TOOL_TYPE { get; set; }
    //    public string INSTALLMENTS_PAYMENT_TYPE { get; set; }
    //    public string INSTALLMENTS_TOOL_TYPE { get; set; }
    //    public string BANK_NO { get; set; }
    //    public string BANK_SUB_BRANCH { get; set; }
    //    public string ACCOUNT_NO { get; set; }
    //    public string ACCOUNT_NAME { get; set; }
    //    public string ACCOUNT_SURNAME { get; set; }
    //    public PROMISE[] PROMISES { get; set; }
    //    public string CREDIT_CARDS { get; set; }
    //}
    //public class PROMISE
    //{
    //    public string PROMISE_ORDER_NO { get; set; }
    //    public string PROMISE_DATE { get; set; }
    //    public string PROMISE_AMOUNT { get; set; }
    //    public string PAYMENT_TOOL_TYPE { get; set; }
    //    public string CREDIT_CARD_REF_INDEX { get; set; }
    //    public string IS_CLOSED { get; set; }
    //    public string BANK_NO { get; set; }
    //    public string BANK_SUB_BRANCH { get; set; }
    //    public string PAYMENT_TOOL_REF { get; set; }
    //    public string PAYMENT_TOOL_REF_DATE { get; set; }
    //    public string PAYMENT_TOOL_TYPE_REF { get; set; }
    //    public string REF_NAME { get; set; }
    //    public string REF_SURNAME { get; set; }
    //}

    //public class INSURED
    //{
    //    public string INSURED_FIRM_CODE { get; set; }
    //    public string INSURED_UNIT_TYPE { get; set; }
    //    public string INSURED_FIRM_NAME { get; set; }
    //    public string INSURED_NAME { get; set; }
    //    public string INSURED_SURNAME { get; set; }
    //    public string PERSONEL_COMMERCIAL { get; set; }
    //    public string INSURED_NO { get; set; }
    //    public string MAIN_INSURED { get; set; }
    //    public string INFO1 { get; set; }
    //    public string INFO2 { get; set; }
    //    public string INFO3 { get; set; }
    //    public string INFO4 { get; set; }
    //    public string INFO5 { get; set; }
    //    public string INFO6 { get; set; }

    //}
}
