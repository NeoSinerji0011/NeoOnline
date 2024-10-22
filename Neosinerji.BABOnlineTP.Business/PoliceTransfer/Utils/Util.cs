using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using System.IO;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public static class Util
    {

        //Date Formats
        public const string DateFormat0 = "ddMMyyyy";
        public const string DateFormat1 = "dd/MM/yyyy";
        public const string DateFormat2 = "yyyyMMdd";
        public const string DateFormat3 = "yyyy-MM-ddT00:00:00";
        public const string DateFormat4 = "dd.MM.yyyy 00:00:00";
        public const string DateFormat5 = "yyyy";
        public const string DateFormat6 = "yyyy-MM-dd";



        private static NumberFormatInfo _numberFormat;
        [XmlIgnore]
        private static NumberFormatInfo NumberFormat
        {
            get
            {
                _numberFormat = null;
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ".";
                    _numberFormat.NumberDecimalDigits = 2;
                }

                return _numberFormat;
            }
        }
        [XmlIgnore]
        private static NumberFormatInfo NumberFormat2
        {
            get
            {
                _numberFormat = null;
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ",";
                    _numberFormat.NumberDecimalDigits = 2;
                }

                return _numberFormat;
            }
        }
        public static decimal ToDecimal(string value)
        {
            value = value.Trim();
            if (String.IsNullOrEmpty(value))
                return decimal.Zero;
            decimal result = 0;
            if (value.Contains(","))
            {
                try
                {
                    result = decimal.Parse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormat2);
                }
                catch (Exception)
                {
                    result = decimal.Parse(value.Replace(",", string.Empty));

                }

            }
            else result = decimal.Parse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormat);

            return result;
        }

        public static int toInt(string prm)
        {
            int intValue;
            if (int.TryParse(prm, out intValue))
            {
                return intValue;
            }
            else
            {
                return 0;
            }
        }

        public static DateTime? toDate(string prm)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            if (!String.IsNullOrEmpty(prm))
            {
                DateTime date = Convert.ToDateTime(prm, culture);
                return date;
            }
            return null;
        }

        public static DateTime? toDate(string prm, string dateFormat)
        {

            DateTime dateTime;

            if (DateTime.TryParseExact(prm, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            return null;
        }



        public static string[] generateSheetColums(ISheet sht, string[] columnNames)
        {
            for (int indx = 0; indx <= sht.LastRowNum; indx++)
            {
                IRow row = sht.GetRow(indx);
                if (row == null) continue;
                if (row.FirstCellNum != 0) continue;
                //check column sequenc is correct !!!
                if (row.GetCell(0).StringCellValue == columnNames[0])
                {
                    if (checkColumnSequence(row, columnNames) == true)
                    {
                        return columnNames;
                    }
                }
            }

            return columnNames;
        }


        public static int checkSheetCorrect(ISheet sht, string[] columnNames)
        {
            for (int indx = 0; indx <= sht.LastRowNum; indx++)
            {
                IRow row = sht.GetRow(indx);
                if (row == null) continue;
                if (row.FirstCellNum != 0) continue;
                var data="";
                try
                {

             
                //check column sequenc is correct !!!
                  data = row.GetCell(0).StringCellValue;
                }
                catch (Exception ex)
                {

                   
                }
                if (data == columnNames[0])
                {
                    if (checkColumnSequence(row, columnNames) == true)
                    {
                        return indx + 1;
                    }
                }
            }

            return -1;
        }

        private static bool checkColumnSequence(IRow row, string[] columnNames)
        {
            bool tf = false;

            for (int indx = 0; indx < columnNames.Length; indx++)
            {
                string colName = row.GetCell(indx).StringCellValue;
                if (colName != "Dask Poliçe Numarası")
                {
                    if (colName != "")
                    {
                        if (colName.Equals(columnNames[indx]))
                            tf = true;
                        else
                        {
                            tf = false;
                            break;
                        }
                    }
                }
            }
            return tf;
        }

        public static PoliceGenelBrans PoliceBransAdiEslestir(List<BransUrun> SigortaSirketiBransList, List<Bran> branslar, string tumUrunAdi, string tumUrunKodu)
        {

            PoliceGenelBrans policeGenelBrans = new PoliceGenelBrans();

            bool kontrol = false;


            if (SigortaSirketiBransList != null)
            {

                if (tumUrunKodu != null)
                {

                    kontrol = true;
                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunKodu.Trim() == tumUrunKodu
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();
                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;

                        return policeGenelBrans;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
                if (kontrol == false && tumUrunAdi != null)
                {
                    kontrol = true;

                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunAdi == tumUrunAdi
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();


                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
            }

            if (kontrol == false)
            {
                // bransUrun tablosunda sigorta sirketi yoksa
                policeGenelBrans.TUMUrunAdi = tumUrunAdi;
                policeGenelBrans.TUMUrunKodu = tumUrunKodu;
                policeGenelBrans.BransAdi = BransListeCeviri.TanimsizBransAciklama;
                policeGenelBrans.BransKodu = BransListeCeviri.TanimsizBransKodu;
                policeGenelBrans.TUMBransAdi = "?";
                policeGenelBrans.TUMBransKodu = "-9999";
            }

            return policeGenelBrans;


        }

        public static PoliceGenelBrans PoliceBransAdiEslestirAnadolu(List<BransUrun> SigortaSirketiBransList, List<Bran> branslar, string tumUrunAdi)
        {
            PoliceGenelBrans policeGenelBrans = new PoliceGenelBrans();

            bool kontrol = false;


            if (SigortaSirketiBransList != null)
            {

                if (tumUrunAdi != null)
                {

                    kontrol = true;
                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunAdi.Trim() == tumUrunAdi
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();
                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;

                        return policeGenelBrans;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
                if (kontrol == false && tumUrunAdi != null)
                {
                    kontrol = true;

                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunAdi == tumUrunAdi
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();


                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
            }

            if (kontrol == false)
            {
                // bransUrun tablosunda sigorta sirketi yoksa
                policeGenelBrans.TUMUrunAdi = tumUrunAdi;
                policeGenelBrans.TUMUrunKodu = tumUrunAdi;
                policeGenelBrans.BransAdi = BransListeCeviri.TanimsizBransAciklama;
                policeGenelBrans.BransKodu = BransListeCeviri.TanimsizBransKodu;
                policeGenelBrans.TUMBransAdi = "?";
                policeGenelBrans.TUMBransKodu = "-9999";
            }

            return policeGenelBrans;


        }

        public static PoliceGenelBrans PoliceBransAdiEslestirDoga(List<BransUrun> SigortaSirketiBransList, List<Bran> branslar, string tumUrunAdi, string tumUrunKodu)
        {

            PoliceGenelBrans policeGenelBrans = new PoliceGenelBrans();

            bool kontrol = false;


            if (SigortaSirketiBransList != null)
            {

                if (tumUrunAdi != null)
                {

                    kontrol = true;
                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunAdi.Trim() == tumUrunAdi
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();
                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;

                        return policeGenelBrans;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
                if (kontrol == false && tumUrunKodu != null)
                {
                    kontrol = true;

                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunKodu == tumUrunKodu
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();


                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
            }

            if (kontrol == false)
            {
                // bransUrun tablosunda sigorta sirketi yoksa
                policeGenelBrans.TUMUrunAdi = tumUrunAdi;
                policeGenelBrans.TUMUrunKodu = tumUrunKodu;
                policeGenelBrans.BransAdi = BransListeCeviri.TanimsizBransAciklama;
                policeGenelBrans.BransKodu = BransListeCeviri.TanimsizBransKodu;
                policeGenelBrans.TUMBransAdi = "?";
                policeGenelBrans.TUMBransKodu = "-9999";
            }

            return policeGenelBrans;


        }

        // genc icin
        public static PoliceGenelBrans PoliceBransAdiEslestirWithBirlikKod(string birlikKodu, List<Bran> branslar, string tumUrunAdi, string tumUrunKodu)
        {

            PoliceGenelBrans policeGenelBrans = new PoliceGenelBrans();

            bool kontrol = false;

            IBransUrunService _BransUrunService = DependencyResolver.Current.GetService<IBransUrunService>();

            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(birlikKodu);


            if (SigortaSirketiBransList != null)
            {

                if (tumUrunKodu != null)
                {

                    kontrol = true;
                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunKodu.Trim() == tumUrunKodu
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();
                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;

                        return policeGenelBrans;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
                if (kontrol == false && tumUrunAdi != null)
                {
                    kontrol = true;

                    var brans = (from k in SigortaSirketiBransList
                                 where k.SigortaSirketUrunAdi == tumUrunAdi
                                 join e in branslar on k.BransKodu equals e.BransKodu
                                 select new { k, e.BransAdi, e.BransKodu }).FirstOrDefault();


                    if (brans != null)
                    {
                        policeGenelBrans.BransAdi = brans.BransAdi;
                        policeGenelBrans.BransKodu = brans.BransKodu;
                        policeGenelBrans.TUMUrunAdi = brans.k.SigortaSirketUrunAdi;
                        policeGenelBrans.TUMUrunKodu = brans.k.SigortaSirketUrunKodu;
                        policeGenelBrans.TUMBransAdi = brans.k.SigortaSirketBransAdi;
                        policeGenelBrans.TUMBransKodu = brans.k.SigortaSirketBransKodu;
                    }
                    else
                    {
                        kontrol = false;
                    }
                }
            }

            if (kontrol == false)
            {
                // bransUrun tablosunda sigorta sirketi yoksa
                policeGenelBrans.TUMUrunAdi = tumUrunAdi;
                policeGenelBrans.TUMUrunKodu = tumUrunKodu;
                policeGenelBrans.BransAdi = BransListeCeviri.TanimsizBransAciklama;
                policeGenelBrans.BransKodu = BransListeCeviri.TanimsizBransKodu;
                policeGenelBrans.TUMBransAdi = "?";
                policeGenelBrans.TUMBransKodu = "-9999";
            }

            return policeGenelBrans;


        }


        public static string ConvertToTUMBirlikKod(string sirkod)
        {
            string result = String.Empty;
            sirkod = sirkod.ToLower().Replace('ı', 'i').Trim();

            switch (sirkod)
            {
                case "eureko": result = SigortaSirketiBirlikKodlari.EUREKOSIGORTA; break;
                case "ak": result = SigortaSirketiBirlikKodlari.AKSIGORTA; break;
                case "gulf": result = SigortaSirketiBirlikKodlari.GULFSIGORTA; break;
                case "anadolu": result = SigortaSirketiBirlikKodlari.ANADOLUSIGORTA; break;
                case "ankara": result = SigortaSirketiBirlikKodlari.ANKARASIGORTA; break;
                case "gru": result = SigortaSirketiBirlikKodlari.BASAKGROUPAMASIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.BATISIGORTA; break;
                case "halk": result = SigortaSirketiBirlikKodlari.HALKSIGORTA; break;
                case "turkiye": result = SigortaSirketiBirlikKodlari.TURKIYESIGORTA; break;
                case "unico": result = SigortaSirketiBirlikKodlari.AVIVASIGORTA; break;
                case "zurich": result = SigortaSirketiBirlikKodlari.ZURICHSIGORTA; break;
                case "turkland": result = SigortaSirketiBirlikKodlari.TURKLANDSIGORTA; break;
                case "ege": result = SigortaSirketiBirlikKodlari.EGESIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.AKDENIZSIGORTA; break;
                case "generali": result = SigortaSirketiBirlikKodlari.GENERALISIGORTA; break;
                case "gunes": result = SigortaSirketiBirlikKodlari.GUNESSIGORTA; break;
                //  case "GRU": result = SigortaSirketiBirlikKodlari.GUVENSIGORTA; break;
                case "yks": result = SigortaSirketiBirlikKodlari.YAPIKREDISIGORTA; break;
                //  case "GRU": result = SigortaSirketiBirlikKodlari.HURSIGORTA; break;
                case "ergo": result = SigortaSirketiBirlikKodlari.ERGOISVICRESIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.MAGDEBURGERSIGORTA; break;
                //  case "GRU": result = SigortaSirketiBirlikKodlari.MERKEZSIGORTA; break;
                case "axa": result = SigortaSirketiBirlikKodlari.AXASIGORTA; break;
                case "ray": result = SigortaSirketiBirlikKodlari.RAYSIGORTA; break;
                case "allianz": result = SigortaSirketiBirlikKodlari.ALLIANZSIGORTA; break;
                case "liberty": result = SigortaSirketiBirlikKodlari.LIBERTYSIGORTA; break;
                case "sbn": result = SigortaSirketiBirlikKodlari.SBNSIGORTA; break;
                case "mapfre": result = SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA; break;
                case "nippon": result = SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA; break;
                //  case "GRU": result = SigortaSirketiBirlikKodlari.UNIVERSALSIGORTA; break;
                case "hdi": result = SigortaSirketiBirlikKodlari.HDISIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.EGSSIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.EUROSIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.ISIKSIGORTA; break;
                // case "GRU": result = SigortaSirketiBirlikKodlari.KAPITALSIGORTA; break;
                case "fiba": result = SigortaSirketiBirlikKodlari.FIBAEMEKLILIK; break;
                case "sompo": result = SigortaSirketiBirlikKodlari.SOMPOJAPANSIGORTA; break;
                case "magde": result = SigortaSirketiBirlikKodlari.MAGDEBURGERSIGORTA; break;
                case "dubai": result = SigortaSirketiBirlikKodlari.DUBAIGROUPSIGORTA; break;
                case "bereket": result = SigortaSirketiBirlikKodlari.BEREKET; break;
                case "acibadem": result = SigortaSirketiBirlikKodlari.ACIBADEM; break;
                case "koru": result = SigortaSirketiBirlikKodlari.KORUSIGORTA; break;
                case "orient": result = SigortaSirketiBirlikKodlari.ORIENTSIGORTA; break;
                case "ethica": result = SigortaSirketiBirlikKodlari.ETHICA; break;
                case "neova": result = SigortaSirketiBirlikKodlari.NEOVASIGORTA; break;
                case "allianzhayat": result = SigortaSirketiBirlikKodlari.ALLIANZHAYATVEEMEKLILIK; break;
                case "doga": result = SigortaSirketiBirlikKodlari.SSDOGASİGORTAKOOPERATİF; break;
                case "quick": result = SigortaSirketiBirlikKodlari.QUICKSIGORTA; break;
                case "gri": result = SigortaSirketiBirlikKodlari.GRISIGORTA; break;
                case "prive": result = SigortaSirketiBirlikKodlari.PRIVESIGORTA; break;
                case "aveonglobal": result = SigortaSirketiBirlikKodlari.AVEONGLOBAL; break;
                case "hepiyi": result = SigortaSirketiBirlikKodlari.HEPIYISIGORTA; break;
                case "ana": result = SigortaSirketiBirlikKodlari.ANASIGORTA; break;
            }

            return result;
        }

        public static int ConvertToTVMGencKodu(string sortahkodu)
        {
            int result = 0;

            switch (sortahkodu)
            {
                case "01": result = 118001; break;
                //   case "": result = 1181001; break;
                case "89": result = 1182001; break;
                case "05": result = 1182002; break;
                case "92": result = 1182003; break;
                case "50": result = 1182004; break;
                case "13": result = 1182005; break;
                case "198": result = 1182006; break;
                case "75": result = 1182007; break;
                case "10": result = 1182008; break;
                //   case "": result = 1182009; break;
                //  case "": result = 1182010; break;
                case "231": result = 1182011; break;
                case "16": result = 1182012; break;
                case "213": result = 1182013; break;
                case "153": result = 1182014; break;
                case "267": result = 1182015; break;
                case "269": result = 1182016; break;
                case "274": result = 1182017; break;
                case "70": result = 1182018; break;
                case "257": result = 1182019; break;
                case "282": result = 1182020; break;
                case "195": result = 1182021; break;
                case "40": result = 1182022; break;
                case "104": result = 1182023; break;
                case "201": result = 1182024; break;
                case "09": result = 1182025; break;
                case "160": result = 1182026; break;
                case "73": result = 1182027; break;
                case "178": result = 1182028; break;
                case "1001": result = 1182029; break;
                case "154": result = 1182030; break;
                case "278": result = 1182031; break;
                //  case "": result = 1182032; break;
                case "233": result = 1182033; break;
                case "111": result = 1182034; break;
                case "268": result = 1182035; break;
                case "06": result = 1182036; break;
                case "232": result = 1182037; break;
                case "280": result = 1182038; break;
                case "118": result = 1182039; break;
                case "27": result = 1182040; break;
                case "53": result = 1182041; break;
                // case "": result = 1182042; break;
                case "58": result = 1182044; break;
                case "81": result = 1182046; break;
                case "202": result = 1182047; break;
                case "28": result = 1182048; break;
                case "39": result = 1182051; break;
                case "199": result = 1182053; break;
                case "29": result = 1182054; break;
                case "30": result = 1182055; break;
                case "174": result = 1182056; break;
                case "240": result = 1182057; break;
                case "31": result = 1182058; break;
                case "55": result = 1182060; break;
                case "164": result = 1182061; break;
                //  case "": result = 1182062; break;
                case "219": result = 1182063; break;
                case "113": result = 1182064; break;
                case "08": result = 1182065; break;
                case "07": result = 1182066; break;
                case "208": result = 1182067; break;
                case "33": result = 1182068; break;
                //   case "": result = 1182069; break;
                case "227": result = 1182070; break;
                case "1000": result = 1183001; break;
                case "42": result = 1183002; break;
                case "15": result = 1183003; break;
                case "224": result = 1183004; break;
                case "251": result = 1183005; break;
                case "256": result = 1183006; break;
                //  case "": result = 1183007; break;
                //  case "": result = 1183008; break;
                case "1002": result = 1183009; break;
                //  case "": result = 1183010; break;
                //  case "": result = 1183011; break;
                case "168": result = 1183012; break;
                case "04": result = 1183014; break;
                case "116": result = 1183015; break;
                case "126": result = 1183016; break;
                case "196": result = 1183017; break;
                case "20": result = 1183018; break;
                case "173": result = 1183019; break;
                case "272": result = 1183020; break;
                case "123": result = 1183021; break;
                case "254": result = 1183022; break;
                case "125": result = 1183023; break;
                //   case "": result = 1183024; break;
                //  case "": result = 1183025; break;
                case "284": result = 1183026; break;
                case "286": result = 1183027; break;
                case "250": result = 1183028; break;
                case "60": result = 1183029; break;
                case "167": result = 1183030; break;
                //  case "": result = 1183031; break;
                case "98": result = 1183032; break;
                case "253": result = 1183033; break;
                case "93": result = 1183034; break;
                //  case "": result = 1183035; break;
                case "239": result = 1184001; break;
                case "221": result = 1184002; break;
                case "217": result = 1184003; break;
                case "230": result = 1184004; break;
                case "273": result = 1184005; break;
                case "71": result = 1185001; break;
                case "97": result = 1186001; break;
                case "148": result = 1186002; break;
                case "170": result = 1186003; break;
                //    case "": result = 1186004; break;
                case "285": result = 1186005; break;
                case "155": result = 1186006; break;
                case "209": result = 1186007; break;
                case "263": result = 1186008; break;
                case "242": result = 1186009; break;
                case "152": result = 1186010; break;
                case "211": result = 1186011; break;
                case "266": result = 1186012; break;
                case "288": result = 1186013; break;
                case "214": result = 1186014; break;
                case "169": result = 1186015; break;
                case "87": result = 1186016; break;
                case "185": result = 1186017; break;
                case "136": result = 1186018; break;
                case "277": result = 1186019; break;
                case "180": result = 1186020; break;
                case "235": result = 1186021; break;
                case "57": result = 1186022; break;
                    //  case "": result = 1186023; break;
            }

            return result;
        }
        public static List<NeoOnline_TahsilatKapatma> tahsilatDosayasiOkur(string path)
        {
            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            NeoOnline_TahsilatKapatma neoOnline_TahsilatKapatma;

            FileStream excelFile = new FileStream(path, FileMode.Open, FileAccess.Read);
            HSSFWorkbook wb1 = new HSSFWorkbook(excelFile);
            ISheet sheet1 = wb1.GetSheet("Sheet1");
            int startRow = sheet1.FirstRowNum;
            for (int indx = startRow + 2; indx <= sheet1.LastRowNum; indx++)
            {
                IRow row = sheet1.GetRow(indx);
                var pno = row.GetCell(1).NumericCellValue.ToString();
                var yno = row.GetCell(2).NumericCellValue.ToString();
                string eno = "";
                try
                {
                    eno = row.GetCell(3).NumericCellValue.ToString();
                }
                catch (Exception)
                {
                    eno = ekNoAyrac(row.GetCell(3).StringCellValue);
                }
                var kkno = row.GetCell(7).StringCellValue.Trim();
                neoOnline_TahsilatKapatma = new NeoOnline_TahsilatKapatma { Police_No = pno, Yenileme_No = yno, Ek_No = eno, Kart_No = kkno };
                policeTahsilatKapatma.Add(neoOnline_TahsilatKapatma);
            }
            excelFile.Dispose();
            excelFile.Close();
            return policeTahsilatKapatma;
        }
        static string ekNoAyrac(string item)
        {
            string sonuc = "";
            for (int i = 0; i < item.Length; i++)
            {
                if (int.TryParse(item[i].ToString(), out int result))
                {
                    sonuc += item[i].ToString();
                }
            }
            return sonuc;
        }
        //public static bool tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas=null, PoliceGenel police=null)
        //{
        //    foreach (var item in neoOnline_TahsilatKapatmas)
        //    {
        //        if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim())
        //        {

        //            return true;
        //        }
        //    }
        //    return false;
        //}

    }
    public class NeoOnline_TahsilatKapatma
    {
        public string Sirket_Ismi { get; set; }
        public string Police_No { get; set; }
        public string Yenileme_No { get; set; }
        public string Ek_No { get; set; }
        public string Brut_Prim { get; set; }
        public string Taksit_Sayisi { get; set; }
        public string Kart_Sahibi { get; set; }
        public string Kimlik_No { get; set; }
        public string Kart_No { get; set; }
        public string Doviz_Kodu { get; set; }
        public string Tahsilat_Tutari { get; set; }
        public string Tahsilat_Tarihi { get; set; }



    }
    public class PoliceGenelBrans
    {
        public string BransAdi { get; set; }
        public int? BransKodu { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string TUMBransAdi { get; set; }
        public string TUMBransKodu { get; set; }

    }


}




