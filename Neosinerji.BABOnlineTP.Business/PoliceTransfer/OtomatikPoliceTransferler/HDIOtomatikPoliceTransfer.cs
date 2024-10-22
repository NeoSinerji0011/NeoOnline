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
    public class HDIOtomatikPoliceTransfer : IHDIOtomatikPoliceTransferReader
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        public string _tahsilatDosyaYolu;
        private string _KullaniciAdi;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;

        public HDIOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi,string tahsilatDosyaYolu = "")
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

        public List<Police> GetHDIAutoPoliceTransfer()
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

            #endregion

            try
            {
                //HDI poliçe xml leri storage e yazılmayacak
                // string url = "";
                var responseXML = _serviceURL + "AUser=" + _KullaniciAdi + "&Pwd=" + _Sifre + "&BasTar=" + _TanzimBaslangicTarihi.ToString("yyyyMMdd") + "&BitTar=" + _TanzimBitisTarihi.ToString("yyyyMMdd");
                var policeler = _IPoliceTransferService.getPoliceler(_sirketKodu, responseXML+ (_tahsilatDosyaYolu.Length>0? "#"+_tahsilatDosyaYolu:""), _tvmKodu);
                //if (policeler != null)
                //{
                //    WebClient myClient = new WebClient();
                //    byte[] data = myClient.DownloadData(responseXML);

                //    string fileName = String.Format("HDI_{0}.xml", _TanzimBaslangicTarihi.ToString("dd.MM.yyyy") + "_" + _TanzimBitisTarihi.ToString("dd.MM.yyyy") + "_" + System.Guid.NewGuid().ToString("N"));
                //    url = _IPoliceTransferStorage.UploadFile(fileName, data);


                //    AutoPoliceTransfer item = new AutoPoliceTransfer();
                //    item.TvmKodu = _tvmKodu;
                //    item.SirketKodu = _sirketKodu;
                //    item.PoliceTransferUrl = url;
                //    item.TanzimBaslangicTarihi = _TanzimBaslangicTarihi;
                //    item.TanzimBitisTarihi = _TanzimBitisTarihi;
                //    item.KayitTarihi = TurkeyDateTime.Now;
                //    item.KaydiEkleyenKullaniciKodu = _IAktifKullaniciService.KullaniciKodu;

                //    _IPoliceContext.AutoPoliceTransferRepository.Create(item);
                //    _IPoliceContext.Commit();
                //}
                //else
                //{
                //    _IPoliceTransferService.setMessage("Otomatik Poliçe Transfer edilirken bir sorun oluştu.");
                //}
                foreach (var item in policeler)
                {
                    if (item.GenelBilgiler.PoliceArac.TescilSeriNo != null)
                    {
                        var temptescil = TescilPart(item.GenelBilgiler.PoliceArac.TescilSeriNo);
                        item.GenelBilgiler.PoliceArac.TescilSeriKod = temptescil[0];
                        item.GenelBilgiler.PoliceArac.TescilSeriNo = temptescil[1];
                    }
                }
                return policeler;
            }
            catch (Exception)
            {
                _IPoliceTransferService.setMessage("Otomatik Poliçe Transfer edilirken bir sorun oluştu.");
                return null;
            }
        }
        string[] TescilPart(string val)
        {
            string temp = "", temp2 = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (int.TryParse(val[i].ToString(), out int res))
                {
                    temp2 = val.Substring(i);
                    break;
                }
                temp += val[i].ToString();
            }
            temp = temp.Replace(" ", "");
            temp2 = temp2.Replace(" ", "");


            return new string[] { temp, temp2 };
        }
    }
}
