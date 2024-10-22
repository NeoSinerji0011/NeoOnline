using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class WebServisCevaplar
    {
        /// <summary>
        /// HDI Referans No
        /// </summary>
        public const int HDI_Referans_No = 1;

        /// <summary>
        /// MAPFRE Poliçe No
        /// </summary>
        public const int MAPFRE_Teklif_Police_No = 2;

        /// <summary>
        /// MAPFRE Poliçe ID
        /// </summary>
        public const int MAPFRE_Teklif_Police_Id = 3;

        /// <summary>
        /// MAPFRE Zeyil ID
        /// </summary>
        public const int MAPFRE_Teklif_Zeyil_Id = 4;

        /// <summary>
        /// MAPFRE Poliçe Başlama Tarihi
        /// </summary>
        public const int MAPFRE_Teklif_PolBaslama_Tarih = 5;

        /// <summary>
        /// MAPFRE Poliçe Bitiş Tarihi
        /// </summary>
        public const int MAPFRE_Teklif_PolBitis_Tarih = 6;

        /// <summary>
        /// MAPFRE Poliçe No
        /// </summary>
        public const int MAPFRE_Police_No = 7;

        /// <summary>
        /// MAPFRE Poliçe ID
        /// </summary>
        public const int MAPFRE_Police_Id = 8;

        /// <summary>
        /// MAPFRE Zeyil ID
        /// </summary>
        public const int MAPFRE_Zeyil_Id = 9;

        /// <summary>
        /// MAPFRE Zeyil ID
        /// </summary>
        public const int ANADOLU_Teklif_Id = 10;

        /*
         *      DASKRefNo
                HDIPoliceNo 	Poliçe Onaylandığında döner
                HDIYenilemeNo	Poliçe Onaylandığında döner
                HDIZeyilNo  	Poliçe Onaylandığında döner
	
                DASKPoliceNo	Poliçe Onaylandığında döner
                DASKYenilemeNo	Poliçe Onaylandığında döner
                DASKZeyilNo	    Poliçe Onaylandığında döner

         */

        public const int HDIPoliceNo = 11;
        public const int HDIYenilemeNo = 12;
        public const int HDIZeyilNo = 13;
        public const int DASKPoliceNo = 14;
        public const int DASKYenilemeNo = 15;
        public const int DASKZeyilNo = 16;
        public const int DASKRefNoTeklif = 17;
        public const int DASKRefNoPolice = 20;

        //KONUT POLİCELEŞTİRME CEVAPLARI
        public const int HDIBIMReferans = 18;
        public const int HDISeriNo = 19;

        //Poliçe xml muhasebe api dönüş id
        public const int PoliceMuhasebeId = 21;

        //AEGON
        public const int SurumNo = 22;
        public const int TibbiTetkikSonucu = 23;
        public const int Uyari = 24;

        //RAY
        public const int RAY_Teklif_Police_No = 25;
        public const int RAY_Musteri_No = 34;

        //SOMPO JAPAN
        public const int SOMPOJAPAN_Teklif_Police_No = 26;
        public const int SOMPOJAPAN_Kasko_Session_No = 30;
        public const int SOMPOJAPAN_Trafik_Session_No = 31;

        //TÜRK NİPPON

        /// <summary>
        /// Türk Nippon Seyahat Poliçe numarası integer
        /// </summary>
        public const int TURKNIPPON_Teklif_Police_No = 201;

        /// <summary>
        /// Türk Nippon Seyahat Sigortalı No
        /// </summary>
        public const int TURKNIPPON_SigortaliUnit_No = 202;

        /// <summary>
        /// Türk Nippon Seyahat Müşteri No
        /// </summary>
        public const int TURKNIPPON_Client_No = 203;

        /// <summary>
        /// Türk Nippon Seyahat İşlem Takip Kodu
        /// </summary>
        public const int TURKNIPPON_IslemTakipKodu = 204;

        /// <summary>
        /// Türk Nippon Seyahat PersonalOrParticular Kodu, alabileceği değerler "B" veya "M".
        /// </summary>
        public const int TURKNIPPON_PersonalOrParticular = 205;

        /// <summary>
        ///  Türk Nippon Seyahat Yurtiçi veya Yurtdışı seçeneği boolean
        /// </summary>
        public const int TURKNIPPON_Seyahat_YurticiVeyaYurtdisi = 206;
        public const int TURKNIPPON_PoliceBaslangicTarihi = 207;
        public const int TURKNIPPON_PoliceBitisTarihi = 208;
        public const int TURKNIPPON_Seyahat_UlkeKodu = 209;
        public const int TURKNIPPON_Seyahat_KapsamKodu = 210;
        public const int TURKNIPPON_Seyahat_AlternatifKodu = 211;
        public const int TURKNIPPON_Seyahat_PaketKodu = 212;
        public const int TURKNIPPON_Seyahat_EuroKuru = 213;

        //EUREKO
        public const int EUREKO_ChangeSeqNum= 30;
        public const int EUREKO_SigortaEttirenMusteriNo = 31;
        public const int EUREKO_SigortaliMusteriNo = 32;
        public const int EUREKO_DainiMurteinMusteriNo = 33;
        public const int EUREKO_TariffLevelCode = 36;



        public const int EUREKO_PolicyGroupNum = 38;

        public const int EUREKO_PolicyNum = 39;
        public const int EUREKO_EndorsementNum = 40;
        public const int EUREKO_InternalEndorsementNum = 41;
        public const int EUREKO_RenewalNum = 42;
        public const int GROUPAMA_TeklifVersiyonNo = 43;

        public const int Gulf_Teklif_Police_No = 44;
        public const int Gulf_Musteri_No = 45;
        public const int Gulf_IslemTakipKodu = 46;
        public const int TURKNIPPON_DaskUyariMesaji = 47;
        public const int TeklifUyariMesaji = 48;
        public const int TeklifBilgiMesaji = 49;
        public const int KoruTeklifHashKodu = 50;
        public const int Koru3DParatikaToken = 51;
        public const int KoruTokenGuidId = 52;
        public const int Koru3DParatikaSonOdemeDurumu = 53;

        public const int UNICO_AuthenticationKey = 54;
        public const int UNICO_oncekipolno = 55;
        public const int UNICO_oncekisigortasirketi = 56;
        public const int UNICO_oncekiacenteno = 57;
        public const int UNICO_oncekiyenilemeno = 58;
        public const int UNICO_licencePlate = 59;
        public const int UNICO_registrySerial = 60;
        public const int UNICO_registryNo = 61;
        public const int UNICO_queryInput = 62;
        public const int UNICO_queryInputType = 63;
        public const int UNICO_startDate = 64;
        public const int UNICO_endDate = 65;
        public const int UNICO_appSecurityKey = 66;

        //AK
        public const int AK_Teklif_Police_No = 68;
        public const int AK_Kasko_Session_No = 69;
        public const int AK_Trafik_Session_No = 70;
    }
}
