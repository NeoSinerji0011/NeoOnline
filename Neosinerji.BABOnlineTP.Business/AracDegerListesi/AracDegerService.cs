using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AracDegerService : IAracDegerService
    {
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigService;
        ILogService _Log;

        public AracDegerService(IAracContext aracContext, IKonfigurasyonService konfigService, ILogService log)
        {
            _AracContext = aracContext;
            _KonfigService = konfigService;
            _Log = log;
        }

        public void AracDegerlistesiAktar(string path)
        {
            ADL adl = new ADL();
            adl.Init(path);
            adl.Start();
            this.HDIAracListesiAktar();
        }

        public void HDIAracListesiAktar()
        {
            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            ITVMContext _TVMContext = DependencyResolver.Current.GetService<ITVMContext>();

            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { aktifKullanici.TVMKodu, TeklifUretimMerkezleri.HDI });

            KonfigTable konfig = _KonfigService.GetKonfig(Konfig.BundleHDIPlaka);

            StringBuilder requestXml = new StringBuilder();
            requestXml.Append("<HDISIGORTA>");
            requestXml.AppendFormat("<user>{0}</user>", servisKullanici.KullaniciAdi);
            requestXml.AppendFormat("<pwd>{0}</pwd>", servisKullanici.Sifre);
            requestXml.Append("<Uygulama>UYG004</Uygulama>");
            requestXml.AppendFormat("<Refno>{0}</Refno>", Guid.NewGuid().ToString());
            requestXml.Append("</HDISIGORTA>");

            try
            {
                string request = konfig[Konfig.HDI_ServiceURL] + requestXml.ToString();

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(request);
                httpRequest.Timeout = 200000;
                httpRequest.Method = "GET";
                httpRequest.ContentType = "text/xml";

                HttpWebResponse webresponse;
                webresponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream data = webresponse.GetResponseStream();
                StreamReader reader = new StreamReader(data, Encoding.GetEncoding(1254));
                string ResponseString = reader.ReadToEnd();

                ResponseString = ResponseString.Replace("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?>", string.Empty);

                _AracContext.HdiWebServiceRunProcedure(ResponseString);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }
    }
}
