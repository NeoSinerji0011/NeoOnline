using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler
{
    public class MAPFREDASKOtomtikPoliceTranfer : IMAPFREDASKOtomtikPoliceTranfer
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;

        public MAPFREDASKOtomtikPoliceTranfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            this._tvmKodu = tvmKodu;
            this._sirketKodu = sirketKodu;
            this._serviceURL = serviceURL;
            this._KullaniciAdi = KullaniciAdi;
            this._Sifre = Sifre;
            this._TanzimBaslangicTarihi = TanzimBaslangicTarihi;
            this._TanzimBitisTarihi = TanzimBitisTarihi;
        }

        public List<Police> GetMAPFREDASKAutoPoliceTransfer()
        {

            #region Service DependencyResolver

            IPoliceTransferService _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();

            IPoliceContext _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
            _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();

            IPoliceTransferStorage _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();
            _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();

            IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();


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
                    //string webServisURL = "";
                    //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                    //webServisURL = konfig[Konfig.MAPFREDASK_OtomatikPoliceTransferURL];
                    //_serviceURL = webServisURL;

                    var responseXML = _serviceURL + "?direction=transferEdilecekPoliceler" + "&userName=" + _KullaniciAdi + "&userPassword=" + _Sifre + "&tanzimTarihi=" + _TanzimBaslangicTarihi.ToString("yyyy/MM/dd").Replace('.', '/');
                    var police = _IPoliceTransferService.getPoliceler(_sirketKodu, responseXML, _tvmKodu);
                  
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
