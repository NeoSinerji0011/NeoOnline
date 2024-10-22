using Neosinerji.BABOnlineTP.Business.dogapolicetransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler
{
    public class DOGAOtomatikPoliceTransfer : IDOGAOtomatikPoliceTransfer
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;
        private string _bransBaslangic;
        public string _tahsilatDosyaYolu;
        private string _bransBitis;
        private string _partajNo;

        public DOGAOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string tahsilatDosyaYolu = "")
        {
            this._tvmKodu = tvmKodu;
            this._sirketKodu = sirketKodu;
            this._serviceURL = serviceURL;
            this._KullaniciAdi = KullaniciAdi;
            this._Sifre = Sifre;
            this._TanzimBaslangicTarihi = TanzimBaslangicTarihi;
            this._TanzimBitisTarihi = TanzimBitisTarihi;
            this._tahsilatDosyaYolu = tahsilatDosyaYolu;

            //this._bransBaslangic = bransBaslangic;
            //this._bransBitis = bransBitis;
            //this._partajNo = partajNo;
        }
        public List<Police> GetDOGAAutoPoliceTransfer()
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
                List<Police> policeler = new List<Police>();
                List<Police> returnPoliceler = new List<Police>();
                Police policeItem = new Police();
                using (AcenteBilgiServisleri dogaClnt = new AcenteBilgiServisleri())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    //v_Service.Url = @"https://www.dogasigortaportal.com/WebServisleri/AcenteBilgiServisleri.asmx";
                    dogaClnt.Url = _serviceURL;
                    dogaClnt.Credentials = new NetworkCredential(_KullaniciAdi, _Sifre);
                    _bransBaslangic = "0";
                    _bransBitis = "999";
                    _partajNo = "502081";  //4A

                    GeriyePoliceTransferCevap policeListesi = dogaClnt.PoliceListesi(_KullaniciAdi, _Sifre, _partajNo, _bransBaslangic, _bransBitis, _TanzimBaslangicTarihi.ToString().Replace(".", "/"), _TanzimBitisTarihi.ToString().Replace(".", "/"));

                    DogaPoliceListModel polItemModel = new DogaPoliceListModel();
                    List<DogaPoliceListModel> polListModel = new List<DogaPoliceListModel>();
                    //todo: policeListesi for yazılacak
                    //for (int i = 0; i < policeListesi.Policeler; i++)
                    //{
                    //    var polices = policeListesi.Policeler.ChildNodes;

                    //    for (int j = 0; j < polices.Count; j++)
                    //    {
                    //        if (polices[j].Name == "PoiceNo")
                    //        {
                    //            polItemModel = new DogaPoliceListModel();
                    //            polItemModel.PoliceNo = polices[j].InnerText;
                    //        }
                    //        if (polices[j].Name == "ZeyilNo")
                    //        {
                    //            polItemModel.ZeylSiraNo = polices[j].InnerText;
                    //        }
                    //        if (polices[j].Name == "TecditNo")
                    //        {
                    //            polItemModel.YenilemeNo = polices[j].InnerText;
                    //        }
                    //    }
                    //    polListModel.Add(polItemModel);
                    //}
                    for (int i = 0; i < polListModel.Count; i++)
                    {
                        GeriyePoliceTransferCevap policeDetay = dogaClnt.TekPolice(_KullaniciAdi, _Sifre, _partajNo, polItemModel.Brans, polItemModel.PoliceNo, polItemModel.YenilemeNo, polItemModel.ZeylSiraNo);
                        policeler = _IPoliceTransferService.getDogaPoliceler(_sirketKodu + (_tahsilatDosyaYolu.Length > 0 ? "#" + _tahsilatDosyaYolu : ""), policeListesi, _tvmKodu);
                    }
                }

                return returnPoliceler;
            }
            catch (Exception ex)
            {

                _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu. Detay" + ex.Message.ToString());
                return null;
                throw;
            }
        }
        public class DogaPoliceListModel
        {
            public string PoliceNo { get; set; }
            public string ZeylSiraNo { get; set; }
            public string YenilemeNo { get; set; } //tecdit
            public string Brans { get; set; }
        }
    }
}
