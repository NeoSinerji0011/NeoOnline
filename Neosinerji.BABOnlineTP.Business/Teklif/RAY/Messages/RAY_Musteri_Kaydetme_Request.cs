using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("REQUEST", Namespace = "", IsNullable = false)]
    public class RAY_Musteri_Kaydetme_Request
    {
        public UNSMAS UNSMAS;

        [XmlElement("UNSADR")]
        public UNSADRR[] UNSADR { get; set; }

        [XmlElement("USRCVP")]
        public USRCVP[] USRCVP { get; set; }

        public RAY_Musteri_Kaydetme_Request()
        { }
    }

    public class UNSMAS
    {
        public string PROCESS_ID;
        public string USER_NAME;
        public string PASSWORD;
        public string CHANNEL;
        public string CLIENT_IP;
        public string CITIZENSHIP_NUMBER;
        public string TAX_NUMBER;
        public string TAX_OFFICE;
        public string FOREIGN_CITIZENSHIP_NUMBER;
        public string IDENTITY_NO;
        public string COUNTRY_CODE;
        public string NATIONALITY;
        public string PERSONAL_COMMERCIAL;
        public string FIRM_NAME;
        public string NAME;
        public string SURNAME;
        public string GENDER;
        public string BIRTH_DATE;
        public string BIRTH_PLACE;
        public string GSM_COUNTRY_CODE1;
        public string GSM_CODE1;
        public string GSM_NUMBER1;
        public string PHONE_COUNTRY_CODE1;
        public string PHONE_CODE1;
        public string PHONE_NUMBER1;
        public string PHONE_LINE1;
        public string FAX_COUNTRY_CODE1;
        public string FAX_CODE1;
        public string FAX_NUMBER1;
        public string FAX_LINE1;
        public string EMAIL;
        public string URL_ADDRESS1;
        public string WORK_AREA;
        public string SECTOR;
        public string FATHER_NAME;
        public string MOTHER_NAME;
        public string CONNECT_ADDRESS;
        public string MARITAL_STATUS;
        public string OCCUPATION;
        public string RESIDENT_IN_STATE;

        public string EMPLOYEE_COUNT;
        public string BLACK_LIST_CODE;
        public string BLACK_LIST_ENTRY_REASON;
        public string BLACK_LIST_ENRTY_DATE;
        public string SCORE;

    }

    public class UNSADRR
    {
        public string WHICH_ADRESS;
        public string ADR_TYPE;
        public string ADR_DATA;
    }

    public class USRCVP
    {
        public string QUESTION_CODE;
        public string ANSWER;
    }
}
