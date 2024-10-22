using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using DataSharetest.DataShare;
using System.IO;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler
{
   
    public class ETHICAOtomatikPoliceTransfer : IETHICAOtomatikPoliceTransfer
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        private string _Sifre;
        public string _tahsilatDosyaYolu;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;
        IBransUrunService _BransUrunService;

        public ETHICAOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string tahsilatDosyaYolu = "")
        {
            this._tvmKodu = tvmKodu;
            this._sirketKodu = sirketKodu;
            this._serviceURL = serviceURL;
            this._KullaniciAdi = KullaniciAdi;
            this._Sifre = Sifre;
            this._TanzimBaslangicTarihi = TanzimBaslangicTarihi;
            this._TanzimBitisTarihi = TanzimBitisTarihi;
            this._tahsilatDosyaYolu = tahsilatDosyaYolu;
        }
        public List<Police> GetETHICAAutoPoliceTransfer()
        {
            #region Service DependencyResolver

            IPoliceTransferService _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();

            IPoliceContext _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
            _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();

            IPoliceTransferStorage _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();
            _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();

            IAktifKullaniciService _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            ITVMService _ITVMService = DependencyResolver.Current.GetService<ITVMService>();


            #endregion


            try
            {
                
                var responseXML = _serviceURL + "AUser=" + _KullaniciAdi + "&Pwd=" + _Sifre + "&BasTar=" + _TanzimBaslangicTarihi.ToString("yyyyMMdd") + "&BitTar=" + _TanzimBitisTarihi.ToString("yyyyMMdd");
                var policeler = _IPoliceTransferService.getPoliceler(_sirketKodu, responseXML+ (_tahsilatDosyaYolu.Length > 0 ? "#" + _tahsilatDosyaYolu : ""), _tvmKodu);
             
               
                return policeler;
            }
            catch (Exception)
            {
                _IPoliceTransferService.setMessage("Otomatik Poliçe Transfer edilirken bir sorun oluştu.");
                return null;
            }
        }

    }
}
