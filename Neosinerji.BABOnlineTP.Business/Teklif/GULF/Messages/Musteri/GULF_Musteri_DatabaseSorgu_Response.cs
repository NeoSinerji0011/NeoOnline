using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages
{
    [Serializable, XmlRoot("RESPONSE")]  
    public class GULF_Musteri_DatabaseSorgu_Response
    {
        public string OPERATION_ID { get; set; }
        public string RESULT { get; set; }
        public string ERROR { get; set; }

        public DATA DATA { get; set; }
    }

    public class DATA
    {
        public ENTITY ENTITY { get; set; }
    }
    public class ENTITY
    {
        public MASTER MASTER { get; set; }
        public ADDRESS[] ADDRESSES { get; set; }
        public QUESTIONS QUESTIONS { get; set; }

    }
    public class MASTER
    {
        public string FIRM_CODE { get; set; }
        public string UNIT_TYPE { get; set; }
        public string UNIT_NO { get; set; }
        public string ADDRESSING { get; set; }
        public string NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string SURNAME { get; set; }
        public string MAIDEN_SURNAME { get; set; }
        public string FATHER_NAME { get; set; }
        public string MOTHER_NAME { get; set; }
        public string MOTHER_MAIDEN_SURNAME { get; set; }
        public string BIRTH_DATE { get; set; }
        public string BIRTH_PLACE_CITY_CODE { get; set; }
        public string BIRTH_PLACE { get; set; }
        public string WEIGHT { get; set; }
        public string HEIGHT { get; set; }
        public string GENDER { get; set; }
        public string BLOOD_TYPE { get; set; }
        public string GSM_COUNTRY_CODE1 { get; set; }
        public string GSM_CODE1 { get; set; }
        public string GSM_NUMBER1 { get; set; }
        public string GSM_COUNTRY_CODE2 { get; set; }
        public string GSM_CODE2 { get; set; }
        public string GSM_NUMBER2 { get; set; }
        public string TAX_OFFICE { get; set; }
        public string TAX_NO { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string OCCUPATION { get; set; }
        public string WORK_STATUS { get; set; }
        public string EDUC_STAT { get; set; }
        public string INCOME_LEVEL { get; set; }
        public string NATIONALITY { get; set; }
        public string CITIZENSHIP_NUMBER { get; set; }
        public string IDENTITY_TYPE { get; set; }
        public string IDENTITY_NO { get; set; }
        public string IDENTITY_OFFICE { get; set; }
        public string IB_SERIES { get; set; }
        public string IB_SERIAL_NO { get; set; }
        public string IB_REGISTERED_CITY { get; set; }
        public string IB_COUNTY { get; set; }
        public string IB_QUARTER { get; set; }
        public string IB_VOLUME_NO { get; set; }
        public string IB_FAMILY_ORDER_NO { get; set; }
        public string IB_ORDER_NO { get; set; }
        public string IB_PLACE_GIVEN { get; set; }
        public string IB_GIVING_REASON { get; set; }
        public string IB_REGISTER_NO { get; set; }
        public string IB_DATE_GIVEN { get; set; }
        public string DRIVING_LICENCE_NO { get; set; }
        public string DRIVING_LICENCE_CLASS { get; set; }
        public string FIRST_INSURANCE_DATE { get; set; }
        public string SOCIAL_SECURITY_NUMBER { get; set; }
        public string RESIDENT_IN_STATE { get; set; }
        public string ICQ_NO { get; set; }
        public string W_COMP_TYPE { get; set; }
        public string WORKING_COMPANY { get; set; }
        public string WORK_STARTING_DATE { get; set; }
        public string CHILDREN_COUNT { get; set; }
        public string CARS_COUNT { get; set; }
        public string HOUSES_COUNT { get; set; }
        public string REPRESENTATIVE_CODE { get; set; }
        public string REPRESENTATIVE_TYPE { get; set; }
        public string TITLE_CODE { get; set; }
        public string NET_SALARY { get; set; }
        public string TOTAL_INCOME { get; set; }
        public string TOTAL_EXPENSE { get; set; }
        public string FAMILY_NET_SALARY { get; set; }
        public string FAMILY_TOTAL_INCOME { get; set; }
        public string FAMILY_TOTAL_EXPENSE { get; set; }
        public string RESIDENCE_STARTING_YEAR { get; set; }
        public string RESIDENCE_STATUS { get; set; }
        public string FIRM_NAME { get; set; }
        public string COMPANY_TYPE { get; set; }
        public string ESTABLISH_DATE { get; set; }
        public string WORK_AREA { get; set; }
        public string INDUSTRY_OFFICE { get; set; }
        public string REGISTER_NO { get; set; }
        public string COMMERCE_OFFICE { get; set; }
        public string COMMERCE_REGISTER_NO { get; set; }
        public string CHAMBER_REGISTER_NO { get; set; }
        public string SECTOR { get; set; }
        public string CHANNEL_TYPE { get; set; }
        public string EMPLOYEE_COUNT { get; set; }
        public string CIRO { get; set; }
        public string CIRO_CUR_TYPE { get; set; }
        public string CONTRACTED_ORGANIZATION { get; set; }
        public string FINANCEMENT_CODE { get; set; }
        public string OFFICIAL_GAZETTE_DATE { get; set; }
        public string OFFICIAL_GAZETTE_NUMBER { get; set; }
        public string PERSONAL_COMMERCIAL { get; set; }
        public string SHORT_NAME { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string ADDRESS4 { get; set; }
        public string CONNECT_ADDRESS { get; set; }
        public string PHONE_COUNTRY_CODE1 { get; set; }
        public string PHONE_CODE1 { get; set; }
        public string PHONE_NUMBER1 { get; set; }
        public string PHONE_LINE1 { get; set; }
        public string PHONE_COUNTRY_CODE2 { get; set; }
        public string PHONE_CODE2 { get; set; }
        public string PHONE_NUMBER2 { get; set; }
        public string PHONE_LINE2 { get; set; }
        public string PHONE_COUNTRY_CODE3 { get; set; }
        public string PHONE_CODE3 { get; set; }
        public string PHONE_NUMBER3 { get; set; }
        public string PHONE_LINE3 { get; set; }
        public string PHONE_COUNTRY_CODE4 { get; set; }
        public string PHONE_CODE4 { get; set; }
        public string PHONE_NUMBER4 { get; set; }
        public string PHONE_LINE4 { get; set; }
        public string FAX_COUNTRY_CODE1 { get; set; }
        public string FAX_CODE1 { get; set; }
        public string FAX_NUMBER1 { get; set; }
        public string FAX_LINE1 { get; set; }
        public string FAX_COUNTRY_CODE2 { get; set; }
        public string FAX_CODE2 { get; set; }
        public string FAX_NUMBER2 { get; set; }
        public string FAX_LINE2 { get; set; }
        public string EMAIL_ADDRESS1 { get; set; }
        public string EMAIL_ADDRESS2 { get; set; }
        public string URL_ADDRESS1 { get; set; }
        public string URL_ADDRESS2 { get; set; }
        public string LAST_USER_NAME { get; set; }
        public string ENTRY_DATE { get; set; }
        public string ENTRY_TIME { get; set; }
        public string OPER_SFS_PRODUCT_NO { get; set; }
        public string OPER_NO { get; set; }
        public string CENTRAL_CLIENT_NO { get; set; }
        public string BLACK_LIST_CODE { get; set; }
        public string BLACK_LIST_ENTRY_DATE { get; set; }
        public string BLACK_LIST_ENTRY_REASON { get; set; }
        public string VIP_CODE { get; set; }
        public string CLIENT_STATUS { get; set; }
        public string STATUS { get; set; }
        public string TOWN_CODE { get; set; }
        public string CITY_CODE { get; set; }
        public string COUNTRY_CODE { get; set; }
        public string SPECIALCODE1 { get; set; }
        public string SPECIALCODE2 { get; set; }
        public string SPECIALCODE3 { get; set; }
        public string CLOSING_DATE { get; set; }
        public string CLOSING_REASON { get; set; }
        public string CHANNEL_NO { get; set; }
        public string EXTERNAL_CLIENT_NO { get; set; }
        public string SYSTEM_ENTRY_DATE { get; set; }
        public string AUTHORIZED { get; set; }
        public string RELATED_ENTITY { get; set; }
        public string CONTACT_TYPE { get; set; }
        public string CONTACT_SOURCE { get; set; }
        public string REFERENCE_TYPE { get; set; }
        public string REFERER { get; set; }
        public string UNIT_STATUS { get; set; }
        public string CONTROL_FLAG { get; set; }
        public string PROFILE { get; set; }
        public string DESIGN_NO { get; set; }
        public string SCORE { get; set; }
        public string UNIT_BANK_CODE { get; set; }
        public string UNIT_BANK_BRANCH_CODE { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string RESP_CLIENT_TYPE { get; set; }
        public string SYSTEM_ENTRY_USER { get; set; }
        public string IDENTITY_CHECKED { get; set; }
        public string CAN_BE_SENT_MAIL { get; set; }
        public string CHANNEL_CLIENT_NO { get; set; }
        public string SUB_CHANNEL_CLIENT_NO { get; set; }
        public string COMMON_CLIENT_CODE { get; set; }
        public string SECOND_RELATED_ENTITY { get; set; }
        public string IS_CALL_CENTER { get; set; }
        public string EXT_INT_CHECKED { get; set; }
        public string FOREIGN_CITIZENSHIP_NUMBER { get; set; }
        public string ADDRESS_CHECKED { get; set; }
    }
    public class QUESTIONS
    {
        public QUESTION[] QUESTION { get; set; }
    }

    public class QUESTION
    {
        public string QUESTION_CODE { get; set; }
        public string ANSWER { get; set; }
    }
    public class ADDRESS
    {
        public string WHICH_ADRESS;
        public string ADR_TYPE;
        public string ADR_DATA;
    }
}