using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.Text;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class ExceptionTools
    {
        public static string GetMessage(DbUpdateException dex)
        {
            SqlException s = dex.InnerException.InnerException as SqlException;

            if (s.Number == 2627)
            {
                return babonline.Message_DublicateRecord;
            }

            return babonline.Message_DocumentSaveError;
        }
    }

    #region Vcard

    public class vCardResult : ActionResult
    {
        private VCard _card;
        protected vCardResult() { }
        public vCardResult(VCard card)
        {
            _card = card;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "text/vcard";
            response.AddHeader("Content-Disposition", "attachment; fileName=" + _card.FirstName + " " + _card.LastName + ".vcf");

            var cardString = _card.ToString();
            var inputEncoding = Encoding.Default;
            var outputEncoding = Encoding.GetEncoding("windows-1254");
            var cardBytes = inputEncoding.GetBytes(cardString);
            var outputBytes = Encoding.Convert(inputEncoding,
                                    outputEncoding, cardBytes);
            response.OutputStream.Write(outputBytes, 0, outputBytes.Length);
        }

    }

    public class VCard
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string JobTitle { get; set; }
        public string StreetAddress { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string BussinesPhone { get; set; }
        public string Email { get; set; }
        public string HomePage { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public byte[] Image { get; set; }
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("BEGIN:VCARD");
            builder.AppendLine("VERSION:2.1");

            // Name
            builder.AppendLine("N:" + LastName + ";" + FirstName);

            // Full name
            builder.AppendLine("FN:" + FirstName + " " + LastName);

            // Address
            builder.Append("ADR;HOME;PREF:;;");

            //if (!String.IsNullOrEmpty(Latitude) && !String.IsNullOrEmpty(Longitude))
            //    builder.Append("GEO;geo:" + Latitude + "," + Longitude);

            builder.Append(StreetAddress + ";");
            builder.Append(City + ";;");
            builder.Append(Zip + ";");
            builder.AppendLine(CountryName);

            // Other data
            builder.AppendLine("ORG:" + Organization);
            builder.AppendLine("TITLE:" + JobTitle);
            builder.AppendLine("TEL;HOME;VOICE:" + Phone);
            builder.AppendLine("TEL;CELL;VOICE:" + Mobile);
            builder.AppendLine("TEL;WORK;VOICE:" + BussinesPhone);
            builder.AppendLine("URL;" + HomePage);
            builder.AppendLine("EMAIL;PREF;INTERNET:" + Email);
            builder.AppendLine("END:VCARD");

            return builder.ToString();
        }

    }

    #endregion

    #region Excel

    public class ExcelResult : ActionResult
    {
        private readonly XLWorkbook _workbook;
        private readonly string _fileName;

        public ExcelResult()
            :base()
        {

        }

        public ExcelResult(XLWorkbook workbook, string fileName)
        {
            _workbook = workbook;
            _fileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AddHeader("content-disposition",
                               "attachment;filename=\"" + _fileName + ".xlsx\"");

            using (var memoryStream = new MemoryStream())
            {
                _workbook.SaveAs(memoryStream);
                memoryStream.WriteTo(response.OutputStream);
            }
            response.End();
        }
    }

    #endregion
}