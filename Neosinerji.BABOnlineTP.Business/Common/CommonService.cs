using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class CommonService : ICommonService
    {
        public enum DosyaTipleri : byte
        {
            WordBelgesi = 1,
            ResimDosyasi = 2,
            ExcelDosyasi = 3,
            SunumDosyasi = 4,
            MetinDosyasi = 5,
            PdfDosyasi = 6,
        }
        public string GetDosyaTipAciklama(byte tipKodu)
        {
            string aciklama = "";
            switch (tipKodu)
            {
                case 1: aciklama = "WORD BELGESİ"; break;
                case 2: aciklama = "RESİM DOSYASI"; break;
                case 3: aciklama = "EXCELL DOSYASI"; break;
                case 4: aciklama = "POWER POİNT DOSYASI"; break;
                case 5: aciklama = "PDF DOSYASI"; break;
                case 6: aciklama = "METİN DOSYASI"; break;
            }
            return aciklama;
        }
        public byte GetFileType(string extension)
        {
            List<string> wordDosyaTurleri = new List<string>() { "docx", "doc" };
            List<string> excelDosyaTurleri = new List<string>() { "xls", "xlsx" };
            List<string> pdfDosyaTurleri = new List<string>() { "pdf" };
            List<string> metinDosyaTurleri = new List<string>() { "txt" };
            List<string> sunumDosyaTurleri = new List<string>() { "pdf" };
            List<string> resimDosyaTurleri = new List<string>() { "jpeg", "jpg", "png", "bmp", "gif", "psd" };

            byte fileType = 0;

            if (wordDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.WordBelgesi;
            }
            else if (excelDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.ExcelDosyasi;
            }
            else if (pdfDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.PdfDosyasi;
            }
            else if (metinDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.MetinDosyasi;
            }
            else if (sunumDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.SunumDosyasi;
            }
            else if (resimDosyaTurleri.Contains(extension.ToLower()))
            {
                fileType = (byte)DosyaTipleri.ResimDosyasi;
            }
            return fileType;
        }

        public string DurumRenk(byte durum)
        {
            string renk = "";
            switch (durum)
            {
                case 1: renk = "#ff80ff"; break;
                case 2: renk = "#ff9933"; break;
                case 3: renk = "#66b3ff"; break;
                case 4: renk = "#ff0000"; break;
                default: break;
            }
            return renk;
        }
        public string OncelikSeviyesiRenk(byte oncelikSeviye)
        {
            string renk = "";
            switch (oncelikSeviye)
            {
                case 1: renk = "#ff0000"; break;
                case 2: renk = "#993300"; break;
                case 3: renk = "#009900"; break;
                default: break;
            }
            return renk;
        }
        public int GunFarkikBul(DateTime dt1, DateTime dt2)
        {

            TimeSpan zaman = new TimeSpan(); // zaman farkını bulmak adına kullanılacak olan nesne

            zaman = dt1 - dt2;//metoda gelen 2 tarih arasındaki fark

            return Math.Abs(zaman.Days); // 2 tarih arasındaki farkın kaç gün olduğu döndürülüyor.

        }
        public List<SelectListItem> DonemAralikListesi()
        {
            List<SelectListItem> listYillar = new List<SelectListItem>();
            int yilAraligi1 = 0;
            int yilAraligi2 = 0;
            for (int yil = TurkeyDateTime.Today.AddYears(2).Year; yil >= 2015; yil--)
            {
                yilAraligi1 = yil;
                yilAraligi2 = yil - 1;
                string yilValue = yilAraligi1.ToString() + "-" + yilAraligi2.ToString();
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yilValue  , Text = yilValue },
                });
            }
            return listYillar;
        }
    }
    public class ListModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
