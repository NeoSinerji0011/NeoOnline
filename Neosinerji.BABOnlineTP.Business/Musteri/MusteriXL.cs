using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MusteriXL
    {
        private DataTable GetTable()
        {
            DataTable dt = new DataTable();

            DataColumn[] Coll = new DataColumn[59];

            Coll[0] = new DataColumn("Müşteri Türü *", typeof(string));
            Coll[1] = new DataColumn("Müşteri Ünvanı", typeof(string));
            Coll[2] = new DataColumn("Müşteri Kısa Adı", typeof(string));
            Coll[3] = new DataColumn("Acente Kodu *", typeof(string));
            Coll[4] = new DataColumn("MT Kullanıcı Kodu *", typeof(string));
            Coll[5] = new DataColumn("Müşteri Kategorisi *", typeof(string));
            Coll[6] = new DataColumn("Durumu *", typeof(string));
            Coll[7] = new DataColumn("Pasif Nedeni", typeof(string));
            Coll[8] = new DataColumn("Pasif Tarihi", typeof(string));
            Coll[9] = new DataColumn("Risk Limiti", typeof(string));
            Coll[10] = new DataColumn("Risk Limiti ... Ulaştığında Haber Ver", typeof(string));
            Coll[11] = new DataColumn("Adı", typeof(string));
            Coll[12] = new DataColumn("Soyadı", typeof(string));
            Coll[13] = new DataColumn("Cinsiyeti", typeof(string));
            Coll[14] = new DataColumn("Uyruğu", typeof(string));
            Coll[15] = new DataColumn("Ülkesi *", typeof(string));
            Coll[16] = new DataColumn("Doğum Tarihi", typeof(string));
            Coll[17] = new DataColumn("E-Posta Adresi *", typeof(string));
            Coll[18] = new DataColumn("E-Posta Adres Sahibi", typeof(string));
            Coll[19] = new DataColumn("Web Adresi", typeof(string));
            Coll[20] = new DataColumn("TC Kimlik No", typeof(string));
            Coll[21] = new DataColumn("Yabancı Kimlik No", typeof(string));
            Coll[22] = new DataColumn("Pasaport No", typeof(string));
            Coll[23] = new DataColumn("Vergi Dairesi", typeof(string));
            Coll[24] = new DataColumn("Vergi Numarası", typeof(string));
            Coll[25] = new DataColumn("Segment Kodu", typeof(string));
            Coll[26] = new DataColumn("RIM No", typeof(string));
            Coll[27] = new DataColumn("İlişkili RIM No", typeof(string));
            Coll[28] = new DataColumn("Baba Adı", typeof(string));
            Coll[29] = new DataColumn("Öğrenim Durumu", typeof(string));
            Coll[30] = new DataColumn("Medeni Durumu", typeof(string));
            Coll[31] = new DataColumn("Kan Grubu", typeof(string));
            Coll[32] = new DataColumn("Meslek", typeof(string));
            Coll[33] = new DataColumn("Çalıştığı Yer", typeof(string));
            Coll[34] = new DataColumn("Ünvan", typeof(string));
            Coll[35] = new DataColumn("Sosyal Güvenlik Kurumu", typeof(string));
            Coll[36] = new DataColumn("Sosyal Güvenlik No", typeof(string));
            Coll[37] = new DataColumn("Sicil No", typeof(string));
            Coll[38] = new DataColumn("Adres Tipi *", typeof(string));
            Coll[39] = new DataColumn("Ülke Adı *", typeof(string));
            Coll[40] = new DataColumn("Şehir Adı *", typeof(string));
            Coll[41] = new DataColumn("İlçe Adı *", typeof(string));
            Coll[42] = new DataColumn("Semt Adı *", typeof(string));
            Coll[43] = new DataColumn("Mahalle Adı *", typeof(string));
            Coll[44] = new DataColumn("Posta Kodu", typeof(string));
            Coll[45] = new DataColumn("Cadde Adı *", typeof(string));
            Coll[46] = new DataColumn("Sokak Adı *", typeof(string));
            Coll[47] = new DataColumn("Apartman Adı *", typeof(string));
            Coll[48] = new DataColumn("Site", typeof(string));
            Coll[49] = new DataColumn("Bina No *", typeof(string));
            Coll[50] = new DataColumn("Daire No *", typeof(string));
            Coll[51] = new DataColumn("Kat", typeof(string));
            Coll[52] = new DataColumn("Diğer Adres", typeof(string));
            Coll[53] = new DataColumn("Telefon Tipi *", typeof(string));
            Coll[54] = new DataColumn("U Alan Kodu *", typeof(string));
            Coll[55] = new DataColumn("Alan Kodu *", typeof(string));
            Coll[56] = new DataColumn("Telefon Numarası *", typeof(string));
            Coll[57] = new DataColumn("Dahili No", typeof(string));
            Coll[58] = new DataColumn("Telefon Sahibi", typeof(string));

            dt.Columns.AddRange(Coll);

            return dt;
        }

        //protected void btnUpload_Click(object sender, EventArgs e)
        //{
        //    if (fileUpload.HasFile)
        //    {
        //        IdeaLogger.BeginLog(PageName, WPContainer.GetUserId(), WPContainer.GetUserName(), "File Upload Started:" + fileUpload.FileName);
        //        int errorcount = 0;
        //        try
        //        {
        //            string extension = System.IO.Path.GetExtension(fileUpload.FileName);

        //            if (extension.EndsWith("xlsx") == false)
        //            {
        //                WPContainer.SetError(Resources.tekfen.CustomerImportFileError);
        //                return;
        //            }

        //            Guid FUID = System.Guid.NewGuid();
        //            string FUIDFileName = FUID.ToString("N") + extension;

        //            string fpath = Server.MapPath("ImportFiles") + "\\" + FUIDFileName;
        //            fpath = fpath.Replace("\\App", "");
        //            fileUpload.SaveAs(fpath);

        //            XL2007 xl = new XL2007(fpath, "Sheet1");

        //            if (xl.Open())
        //            {
        //                DataTable dt = GetTable();

        //                xl.Fill(dt);
        //                xl.Dispose();
        //                if (dt.Rows.Count > 0)
        //                {
        //                    dt.Columns.Add("Status", typeof(bool));
        //                    dt.Columns.Add("ImportStatus", typeof(string));
        //                    dt.Columns.Add("Müşteri Adı", typeof(string));

        //                    errorcount = CheckDataset(dt);

        //                    this.DTCusImport = dt;
        //                    grdLIST.DataSource = dt;
        //                    grdLIST.DataBind();

        //                    pnllList.Visible = true;

        //                    if (errorcount > 0)
        //                    {
        //                        btnTransfer.Visible = false;
        //                        lblTransferError.Visible = true;
        //                    }
        //                    else
        //                    {
        //                        btnTransfer.Visible = true;
        //                        lblTransferError.Visible = false;
        //                    }

        //                    IdeaLogger.EndLog(PageName, WPContainer.GetUserId(), WPContainer.GetUserName(), "File Uploaded:" + fileUpload.FileName);
        //                }
        //                else
        //                {
        //                    WPContainer.SetError(GetGlobalResourceObject("tekfen", "MsgErrImportFileHasNoContent").ToString());
        //                    IdeaLogger.EndLog(PageName, WPContainer.GetUserId(), WPContainer.GetUserName(), "File Upload Error:" + fileUpload.FileName);
        //                }
        //            }

        //            IdeaLogger.EndLog(PageName, WPContainer.GetUserId(), WPContainer.GetUserName(), "File Uploaded:" + fileUpload.FileName);

        //        }
        //        catch (Exception ex)
        //        {
        //            IdeaLogger.EndLog(PageName, WPContainer.GetUserId(), WPContainer.GetUserName(), "File Upload Error:" + fileUpload.FileName + " " + ex.Message);

        //            WPContainer.SetError(GetGlobalResourceObject("tekfen", "UploadDocumentError").ToString() + ex.Message);
        //        }
        //    }
        //}

        //private int CheckDataset(DataTable dt)
        //{
        //    int errorcount = 0;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        //customer sayfasındaki kontroller eklenecek.
        //        string CustomerCategoryName = IDConvert.ToString(row["Müşteri Türü *"]);
        //        string CustomerName = IDConvert.ToString(row["Müşteri Ünvanı"]);
        //        string FirstName = IDConvert.ToString(row["Adı"]);
        //        string AgentCode = IDConvert.ToString(row["Acente Kodu *"]);
        //        string LastName = IDConvert.ToString(row["Soyadı"]);
        //        string HSBCRIM = IDConvert.ToString(row["RIM No"]);
        //        string CustomerTypeName = IDConvert.ToString(row["Müşteri Kategorisi *"]);
        //        string TCIdentityNumber = IDConvert.ToString(row["TC Kimlik No"]);
        //        string TaxOffice = IDConvert.ToString(row["Vergi Dairesi"]);
        //        string TaxNumber = IDConvert.ToString(row["Vergi Numarası"]);
        //        string ForeignId = IDConvert.ToString(row["Yabancı Kimlik No"]);
        //        string PassportNo = IDConvert.ToString(row["Pasaport No"]);
        //        string CountryName = IDConvert.ToString(row["Ülke Adı *"]);
        //        string CountryCode = IDConvert.ToString(row["Ülkesi *"]);
        //        string CityName = IDConvert.ToString(row["Şehir Adı *"]);
        //        string DistrictName = IDConvert.ToString(row["İlçe Adı *"]);
        //        string AdressTypeName = IDConvert.ToString(row["Adres Tipi *"]);
        //        string PhoneTypeName = IDConvert.ToString(row["Telefon Tipi *"]);
        //        string PassiveReasonName = IDConvert.ToString(row["Pasif Nedeni"]);
        //        string Nationality = IDConvert.ToString(row["Uyruğu"]);
        //        string CusStatus = IDConvert.ToString(row["Durumu *"]);
        //        string Gender = IDConvert.ToString(row["Cinsiyeti"]);
        //        string EducationName = IDConvert.ToString(row["Öğrenim Durumu"]);
        //        string MaritalStatus = IDConvert.ToString(row["Medeni Durumu"]);
        //        string BloodGroup = IDConvert.ToString(row["Kan Grubu"]);
        //        string Business = IDConvert.ToString(row["Meslek"]);
        //        string DutyName = IDConvert.ToString(row["Ünvan"]);
        //        string SGK = IDConvert.ToString(row["Sosyal Güvenlik Kurumu"]);
        //        string MTUSerCode = IDConvert.ToString(row["MT Kullanıcı Kodu *"]);
        //        string EMailAddress = IDConvert.ToString(row["E-Posta Adresi *"]);
        //        string PostalCode = IDConvert.ToString(row["Posta Kodu"]);

        //        string TownName = IDConvert.ToString(row["Semt Adı *"]);
        //        string QuarterName = IDConvert.ToString(row["Mahalle Adı *"]);
        //        string HighWayName = IDConvert.ToString(row["Cadde Adı *"]);
        //        string StreetName = IDConvert.ToString(row["Sokak Adı *"]);

        //        string BuildingNo = IDConvert.ToString(row["Bina No *"]);
        //        string FlatNo = IDConvert.ToString(row["Daire No *"]);
        //        string FloorNo = IDConvert.ToString(row["Kat"]);
        //        string IntAreaCode = IDConvert.ToString(row["U Alan Kodu *"]);
        //        string AreaCode = IDConvert.ToString(row["Alan Kodu *"]);
        //        string Phone = IDConvert.ToString(row["Telefon Numarası *"]);

        //        int status = 0;
        //        string statusCode = String.Empty;
        //        string statusText = String.Empty;

        //        int CustomerCategory = CustomerCategoryName == "Gerçek" ? 1 : 2;
        //        int NationalityId = Nationality == "T.C." ? 1 : 2;

        //        #region Client validations
        //        int checkDB = 0;

        //        checkDB = CheckEMail(EMailAddress, out status, out statusCode, out statusText);

        //        if (checkDB == 0 && CustomerCategory == 1 && NationalityId == 1)
        //            checkDB = CheckTCIdentityNumber(TCIdentityNumber, out status, out statusCode, out statusText);

        //        if (checkDB == 0 && CustomerCategory == 1 && NationalityId == 2)
        //            checkDB = CheckForeignId(ForeignId, out status, out statusCode, out statusText);

        //        if (checkDB == 0 && CustomerCategory == 2)
        //            checkDB = CheckTaxOffice(TaxOffice, out status, out statusCode, out statusText);

        //        if (checkDB == 0 && CustomerCategory == 2)
        //            checkDB = CheckTaxNumber(TaxNumber, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckPostalCode(PostalCode, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckTownName(TownName, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckQuarterName(QuarterName, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckHighWayName(HighWayName, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckStreetName(StreetName, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckBuildingNo(BuildingNo, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckFlatNo(FlatNo, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckFloorNo(FloorNo, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckIntAreaCode(IntAreaCode, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckAreaCode(AreaCode, out status, out statusCode, out statusText);

        //        if (checkDB == 0)
        //            checkDB = CheckPhoneNum(Phone, out status, out statusCode, out statusText);
        //        #endregion

        //        if (checkDB == 0)
        //        {
        //            Data.CusCustomer.ImportCheck(CustomerCategoryName, CustomerName, AgentCode, CustomerTypeName, TaxNumber, HSBCRIM,
        //                FirstName, LastName, TCIdentityNumber, CountryCode, PassportNo, ForeignId, AdressTypeName,
        //                CountryName, CityName, DistrictName, PhoneTypeName, PassiveReasonName, Nationality, CusStatus, Gender,
        //                EducationName, MaritalStatus, BloodGroup, Business, DutyName, SGK, MTUSerCode,
        //                out status, out statusCode, out statusText);
        //        }
        //        if (status > 0) errorcount++;
        //        row["Status"] = status == 0 ? true : false;
        //        if (String.IsNullOrEmpty(statusCode) == false)
        //        {
        //            string msg = GetGlobalResourceObject("tekfen", statusCode).ToString();
        //            row["ImportStatus"] = msg;
        //        }
        //        else
        //        {
        //            row["ImportStatus"] = GetGlobalResourceObject("tekfen", "CustomerImportOk").ToString(); ;
        //        }

        //        if (!String.IsNullOrEmpty(CustomerName))
        //        {
        //            row["Müşteri Adı"] = CustomerName;
        //        }

        //        if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName))
        //        {
        //            row["Müşteri Adı"] = FirstName + " " + LastName;
        //        }
        //    }
        //    dt.AcceptChanges();

        //    return errorcount;
        //}

        //private int CheckEMail(string EMailAddress, out int status, out string statusCode, out string statusText)
        //{
        //    status = 0;
        //    statusCode = String.Empty;
        //    statusText = String.Empty;

        //    if (String.IsNullOrEmpty(EMailAddress))
        //    {
        //        status = 1;
        //        statusCode = "EMailRequired";
        //        statusText = EMailAddress;
        //        return status;
        //    }

        //    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Resources.tekfen.EMailRegex);
        //    bool ismatch = regex.IsMatch(EMailAddress);

        //    if (ismatch)
        //        return 0;

        //    status = 1;
        //    statusCode = "EMailFormatError";
        //    statusText = EMailAddress;

        //    return 1;
        //}

        private int CheckTCIdentityNumber(string TCIdentityNumber, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            bool all = HasAllDigits(TCIdentityNumber);

            if (all)
                return 0;

            status = 1;
            statusCode = "MsgErrTCIdentityDigit";
            statusText = TCIdentityNumber;

            return 1;
        }

        private int CheckForeignId(string ForeignId, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            bool all = HasAllDigits(ForeignId);

            if (all)
                return 0;

            status = 1;
            statusCode = "MsgErrForeignIdDigit";
            statusText = ForeignId;

            return 1;
        }

        private int CheckTaxOffice(string TaxOffice, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (TaxOffice.Length == 0)
            {
                status = 1;
                statusCode = "MsgErrTaxOfficeRequired";
                statusText = TaxOffice;
                return status;
            }

            return 0;
        }

        private int CheckTaxNumber(string TaxNumber, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (TaxNumber.Length == 0)
            {
                status = 1;
                statusCode = "MsgErrTaxNumberRequired";
                statusText = TaxNumber;
                return status;
            }

            bool all = HasAllDigits(TaxNumber);

            if (all)
                return 0;

            status = 1;
            statusCode = "MsgErrTaxNumberDigit";
            statusText = TaxNumber;

            return 1;
        }

        private int CheckPostalCode(string PostalCode, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (PostalCode.Length == 0)
                return 0;

            bool all = HasAllDigits(PostalCode);

            if (all)
                return 0;

            status = 1;
            statusCode = "MsgErrPostalCodeDigit";
            statusText = PostalCode;

            return 1;
        }

        private int CheckBuildingNo(string BuildingNo, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (BuildingNo.Length == 0)
            {

                status = 1;
                statusCode = "BuildingNoRequired";
                statusText = BuildingNo;

                return 1;
            }

            bool hasLetter = HasAnyLetter(BuildingNo);

            if (hasLetter)
            {
                status = 1;
                statusCode = "MsgErrBuildingNoDigit";
                statusText = BuildingNo;
            }

            return status;
        }

        private int CheckFlatNo(string FlatNo, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (FlatNo.Length == 0)
            {

                status = 1;
                statusCode = "FlatNoRequired";
                statusText = FlatNo;
                return status;
            }

            bool hasLetter = HasAnyLetter(FlatNo);

            if (hasLetter)
            {
                status = 1;
                statusCode = "MsgErrFlatNoDigit";
                statusText = FlatNo;
            }

            return status;
        }

        private int CheckFloorNo(string FloorNo, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            bool hasLetter = HasAnyLetter(FloorNo);

            if (hasLetter)
            {
                status = 1;
                statusCode = "MsgErrFloorNoDigit";
                statusText = FloorNo;
            }

            return status;
        }

        private int CheckIntAreaCode(string IntAreaCode, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (IntAreaCode.Length == 0)
            {

                status = 1;
                statusCode = "IntAreaCodeRequired";
                statusText = IntAreaCode;
                return status;
            }

            bool all = HasAllDigits(IntAreaCode);

            if (!all)
            {
                status = 1;
                statusCode = "MsgErrIntAreaCodeDigit";
                statusText = IntAreaCode;
            }

            return status;
        }

        private int CheckAreaCode(string AreaCode, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (AreaCode.Length == 0)
            {

                status = 1;
                statusCode = "AreaCodeRequired";
                statusText = AreaCode;
                return status;
            }

            bool all = HasAllDigits(AreaCode);

            if (!all)
            {
                status = 1;
                statusCode = "MsgErrAreaCodeDigit";
                statusText = AreaCode;
            }

            return status;
        }

        private int CheckPhoneNum(string PhoneNum, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (PhoneNum.Length == 0)
            {

                status = 1;
                statusCode = "PhoneNumRequired";
                statusText = PhoneNum;
                return status;
            }

            bool all = HasAllDigits(PhoneNum);

            if (!all)
            {
                status = 1;
                statusCode = "MsgErrPhoneNumDigit";
                statusText = PhoneNum;
            }

            return status;
        }

        private int CheckTownName(string TownName, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (TownName.Length == 0)
            {
                status = 1;
                statusCode = "TownNameRequired";
                statusText = TownName;
                return status;
            }

            return status;
        }

        private int CheckQuarterName(string QuarterName, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (QuarterName.Length == 0)
            {
                status = 1;
                statusCode = "QuarterNameRequired";
                statusText = QuarterName;
                return status;
            }

            return status;
        }

        private int CheckHighWayName(string HighWayName, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (HighWayName.Length == 0)
            {
                status = 1;
                statusCode = "HighWayNameRequired";
                statusText = HighWayName;
                return status;
            }

            return status;
        }

        private int CheckStreetName(string StreetName, out int status, out string statusCode, out string statusText)
        {
            status = 0;
            statusCode = String.Empty;
            statusText = String.Empty;

            if (StreetName.Length == 0)
            {
                status = 1;
                statusCode = "StreetNameRequired";
                statusText = StreetName;
                return status;
            }

            return status;
        }

        private bool HasAllDigits(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasAnyLetter(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]) == true)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
