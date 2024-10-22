using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using System.Xml;
using Neosinerji.BABOnlineTP.Web.Areas.PoliceTransfer.Models;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Business.Service.PoliceMuhasebeService;
using Neosinerji.BABOnlineTP.Business.PoliceMuhasebe;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceOnaylama;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceTransfer.Controllers
{

    [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = 0, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class PoliceTransferController : Controller
    {
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;
        IPoliceTransferService _PoliceTransferService;
        IPoliceMuhasebeService _PoliceMuhasebeService;

        IPoliceService _PoliceService;
        ITaliPoliceService _TaliPoliceService;
        IKomisyonService _KomisyonService;
        ITeklifService _TeklifService;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ITUMService _TUMService;
        ITVMService _TVMService;
        ISigortaSirketleriService _SigortaSirketleriService;
        public PoliceTransferController(
                                 IKullaniciService kullanici,
                                 IAktifKullaniciService aktifKullanici,
                                 IPoliceTransferService policeTransferService,
                                 IPoliceService policeService,
                                 ITaliPoliceService taliPoliceService,
                                 IKomisyonService komisyonService,
                                ITeklifService teklifService,
            IKonfigurasyonService konfigurasyonService, ITVMContext TVMContext, ITUMService TUMService, ITVMService tvmService, ISigortaSirketleriService sigortaSirketleriService, IPoliceMuhasebeService policeMuhasebeService)
        {
            _KullaniciService = kullanici;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _PoliceTransferService = policeTransferService;
            _PoliceService = policeService;
            _TaliPoliceService = taliPoliceService;
            _KomisyonService = komisyonService;
            _TeklifService = teklifService;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = TVMContext;
            _TUMService = TUMService;
            _TVMService = tvmService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _PoliceMuhasebeService = policeMuhasebeService;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.PoliceTransfer, SekmeKodu = 0)]
        public ActionResult PoliceTransfer()
        {
            try
            {
                PoliceTransferModel model = new PoliceTransferModel();
                model.SigortaSirketleri = this.SigortaSirketleri;
                model.AutoPoliceTransferSirketleri = this.AutoPoliceTransferSirketleri;
                model.Islemler = new SelectList(Islemler.PoliceTransferIslemTipleri(), "Value", "Text");
                model.TransferTipleri = Islemler.PoliceTransferTipleri();
                model.TaliAcenteler = this.PoliceTransferTaliAcenteler;
                List<SelectListItem> AxaPoliceTipi = new List<SelectListItem>();

                AxaPoliceTipi.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Elementer" },
                new SelectListItem() { Value = "1", Text ="Hayat/Sağlık/Emeklilik"}
            });

                model.AxaPoliceTipleri = new SelectList(AxaPoliceTipi, "Value", "Text", model.AxaPoliceTipi);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/Errorpage/500");
            }
        }

        [HttpPost]
        public ActionResult PoliceKaydet(PoliceTranferKayitModel model, HttpPostedFileBase tahsilatFile)
        {
            if (ModelState.IsValid && !String.IsNullOrEmpty(model.Path))
            {
                List<Business.Police> policeler = _PoliceTransferService.getPoliceler(model.SigortaSirketiKodu, model.Path, _AktifKullanici.TVMKodu);

                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullanici.TVMKodu);

                if (tvmTaliVar)
                {
                    List<TaliAcenteKomisyonOrani> taliKomisyonOranlari;
                    TaliAcenteKomisyonOrani taliKomisyonOrani;
                    TaliAcenteKomisyonOrani disKaynakKomisyonOrani;

                    bool disUretim = false;
                    if (!String.IsNullOrEmpty(model.taliTvmKodu))
                    {
                        //disUretim = this.UzerindenPoliceTransferYapiliyorMu(Convert.ToInt32(model.taliTvmKodu));
                        disUretim = true;
                    }
                    foreach (var pol in policeler)
                    {
                        taliKomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
                        taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                        List<TaliAcenteKomisyonOrani> disKaynakKomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
                        int zeylinPoliceTaliTVMKodu = 0;
                        decimal? zeylinTaliKomisyonOrani = null;
                        int taliTVMKodu = 0;
                        decimal GerceklesenTaliUretim = 0;
                        //---Poliçe Transferden gelen poliçe teklifgenel tablosunda var ise tvmkodu okunuyor
                        string tcknVkn = "";
                        if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo))
                        {
                            tcknVkn = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                        }
                        else if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                        {
                            tcknVkn = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        }

                        if (pol.GenelBilgiler.TaliAcenteKodu != null)
                        {
                            taliTVMKodu = pol.GenelBilgiler.TaliAcenteKodu.Value;
                        }
                        else
                        {
                            taliTVMKodu = _TeklifService.GetTUMPoliceTvmKodu(pol.GenelBilgiler.PoliceNumarasi, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.TUMUrunKodu, tcknVkn);
                        }

                        if (pol.GenelBilgiler.BransKodu != BransListeCeviri.TanimsizBransKodu)
                        {
                            #region Dış Kaynak Komisyonu var ise
                            if (disUretim)
                            {
                                disKaynakKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, Convert.ToInt32(model.taliTvmKodu), 0, model.SigortaSirketiKodu, pol.GenelBilgiler.BransKodu.Value);

                                if (disKaynakKomisyonOranlari != null && disKaynakKomisyonOranlari.Count > 0)
                                {
                                    disKaynakKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    if (disKaynakKomisyonOranlari.Count > 0)
                                    {
                                        disKaynakKomisyonOrani = disKaynakKomisyonOranlari[0];
                                    }
                                    if (disKaynakKomisyonOrani != null)
                                    {
                                        if (disKaynakKomisyonOrani.KomisyonOran != null)
                                        {
                                            if (disKaynakKomisyonOrani.KomisyonOran > 100)
                                            {
                                                disKaynakKomisyonOrani.KomisyonOran = disKaynakKomisyonOrani.KomisyonOran / 10;
                                            }
                                            pol.GenelBilgiler.Komisyon = (pol.GenelBilgiler.Komisyon * disKaynakKomisyonOrani.KomisyonOran) / 100;
                                            pol.GenelBilgiler.UretimTaliAcenteKodu = Convert.ToInt32(model.taliTvmKodu);
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Zeyl ise Poliçesi onaylanmış mı kontrol ediliyor

                            if (pol.GenelBilgiler.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                            {
                                if (pol.GenelBilgiler.EkNo > 1 || pol.GenelBilgiler.BransKodu == BransListeCeviri.Dask)
                                {
                                    if (pol.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (pol.GenelBilgiler.EkNo.ToString().Substring(4, 1) != "1")
                                        {
                                            var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                            if (ZeylinPolicesi != null)
                                            {
                                                zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                                zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                            }
                                        }
                                    }
                                    else if (pol.GenelBilgiler.EkNo > 1)
                                    {
                                        var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                        if (ZeylinPolicesi != null)
                                        {
                                            zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                            zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (pol.GenelBilgiler.EkNo > 0)
                                {
                                    var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 0);
                                    if (ZeylinPolicesi != null)
                                    {
                                        zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                        zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                    }
                                }
                            }

                            if (zeylinPoliceTaliTVMKodu != 0 && zeylinTaliKomisyonOrani != null)
                            {
                                pol.GenelBilgiler.TaliAcenteKodu = zeylinPoliceTaliTVMKodu;
                                pol.GenelBilgiler.TaliKomisyonOran = zeylinTaliKomisyonOrani;
                                pol.GenelBilgiler.TaliKomisyon = (pol.GenelBilgiler.Komisyon * pol.GenelBilgiler.TaliKomisyonOran) / 100;
                                pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = TurkeyDateTime.Now;
                                pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                            }
                            #endregion

                            if (zeylinPoliceTaliTVMKodu == 0 && taliTVMKodu != 0) // Poliçe ise ,Zeyl onaylanmamışsa ve tali acente kodu dolu ise
                            {
                                if (String.IsNullOrEmpty(model.taliTvmKodu))
                                {
                                    model.taliTvmKodu = "0";
                                }
                                GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(taliTVMKodu, pol.GenelBilgiler.TanzimTarihi.Value.Year, pol.GenelBilgiler.BransKodu.Value);
                                taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, taliTVMKodu, Convert.ToInt32(model.taliTvmKodu), pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.BransKodu.Value);

                                if (taliKomisyonOranlari != null)
                                {
                                    taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    if (taliKomisyonOranlari.Count() > 1)
                                    {
                                        //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                        decimal maxUertim = 0;
                                        foreach (var item in taliKomisyonOranlari)
                                        {
                                            if (maxUertim < item.MaxUretim)
                                            {
                                                maxUertim = item.MaxUretim.Value;
                                            }
                                        }
                                        GerceklesenTaliUretim += pol.GenelBilgiler.NetPrim ?? 0;
                                        taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                                        // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                        if (taliKomisyonOrani == null)
                                        {
                                            taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                        }
                                    }
                                    //kademeli çalışmıyorsa
                                    else if (taliKomisyonOranlari.Count > 0)
                                    {
                                        if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                        {
                                            taliKomisyonOrani = taliKomisyonOranlari[0];
                                        }
                                    }
                                }

                                if (taliKomisyonOrani.KomisyonOran.HasValue)
                                {
                                    pol.GenelBilgiler.TaliAcenteKodu = taliTVMKodu;
                                    if (taliKomisyonOrani.KomisyonOran > 100)
                                    {
                                        taliKomisyonOrani.KomisyonOran = taliKomisyonOrani.KomisyonOran / 10;
                                    }
                                    pol.GenelBilgiler.TaliKomisyonOran = taliKomisyonOrani.KomisyonOran;
                                    pol.GenelBilgiler.TaliKomisyon = (pol.GenelBilgiler.Komisyon * pol.GenelBilgiler.TaliKomisyonOran) / 100;
                                    pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = TurkeyDateTime.Now;
                                    pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                                }
                            }
                        }
                    }


                }
                if (policeler != null)
                {
                    _PoliceService.Add(policeler);
                    model.basariliKayitlar = _PoliceService.getBasariliKayitlar();
                    model.basarisizKayitlar = _PoliceService.getVarolanKayitlar();
                    model.HataliEklenmeyenPoliceSayisi = _PoliceService.getHataliEklenmeyenKayitlar();
                    var HataliPoliceList = _PoliceService.getEklenmeyenPoliceler();
                    model.HataliEklenmeyenPoliceLsitesi = new List<PoliceKontrolModel>();
                    PoliceKontrolModel policeItem = new PoliceKontrolModel();
                    if (HataliPoliceList != null)
                    {
                        foreach (var item in HataliPoliceList)
                        {
                            policeItem = new PoliceKontrolModel();
                            policeItem.PoliceNo = item.PoliceNo;
                            policeItem.EkNo = item.EkNo;
                            policeItem.YenilemeNo = item.YenilemeNo;
                            policeItem.Hatatip = item.Hatatip;
                            model.HataliEklenmeyenPoliceLsitesi.Add(policeItem);
                        }
                    }
                    return PartialView("_PoliceTransferSonuc", model);
                    // return Json(new { Success = true, BasariliKayit = model.basariliKayitlar, BasarisizKayit = model.basarisizKayitlar, HataliKayit = model.basarisizKayitlar,HataliKayitlsit=model.HataliEklenmeyenPoliceLsitesi });
                }
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        [AjaxException]
        public ActionResult Upload(PoliceTransferModel model, HttpPostedFileBase file, HttpPostedFileBase tahsilatFile)
        {
            try
            {
                if (model.IslemTipi == PoliceIslemTipi.DosyadanTransfer)
                {
                    if (ModelState["AutoPoliceTransferSirketiKodu"] != null)
                        ModelState["AutoPoliceTransferSirketiKodu"].Errors.Clear();

                    if (ModelState["TanzimBaslangicTarihi"] != null)
                        ModelState["TanzimBaslangicTarihi"].Errors.Clear();

                    if (ModelState["TanzimBitisTarihi"] != null)
                        ModelState["TanzimBitisTarihi"].Errors.Clear();

                    if (ModelState["TransferTipi"] != null)
                        ModelState["TransferTipi"].Errors.Clear();

                    if (ModelState["TaliAcenteKodu"] != null)
                        ModelState["TaliAcenteKodu"].Errors.Clear();
                }

                //if (!System.IO.File.Exists(path1) && _AktifKullanici.TVMKodu == 186)
                //{
                //    return Json(new { Success186 = true, Mesaj = "Kredi Kartı Tahsilat dosyası bulunamadığı için tahsilatlar otomatik kapatılacaktır. Devam etmek için 'Evet'e basınız. 'Iptal' durumunda 'NeoOnline_TahsilatKapatma.xls' dosyasını yükleyerek işlemi yeniden başlatabilirsiniz. Tahsilat kapatma dosyasında bulunan kayıtlara ait poliçelerin tahsilatı kapatılmayarak açık bırakılacaktır." });
                //}



                if (ModelState.IsValid && file != null && file.ContentLength > 0 && !String.IsNullOrEmpty(model.SigortaSirketiKodu))
                {
                    string path = String.Empty;
                    List<Business.Police> policeler = null;

                    path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);

                    file.SaveAs(path);
                    string pathTahsilat = String.Empty;
                    if (model.TahsilatDosyasiVarmi2 == 1)
                    {
                        pathTahsilat = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + tahsilatFile.FileName);

                        tahsilatFile.SaveAs(pathTahsilat);
                        path += "#" + pathTahsilat;
                    }
                    policeler = _PoliceTransferService.getPoliceler(model.SigortaSirketiKodu, path, _AktifKullanici.TVMKodu);

                    if (policeler != null)
                    {
                        PoliceTranferKayitModel police = new PoliceTranferKayitModel();
                        police.policeCount = policeler.Count();
                        police.Path = path;
                        police.SigortaSirketiKodu = model.SigortaSirketiKodu;
                        police.taliTvmKodu = model.TaliAcenteKodu;
                        return PartialView("_PoliceKayit", police);
                    }
                    var TahsilatMi = _PoliceTransferService.getTahsilatMi();
                    if (TahsilatMi)
                    {
                        PoliceTranferKayitModel police = new PoliceTranferKayitModel();
                        string msj = _PoliceTransferService.getMessage();
                        police.TahsilatMesaj = msj;
                        police.TahsilatMi = true;

                        return PartialView("_PoliceKayit", police);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        async System.Threading.Tasks.Task<string> downloadXmlAsync(string url)
        {
            // Download zip file
            var fileContent = new System.Net.WebClient().DownloadData(url); //byte[]
            var guid = Guid.NewGuid().ToString();

            var path = Path.Combine(Server.MapPath("~/Files"), "_" + guid + ".zip");
            //System.IO.File.WriteAllBytesAsync(path, fileContent);
            FileStream fs = System.IO.File.OpenWrite(path);


            fs.Write(fileContent, 0, fileContent.Length);
            fs.Dispose();
            fs.Close();
            // Extract from zip archive
            //var extractPath = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "/");
            var extractPath = Path.Combine(Server.MapPath("~/Files"), "_" + guid + "\\");
            System.IO.Compression.ZipFile.ExtractToDirectory(path, extractPath);

            // Get files
            //string[] xmlFiles = Directory.GetFiles(@extractPath, "*.xml", SearchOption.AllDirectories); // we need xmlFiles[0]

            //string xmlContent = System.IO.File.ReadAllText(xmlFiles[0]);
            //xmlContent = xmlContent.Replace("\r", "");
            //xmlContent = xmlContent.Replace("\n", "");
            string[]  filePaths = Directory.GetFiles(extractPath, "*.xml",
                                         SearchOption.AllDirectories);
            // @todo: delete template files
            return filePaths[0];
        }
        [HttpPost]
        [AjaxException]
        public ActionResult Upload186(PoliceTransferModel model, HttpPostedFileBase file)
        {
            try
            {
                if (model.IslemTipi == PoliceIslemTipi.DosyadanTransfer)
                {
                    if (ModelState["AutoPoliceTransferSirketiKodu"] != null)
                        ModelState["AutoPoliceTransferSirketiKodu"].Errors.Clear();

                    if (ModelState["TanzimBaslangicTarihi"] != null)
                        ModelState["TanzimBaslangicTarihi"].Errors.Clear();

                    if (ModelState["TanzimBitisTarihi"] != null)
                        ModelState["TanzimBitisTarihi"].Errors.Clear();

                    if (ModelState["TransferTipi"] != null)
                        ModelState["TransferTipi"].Errors.Clear();

                    if (ModelState["TaliAcenteKodu"] != null)
                        ModelState["TaliAcenteKodu"].Errors.Clear();
                }

                if (ModelState.IsValid && file != null && file.ContentLength > 0 && !String.IsNullOrEmpty(model.SigortaSirketiKodu))
                {
                    string path = String.Empty;
                    List<Business.Police> policeler = null;

                    path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
                    file.SaveAs(path);
                    policeler = _PoliceTransferService.getPoliceler(model.SigortaSirketiKodu, path, _AktifKullanici.TVMKodu);

                    if (policeler != null)
                    {
                        PoliceTranferKayitModel police = new PoliceTranferKayitModel();
                        police.policeCount = policeler.Count();
                        police.Path = path;
                        police.SigortaSirketiKodu = model.SigortaSirketiKodu;
                        police.taliTvmKodu = model.TaliAcenteKodu;
                        return PartialView("_PoliceKayit", police);
                    }
                    var TahsilatMi = _PoliceTransferService.getTahsilatMi();
                    if (TahsilatMi)
                    {
                        PoliceTranferKayitModel police = new PoliceTranferKayitModel();
                        string msj = _PoliceTransferService.getMessage();
                        police.TahsilatMesaj = msj;
                        police.TahsilatMi = true;

                        return PartialView("_PoliceKayit", police);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }
        string tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas = null, PoliceGenel police = null)
        {
            foreach (var item in neoOnline_TahsilatKapatmas)
            {
                if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                {
                    return item.Kart_No.Trim();
                }
            }
            return "";
        }
        [HttpPost]
        public ActionResult OtomatikPoliceKaydet(PoliceTransferModel model, HttpPostedFileBase tahsilatFile)
        {
            try
            {
                if (model.IslemTipi == 1)
                {
                    if (ModelState["SigortaSirketiKodu"] != null)
                        ModelState["SigortaSirketiKodu"].Errors.Clear();

                    if (ModelState["TaliAcenteKodu"] != null)
                        ModelState["TaliAcenteKodu"].Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    string path = String.Empty;
                    List<Business.Police> policeler = null;
                    string TahsilatKapamaResultMesaj = "";
                    string webServisURL = this.getSirketServiceURL(model.AutoPoliceTransferSirketiKodu, model.TransferTipi, model.AxaPoliceTipi);

                    var tumKodu = _TUMService.GetTUMKodu(model.AutoPoliceTransferSirketiKodu);

                    TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { _AktifKullanici.TVMKodu, tumKodu });
                    if (servisKullanici != null)
                    {

                        if (model.TransferTipi == PoliceTransferTipi.PoliceTransfer)
                        {
                            string pathTahsilat = String.Empty;
                            if (model.TahsilatDosyasiVarmi == 1)
                            {
                                pathTahsilat = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + tahsilatFile.FileName);

                                tahsilatFile.SaveAs(pathTahsilat);
                                path += "#" + pathTahsilat;
                                
                            }
                            if (model.AutoPoliceTransferSirketiKodu == SigortaSirketiBirlikKodlari.ETHICA)
                            {
                                //servisKullanici.KullaniciAdi2 ilgili sirket için bu alana ip adresini eklenmiştir. 
                                var basTraih = (model.TanzimBaslangicTarihi.Day < 10 ? "0" + model.TanzimBaslangicTarihi.Day.ToString() : model.TanzimBaslangicTarihi.Day.ToString()) + "." + (model.TanzimBaslangicTarihi.Month<10?"0"+ model.TanzimBaslangicTarihi.Month.ToString() : model.TanzimBaslangicTarihi.Month.ToString()) + "." + model.TanzimBaslangicTarihi.Year;
                                var bitTarih = (model.TanzimBitisTarihi.Day < 10 ? "0" + model.TanzimBitisTarihi.Day.ToString() : model.TanzimBitisTarihi.Day.ToString()) + "." + (model.TanzimBitisTarihi.Month < 10 ? "0" + model.TanzimBitisTarihi.Month.ToString() : model.TanzimBitisTarihi.Month.ToString()) + "." + model.TanzimBitisTarihi.Year;
                                string ulrdata = "?remoteIp=" + servisKullanici.KullaniciAdi2 + "&userName=" + servisKullanici.KullaniciAdi + "&password=" + servisKullanici.Sifre + "&beginDate=" + basTraih + "&endDate=" + bitTarih;
                                var res = downloadXmlAsync(webServisURL + ulrdata);
                                path = res.Result;

                                path += "#" + pathTahsilat;


                                policeler = _PoliceTransferService.getPoliceler(model.AutoPoliceTransferSirketiKodu, path, _AktifKullanici.TVMKodu);
                            }
                            else if (model.AutoPoliceTransferSirketiKodu != SigortaSirketiBirlikKodlari.AXASIGORTA)
                            {
                                policeler = _PoliceTransferService.getAutoPoliceler(_AktifKullanici.TVMKodu, model.AutoPoliceTransferSirketiKodu, webServisURL, servisKullanici.KullaniciAdi, servisKullanici.Sifre, model.TanzimBaslangicTarihi, model.TanzimBitisTarihi, pathTahsilat, servisKullanici.PartajNo_);

                            }
                            else
                            {
                                if (model.AxaPoliceTipi == 0) //Elementer
                                {
                                    policeler = _PoliceTransferService.getAutoPoliceler(_AktifKullanici.TVMKodu, model.AutoPoliceTransferSirketiKodu, webServisURL, servisKullanici.KullaniciAdi, servisKullanici.Sifre, model.TanzimBaslangicTarihi, model.TanzimBitisTarihi);

                                }
                                else if (model.AxaPoliceTipi == 1)
                                {
                                    AXAOtomatikPoliceTransfer hayatTransfer = new AXAOtomatikPoliceTransfer(_AktifKullanici.TVMKodu, model.AutoPoliceTransferSirketiKodu, webServisURL, servisKullanici.KullaniciAdi, servisKullanici.Sifre, model.TanzimBaslangicTarihi, model.TanzimBitisTarihi);
                                    policeler = hayatTransfer.GetAXAHayatAutoPoliceTransfer();
                                }
                            }

                        }
                        else
                        {

                            TahsilatKapamaResultMesaj = _PoliceTransferService.getAutoTahsilatPoliceKapatma(_AktifKullanici.TVMKodu, model.AutoPoliceTransferSirketiKodu, webServisURL, servisKullanici.KullaniciAdi, servisKullanici.Sifre, model.TanzimBaslangicTarihi, model.TanzimBitisTarihi);
                        }
                    }

                    if (model.TransferTipi == PoliceTransferTipi.PoliceTransfer)
                    {
                        if (policeler != null)
                        {
                            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullanici.TVMKodu);
                            if (tvmTaliVar)
                            {
                                List<TaliAcenteKomisyonOrani> taliKomisyonOranlari;
                                TaliAcenteKomisyonOrani taliKomisyonOrani;
                                TaliAcenteKomisyonOrani disKaynakKomisyonOrani;

                                bool disUretim = false;
                                if (!String.IsNullOrEmpty(model.TaliAcenteKoduOtomatik))
                                {
                                    disUretim = this.UzerindenPoliceTransferYapiliyorMu(Convert.ToInt32(model.TaliAcenteKoduOtomatik));
                                }
                                foreach (var pol in policeler)
                                {

                                    taliKomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
                                    taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    List<TaliAcenteKomisyonOrani> disKaynakKomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
                                    int zeylinPoliceTaliTVMKodu = 0;
                                    decimal? zeylinTaliKomisyonOrani = null;
                                    int taliTVMKodu = 0;
                                    decimal GerceklesenTaliUretim = 0;
                                    //---Poliçe Transferden gelen poliçe teklifgenel tablosunda var ise tvmkodu okunuyor
                                    string tcknVkn = "";
                                    if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo))
                                    {
                                        tcknVkn = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                    }
                                    else if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                    {
                                        tcknVkn = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    }

                                    if (pol.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        taliTVMKodu = pol.GenelBilgiler.TaliAcenteKodu.Value;
                                    }
                                    else
                                    {
                                        taliTVMKodu = _TeklifService.GetTUMPoliceTvmKodu(pol.GenelBilgiler.PoliceNumarasi, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.TUMUrunKodu, tcknVkn);
                                    }

                                    if (pol.GenelBilgiler.BransKodu != BransListeCeviri.TanimsizBransKodu)
                                    {
                                        #region Dış Kaynak Komisyonu var ise
                                        if (disUretim)
                                        {
                                            if (model.SigortaSirketiKodu == null)
                                            {
                                                if (model.AutoPoliceTransferSirketiKodu == "500")
                                                {
                                                    model.SigortaSirketiKodu = pol.GenelBilgiler.TUMBirlikKodu;
                                                }
                                                else
                                                {
                                                    model.SigortaSirketiKodu = model.AutoPoliceTransferSirketiKodu;
                                                }
                                            }
                                            disKaynakKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, Convert.ToInt32(model.TaliAcenteKoduOtomatik), 0, model.SigortaSirketiKodu, pol.GenelBilgiler.BransKodu.Value);

                                            if (disKaynakKomisyonOranlari != null && disKaynakKomisyonOranlari.Count > 0)
                                            {
                                                disKaynakKomisyonOrani = new TaliAcenteKomisyonOrani();
                                                if (disKaynakKomisyonOranlari.Count > 0)
                                                {
                                                    disKaynakKomisyonOrani = disKaynakKomisyonOranlari[0];
                                                }
                                                if (disKaynakKomisyonOrani != null)
                                                {
                                                    if (disKaynakKomisyonOrani.KomisyonOran != null)
                                                    {
                                                        if (disKaynakKomisyonOrani.KomisyonOran > 100)
                                                        {
                                                            disKaynakKomisyonOrani.KomisyonOran = disKaynakKomisyonOrani.KomisyonOran / 10;
                                                        }
                                                        pol.GenelBilgiler.Komisyon = (pol.GenelBilgiler.Komisyon * disKaynakKomisyonOrani.KomisyonOran) / 100;
                                                        pol.GenelBilgiler.UretimTaliAcenteKodu = Convert.ToInt32(model.TaliAcenteKoduOtomatik);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Zeyl ise Poliçesi onaylanmış mı kontrol ediliyor

                                        if (pol.GenelBilgiler.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (pol.GenelBilgiler.EkNo > 1 || pol.GenelBilgiler.BransKodu == BransListeCeviri.Dask)
                                            {
                                                if (pol.GenelBilgiler.EkNo.ToString().Length > 4)
                                                {
                                                    if (pol.GenelBilgiler.EkNo.ToString().Substring(4, 1) != "1")
                                                    {
                                                        var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                                        if (ZeylinPolicesi != null)
                                                        {
                                                            zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                                            zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                                        }
                                                    }
                                                }
                                                else if (pol.GenelBilgiler.EkNo > 1)
                                                {
                                                    var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                                    if (ZeylinPolicesi != null)
                                                    {
                                                        zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                                        zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pol.GenelBilgiler.EkNo > 0)
                                            {
                                                var ZeylinPolicesi = _PoliceService.getZeylinPolicesi(_AktifKullanici.TVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 0);
                                                if (ZeylinPolicesi != null)
                                                {
                                                    zeylinTaliKomisyonOrani = ZeylinPolicesi.TaliKomisyonOran;
                                                    zeylinPoliceTaliTVMKodu = ZeylinPolicesi.TaliAcenteKodu.Value;
                                                }
                                            }
                                        }

                                        if (zeylinPoliceTaliTVMKodu != 0 && zeylinTaliKomisyonOrani != null)
                                        {
                                            pol.GenelBilgiler.TaliAcenteKodu = zeylinPoliceTaliTVMKodu;
                                            pol.GenelBilgiler.TaliKomisyonOran = zeylinTaliKomisyonOrani;
                                            pol.GenelBilgiler.TaliKomisyon = (pol.GenelBilgiler.Komisyon * pol.GenelBilgiler.TaliKomisyonOran) / 100;
                                            pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = TurkeyDateTime.Now;
                                            pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                                        }
                                        #endregion

                                        if (zeylinPoliceTaliTVMKodu == 0 && taliTVMKodu != 0) // Poliçe ise ,Zeyl onaylanmamışsa ve tali acente kodu dolu ise
                                        {
                                            if (String.IsNullOrEmpty(model.TaliAcenteKoduOtomatik))
                                            {
                                                model.TaliAcenteKoduOtomatik = "0";
                                            }
                                            GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(taliTVMKodu, pol.GenelBilgiler.TanzimTarihi.Value.Year, pol.GenelBilgiler.BransKodu.Value);
                                            taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, taliTVMKodu, Convert.ToInt32(model.TaliAcenteKoduOtomatik), pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.BransKodu.Value);

                                            if (taliKomisyonOranlari != null)
                                            {
                                                taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                                if (taliKomisyonOranlari.Count() > 1)
                                                {
                                                    //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                                    decimal maxUertim = 0;
                                                    foreach (var item in taliKomisyonOranlari)
                                                    {
                                                        if (maxUertim < item.MaxUretim)
                                                        {
                                                            maxUertim = item.MaxUretim.Value;
                                                        }
                                                    }
                                                    GerceklesenTaliUretim += pol.GenelBilgiler.NetPrim ?? 0;
                                                    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                                                    // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                                    if (taliKomisyonOrani == null)
                                                    {
                                                        taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                                    }
                                                }
                                                //kademeli çalışmıyorsa
                                                else if (taliKomisyonOranlari.Count > 0)
                                                {
                                                    if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                                    {
                                                        taliKomisyonOrani = taliKomisyonOranlari[0];
                                                    }
                                                }
                                            }

                                            if (taliKomisyonOrani.KomisyonOran.HasValue)
                                            {
                                                pol.GenelBilgiler.TaliAcenteKodu = taliTVMKodu;
                                                if (taliKomisyonOrani.KomisyonOran > 100)
                                                {
                                                    taliKomisyonOrani.KomisyonOran = taliKomisyonOrani.KomisyonOran / 10;
                                                }
                                                pol.GenelBilgiler.TaliKomisyonOran = taliKomisyonOrani.KomisyonOran;
                                                pol.GenelBilgiler.TaliKomisyon = (pol.GenelBilgiler.Komisyon * pol.GenelBilgiler.TaliKomisyonOran) / 100;
                                                pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = TurkeyDateTime.Now;
                                                pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                                            }
                                        }
                                    }

                                    #region Eski Kodlar


                                    //if (taliTVMKodu > 0 && pol.GenelBilgiler.BransKodu != BransListeCeviri.TanimsizBransKodu)
                                    //{
                                    //    List<PoliceTaliAcenteler> policeTaliAcenteModel = new List<PoliceTaliAcenteler>();
                                    //    policeTaliAcenteModel = _TaliPoliceService.GetPoliceTaliAcenteList(taliTVMKodu, pol.GenelBilgiler.PoliceNumarasi, pol.GenelBilgiler.TUMBirlikKodu);
                                    //    if (policeTaliAcenteModel != null)
                                    //    {
                                    //        foreach (var item in policeTaliAcenteModel)
                                    //        {
                                    //            item.PoliceTransferEslestimi = 1;
                                    //            _TaliPoliceService.UpdatePoliceTaliAcenteler(item);
                                    //        }
                                    //    }
                                    //    if (String.IsNullOrEmpty(model.disKaynakKodu.ToString()))
                                    //    {
                                    //        model.disKaynakKodu = 0;
                                    //    }
                                    //    //poliçe transfer de branskodu belirlenemeyen (brans urun eşleşmesi yapılamayan) poliçelerede talikomisyon oranı ve talisi belirlenemiiyor. bu poliçelere tali atama
                                    //    //ve aynı zamanda komisyon oranı belirlenmesi komisyon hesapla ekranında yapılabilir.
                                    //    GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(taliTVMKodu, pol.GenelBilgiler.TanzimTarihi.Value.Year, pol.GenelBilgiler.BransKodu.Value);
                                    //    taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, taliTVMKodu, model.disKaynakKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.BransKodu.Value);

                                    //    pol.GenelBilgiler.TaliAcenteKodu = taliTVMKodu;

                                    //    taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    //    if (pol.GenelBilgiler.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    //    {
                                    //        if (pol.GenelBilgiler.EkNo > 1 || pol.GenelBilgiler.BransKodu == BransListeCeviri.Dask)
                                    //        {
                                    //            if (pol.GenelBilgiler.EkNo.ToString().Length > 4)
                                    //            {
                                    //                if (pol.GenelBilgiler.EkNo.ToString().Substring(4, 1) != "1")
                                    //                {
                                    //                    var ZeylinPolicesi = _PoliceService.getPolice(taliTVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                    //                    if (ZeylinPolicesi != null)
                                    //                    {
                                    //                        taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                    //                    }
                                    //                }
                                    //            }
                                    //            else
                                    //            {
                                    //                var ZeylinPolicesi = _PoliceService.getPolice(taliTVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 1);
                                    //                if (ZeylinPolicesi != null)
                                    //                {
                                    //                    taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        if (pol.GenelBilgiler.EkNo > 0)
                                    //        {
                                    //            taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    //            var ZeylinPolicesi = _PoliceService.getPolice(taliTVMKodu, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.PoliceNumarasi, 0);
                                    //            if (ZeylinPolicesi != null)
                                    //            {
                                    //                taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                    //            }
                                    //        }
                                    //    }

                                    //    if (taliKomisyonOranlari != null)
                                    //    {
                                    //        if (!taliKomisyonOrani.KomisyonOran.HasValue)
                                    //        {
                                    //            taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                    //            if (taliKomisyonOranlari.Count() > 1)
                                    //            {
                                    //                //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                    //                decimal maxUertim = 0;
                                    //                foreach (var item in taliKomisyonOranlari)
                                    //                {
                                    //                    if (maxUertim < item.MaxUretim)
                                    //                    {
                                    //                        maxUertim = item.MaxUretim.Value;
                                    //                    }
                                    //                }
                                    //                GerceklesenTaliUretim += pol.GenelBilgiler.NetPrim ?? 0;
                                    //                taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                                    //                // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                    //                if (taliKomisyonOrani == null)
                                    //                {
                                    //                    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                    //                }
                                    //            }
                                    //            //kademeli çalışmıyorsa
                                    //            else if (taliKomisyonOranlari.Count > 0)
                                    //            {
                                    //                if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                    //                {
                                    //                    taliKomisyonOrani = taliKomisyonOranlari[0];
                                    //                }
                                    //            }
                                    //        }

                                    //        if (disUretim)
                                    //        {
                                    //            pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = taliKomisyonOrani.GuncellemeTarihi;
                                    //            pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = taliKomisyonOrani.GuncellemeKullaniciKodu;
                                    //            pol.GenelBilgiler.Komisyon = (pol.GenelBilgiler.Komisyon * taliKomisyonOrani.KomisyonOran) / 100;
                                    //            pol.GenelBilgiler.UretimTaliAcenteKodu = Convert.ToInt32(model.TaliAcenteKodu);
                                    //            //pol.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(model.TaliAcenteKodu);
                                    //        }
                                    //        else
                                    //        {
                                    //            pol.GenelBilgiler.TaliKomisyonOran = taliKomisyonOrani.KomisyonOran;
                                    //            pol.GenelBilgiler.TaliKomisyonGuncellemeTarihi = taliKomisyonOrani.GuncellemeTarihi;
                                    //            pol.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = taliKomisyonOrani.GuncellemeKullaniciKodu;
                                    //            pol.GenelBilgiler.TaliKomisyon = (pol.GenelBilgiler.Komisyon * pol.GenelBilgiler.TaliKomisyonOran) / 100;
                                    //        }
                                    //    }
                                    //}
                                    #endregion

                                }


                            }
                            if (policeler != null)
                            {
                                _PoliceService.Add(policeler);
                                model.updateKayitlar = _PoliceService.getUpdateKayit();
                                model.basariliKayitlar = _PoliceService.getBasariliKayitlar();
                                model.varOlanKayitlar = _PoliceService.getVarolanKayitlar();
                                model.hataliEklenmeyenKayitlar = _PoliceService.getHataliEklenmeyenKayitlar();
                                return Json(new { Success = true, updateKayit = model.updateKayitlar, BasariliKayit = model.basariliKayitlar, BasarisizKayit = model.varOlanKayitlar, HataliEklenmeyenKayitlar = model.hataliEklenmeyenKayitlar, toplamPoliceSayisi = policeler.Count() });
                            }
                        }
                        else
                        {
                            string hataMesaji = _PoliceTransferService.getMessage();
                            if (hataMesaji != null)
                            {
                                return Json(new { Success = false, Mesaj = hataMesaji });
                            }
                            else
                            {
                                var responseXML = webServisURL + "AUser=" + servisKullanici.KullaniciAdi + "&Pwd=" + servisKullanici.Sifre + "&BasTar=" + model.TanzimBaslangicTarihi.ToString("yyyyMMdd") + "&BitTar=" + model.TanzimBitisTarihi.ToString("yyyyMMdd");
                                return Json(new { Success = false, Mesaj = "Poliçe transferi yapılırken bir hata oluştu. \n" + responseXML });
                            }
                        }

                    }
                    else
                    {
                        string TahsilatMesaj = TahsilatKapamaResultMesaj;
                        return Json(new { Success = true, Mesaj = TahsilatMesaj });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        public ActionResult NeoOnlinePoliceTransfer()
        {
            GencTransferKayitModel model = new GencTransferKayitModel();

            model.TaliAcenteler = this.PoliceTransferTaliAcenteler;

            return View(model);
        }

        [HttpPost]
        public ActionResult NeoOnlinePoliceTransfer(GencTransferKayitModel model, HttpPostedFileBase file)
        {

            model.TaliAcenteler = this.PoliceTransferTaliAcenteler;
            string path = String.Empty;
            //List<Business.Police> policeler = null;

            GencListeler listeler = null;


            path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
            file.SaveAs(path);

            // policeler = _PoliceTransferService.gencPoliceler(path);

            listeler = _PoliceTransferService.gencPoliceler(path);

            #region Dış Kaynak Komisyonu var ise

            TaliAcenteKomisyonOrani disKaynakKomisyonOrani;
            List<TaliAcenteKomisyonOrani> disKaynakKomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
            bool disUretim = false;
            if (!String.IsNullOrEmpty(model.TaliAcenteKodu))
            {
                //disUretim = this.UzerindenPoliceTransferYapiliyorMu(Convert.ToInt32(model.taliTvmKodu));
                disUretim = true;
            }



            if (disUretim)
            {
                foreach (var pol in listeler.policeListesi)
                {
                    disKaynakKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullanici.TVMKodu, Convert.ToInt32(model.TaliAcenteKodu), 0, pol.GenelBilgiler.TUMBirlikKodu, pol.GenelBilgiler.BransKodu.Value);

                    if (disKaynakKomisyonOranlari != null && disKaynakKomisyonOranlari.Count > 0)
                    {
                        disKaynakKomisyonOrani = new TaliAcenteKomisyonOrani();
                        if (disKaynakKomisyonOranlari.Count > 0)
                        {
                            disKaynakKomisyonOrani = disKaynakKomisyonOranlari[0];
                        }
                        if (disKaynakKomisyonOrani != null)
                        {
                            if (disKaynakKomisyonOrani.KomisyonOran != null)
                            {
                                if (disKaynakKomisyonOrani.KomisyonOran > 100)
                                {
                                    disKaynakKomisyonOrani.KomisyonOran = disKaynakKomisyonOrani.KomisyonOran / 10;
                                }
                                pol.GenelBilgiler.Komisyon = (pol.GenelBilgiler.Komisyon * disKaynakKomisyonOrani.KomisyonOran) / 100;
                                pol.GenelBilgiler.UretimTaliAcenteKodu = Convert.ToInt32(model.TaliAcenteKodu);
                            }
                        }
                    }
                }



            }
            #endregion

            if (listeler != null)
            {
                _PoliceService.GencPolicelerAdd(listeler);
                model.basariliKayitlar = _PoliceService.getBasariliKayitlar();
                model.basarisizKayitlar = _PoliceService.getVarolanKayitlar();
                model.EklenmeyenPoliceler = _PoliceService.getEklenmeyenPoliceler();
                model.ToplamPoliceSayisi = listeler.policeListesi.Count();
            }
            //taliModel.basariliKayitlar = _TaliAcenteTransferService.getBasariliKayitlar();
            //taliModel.basarisizKayitlar = _TaliAcenteTransferService.getBasarisizKayitlar();
            //  return Json(new { Success = true, BasariliKayit = taliModel.basariliKayitlar, BasarisizKayit = taliModel.basarisizKayitlar });

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.PoliceTransferDosyaGoruntule, SekmeKodu = 0)]
        public ActionResult PoliceTransferDosyaGoruntuleme()
        {
            PoliceTransferGoruntulemeModel model = new PoliceTransferGoruntulemeModel();

            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList().Where(s => s.SirketKodu == SigortaSirketiBirlikKodlari.HDISIGORTA
                || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA
                || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREDASK
                || s.SirketKodu == SigortaSirketiBirlikKodlari.AXASIGORTA).ToList();
            model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");


            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.PoliceTransferDosyaGoruntule, SekmeKodu = 0)]
        public ActionResult PoliceTransferDosyaGoruntuleme(PoliceTransferGoruntulemeModel model)
        {
            #region multiselects / sirketler

            if (model.SigortaSirketleriSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.SigortaSirketleriSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.SigortaSirket = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.SigortaSirket = model.SigortaSirket + liste[i] + ",";
                    else model.SigortaSirket = model.SigortaSirket + liste[i];
                }
            }

            #endregion
            var policeXMLDosyasiList = _PoliceTransferService.AutoPoliceTransferGetir(model.TanzimBaslangicTarihi, model.TanzimBitisTarihi, model.SigortaSirket, _AktifKullanici.TVMKodu);
            model.list = new List<AutoPoliceTransferModel>();
            AutoPoliceTransferModel polModel = new AutoPoliceTransferModel();
            if (policeXMLDosyasiList != null)
            {
                foreach (var items in policeXMLDosyasiList)
                {
                    polModel = new AutoPoliceTransferModel();
                    polModel.TVMKodu = items.TvmKodu;
                    polModel.TVMUnvan = items.Unvani;
                    polModel.SirketKodu = items.SirketKodu;
                    polModel.SirketUnvani = items.SirketAdi;
                    polModel.TanzimBaslangicTarihi = items.TanzimBaslangicTarihi;
                    polModel.TanzimBitisTarihi = items.TanzimBitisTarihi;
                    polModel.KayitTarihi = items.KayitTarihi;
                    polModel.KaydiEkleyenKullaniciKodu = items.KaydiEkleyenKullaniciKodu;
                    polModel.KaydiEkleyenKullaniciUnvan = items.Adi + " " + items.Soyadi;
                    polModel.PoliceTransferUrl = items.PoliceTransferUrl;
                    model.list.Add(polModel);

                }
                model.list.OrderByDescending(s => s.SirketUnvani);
            }

            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList().Where(s => s.SirketKodu == SigortaSirketiBirlikKodlari.HDISIGORTA
                || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA
                || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREDASK
                || s.SirketKodu == SigortaSirketiBirlikKodlari.AXASIGORTA).ToList();
            model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");


            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OtomatikPoliceOnaylama, SekmeKodu = 0)]
        public ActionResult OtomatikPoliceOnaylama()
        {
            OtomatikPoliceOnayModel model = new OtomatikPoliceOnayModel();
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OtomatikPoliceOnaylama, SekmeKodu = 0)]
        public ActionResult OtomatikPoliceOnaylama(HttpPostedFileBase file)
        {
            try
            {
                OtomatikPoliceOnayModel model = new OtomatikPoliceOnayModel();
                PoliceOnaySonucModels PoliceOnaySonuc = new PoliceOnaySonucModels();
                if (ModelState.IsValid && file != null && file.ContentLength > 0)
                {
                    string path = String.Empty;

                    path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
                    file.SaveAs(path);
                    OtomatikPoliceOnaylamaReader reader = new OtomatikPoliceOnaylamaReader(path);

                    var OnaylananPoliceler = reader.PoliceleriOnayla();
                    model.toplamOkunanKayit = OnaylananPoliceler.Count;
                    if (OnaylananPoliceler.Count > 0)
                    {
                        foreach (var item in OnaylananPoliceler)
                        {
                            if (!String.IsNullOrEmpty(item.GenelHataMesaji))
                            {
                                model.genelHataMesaji = item.GenelHataMesaji;
                            }
                            else if (!item.GuncellemeBasarili)
                            {
                                PoliceOnaySonuc = new PoliceOnaySonucModels();
                                PoliceOnaySonuc.SigortaSirketKodu = item.SigortaSirketKodu;
                                PoliceOnaySonuc.SigortaSirketUnvani = item.SigortaSirketUnvani;
                                PoliceOnaySonuc.TaliAcenteKodu = item.TaliAcenteKodu;
                                PoliceOnaySonuc.TaliAcenteUnvani = item.TaliAcenteUnvani;
                                PoliceOnaySonuc.YenilemeNumarasi = item.YenilemeNumarasi;
                                PoliceOnaySonuc.EkNumarasi = item.EkNumarasi;
                                PoliceOnaySonuc.PoliceNumarasi = item.PoliceNumarasi;
                                PoliceOnaySonuc.GuncellemeBasarili = item.GuncellemeBasarili;
                                PoliceOnaySonuc.BilgiMesaji = item.BilgiMesaji;
                                model.HataliPoliceListesi.Add(PoliceOnaySonuc);
                                model.hataliKayit++;
                            }
                            else
                            {
                                model.basariliKayit++;
                            }
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public bool UzerindenPoliceTransferYapiliyorMu(int tvmKodu)
        {
            var tvmDetay = _TVMService.GetDetay(tvmKodu);
            bool AltKaynakTransfer = false;
            if (tvmDetay != null)
            {
                if (tvmDetay.Tipi == TVMTipleri.UzerindenIsYapilanACente && tvmDetay.PoliceTransfer == 1)
                {
                    AltKaynakTransfer = true;
                }
            }
            return AltKaynakTransfer;
        }

        private List<SelectListItem> _SigortaSirketleri;
        protected List<SelectListItem> SigortaSirketleri
        {
            get
            {
                if (_SigortaSirketleri == null)
                {
                    ISigortaSirketleriService _SigortaSirketService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
                    List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketService.GetList();

                    _SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
                }

                return _SigortaSirketleri;
            }
        }

        private List<SelectListItem> _AutoPoliceTransferSirketleri;
        protected List<SelectListItem> AutoPoliceTransferSirketleri
        {
            get
            {
                if (_AutoPoliceTransferSirketleri == null)
                {
                    ISigortaSirketleriService _SigortaSirketService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
                    List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketService.GetList().Where(s => s.SirketKodu == SigortaSirketiBirlikKodlari.HDISIGORTA
                        || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA
                           || s.SirketKodu == SigortaSirketiBirlikKodlari.MAPFREDASK
                        || s.SirketKodu == SigortaSirketiBirlikKodlari.AXASIGORTA
                        || s.SirketKodu == SigortaSirketiBirlikKodlari.SSDOGASİGORTAKOOPERATİF
                        || s.SirketKodu == SigortaSirketiBirlikKodlari.ETHICA
                        || s.SirketKodu == SigortaSirketiBirlikKodlari.RAYSIGORTA
                        ).ToList();

                    _AutoPoliceTransferSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
                }

                return _AutoPoliceTransferSirketleri;
            }
        }

        private List<SelectListItem> _PoliceTransferTaliAcenteler;
        protected List<SelectListItem> PoliceTransferTaliAcenteler
        {
            get
            {
                if (_PoliceTransferTaliAcenteler == null)
                {
                    List<TVMDetay> taliTvmler = _TVMService.GetListTVMDetayPoliceTransferTali();

                    _PoliceTransferTaliAcenteler = new SelectList(taliTvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                }

                return _PoliceTransferTaliAcenteler;
            }
        }
        public string getSirketServiceURL(string sigortaSirketKodu, string transferTipi, byte axaPoliceTipi)
        {
            string webServisURL = "";
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA)
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                if (transferTipi == PoliceTransferTipi.PoliceTransfer)
                {
                    webServisURL = konfig[Konfig.MAPFRE_PoliceTransferURL];
                }
                else
                {
                    webServisURL = konfig[Konfig.MAPFRE_TahsilatPoliceTransferURL];
                }
            }
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.MAPFREDASK)
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                if (transferTipi == PoliceTransferTipi.PoliceTransfer)
                {
                    webServisURL = konfig[Konfig.MAPFREDASK_OtomatikPoliceTransferURL];
                }

            }
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.HDISIGORTA)
            {

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIPlaka);
                if (transferTipi == "0")
                    webServisURL = konfig[Konfig.HDI_PoliceTransferServiceURL];
                else
                    webServisURL = konfig[Konfig.HDI_TahsilatKapatmaServiceURL];

            }
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.AXASIGORTA)
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);
                if (axaPoliceTipi == 0)
                {
                    webServisURL = konfig[Konfig.AXA_Oto_Pol_Transfer];
                }
                else if (axaPoliceTipi == 1)
                {
                    webServisURL = konfig[Konfig.AxaOtoPolHayatTransfer];
                }
            }
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.SSDOGASİGORTAKOOPERATİF)
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleDogaPolTransfer);
                webServisURL = konfig[Konfig.Doga_PolTransferServiceURL];
            }
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.ETHICA)
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEthicaPolTransfer);
                webServisURL = konfig[Konfig.ETHICA_ServiceURL];
            }
            return webServisURL;
        }
    }
}
