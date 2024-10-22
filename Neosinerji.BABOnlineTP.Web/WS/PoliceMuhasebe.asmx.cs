using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Neosinerji.BABOnlineTP.Business.PoliceMuhasebe;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Service.PoliceMuhasebeService;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.WS
{
    /// <summary>
    /// Summary description for PoliceMuhasebe
    /// </summary>
    [WebService(Namespace = "http://babonline.neosinerji.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PoliceMuhasebe : System.Web.Services.WebService
    {
        ITVMService tvmService;
        IPoliceMuhasebeService policeMuhasebeService;
        ISigortaSirketleriService sigortaSirketleriService;
        IBransService bransService;
        IUrunService urunService;
        private static Int32 IZIN_VERILEN_ACENTE_KODU = 136;
        public PoliceMuhasebe()
        {
            this.tvmService = DependencyResolver.Current.GetService<ITVMService>();
            this.policeMuhasebeService = DependencyResolver.Current.GetService<IPoliceMuhasebeService>();
            this.sigortaSirketleriService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
            this.bransService = DependencyResolver.Current.GetService<IBransService>();
            this.urunService = DependencyResolver.Current.GetService<IUrunService>();
        }

        [WebMethod]
        public AcenteEntity acenteBilgisi(String kimlikNo)
        {
            TVMDetay tvmDetay = tvmService.GetDetayByVergiNo(kimlikNo);
            if (tvmDetay == null)
            {
                tvmDetay = tvmService.GetDetayByTCKimlikNo(kimlikNo);
            }
            //AcenteEntity acente = new AcenteEntity(tvmDetay, null);
            AcenteEntity acente = new AcenteEntity(tvmDetay);

            return acente;
        }

        [WebMethod]
        public PoliceEntity[] acentePoliceListesi(Int32 AcenteKodu, DateTime TarihAraligiBaslangic, DateTime TarihAraligiBitis)
        {
            if(AcenteKodu != IZIN_VERILEN_ACENTE_KODU)
            {
                return null;
            }
            List<PoliceGenel> policeler = policeMuhasebeService.getPoliceMuhasebeList(AcenteKodu, TarihAraligiBaslangic, TarihAraligiBitis);
            TVMDetay tvmDetay = tvmService.GetDetay(AcenteKodu);
            List<PoliceEntity> policeEntityList = policeMuhasebeService.createEntityListFromModelList(policeler, tvmDetay);
            if (policeEntityList != null)
            {
                return policeEntityList.ToArray();
            }
            return null;
        }

        [WebMethod]
        public PoliceKeyModel acentePoliceKeyListesi(Int32 AcenteKodu, DateTime TarihAraligiBaslangic, DateTime TarihAraligiBitis)
        {
            PoliceKeyModel model = new PoliceKeyModel();
            if (AcenteKodu != IZIN_VERILEN_ACENTE_KODU)
            {
                model.basarili = false;
                model.hataMesaji = "Acente kodu hatalı.";
                model.policeList = null;
                return model;
            }
            int gunFarki = this.GunFarkikBul(TarihAraligiBaslangic, TarihAraligiBitis);
            if (gunFarki >= 16)
            {
                model.basarili = false;
                model.hataMesaji = "Maximum 15 günlük poliçe transferi yapılabilir. (1 haftalık önerilir!)";
                model.policeList = null;
            }
            else
            {
                List<PoliceGenel> policeler = policeMuhasebeService.getPoliceMuhasebeList(AcenteKodu, TarihAraligiBaslangic, TarihAraligiBitis);

                List<PoliceKeyEntity> policeKeyList = policeMuhasebeService.createKeyEntityListFromModelList(policeler);
                if (policeKeyList != null && policeKeyList.Count > 0)
                {
                    model.policeList = policeKeyList.ToArray();
                    model.basarili = true;
                    model.hataMesaji = "";
                }
            }

            return model;
        }

        [WebMethod]
        public PoliceEntity getPoliceByKey(String SigortaSirketKodu, String PoliceNo, int YenilemeNo, int EkNo)
        {
            PoliceGenel police = policeMuhasebeService.getPoliceByKey(SigortaSirketKodu, PoliceNo, YenilemeNo, EkNo);
            TVMDetay tvmDetay = null;
            if (police != null)
            {
                tvmDetay = tvmService.GetDetay((Int32)police.TVMKodu);
            }

            PoliceEntity policeEntity = policeMuhasebeService.createEntityFromModel(police, tvmDetay);
            return policeEntity;
        }

        [WebMethod]
        public PoliceEntity getPoliceById(Int32 PoliceNo, String PoliceHash)
        {
            PoliceGenel police = policeMuhasebeService.getPoliceByIdHash(PoliceNo, PoliceHash);
            TVMDetay tvmDetay = null;
            if (police != null)
            {
                tvmDetay = tvmService.GetDetay((Int32)police.TVMKodu);
            }
            PoliceEntity policeEntity = policeMuhasebeService.createEntityFromModel(police, tvmDetay);
            return policeEntity;
        }

        [WebMethod]
        public SigortaSirketResult getSigortaSirketList()
        {
            SigortaSirketResult result = new SigortaSirketResult();
            result.sirketList = new List<SigortaSirketList>();
            SigortaSirketList itemSirket = new SigortaSirketList();
            try
            {
                result.basarili = true;
                result.hataMesaji = "";
                var sirketList = sigortaSirketleriService.GetList();
                if (sirketList.Count > 0)
                {
                    foreach (var item in sirketList)
                    {
                        itemSirket = new SigortaSirketList();
                        if (!String.IsNullOrEmpty(item.SirketKodu))
                        {
                            itemSirket.sigortaSirketKodu = item.SirketKodu;
                        }
                        else
                        {
                            itemSirket.sigortaSirketKodu = "";
                        }
                        if (!String.IsNullOrEmpty(item.SirketAdi))
                        {
                            itemSirket.sigortaSirketUnvani = item.SirketAdi;
                        }
                        else
                        {
                            itemSirket.sigortaSirketUnvani = "";
                        }
                        if (!String.IsNullOrEmpty(item.VergiDairesi))
                        {
                            itemSirket.vergiDairesi = item.VergiDairesi;
                        }
                        else
                        {
                            itemSirket.vergiDairesi = "";
                        }
                        if (!String.IsNullOrEmpty(item.VergiNumarasi))
                        {
                            itemSirket.vergiNumarasi = item.VergiNumarasi;
                        }
                        else
                        {
                            itemSirket.vergiNumarasi = "";
                        }
                        if (!String.IsNullOrEmpty(item.SirketLogo))
                        {
                            itemSirket.logo = item.SirketLogo;
                        }
                        else
                        {
                            itemSirket.logo = "";
                        }

                        result.sirketList.Add(itemSirket);
                    }
                }

            }
            catch (Exception ex)
            {
                result.basarili = false;
                result.hataMesaji = ex.Message;
                result.sirketList = new List<SigortaSirketList>();
            }
            return result;
        }

        [WebMethod]
        public BransListResult getBransList()
        {
            BransListResult result = new BransListResult();
            result.bransList = new List<BransList>();
            BransList itemBrans = new BransList();
            try
            {
                result.basarili = true;
                result.hataMesaji = "";
                var bransList = bransService.GetList(0);
                if (bransList.Count > 0)
                {
                    foreach (var item in bransList)
                    {
                        itemBrans = new BransList();
                        itemBrans.bransKodu = item.BransKodu;
                        if (!String.IsNullOrEmpty(item.BransAdi))
                        {
                            itemBrans.bransAdi = item.BransAdi;
                        }
                        else
                        {
                            itemBrans.bransAdi = "";
                        }
                        result.bransList.Add(itemBrans);
                    }
                }

            }
            catch (Exception ex)
            {
                result.basarili = false;
                result.hataMesaji = ex.Message;
                result.bransList = new List<BransList>();
            }
            return result;
        }

        [WebMethod]
        public UrunListResult getUrunList()
        {
            UrunListResult result = new UrunListResult();
            result.urunList = new List<UrunList>();
            UrunList itemUrun = new UrunList();
            try
            {
                result.basarili = true;
                result.hataMesaji = "";
                var urunList = urunService.GetListUrun();
                if (urunList.Count > 0)
                {
                    foreach (var item in urunList)
                    {
                        itemUrun = new UrunList();
                        itemUrun.urunKodu = item.UrunKodu;
                        if (!String.IsNullOrEmpty(item.UrunAdi))
                        {
                            itemUrun.urunAdi = item.UrunAdi;
                        }
                        else
                        {
                            itemUrun.urunAdi = "";
                        }
                        result.urunList.Add(itemUrun);
                    }
                }

            }
            catch (Exception ex)
            {
                result.basarili = false;
                result.hataMesaji = ex.Message;
                result.urunList = new List<UrunList>();
            }
            return result;
        }

        public int GunFarkikBul(DateTime dt1, DateTime dt2)
        {

            TimeSpan zaman = new TimeSpan(); // zaman farkını bulmak adına kullanılacak olan nesne

            zaman = dt1 - dt2;//metoda gelen 2 tarih arasındaki fark

            return Math.Abs(zaman.Days); // 2 tarih arasındaki farkın kaç gün olduğu döndürülüyor.

        }

        public class PoliceKeyModel
        {
            public PoliceKeyEntity[] policeList { get; set; }
            public bool basarili { get; set; }
            public string hataMesaji { get; set; }
        }

        public class SigortaSirketResult
        {
            public bool basarili { get; set; }
            public string hataMesaji { get; set; }

            public List<SigortaSirketList> sirketList = new List<SigortaSirketList>();

        }
        public class SigortaSirketList
        {
            public string sigortaSirketKodu { get; set; }
            public string sigortaSirketUnvani { get; set; }
            public string vergiDairesi { get; set; }
            public string vergiNumarasi { get; set; }
            public string logo { get; set; }
        }
        public class BransListResult
        {
            public bool basarili { get; set; }
            public string hataMesaji { get; set; }

            public List<BransList> bransList = new List<BransList>();

        }
        public class BransList
        {
            public int bransKodu { get; set; }
            public string bransAdi { get; set; }
        }
        public class UrunListResult
        {
            public bool basarili { get; set; }
            public string hataMesaji { get; set; }

            public List<UrunList> urunList = new List<UrunList>();

        }
        public class UrunList
        {
            public int urunKodu { get; set; }
            public string urunAdi { get; set; }
        }
    }
}
