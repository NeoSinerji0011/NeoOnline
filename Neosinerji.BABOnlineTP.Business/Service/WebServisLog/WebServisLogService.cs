using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class WebServisLogService : IWebServisLogService
    {
        ITeklifContext _db;
        WEBServisLog _AktifLog;
        private string _AktifIstek;

        public WebServisLogService(ITeklifContext teklifContext)
        {
            _db = teklifContext;
        }

        public void BeginLog(string istek, byte istekTipi)
        {
            this._AktifLog = new WEBServisLog();
            _AktifLog.IstekTarihi = TurkeyDateTime.Now;
            _AktifLog.IstekTipi = istekTipi;
            
            this._AktifIstek = istek;
        }

        public void BeginLog(object request, Type type, byte istekTipi)
        {
            try
            {
                string istek = String.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer s = new XmlSerializer(type);
                        s.Serialize(xmlWriter, request);
                    }

                    istek = Encoding.UTF8.GetString(ms.ToArray());
                }

                this.BeginLog(istek, istekTipi);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
        }

        public void EndLog(string cevap, bool basarili)
        {
            if (this._AktifLog == null)
                return;

            IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();

            _AktifLog.CevapTarihi = TurkeyDateTime.Now;
            _AktifLog.BasariliBasarisiz = basarili ? WebServisBasariTipleri.Basarili : WebServisBasariTipleri.Basarisiz;

            string directory = "teklif";

            switch (_AktifLog.IstekTipi)
            {
                case WebServisIstekTipleri.Police: directory = "police"; break;
                case WebServisIstekTipleri.MusteriKayit: directory = "teklif"; break;
                case WebServisIstekTipleri.Muhasebe: directory = "muhasebe"; break;
                case WebServisIstekTipleri.KimlikSorgu:
                case WebServisIstekTipleri.PlakaSorgu: directory = "sorgu"; break;
                default: directory = "sorgu"; break;
            }

            string istekURL = storage.UploadXml(directory, _AktifIstek);
            string cevapURL = storage.UploadXml(directory, cevap);

            _AktifLog.IstekUrl = istekURL;
            _AktifLog.CevapUrl = cevapURL;

            _db.WEBServisLogRepository.Create(this._AktifLog);
            _db.Commit();
            _AktifLog = null;
        }

        public void EndLog(object response, bool basarili, Type type)
        {
            try
            {
                string cevap = String.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer s = new XmlSerializer(type);
                        s.Serialize(xmlWriter, response);
                    }

                    cevap = Encoding.UTF8.GetString(ms.ToArray());
                }

                this.EndLog(cevap, basarili);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
        }
    }
}
