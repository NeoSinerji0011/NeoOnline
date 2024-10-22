using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business.a
{
    public class WebServiceLogService : IWebServiceLogService
    {
        public ITeklif Teklif { get; set; }
        public byte IstekTipi { get; set; }
        public string Istek { get; set; }
        public string Cevap { get; set; }

        private string _Directory;
        private DateTime _IstekTarihi;
        private bool _saved;

        public WebServiceLogService(byte istekTipi)
        {
            this.IstekTipi = istekTipi;

            switch (this.IstekTipi)
            {
                case WebServisIstekTipleri.Teklif: _Directory = "teklif"; break;
                case WebServisIstekTipleri.Police: _Directory = "police"; break;
                case WebServisIstekTipleri.KimlikSorgu: _Directory = "sorgu"; break;
                default: _Directory = ""; break;
            }

            _IstekTarihi = TurkeyDateTime.Now;
            _saved = false;
        }

        public WebServiceLogService(ITeklif teklif, byte istekTipi)
            :this(istekTipi)
        {
            this.Teklif = teklif;
        }

        public void SaveLog()
        {
            ITeklifContext teklifContext = DependencyResolver.Current.GetService<ITeklifContext>();
            IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();

            string istekURL = storage.UploadXml(this._Directory, this.Istek);
            string cevapURL = storage.UploadXml(this._Directory, this.Cevap);

            WEBServisLog log = new WEBServisLog();
            log.TeklifId = this.Teklif.GenelBilgiler.TeklifId;
            log.IstekTipi = this.IstekTipi;
            log.IstekTarihi = this._IstekTarihi;
            log.CevapTarihi = TurkeyDateTime.Now;
            log.IstekUrl = istekURL;
            log.CevapUrl = cevapURL;

            teklifContext.WEBServisLogRepository.Create(log);

            _saved = true;
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_saved)
            {
                if (disposing)
                {
                    this.SaveLog();
                }
            }
        } 
        #endregion
    }
}
