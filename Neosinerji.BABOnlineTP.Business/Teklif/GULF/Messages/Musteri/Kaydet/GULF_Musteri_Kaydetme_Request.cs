using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Musteri.Kaydet
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_Musteri_Kaydetme_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }

        [XmlElement("ENTITY")]
        public ENTITYY ENTITY { get; set; }
    }

    //public class AUTH
    //{
    //    public string USER_NAME;
    //    public string PASSWORD;
    //    public string EXT_CLIENT_IP;
    //    public string CALLER_GUID;
    //}
    public class ENTITYY
    {
        [XmlElement("MASTER")]
        public MASTER MASTER { get; set; }

        [XmlElement("ADDRESSES")]
        public ADDRESS[] ADDRESSES { get; set; }
    }

    public class MASTER
    {
        public string CITIZENSHIP_NUMBER;
        public string TAX_NO;
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
        public string EMAIL_ADDRESS1;
        public string URL_ADDRESS1;
        public string WORK_AREA;
        public string SECTOR;
        public string EMPLOYEE_COUNT;
        public string BLACK_LIST_CODE;
        public string BLACK_LIST_ENTRY_REASON;
        public string BLACK_LIST_ENTRY_DATE;
        public string SCORE;
        public string CHANNEL_NO;
        public string FATHER_NAME;
        public string MOTHER_NAME;
        public string CONNECT_ADDRESS;
        public string ADDRESS1;
        public string MARITAL_STATUS;
        public string OCCUPATION;
        public string RESIDENT_IN_STATE;
    }

    //public class ADDRESSES
    //{
    //    [XmlElement("ADDRESS")]
    //    public ADDRESS[] ADDRESS { get; set; }
    //}
    public class ADDRESS
    {
        public string WHICH_ADRESS;
        public string ADR_TYPE;
        public string ADR_DATA;
    }
}
