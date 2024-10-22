using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = 0, SekmeKodu = 0)]
    public class MuhasebeController : Controller
    {
        IAktifKullaniciService _AktifKullaniciService;
        ITVMService _TVMService;
        IPoliceService _PoliceService;
        IBransService _BransService;
        IMuhasebe_CariHesapService _Muhasebe_CariHesapService;
        IUlkeService _UlkeService;
        ISigortaSirketleriService _SigortaSirketleriService;
        ICommonService _CommonService;
        public MuhasebeController(IAktifKullaniciService aktifKullaniciService,
            ITVMService tvmService,
            IPoliceService policeService,
            IBransService bransService,
            ISigortaSirketleriService sigortaSirketleriService,
            IMuhasebe_CariHesapService muhasebe_CariHesapService,
            IUlkeService ulkeService,
            ICommonService commonService)
        {
            _AktifKullaniciService = aktifKullaniciService;
            _BransService = bransService;
            _TVMService = tvmService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _PoliceService = policeService;
            _Muhasebe_CariHesapService = muhasebe_CariHesapService;
            _UlkeService = ulkeService;
            _CommonService = commonService;
        }

        #region Cari Hesap Ekstre Listesi
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceTahsilatEkstresi, SekmeKodu = 0)]
        public ActionResult HesapEkstre()
        {
            HesapEkstresiAraModel model = new HesapEkstresiAraModel();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();

            model.list = new List<HesapEkstresiModel>();
            //model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-30);
            //model.BitisTarihi =TurkeyDateTime.Now;

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 2010; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
            });
            }

            model.Yillar = new SelectList(listYillar, "Value", "Text", "0").ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();

            listAylar.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "01", Text = "01" },
                new SelectListItem() { Value = "02", Text = "02" },
                new SelectListItem() { Value = "03", Text = "03" },
                new SelectListItem() { Value = "04", Text = "04" },
                new SelectListItem() { Value = "05", Text = "05" },
                new SelectListItem() { Value = "06", Text = "06" },
                new SelectListItem() { Value = "07", Text = "07" },
                new SelectListItem() { Value = "08", Text = "08" },
                new SelectListItem() { Value = "09", Text = "09" },
                new SelectListItem() { Value = "10", Text = "10" },
                new SelectListItem() { Value = "11", Text = "11" },
                new SelectListItem() { Value = "12", Text = "12" },
            });

            model.Aylar = new SelectList(listAylar, "Value", "Text").ToList();

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            //durum: 0 şahıs 1 firma
            model.Durumlar = new SelectList(DurumListesiAktifPasif.FirmamiSahismi(), "Value", "Text", "0");
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;
            }
            else
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceTahsilatEkstresi, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult HesapEkstre(HesapEkstresiAraModel model)
        {
            model.list = new List<HesapEkstresiModel>();
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.FirmamiSahismi(), "Value", "Text", "0");
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 2010; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Yillar = new SelectList(listYillar, "Value", "Text", "0").ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            listAylar.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "01", Text = "01" },
                new SelectListItem() { Value = "02", Text = "02" },
                new SelectListItem() { Value = "03", Text = "03" },
                new SelectListItem() { Value = "04", Text = "04" },
                new SelectListItem() { Value = "05", Text = "05" },
                new SelectListItem() { Value = "06", Text = "06" },
                new SelectListItem() { Value = "07", Text = "07" },
                new SelectListItem() { Value = "08", Text = "08" },
                new SelectListItem() { Value = "09", Text = "09" },
                new SelectListItem() { Value = "10", Text = "10" },
                new SelectListItem() { Value = "11", Text = "11" },
                new SelectListItem() { Value = "12", Text = "12" },
            });
            model.Aylar = new SelectList(listAylar, "Value", "Text").ToList();

            decimal toplamOdenenTutar = 0;
            decimal toplamBorc = 0;
            decimal devredenBakiye = 0;
            decimal? yuruyenBakiye = 0;
            HesapEkstresiModel sonucModel;
            int devirYil = model.Yil - 1;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;

                #region tcvkno
                if (model.TcknVkn != null)
                {
                    int kimlikNoSayac = 0;
                    model.TcknVkn = model.TcknVkn.Trim();
                    var policetcvkn = _Muhasebe_CariHesapService.MuhasebeCariHesapTcVknAra(model.TcknVkn, _AktifKullaniciService.TVMKodu, model.Yil);

                    var devirpolicetcvkn = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapTcVknAra(model.TcknVkn, _AktifKullaniciService.TVMKodu, devirYil);
                    if (devirpolicetcvkn != null)
                    {
                        foreach (var item in devirpolicetcvkn.TahsilatTckVn)
                        {
                            sonucModel = new HesapEkstresiModel();
                            toplamOdenenTutar += item.OdenenTutar;
                            toplamBorc += item.TaksitTutari;
                        }
                        devredenBakiye = toplamBorc - toplamOdenenTutar;

                    }

                    //var tahsilatList = policetcvkn.TahsilatTckVn.Where(j => j.TaksitVadeTarihi.Year == model.Yil).ToList();
                    yuruyenBakiye += devredenBakiye;
                    if (policetcvkn.TahsilatTckVn.Count > 0)
                    {
                        foreach (var item in policetcvkn.TahsilatTckVn)
                        {
                            sonucModel = new HesapEkstresiModel();
                            sonucModel.PoliceId = item.PoliceId;
                            //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                            //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                            //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                            sonucModel.SigortaliUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                            sonucModel.YenilemeNo = item.PoliceGenel.YenilemeNo.HasValue ? item.PoliceGenel.YenilemeNo.Value.ToString() : null;
                            sonucModel.BransAdi = item.PoliceGenel.BransAdi;
                            if (item.PoliceGenel.BransKodu != null)
                            {
                                sonucModel.BransKodu = item.PoliceGenel.BransKodu.Value;
                            }
                            sonucModel.UrunAdi = item.PoliceGenel.TUMUrunAdi;
                            sonucModel.Borc = item.TaksitTutari;
                            sonucModel.Alacak = item.OdenenTutar;
                            sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                            sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                            yuruyenBakiye += sonucModel.ToplamBakiye;
                            sonucModel.ToplamBakiye = yuruyenBakiye;
                            sonucModel.DevirAlacak = toplamOdenenTutar;
                            sonucModel.DevirBorc = toplamBorc;
                            sonucModel.VadeTarihi = item.TaksitVadeTarihi;
                            sonucModel.EvrakNo = item.OdemeBelgeNo;
                            if (sonucModel.Borc >= sonucModel.Alacak)
                            {
                                sonucModel.BorcTipi = "B";
                            }
                            else if (sonucModel.Alacak < 0)
                            {
                                sonucModel.BorcTipi = "A";
                            }

                            sonucModel.Devir = devredenBakiye;
                            if (item.OdemeBelgeTarihi != null)
                            {
                                sonucModel.IslemTarih = item.OdemeBelgeTarihi.Value;

                            }
                            if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                            {
                                sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + item.PoliceGenel.BransAdi + "- " + item.PoliceGenel.PoliceSigortali.AdiUnvan + " " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                        " -" + item.PoliceGenel.PoliceArac.PlakaKodu + " " + item.PoliceGenel.PoliceArac.PlakaNo;
                            }
                            else
                            {
                                sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + item.PoliceGenel.BransAdi + " - " + item.PoliceGenel.PoliceSigortali.AdiUnvan + "  " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                    " - " + item.PoliceGenel.PoliceSigortali.IlAdi + " / " + item.PoliceGenel.PoliceSigortali.IlceAdi;
                            }
                            // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                            if (item.PoliceGenel.TaliAcenteKodu != null)
                            {
                                sonucModel.TaliAcenteKodu = item.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                var taliTvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value);
                                sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                            }
                            if (item.PoliceGenel.UretimTaliAcenteKodu != null)
                            {
                                sonucModel.DisKaynakKodu = item.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                var disKaynakUnvan = _TvmService.GetDetay(item.PoliceGenel.UretimTaliAcenteKodu.Value);
                                sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                            }
                            sonucModel.VerilenKomisyon = item.PoliceGenel.TaliKomisyon;
                            sonucModel.BaslangicTarihi = item.PoliceGenel.BaslangicTarihi;
                            if (item.PoliceGenel.BitisTarihi != null)
                            {
                                sonucModel.BitisTarihi = item.PoliceGenel.BitisTarihi.Value.Date;
                            }

                            sonucModel.TanzimTarihi = item.PoliceGenel.TanzimTarihi.Value.Date;
                            sonucModel.BrütPrim = item.PoliceGenel.BrutPrim != null ? item.PoliceGenel.BrutPrim : 0;
                            sonucModel.EkNo = item.PoliceGenel.EkNo.HasValue ? item.PoliceGenel.EkNo.Value.ToString() : null;
                            sonucModel.Komisyon = item.PoliceGenel.Komisyon;
                            sonucModel.NetPrim = item.PoliceGenel.NetPrim;
                            sonucModel.OdemeSekli = item.PoliceGenel.OdemeSekli.HasValue ? item.PoliceGenel.OdemeSekli.Value.ToString() : null;
                            sonucModel.SigortaSirketi = item.PoliceGenel.SigortaSirketleri.SirketAdi;
                            sonucModel.PoliceNo = item.PoliceGenel.PoliceNumarasi;
                            sonucModel.TaksitSayisi = policetcvkn.TahsilatTckVn.Find(s => s.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis.Count();
                            sonucModel.ParaBirimi = item.PoliceGenel.ParaBirimi;
                            if (sonucModel.OdemeSekli == "1")
                            {
                                sonucModel.OdemeSekli = "Peşin";
                            }
                            else
                            {
                                sonucModel.OdemeSekli = "Taksit";
                            }
                            foreach (var items in policetcvkn.TahsilatTckVn.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis)
                            {
                                sonucModel.OdemeTipim = items.OdemeTipi.ToString();
                            }
                            if (item.OdemTipi == 1)
                            {
                                sonucModel.IslemTipi = "NK";
                            }
                            if (item.OdemTipi == 2)
                            {
                                sonucModel.IslemTipi = "Müşteri KK";
                            }
                            if (item.OdemTipi == 3)
                            {
                                sonucModel.IslemTipi = "HVL";
                            }
                            if (item.OdemTipi == 4)
                            {
                                sonucModel.IslemTipi = "ÇEK";
                            }
                            if (item.OdemTipi == 5)
                            {
                                sonucModel.IslemTipi = "Acente KK";
                            }
                            if (item.OdemTipi == 6)
                            {
                                sonucModel.IslemTipi = "Acente Pos";
                            }
                            if (item.OdemTipi == 7)
                            {
                                sonucModel.IslemTipi = "SENET";
                            }
                            if (item.OdemTipi == 9)
                            {
                                sonucModel.IslemTipi = "Acente B. KK";
                            }
                            if (item.OdemTipi == 0)
                            {
                                sonucModel.IslemTipi = "HVL";
                            }
                            ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(item.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? item.PoliceGenel.PoliceSigortaEttiren.KimlikNo : item.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                            ViewBag.SigortaEttirenUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                            ViewBag.Donemm = model.Yil;
                            model.list.Add(sonucModel);
                        }
                    }
                    //else
                    //{
                    //    sonucModel = new HesapEkstresiModel();
                    //    sonucModel.Devir = devredenBakiye;
                    //    sonucModel.DevirAlacak = toplamOdenenTutar;
                    //    sonucModel.DevirBorc = toplamBorc;
                    //    model.list.Add(sonucModel);

                    //}
                }
                #endregion

                #region odemeBelgeNo
                if (model.OdemeBelgeNo != null)
                {
                    int odemebelgeNoSayac = 0;
                    model.OdemeBelgeNo = model.OdemeBelgeNo.Trim();
                    var policetcvkn = _Muhasebe_CariHesapService.MuhasebeCariHesapOdemeBelgeNoAra(model.OdemeBelgeNo, _AktifKullaniciService.TVMKodu, model.Yil);

                    var devirpolicetcvkn = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapOdemeBelgeNoAra(model.OdemeBelgeNo, _AktifKullaniciService.TVMKodu, devirYil);
                    if (devirpolicetcvkn != null)
                    {
                        foreach (var item in devirpolicetcvkn.TahsilatTckVn)
                        {
                            sonucModel = new HesapEkstresiModel();
                            toplamOdenenTutar += item.OdenenTutar;
                            toplamBorc += item.TaksitTutari;
                        }
                        devredenBakiye = toplamBorc - toplamOdenenTutar;

                    }
                    //var tahsilatList = policetcvkn.TahsilatTckVn.Where(j => j.TaksitVadeTarihi.Year == model.Yil).ToList();
                    yuruyenBakiye += devredenBakiye;
                    if (policetcvkn.TahsilatTckVn.Count > 0)
                    {
                        foreach (var item in policetcvkn.TahsilatTckVn)
                        {
                            sonucModel = new HesapEkstresiModel();
                            sonucModel.PoliceId = item.PoliceId;
                            //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                            //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                            //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                            sonucModel.SigortaliUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                            sonucModel.YenilemeNo = item.PoliceGenel.YenilemeNo.HasValue ? item.PoliceGenel.YenilemeNo.Value.ToString() : null;
                            sonucModel.BransAdi = item.PoliceGenel.BransAdi;
                            if (item.PoliceGenel.BransKodu != null)
                            {
                                sonucModel.BransKodu = item.PoliceGenel.BransKodu.Value;
                            }
                            sonucModel.UrunAdi = item.PoliceGenel.TUMUrunAdi;
                            sonucModel.Borc = item.TaksitTutari;
                            sonucModel.Alacak = item.OdenenTutar;
                            sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                            sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                            yuruyenBakiye += sonucModel.ToplamBakiye;
                            sonucModel.ToplamBakiye = yuruyenBakiye;
                            sonucModel.DevirAlacak = toplamOdenenTutar;
                            sonucModel.DevirBorc = toplamBorc;
                            sonucModel.VadeTarihi = item.TaksitVadeTarihi;
                            sonucModel.EvrakNo = item.OdemeBelgeNo;
                            if (sonucModel.Borc >= sonucModel.Alacak)
                            {
                                sonucModel.BorcTipi = "B";
                            }
                            else if (sonucModel.Alacak < 0)
                            {
                                sonucModel.BorcTipi = "A";
                            }

                            sonucModel.Devir = devredenBakiye;
                            if (item.OdemeBelgeTarihi != null)
                            {
                                sonucModel.IslemTarih = item.OdemeBelgeTarihi.Value;

                            }
                            if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                            {
                                sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + item.PoliceGenel.BransAdi + "- " + item.PoliceGenel.PoliceSigortali.AdiUnvan + " " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                        " -" + item.PoliceGenel.PoliceArac.PlakaKodu + " " + item.PoliceGenel.PoliceArac.PlakaNo;
                            }
                            else
                            {
                                sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + item.PoliceGenel.BransAdi + " - " + item.PoliceGenel.PoliceSigortali.AdiUnvan + "  " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                    " - " + item.PoliceGenel.PoliceSigortali.IlAdi + " / " + item.PoliceGenel.PoliceSigortali.IlceAdi;
                            }
                            // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                            if (item.PoliceGenel.TaliAcenteKodu != null)
                            {
                                sonucModel.TaliAcenteKodu = item.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                var taliTvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value);
                                sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                            }
                            if (item.PoliceGenel.UretimTaliAcenteKodu != null)
                            {
                                sonucModel.DisKaynakKodu = item.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                var disKaynakUnvan = _TvmService.GetDetay(item.PoliceGenel.UretimTaliAcenteKodu.Value);
                                sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                            }
                            sonucModel.VerilenKomisyon = item.PoliceGenel.TaliKomisyon;
                            sonucModel.BaslangicTarihi = item.PoliceGenel.BaslangicTarihi;
                            if (item.PoliceGenel.BitisTarihi != null)
                            {
                                sonucModel.BitisTarihi = item.PoliceGenel.BitisTarihi.Value.Date;
                            }

                            sonucModel.TanzimTarihi = item.PoliceGenel.TanzimTarihi.Value.Date;
                            sonucModel.BrütPrim = item.PoliceGenel.BrutPrim != null ? item.PoliceGenel.BrutPrim : 0;
                            sonucModel.EkNo = item.PoliceGenel.EkNo.HasValue ? item.PoliceGenel.EkNo.Value.ToString() : null;
                            sonucModel.Komisyon = item.PoliceGenel.Komisyon;
                            sonucModel.NetPrim = item.PoliceGenel.NetPrim;
                            sonucModel.OdemeSekli = item.PoliceGenel.OdemeSekli.HasValue ? item.PoliceGenel.OdemeSekli.Value.ToString() : null;
                            sonucModel.SigortaSirketi = item.PoliceGenel.SigortaSirketleri.SirketAdi;
                            sonucModel.PoliceNo = item.PoliceGenel.PoliceNumarasi;
                            sonucModel.TaksitSayisi = policetcvkn.TahsilatTckVn.Find(s => s.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis.Count();
                            sonucModel.ParaBirimi = item.PoliceGenel.ParaBirimi;
                            if (sonucModel.OdemeSekli == "1")
                            {
                                sonucModel.OdemeSekli = "Peşin";
                            }
                            else
                            {
                                sonucModel.OdemeSekli = "Taksit";
                            }
                            foreach (var items in policetcvkn.TahsilatTckVn.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis)
                            {
                                sonucModel.OdemeTipim = items.OdemeTipi.ToString();
                            }
                            if (item.OdemTipi == 1)
                            {
                                sonucModel.IslemTipi = "NK";
                            }
                            if (item.OdemTipi == 2)
                            {
                                sonucModel.IslemTipi = "Müşteri KK";
                            }
                            if (item.OdemTipi == 3)
                            {
                                sonucModel.IslemTipi = "HVL";
                            }
                            if (item.OdemTipi == 4)
                            {
                                sonucModel.IslemTipi = "ÇEK";
                            }
                            if (item.OdemTipi == 5)
                            {
                                sonucModel.IslemTipi = "Acente KK";
                            }
                            if (item.OdemTipi == 6)
                            {
                                sonucModel.IslemTipi = "Acente Pos";
                            }
                            if (item.OdemTipi == 7)
                            {
                                sonucModel.IslemTipi = "SENET";
                            }
                            if (item.OdemTipi == 9)
                            {
                                sonucModel.IslemTipi = "Acente B. KK";
                            }
                            if (item.OdemTipi == 0)
                            {
                                sonucModel.IslemTipi = "HVL";
                            }
                            ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(item.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? item.PoliceGenel.PoliceSigortaEttiren.KimlikNo : item.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                            ViewBag.SigortaEttirenUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                            ViewBag.Donemm = model.Yil;
                            model.list.Add(sonucModel);
                        }
                    }
                    //else
                    //{
                    //    sonucModel = new HesapEkstresiModel();
                    //    sonucModel.Devir = devredenBakiye;
                    //    sonucModel.DevirAlacak = toplamOdenenTutar;
                    //    sonucModel.DevirBorc = toplamBorc;
                    //    model.list.Add(sonucModel);

                    //}
                }
                #endregion


                #region s.ettiren unvanı
                if (model.Durum == 0)
                {
                    if (model.Unvan != null && model.UnvanSoyad != null)
                    {
                        model.Unvan = model.Unvan.Trim();
                        model.UnvanSoyad = model.UnvanSoyad.Trim();
                        var muhasebeSahisUnvan = _Muhasebe_CariHesapService.MuhasebeCariHesapUnvan(model.Unvan, model.UnvanSoyad, _AktifKullaniciService.TVMKodu, model.Durum, model.Yil);
                        //var devirmuhasebeSahisUnvan = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapUnvan(model.Unvan, model.UnvanSoyad, _AktifKullaniciService.TVMKodu, model.Durum);
                        if (muhasebeSahisUnvan != null)
                        {
                            foreach (var item in muhasebeSahisUnvan.OfflineSigortaEttiren)
                            {
                                sonucModel = new HesapEkstresiModel();
                                sonucModel.PoliceId = item.PoliceId;

                                sonucModel.KimlikNo = item.KimlikNo;
                                var polBilgiBul = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, devirYil);

                                foreach (var items in polBilgiBul.TahsilatTckVn)
                                {
                                    toplamOdenenTutar += items.OdenenTutar;
                                    toplamBorc += items.TaksitTutari;
                                }
                                devredenBakiye = toplamBorc - toplamOdenenTutar;

                            }

                        }
                        yuruyenBakiye += devredenBakiye;
                        if (muhasebeSahisUnvan != null)
                        {

                            foreach (var item in muhasebeSahisUnvan.OfflineSigortaEttiren)
                            {
                                sonucModel = new HesapEkstresiModel();
                                var tahsilats = muhasebeSahisUnvan.OfflineSigortaEttiren.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceTahsilats;
                                var tahsilatList = tahsilats.Where(m => m.TaksitVadeTarihi.Year == model.Yil).ToList();

                                sonucModel.KimlikNo = item.KimlikNo;
                                var polBilgiBul = _Muhasebe_CariHesapService.MuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, model.Yil);
                                foreach (var items in tahsilatList)
                                {
                                    sonucModel = new HesapEkstresiModel();
                                    sonucModel.PoliceId = item.PoliceId;
                                    //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                                    //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                                    //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                                    sonucModel.SigortaliUnvani = item.PoliceGenel.PoliceSigortali.AdiUnvan + " " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan;
                                    sonucModel.YenilemeNo = item.PoliceGenel.YenilemeNo.HasValue ? item.PoliceGenel.YenilemeNo.Value.ToString() : null;
                                    sonucModel.BransAdi = item.PoliceGenel.BransAdi;
                                    if (item.PoliceGenel.BransKodu != null)
                                    {
                                        sonucModel.BransKodu = item.PoliceGenel.BransKodu.Value;
                                    }
                                    sonucModel.UrunAdi = item.PoliceGenel.TUMUrunAdi;
                                    sonucModel.Borc = items.TaksitTutari;
                                    sonucModel.Alacak = items.OdenenTutar;
                                    sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                                    sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                                    yuruyenBakiye += sonucModel.ToplamBakiye;
                                    sonucModel.ToplamBakiye = yuruyenBakiye;
                                    sonucModel.VadeTarihi = items.TaksitVadeTarihi;
                                    sonucModel.EvrakNo = items.OdemeBelgeNo;
                                    if (items.OdemeBelgeTarihi != null)
                                    {
                                        sonucModel.IslemTarih = items.OdemeBelgeTarihi.Value;
                                    }
                                    //else
                                    //{
                                    //    sonucModel.IslemTarih = Convert.ToDateTime(null);
                                    //}
                                    sonucModel.Devir = devredenBakiye;
                                    sonucModel.DevirAlacak = toplamOdenenTutar;
                                    sonucModel.DevirBorc = toplamBorc;
                                    if (sonucModel.Borc >= sonucModel.Alacak)
                                    {
                                        sonucModel.BorcTipi = "B";
                                    }
                                    else if (sonucModel.Alacak < 0)
                                    {
                                        sonucModel.BorcTipi = "A";
                                    }
                                    if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                                    {
                                        sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + item.PoliceGenel.BransAdi + "- " + item.PoliceGenel.PoliceSigortali.AdiUnvan + "- " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                                " -" + item.PoliceGenel.PoliceArac.PlakaKodu + " " + item.PoliceGenel.PoliceArac.PlakaNo;
                                    }
                                    else
                                    {
                                        sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " - " + item.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + item.PoliceGenel.BransAdi + " - " + item.PoliceGenel.PoliceSigortali.AdiUnvan + " - " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                            " - " + item.PoliceGenel.PoliceSigortali.IlAdi + " / " + item.PoliceGenel.PoliceSigortali.IlceAdi;
                                    }
                                    // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                                    //if (item.PoliceGenel.TaliAcenteKodu != null)
                                    //{
                                    //    sonucModel.TaliAcenteKodu = item.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                    //    var taliTvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value);
                                    //    sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                                    //}
                                    //if (item.PoliceGenel.UretimTaliAcenteKodu != null)
                                    //{
                                    //    sonucModel.DisKaynakKodu = item.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                    //    var disKaynakUnvan = _TvmService.GetDetay(item.PoliceGenel.UretimTaliAcenteKodu.Value);
                                    //    sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                                    //}
                                    sonucModel.VerilenKomisyon = item.PoliceGenel.TaliKomisyon;
                                    sonucModel.BaslangicTarihi = item.PoliceGenel.BaslangicTarihi;
                                    if (item.PoliceGenel.BitisTarihi != null)
                                    {
                                        sonucModel.BitisTarihi = item.PoliceGenel.BitisTarihi.Value.Date;
                                    }
                                    sonucModel.TanzimTarihi = item.PoliceGenel.TanzimTarihi.Value.Date;
                                    sonucModel.BrütPrim = item.PoliceGenel.BrutPrim != null ? item.PoliceGenel.BrutPrim : 0;
                                    sonucModel.EkNo = item.PoliceGenel.EkNo.HasValue ? item.PoliceGenel.EkNo.Value.ToString() : null;
                                    sonucModel.Komisyon = item.PoliceGenel.Komisyon;
                                    sonucModel.NetPrim = item.PoliceGenel.NetPrim;
                                    sonucModel.OdemeSekli = item.PoliceGenel.OdemeSekli.HasValue ? item.PoliceGenel.OdemeSekli.Value.ToString() : null;
                                    sonucModel.SigortaSirketi = item.PoliceGenel.SigortaSirketleri.SirketAdi;
                                    sonucModel.PoliceNo = item.PoliceGenel.PoliceNumarasi;
                                    sonucModel.TaksitSayisi = muhasebeSahisUnvan.OfflineSigortaEttiren.Find(s => s.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis.Count();
                                    sonucModel.ParaBirimi = item.PoliceGenel.ParaBirimi;
                                    if (sonucModel.OdemeSekli == "1")
                                    {
                                        sonucModel.OdemeSekli = "Peşin";
                                    }
                                    else
                                    {
                                        sonucModel.OdemeSekli = "Taksit";
                                    }
                                    //foreach (var itemss in muhasebeSahisUnvan.OfflineSigortaEttiren.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceTahsilats)
                                    //{
                                    //    sonucModel.OdemeTipim = itemss.OdemTipi.ToString();
                                    //}
                                    if (items.OdemTipi == 1)
                                    {
                                        sonucModel.IslemTipi = "NK";
                                    }
                                    else if (items.OdemTipi == 2)
                                    {
                                        sonucModel.IslemTipi = "Müşteri KK";
                                    }
                                    else if (items.OdemTipi == 3)
                                    {
                                        sonucModel.IslemTipi = "HVL";
                                    }
                                    if (items.OdemTipi == 4)
                                    {
                                        sonucModel.IslemTipi = "ÇEK";
                                    }
                                    if (items.OdemTipi == 7)
                                    {
                                        sonucModel.IslemTipi = "SENET";
                                    }
                                    if (items.OdemTipi == 9)
                                    {
                                        sonucModel.IslemTipi = "Acente B. KK";
                                    }
                                    if (items.OdemTipi == 5)
                                    {
                                        sonucModel.IslemTipi = "Acente KK";
                                    }
                                    if (items.OdemTipi == 6)
                                    {
                                        sonucModel.IslemTipi = "Acente Pos";
                                    }
                                    if (items.OdemTipi == 0)
                                    {
                                        sonucModel.IslemTipi = "HVL";
                                    }
                                    ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(item.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? item.PoliceGenel.PoliceSigortaEttiren.KimlikNo : item.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                    ViewBag.SigortaEttirenUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                    ViewBag.Donemm = model.Yil;
                                    model.list.Add(sonucModel);

                                }

                            }
                        }
                    }
                }
                if (model.Durum == 1)
                {
                    if (model.UnvanFirma != null)
                    {
                        model.UnvanFirma = model.UnvanFirma.Trim();
                        var muhasebeFirmaUnvan = _Muhasebe_CariHesapService.MuhasebeCariHesapFirmaUnvan(model.UnvanFirma, model.TVMKodu, model.Durum, model.Yil);
                        //var devirmuhasebeFirmaUnvan = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapFirmaUnvan(model.UnvanFirma, model.TVMKodu, model.Durum);
                        if (muhasebeFirmaUnvan != null)
                        {
                            foreach (var item in muhasebeFirmaUnvan.OfflineSigortaEttiren)
                            {
                                sonucModel = new HesapEkstresiModel();
                                sonucModel.PoliceId = item.PoliceId;
                                sonucModel.KimlikNo = item.VergiKimlikNo;
                                var polBilgiBul = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, devirYil);
                                foreach (var items in polBilgiBul.TahsilatTckVn)
                                {
                                    toplamOdenenTutar += items.OdenenTutar;
                                    toplamBorc += items.TaksitTutari;
                                    devredenBakiye = toplamBorc - toplamOdenenTutar;
                                }
                            }

                        }
                        yuruyenBakiye += devredenBakiye;
                        if (muhasebeFirmaUnvan != null)
                        {
                            foreach (var item in muhasebeFirmaUnvan.OfflineSigortaEttiren)
                            {

                                foreach (var items in muhasebeFirmaUnvan.OfflineSigortaEttiren.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceTahsilats)
                                {
                                    sonucModel = new HesapEkstresiModel();

                                    sonucModel.PoliceId = item.PoliceId;
                                    //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                                    //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                                    //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                                    sonucModel.SigortaliUnvani = item.PoliceGenel.PoliceSigortali.AdiUnvan + " " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan;
                                    sonucModel.YenilemeNo = item.PoliceGenel.YenilemeNo.HasValue ? item.PoliceGenel.YenilemeNo.Value.ToString() : null;
                                    sonucModel.BransAdi = item.PoliceGenel.BransAdi;
                                    if (item.PoliceGenel.BransKodu != null)
                                    {
                                        sonucModel.BransKodu = item.PoliceGenel.BransKodu.Value;
                                    }
                                    sonucModel.UrunAdi = item.PoliceGenel.TUMUrunAdi;
                                    sonucModel.Borc = items.TaksitTutari;
                                    sonucModel.Alacak = items.OdenenTutar;
                                    sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                                    sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                                    yuruyenBakiye += sonucModel.ToplamBakiye;
                                    sonucModel.ToplamBakiye = yuruyenBakiye;
                                    sonucModel.VadeTarihi = items.TaksitVadeTarihi;
                                    sonucModel.EvrakNo = items.OdemeBelgeNo;
                                    if (items.OdemeBelgeTarihi != null)
                                    {
                                        sonucModel.IslemTarih = items.OdemeBelgeTarihi.Value;
                                    }
                                    sonucModel.Devir = devredenBakiye;
                                    sonucModel.DevirAlacak = toplamOdenenTutar;
                                    sonucModel.DevirBorc = toplamBorc;
                                    if (sonucModel.Borc >= sonucModel.Alacak)
                                    {
                                        sonucModel.BorcTipi = "B";
                                    }
                                    else if (sonucModel.Alacak < 0)
                                    {
                                        sonucModel.BorcTipi = "A";
                                    }
                                    if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                                    {
                                        sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " - " + item.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + item.PoliceGenel.BransAdi + "- " + item.PoliceGenel.PoliceSigortali.AdiUnvan + "- " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                                " -" + item.PoliceGenel.PoliceArac.PlakaKodu + " " + item.PoliceGenel.PoliceArac.PlakaNo;
                                    }
                                    else
                                    {
                                        sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " - " + item.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + item.PoliceGenel.BransAdi + " - " + item.PoliceGenel.PoliceSigortali.AdiUnvan + " - " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                            " - " + item.PoliceGenel.PoliceSigortali.IlAdi + " / " + item.PoliceGenel.PoliceSigortali.IlceAdi;
                                    }
                                    // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                                    if (item.PoliceGenel.TaliAcenteKodu != null)
                                    {
                                        sonucModel.TaliAcenteKodu = item.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                        var taliTvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value);
                                        sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                                    }
                                    if (item.PoliceGenel.UretimTaliAcenteKodu != null)
                                    {
                                        sonucModel.DisKaynakKodu = item.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                        var disKaynakUnvan = _TvmService.GetDetay(item.PoliceGenel.UretimTaliAcenteKodu.Value);
                                        sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                                    }
                                    sonucModel.VerilenKomisyon = item.PoliceGenel.TaliKomisyon;
                                    sonucModel.BaslangicTarihi = item.PoliceGenel.BaslangicTarihi;
                                    if (item.PoliceGenel.BitisTarihi != null)
                                    {
                                        sonucModel.BitisTarihi = item.PoliceGenel.BitisTarihi.Value.Date;
                                    }
                                    sonucModel.TanzimTarihi = item.PoliceGenel.TanzimTarihi.Value.Date;
                                    sonucModel.BrütPrim = item.PoliceGenel.BrutPrim;
                                    sonucModel.EkNo = item.PoliceGenel.EkNo.HasValue ? item.PoliceGenel.EkNo.Value.ToString() : null;
                                    sonucModel.Komisyon = item.PoliceGenel.Komisyon;
                                    sonucModel.NetPrim = item.PoliceGenel.NetPrim;
                                    sonucModel.OdemeSekli = item.PoliceGenel.OdemeSekli.HasValue ? item.PoliceGenel.OdemeSekli.Value.ToString() : null;
                                    sonucModel.SigortaSirketi = item.PoliceGenel.SigortaSirketleri.SirketAdi;
                                    sonucModel.PoliceNo = item.PoliceGenel.PoliceNumarasi;
                                    sonucModel.TaksitSayisi = muhasebeFirmaUnvan.OfflineSigortaEttiren.Find(s => s.PoliceId == item.PoliceId).PoliceGenel.PoliceOdemePlanis.Count();
                                    sonucModel.ParaBirimi = item.PoliceGenel.ParaBirimi;
                                    if (sonucModel.OdemeSekli == "1")
                                    {
                                        sonucModel.OdemeSekli = "Peşin";
                                    }
                                    else
                                    {
                                        sonucModel.OdemeSekli = "Taksit";
                                    }
                                    foreach (var itemss in muhasebeFirmaUnvan.OfflineSigortaEttiren.Find(f => f.PoliceId == item.PoliceId).PoliceGenel.PoliceTahsilats)
                                    {
                                        sonucModel.OdemeTipim = itemss.OdemTipi.ToString();
                                    }
                                    if (items.OdemTipi == 1)
                                    {
                                        sonucModel.IslemTipi = "NK";
                                    }
                                    if (items.OdemTipi == 2)
                                    {
                                        sonucModel.IslemTipi = "Müşteri KK";
                                    }
                                    if (items.OdemTipi == 3)
                                    {
                                        sonucModel.IslemTipi = "HVL";
                                    }
                                    if (items.OdemTipi == 4)
                                    {
                                        sonucModel.IslemTipi = "ÇEK";
                                    }
                                    if (items.OdemTipi == 7)
                                    {
                                        sonucModel.IslemTipi = "SENET";
                                    }
                                    if (items.OdemTipi == 5)
                                    {
                                        sonucModel.IslemTipi = "Acente KK";
                                    }
                                    if (items.OdemTipi == 6)
                                    {
                                        sonucModel.IslemTipi = "Acente Pos";
                                    }
                                    if (items.OdemTipi == 9)
                                    {
                                        sonucModel.IslemTipi = "Acente B. KK";
                                    }
                                    if (items.OdemTipi == 0)
                                    {
                                        sonucModel.IslemTipi = "HVL";
                                    }
                                    ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(item.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? item.PoliceGenel.PoliceSigortaEttiren.KimlikNo : item.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                    ViewBag.SigortaEttirenUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                    ViewBag.Donemm = model.Yil;
                                    model.list.Add(sonucModel);

                                }
                            }
                        }
                    }

                }
                #endregion

                #region satış kanalından ara

                if (model.tvmList != null)
                {
                    if (model.tvmList != null)
                    {
                        List<int> liste = new List<int>();
                        foreach (var item in model.tvmList)
                        {
                            if (item != "multiselect-all")
                            {
                                liste.Add(Convert.ToInt32(item));
                            }
                        }
                        model.TVMListe = liste;
                    }
                    else
                    {
                        // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                        model.TVMListe = new List<int>();
                    }
                    var satisKanaliAra = _Muhasebe_CariHesapService.MuhasebeCariHesapSatisKanaliAra(model.TVMKodu, model.TVMListe, model.Ay, model.Yil);
                    var devirsatisKanaliAra = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapSatisKanaliAra(model.TVMKodu, model.TVMListe);
                    if (devirsatisKanaliAra != null)
                    {
                        foreach (var item in devirsatisKanaliAra.OfflineGenel)
                        {
                            sonucModel = new HesapEkstresiModel();
                            sonucModel.PoliceId = item.PoliceId;
                            sonucModel.KimlikNo = item.PoliceSigortaEttiren.KimlikNo;

                            if (sonucModel.KimlikNo == null || sonucModel.KimlikNo == "")
                            {
                                sonucModel.KimlikNo = item.PoliceSigortaEttiren.VergiKimlikNo;
                            }

                            var polBilgiBul = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, devirYil);

                            foreach (var items in polBilgiBul.TahsilatTckVn)
                            {
                                toplamOdenenTutar += items.OdenenTutar;
                                toplamBorc += items.TaksitTutari;
                            }
                            devredenBakiye = toplamBorc - toplamOdenenTutar;

                        }

                    }
                    yuruyenBakiye += devredenBakiye;
                    if (satisKanaliAra != null)
                    {
                        foreach (var items in satisKanaliAra.OfflineGenel)
                        {
                            var tahsilats = satisKanaliAra.OfflineGenel.Find(f => f.PoliceId == items.PoliceId).PoliceTahsilats;
                            var tahsilatList = tahsilats.Where(m => m.TaksitVadeTarihi.Month == model.Ay && m.TaksitVadeTarihi.Year == model.Yil).ToList();
                            foreach (var item in tahsilatList)
                            {
                                sonucModel = new HesapEkstresiModel();

                                sonucModel.PoliceId = item.PoliceId;
                                //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                                //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                                //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                                sonucModel.SigortaliUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                sonucModel.YenilemeNo = item.PoliceGenel.YenilemeNo.HasValue ? item.PoliceGenel.YenilemeNo.Value.ToString() : null;
                                sonucModel.BransAdi = item.PoliceGenel.BransAdi;
                                if (item.PoliceGenel.BransKodu != null)
                                {
                                    sonucModel.BransKodu = item.PoliceGenel.BransKodu.Value;
                                }
                                sonucModel.UrunAdi = item.PoliceGenel.TUMUrunAdi;
                                sonucModel.Borc = item.TaksitTutari;
                                sonucModel.Alacak = item.OdenenTutar;
                                sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                                sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                                yuruyenBakiye += sonucModel.ToplamBakiye;
                                sonucModel.ToplamBakiye = yuruyenBakiye;
                                sonucModel.VadeTarihi = item.TaksitVadeTarihi;
                                sonucModel.EvrakNo = item.OdemeBelgeNo;
                                sonucModel.Devir = devredenBakiye;
                                sonucModel.DevirAlacak = toplamOdenenTutar;
                                sonucModel.DevirBorc = toplamBorc;
                                if (sonucModel.Borc >= sonucModel.Alacak)
                                {
                                    sonucModel.BorcTipi = "B";
                                }
                                else if (sonucModel.Alacak < 0)
                                {
                                    sonucModel.BorcTipi = "A";
                                }
                                sonucModel.IslemTarih = item.OdemeBelgeTarihi.Value;
                                if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                                {
                                    sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + item.PoliceGenel.BransAdi + "- " + item.PoliceGenel.PoliceSigortali.AdiUnvan + " " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                            " -" + item.PoliceGenel.PoliceArac.PlakaKodu + " " + item.PoliceGenel.PoliceArac.PlakaNo;
                                }
                                else
                                {
                                    sonucModel.Aciklama = "Pol.No:" + item.PoliceGenel.PoliceNumarasi + " -Tk.No:" + item.TaksitNo + "-" + item.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + item.PoliceGenel.BransAdi + " - " + item.PoliceGenel.PoliceSigortali.AdiUnvan + "  " + item.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                        " - " + item.PoliceGenel.PoliceSigortali.IlAdi + " / " + item.PoliceGenel.PoliceSigortali.IlceAdi;
                                }
                                // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                                if (item.PoliceGenel.TaliAcenteKodu != null)
                                {
                                    sonucModel.TaliAcenteKodu = item.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                    var taliTvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value);
                                    sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                                }
                                if (item.PoliceGenel.UretimTaliAcenteKodu != null)
                                {
                                    sonucModel.DisKaynakKodu = item.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                    var disKaynakUnvan = _TvmService.GetDetay(item.PoliceGenel.UretimTaliAcenteKodu.Value);
                                    sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                                }
                                sonucModel.VerilenKomisyon = item.PoliceGenel.TaliKomisyon;
                                sonucModel.BaslangicTarihi = item.PoliceGenel.BaslangicTarihi;
                                if (item.PoliceGenel.BitisTarihi != null)
                                {
                                    sonucModel.BitisTarihi = item.PoliceGenel.BitisTarihi.Value.Date;
                                }

                                sonucModel.TanzimTarihi = item.PoliceGenel.TanzimTarihi.Value.Date;
                                sonucModel.BrütPrim = item.PoliceGenel.BrutPrim != null ? item.PoliceGenel.BrutPrim : 0;
                                sonucModel.EkNo = item.PoliceGenel.EkNo.HasValue ? item.PoliceGenel.EkNo.Value.ToString() : null;
                                sonucModel.Komisyon = item.PoliceGenel.Komisyon;
                                sonucModel.NetPrim = item.PoliceGenel.NetPrim;
                                sonucModel.OdemeSekli = item.PoliceGenel.OdemeSekli.HasValue ? item.PoliceGenel.OdemeSekli.Value.ToString() : null;
                                sonucModel.SigortaSirketi = item.PoliceGenel.SigortaSirketleri.SirketAdi;
                                sonucModel.PoliceNo = item.PoliceGenel.PoliceNumarasi;
                                sonucModel.TaksitSayisi = satisKanaliAra.OfflineGenel.Find(s => s.PoliceId == item.PoliceId).PoliceOdemePlanis.Count();
                                sonucModel.ParaBirimi = item.PoliceGenel.ParaBirimi;
                                if (sonucModel.OdemeSekli == "1")
                                {
                                    sonucModel.OdemeSekli = "Peşin";
                                }
                                else
                                {
                                    sonucModel.OdemeSekli = "Taksit";
                                }
                                foreach (var itemss in satisKanaliAra.OfflineGenel.Find(f => f.PoliceId == item.PoliceId).PoliceOdemePlanis)
                                {
                                    sonucModel.OdemeTipim = itemss.OdemeTipi.ToString();
                                }
                                if (item.OdemTipi == 1)
                                {
                                    sonucModel.IslemTipi = "NK";
                                }
                                if (item.OdemTipi == 2)
                                {
                                    sonucModel.IslemTipi = "KK";
                                }
                                if (item.OdemTipi == 3)
                                {
                                    sonucModel.IslemTipi = "HVL";
                                }
                                if (item.OdemTipi == 4)
                                {
                                    sonucModel.IslemTipi = "ÇEKSEN";
                                }
                                if (item.OdemTipi == 0)
                                {
                                    sonucModel.IslemTipi = "HVL";
                                }
                                ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(item.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? item.PoliceGenel.PoliceSigortaEttiren.KimlikNo : item.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                ViewBag.SigortaEttirenUnvani = item.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                ViewBag.Donemm = model.Yil;
                                model.list.Add(sonucModel);
                            }
                        }
                    }

                }
                #endregion

                #region grup kodundan arama
                if (model.GrupKodu != null)
                {
                    model.GrupKodu = model.GrupKodu.Trim();
                    var grupKoduAra = _Muhasebe_CariHesapService.MuhasebeCariHesapGrupKoduAra(model.TVMKodu, model.GrupKodu, model.Yil);
                    //var devirgrupKoduAra = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapGrupKoduAra(model.TVMKodu, model.GrupKodu);
                    if (grupKoduAra != null)
                    {
                        foreach (var item in grupKoduAra.OfflineMusteriGenel)
                        {
                            sonucModel = new HesapEkstresiModel();
                            sonucModel.KimlikNo = item.KimlikNo;
                            var polBilgiBul = _Muhasebe_CariHesapService.DevirMuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, devirYil);

                            foreach (var items in polBilgiBul.TahsilatTckVn)
                            {
                                toplamOdenenTutar += items.OdenenTutar;
                                toplamBorc += items.TaksitTutari;

                            }
                            devredenBakiye = toplamBorc - toplamOdenenTutar;

                        }

                    }
                    yuruyenBakiye += devredenBakiye;
                    if (grupKoduAra != null)
                    {
                        foreach (var item in grupKoduAra.OfflineMusteriGenel)
                        {
                            sonucModel = new HesapEkstresiModel();
                            sonucModel.KimlikNo = item.KimlikNo;
                            var polBilgiBul = _Muhasebe_CariHesapService.MuhasebeCariHesapTcVknAra(sonucModel.KimlikNo, model.TVMKodu, model.Yil);

                            foreach (var items in polBilgiBul.TahsilatTckVn)
                            {
                                sonucModel = new HesapEkstresiModel();
                                sonucModel.PoliceId = items.PoliceId;
                                //sonucModel.AcenteKodu = item.PoliceGenel.TVMKodu.Value.ToString();
                                //var tvmUnvan = _TvmService.GetDetay(item.PoliceGenel.TVMKodu.Value);
                                //sonucModel.AcenteUnvani = item.PoliceGenel.TVMDetay != null ? item.PoliceGenel.TVMDetay.Unvani : tvmUnvan != null ? tvmUnvan.Unvani : null;
                                sonucModel.SigortaliUnvani = items.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + items.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                sonucModel.YenilemeNo = items.PoliceGenel.YenilemeNo.HasValue ? items.PoliceGenel.YenilemeNo.Value.ToString() : null;
                                sonucModel.BransAdi = items.PoliceGenel.BransAdi;
                                if (items.PoliceGenel.BransKodu != null)
                                {
                                    sonucModel.BransKodu = items.PoliceGenel.BransKodu.Value;
                                }
                                sonucModel.UrunAdi = items.PoliceGenel.TUMUrunAdi;
                                sonucModel.Borc = items.TaksitTutari;
                                sonucModel.Alacak = items.OdenenTutar;
                                sonucModel.ToplamBakiye = sonucModel.Borc - sonucModel.Alacak;
                                sonucModel.Bakiye = sonucModel.Borc - sonucModel.Alacak;
                                yuruyenBakiye += sonucModel.ToplamBakiye;
                                sonucModel.ToplamBakiye = yuruyenBakiye;
                                sonucModel.VadeTarihi = items.TaksitVadeTarihi;
                                sonucModel.EvrakNo = items.OdemeBelgeNo;
                                sonucModel.Devir = devredenBakiye;
                                sonucModel.DevirAlacak = toplamOdenenTutar;
                                sonucModel.DevirBorc = toplamBorc;
                                if (sonucModel.Borc >= sonucModel.Alacak)
                                {
                                    sonucModel.BorcTipi = "B";
                                }
                                else if (sonucModel.Alacak < 0)
                                {
                                    sonucModel.BorcTipi = "A";
                                }
                                if (items.OdemeBelgeTarihi != null)
                                {
                                    sonucModel.IslemTarih = items.OdemeBelgeTarihi.Value;
                                }
                                else
                                {
                                    sonucModel.IslemTarih = Convert.ToDateTime(null);
                                }
                                if (sonucModel.BransKodu == 1 || sonucModel.BransKodu == 2)
                                {
                                    sonucModel.Aciklama = "Pol.No:" + items.PoliceGenel.PoliceNumarasi + " -Tk.No:" + items.TaksitNo + "-" + items.PoliceGenel.SigortaSirketleri.SirketAdi + "- " + items.PoliceGenel.BransAdi + "- " + items.PoliceGenel.PoliceSigortali.AdiUnvan + "- " + items.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                            " -" + items.PoliceGenel.PoliceArac.PlakaKodu + " " + items.PoliceGenel.PoliceArac.PlakaNo;
                                }
                                else
                                {
                                    sonucModel.Aciklama = "Pol.No:" + items.PoliceGenel.PoliceNumarasi + " -Tk.No:" + items.TaksitNo + "-" + items.PoliceGenel.SigortaSirketleri.SirketAdi + " - " + items.PoliceGenel.BransAdi + " - " + items.PoliceGenel.PoliceSigortali.AdiUnvan + "  " + items.PoliceGenel.PoliceSigortali.SoyadiUnvan +
                                        " - " + items.PoliceGenel.PoliceSigortali.IlAdi + " / " + items.PoliceGenel.PoliceSigortali.IlceAdi;
                                }
                                // sonucModel.TaliAcenteAdi = item.PoliceGenel.TaliAcenteKodu != null ? _TvmService.GetDetay(item.PoliceGenel.TaliAcenteKodu.Value).Unvani : null;

                                if (items.PoliceGenel.TaliAcenteKodu != null)
                                {
                                    sonucModel.TaliAcenteKodu = items.PoliceGenel.TaliAcenteKodu.Value.ToString();
                                    var taliTvmUnvan = _TvmService.GetDetay(items.PoliceGenel.TaliAcenteKodu.Value);
                                    sonucModel.TaliAcenteAdi = taliTvmUnvan != null ? taliTvmUnvan.Unvani : null;
                                }
                                if (items.PoliceGenel.UretimTaliAcenteKodu != null)
                                {
                                    sonucModel.DisKaynakKodu = items.PoliceGenel.UretimTaliAcenteKodu.Value.ToString();
                                    var disKaynakUnvan = _TvmService.GetDetay(items.PoliceGenel.UretimTaliAcenteKodu.Value);
                                    sonucModel.DisKaynakAdi = disKaynakUnvan != null ? disKaynakUnvan.Unvani : null;
                                }
                                sonucModel.VerilenKomisyon = items.PoliceGenel.TaliKomisyon;
                                sonucModel.BaslangicTarihi = items.PoliceGenel.BaslangicTarihi;
                                if (items.PoliceGenel.BitisTarihi != null)
                                {
                                    sonucModel.BitisTarihi = items.PoliceGenel.BitisTarihi.Value.Date;
                                }

                                sonucModel.TanzimTarihi = items.PoliceGenel.TanzimTarihi.Value.Date;
                                sonucModel.BrütPrim = items.PoliceGenel.BrutPrim != null ? items.PoliceGenel.BrutPrim : 0;
                                sonucModel.EkNo = items.PoliceGenel.EkNo.HasValue ? items.PoliceGenel.EkNo.Value.ToString() : null;
                                sonucModel.Komisyon = items.PoliceGenel.Komisyon;
                                sonucModel.NetPrim = items.PoliceGenel.NetPrim;
                                sonucModel.OdemeSekli = items.PoliceGenel.OdemeSekli.HasValue ? items.PoliceGenel.OdemeSekli.Value.ToString() : null;
                                sonucModel.SigortaSirketi = items.PoliceGenel.SigortaSirketleri.SirketAdi;
                                sonucModel.PoliceNo = items.PoliceGenel.PoliceNumarasi;
                                //sonucModel.TaksitSayisi = polBilgiBul.TahsilatTckVn.Find(s => s.PoliceId == items.PoliceId).PoliceGenel.PoliceOdemePlanis.Count();
                                sonucModel.ParaBirimi = items.PoliceGenel.ParaBirimi;
                                if (sonucModel.OdemeSekli == "1")
                                {
                                    sonucModel.OdemeSekli = "Peşin";
                                }
                                else
                                {
                                    sonucModel.OdemeSekli = "Taksit";
                                }
                                foreach (var itemss in polBilgiBul.TahsilatTckVn.Find(f => f.PoliceId == items.PoliceId).PoliceGenel.PoliceOdemePlanis)
                                {
                                    sonucModel.OdemeTipim = itemss.OdemeTipi.ToString();
                                }
                                if (items.OdemTipi == 1)
                                {
                                    sonucModel.IslemTipi = "NK";
                                }
                                else if (items.OdemTipi == 2)
                                {
                                    sonucModel.IslemTipi = "Müşteri KK";
                                }
                                else if (items.OdemTipi == 3)
                                {
                                    sonucModel.IslemTipi = "HVL";
                                }
                                if (items.OdemTipi == 4)
                                {
                                    sonucModel.IslemTipi = "ÇEK";
                                }
                                if (items.OdemTipi == 7)
                                {
                                    sonucModel.IslemTipi = "SENET";
                                }
                                if (items.OdemTipi == 5)
                                {
                                    sonucModel.IslemTipi = "Acente KK";
                                }
                                if (items.OdemTipi == 6)
                                {
                                    sonucModel.IslemTipi = "Acente Pos";
                                }
                                if (items.OdemTipi == 9)
                                {
                                    sonucModel.IslemTipi = "Acente B. KK";
                                }
                                if (items.OdemTipi == 0)
                                {
                                    sonucModel.IslemTipi = "HVL";
                                }
                                ViewBag.SigEtKimlikno = !String.IsNullOrEmpty(items.PoliceGenel.PoliceSigortaEttiren.KimlikNo) ? items.PoliceGenel.PoliceSigortaEttiren.KimlikNo : items.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                ViewBag.SigortaEttirenUnvani = items.PoliceGenel.PoliceSigortaEttiren.AdiUnvan + " " + items.PoliceGenel.PoliceSigortaEttiren.SoyadiUnvan;
                                ViewBag.Donemm = model.Yil;
                                model.list.Add(sonucModel);
                            }
                        }
                    }


                }
                #endregion


            }


            else
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);

        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapEkstresi, SekmeKodu = 0)]
        public ActionResult CariHesapHareketEktresi()
        {
            CariHesapEkstreListModel model = new CariHesapEkstreListModel();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();

            model.list = new List<CariHesapEkstreEkranModel>();
            List<SelectListItem> listYillar = new List<SelectListItem>();
            int yilAraligi1 = 0;
            int yilAraligi2 = 0;
            for (int yil = TurkeyDateTime.Today.AddYears(2).Year; yil >= 2015; yil--)
            {
                yilAraligi1 = yil;
                yilAraligi2 = yil - 1;
                string yilValue = yilAraligi1.ToString() + "-" + yilAraligi2.ToString();
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yilValue  , Text = yilValue },
                });
            }


            model.Donem = TurkeyDateTime.Now.Year.ToString() + "-" + (TurkeyDateTime.Now.Year - 1).ToString();
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();
            List<CariHesaplari> cariHesaplar = new List<CariHesaplari>();
            model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "Unvan", "").ListWithOptionLabel();

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.CurrentAccountName }, // "Cari Hesap Adi"
                new SelectListItem() { Value = "1", Text = babonline.CustomerGroupCode } // "Müşteri Grup Kodu"
            });
            model.AramaTip = 0;
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            List<SelectListItem> mizanTips = new List<SelectListItem>();
            mizanTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Detailed }, // Detaylı
                new SelectListItem() { Value = "1", Text = babonline.Summary } // Özet
            });
            model.MizanTip = 0;
            model.MizanTipleri = new SelectList(mizanTips, "Value", "Text", model.MizanTip);

            List<SelectListItem> pdfTips = new List<SelectListItem>();
            pdfTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Add_Policy_Payable_Table }, // Yaşlandırma Tablosu Dahil
                new SelectListItem() { Value = "1", Text = babonline.Non_Add_Policy_Payable_Table } // "Yaşlandırma Tablosu Hariç"
            });
            model.PdfTip = 0;
            model.PdfTipleri = new SelectList(pdfTips, "Value", "Text", model.PdfTip);

            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;
            }
            else
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapEkstresi, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult CariHesapHareketEktresi(CariHesapEkstreListModel model)
        {
            ViewBag.AnaTVM = false;
            string cariTipi = "";
            if (model.CariHesapKodu != null)
            {
                cariTipi = model.CariHesapKodu.Substring(0, 3);
            }
            TimeSpan fark = new TimeSpan();
            DateTime BitisTarihi = new DateTime();
            DateTime BaslangicTarihi = new DateTime();
            var partsDonem = model.Donem.Split('-');

            //Ekrandan seçilen model aralığının büyük olanına göre gün ay filtresi çalışacak
            if (!String.IsNullOrEmpty(model.BitisGunAy) && !String.IsNullOrEmpty(model.BaslangicGunAy))
            {
                string bitisT = model.BitisGunAy + "." + partsDonem[0];
                string baslangicT = model.BaslangicGunAy + "." + partsDonem[0];

                BitisTarihi = Convert.ToDateTime(bitisT);
                BaslangicTarihi = Convert.ToDateTime(baslangicT);
                fark = BitisTarihi - BaslangicTarihi;
            }

            if (!(Math.Abs(fark.Days) > 31) || (!String.IsNullOrEmpty(cariTipi) && cariTipi == "120") || model.AramaTip == 1)
            {
                model.list = new List<CariHesapEkstreEkranModel>();

                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
                int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                CariHesapEkstreEkranModel sonucModel = new CariHesapEkstreEkranModel();

                if (tvmTaliVar || KontrolTVMKod == -9999)
                {
                    ViewBag.AnaTVM = true;

                    #region tcvkno
                    if (model.CariHesapKodu != null)
                    {
                        model.CariHesapKodu = model.CariHesapKodu.Trim();
                    }

                    var returnModel = _Muhasebe_CariHesapService.CariHesapEkstre(model.CariHesapKodu, _AktifKullaniciService.TVMKodu, model.MusteriGurpKodu, model.AramaTip, model.Donem, BaslangicTarihi, BitisTarihi, model.MizanTip, model.PdfTip);

                    if (returnModel != null)
                    {
                        model.PDFURL = returnModel.PDFURL;
                        if (model.PDFURL != "hata")
                        {
                            model.pdfVar = true;
                        }
                        else
                        {
                            model.pdfVar = false;
                        }

                        if (returnModel.ekstreList.Count > 0)
                        {
                            decimal yuruyenBakiye = 0;

                            for (int i = returnModel.ekstreList.Count - 1; i >= 0; i--)
                            {
                                var item = returnModel.ekstreList[i];
                                sonucModel = new CariHesapEkstreEkranModel();
                                if (item.Bakiye.HasValue)
                                {
                                    yuruyenBakiye += item.Bakiye.Value;
                                }
                                sonucModel.Aciklama = item.Aciklama;
                                sonucModel.Adi = "";
                                sonucModel.Soyad = "";
                                sonucModel.Alacak = item.Alacak;
                                sonucModel.Borc = item.Borc;
                                sonucModel.Bakiye = yuruyenBakiye;

                                if (yuruyenBakiye < 0)
                                {
                                    sonucModel.BorcTipi = "A";
                                }
                                else
                                {
                                    sonucModel.BorcTipi = "B";
                                }
                                sonucModel.EvrakNo = item.EvrakNo;
                                sonucModel.EvrakTipi = item.EvrakTipi;
                                sonucModel.OdemeTarihi = item.OdemeTarihi;
                                sonucModel.VadeTarihi = item.VadeTarihi;
                                sonucModel.OdemeTipi = item.OdemeTipi;
                                sonucModel.ParaBirimi = item.ParaBirimi;
                                if (String.IsNullOrEmpty(model.MusteriGurpKodu))
                                {
                                    model.MusteriGurpKodu = item.MusteriGrupKodu;
                                }

                                model.list.Add(sonucModel);
                            }

                            model.BorcToplam = model.list.Select(s => s.Borc).Sum();
                            model.AlacakToplam = model.list.Select(s => s.Alacak).Sum();
                            model.BakiyeToplam = model.BorcToplam - model.AlacakToplam;
                        }

                        if (returnModel.cariHesap != null)
                        {
                            model.cariHesapBA = new CariHesapBAModel();

                            #region Aylık Borç Alacak
                            model.cariHesapBA.Borc1 = returnModel.cariHesap.Borc1.HasValue ? returnModel.cariHesap.Borc1.Value : 0;
                            model.cariHesapBA.Alacak1 = returnModel.cariHesap.Alacak1.HasValue ? returnModel.cariHesap.Alacak1.Value : 0;

                            model.cariHesapBA.Borc2 = returnModel.cariHesap.Borc2.HasValue ? returnModel.cariHesap.Borc2.Value : 0;
                            model.cariHesapBA.Alacak2 = returnModel.cariHesap.Alacak2.HasValue ? returnModel.cariHesap.Alacak2.Value : 0;

                            model.cariHesapBA.Borc3 = returnModel.cariHesap.Borc3.HasValue ? returnModel.cariHesap.Borc3.Value : 0;
                            model.cariHesapBA.Alacak3 = returnModel.cariHesap.Alacak3.HasValue ? returnModel.cariHesap.Alacak3.Value : 0;

                            model.cariHesapBA.Borc4 = returnModel.cariHesap.Borc4.HasValue ? returnModel.cariHesap.Borc4.Value : 0;
                            model.cariHesapBA.Alacak4 = returnModel.cariHesap.Alacak4.HasValue ? returnModel.cariHesap.Alacak4.Value : 0;

                            model.cariHesapBA.Borc5 = returnModel.cariHesap.Borc5.HasValue ? returnModel.cariHesap.Borc5.Value : 0;
                            model.cariHesapBA.Alacak5 = returnModel.cariHesap.Alacak5.HasValue ? returnModel.cariHesap.Alacak5.Value : 0;

                            model.cariHesapBA.Borc6 = returnModel.cariHesap.Borc6.HasValue ? returnModel.cariHesap.Borc6.Value : 0;
                            model.cariHesapBA.Alacak6 = returnModel.cariHesap.Alacak6.HasValue ? returnModel.cariHesap.Alacak6.Value : 0;

                            model.cariHesapBA.Borc7 = returnModel.cariHesap.Borc7.HasValue ? returnModel.cariHesap.Borc7.Value : 0;
                            model.cariHesapBA.Alacak7 = returnModel.cariHesap.Alacak7.HasValue ? returnModel.cariHesap.Alacak7.Value : 0;

                            model.cariHesapBA.Borc8 = returnModel.cariHesap.Borc8.HasValue ? returnModel.cariHesap.Borc8.Value : 0;
                            model.cariHesapBA.Alacak8 = returnModel.cariHesap.Alacak8.HasValue ? returnModel.cariHesap.Alacak8.Value : 0;

                            model.cariHesapBA.Borc9 = returnModel.cariHesap.Borc9.HasValue ? returnModel.cariHesap.Borc9.Value : 0;
                            model.cariHesapBA.Alacak9 = returnModel.cariHesap.Alacak9.HasValue ? returnModel.cariHesap.Alacak9.Value : 0;

                            model.cariHesapBA.Borc10 = returnModel.cariHesap.Borc10.HasValue ? returnModel.cariHesap.Borc10.Value : 0;
                            model.cariHesapBA.Alacak10 = returnModel.cariHesap.Alacak10.HasValue ? returnModel.cariHesap.Alacak10.Value : 0;

                            model.cariHesapBA.Borc11 = returnModel.cariHesap.Borc11.HasValue ? returnModel.cariHesap.Borc11.Value : 0;
                            model.cariHesapBA.Alacak11 = returnModel.cariHesap.Alacak11.HasValue ? returnModel.cariHesap.Alacak11.Value : 0;

                            model.cariHesapBA.Borc12 = returnModel.cariHesap.Borc12.HasValue ? returnModel.cariHesap.Borc12.Value : 0;
                            model.cariHesapBA.Alacak12 = returnModel.cariHesap.Alacak12.HasValue ? returnModel.cariHesap.Alacak12.Value : 0;
                            #endregion

                            #region Toplam Borç
                            model.cariHesapBA.toplamBorc = 0;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc1;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc2;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc3;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc4;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc5;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc6;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc7;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc8;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc9;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc10;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc11;
                            model.cariHesapBA.toplamBorc += model.cariHesapBA.Borc12;
                            #endregion

                            #region Toplam Alacak
                            model.cariHesapBA.toplamAlacak = 0;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak1;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak2;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak3;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak4;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak5;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak6;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak7;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak8;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak9;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak10;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak11;
                            model.cariHesapBA.toplamAlacak += model.cariHesapBA.Alacak12;
                            #endregion

                        }



                    }

                    #endregion
                }
                else
                {
                    ViewBag.AnaTVM = false;
                }

            }
            model.Donemler = new SelectList(_CommonService.DonemAralikListesi(), "Value", "Text", model.Donem).ToList();

            var hesapList = _Muhasebe_CariHesapService.GetCariHesapList(model.CariHesapAdi);
            if (hesapList.Count > 0)
            {
                model.CariHesapList = new SelectList(hesapList, "CariHesapKodu", "Unvan", model.CariHesapKodu).ListWithOptionLabel();
                var selected = model.CariHesapList.Where(s => s.Selected == true && s.Value != "").FirstOrDefault();
                if (selected != null)
                {
                    model.CariHesapText = selected.Text;
                }
            }
            else
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
                model.CariHesapList = list;

            }
            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text =  babonline.CurrentAccountName}, // "Cari Hesap Adi"
                new SelectListItem() { Value = "1", Text =  babonline.CustomerGroupCode } //Müşteri Grup Kodu
            });
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            List<SelectListItem> mizanTips = new List<SelectListItem>();
            mizanTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Detailed }, // Detaylı
                new SelectListItem() { Value = "1", Text = babonline.Summary } // Özet
            });
            model.MizanTipleri = new SelectList(mizanTips, "Value", "Text", model.MizanTip);

            List<SelectListItem> pdfTips = new List<SelectListItem>();
            pdfTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Add_Policy_Payable_Table }, // Yaşlandırma Tablosu Dahil
                new SelectListItem() { Value = "1", Text = babonline.Non_Add_Policy_Payable_Table } // Yaşlandırma Tablosu Hariç
            });
            model.PdfTipleri = new SelectList(pdfTips, "Value", "Text", model.PdfTip);
            return View(model);
        }

        public class ListModel
        {
            public string key { get; set; }
            public string value { get; set; }
        }

        #endregion

        #region Cari Hareket İşlemleri

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHareketGirisi, SekmeKodu = 0)]
        public ActionResult CariHareketEkle()
        {
            CariHareketEkleModel model = new CariHareketEkleModel();

            List<CariHesaplari> cariHesaplar = new List<CariHesaplari>();
            var evrakTipleri = _Muhasebe_CariHesapService.getCariEvrakTipler();
            var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
            model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            //model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            model.DovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", "1");
            model.EvrakTipleri = new SelectList(evrakTipleri, "Kodu", "Aciklama", "1");
            model.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");
            model.MasrafMerkezleri = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.BorcAlacakTipler = new SelectList(MuhasebeListModels.BorcAlacakTipleri(), "Value", "Text", "1");
            model.KarsiCari = new KarsiCariHareketEkleModel();
            model.KarsiCari.KarsiCariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            model.KarsiCari.KarsiDovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", "TL");
            model.KarsiCari.KarsiEvrakTipleri = new SelectList(evrakTipleri, "Kodu", "Aciklama", "1");
            model.KarsiCari.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");
            model.KarsiCari.BorcAlacakTipler = new SelectList(MuhasebeListModels.BorcAlacakTipleri(), "Value", "Text", "1");
            model.KarsiCari.MasrafMerkezleri = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.CariEvrakList = new List<SelectListItem>();
            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHareketGirisi, SekmeKodu = 0)]
        public ActionResult CariHareketEkle(CariHareketEkleModel model)
        {
            try
            {
                var tempcariHesap = _Muhasebe_CariHesapService.GetCariHesap(model.CariHesapKodu);
                if (tempcariHesap == null)
                    return Json(new { success = true, hata = "Cari Hesap Kodu seçiniz." });
                CariHareketleri cariHareket = new CariHareketleri();
                cariHareket.CariHesapId = tempcariHesap.CariHesapId;
                cariHareket.CariHesapKodu = model.CariHesapKodu;
                cariHareket.EvrakTipi = model.EvrakTipi;
                cariHareket.OdemeTipi = model.OdemeTipi;
                cariHareket.BorcAlacakTipi = model.BorcAlacakTipi;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                if (!String.IsNullOrEmpty(model.EvrakNo))
                {
                    cariHareket.EvrakNo = model.EvrakNo;
                }
                else if (!String.IsNullOrEmpty(model.CariEvrakNo))
                {
                    var parts = model.CariEvrakNo.Split('-');
                    if (parts.Length > 1)
                    {
                        cariHareket.EvrakNo = parts[0];
                    }
                    else
                    {
                        cariHareket.EvrakNo = model.CariEvrakNo;
                    }
                }

                cariHareket.Aciklama = model.Aciklama;
                cariHareket.Tutar = Convert.ToDecimal(model.Tutar);
                cariHareket.CariHareketTarihi = model.CariHareketTarihi;
                cariHareket.OdemeTarihi = model.OdemeTarihi;
                cariHareket.KayitTarihi = TurkeyDateTime.Now;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                cariHareket.MusteriGrupKodu = model.MusteriGrupKodu;
                cariHareket.DovizTipi = model.DovizTipi;
                cariHareket.DovizKuru = Convert.ToDecimal(model.DovizKuru);
                cariHareket.DovizTutari = Convert.ToDecimal(model.DovizTutari);
                cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
                var cariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                if (cariHareketEkle != null)
                {
                    model.KarsiCari = new KarsiCariHareketEkleModel();
                    model.KarsiCari.OdemeTarihi = cariHareketEkle.OdemeTarihi.Value;
                    model.KarsiCari.CariHareketTarihi = cariHareketEkle.CariHareketTarihi.Value;
                    model.KarsiCari.DovizKuru = cariHareketEkle.DovizKuru.HasValue ? cariHareketEkle.DovizKuru.Value.ToString() : "";
                    model.KarsiCari.DovizTutari = cariHareketEkle.DovizTutari.HasValue ? cariHareketEkle.DovizKuru.Value.ToString() : "";
                    model.KarsiCari.Tutar = cariHareketEkle.Tutar.ToString();
                    model.KarsiCari.Aciklama = cariHareketEkle.Aciklama;
                    model.KarsiCari.BorcAlacakTipi = cariHareketEkle.BorcAlacakTipi;
                    model.KarsiCari.DovizTipi = cariHareketEkle.DovizTipi;
                    model.KarsiCari.KarsiDovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", model.KarsiCari.DovizTipi);
                    model.KarsiCari.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", model.OdemeTipi);
                    return Json(new { model, success = true, hata = "", id = cariHareketEkle.Id });
                    //return RedirectToAction("CariHareketDetay", "Muhasebe", new { id = cariHareketEkle.Id });
                }
                else
                {
                    return Json(new { success = true, hata = "Cari Hareket Eklenemedi." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { model, success = false, hata = ex.ToString() });
            }

        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.BrokerCariHareketEkle, SekmeKodu = 0)]
        public ActionResult BrokerCariHareketEkle()
        {
            BrokerCariHareketEkleModel model = new BrokerCariHareketEkleModel();

            List<CariHesaplari> cariHesaplar = new List<CariHesaplari>();
            var evrakTipleri = _Muhasebe_CariHesapService.getCariEvrakTipler();
            var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
            model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            //model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            model.DovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", "1");
            model.EvrakTipleri = new SelectList(evrakTipleri, "Kodu", "Aciklama", "1");
            model.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");
            model.MasrafMerkezleri = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.BorcAlacakTipler = new SelectList(MuhasebeListModels.BorcAlacakTipleri(), "Value", "Text", "1");
            model.KarsiCari = new KarsiCariHareketEkleModel();
            model.KarsiCari.KarsiCariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "AdUnvan", "").ListWithOptionLabel();
            model.KarsiCari.KarsiDovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", "TL");
            model.KarsiCari.KarsiEvrakTipleri = new SelectList(evrakTipleri, "Kodu", "Aciklama", "1");
            model.KarsiCari.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");
            model.KarsiCari.BorcAlacakTipler = new SelectList(MuhasebeListModels.BorcAlacakTipleri(), "Value", "Text", "1");
            model.KarsiCari.MasrafMerkezleri = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.CariEvrakList = new List<SelectListItem>();
            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.BrokerCariHareketEkle, SekmeKodu = 0)]
        public ActionResult BrokerCariHareketEkle(BrokerCariHareketEkleModel model)
        {
            try
            {
                var tempcariHesap = _Muhasebe_CariHesapService.GetCariHesap(model.CariHesapKodu);
                if (tempcariHesap == null)
                    return Json(new { success = true, hata = "Cari Hesap Kodu seçiniz." });
                CariHareketleri cariHareket = new CariHareketleri();
                cariHareket.CariHesapId = tempcariHesap.CariHesapId;
                cariHareket.CariHesapKodu = model.CariHesapKodu;
                cariHareket.EvrakTipi = model.EvrakTipi;
                cariHareket.OdemeTipi = model.OdemeTipi;
                cariHareket.BorcAlacakTipi = model.BorcAlacakTipi;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                if (!String.IsNullOrEmpty(model.EvrakNo))
                {
                    cariHareket.EvrakNo = model.EvrakNo;
                }
                else if (!String.IsNullOrEmpty(model.CariEvrakNo))
                {
                    var parts = model.CariEvrakNo.Split('-');
                    if (parts.Length > 1)
                    {
                        cariHareket.EvrakNo = parts[0];
                    }
                    else
                    {
                        cariHareket.EvrakNo = model.CariEvrakNo;
                    }
                }

                cariHareket.Aciklama = model.Aciklama;
                cariHareket.Tutar = Convert.ToDecimal(model.Tutar);
                cariHareket.CariHareketTarihi = model.CariHareketTarihi;
                cariHareket.OdemeTarihi = model.OdemeTarihi;
                cariHareket.KayitTarihi = TurkeyDateTime.Now;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                cariHareket.MusteriGrupKodu = model.MusteriGrupKodu;
                cariHareket.DovizTipi = model.DovizTipi;
                cariHareket.DovizKuru = Convert.ToDecimal(model.DovizKuru);
                cariHareket.DovizTutari = Convert.ToDecimal(model.DovizTutari);
                cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
                var cariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                if (cariHareketEkle != null)
                {
                    model.KarsiCari = new KarsiCariHareketEkleModel();
                    model.KarsiCari.OdemeTarihi = cariHareketEkle.OdemeTarihi.Value;
                    model.KarsiCari.CariHareketTarihi = cariHareketEkle.CariHareketTarihi.Value;
                    model.KarsiCari.DovizKuru = cariHareketEkle.DovizKuru.HasValue ? cariHareketEkle.DovizKuru.Value.ToString() : "";
                    model.KarsiCari.DovizTutari = cariHareketEkle.DovizTutari.HasValue ? cariHareketEkle.DovizKuru.Value.ToString() : "";
                    model.KarsiCari.Tutar = cariHareketEkle.Tutar.ToString();
                    model.KarsiCari.Aciklama = cariHareketEkle.Aciklama;
                    model.KarsiCari.BorcAlacakTipi = cariHareketEkle.BorcAlacakTipi;
                    model.KarsiCari.DovizTipi = cariHareketEkle.DovizTipi;
                    model.KarsiCari.KarsiDovizTipleri = new SelectList(TeklifProvider.ParaBirimTipleri(), "Value", "Text", model.KarsiCari.DovizTipi);
                    model.KarsiCari.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", model.OdemeTipi);
                    return Json(new { model, success = true, hata = "", id = cariHareketEkle.Id });
                    //return RedirectToAction("CariHareketDetay", "Muhasebe", new { id = cariHareketEkle.Id });
                }
                else
                {
                    return Json(new { success = true, hata = "Cari Hareket Eklenemedi." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { model, success = false, hata = ex.ToString() });
            }
        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.BrokerCariHareketEkle, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult BrokerKarsiCariHareketEkle(KarsiCariHareketEkleModel model)
        {
            try
            {
                CariHareketleri cariHareket = new CariHareketleri();
                cariHareket.CariHesapKodu = model.KarsiCari.KarsiCariHesapKodu;
                cariHareket.EvrakTipi = model.KarsiCari.KarsiEvrakTipi;
                cariHareket.BorcAlacakTipi = model.KarsiCari.BorcAlacakTipi;
                cariHareket.EvrakNo = model.KarsiCari.KarsiEvrakNo;
                cariHareket.Aciklama = model.KarsiCari.Aciklama;
                cariHareket.Tutar = Convert.ToDecimal(model.KarsiCari.Tutar);
                cariHareket.CariHareketTarihi = model.KarsiCari.CariHareketTarihi;
                cariHareket.OdemeTarihi = model.KarsiCari.OdemeTarihi;
                cariHareket.OdemeTipi = model.KarsiCari.OdemeTipi;
                cariHareket.KayitTarihi = TurkeyDateTime.Now;
                cariHareket.MusteriGrupKodu = model.KarsiCari.MusteriGrupKodu;
                cariHareket.DovizTipi = model.KarsiCari.DovizTipi;
                cariHareket.DovizKuru = Convert.ToDecimal(model.KarsiCari.DovizKuru);
                cariHareket.DovizTutari = Convert.ToDecimal(model.KarsiCari.DovizTutari);
                cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                var cariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                if (cariHareketEkle != null)
                {
                    return Json(new { success = true, hata = "" });
                }
                else
                {
                    return Json(new { model, success = false, hata = "Kayıt işlemi sırasında bir hata oluştu. Lütfen yeniden deneyiniz." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { model, success = false, hata = ex.ToString() });
            }
        }
        [AjaxException]
        public ActionResult CariHareketveKarsiHareketEkle(CariHareketEkleModel model)
        {
            try
            {
                CariHareketleri cariHareket = new CariHareketleri();
                cariHareket.CariHesapKodu = model.CariHesapKodu;
                cariHareket.EvrakTipi = model.EvrakTipi;
                cariHareket.OdemeTipi = model.OdemeTipi;
                cariHareket.BorcAlacakTipi = model.BorcAlacakTipi;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                if (!String.IsNullOrEmpty(model.EvrakNo))
                {
                    cariHareket.EvrakNo = model.EvrakNo;
                }
                else if (!String.IsNullOrEmpty(model.CariEvrakNo))
                {
                    var parts = model.CariEvrakNo.Split('-');
                    if (parts.Length > 1)
                    {
                        cariHareket.EvrakNo = parts[0];
                    }
                    else
                    {
                        cariHareket.EvrakNo = model.CariEvrakNo;
                    }
                }
                cariHareket.Aciklama = model.Aciklama;
                cariHareket.Tutar = Convert.ToDecimal(model.Tutar);
                cariHareket.CariHareketTarihi = model.CariHareketTarihi;
                cariHareket.OdemeTarihi = model.OdemeTarihi;
                cariHareket.KayitTarihi = TurkeyDateTime.Now;
                cariHareket.MasrafMerkezi = model.MasrafMerkezi;
                cariHareket.MusteriGrupKodu = model.MusteriGrupKodu;
                cariHareket.DovizTipi = model.DovizTipi;
                cariHareket.DovizKuru = Convert.ToDecimal(model.DovizKuru);
                cariHareket.DovizTutari = Convert.ToDecimal(model.DovizTutari);
                cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                var cariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                if (cariHareketEkle != null)
                {
                    cariHareket = new CariHareketleri();
                    cariHareket.CariHesapKodu = model.KarsiCari.KarsiCariHesapKodu;
                    cariHareket.EvrakTipi = model.KarsiCari.KarsiEvrakTipi;
                    cariHareket.BorcAlacakTipi = model.KarsiCari.BorcAlacakTipi;
                    cariHareket.EvrakNo = model.KarsiCari.KarsiEvrakNo;
                    cariHareket.Aciklama = model.KarsiCari.Aciklama;
                    cariHareket.Tutar = Convert.ToDecimal(model.KarsiCari.Tutar);
                    cariHareket.CariHareketTarihi = model.KarsiCari.CariHareketTarihi;
                    cariHareket.OdemeTarihi = model.KarsiCari.OdemeTarihi;
                    cariHareket.OdemeTipi = model.KarsiCari.OdemeTipi;
                    cariHareket.KayitTarihi = TurkeyDateTime.Now;
                    cariHareket.MusteriGrupKodu = model.KarsiCari.MusteriGrupKodu;
                    cariHareket.DovizTipi = model.KarsiCari.DovizTipi;
                    cariHareket.DovizKuru = Convert.ToDecimal(model.KarsiCari.DovizKuru);
                    cariHareket.DovizTutari = Convert.ToDecimal(model.KarsiCari.DovizTutari);
                    cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                    var KarsiCariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                    if (KarsiCariHareketEkle != null)
                    {
                        return Json(new { success = true, hata = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { model, success = false, hata = "Karşı Cari Haraket Eklerken Bir Hata Oluştu!\nCari Hareket Yapıldı Ancak Karşı Cari Hareket Yapılamadı." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, hata = "Cari Hareketler Eklenemedi." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { model, success = false, hata = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHareketGirisi, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult KarsiCariHareketEkle(KarsiCariHareketEkleModel model)
        {
            try
            {
                CariHareketleri cariHareket = new CariHareketleri();
                cariHareket.CariHesapKodu = model.KarsiCari.KarsiCariHesapKodu;
                cariHareket.EvrakTipi = model.KarsiCari.KarsiEvrakTipi;
                cariHareket.BorcAlacakTipi = model.KarsiCari.BorcAlacakTipi;
                cariHareket.EvrakNo = model.KarsiCari.KarsiEvrakNo;
                cariHareket.Aciklama = model.KarsiCari.Aciklama;
                cariHareket.Tutar = Convert.ToDecimal(model.KarsiCari.Tutar);
                cariHareket.CariHareketTarihi = model.KarsiCari.CariHareketTarihi;
                cariHareket.OdemeTarihi = model.KarsiCari.OdemeTarihi;
                cariHareket.OdemeTipi = model.KarsiCari.OdemeTipi;
                cariHareket.KayitTarihi = TurkeyDateTime.Now;
                cariHareket.MusteriGrupKodu = model.KarsiCari.MusteriGrupKodu;
                cariHareket.DovizTipi = model.KarsiCari.DovizTipi;
                cariHareket.DovizKuru = Convert.ToDecimal(model.KarsiCari.DovizKuru);
                cariHareket.DovizTutari = Convert.ToDecimal(model.KarsiCari.DovizTutari);
                cariHareket.TVMKodu = _AktifKullaniciService.TVMKodu;
                var cariHareketEkle = _Muhasebe_CariHesapService.CariHareketEkle(cariHareket);
                if (cariHareketEkle != null)
                {
                    return Json(new { success = true, hata = "" });
                }
                else
                {
                    return Json(new { model, success = false, hata = "Kayıt işlemi sırasında bir hata oluştu. Lütfen yeniden deneyiniz." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { model, success = false, hata = ex.ToString() });
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHareketGirisi, SekmeKodu = 0)]
        [AjaxException]
        public ActionResult getCariHesapByHesapKodu(string hesapKodu)
        {
            try
            {
                var CariHesap = _Muhasebe_CariHesapService.GetCariHesap(hesapKodu);
                if (CariHesap != null)
                {
                    return Json(new { success = true, hesapAdi = CariHesap.Unvan, hesapKodu = CariHesap.CariHesapKodu }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CariHareketDetay(int id)
        {
            CariHareketDetayModel model = new CariHareketDetayModel();
            var detay = _Muhasebe_CariHesapService.getCariHareketDetay(id);
            if (detay != null)
            {
                model.CariHesapKodu = detay.CariHesapKodu;
                model.Aciklama = detay.Aciklama;
                model.DovizKuru = detay.DovizKuru.HasValue ? detay.DovizKuru.Value.ToString() : "";
                model.DovizTutari = detay.DovizTutari.HasValue ? detay.DovizTutari.Value.ToString() : "";
                model.Tutar = detay.Tutar.ToString("N2");
                model.MusteriGrupKodu = detay.MusteriGrupKodu;
                model.KayitTarihi = detay.KayitTarihi.ToString("dd/MM/yyyy");
                model.OdemeTarihi = detay.OdemeTarihi.HasValue ? detay.OdemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                model.CariHareketTarihi = detay.CariHareketTarihi.HasValue ? detay.CariHareketTarihi.Value.ToString("dd/MM/yyyy") : "";
                model.EvrakNo = detay.EvrakNo;
                model.MasrafMerkeziText = "";
                model.DovizTipiText = detay.DovizTipi;
                if (detay.BorcAlacakTipi == "B")
                {
                    model.BorcAlacakTipiText = babonline.Debt; // Borç
                }
                else
                {
                    model.BorcAlacakTipiText = babonline.Receivables; // Alacak
                }
                model.EvrakTipiText = _Muhasebe_CariHesapService.getCariEvrakTip(detay.EvrakTipi);

                if (detay.OdemeTipi != null)
                {
                    var tipAdi = _Muhasebe_CariHesapService.getCariOdemeTip(detay.OdemeTipi.Value);
                    model.OdemeTipiText = tipAdi;
                }
                else
                {
                    model.OdemeTipiText = "";
                }

            }
            return View(model);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapHareketKontrolListesi, SekmeKodu = 0)]
        public ActionResult CariHesapHareketKontrolListesi()
        {
            CariHesapHareketKontrolListesiModel model = new CariHesapHareketKontrolListesiModel();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();

            model.list = new List<CariHareketListeEkranModel>();
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donem = TurkeyDateTime.Now.Year;
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();
            List<CariHesaplari> cariHesaplar = new List<CariHesaplari>();
            model.CariHesapList = new SelectList(cariHesaplar, "CariHesapKodu", "Unvan", "").ListWithOptionLabel();

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.CurrentAccountName }, // Cari Hesap Adi
                new SelectListItem() { Value = "1", Text = babonline.CustomerGroupCode }, // Müşteri Grup Kodu
                new SelectListItem() { Value=  "2", Text = babonline.DateRange } // İki Tarih Arası
            });
            model.AramaTip = 0;
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;
            }
            else
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapHareketKontrolListesi, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult CariHesapHareketKontrolListesi(CariHesapHareketKontrolListesiModel model)
        {
            if (model.AramaTip == 2)
                model.AramaDurumu = false;
            else
                model.AramaDurumu = true;

            //aramadurumu true ise ada veya müşteri grub koduna göre arıyor. false ise iki tarih arası arıyor.
            if (model.AramaDurumu)
            {
                model.list = new List<CariHareketListeEkranModel>();
                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
                ViewBag.TVMKodu = _AktifKullaniciService.TVMKodu;
                int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                CariHareketListeEkranModel sonucModel = new CariHareketListeEkranModel();

                if (tvmTaliVar || KontrolTVMKod == -9999)
                {
                    ViewBag.AnaTVM = true;

                    #region tcvkno
                    if (model.CariHesapKodu != null)
                    {
                        model.CariHesapKodu = model.CariHesapKodu.Trim();
                    }

                    var EkstreList = _Muhasebe_CariHesapService.CariHesapHareketKontrolListesi(model.CariHesapKodu, _AktifKullaniciService.TVMKodu, model.Donem, model.MusteriGurpKodu, model.AramaTip, true, model.baslangicTarihi, model.bitisTarihi);
                    if (EkstreList != null && EkstreList.Count > 0)
                    {
                        int sayac = 0;
                        foreach (var item in EkstreList)
                        {
                            sonucModel = new CariHareketListeEkranModel();
                            sonucModel.Aciklama = item.Aciklama;
                            sonucModel.Alacak = item.Alacak;
                            sonucModel.Borc = item.Borc;
                            sonucModel.EvrakNo = item.EvrakNo;
                            sonucModel.EvrakTipi = item.EvrakTipi;
                            sonucModel.OdemeTarihi = item.OdemeTarihi;
                            sonucModel.VadeTarihi = item.VadeTarihi;
                            sonucModel.OdemeTipi = item.OdemeTipi;
                            sonucModel.ParaBirimi = item.ParaBirimi;
                            sonucModel.id = item.id;
                            sonucModel.CariHesapKodu = model.CariHesapKodu;
                            model.MusteriGurpKodu = item.MusteriGrupkodu;

                            model.list.Add(sonucModel);
                        }
                        model.BorcToplam = model.list.Select(s => s.Borc).Sum();
                        model.AlacakToplam = model.list.Select(s => s.Alacak).Sum();


                    }

                    #endregion
                }
                else
                {
                    ViewBag.AnaTVM = false;
                }
                List<SelectListItem> listYillar = new List<SelectListItem>();
                for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
                {
                    listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });
                }
                model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

                var hesapList = _Muhasebe_CariHesapService.GetCariHesapList(model.CariHesapAdi);
                if (hesapList.Count > 0)
                {
                    model.CariHesapList = new SelectList(hesapList, "CariHesapKodu", "Unvan", model.CariHesapKodu).ListWithOptionLabel();
                    var selected = model.CariHesapList.Where(s => s.Selected == true && s.Value != "").FirstOrDefault();
                    if (selected != null)
                    {
                        model.CariHesapText = selected.Text;
                    }
                }
                else
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
                    model.CariHesapList = list;

                }
                List<SelectListItem> aramaTips = new List<SelectListItem>();
                aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.CurrentAccountName }, // Cari Hesap Adi
                new SelectListItem() { Value = "1", Text = babonline.CustomerGroupCode }, // Müşteri Grup Kodu
                new SelectListItem() { Value=  "2", Text = babonline.DateRange } // İki Tarih Arası
            });
                model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);
                return View(model);

            }
            else
            {

                model.list = new List<CariHareketListeEkranModel>();
                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
                ViewBag.TVMKodu = _AktifKullaniciService.TVMKodu;
                int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                CariHareketListeEkranModel sonucModel = new CariHareketListeEkranModel();

                if (tvmTaliVar || KontrolTVMKod == -9999)
                {
                    ViewBag.AnaTVM = true;

                    #region tcvkno
                    if (model.CariHesapKodu != null)
                    {
                        model.CariHesapKodu = model.CariHesapKodu.Trim();
                    }
                    var EkstreList = _Muhasebe_CariHesapService.CariHesapHareketKontrolListesi("", _AktifKullaniciService.TVMKodu, 0, "", model.AramaTip, false, model.baslangicTarihi, model.bitisTarihi);

                    if (EkstreList != null && EkstreList.Count > 0)
                    {
                        foreach (var item in EkstreList)
                        {
                            sonucModel = new CariHareketListeEkranModel();
                            sonucModel.Aciklama = item.Aciklama;
                            //carihesapadi da gelecek 
                            var CariHesapAdiveKodu = _Muhasebe_CariHesapService.GetCariHesapAdi(item.CariHesapAdiKodu);
                            //item1 adi , item2 kodu
                            sonucModel.CariHesapAdiKodu = CariHesapAdiveKodu.Item1 + CariHesapAdiveKodu.Item2;
                            sonucModel.CariHesapKodu = CariHesapAdiveKodu.Item2;
                            sonucModel.MusteriGrupkodu = item.MusteriGrupkodu;
                            sonucModel.Alacak = item.Alacak;
                            sonucModel.Borc = item.Borc;
                            sonucModel.EvrakNo = item.EvrakNo;
                            sonucModel.EvrakTipi = item.EvrakTipi;
                            sonucModel.OdemeTarihi = item.OdemeTarihi;
                            sonucModel.VadeTarihi = item.VadeTarihi;
                            sonucModel.id = item.id;
                            sonucModel.OdemeTipi = item.OdemeTipi;
                            sonucModel.ParaBirimi = item.ParaBirimi;
                            model.list.Add(sonucModel);
                        }
                        model.BorcToplam = model.list.Select(s => s.Borc).Sum();
                        model.AlacakToplam = model.list.Select(s => s.Alacak).Sum();

                    }

                    #endregion
                }
                else
                {
                    ViewBag.AnaTVM = false;
                }
                List<SelectListItem> listYillar = new List<SelectListItem>();
                for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
                {
                    listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });
                }
                model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

                var hesapList = _Muhasebe_CariHesapService.GetCariHesapList(model.CariHesapAdi);
                if (hesapList.Count > 0)
                {
                    model.CariHesapList = new SelectList(hesapList, "CariHesapKodu", "Unvan", model.CariHesapKodu).ListWithOptionLabel();
                    var selected = model.CariHesapList.Where(s => s.Selected == true && s.Value != "").FirstOrDefault();
                    if (selected != null)
                    {
                        model.CariHesapText = selected.Text;
                    }
                }
                else
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
                    model.CariHesapList = list;

                }
                List<SelectListItem> aramaTips = new List<SelectListItem>();
                aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.CurrentAccountName }, // Cari Hesap Adi
                new SelectListItem() { Value = "1", Text = babonline.CustomerGroupCode }, // Müşteri Grup Kodu
                new SelectListItem() { Value=  "2", Text = babonline.DateRange } // İki Tarih Arası
            });
                model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);
                return View(model);

            }
        }

        //[Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.GelirGiderTablosu, SekmeKodu = 0)]
        public ActionResult GelirGiderTablosu()
        {
            GelirGiderTablosuAnaModel model = new GelirGiderTablosuAnaModel();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donem = TurkeyDateTime.Now.Year;
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Ay = TurkeyDateTime.Now.Month;
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();

            //List<SelectListItem> aramaTips = new List<SelectListItem>();
            //aramaTips.AddRange(new SelectListItem[] {
            //    new SelectListItem() { Value = "0", Text = "Dönem" },
            //    new SelectListItem() { Value=  "1", Text = "İki Tarih"}
            //});
            //model.AramaTip = 0;
            //model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Income }, // Gelir
                new SelectListItem() { Value = "1", Text = babonline.Expense }, // Gider
                new SelectListItem() { Value = "2", Text = babonline.Both }, // "Her İkisi"
            });
            model.AramaTip = 0;
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;
            }
            else
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);
        }

        //[Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.GelirGiderTablosu, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult GelirGiderTablosu(GelirGiderTablosuAnaModel model)
        {

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            ViewBag.TVMKodu = _AktifKullaniciService.TVMKodu;
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
            GelirGiderTablosuEkranModel sonucModel = new GelirGiderTablosuEkranModel();

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                ViewBag.AnaTVM = true;

                List<string> cariHesapTipList = new List<string>();


                if (model.AramaTip == 0) // Gelir - 101-600-602
                {
                    cariHesapTipList.Add("101");
                    cariHesapTipList.Add("600");
                    cariHesapTipList.Add("602");

                }
                else if (model.AramaTip == 1) // Gider - 103-330-610-611-612-740
                {
                    cariHesapTipList.Add("103");
                    cariHesapTipList.Add("330");
                    cariHesapTipList.Add("610");
                    cariHesapTipList.Add("611");
                    cariHesapTipList.Add("612");
                    cariHesapTipList.Add("740");

                }
                else if (model.AramaTip == 2) // Her ikisi - 101-600-602 -103-330-610-611-612-740
                {
                    cariHesapTipList.Add("101");
                    cariHesapTipList.Add("600");
                    cariHesapTipList.Add("602");
                    cariHesapTipList.Add("103");
                    cariHesapTipList.Add("330");
                    cariHesapTipList.Add("610");
                    cariHesapTipList.Add("611");
                    cariHesapTipList.Add("612");
                    cariHesapTipList.Add("740");

                }

                #region tcvkno
                foreach (string cariHesapTip in cariHesapTipList)
                {
                    var EkstreList = _Muhasebe_CariHesapService.CariHesapGelirGiderListesi(_AktifKullaniciService.TVMKodu, cariHesapTip, model.Donem, model.baslangicTarihi, model.bitisTarihi, model.AramaTip).GroupBy(x => x.CariHesapAdiKodu);

                    if (EkstreList != null)
                    {

                        if (model.AramaTip == 0 || model.AramaTip == 1)
                        {
                            foreach (var cariHesap in EkstreList)
                            {
                                model.dict[cariHesap.Key] = new GelirGiderTablosuModel();
                                string cariHesapAdi = _Muhasebe_CariHesapService.GetCariHesap(cariHesap.Key).Unvan;
                                model.dict[cariHesap.Key].CariHesapAdi = cariHesapAdi;

                                foreach (var cariHareket in cariHesap)
                                {
                                    sonucModel = new GelirGiderTablosuEkranModel();
                                    sonucModel.Aciklama = cariHareket.Aciklama;
                                    sonucModel.Alacak = cariHareket.Alacak;
                                    sonucModel.Borc = cariHareket.Borc;
                                    sonucModel.EvrakNo = cariHareket.EvrakNo;
                                    sonucModel.EvrakTipi = cariHareket.EvrakTipi;
                                    sonucModel.OdemeTarihi = cariHareket.OdemeTarihi;
                                    sonucModel.VadeTarihi = cariHareket.VadeTarihi;
                                    sonucModel.OdemeTipi = cariHareket.OdemeTipi;
                                    sonucModel.ParaBirimi = cariHareket.ParaBirimi;
                                    sonucModel.CariHesapKodu = cariHareket.CariHesapAdiKodu;
                                    sonucModel.id = cariHareket.id;
                                    model.dict[cariHesap.Key].list.Add(sonucModel);
                                }

                                string paraBirimi = model.dict[cariHesap.Key].list.Select(s => s.ParaBirimi).FirstOrDefault();
                                model.dict[cariHesap.Key].paraBirimi = paraBirimi;

                                model.dict[cariHesap.Key].BorcToplam = model.dict[cariHesap.Key].list.Select(s => s.Borc).Sum();
                                model.dict[cariHesap.Key].AlacakToplam = model.dict[cariHesap.Key].list.Select(s => s.Alacak).Sum();
                                model.dict[cariHesap.Key].BakiyeToplam = model.dict[cariHesap.Key].BorcToplam - model.dict[cariHesap.Key].AlacakToplam;
                            }
                        }
                        else
                        {
                            foreach (var cariHesap in EkstreList)
                            {
                                model.dict[cariHesap.Key] = new GelirGiderTablosuModel();
                                string cariHesapAdi = _Muhasebe_CariHesapService.GetCariHesap(cariHesap.Key).Unvan;
                                model.dict[cariHesap.Key].CariHesapAdi = cariHesapAdi;

                                foreach (var cariHareket in cariHesap)
                                {
                                    sonucModel = new GelirGiderTablosuEkranModel();
                                    sonucModel.Aciklama = cariHareket.Aciklama;
                                    sonucModel.Alacak = cariHareket.Alacak;
                                    sonucModel.Borc = cariHareket.Borc;
                                    sonucModel.EvrakNo = cariHareket.EvrakNo;
                                    sonucModel.EvrakTipi = cariHareket.EvrakTipi;
                                    sonucModel.OdemeTarihi = cariHareket.OdemeTarihi;
                                    sonucModel.VadeTarihi = cariHareket.VadeTarihi;
                                    sonucModel.OdemeTipi = cariHareket.OdemeTipi;
                                    sonucModel.ParaBirimi = cariHareket.ParaBirimi;
                                    sonucModel.CariHesapKodu = cariHareket.CariHesapAdiKodu;
                                    sonucModel.id = cariHareket.id;
                                    model.dict[cariHesap.Key].list.Add(sonucModel);
                                }

                                string paraBirimi = model.dict[cariHesap.Key].list.Select(s => s.ParaBirimi).FirstOrDefault();
                                model.dict[cariHesap.Key].paraBirimi = paraBirimi;
                                if (cariHesap.Key.Substring(0, 3) == "101" || cariHesap.Key.Substring(0, 3) == "600" || cariHesap.Key.Substring(0, 3) == "602")
                                {
                                    model.dict[cariHesap.Key].BorcToplam = model.dict[cariHesap.Key].list.Select(s => s.Borc).Sum();
                                    model.dict[cariHesap.Key].AlacakToplam = model.dict[cariHesap.Key].list.Select(s => s.Alacak).Sum();
                                    model.dict[cariHesap.Key].BakiyeToplam = model.dict[cariHesap.Key].BorcToplam - model.dict[cariHesap.Key].AlacakToplam;
                                }
                                else
                                {
                                    model.dict[cariHesap.Key].BorcToplamHer = model.dict[cariHesap.Key].list.Select(s => s.Borc).Sum();
                                    model.dict[cariHesap.Key].AlacakToplamHer = model.dict[cariHesap.Key].list.Select(s => s.Alacak).Sum();
                                    model.dict[cariHesap.Key].BakiyeToplamHer = model.dict[cariHesap.Key].BorcToplamHer - model.dict[cariHesap.Key].AlacakToplamHer;
                                }

                            }
                        }
                    }



                }
                #endregion





            }
            else
            {
                ViewBag.AnaTVM = false;
            }
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });
            }
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Income }, // Gelir
                new SelectListItem() { Value = "1", Text = babonline.Expense }, // Gider
                new SelectListItem() { Value=  "2", Text = babonline.Both }, // her ikisi
            });
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        public Boolean CariHesapHareketSil(int TVMKodu, int id, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak)
        {
            try
            {
                _Muhasebe_CariHesapService.DeleteCariHareket(id);
                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(TVMKodu, cariHesapKodu, donem, ay, borc, alacak);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [AjaxException]
        public ActionResult GetMusteriPoliceleri(string kimlikNo)
        {
            try
            {
                ListModel lModel = new ListModel();
                List<ListModel> listModel = new List<ListModel>();
                MusteriPoliceResultView model = new MusteriPoliceResultView();
                var parts = kimlikNo.Split('.');
                kimlikNo = parts[2];
                var musteriPoliceler = _Muhasebe_CariHesapService.GetMusteriPoliceleri(_AktifKullaniciService.TVMKodu, kimlikNo);
                StringBuilder listValue = new StringBuilder();
                StringBuilder ekNo = new StringBuilder();
                StringBuilder yenilemeNo = new StringBuilder();
                StringBuilder brutPrim = new StringBuilder();
                if (musteriPoliceler != null)
                {
                    for (int i = 0; i < musteriPoliceler.Count(); i++)
                    {
                        lModel = new ListModel();
                        listValue = new StringBuilder();
                        ekNo = new StringBuilder();
                        yenilemeNo = new StringBuilder();
                        brutPrim = new StringBuilder();
                        brutPrim.Append(musteriPoliceler[i].BrutPrim.HasValue ? musteriPoliceler[i].BrutPrim.Value.ToString("N2") : "0");
                        ekNo.Append(musteriPoliceler[i].Ekno.HasValue ? musteriPoliceler[i].Ekno.Value.ToString() : "0");
                        yenilemeNo.Append(musteriPoliceler[i].YenilemeNo.HasValue ? musteriPoliceler[i].YenilemeNo.Value.ToString() : "0");
                        listValue.Append(musteriPoliceler[i].PoliceNumarasi + "/" + yenilemeNo + "/" + ekNo + "/" + brutPrim);
                        var odemeTipi = musteriPoliceler[i].OdemTipiKKveyaBKKmi ? "1" : "0";
                        lModel.value = listValue.ToString();
                        // 0 = kk ve bkk ile ödeme yapılmamış. 1 ise kk veya bkk ile ödeme yapılmış 
                        lModel.key = listValue.ToString() + "-" + odemeTipi;
                        listModel.Add(lModel);
                    }
                }
                model.list = new SelectList(listModel, "Key", "Value").ListWithOptionLabel();

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public class MusteriPoliceResultView
        {
            public List<SelectListItem> list { get; set; }
            public string hata { get; set; }
        }


        #endregion

        #region Cari Hesap İşlemleri
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapGirisi, SekmeKodu = 0)]
        public ActionResult CariHesapEkle()
        {
            var eklendiMi = "false";
            if (TempData.Count > 0)
            {
                eklendiMi = !(String.IsNullOrEmpty(TempData["eklendiMi"].ToString())) ? TempData["eklendiMi"].ToString() : "false";
            }
            if (eklendiMi == "true")
            {
                ViewBag.eklendiMi = "true";
            }
            else
            {
                ViewBag.eklendiMi = "false";
            }
            TempData["eklendiMi"] = "false";

            CariHesapEkleModel model = new CariHesapEkleModel();

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.UlkeKodu = "TUR";
            model.IlKodu = "34";
            model.CariHesapTipleri = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "");
            List<Ulke> ulkeler = _UlkeService.GetUlkeList();
            List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
            List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);
            model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
            model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
            model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "0").ListWithOptionLabel();

            List<SigortaSirketleri> sigortalar = _SigortaSirketleriService.GetList();
            var sirketKodu = sigortalar.FirstOrDefault().SirketKodu;
            model.sigortaSirketleri = new SelectList(sigortalar, "SirketKodu", "SirketAdi", sirketKodu).ListWithOptionLabel(false);
            var brokerlar = _TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani);
            model.yurtDisiBrokerlar = new SelectList(_TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel(false);

            model.paraBirimleri = new SelectList(_TVMService.GetParaBirimleri(), "Id", "Birimi").ListWithOptionLabel(false);
            model.paraBirimleri.RemoveAt(0);

            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapGirisi, SekmeKodu = 0)]
        public ActionResult CariHesapEkle(CariHesapEkleModel model)
        {
            try
            {

                var carikodu = model.CariHesapTipi + model.KimlikNo;
                var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(carikodu, _AktifKullaniciService.TVMKodu);
                if (carihesap.Count > 0)
                {
                    // cari hesap var hatası versin
                    ModelState.AddModelError("CariHesapKodu", "Bu Cari Hesap Zaten Var!");
                    model.CariHesapTipleri = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "");
                    List<Ulke> ulkeler = _UlkeService.GetUlkeList();
                    List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
                    List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);
                    model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
                    model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
                    model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();
                    List<SigortaSirketleri> sigortalar = _SigortaSirketleriService.GetList();
                    model.sigortaSirketleri = new SelectList(sigortalar, "SirketKodu", "SirketAdi", model.sigortaSirketi).ListWithOptionLabel(false);
                    model.CariHesapKodu = model.CariHesapTipi + model.KimlikNo;

                    return View(model);

                }

            }
            catch (Exception)
            {

                throw;
            }

            try
            {
                CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                CariHesaplari cariHesapEkle = new CariHesaplari();
                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                model.CariHesapTipleri = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "");
                List<Ulke> ulkeler = _UlkeService.GetUlkeList();
                List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
                List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);
                List<SigortaSirketleri> sigortalar = _SigortaSirketleriService.GetList();
                model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();
                var sirketKodu = sigortalar.FirstOrDefault().SirketKodu;
                model.sigortaSirketleri = new SelectList(sigortalar, "SirketKodu", "SirketAdi", sirketKodu).ListWithOptionLabel(false);


                if (ModelState.IsValid)
                {

                    #region 320 hesap Cari hesap ekleme
                    cariHesapEkle.TVMKodu = model.TVMKodu;
                    cariHesapEkle.UlkeKodu = model.UlkeKodu;
                    cariHesapEkle.TVMUnvani = model.TVMUnvani;
                    cariHesapEkle.IlKodu = model.IlKodu;
                    cariHesapEkle.IlceKodu = model.IlceKodu;
                    cariHesapEkle.KayitTarihi = TurkeyDateTime.Now;
                    cariHesapEkle.GuncellemeTarihi = TurkeyDateTime.Now;
                    cariHesapEkle.Adres = model.Adres;
                    cariHesapEkle.Unvan = model.Unvan;
                    cariHesapEkle.Telefon1 = model.Telefon1;
                    cariHesapEkle.Telefon2 = model.Telefon2;
                    cariHesapEkle.CepTel = model.CepTel;
                    cariHesapEkle.BilgiNotu = model.BilgiNotu;
                    cariHesapEkle.Email = model.Email;
                    cariHesapEkle.WebAdresi = model.WebAdresi;
                    cariHesapEkle.VergiDairesi = model.VergiDairesi;
                    cariHesapEkle.UyariNotu = model.UyariNotu;
                    cariHesapEkle.PostaKodu = model.PostaKodu;
                    cariHesapEkle.KimlikNo = model.KimlikNo;
                    cariHesapEkle.CariHesapTipi = model.CariHesapTipi;
                    cariHesapEkle.CariHesapKodu = model.CariHesapTipi + model.KimlikNo;
                    cariHesapEkle.MusteriGrupKodu = model.MusteriGrupKodu;
                    cariHesapEkle.DisaktarimCariKodu = model.DisaktarimCariKodu;
                    cariHesapEkle.DisaktarimMuhasebeKodu = model.DisaktarimMuhasebeKodu;
                    if (model.CariHesapTipi == "320.01.")
                    {
                        cariHesapEkle.KomisyonGelirleriMuhasebeKodu = "600.01." + model.KimlikNo;
                        cariHesapEkle.SatisIadeleriMuhasebeKodu = "610.01." + model.KimlikNo;
                    }
                    _Muhasebe_CariHesapService.CariHesapEkle(cariHesapEkle);
                    #endregion
                    if (model.CariHesapTipi == "320.01.")
                    {
                        #region 600.01 Cari hesap ekleme
                        cariHesapEkle.TVMKodu = model.TVMKodu;
                        cariHesapEkle.UlkeKodu = model.UlkeKodu;
                        cariHesapEkle.TVMUnvani = model.TVMUnvani;
                        cariHesapEkle.IlKodu = model.IlKodu;
                        cariHesapEkle.IlceKodu = model.IlceKodu;
                        cariHesapEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapEkle.GuncellemeTarihi = TurkeyDateTime.Now;
                        cariHesapEkle.Adres = model.Adres;
                        cariHesapEkle.Unvan = model.Unvan;
                        cariHesapEkle.Telefon1 = model.Telefon1;
                        cariHesapEkle.Telefon2 = model.Telefon2;
                        cariHesapEkle.CepTel = model.CepTel;
                        cariHesapEkle.BilgiNotu = model.BilgiNotu;
                        cariHesapEkle.Email = model.Email;
                        cariHesapEkle.WebAdresi = model.WebAdresi;
                        cariHesapEkle.VergiDairesi = model.VergiDairesi;
                        cariHesapEkle.UyariNotu = model.UyariNotu;
                        cariHesapEkle.PostaKodu = model.PostaKodu;
                        cariHesapEkle.KimlikNo = model.KimlikNo;
                        cariHesapEkle.CariHesapTipi = model.CariHesapTipi;
                        cariHesapEkle.CariHesapKodu = "600.01." + model.KimlikNo;
                        cariHesapEkle.MusteriGrupKodu = model.MusteriGrupKodu;
                        cariHesapEkle.DisaktarimCariKodu = model.DisaktarimCariKodu;
                        cariHesapEkle.DisaktarimMuhasebeKodu = model.DisaktarimMuhasebeKodu;

                        _Muhasebe_CariHesapService.CariHesapEkle(cariHesapEkle);
                        #endregion
                        #region 610.01 Cari hesap ekleme
                        cariHesapEkle.TVMKodu = model.TVMKodu;
                        cariHesapEkle.UlkeKodu = model.UlkeKodu;
                        cariHesapEkle.TVMUnvani = model.TVMUnvani;
                        cariHesapEkle.IlKodu = model.IlKodu;
                        cariHesapEkle.IlceKodu = model.IlceKodu;
                        cariHesapEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapEkle.GuncellemeTarihi = TurkeyDateTime.Now;
                        cariHesapEkle.Adres = model.Adres;
                        cariHesapEkle.Unvan = model.Unvan;
                        cariHesapEkle.Telefon1 = model.Telefon1;
                        cariHesapEkle.Telefon2 = model.Telefon2;
                        cariHesapEkle.CepTel = model.CepTel;
                        cariHesapEkle.BilgiNotu = model.BilgiNotu;
                        cariHesapEkle.Email = model.Email;
                        cariHesapEkle.WebAdresi = model.WebAdresi;
                        cariHesapEkle.VergiDairesi = model.VergiDairesi;
                        cariHesapEkle.UyariNotu = model.UyariNotu;
                        cariHesapEkle.PostaKodu = model.PostaKodu;
                        cariHesapEkle.KimlikNo = model.KimlikNo;
                        cariHesapEkle.CariHesapTipi = model.CariHesapTipi;
                        cariHesapEkle.CariHesapKodu = "610.01." + model.KimlikNo;
                        cariHesapEkle.MusteriGrupKodu = model.MusteriGrupKodu;
                        cariHesapEkle.DisaktarimCariKodu = model.DisaktarimCariKodu;
                        cariHesapEkle.DisaktarimMuhasebeKodu = model.DisaktarimMuhasebeKodu;

                        _Muhasebe_CariHesapService.CariHesapEkle(cariHesapEkle);
                        #endregion
                    }

                    if (cariHesapEkle.CariHesapKodu != null)
                    {
                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                        cariHesapBorcAlacakEkle.CariHesapId = cariHesapEkle.CariHesapId;
                        cariHesapBorcAlacakEkle.Donem = Convert.ToInt32(TurkeyDateTime.Now.Year);
                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                        cariHesapBorcAlacakEkle.Borc1 = 0;
                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                        cariHesapBorcAlacakEkle.Borc2 = 0;
                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                        cariHesapBorcAlacakEkle.Borc3 = 0;
                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                        cariHesapBorcAlacakEkle.Borc4 = 0;
                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                        cariHesapBorcAlacakEkle.Borc5 = 0;
                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                        cariHesapBorcAlacakEkle.Borc6 = 0;
                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                        cariHesapBorcAlacakEkle.Borc7 = 0;
                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                        cariHesapBorcAlacakEkle.Borc8 = 0;
                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                        cariHesapBorcAlacakEkle.Borc9 = 0;
                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                        cariHesapBorcAlacakEkle.Borc10 = 0;
                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                        cariHesapBorcAlacakEkle.Borc11 = 0;
                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                        cariHesapBorcAlacakEkle.Borc12 = 0;
                        _Muhasebe_CariHesapService.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);

                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                        cariHesapBorcAlacakEkle.CariHesapId = cariHesapEkle.CariHesapId;
                        cariHesapBorcAlacakEkle.Donem = Convert.ToInt32(TurkeyDateTime.Now.Year + 1);
                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                        cariHesapBorcAlacakEkle.Borc1 = 0;
                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                        cariHesapBorcAlacakEkle.Borc2 = 0;
                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                        cariHesapBorcAlacakEkle.Borc3 = 0;
                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                        cariHesapBorcAlacakEkle.Borc4 = 0;
                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                        cariHesapBorcAlacakEkle.Borc5 = 0;
                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                        cariHesapBorcAlacakEkle.Borc6 = 0;
                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                        cariHesapBorcAlacakEkle.Borc7 = 0;
                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                        cariHesapBorcAlacakEkle.Borc8 = 0;
                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                        cariHesapBorcAlacakEkle.Borc9 = 0;
                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                        cariHesapBorcAlacakEkle.Borc10 = 0;
                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                        cariHesapBorcAlacakEkle.Borc11 = 0;
                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                        cariHesapBorcAlacakEkle.Borc12 = 0;
                        _Muhasebe_CariHesapService.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                    }
                    TempData["eklendiMi"] = "true";

                    return RedirectToAction("CariHesapEkle", "Muhasebe");

                }
            }
            catch (Exception)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }

            return View(model);
        }
        public ActionResult CariHesapEklePartial()
        {
            CariHesapEklePartialModel model = new CariHesapEklePartialModel();


            return PartialView("_CariHesapEklePartial", model);
        }

        public ActionResult GetHesapEklePartial(CariHesapEklePartialModel model)
        {

            CariHesapEkleModel hesapModel = new CariHesapEkleModel();
            try
            {
                var hesapDetay = _Muhasebe_CariHesapService.GetCariKodu(model.CariHesapKodu);

                if (hesapDetay != null)
                {
                    SetHesapModel(ref hesapModel, ref hesapDetay);
                }
            }
            catch (Exception ex)
            {
                //policeModel.IslemMesaji = "Poliçe görüntüleme işleminde bir hata oluştu." + ex.Message.ToString();
                //return Json(new { HataMesaji = policeModel.IslemMesaji }, JsonRequestBehavior.AllowGet);
                return new RedirectResult("~/Error/ErrorPage/500");
                throw ex;
            }
            return Json(new { success = true, data = hesapModel }, JsonRequestBehavior.AllowGet);
        }

        private void SetHesapModel(ref CariHesapEkleModel hesapModel, ref CariHesaplari hesapDetay)
        {
            hesapModel.CariHesapTipi = hesapDetay.CariHesapTipi;
            hesapModel.CariHesapKodu = hesapDetay.CariHesapKodu;
            hesapModel.KimlikNo = hesapDetay.KimlikNo;
            hesapModel.MusteriGrupKodu = hesapDetay.MusteriGrupKodu;
            hesapModel.Unvan = hesapDetay.Unvan;
            hesapModel.VergiDairesi = hesapDetay.VergiDairesi;
            hesapModel.Telefon1 = hesapDetay.Telefon1;
            hesapModel.Telefon2 = hesapDetay.Telefon2;
            hesapModel.CepTel = hesapDetay.CepTel;
            hesapModel.Email = hesapDetay.Email;
            hesapModel.Adres = hesapDetay.Adres;
            hesapModel.DisaktarimMuhasebeKodu = hesapDetay.DisaktarimMuhasebeKodu;
            hesapModel.DisaktarimCariKodu = hesapDetay.DisaktarimCariKodu;
            hesapModel.KomisyonGelirleriMuhasebeKodu = hesapDetay.KomisyonGelirleriMuhasebeKodu;
            hesapModel.SatisIadeleriMuhasebeKodu = hesapDetay.SatisIadeleriMuhasebeKodu;
            hesapModel.PostaKodu = hesapDetay.PostaKodu;
            hesapModel.UlkeKodu = hesapDetay.UlkeKodu;
            hesapModel.IlKodu = hesapDetay.IlKodu;
            hesapModel.IlceKodu = hesapDetay.IlceKodu;
            hesapModel.BilgiNotu = hesapDetay.BilgiNotu;
            hesapModel.UyariNotu = hesapDetay.UyariNotu;

        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapGirisi, SekmeKodu = 0)]
        public ActionResult CariHesapDetay(int id)
        {
            CariHesaplari cariDetay = _Muhasebe_CariHesapService.GetCariDetayYetkili(id);
            CariHesapDetayModel detayModel = new CariHesapDetayModel();
            try
            {
                if (cariDetay != null)
                {
                    Ulke ulke = _UlkeService.GetUlke(cariDetay.UlkeKodu);
                    if (ulke != null)
                        detayModel.UlkeAdi = ulke.UlkeAdi;

                    Il il = _UlkeService.GetIl(cariDetay.UlkeKodu, cariDetay.IlKodu);
                    if (il != null)
                        detayModel.IlAdi = il.IlAdi;

                    if (cariDetay.IlceKodu != null)
                    {
                        Ilce ilce = _UlkeService.GetIlce(cariDetay.IlceKodu);
                        detayModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                    }
                    else
                    {
                        detayModel.IlceAdi = string.Empty;
                    }
                    detayModel.CariHesapTipleri = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "");
                    detayModel.TVMUnvani = cariDetay.TVMUnvani;
                    detayModel.Unvan = cariDetay.Unvan;
                    detayModel.Telefon1 = cariDetay.Telefon1;
                    detayModel.Telefon2 = cariDetay.Telefon2;
                    detayModel.CepTel = cariDetay.CepTel;
                    detayModel.Email = cariDetay.Email;
                    detayModel.WebAdresi = cariDetay.WebAdresi;
                    detayModel.UyariNotu = cariDetay.UyariNotu;
                    detayModel.BilgiNotu = cariDetay.BilgiNotu;
                    detayModel.VergiDairesi = cariDetay.VergiDairesi;
                    detayModel.TVMKodu = cariDetay.TVMKodu;
                    detayModel.PostaKodu = cariDetay.PostaKodu;
                    detayModel.DisaktarimCariKodu = cariDetay.DisaktarimCariKodu;
                    detayModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(cariDetay.CariHesapTipi.ToString());
                    detayModel.DisaktarimMuhasebeKodu = cariDetay.DisaktarimMuhasebeKodu;
                    detayModel.DisaktarimCariKodu = cariDetay.DisaktarimCariKodu;
                    detayModel.KomisyonGelirleriMuhasebeKodu = cariDetay.KomisyonGelirleriMuhasebeKodu;
                    detayModel.KimlikNo = cariDetay.KimlikNo;
                    detayModel.GuncellemeTarihi = TurkeyDateTime.Now;
                    detayModel.Adres = cariDetay.Adres;
                    detayModel.CariHesapKodu = cariDetay.CariHesapKodu;
                    detayModel.MusteriGrupKodu = cariDetay.MusteriGrupKodu;
                    detayModel.SatisIadeleriMuhasebeKodu = cariDetay.SatisIadeleriMuhasebeKodu;
                    detayModel.Id = cariDetay.CariHesapId;
                }

            }
            catch (Exception)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            return View(detayModel);
        }

        [AjaxException]
        public ActionResult GetCariHesaplar(string hesapAdi)
        {
            try
            {
                var hesapList = _Muhasebe_CariHesapService.GetCariHesapList(hesapAdi);
                if (hesapList.Count > 0)

                {
                    hesapList = hesapList.OrderBy(s => s.Unvan).ToList();
                    var hesaplar = new SelectList(hesapList, "CariHesapKodu", "Unvan", hesapList[0].CariHesapKodu);
                    return Json(new { success = true, list = hesaplar }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });

                    return Json(new { success = false, list = list }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }
        public ActionResult CariHesapGuncelle(int id)
        {
            CariHesapGuncelleModel model = new CariHesapGuncelleModel();

            model.TVMKodu = _AktifKullaniciService.TVMKodu;

            model.CariHesapTipleri = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "");

            CariHesaplari cariDetay = _Muhasebe_CariHesapService.GetCariDetayYetkili(id);
            CariHesapDetayModel detayModel = new CariHesapDetayModel();

            if (cariDetay != null)
            {
                model.IlKodu = cariDetay.IlKodu;
                model.IlceKodu = cariDetay.IlceKodu;
                model.UlkeKodu = cariDetay.UlkeKodu;
                List<Ulke> ulkeler = _UlkeService.GetUlkeList();
                List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
                List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);
                model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", cariDetay.UlkeKodu).ListWithOptionLabel();
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", cariDetay.IlKodu).ListWithOptionLabelIller();
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", cariDetay.IlceKodu).ListWithOptionLabel();
                List<SigortaSirketleri> sigortalar = _SigortaSirketleriService.GetList();
                var sirketKodu = sigortalar.FirstOrDefault().SirketKodu;
                model.sigortaSirketleri = new SelectList(sigortalar, "SirketKodu", "SirketAdi", sirketKodu).ListWithOptionLabel(false);
                model.TVMUnvani = cariDetay.TVMUnvani;
                model.Unvan = cariDetay.Unvan;
                model.Telefon1 = cariDetay.Telefon1;
                model.Telefon2 = cariDetay.Telefon2;
                model.CepTel = cariDetay.CepTel;
                model.Email = cariDetay.Email;
                model.WebAdresi = cariDetay.WebAdresi;
                model.UyariNotu = cariDetay.UyariNotu;
                model.BilgiNotu = cariDetay.BilgiNotu;
                model.VergiDairesi = cariDetay.VergiDairesi;
                model.TVMKodu = cariDetay.TVMKodu;
                model.PostaKodu = cariDetay.PostaKodu;
                model.DisaktarimCariKodu = cariDetay.DisaktarimCariKodu;
                model.CariHesapTipi = cariDetay.CariHesapTipi;
                model.DisaktarimMuhasebeKodu = cariDetay.DisaktarimMuhasebeKodu;
                model.KimlikNo = cariDetay.KimlikNo;
                model.GuncellemeTarihi = cariDetay.GuncellemeTarihi;
                model.Adres = cariDetay.Adres;
                model.CariHesapKodu = cariDetay.CariHesapKodu;
                if (model.CariHesapTipi == "320.01.")
                {
                    model.KomisyonGelirleriMuhasebeKodu = "600.01." + cariDetay.KimlikNo;
                    model.SatisIadeleriMuhasebeKodu = "610.01." + cariDetay.KimlikNo;
                }
                model.MusteriGrupKodu = cariDetay.MusteriGrupKodu;
                model.id = cariDetay.CariHesapId;
            }



            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CariHesapGuncelle(CariHesapGuncelleModel model)
        {

            try
            {

                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                CariHesaplari guncellenecekVeri = _Muhasebe_CariHesapService.GetCariDetayYetkili(model.id);
                if (ModelState.IsValid)
                {
                    if (guncellenecekVeri.Adres != model.Adres || guncellenecekVeri.BilgiNotu != model.BilgiNotu || guncellenecekVeri.CariHesapTipi != model.CariHesapTipi || guncellenecekVeri.CepTel != model.CepTel || guncellenecekVeri.DisaktarimCariKodu != model.DisaktarimCariKodu || guncellenecekVeri.DisaktarimMuhasebeKodu != model.DisaktarimMuhasebeKodu || guncellenecekVeri.Email != model.Email || guncellenecekVeri.IlceKodu != model.IlceKodu || guncellenecekVeri.IlKodu != model.IlKodu || guncellenecekVeri.KimlikNo != model.KimlikNo || guncellenecekVeri.KomisyonGelirleriMuhasebeKodu != model.KomisyonGelirleriMuhasebeKodu || guncellenecekVeri.MusteriGrupKodu != model.MusteriGrupKodu || guncellenecekVeri.PostaKodu != model.PostaKodu || guncellenecekVeri.Telefon1 != model.Telefon1 || guncellenecekVeri.Telefon2 != model.Telefon2 || guncellenecekVeri.UlkeKodu != model.UlkeKodu || guncellenecekVeri.Unvan != model.Unvan || guncellenecekVeri.UyariNotu != model.UyariNotu || guncellenecekVeri.VergiDairesi != model.VergiDairesi || guncellenecekVeri.WebAdresi != model.WebAdresi)
                    {
                        guncellenecekVeri.CariHesapId = model.id;
                        guncellenecekVeri.TVMKodu = model.TVMKodu;
                        guncellenecekVeri.UlkeKodu = model.UlkeKodu;
                        guncellenecekVeri.TVMUnvani = model.TVMUnvani;
                        guncellenecekVeri.IlKodu = model.IlKodu;
                        guncellenecekVeri.IlceKodu = model.IlceKodu;
                        guncellenecekVeri.KayitTarihi = guncellenecekVeri.KayitTarihi;
                        guncellenecekVeri.GuncellemeTarihi = TurkeyDateTime.Now;
                        guncellenecekVeri.Adres = model.Adres;
                        guncellenecekVeri.Unvan = model.Unvan;
                        guncellenecekVeri.Telefon1 = model.Telefon1;
                        guncellenecekVeri.Telefon2 = model.Telefon2;
                        guncellenecekVeri.CepTel = model.CepTel;
                        guncellenecekVeri.BilgiNotu = model.BilgiNotu;
                        guncellenecekVeri.Email = model.Email;
                        guncellenecekVeri.WebAdresi = model.WebAdresi;
                        guncellenecekVeri.VergiDairesi = model.VergiDairesi;
                        guncellenecekVeri.UyariNotu = model.UyariNotu;
                        guncellenecekVeri.PostaKodu = model.PostaKodu;
                        guncellenecekVeri.KimlikNo = model.KimlikNo;
                        guncellenecekVeri.CariHesapTipi = model.CariHesapTipi;
                        guncellenecekVeri.CariHesapKodu = model.CariHesapTipi + model.KimlikNo;
                        if (model.CariHesapTipi == "320.01.")
                        {
                            guncellenecekVeri.KomisyonGelirleriMuhasebeKodu = "600.01." + model.KimlikNo;
                            guncellenecekVeri.SatisIadeleriMuhasebeKodu = "610.01." + model.KimlikNo;
                        }
                        guncellenecekVeri.MusteriGrupKodu = model.MusteriGrupKodu;
                        guncellenecekVeri.DisaktarimCariKodu = model.DisaktarimCariKodu;
                        guncellenecekVeri.DisaktarimMuhasebeKodu = model.DisaktarimMuhasebeKodu;
                        _Muhasebe_CariHesapService.CariHesapGuncelle(guncellenecekVeri);



                    }
                    return RedirectToAction("CariHesapDetay", "Muhasebe", new { id = model.id });
                }

            }
            catch (Exception)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapAraGuncelle, SekmeKodu = 0)]
        public ActionResult CariHesapListesi()
        {
            CariHesapListesiAraModel model = new CariHesapListesiAraModel();
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapAraGuncelle, SekmeKodu = 0)]
        public ActionResult CariHesapListesi(CariHesapListesiAraModel model)
        {

            CariHesapListesiModel sonucModel;
            model.list = new List<CariHesapListesiModel>();
            var cariHesapTipleriList = new SelectList(HesapEkstresiModel.CariHesapTipleriModel(), "Value", "Text", "").ToList();
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            IUlkeService _Ulke = DependencyResolver.Current.GetService<IUlkeService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;



            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                #region Unvana göre
                if (model.Unvan != null && model.Unvan.Length > 2)
                {
                    var unvan = model.Unvan.TrimStart();
                    var liste = _Muhasebe_CariHesapService.getCariHesapListesiByUnvan(unvan, model.TVMKodu);
                    foreach (var item in liste)
                    {
                        sonucModel = new CariHesapListesiModel();
                        sonucModel.id = item.CariHesapId;
                        sonucModel.Adres = item.Adres;
                        sonucModel.BilgiNotu = item.BilgiNotu;
                        sonucModel.CariHesapKodu = item.CariHesapKodu;

                        sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(item.CariHesapTipi.ToString());
                        sonucModel.CepTel = item.CepTel;
                        sonucModel.DisaktarimCariKodu = item.DisaktarimCariKodu;
                        sonucModel.DisaktarimMuhasebeKodu = item.DisaktarimMuhasebeKodu;
                        sonucModel.Email = item.Email;
                        sonucModel.GuncellemeTarihi = item.GuncellemeTarihi;

                        Il il = _UlkeService.GetIl(item.UlkeKodu, item.IlKodu);
                        if (il != null)
                            sonucModel.IlAdi = il.IlAdi;

                        if (sonucModel.IlceKodu != null)
                        {
                            Ilce ilce = _UlkeService.GetIlce(item.IlceKodu);
                            sonucModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                        }
                        sonucModel.KayitTarihi = item.KayitTarihi;
                        /////////////////////////////////////////////////
                        sonucModel.KimlikNo = item.KimlikNo;
                        sonucModel.KomisyonGelirleriMuhasebeKodu = item.KomisyonGelirleriMuhasebeKodu;
                        sonucModel.MusteriGrupKodu = item.MusteriGrupKodu;
                        sonucModel.PostaKodu = item.PostaKodu;
                        sonucModel.Telefon1 = item.Telefon1;
                        sonucModel.Telefon2 = item.Telefon2;
                        sonucModel.Unvan = item.Unvan;
                        sonucModel.UyariNotu = item.UyariNotu;
                        sonucModel.WebAdresi = item.WebAdresi;
                        sonucModel.VergiDairesi = item.VergiDairesi;
                        model.list.Add(sonucModel);
                    }


                }


                #endregion

                #region Müşteri  grub koduna göre
                if (model.MusteriGrupKodu != null && model.MusteriGrupKodu.Length > 2)
                {
                    var musteriGrupKodu = model.MusteriGrupKodu.TrimStart();
                    var liste = _Muhasebe_CariHesapService.getCariHesapListesiByMusteriGrupKodu(musteriGrupKodu, model.TVMKodu);
                    foreach (var item in liste)
                    {
                        sonucModel = new CariHesapListesiModel();
                        sonucModel.Adres = item.Adres;
                        sonucModel.id = item.CariHesapId;
                        sonucModel.BilgiNotu = item.BilgiNotu;
                        sonucModel.CariHesapKodu = item.CariHesapKodu;
                        sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(item.CariHesapTipi.ToString());
                        Il il = _UlkeService.GetIl(item.UlkeKodu, item.IlKodu);
                        if (il != null)
                            sonucModel.IlAdi = il.IlAdi;

                        if (sonucModel.IlceKodu != null)
                        {
                            Ilce ilce = _UlkeService.GetIlce(item.IlceKodu);
                            sonucModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                        }
                        sonucModel.CepTel = item.CepTel;
                        sonucModel.DisaktarimCariKodu = item.DisaktarimCariKodu;
                        sonucModel.DisaktarimMuhasebeKodu = item.DisaktarimMuhasebeKodu;
                        sonucModel.Email = item.Email;
                        sonucModel.GuncellemeTarihi = item.GuncellemeTarihi;
                        sonucModel.IlKodu = item.IlKodu;
                        sonucModel.IlceKodu = item.IlceKodu;
                        sonucModel.KayitTarihi = item.KayitTarihi;
                        ///////////////////////////////////////////////// tckn vkn
                        sonucModel.KimlikNo = item.KimlikNo;
                        sonucModel.KomisyonGelirleriMuhasebeKodu = item.KomisyonGelirleriMuhasebeKodu;
                        sonucModel.MusteriGrupKodu = item.MusteriGrupKodu;
                        sonucModel.PostaKodu = item.PostaKodu;
                        sonucModel.Telefon1 = item.Telefon1;
                        sonucModel.Telefon2 = item.Telefon2;
                        sonucModel.Unvan = item.Unvan;
                        sonucModel.UyariNotu = item.UyariNotu;
                        sonucModel.WebAdresi = item.WebAdresi;
                        sonucModel.VergiDairesi = item.VergiDairesi;
                        model.list.Add(sonucModel);
                    }


                }
                #endregion

                #region cari hesap koduna göre
                if (model.CariHesapKodu != null && model.CariHesapKodu.Length > 2)
                {

                    var cariHesapKodu = model.CariHesapKodu.Trim();
                    var carihesaptipi = cariHesapKodu.Substring(0, 3);
                    if (carihesaptipi != "120")
                    {
                        var liste = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(cariHesapKodu, model.TVMKodu);
                        foreach (var item in liste)
                        {
                            sonucModel = new CariHesapListesiModel();
                            sonucModel.Adres = item.Adres;
                            sonucModel.id = item.CariHesapId;
                            sonucModel.BilgiNotu = item.BilgiNotu;
                            sonucModel.CariHesapKodu = item.CariHesapKodu;
                            //for (int i = 0; i < cariHesapTipleriList.Count(); i++)
                            //{
                            //    if (item.CariHesapTipi.ToString() == cariHesapTipleriList[i].Value)
                            //        sonucModel.CariHesapTipiAdi = cariHesapTipleriList[i].Text;
                            //}
                            sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(item.CariHesapTipi.ToString());
                            Il il = _UlkeService.GetIl(item.UlkeKodu, item.IlKodu);
                            if (il != null)
                                sonucModel.IlAdi = il.IlAdi;

                            if (sonucModel.IlceKodu != null)
                            {
                                Ilce ilce = _UlkeService.GetIlce(item.IlceKodu);
                                sonucModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                            }
                            sonucModel.CepTel = item.CepTel;
                            sonucModel.DisaktarimCariKodu = item.DisaktarimCariKodu;
                            sonucModel.DisaktarimMuhasebeKodu = item.DisaktarimMuhasebeKodu;
                            sonucModel.Email = item.Email;
                            sonucModel.GuncellemeTarihi = item.GuncellemeTarihi;
                            sonucModel.IlKodu = item.IlKodu;
                            sonucModel.IlceKodu = item.IlceKodu;
                            sonucModel.KayitTarihi = item.KayitTarihi;
                            ///////////////////////////////////////////////// tckn vkn
                            sonucModel.KimlikNo = item.KimlikNo;
                            sonucModel.KomisyonGelirleriMuhasebeKodu = item.KomisyonGelirleriMuhasebeKodu;
                            sonucModel.MusteriGrupKodu = item.MusteriGrupKodu;
                            sonucModel.PostaKodu = item.PostaKodu;
                            sonucModel.Telefon1 = item.Telefon1;
                            sonucModel.Telefon2 = item.Telefon2;
                            sonucModel.Unvan = item.Unvan;
                            sonucModel.UyariNotu = item.UyariNotu;
                            sonucModel.WebAdresi = item.WebAdresi;
                            sonucModel.VergiDairesi = item.VergiDairesi;
                            model.list.Add(sonucModel);
                        }


                    }
                    else if (carihesaptipi == "120" && cariHesapKodu.Length <= 10)
                    {
                        var liste = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(cariHesapKodu, model.TVMKodu);
                        foreach (var item in liste)
                        {
                            sonucModel = new CariHesapListesiModel();
                            sonucModel.Adres = item.Adres;
                            sonucModel.id = item.CariHesapId;
                            sonucModel.BilgiNotu = item.BilgiNotu;
                            sonucModel.CariHesapKodu = item.CariHesapKodu;
                            //for (int i = 0; i < cariHesapTipleriList.Count(); i++)
                            //{
                            //    if (item.CariHesapTipi.ToString() == cariHesapTipleriList[i].Value)
                            //        sonucModel.CariHesapTipiAdi = cariHesapTipleriList[i].Text;
                            //}
                            sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(item.CariHesapTipi.ToString());
                            Il il = _UlkeService.GetIl(item.UlkeKodu, item.IlKodu);
                            if (il != null)
                                sonucModel.IlAdi = il.IlAdi;

                            if (sonucModel.IlceKodu != null)
                            {
                                Ilce ilce = _UlkeService.GetIlce(item.IlceKodu);
                                sonucModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                            }
                            sonucModel.CepTel = item.CepTel;
                            sonucModel.DisaktarimCariKodu = item.DisaktarimCariKodu;
                            sonucModel.DisaktarimMuhasebeKodu = item.DisaktarimMuhasebeKodu;
                            sonucModel.Email = item.Email;
                            sonucModel.GuncellemeTarihi = item.GuncellemeTarihi;
                            sonucModel.IlKodu = item.IlKodu;
                            sonucModel.IlceKodu = item.IlceKodu;
                            sonucModel.KayitTarihi = item.KayitTarihi;
                            ///////////////////////////////////////////////// tckn vkn
                            sonucModel.KimlikNo = item.KimlikNo;
                            sonucModel.KomisyonGelirleriMuhasebeKodu = item.KomisyonGelirleriMuhasebeKodu;
                            sonucModel.MusteriGrupKodu = item.MusteriGrupKodu;
                            sonucModel.PostaKodu = item.PostaKodu;
                            sonucModel.Telefon1 = item.Telefon1;
                            sonucModel.Telefon2 = item.Telefon2;
                            sonucModel.Unvan = item.Unvan;
                            sonucModel.UyariNotu = item.UyariNotu;
                            sonucModel.WebAdresi = item.WebAdresi;
                            sonucModel.VergiDairesi = item.VergiDairesi;
                            model.list.Add(sonucModel);
                        }


                    }
                    //else if (carihesaptipi == "120" && cariHesapKodu.Length < 10)
                    //{
                    //    ModelState.AddModelError("Uyarı", "Müşteri hesaplarında en az 10 karakter(120.01.123) girilmelidir.");
                    //}
                }



                #endregion

                #region tüm veriler için

                if (model.Unvan == null && model.MusteriGrupKodu == null && model.CariHesapKodu == null)
                {

                    var _liste = _Muhasebe_CariHesapService.getCariHesapListesiAll(model.TVMKodu);
                    foreach (var item in _liste)
                    {
                        sonucModel = new CariHesapListesiModel();
                        sonucModel.Adres = item.Adres;
                        sonucModel.id = item.CariHesapId;
                        sonucModel.BilgiNotu = item.BilgiNotu;
                        sonucModel.CariHesapKodu = item.CariHesapKodu;
                        sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(item.CariHesapTipi.ToString());
                        Il il = _UlkeService.GetIl(item.UlkeKodu, item.IlKodu);
                        if (il != null)
                            sonucModel.IlAdi = il.IlAdi;

                        if (sonucModel.IlceKodu != null)
                        {
                            Ilce ilce = _UlkeService.GetIlce(item.IlceKodu);
                            sonucModel.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                        }
                        sonucModel.CepTel = item.CepTel;
                        sonucModel.DisaktarimCariKodu = item.DisaktarimCariKodu;
                        sonucModel.DisaktarimMuhasebeKodu = item.DisaktarimMuhasebeKodu;
                        sonucModel.Email = item.Email;
                        sonucModel.GuncellemeTarihi = item.GuncellemeTarihi;
                        sonucModel.IlKodu = item.IlKodu;
                        sonucModel.IlceKodu = item.IlceKodu;
                        sonucModel.KayitTarihi = item.KayitTarihi;
                        ///////////////////////////////////////////////// tckn vkn
                        sonucModel.KimlikNo = item.KimlikNo;
                        sonucModel.KomisyonGelirleriMuhasebeKodu = item.KomisyonGelirleriMuhasebeKodu;
                        sonucModel.MusteriGrupKodu = item.MusteriGrupKodu;
                        sonucModel.PostaKodu = item.PostaKodu;
                        sonucModel.Telefon1 = item.Telefon1;
                        sonucModel.Telefon2 = item.Telefon2;
                        sonucModel.Unvan = item.Unvan;
                        sonucModel.UyariNotu = item.UyariNotu;
                        sonucModel.WebAdresi = item.WebAdresi;
                        sonucModel.VergiDairesi = item.VergiDairesi;
                        model.list.Add(sonucModel);
                    }
                }
                #endregion





            }

            return View(model);
        }
        #endregion
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapAraGuncelle, SekmeKodu = 0)]
        public ActionResult CariHesapSil(string cariHesapKodu)
        {

            try
            {

                var cariHesapBorcAlacakList = _Muhasebe_CariHesapService.CariHesapBorcAlacakGetirByCariHesapKodu(cariHesapKodu);
                var cariHesap = _Muhasebe_CariHesapService.GetCariHesap(cariHesapKodu);
                bool cariHareketVarmi = false;

                foreach (var cariHesapBorcAlacak in cariHesapBorcAlacakList)
                {
                    #region borc
                    if (cariHesapBorcAlacak.Borc1 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc2 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc3 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc4 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc5 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc6 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc7 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc8 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc9 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc10 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc11 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Borc12 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    #endregion

                    #region alacak
                    if (cariHesapBorcAlacak.Alacak1 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak2 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak3 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak4 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak5 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak6 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak7 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak8 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak9 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak10 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak11 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    else if (cariHesapBorcAlacak.Alacak12 != 0)
                    {
                        cariHareketVarmi = true;
                    }
                    #endregion

                }
                string islemMesaji;
                if (cariHareketVarmi)
                {
                    islemMesaji = "Cari hareket kaydı bulunduğundan silinemez.";
                    return Json(new { message = islemMesaji });
                    // return Json(new { success = false, message = "Cari hareket kaydı bulunduğundan silinemez." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var cariHesapBorcAlacak in cariHesapBorcAlacakList)
                    {
                        _Muhasebe_CariHesapService.DeleteCariHesapBorcAlacak(cariHesapBorcAlacak.Id);
                    }
                    _Muhasebe_CariHesapService.DeleteCariHesap(cariHesap.CariHesapId);
                    islemMesaji = "Cari Hesap Silinmiştir.";
                    return Json(new { success = true, message = islemMesaji });
                    //return Json(new { success = true, message = "Cari Hesap Silinmiştir." }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }



        }



        [AjaxException]
        public ActionResult GetSigortaBilgileri(string SirketKodu)
        {
            try
            {

                var sigortaSirketiBilgileri = _SigortaSirketleriService.GetSigortaBilgileri(SirketKodu);
                var result = new { success = true, SirketinAdi = sigortaSirketiBilgileri.SirketAdi, VergiDairesi = sigortaSirketiBilgileri.VergiDairesi.ToString(), VergiNo = sigortaSirketiBilgileri.VergiNumarasi.ToString() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceleriCHAktar, SekmeKodu = 0)]
        public ActionResult CariHesaplaraAktar()
        {
            CariHesaplaraAktarModel model = new CariHesaplaraAktarModel();
            model.TvmKodu = _AktifKullaniciService.TVMKodu;
            model.TanzimBasTarihi = TurkeyDateTime.Now;
            model.TanzimBitTarihi = TurkeyDateTime.Now.AddDays(2);
            ViewBag.AktarimYok = true; //Cari Aktarim Hata tablosu gösterilmemesi için kullanılıyor
            ViewBag.IslemBasarili = false;
            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceleriCHAktar, SekmeKodu = 0)]
        public ActionResult CariHesaplaraAktar(CariHesaplaraAktarModel model)
        {
            if (model.TanzimBasTarihi.Year != model.TanzimBitTarihi.Year || model.TanzimBasTarihi.Month != model.TanzimBitTarihi.Month)
            {
                ModelState.AddModelError("TanzimBitTarihi", "Lütfen aynı ay ve dönemde tarih aralığı giriniz.");
            }

            if (ModelState.IsValid)
            {
                var aktarim = _Muhasebe_CariHesapService.CariAktarimIslemleri(model.TvmKodu, model.TanzimBasTarihi, model.TanzimBitTarihi);
                if (aktarim != null && aktarim.Count > 0)
                {
                    var basarizKayitlar = aktarim.Where(s => s.Basarili == false).ToList();

                    var sigortaSirketleri = _SigortaSirketleriService.GetList();
                    if (basarizKayitlar.Count > 0)
                    {
                        foreach (var item in basarizKayitlar)
                        {
                            var cariHesaDetay = _Muhasebe_CariHesapService.GetCariHesapAdi(item.CariHesapKodu);
                            item.CariHesapAdi = cariHesaDetay.Item1 + cariHesaDetay.Item2;
                            var sirketDetay = sigortaSirketleri.Where(w => w.SirketKodu == item.SigortaSirketKodu).FirstOrDefault();
                            if (sirketDetay != null)
                            {
                                item.SigortaSirketUnvan = sirketDetay.SirketAdi;
                                item.TvmUnvan = _AktifKullaniciService.TVMUnvani;
                            }
                            model.returnList.Add(item);
                            _Muhasebe_CariHesapService.CariAktarimLogAdd(model.returnList);
                            ViewBag.AktarimYok = false;
                        }
                    }
                    else
                    {
                        ViewBag.AktarimYok = false;
                        ViewBag.IslemBasarili = true;
                        ViewBag.AktarimKaydiMesaj = "Cari aktarım işlemi başarılı bir şekilde gerçekleştirilmiştir";
                    }
                }
                else
                {
                    model.returnList = new List<Muhasebe_CariHesapService.CariHesapBAReturnModel>();
                    ViewBag.AktarimYok = true;
                    ViewBag.AktarimKaydiMesaj = "Girilen tarihte poliçe / tahsilat / komisyon bulunamadı.";
                }
            }
            else
            {
                model.returnList = new List<Muhasebe_CariHesapService.CariHesapBAReturnModel>();
                ViewBag.AktarimYok = true;
                ViewBag.AktarimKaydiMesaj = "Lütfen girdiğiniz bilgileri kontrol ediniz. ";
            }
            return View(model);
        }

        public ActionResult CarileriOtomatikAktar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CariHesaplariYarat()
        {
            List<Muhasebe_CariHesapService.CariHesapBAReturnModel> returnList = new List<Muhasebe_CariHesapService.CariHesapBAReturnModel>();
            var aktarim = _Muhasebe_CariHesapService.CariAktarimIslemleri(100);
            if (aktarim != null && aktarim.Count > 0)
            {
                var basarizKayitlar = aktarim.Where(s => s.Basarili == false).ToList();

                var sigortaSirketleri = _SigortaSirketleriService.GetList();
                if (basarizKayitlar.Count > 0)
                {
                    foreach (var item in basarizKayitlar)
                    {
                        var cariHesaDetay = _Muhasebe_CariHesapService.GetCariHesapAdi(item.CariHesapKodu);
                        item.CariHesapAdi = cariHesaDetay.Item1 + cariHesaDetay.Item2;
                        var sirketDetay = sigortaSirketleri.Where(w => w.SirketKodu == item.SigortaSirketKodu).FirstOrDefault();
                        if (sirketDetay != null)
                        {
                            item.SigortaSirketUnvan = sirketDetay.SirketAdi;
                            item.TvmUnvan = _AktifKullaniciService.TVMUnvani;
                        }
                        returnList.Add(item);
                        _Muhasebe_CariHesapService.CariAktarimLogAdd(returnList);

                    }
                }
            }
            return Json(new { succes = true });
        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapMizani, SekmeKodu = 0)]
        public ActionResult CariHesapMizani()
        {
            CariHesapARAMizani model = new CariHesapARAMizani();
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 1; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donem = TurkeyDateTime.Now.Year;
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Ay = TurkeyDateTime.Now.Month;
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();

            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapMizani, SekmeKodu = 0)]
        public ActionResult CariHesapMizani(CariHesapARAMizani model)
        {
            model.BakiyeAyToplam = 0;
            model.CariAyAlacakToplam = 0;
            model.CariAyBorcToplam = 0;
            model.KumulatifAlacakToplam = 0;
            model.KumulatifBakiyeToplam = 0;
            model.KumulatifBorcToplam = 0;


            CariHesapMizani sonucModel;
            model.list = new List<CariHesapMizani>();

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 1; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();


            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                #region carihesapkoduna göre arama
                if (!String.IsNullOrEmpty(model.CariHesapKodu))
                {
                    var cariHesapKodu = model.CariHesapKodu.Trim();
                    var carihesaptipi = cariHesapKodu.Substring(0, 3);
                    if (true)
                    {
                        var veri = _Muhasebe_CariHesapService.CariHesapBorcAlacakGetirByCariHesapKodu(model.Donem, model.CariHesapKodu);
                        foreach (var item in veri)
                        {
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(item.CariHesapKodu, item.TVMKodu);
                            sonucModel = new CariHesapMizani();
                            sonucModel.CariHesapKodu = item.CariHesapKodu;
                            sonucModel.Unvan = carihesap.First().Unvan;
                            sonucModel.MusteriGrupKodu = carihesap.First().MusteriGrupKodu;
                            sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(carihesap.First().CariHesapTipi.ToString());
                            sonucModel.id = item.Id;
                            sonucModel.KimlikNo = item.KimlikNo;
                            sonucModel.TVMKodu = item.TVMKodu;
                            sonucModel.VergiDairesi = carihesap.First().VergiDairesi;
                            switch (model.Ay)
                            {
                                case 1:
                                    sonucModel.CariAyAlacak = item.Alacak1;
                                    sonucModel.CariAyBorc = item.Borc1;
                                    sonucModel.BakiyeAy = item.Borc1 - item.Alacak1;
                                    model.CariAyAlacakToplam += item.Alacak1;
                                    model.CariAyBorcToplam += item.Borc1;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1;
                                    sonucModel.KumulatifBorc = item.Borc1;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 2:
                                    sonucModel.CariAyAlacak = item.Alacak2;
                                    sonucModel.CariAyBorc = item.Borc2;
                                    sonucModel.BakiyeAy = item.Borc2 - item.Alacak2;
                                    model.CariAyAlacakToplam += item.Alacak2;
                                    model.CariAyBorcToplam += item.Borc2;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 3:
                                    sonucModel.CariAyAlacak = item.Alacak3;
                                    sonucModel.CariAyBorc = item.Borc3;
                                    sonucModel.BakiyeAy = item.Borc3 - item.Alacak3;
                                    model.CariAyAlacakToplam += item.Alacak3;
                                    model.CariAyBorcToplam += item.Borc3;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 4:
                                    sonucModel.CariAyAlacak = item.Alacak4;
                                    sonucModel.CariAyBorc = item.Borc4;
                                    sonucModel.BakiyeAy = item.Borc4 - item.Alacak4;
                                    model.CariAyAlacakToplam += item.Alacak4;
                                    model.CariAyBorcToplam += item.Borc4;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 5:
                                    sonucModel.CariAyAlacak = item.Alacak5;
                                    sonucModel.CariAyBorc = item.Borc5;
                                    sonucModel.BakiyeAy = item.Borc5 - item.Alacak5;
                                    model.CariAyAlacakToplam += item.Alacak5;
                                    model.CariAyBorcToplam += item.Borc5;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 6:
                                    sonucModel.CariAyAlacak = item.Alacak6;
                                    sonucModel.CariAyBorc = item.Borc6;
                                    sonucModel.BakiyeAy = item.Borc6 - item.Alacak6;
                                    model.CariAyAlacakToplam += item.Alacak6;
                                    model.CariAyBorcToplam += item.Borc6;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 7:
                                    sonucModel.CariAyAlacak = item.Alacak7;
                                    sonucModel.CariAyBorc = item.Borc7;
                                    sonucModel.BakiyeAy = item.Borc7 - item.Alacak7;
                                    model.CariAyAlacakToplam += item.Alacak7;
                                    model.CariAyBorcToplam += item.Borc7;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 8:
                                    sonucModel.CariAyAlacak = item.Alacak8;
                                    sonucModel.CariAyBorc = item.Borc8;
                                    sonucModel.BakiyeAy = item.Borc8 - item.Alacak8;
                                    model.CariAyAlacakToplam += item.Alacak8;
                                    model.CariAyBorcToplam += item.Borc8;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 9:
                                    sonucModel.CariAyAlacak = item.Alacak9;
                                    sonucModel.CariAyBorc = item.Borc9;
                                    sonucModel.BakiyeAy = item.Borc9 - item.Alacak9;
                                    model.CariAyAlacakToplam += item.Alacak9;
                                    model.CariAyBorcToplam += item.Borc9;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 10:
                                    sonucModel.CariAyAlacak = item.Alacak10;
                                    sonucModel.CariAyBorc = item.Borc10;
                                    sonucModel.BakiyeAy = item.Borc10 - item.Alacak10;
                                    model.CariAyAlacakToplam += item.Alacak10;
                                    model.CariAyBorcToplam += item.Borc10;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 11:
                                    sonucModel.CariAyAlacak = item.Alacak11;
                                    sonucModel.CariAyBorc = item.Borc11;
                                    sonucModel.BakiyeAy = item.Borc11 - item.Alacak11;
                                    model.CariAyAlacakToplam += item.Alacak11;
                                    model.CariAyBorcToplam += item.Borc11;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 12:
                                    sonucModel.CariAyAlacak = item.Alacak12;
                                    sonucModel.CariAyBorc = item.Borc12;
                                    sonucModel.BakiyeAy = item.Borc12 - item.Alacak12;
                                    model.CariAyAlacakToplam += item.Alacak12;
                                    model.CariAyBorcToplam += item.Borc12;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11 + item.Alacak12;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11 + item.Borc12;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                default:
                                    break;
                            }
                            model.KumulatifAlacakToplam += sonucModel.KumulatifAlacak;
                            model.KumulatifBorcToplam += sonucModel.KumulatifBorc;
                            model.KumulatifBakiyeToplam += sonucModel.KumulatifBakiye;
                            model.list.Add(sonucModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Uyarı", "Müşteri hesaplarında en az 10 karakter(120.01.123) girilmelidir.");
                    }
                }
                #endregion
                // müşteri grub kodu ve unvan için oluşturdum listcarihesaplari'ni 
                List<CariHesaplari> listCarihesaplari = new List<CariHesaplari>();
                #region müşteri grup koduna göre
                if (!String.IsNullOrEmpty(model.MusteriGrupKodu))
                {
                    listCarihesaplari = _Muhasebe_CariHesapService.getCariHesapListesiByMusteriGrupKodu(model.MusteriGrupKodu, _AktifKullaniciService.TVMKodu);

                }
                #endregion
                #region unvana göre arama
                if (!String.IsNullOrEmpty(model.Unvan))
                {
                    listCarihesaplari = _Muhasebe_CariHesapService.getCariHesapListesiByUnvan(model.Unvan, _AktifKullaniciService.TVMKodu);
                }
                #endregion
                #region müşteri grub koduna ve unvana göre arama yapılması 
                if (listCarihesaplari.Count > 0)
                {
                    foreach (var carihesabi in listCarihesaplari)
                    {
                        var veri = _Muhasebe_CariHesapService.CariHesapBorcAlacakGetirByCariHesapKodu(model.Donem, carihesabi.CariHesapKodu);
                        foreach (var item in veri)
                        {
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(item.CariHesapKodu, item.TVMKodu);
                            sonucModel = new CariHesapMizani();
                            sonucModel.CariHesapKodu = item.CariHesapKodu;
                            sonucModel.Unvan = carihesap.First().Unvan;
                            sonucModel.MusteriGrupKodu = carihesap.First().MusteriGrupKodu;
                            sonucModel.CariHesapTipiAdi = MuhasebeCommon.GetCariHesapTipAdiNoktali(carihesap.First().CariHesapTipi.ToString());
                            sonucModel.id = item.Id;
                            sonucModel.KimlikNo = item.KimlikNo;
                            sonucModel.TVMKodu = item.TVMKodu;
                            sonucModel.VergiDairesi = carihesap.First().VergiDairesi;
                            switch (model.Ay)
                            {
                                case 1:
                                    sonucModel.CariAyAlacak = item.Alacak1;
                                    sonucModel.CariAyBorc = item.Borc1;
                                    sonucModel.BakiyeAy = item.Borc1 - item.Alacak1;
                                    model.CariAyAlacakToplam += item.Alacak1;
                                    model.CariAyBorcToplam += item.Borc1;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1;
                                    sonucModel.KumulatifBorc = item.Borc1;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 2:
                                    sonucModel.CariAyAlacak = item.Alacak2;
                                    sonucModel.CariAyBorc = item.Borc2;
                                    sonucModel.BakiyeAy = item.Borc2 - item.Alacak2;
                                    model.CariAyAlacakToplam += item.Alacak2;
                                    model.CariAyBorcToplam += item.Borc2;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 3:
                                    sonucModel.CariAyAlacak = item.Alacak3;
                                    sonucModel.CariAyBorc = item.Borc3;
                                    sonucModel.BakiyeAy = item.Borc3 - item.Alacak3;
                                    model.CariAyAlacakToplam += item.Alacak3;
                                    model.CariAyBorcToplam += item.Borc3;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 4:
                                    sonucModel.CariAyAlacak = item.Alacak4;
                                    sonucModel.CariAyBorc = item.Borc4;
                                    sonucModel.BakiyeAy = item.Borc4 - item.Alacak4;
                                    model.CariAyAlacakToplam += item.Alacak4;
                                    model.CariAyBorcToplam += item.Borc4;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 5:
                                    sonucModel.CariAyAlacak = item.Alacak5;
                                    sonucModel.CariAyBorc = item.Borc5;
                                    sonucModel.BakiyeAy = item.Borc5 - item.Alacak5;
                                    model.CariAyAlacakToplam += item.Alacak5;
                                    model.CariAyBorcToplam += item.Borc5;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 6:
                                    sonucModel.CariAyAlacak = item.Alacak6;
                                    sonucModel.CariAyBorc = item.Borc6;
                                    sonucModel.BakiyeAy = item.Borc6 - item.Alacak6;
                                    model.CariAyAlacakToplam += item.Alacak6;
                                    model.CariAyBorcToplam += item.Borc6;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 7:
                                    sonucModel.CariAyAlacak = item.Alacak7;
                                    sonucModel.CariAyBorc = item.Borc7;
                                    sonucModel.BakiyeAy = item.Borc7 - item.Alacak7;
                                    model.CariAyAlacakToplam += item.Alacak7;
                                    model.CariAyBorcToplam += item.Borc7;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 8:
                                    sonucModel.CariAyAlacak = item.Alacak8;
                                    sonucModel.CariAyBorc = item.Borc8;
                                    sonucModel.BakiyeAy = item.Borc8 - item.Alacak8;
                                    model.CariAyAlacakToplam += item.Alacak8;
                                    model.CariAyBorcToplam += item.Borc8;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 9:
                                    sonucModel.CariAyAlacak = item.Alacak9;
                                    sonucModel.CariAyBorc = item.Borc9;
                                    sonucModel.BakiyeAy = item.Borc9 - item.Alacak9;
                                    model.CariAyAlacakToplam += item.Alacak9;
                                    model.CariAyBorcToplam += item.Borc9;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 10:
                                    sonucModel.CariAyAlacak = item.Alacak10;
                                    sonucModel.CariAyBorc = item.Borc10;
                                    sonucModel.BakiyeAy = item.Borc10 - item.Alacak10;
                                    model.CariAyAlacakToplam += item.Alacak10;
                                    model.CariAyBorcToplam += item.Borc10;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 11:
                                    sonucModel.CariAyAlacak = item.Alacak11;
                                    sonucModel.CariAyBorc = item.Borc11;
                                    sonucModel.BakiyeAy = item.Borc11 - item.Alacak11;
                                    model.CariAyAlacakToplam += item.Alacak11;
                                    model.CariAyBorcToplam += item.Borc11;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                case 12:
                                    sonucModel.CariAyAlacak = item.Alacak12;
                                    sonucModel.CariAyBorc = item.Borc12;
                                    sonucModel.BakiyeAy = item.Borc12 - item.Alacak12;
                                    model.CariAyAlacakToplam += item.Alacak12;
                                    model.CariAyBorcToplam += item.Borc12;
                                    model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                    sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11 + item.Alacak12;
                                    sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11 + item.Borc12;
                                    sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                    break;
                                default:
                                    break;
                            }
                            model.KumulatifAlacakToplam += sonucModel.KumulatifAlacak;
                            model.KumulatifBorcToplam += sonucModel.KumulatifBorc;
                            model.KumulatifBakiyeToplam += sonucModel.KumulatifBakiye;
                            model.list.Add(sonucModel);
                        }

                    }
                }
                #endregion
            }



            return View(model);
        }


        // [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapMizani, SekmeKodu = 0)]
        public ActionResult GelirGiderDönemi()
        {
            GelirGiderARADönemi model = new GelirGiderARADönemi();
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 1; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donem = TurkeyDateTime.Now.Year;
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Ay = TurkeyDateTime.Now.Month;
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();


            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Income }, // Gelir
                new SelectListItem() { Value = "1", Text = babonline.Expense }, // Gider
                new SelectListItem() { Value = "2", Text = babonline.Both }, // her ikisi

            });
            model.AramaTip = 0;
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            return View(model);
        }

        [HttpPost]
        //[Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapMizani, SekmeKodu = 0)]
        public ActionResult GelirGiderDönemi(GelirGiderARADönemi model)
        {
            model.BakiyeAyToplam = 0;
            model.CariAyAlacakToplam = 0;
            model.CariAyBorcToplam = 0;
            model.KumulatifAlacakToplam = 0;
            model.KumulatifBakiyeToplam = 0;
            model.KumulatifBorcToplam = 0;


            model.BakiyeAyToplamHer = 0;
            model.CariAyAlacakToplamHer = 0;
            model.CariAyBorcToplamHer = 0;
            model.KumulatifAlacakToplamHer = 0;
            model.KumulatifBakiyeToplamHer = 0;
            model.KumulatifBorcToplamHer = 0;

            GelirGiderDönemi sonucModel;

            model.list = new List<GelirGiderDönemi>();

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 1; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            List<SelectListItem> listAylar = new List<SelectListItem>();
            for (int ay = 1; ay < 13; ay++)
            {
                listAylar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = ay.ToString(), Text = ay.ToString() },
                });
            }
            model.Aylar = new SelectList(listAylar, "Value", "Text", model.Ay).ToList();

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = babonline.Income }, // Gelir
                new SelectListItem() { Value = "1", Text = babonline.Expense }, // Gider
                new SelectListItem() { Value=  "2", Text = babonline.Both }, // her ikisi
            });
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                List<string> cariHesapTipList = new List<string>();


                if (model.AramaTip == 0) // Gelir - 101-600-602
                {
                    cariHesapTipList.Add("101");
                    cariHesapTipList.Add("600");
                    cariHesapTipList.Add("602");
                }
                else if (model.AramaTip == 1) // Gider - 103-330-610-611-612-740
                {
                    cariHesapTipList.Add("103");
                    cariHesapTipList.Add("330");
                    cariHesapTipList.Add("610");
                    cariHesapTipList.Add("611");
                    cariHesapTipList.Add("612");
                    cariHesapTipList.Add("740");

                }
                else if (model.AramaTip == 2) // Her ikisi - 101-600-602 -103-330-610-611-612-740
                {
                    cariHesapTipList.Add("101");
                    cariHesapTipList.Add("600");
                    cariHesapTipList.Add("602");
                    cariHesapTipList.Add("103");
                    cariHesapTipList.Add("330");
                    cariHesapTipList.Add("610");
                    cariHesapTipList.Add("611");
                    cariHesapTipList.Add("612");
                    cariHesapTipList.Add("740");
                }

                if (model.AramaTip == 0 || model.AramaTip == 1)
                {
                    foreach (string cariHesapTip in cariHesapTipList)
                    {
                        if (true)
                        {
                            var veri = _Muhasebe_CariHesapService.CariHesapBorcAlacakGetirByCariHesapKodu(model.Donem, cariHesapTip);
                            foreach (var item in veri)
                            {
                                var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(item.CariHesapKodu, item.TVMKodu);
                                sonucModel = new GelirGiderDönemi();
                                sonucModel.CariHesapKodu = item.CariHesapKodu;
                                sonucModel.Unvan = _Muhasebe_CariHesapService.GetCariHesap(item.CariHesapKodu).Unvan;
                                sonucModel.id = item.Id;
                                sonucModel.KimlikNo = item.KimlikNo;
                                sonucModel.TVMKodu = item.TVMKodu;

                                switch (model.Ay)
                                {
                                    case 1:
                                        sonucModel.CariAyAlacak = item.Alacak1;
                                        sonucModel.CariAyBorc = item.Borc1;
                                        sonucModel.BakiyeAy = item.Borc1 - item.Alacak1;
                                        if (item.Alacak1 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak1;

                                        }
                                        if (item.Borc1 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc1;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1;
                                        sonucModel.KumulatifBorc = item.Borc1;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 2:
                                        sonucModel.CariAyAlacak = item.Alacak2;
                                        sonucModel.CariAyBorc = item.Borc2;
                                        sonucModel.BakiyeAy = item.Borc2 - item.Alacak2;
                                        if (item.Alacak2 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak2;

                                        }
                                        if (item.Borc2 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc2;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 3:
                                        sonucModel.CariAyAlacak = item.Alacak3;
                                        sonucModel.CariAyBorc = item.Borc3;
                                        sonucModel.BakiyeAy = item.Borc3 - item.Alacak3;
                                        if (item.Alacak3 != null)
                                        {

                                            model.CariAyAlacakToplam += item.Alacak3;
                                        }
                                        if (item.Borc3 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc3;
                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 4:
                                        sonucModel.CariAyAlacak = item.Alacak4;
                                        sonucModel.CariAyBorc = item.Borc4;
                                        sonucModel.BakiyeAy = item.Borc4 - item.Alacak4;
                                        if (item.Alacak4 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak4;

                                        }
                                        if (item.Borc4 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc4;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 5:
                                        sonucModel.CariAyAlacak = item.Alacak5;
                                        sonucModel.CariAyBorc = item.Borc5;
                                        sonucModel.BakiyeAy = item.Borc5 - item.Alacak5;
                                        if (item.Alacak5 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak5;

                                        }
                                        if (item.Borc5 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc5;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 6:
                                        sonucModel.CariAyAlacak = item.Alacak6;
                                        sonucModel.CariAyBorc = item.Borc6;
                                        sonucModel.BakiyeAy = item.Borc6 - item.Alacak6;
                                        if (item.Alacak6 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak6;

                                        }
                                        if (item.Borc6 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc6;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 7:
                                        sonucModel.CariAyAlacak = item.Alacak7;
                                        sonucModel.CariAyBorc = item.Borc7;
                                        sonucModel.BakiyeAy = item.Borc7 - item.Alacak7;
                                        if (item.Alacak7 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak7;

                                        }
                                        if (item.Borc7 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc7;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 8:
                                        sonucModel.CariAyAlacak = item.Alacak8;
                                        sonucModel.CariAyBorc = item.Borc8;
                                        sonucModel.BakiyeAy = item.Borc8 - item.Alacak8;
                                        if (item.Alacak8 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak8;

                                        }
                                        if (item.Borc8 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc8;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;
                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 9:
                                        sonucModel.CariAyAlacak = item.Alacak9;
                                        sonucModel.CariAyBorc = item.Borc9;
                                        sonucModel.BakiyeAy = item.Borc9 - item.Alacak9;
                                        if (item.Alacak9 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak9;

                                        }
                                        if (item.Borc9 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc9;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 10:
                                        sonucModel.CariAyAlacak = item.Alacak10;
                                        sonucModel.CariAyBorc = item.Borc10;
                                        sonucModel.BakiyeAy = item.Borc10 - item.Alacak10;
                                        if (item.Alacak10 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak10;

                                        }
                                        if (item.Borc10 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc10;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 11:
                                        sonucModel.CariAyAlacak = item.Alacak11;
                                        sonucModel.CariAyBorc = item.Borc11;
                                        sonucModel.BakiyeAy = item.Borc11 - item.Alacak11;
                                        if (item.Alacak11 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak11;

                                        }
                                        if (item.Borc11 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc11;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    case 12:
                                        sonucModel.CariAyAlacak = item.Alacak12;
                                        sonucModel.CariAyBorc = item.Borc12;
                                        sonucModel.BakiyeAy = item.Borc12 - item.Alacak12;
                                        if (item.Alacak12 != null)
                                        {
                                            model.CariAyAlacakToplam += item.Alacak12;

                                        }
                                        if (item.Borc12 != null)
                                        {
                                            model.CariAyBorcToplam += item.Borc12;

                                        }
                                        if (sonucModel.BakiyeAy != null)
                                        {
                                            model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                        }

                                        sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11 + item.Alacak12;
                                        sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11 + item.Borc12;
                                        sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                        break;
                                    default:
                                        break;
                                }
                                if (sonucModel.KumulatifAlacak != null)
                                {
                                    model.KumulatifAlacakToplam += sonucModel.KumulatifAlacak;
                                }
                                if (sonucModel.KumulatifBorc != null)
                                {
                                    model.KumulatifBorcToplam += sonucModel.KumulatifBorc;
                                }
                                if (sonucModel.KumulatifBakiye != null)
                                {
                                    model.KumulatifBakiyeToplam += sonucModel.KumulatifBakiye;
                                }

                                model.list.Add(sonucModel);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string cariHesapTip in cariHesapTipList)
                    {
                        if (true)
                        {
                            var veri = _Muhasebe_CariHesapService.CariHesapBorcAlacakGetirByCariHesapKodu(model.Donem, cariHesapTip);
                            foreach (var item in veri)
                            {
                                var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(item.CariHesapKodu, item.TVMKodu);
                                sonucModel = new GelirGiderDönemi();
                                sonucModel.CariHesapKodu = item.CariHesapKodu;
                                sonucModel.Unvan = _Muhasebe_CariHesapService.GetCariHesap(item.CariHesapKodu).Unvan;
                                sonucModel.id = item.Id;
                                sonucModel.KimlikNo = item.KimlikNo;
                                sonucModel.TVMKodu = item.TVMKodu;
                                if (cariHesapTip == "101" || cariHesapTip == "600" || cariHesapTip == "602")
                                {
                                    switch (model.Ay)
                                    {
                                        case 1:
                                            sonucModel.CariAyAlacak = item.Alacak1;
                                            sonucModel.CariAyBorc = item.Borc1;
                                            sonucModel.BakiyeAy = item.Borc1 - item.Alacak1;
                                            if (item.Alacak1 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak1;

                                            }
                                            if (item.Borc1 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc1;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1;
                                            sonucModel.KumulatifBorc = item.Borc1;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 2:
                                            sonucModel.CariAyAlacak = item.Alacak2;
                                            sonucModel.CariAyBorc = item.Borc2;
                                            sonucModel.BakiyeAy = item.Borc2 - item.Alacak2;
                                            if (item.Alacak2 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak2;

                                            }
                                            if (item.Borc2 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc2;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 3:
                                            sonucModel.CariAyAlacak = item.Alacak3;
                                            sonucModel.CariAyBorc = item.Borc3;
                                            sonucModel.BakiyeAy = item.Borc3 - item.Alacak3;
                                            if (item.Alacak3 != null)
                                            {

                                                model.CariAyAlacakToplam += item.Alacak3;
                                            }
                                            if (item.Borc3 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc3;
                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 4:
                                            sonucModel.CariAyAlacak = item.Alacak4;
                                            sonucModel.CariAyBorc = item.Borc4;
                                            sonucModel.BakiyeAy = item.Borc4 - item.Alacak4;
                                            if (item.Alacak4 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak4;

                                            }
                                            if (item.Borc4 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc4;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 5:
                                            sonucModel.CariAyAlacak = item.Alacak5;
                                            sonucModel.CariAyBorc = item.Borc5;
                                            sonucModel.BakiyeAy = item.Borc5 - item.Alacak5;
                                            if (item.Alacak5 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak5;

                                            }
                                            if (item.Borc5 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc5;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 6:
                                            sonucModel.CariAyAlacak = item.Alacak6;
                                            sonucModel.CariAyBorc = item.Borc6;
                                            sonucModel.BakiyeAy = item.Borc6 - item.Alacak6;
                                            if (item.Alacak6 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak6;

                                            }
                                            if (item.Borc6 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc6;

                                            }

                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 7:
                                            sonucModel.CariAyAlacak = item.Alacak7;
                                            sonucModel.CariAyBorc = item.Borc7;
                                            sonucModel.BakiyeAy = item.Borc7 - item.Alacak7;
                                            if (item.Alacak7 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak7;

                                            }
                                            if (item.Borc7 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc7;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 8:
                                            sonucModel.CariAyAlacak = item.Alacak8;
                                            sonucModel.CariAyBorc = item.Borc8;
                                            sonucModel.BakiyeAy = item.Borc8 - item.Alacak8;
                                            if (item.Alacak8 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak8;

                                            }
                                            if (item.Borc8 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc8;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;
                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 9:
                                            sonucModel.CariAyAlacak = item.Alacak9;
                                            sonucModel.CariAyBorc = item.Borc9;
                                            sonucModel.BakiyeAy = item.Borc9 - item.Alacak9;
                                            if (item.Alacak9 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak9;

                                            }
                                            if (item.Borc9 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc9;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 10:
                                            sonucModel.CariAyAlacak = item.Alacak10;
                                            sonucModel.CariAyBorc = item.Borc10;
                                            sonucModel.BakiyeAy = item.Borc10 - item.Alacak10;
                                            if (item.Alacak10 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak10;

                                            }
                                            if (item.Borc10 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc10;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 11:
                                            sonucModel.CariAyAlacak = item.Alacak11;
                                            sonucModel.CariAyBorc = item.Borc11;
                                            sonucModel.BakiyeAy = item.Borc11 - item.Alacak11;
                                            if (item.Alacak11 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak11;

                                            }
                                            if (item.Borc11 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc11;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        case 12:
                                            sonucModel.CariAyAlacak = item.Alacak12;
                                            sonucModel.CariAyBorc = item.Borc12;
                                            sonucModel.BakiyeAy = item.Borc12 - item.Alacak12;
                                            if (item.Alacak12 != null)
                                            {
                                                model.CariAyAlacakToplam += item.Alacak12;

                                            }
                                            if (item.Borc12 != null)
                                            {
                                                model.CariAyBorcToplam += item.Borc12;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplam += sonucModel.BakiyeAy;

                                            }

                                            sonucModel.KumulatifAlacak = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11 + item.Alacak12;
                                            sonucModel.KumulatifBorc = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11 + item.Borc12;
                                            sonucModel.KumulatifBakiye = sonucModel.KumulatifBorc - sonucModel.KumulatifAlacak;
                                            break;
                                        default:
                                            break;
                                    }
                                    if (sonucModel.KumulatifAlacak != null)
                                    {
                                        model.KumulatifAlacakToplam += sonucModel.KumulatifAlacak;
                                    }
                                    if (sonucModel.KumulatifBorc != null)
                                    {
                                        model.KumulatifBorcToplam += sonucModel.KumulatifBorc;
                                    }
                                    if (sonucModel.KumulatifBakiye != null)
                                    {
                                        model.KumulatifBakiyeToplam += sonucModel.KumulatifBakiye;
                                    }
                                }
                                else
                                {
                                    switch (model.Ay)
                                    {
                                        case 1:
                                            sonucModel.CariAyAlacakHer = item.Alacak1;
                                            sonucModel.CariAyBorcHer = item.Borc1;
                                            sonucModel.BakiyeAyHer = item.Borc1 - item.Alacak1;
                                            if (item.Alacak1 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak1;

                                            }
                                            if (item.Borc1 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc1;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1;
                                            sonucModel.KumulatifBorcHer = item.Borc1;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 2:
                                            sonucModel.CariAyAlacakHer = item.Alacak2;
                                            sonucModel.CariAyBorcHer = item.Borc2;
                                            sonucModel.BakiyeAyHer = item.Borc2 - item.Alacak2;
                                            if (item.Alacak2 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak2;

                                            }
                                            if (item.Borc2 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc2;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 3:
                                            sonucModel.CariAyAlacakHer = item.Alacak3;
                                            sonucModel.CariAyBorcHer = item.Borc3;
                                            sonucModel.BakiyeAyHer = item.Borc3 - item.Alacak3;
                                            if (item.Alacak3 != null)
                                            {

                                                model.CariAyAlacakToplamHer += item.Alacak3;
                                            }
                                            if (item.Borc3 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc3;
                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 4:
                                            sonucModel.CariAyAlacakHer = item.Alacak4;
                                            sonucModel.CariAyBorcHer = item.Borc4;
                                            sonucModel.BakiyeAyHer = item.Borc4 - item.Alacak4;
                                            if (item.Alacak4 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak4;

                                            }
                                            if (item.Borc4 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc4;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 5:
                                            sonucModel.CariAyAlacakHer = item.Alacak5;
                                            sonucModel.CariAyBorcHer = item.Borc5;
                                            sonucModel.BakiyeAyHer = item.Borc5 - item.Alacak5;
                                            if (item.Alacak5 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak5;

                                            }
                                            if (item.Borc5 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc5;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 6:
                                            sonucModel.CariAyAlacakHer = item.Alacak6;
                                            sonucModel.CariAyBorcHer = item.Borc6;
                                            sonucModel.BakiyeAyHer = item.Borc6 - item.Alacak6;
                                            if (item.Alacak6 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak6;

                                            }
                                            if (item.Borc6 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc6;

                                            }

                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 7:
                                            sonucModel.CariAyAlacakHer = item.Alacak7;
                                            sonucModel.CariAyBorcHer = item.Borc7;
                                            sonucModel.BakiyeAyHer = item.Borc7 - item.Alacak7;
                                            if (item.Alacak7 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak7;

                                            }
                                            if (item.Borc7 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc7;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 8:
                                            sonucModel.CariAyAlacakHer = item.Alacak8;
                                            sonucModel.CariAyBorcHer = item.Borc8;
                                            sonucModel.BakiyeAyHer = item.Borc8 - item.Alacak8;
                                            if (item.Alacak8 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak8;

                                            }
                                            if (item.Borc8 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc8;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;
                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 9:
                                            sonucModel.CariAyAlacakHer = item.Alacak9;
                                            sonucModel.CariAyBorcHer = item.Borc9;
                                            sonucModel.BakiyeAyHer = item.Borc9 - item.Alacak9;
                                            if (item.Alacak9 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak9;

                                            }
                                            if (item.Borc9 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc9;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 10:
                                            sonucModel.CariAyAlacakHer = item.Alacak10;
                                            sonucModel.CariAyBorcHer = item.Borc10;
                                            sonucModel.BakiyeAyHer = item.Borc10 - item.Alacak10;
                                            if (item.Alacak10 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak10;

                                            }
                                            if (item.Borc10 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc10;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 11:
                                            sonucModel.CariAyAlacakHer = item.Alacak11;
                                            sonucModel.CariAyBorcHer = item.Borc11;
                                            sonucModel.BakiyeAyHer = item.Borc11 - item.Alacak11;
                                            if (item.Alacak11 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak11;

                                            }
                                            if (item.Borc11 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc11;

                                            }
                                            if (sonucModel.BakiyeAy != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;
                                        case 12:
                                            sonucModel.CariAyAlacakHer = item.Alacak12;
                                            sonucModel.CariAyBorcHer = item.Borc12;
                                            sonucModel.BakiyeAyHer = item.Borc12 - item.Alacak12;
                                            if (item.Alacak12 != null)
                                            {
                                                model.CariAyAlacakToplamHer += item.Alacak12;

                                            }
                                            if (item.Borc12 != null)
                                            {
                                                model.CariAyBorcToplamHer += item.Borc12;

                                            }
                                            if (sonucModel.BakiyeAyHer != null)
                                            {
                                                model.BakiyeAyToplamHer += sonucModel.BakiyeAyHer;

                                            }

                                            sonucModel.KumulatifAlacakHer = item.Alacak1 + item.Alacak2 + item.Alacak3 + item.Alacak4 + item.Alacak5 + item.Alacak6 + item.Alacak7 + item.Alacak8 + item.Alacak9 + item.Alacak10 + item.Alacak11 + item.Alacak12;
                                            sonucModel.KumulatifBorcHer = item.Borc1 + item.Borc2 + item.Borc3 + item.Borc4 + item.Borc5 + item.Borc6 + item.Borc7 + item.Borc8 + item.Borc9 + item.Borc10 + item.Borc11 + item.Borc12;
                                            sonucModel.KumulatifBakiyeHer = sonucModel.KumulatifBorcHer - sonucModel.KumulatifAlacakHer;
                                            break;

                                        default:
                                            break;
                                    }
                                    if (sonucModel.KumulatifAlacakHer != null)
                                    {
                                        model.KumulatifAlacakToplamHer += sonucModel.KumulatifAlacakHer;
                                    }
                                    if (sonucModel.KumulatifBorcHer != null)
                                    {
                                        model.KumulatifBorcToplamHer += sonucModel.KumulatifBorcHer;
                                    }
                                    if (sonucModel.KumulatifBakiyeHer != null)
                                    {
                                        model.KumulatifBakiyeToplamHer += sonucModel.KumulatifBakiyeHer;
                                    }
                                }


                                model.list.Add(sonucModel);
                            }
                        }
                    }
                }






            }

            return View(model);
        }


        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.TopluPoliceOdeme, SekmeKodu = 0)]
        public ActionResult TopluPoliceTahsilatOdeme()
        {
            TopluPoliceTahsilatOdemeModel model = new TopluPoliceTahsilatOdemeModel();
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TvmUnvani = _AktifKullaniciService.TVMUnvani;

            List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
            model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            //durum 0 şahıs 1 firma
            model.Durumlar = new SelectList(DurumListesiAktifPasif.FirmamiSahismi(), "Value", "Text", "0");
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 2; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donem = TurkeyDateTime.Now.Year;
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();

            return View(model);
        }

        //[Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.CariHesapMizani, SekmeKodu = 0)]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.TopluPoliceOdeme, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult TopluPoliceTahsilatOdeme(TopluPoliceTahsilatOdemeModel model)
        {
            DateTime BitisTarihi = new DateTime();
            DateTime BaslangicTarihi = new DateTime();

            //Ekrandan seçilen model aralığının büyük olanına göre gün ay filtresi çalışacak
            if (!String.IsNullOrEmpty(model.BitisGunAy) && !String.IsNullOrEmpty(model.BaslangicGunAy))
            {
                string bitisT = model.BitisGunAy + "." + model.Donem;
                string baslangicT = model.BaslangicGunAy + "." + model.Donem;

                BitisTarihi = Convert.ToDateTime(bitisT);
                BaslangicTarihi = Convert.ToDateTime(baslangicT);
            }
            if (!String.IsNullOrEmpty(model.BitisGunAy1) && !String.IsNullOrEmpty(model.BaslangicGunAy1))
            {
                string bitisT1 = model.BitisGunAy1 + "." + model.Donem;
                string baslangicT1 = model.BaslangicGunAy1 + "." + model.Donem;

                BitisTarihi = Convert.ToDateTime(bitisT1);
                BaslangicTarihi = Convert.ToDateTime(baslangicT1);
            }

            model.listPolOffline = new List<TopluPoliceTahsilatOdemeAraModel>();
            model.Durumlar = new SelectList(DurumListesiAktifPasif.FirmamiSahismi(), "Value", "Text", "0");

            List<TVMDetay> tvmler = _TVMService.GetListTVMDetayYetkili();
            model.TVMList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year + 2; yil >= 1990; yil--)
            {
                listYillar.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = yil.ToString(), Text = yil.ToString() },
                });

            }
            model.Donemler = new SelectList(listYillar, "Value", "Text", model.Donem).ToList();


            #region Tablo Getir
            if (model.TcknVkn != null && model.TcknVkn != "")
            {
                model.TcknVkn = model.TcknVkn.Trim();
                model.police = _PoliceService.PoliceOffLineTcVknSigortaEttiren(model.TcknVkn, _AktifKullaniciService.TVMKodu, model.Donem);
            }
            else if (model.SigortaEttiren != null && model.SigortaEttiren != "")
            {
                model.SigortaEttiren = model.SigortaEttiren.Trim();
                model.police = _PoliceService.PoliceOffLineTcVknSigortaEttiren(model.SigortaEttiren, _AktifKullaniciService.TVMKodu, model.Donem);
            }
            else if (model.UnvanFirma != null && model.UnvanFirma != "" && model.Durum == 1)
            {
                model.UnvanFirma = model.UnvanFirma.Trim();
                model.police = _PoliceService.PoliceOffLineFirmaUnvanSigortaEttiren(model.UnvanFirma, _AktifKullaniciService.TVMKodu, model.Durum, model.Donem);
            }
            else if (model.Unvan != null && model.UnvanSoyad != null && model.Unvan != "" && model.UnvanSoyad != "" && model.Durum == 0)
            {
                model.Unvan = model.Unvan.Trim();
                model.UnvanSoyad = model.UnvanSoyad.Trim();
                model.police = _PoliceService.PoliceOffLineUnvanSigortaEttiren(model.Unvan, model.UnvanSoyad, _AktifKullaniciService.TVMKodu, model.Durum, model.Donem);
            }
            else if (model.TVMKodu != null && model.TVMKodu != 0)
            {
                // var qwe = Request.Form["group1"];
                model.police = _PoliceService.PoliceOffLineTvmKod(model.TVMKodu, BaslangicTarihi, BitisTarihi);
            }
            #endregion
            try
            {
                #region musteri grup kodu
                if (model.MusteriGrupKodu != null && model.MusteriGrupKodu != "")
                {
                    model.MusteriGrupKodu = model.MusteriGrupKodu.Trim();
                    model.police = _PoliceService.PoliceOffLineMustNo(model.MusteriGrupKodu, _AktifKullaniciService.TVMKodu, BaslangicTarihi, BitisTarihi);

                }
                #endregion

            }

            catch (Exception)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });

            }



            return View(model);
        }
        [AjaxException]
        public ActionResult GetCariHesapByTCKN(int Polid)
        {
            var polGenel = _PoliceService.GetPoliceById(Polid);
            var TCVKN = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

            try
            {

                var carihesapAdiveKodu = _Muhasebe_CariHesapService.GetCariHesapAdiByTCKN(TCVKN);

                if (carihesapAdiveKodu.Item1 != "" && carihesapAdiveKodu.Item2 != "")
                {
                    var result = new { success = true, cariAdi = carihesapAdiveKodu.Item1, cariKodu = carihesapAdiveKodu.Item2 };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var result = new { success = false, cariAdi = polGenel.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan + " " + polGenel.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan, cariKodu = "120.01." + TCVKN };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                var result = new { success = false, cariAdi = polGenel.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan + " " + polGenel.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan, cariKodu = "120.01." + TCVKN };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult TopluPoliceTahsilatOdemePartial()
        {
            TopluPoliceTahsilatOdemeModel model = new TopluPoliceTahsilatOdemeModel();
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TvmUnvani = _AktifKullaniciService.TVMUnvani;
            //durum 0 şahıs 1 firma
            model.Durumlar = new SelectList(DurumListesiAktifPasif.FirmamiSahismi(), "Value", "Text", "0");
            return PartialView("_TopluPoliceTahsilatOdeme", model);
        }
        [AjaxException]
        public ActionResult TopluPoliceTahsilatOdemeYapma(int[] Polid, int[] taksitNo, string[] OdenenTutarString, string BelgeNo, string BelgeTarihi, string OdemeTuru, string AcenteKrediKarti, string odeyenCariHesapKodu, string dekontEvrakNo)
        {
            string mesaj = string.Empty;
            for (int i = 0; i < Polid.Count(); i++)
            {
                decimal OdenenTutar = Convert.ToDecimal(OdenenTutarString[i]);
                //Neosinerji.BABOnlineTP.Business.Police models
                // PoliceTahsilat model = new PoliceTahsilat();
                PoliceGenel polGenelmodel = new PoliceGenel();
                CariHesapBorcAlacak cariBorcAlacakModel = new CariHesapBorcAlacak();
                CariHareketleri cariHaraketler = new CariHareketleri();
                int _TVMKodu = _AktifKullaniciService.TVMKodu;
                var polGenel = _PoliceService.GetPoliceById(Polid[i]);
                var polOdeme = _PoliceService.GetPoliceOdemePlani(Polid[i], taksitNo[i]);
                PoliceTahsilat polTahsilat = polGenel.GenelBilgiler.PoliceTahsilats.Where(s => s.PoliceId == Polid[i] && s.TaksitNo == taksitNo[i]).FirstOrDefault();
                List<PoliceTahsilat> polTahsilatList = new List<PoliceTahsilat>();
                polTahsilatList = polGenel.GenelBilgiler.PoliceTahsilats.Where(s => s.PoliceId == Polid[i] && s.TaksitNo == taksitNo[i]).ToList<PoliceTahsilat>();
                var odeyenCariHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAdi(odeyenCariHesapKodu);


                #region kayıt guncelle

                if (polOdeme != null && OdenenTutar <= polOdeme.TaksitTutari)
                {

                    if (odeyenCariHesabiVarmi.Item2 != null)
                    {
                        //Nakitse
                        if (OdemeTuru == "1")
                        {
                            var kasaHesabi = "100.01.";
                            int odemeturu = Convert.ToInt32(OdemeTuru);
                            var kasaHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(odeyenCariHesabiVarmi.Item2, _AktifKullaniciService.TVMKodu);
                            if (kasaHesaplari == null || carihesap == null)
                            {
                                mesaj = "false";
                            }
                            var kasaHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(kasaHesaplari.CariHesapNo);

                            if (carihesap != null && carihesap.Count > 0 && kasaHesabiVarmi != null && kasaHesabiVarmi.Count > 0)
                            {
                                //cariBorcAlacakModel.CariHesapKodu = odeyenCariHesapKodu;
                                //cariBorcAlacakModel.TVMKodu = _AktifKullaniciService.TVMKodu;
                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;
                                string tcVkn = String.Empty;
                                if (!String.IsNullOrEmpty(odeyenCariHesapKodu))
                                {
                                    var parts = odeyenCariHesapKodu.Split('.');
                                    if (parts != null && parts.Count() == 3)
                                    {
                                        tcVkn = parts[2];
                                    }
                                }
                                cariBorcAlacakModel.KimlikNo = tcVkn;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (kasaHesabiVarmi != null)
                                {
                                    foreach (var items in kasaHesabiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    if (!String.IsNullOrEmpty(BelgeTarihi))
                                    {
                                        var parts = BelgeTarihi.Split('.');
                                        if (parts != null && parts.Count() == 3)
                                        {
                                            donemKasaYil = parts[2];
                                            donemKasaAy = parts[1];
                                        }
                                    }

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.CariHesapKodu = kasaHesaplari.CariHesapNo;
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }
                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {
                                mesaj = "false";
                            }

                        }
                        //Müşteri Kredi Kartı
                        if (OdemeTuru == "2")
                        {
                            var sigortaSirketKodu = polGenel.GenelBilgiler.TUMBirlikKodu;
                            var sirketVergiNo = _SigortaSirketleriService.GetSigortaBilgileri(sigortaSirketKodu);
                            var sigortaSirketiCariHesabiKodu = "320.01." + "" + sirketVergiNo.VergiNumarasi;
                            var sigortaSirketiCariHesasbiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(sigortaSirketiCariHesabiKodu);

                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(odeyenCariHesabiVarmi.Item2, _AktifKullaniciService.TVMKodu);
                            //var musteriKkartiCariKodu = odeyenCariHesapKodu; // hangi şirket olduğu bılınmıyor

                            if (carihesap != null && carihesap.Count > 0 && sigortaSirketiCariHesasbiVarmi.Count > 0 && sigortaSirketiCariHesasbiVarmi != null)
                            {

                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;
                                string tcVkn = String.Empty;
                                if (!String.IsNullOrEmpty(odeyenCariHesapKodu))
                                {
                                    var parts = odeyenCariHesapKodu.Split('.');
                                    if (parts != null && parts.Count() == 3)
                                    {
                                        tcVkn = parts[2];
                                    }
                                }
                                cariBorcAlacakModel.KimlikNo = tcVkn;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (sigortaSirketiCariHesasbiVarmi != null && sigortaSirketiCariHesasbiVarmi.Count > 0)
                                {
                                    foreach (var items in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.GuncellemeTarihi = items.GuncellemeTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    donemKasaAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                    donemKasaYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = BelgeNo + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;

                                    foreach (var itemSigortasirketCari in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemSigortasirketCari.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {
                                mesaj = "false";
                            }
                        }
                        //Havale ise
                        if (OdemeTuru == "3")
                        {
                            //var bankaHesabi = "102.01.";
                            int odemeturu = Convert.ToInt32(OdemeTuru);
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(odeyenCariHesabiVarmi.Item2, _AktifKullaniciService.TVMKodu);
                            // havale ise acentenin banka hesabını çağırıyoruz. o yüzden 8 yazdık. 8= acente banka hesabı
                            var bankaHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, 8, AcenteKrediKarti);
                            var bankaHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(bankaHesaplari.CariHesapNo);

                            //var bankaHesabiVarmi = _Muhasebe_CariHesapService.GetCariKasaHesapList(bankaHesabi);
                            if (carihesap != null && carihesap.Count > 0 && bankaHesabiVarmi != null && bankaHesabiVarmi.Count > 0)
                            {

                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;
                                string tcVkn = String.Empty;
                                if (!String.IsNullOrEmpty(odeyenCariHesapKodu))
                                {
                                    var parts = odeyenCariHesapKodu.Split('.');
                                    if (parts != null && parts.Count() == 3)
                                    {
                                        tcVkn = parts[2];
                                    }
                                }
                                cariBorcAlacakModel.KimlikNo = tcVkn;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (bankaHesabiVarmi != null)
                                {
                                    foreach (var itemss in bankaHesabiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = itemss.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = itemss.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = itemss.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = itemss.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = itemss.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    if (!String.IsNullOrEmpty(BelgeTarihi))
                                    {
                                        var parts = BelgeTarihi.Split('.');
                                        if (parts != null && parts.Count() == 3)
                                        {
                                            donemKasaYil = parts[2];
                                            donemKasaAy = parts[1];
                                        }
                                    }

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);

                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemBankaCariHesap in bankaHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemBankaCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {
                                mesaj = "false";
                            }

                        }
                        //Çek
                        if (OdemeTuru == "4")
                        {
                            int odemeturu = Convert.ToInt32(OdemeTuru);

                            //var alinanCekler = "101.01.";
                            var sEttirenKimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            var sEttirenCariHesabi = "120.01." + "" + sEttirenKimlikNo;
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(sEttirenCariHesabi, _AktifKullaniciService.TVMKodu);
                            var alinanCekHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);

                            var alinanCekHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(alinanCekHesaplari.CariHesapNo);
                            if (carihesap != null && carihesap.Count > 0 && alinanCekHesabiVarmi != null && alinanCekHesabiVarmi.Count > 0)
                            {
                                //cariBorcAlacakModel.CariHesapKodu = odeyenCariHesapKodu;
                                //cariBorcAlacakModel.TVMKodu = _AktifKullaniciService.TVMKodu;
                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;

                                cariBorcAlacakModel.KimlikNo = sEttirenKimlikNo;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";
                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (alinanCekHesabiVarmi != null)
                                {
                                    foreach (var items in alinanCekHesabiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    if (!String.IsNullOrEmpty(BelgeTarihi))
                                    {
                                        var parts = BelgeTarihi.Split('.');
                                        if (parts != null && parts.Count() == 3)
                                        {
                                            donemKasaYil = parts[2];
                                            donemKasaAy = parts[1];
                                        }
                                    }

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemCekCariHesap in alinanCekHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemCekCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);

                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {

                                mesaj = "false";
                            }

                        }
                        //Acente Kredi Kartı
                        if (OdemeTuru == "5")
                        {
                            //var sigortaSirketi = polGenel.GenelBilgiler.TUMBirlikKodu;

                            int odemeturu = Convert.ToInt32(OdemeTuru);
                            var acenteKKHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);
                            var acenteKKHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(acenteKKHesaplari.CariHesapNo);

                            //var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(odeyenCariHesabiVarmi.Item2, _AktifKullaniciService.TVMKodu);

                            var sigortaSirketKodu = polGenel.GenelBilgiler.TUMBirlikKodu;
                            var sirketVergiNo = _SigortaSirketleriService.GetSigortaBilgileri(sigortaSirketKodu);
                            var sigortaSirketiCariHesabiKodu = "320.01." + "" + sirketVergiNo.VergiNumarasi;
                            var sigortaSirketiCariHesasbiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(sigortaSirketiCariHesabiKodu);
                            if (acenteKKHesabiVarmi != null && acenteKKHesabiVarmi.Count > 0 && sigortaSirketiCariHesasbiVarmi != null && sigortaSirketiCariHesasbiVarmi.Count > 0)
                            {

                                foreach (var item in acenteKKHesabiVarmi)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;
                                string tcVkn = String.Empty;
                                if (!String.IsNullOrEmpty(odeyenCariHesapKodu))
                                {
                                    var parts = odeyenCariHesapKodu.Split('.');
                                    if (parts != null && parts.Count() == 3)
                                    {
                                        tcVkn = parts[2];
                                    }
                                }
                                cariBorcAlacakModel.KimlikNo = tcVkn;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (sigortaSirketiCariHesasbiVarmi != null)
                                {
                                    foreach (var items in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    donemKasaAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                    donemKasaYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";
                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme
                                    // ss şirketi
                                    cariHaraketler = new CariHareketleri();
                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemSSCariHesap in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemSSCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    //Acente kk
                                    cariHaraketler = new CariHareketleri();
                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemsss in acenteKKHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemsss.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "A";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);
                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {

                                mesaj = "false";
                            }

                        }
                        //Acente Pos Hesabi
                        if (OdemeTuru == "6")
                        {
                            int odemeturu = Convert.ToInt32(OdemeTuru);

                            var sEttirenKimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            var sEttirenCariHesabi = "120.01." + "" + sEttirenKimlikNo;
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(sEttirenCariHesabi, _AktifKullaniciService.TVMKodu);
                            var posHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);

                            var posHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(posHesaplari.CariHesapNo);
                            if (carihesap != null && carihesap.Count > 0 && posHesabiVarmi != null && posHesabiVarmi.Count > 0)
                            {
                                //cariBorcAlacakModel.CariHesapKodu = odeyenCariHesapKodu;
                                //cariBorcAlacakModel.TVMKodu = _AktifKullaniciService.TVMKodu;
                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;

                                cariBorcAlacakModel.KimlikNo = sEttirenKimlikNo;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (posHesabiVarmi != null)
                                {
                                    foreach (var items in posHesabiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    if (!String.IsNullOrEmpty(BelgeTarihi))
                                    {
                                        var parts = BelgeTarihi.Split('.');
                                        if (parts != null && parts.Count() == 3)
                                        {
                                            donemKasaYil = parts[2];
                                            donemKasaAy = parts[1];
                                        }
                                    }

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemPosCariHesap in posHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemPosCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {

                                mesaj = "false";
                            }

                        }
                        //Senet
                        if (OdemeTuru == "7")
                        {
                            int odemeturu = Convert.ToInt32(OdemeTuru);
                            var alinanSenetHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);
                            //var Alacak senetleri = "121.01.";
                            var sEttirenKimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : polGenel.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            var sEttirenCariHesabi = "120.01." + "" + sEttirenKimlikNo;
                            var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(sEttirenCariHesabi, _AktifKullaniciService.TVMKodu);
                            var alacakSenetHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(alinanSenetHesaplari.CariHesapNo);
                            if (carihesap != null && carihesap.Count > 0 && alacakSenetHesabiVarmi != null && alacakSenetHesabiVarmi.Count > 0)
                            {
                                //cariBorcAlacakModel.CariHesapKodu = odeyenCariHesapKodu;
                                //cariBorcAlacakModel.TVMKodu = _AktifKullaniciService.TVMKodu;
                                foreach (var item in carihesap)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;

                                cariBorcAlacakModel.KimlikNo = sEttirenKimlikNo;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";
                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (alacakSenetHesabiVarmi != null)
                                {
                                    foreach (var items in alacakSenetHesabiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    if (!String.IsNullOrEmpty(BelgeTarihi))
                                    {
                                        var parts = BelgeTarihi.Split('.');
                                        if (parts != null && parts.Count() == 3)
                                        {
                                            donemKasaYil = parts[2];
                                            donemKasaAy = parts[1];
                                        }
                                    }

                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme

                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemSenetCariHesap in alacakSenetHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemSenetCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            else
                            {

                                mesaj = "false";
                            }

                        }
                        //Acente Bireysel Kredi Kartı
                        if (OdemeTuru == "9")
                        {
                            //var sigortaSirketi = polGenel.GenelBilgiler.TUMBirlikKodu;

                            int odemeturu = Convert.ToInt32(OdemeTuru);
                            var acenteBKKHesaplari = _TVMService.GetListTVMBankaCariHesaplari(polGenel.GenelBilgiler.TVMKodu.Value, odemeturu, AcenteKrediKarti);
                            var acenteBKKHesabiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(acenteBKKHesaplari.CariHesapNo);

                            //var carihesap = _Muhasebe_CariHesapService.getCariHesapListesiByCariHesapKodu(odeyenCariHesabiVarmi.Item2, _AktifKullaniciService.TVMKodu);

                            var sigortaSirketKodu = polGenel.GenelBilgiler.TUMBirlikKodu;
                            var sirketVergiNo = _SigortaSirketleriService.GetSigortaBilgileri(sigortaSirketKodu);
                            var sigortaSirketiCariHesabiKodu = "320.01." + "" + sirketVergiNo.VergiNumarasi;
                            var sigortaSirketiCariHesasbiVarmi = _Muhasebe_CariHesapService.GetCariHesapAcenteBankaPosKasaHesapList(sigortaSirketiCariHesabiKodu);
                            if (acenteBKKHesabiVarmi != null && acenteBKKHesabiVarmi.Count > 0 && sigortaSirketiCariHesasbiVarmi != null && sigortaSirketiCariHesasbiVarmi.Count > 0)
                            {

                                foreach (var item in acenteBKKHesabiVarmi)
                                {
                                    cariBorcAlacakModel.CariHesapId = item.CariHesapId;
                                    cariBorcAlacakModel.CariHesapKodu = item.CariHesapKodu;
                                    cariBorcAlacakModel.TVMKodu = item.TVMKodu;
                                }
                                cariBorcAlacakModel.KayitTarihi = TurkeyDateTime.Now;
                                string tcVkn = String.Empty;
                                if (!String.IsNullOrEmpty(odeyenCariHesapKodu))
                                {
                                    var parts = odeyenCariHesapKodu.Split('.');
                                    if (parts != null && parts.Count() == 3)
                                    {
                                        tcVkn = parts[2];
                                    }
                                }
                                cariBorcAlacakModel.KimlikNo = tcVkn;
                                string donemYil = String.Empty;
                                string donemAy = String.Empty;
                                donemAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                donemYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";

                                cariBorcAlacakModel.Donem = Convert.ToInt32(donemYil);
                                cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;
                                _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemYil, donemAy, 0, OdenenTutar);

                                if (sigortaSirketiCariHesasbiVarmi != null)
                                {
                                    foreach (var items in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariBorcAlacakModel.CariHesapId = items.CariHesapId;
                                        cariBorcAlacakModel.CariHesapKodu = items.CariHesapKodu;
                                        cariBorcAlacakModel.TVMKodu = items.TVMKodu;
                                        cariBorcAlacakModel.KimlikNo = items.KimlikNo;
                                        cariBorcAlacakModel.KayitTarihi = items.KayitTarihi;
                                    }

                                    string donemKasaYil = String.Empty;
                                    string donemKasaAy = String.Empty;
                                    donemKasaAy = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Month.ToString() : "";
                                    donemKasaYil = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value.Year.ToString() : "";
                                    cariBorcAlacakModel.Donem = Convert.ToInt32(donemKasaYil);
                                    cariBorcAlacakModel.GuncellemeTarihi = TurkeyDateTime.Now;

                                    _Muhasebe_CariHesapService.UpdatePolTahCariHesapBorcAlacak(cariBorcAlacakModel.TVMKodu, cariBorcAlacakModel.CariHesapKodu, donemKasaYil, donemKasaAy, OdenenTutar, 0);
                                    #region cari haraket ekleme
                                    // ss şirketi
                                    cariHaraketler = new CariHareketleri();
                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti + "-" + "Tahsilat";
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemSSCariHesap in sigortaSirketiCariHesasbiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemSSCariHesap.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "B";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);

                                    //Acente kk
                                    cariHaraketler = new CariHareketleri();
                                    cariHaraketler.EvrakNo = polGenel.GenelBilgiler.PoliceNumarasi + "-" + polGenel.GenelBilgiler.YenilemeNo + "-" + polGenel.GenelBilgiler.EkNo;
                                    cariHaraketler.DovizKuru = polGenel.GenelBilgiler.DovizKur;
                                    cariHaraketler.DovizTipi = polGenel.GenelBilgiler.ParaBirimi;
                                    cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    cariHaraketler.OdemeTipi = Convert.ToInt32(OdemeTuru);
                                    cariHaraketler.Tutar = OdenenTutar;
                                    cariHaraketler.Aciklama = AcenteKrediKarti;
                                    cariHaraketler.GuncellemeTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.OdemeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    cariHaraketler.CariHareketTarihi = polOdeme.VadeTarihi;
                                    foreach (var itemsss in acenteBKKHesabiVarmi)
                                    {
                                        cariHaraketler.CariHesapKodu = itemsss.CariHesapKodu;

                                    }
                                    cariHaraketler.BorcAlacakTipi = "A";
                                    cariHaraketler.KayitTarihi = TurkeyDateTime.Now;
                                    cariHaraketler.TVMKodu = polGenel.GenelBilgiler.TVMKodu.Value;
                                    _Muhasebe_CariHesapService.PolTahCariHareketEkle(cariHaraketler);
                                    #endregion
                                }

                                #region pol tah
                                if (polTahsilatList.Count != 0)
                                {
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilat.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilat.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilat.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilat.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilat.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilat.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilat.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilat.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilat.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        //müşteri kredi kartı ve havale ise
                                        polTahsilat.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        // acenta ile ilgili her şey(pos,kk,banka,kasa,çek,senet)
                                        polTahsilat.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilat.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilat.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilat.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilat.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilat.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilat.OdenenTutar;
                                    polTahsilat.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.UpdatePoliceTahsilat(polTahsilat);
                                    mesaj = "true";
                                }
                                else
                                {
                                    PoliceTahsilat polTahsilatt = new PoliceTahsilat();
                                    foreach (var item in polTahsilatList)
                                    {
                                        polTahsilat.TahsilatId = item.TahsilatId;
                                    }
                                    polTahsilatt.PoliceId = polGenel.GenelBilgiler.PoliceId;
                                    polTahsilatt.KimlikNo = !String.IsNullOrEmpty(polGenel.GenelBilgiler.PoliceSigortali.KimlikNo) ? polGenel.GenelBilgiler.PoliceSigortali.KimlikNo : polGenel.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    polTahsilatt.PoliceNo = polGenel.GenelBilgiler.PoliceNumarasi;
                                    polTahsilatt.CariHesapKodu = odeyenCariHesapKodu;
                                    polTahsilatt.ZeyilNo = polGenel.GenelBilgiler.EkNo.ToString();
                                    polTahsilatt.BrutPrim = Convert.ToDecimal(polGenel.GenelBilgiler.BrutPrim);
                                    if (OdemeTuru != null)
                                    {
                                        polTahsilatt.OdemTipi = Convert.ToInt32(OdemeTuru);
                                    }
                                    polTahsilatt.Dekont_EvrakNo = dekontEvrakNo;
                                    polTahsilatt.TaksitNo = polOdeme.TaksitNo;
                                    polTahsilatt.TaksitVadeTarihi = polOdeme.VadeTarihi.HasValue ? polOdeme.VadeTarihi.Value : TurkeyDateTime.Today.Date;
                                    polTahsilatt.TaksitTutari = polOdeme.TaksitTutari.HasValue ? polOdeme.TaksitTutari.Value : 0;
                                    polTahsilatt.OdenenTutar += OdenenTutar;
                                    if (OdemeTuru == "2")
                                    {
                                        polTahsilatt.OdemeBelgeNo = BelgeNo;
                                    }
                                    else
                                    {
                                        polTahsilatt.OdemeBelgeNo = AcenteKrediKarti;
                                    }
                                    polTahsilatt.OdemeBelgeTarihi = Convert.ToDateTime(BelgeTarihi);
                                    polTahsilatt.KayitTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                                    polTahsilatt.KalanTaksitTutari = polOdeme.TaksitTutari.Value - polTahsilatt.OdenenTutar;
                                    polTahsilatt.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                                    _PoliceService.CreatePoliceTahsilat(polTahsilatt);
                                    mesaj = "true";
                                }
                                #endregion
                                #region odemeplaniiş odemetipi güncelle
                                if (polGenel.GenelBilgiler.PoliceOdemePlanis.Count == 1)
                                {
                                    var ödendiMi = _PoliceService.policeOdemePlaniIsOdemeTipiGuncelle(polGenel.GenelBilgiler.PoliceId, OdemeTuru);
                                }
                                #endregion
                            }
                            {

                                mesaj = "false";
                            }

                        }
                    }

                }

                #endregion
            }

            return Json(new { mesaj });
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceleriCariyeAktar, SekmeKodu = 0)]
        public ActionResult PoliceleriCariyeAktar()
        {
            PoliceleriCariyeAktar model = new PoliceleriCariyeAktar();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            var sigortaSirketleri = _SigortaSirketleriService.GetList();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi").ListWithOptionLabel();
            model.Branslar = new SelectList(brans, "BransKodu", "BransAdi").ListWithOptionLabel();
            return View(model);
        }

        [AjaxException]
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceleriCariyeAktar, SekmeKodu = 0)]
        public ActionResult PoliceleriCariyeAktar(DateTime baslangicTarihi, DateTime bitisTarihi, string sirketKodu, Nullable<int> bransKodu)
        {
            MuhasebeAktarimKonfigurasyon model = new MuhasebeAktarimKonfigurasyon();
            //Acente Unvanı Aktif Kullanıcının tvmkodu ve unvanı kaydedilecek.
            model.TvmKodu = _AktifKullaniciService.TVMKodu;
            model.TvmUnvani = _AktifKullaniciService.TVMUnvani;
            //Sira No Db den bakılıp son numaraya 1 eklenerek arttırılacak.
            model.SiraNo = 1;
            //Aktarım Tamamlandı alanı otomatik 0 olarak kaydedilecek.
            model.AktarimTamamlandi = 0;
            model.BaslangicTarihi = baslangicTarihi;
            model.BitisTarihi = bitisTarihi;
            if (sirketKodu == "")
            {
                model.SirketKodu = null;
            }
            else
            {
                model.SirketKodu = sirketKodu;
            }
            model.BransKodu = bransKodu;
            string aktarimMesaji = "";
            int sonuc = _Muhasebe_CariHesapService.PoliceleriCariyeAktarma(model);
            if (sonuc > 0)
            {
                string uri = System.Web.Configuration.WebConfigurationManager.AppSettings["muhasebeaktarim"];
                //string uri = "http://localhost:45571/";
                //string uri = "http://neoonlinemuhasebeaktarim.azurewebsites.net";
                //string uri = "http://neoonlinemuhasebeaktarim-test.azurewebsites.net/";

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpRequest.Timeout = 200000;
                httpRequest.Method = "GET";
                httpRequest.KeepAlive = false;
                httpRequest.ContentType = "application/json";
                HttpWebResponse webresponse;
                webresponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream data = webresponse.GetResponseStream();
                StreamReader reader = new StreamReader(data, Encoding.GetEncoding(1254));
                string ResponseString = reader.ReadToEnd();
                if (ResponseString == "true")
                {
                    aktarimMesaji = model.BaslangicTarihi.ToString("dd/MM/yyyy") + "/" + model.BitisTarihi.ToString("dd/MM/yyyy") + " arasındaki poliçeleriniz muhasebeye aktarılmaya başlamıştır. ";
                }
                reader.Close();
                webresponse.Dispose();
            }
            var result = new { success = true, aktarimMesaji, konfigurasyonId = sonuc };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AjaxException]
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.PoliceleriCariyeAktar, SekmeKodu = 0)]
        public ActionResult GetMuhasebeAktarimStatus(int konfigurasyonId)
        {
            double muhasebeAktarimYuzdesi = _Muhasebe_CariHesapService.GetMuhasebeAktarimYuzdesi(konfigurasyonId);
            return Content(muhasebeAktarimYuzdesi.ToString());
        }

        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = AltMenuler.MuhasebeAktarimListesi, SekmeKodu = 0)]
        public ActionResult MuhasebeAktarimListesi()
        {
            MuhasebeAktarimModel model = new MuhasebeAktarimModel();
            MuhasebeAktarim item = new MuhasebeAktarim();
            var list = _Muhasebe_CariHesapService.GetMuhasebeAktarimListesi();
            var bransList = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            var sirketList = _SigortaSirketleriService.GetList();
            if (list.Count > 0)
            {
                for (int i = list.Count() - 1; i >= 0; i--)
                {
                    item = new MuhasebeAktarim();
                    item.AktarimTamamlandi = list[i].AktarimTamamlandi;
                    if (item.AktarimTamamlandi == 1)
                    {
                        item.AktarimTamamlandiText = babonline.TransferCompleted + "."; //Aktarım Tamamlandı
                    }
                    else
                    {
                        item.AktarimTamamlandiText = babonline.TransferInProgress + "."; // Aktarım Devam Ediyor
                    }
                    item.BaslangicTarihi = list[i].BaslangicTarihi;
                    item.BitisTarihi = list[i].BitisTarihi;
                    item.BransKodu = list[i].BransKodu;
                    if (item.BransKodu != null)
                    {
                        var bransDetay = bransList.Where(w => w.BransKodu == item.BransKodu).FirstOrDefault();
                        if (bransDetay != null)
                        {
                            item.BransAdi = bransDetay.BransAdi;
                        }
                    }
                    item.SirketKodu = list[i].SirketKodu;
                    if (item.SirketKodu != null)
                    {
                        var sirketDetay = sirketList.Where(w => w.SirketKodu == item.SirketKodu).FirstOrDefault();
                        if (sirketDetay != null)
                        {
                            item.SirketUnvan = sirketDetay.SirketAdi;
                        }
                    }
                    item.TVMKodu = list[i].TvmKodu;
                    item.TVMUnvani = list[i].TvmUnvani;
                    model.list.Add(item);
                }
            }
            return View(model);
        }

    }
}