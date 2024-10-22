using Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class MapfreOtomatikPoliceTransfer : IMAPFREOtomatikPoliceTransferReader
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        public string _tahsilatDosyaYolu;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;

        public MapfreOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string tahsilatDosyaYolu = "")
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

        public List<Police> GetMAPFREAutoPoliceTransfer()
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
            List<Police> policeler = new List<Police>();
            #endregion

            try
            {
                var gunFarki = this.GunFarkikBul(_TanzimBitisTarihi, _TanzimBaslangicTarihi);
                for (var i = 0; i <= gunFarki; i++)
                {
                    if (i != 0)
                    {
                        _TanzimBaslangicTarihi = _TanzimBaslangicTarihi.AddDays(1);
                    }

                    //mapfre poliçe xml leri storage e yazılmayacak
                   // var dosyaVarMi = false;
                    //var DosyaURL = _IPoliceTransferService.AutoPoliceTransferGetir(_TanzimBaslangicTarihi, _sirketKodu, _tvmKodu);
                    //if (DosyaURL != null)
                    //{
                    //    dosyaVarMi = true;
                    //}
                    var responseXML = _serviceURL + "?direction=transferEdilecekPoliceler" + "&userName=" + _KullaniciAdi + "&userPassword=" + _Sifre + "&tanzimTarihi=" + _TanzimBaslangicTarihi.ToString("yyyy/MM/dd").Replace('.', '/');
                    var police = _IPoliceTransferService.getPoliceler(_sirketKodu, responseXML+ (_tahsilatDosyaYolu.Length > 0 ? "#" + _tahsilatDosyaYolu : ""), _tvmKodu);
                    //MAPFREDASKOtomtikPoliceTranfer aa = new MAPFREDASKOtomtikPoliceTranfer( _tvmKodu,  _sirketKodu,  _serviceURL,  _KullaniciAdi,  _Sifre,  _TanzimBaslangicTarihi,  _TanzimBitisTarihi);
                    //aa.GetMAPFREDASKAutoPoliceTransfer();
                    //if (!dosyaVarMi)
                    //{
                    //    string url = "";
                    //    if (police != null)
                    //    {
                    //        WebClient myClient = new WebClient();
                    //        byte[] data = myClient.DownloadData(responseXML);

                    //        string fileName = String.Format("Mapfre_{0}.xml", _TanzimBaslangicTarihi.ToString("dd.MM.yyyy") + "_" + System.Guid.NewGuid().ToString("N"));
                    //        url = _IPoliceTransferStorage.UploadFile(fileName, data);
                    //    }

                    //    AutoPoliceTransfer item = new AutoPoliceTransfer();
                    //    item.TvmKodu = _tvmKodu;
                    //    item.SirketKodu = _sirketKodu;
                    //    item.PoliceTransferUrl = url;
                    //    item.TanzimBaslangicTarihi = _TanzimBaslangicTarihi;
                    //    item.TanzimBitisTarihi = _TanzimBaslangicTarihi;
                    //    item.KayitTarihi = TurkeyDateTime.Now;
                    //    item.KaydiEkleyenKullaniciKodu = _IAktifKullaniciService.KullaniciKodu;

                    //    _IPoliceContext.AutoPoliceTransferRepository.Create(item);
                    //    _IPoliceContext.Commit();

                    //}
                    if (police != null)
                    {
                        foreach (var items in police)
                        {
                            policeler.Add(items);
                        }
                    }
                }
                return policeler;
            }
            catch (Exception)
            {
                _IPoliceTransferService.setMessage("Otomatik Poliçe Transfer edilirken bir sorun oluştu.");
                return null;
                throw;
            }
        }

        public int GunFarkikBul(DateTime dt1, DateTime dt2)
        {
            TimeSpan zaman = new TimeSpan(); // zaman farkını bulmak adına kullanılacak olan nesne
            zaman = dt1 - dt2;//metoda gelen 2 tarih arasındaki fark
            return Math.Abs(zaman.Days); // 2 tarih arasındaki farkın kaç gün olduğu döndürülüyor.
        }
    }
}
