using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.UretimHedefPlanlanan;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.PoliceUretimHedefPlanlanan.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using System.Linq.Expressions;

namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceUretimHedefPlanlanan.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UretimHedef, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class PoliceUretimHedefPlaniController : Controller
    {
        IAktifKullaniciService _AktifKullaniciService;
        IPoliceUretimHedefPlanlananService _PoliceUretimHedefPlanlananService;
        public PoliceUretimHedefPlaniController(
                                IAktifKullaniciService aktifKullaniciService, IPoliceUretimHedefPlanlananService policeUretimHedefPlanlananService)
        {

            _AktifKullaniciService = aktifKullaniciService;
            _PoliceUretimHedefPlanlananService = policeUretimHedefPlanlananService;
        }

        //Odeme Girişi ekranının gelmesi için
        //[Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UretimHedef, SekmeKodu = 0)]
        public ActionResult UretimHedefPlanlamaEkrani()
        {

            IBransService _BransService = DependencyResolver.Current.GetService<IBransService>();
            PoliceUretimHedefPlanlananModel model = new PoliceUretimHedefPlanlananModel();
            List<Bran> bransListesi = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            PoliceUretimHedefPlanlananListe policeHedef;
            model.BransListe = bransListesi;
            model.AcenteTVMKodu = _AktifKullaniciService.TVMKodu;
            model.Year = 2015;
            var policeUretimPlanlanan = _PoliceUretimHedefPlanlananService.GetPoliceUretimHedefPlanlananListe(model.AcenteTVMKodu, model.Yil);
            model.policeUretimHedefPlanlananListe = new List<PoliceUretimHedefPlanlananListe>();
            // model.policeUretimHedefPlanlananModel = new List<PoliceUretimHedefPlanlananListe>();
            foreach (var item in bransListesi)
            {
                policeHedef = new PoliceUretimHedefPlanlananListe();
                policeHedef.BransKodu = item.BransKodu;
                policeHedef.BransAdi = BransListeCeviri.BransTipi(item.BransKodu) == "" ? item.BransAdi : BransListeCeviri.BransTipi(item.BransKodu);
                model.policeUretimHedefPlanlananListe.Add(policeHedef);

            }

            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil <= TurkeyDateTime.Today.Year + 1; yil++)
                yillar.Add(yil);
            policeHedef = new PoliceUretimHedefPlanlananListe();
            model.Yillar = new SelectList(yillar).ListWithOptionLabel();
            model.AcenteTVMKodu = policeHedef.AcenteTVMKodu;
            model.AcenteTVMUnvani = _AktifKullaniciService.TVMUnvani;
            model.AcenteTVMKodu = _AktifKullaniciService.TVMKodu;

            return View(model);
        }

        //Ödeme Girişindeki ara butonu için
        [HttpPost]
        //[Authorization(AnaMenuKodu =AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.UretimHedef, SekmeKodu = 0)]
        public ActionResult UretimHedefPlanlamaEkrani(PoliceUretimHedefPlanlananListe aramaModel)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            IBransService _BransService = DependencyResolver.Current.GetService<IBransService>();
            PoliceUretimHedefPlanlananModel model = new PoliceUretimHedefPlanlananModel();
            List<Bran> bransListesi = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            PoliceUretimHedefPlanlananListe policeHedef;
            model.BransListe = bransListesi;
            var policeUretimPlanlanan = _PoliceUretimHedefPlanlananService.GetPoliceUretimHedefPlanlananListe(aramaModel.AcenteTVMKodu, aramaModel.Year);
            //var policeUretimHedefPlaniAramaEkrani = _PoliceUretimHedefPlanlananService.GetPoliceUretimHedefPlaniAramaEkrani(Convert.ToInt32(aramaModel.AcenteTVMKodu), Convert.ToDateTime           (aramaModel.KayitTarihi));

            model.policeUretimHedefPlanlananListe = new List<PoliceUretimHedefPlanlananListe>();
            if (policeUretimPlanlanan.Count == 0)
            {
                foreach (var item in bransListesi)
                {
                    policeHedef = new PoliceUretimHedefPlanlananListe();
                    policeHedef.BransKodu = item.BransKodu;
                    policeHedef.BransAdi = BransListeCeviri.BransTipi(item.BransKodu) == "" ? item.BransAdi : BransListeCeviri.BransTipi(item.BransKodu);
                    model.policeUretimHedefPlanlananListe.Add(policeHedef);

                }
            }
            else
            {
                foreach (var item in policeUretimPlanlanan)
                {

                    policeHedef = new PoliceUretimHedefPlanlananListe();
                    policeHedef.BransKodu = Convert.ToInt32(item.BransKodu);
                    // policeHedef.PoliceAdedi = Convert.ToInt32(item.PoliceAdedi);
                    // policeHedef.Prim = Convert.ToDecimal(item.Prim);
                    policeHedef.OcakAdedi = Convert.ToInt32(item.PoliceAdedi1);
                    policeHedef.OcakPrim = Convert.ToDecimal(item.Prim1);
                    policeHedef.SubatAdedi = Convert.ToInt32(item.PoliceAdedi2);
                    policeHedef.SubatPrim = Convert.ToDecimal(item.Prim2);
                    policeHedef.MartAdedi = Convert.ToInt32(item.PoliceAdedi3);
                    policeHedef.MartPrim = Convert.ToDecimal(item.Prim3);
                    policeHedef.NisanAdedi = Convert.ToInt32(item.PoliceAdedi4);
                    policeHedef.NisanPrim = Convert.ToDecimal(item.Prim4);
                    policeHedef.MayisAdedi = Convert.ToInt32(item.PoliceAdedi5);
                    policeHedef.MayisPrim = Convert.ToDecimal(item.Prim5);
                    policeHedef.HaziranAdedi = Convert.ToInt32(item.PoliceAdedi6);
                    policeHedef.HaziranPrim = Convert.ToDecimal(item.Prim6);
                    policeHedef.TemmuzAdedi = Convert.ToInt32(item.PoliceAdedi7);
                    policeHedef.TemmuzPrim = Convert.ToDecimal(item.Prim7);
                    policeHedef.AgustosAdedi = Convert.ToInt32(item.PoliceAdedi8);
                    policeHedef.AgustosPrim = Convert.ToDecimal(item.Prim8);
                    policeHedef.EylulAdedi = Convert.ToInt32(item.PoliceAdedi9);
                    policeHedef.EylulPrim = Convert.ToDecimal(item.Prim9);
                    policeHedef.EkimAdedi = Convert.ToInt32(item.PoliceAdedi10);
                    policeHedef.EkimPrim = Convert.ToDecimal(item.Prim10);
                    policeHedef.KasimAdedi = Convert.ToInt32(item.PoliceAdedi11);
                    policeHedef.KasimPrim = Convert.ToDecimal(item.Prim11);
                    policeHedef.AralikAdedi = Convert.ToInt32(item.PoliceAdedi12);
                    policeHedef.AralikPrim = Convert.ToDecimal(item.Prim12);
                    policeHedef.KayitTarihi = Convert.ToDateTime(item.KayitTarihi);
                    policeHedef.GuncellemeTarihi = Convert.ToDateTime(item.GuncellemeTarihi);
                    policeHedef.Yil = item.Donem;
                    policeHedef.BransAdi = BransListeCeviri.BransTipi(item.BransKodu.Value) == "" ? bransListesi.Find(s => s.BransKodu == item.BransKodu).BransAdi : BransListeCeviri.BransTipi(item.BransKodu.Value); //bransListesi.Find(s => s.BransKodu == item.BransKodu).BransAdi;
                    policeHedef.KayitId = item.Id;
                    model.policeUretimHedefPlanlananListe.Add(policeHedef);
                }
            }


            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil <= TurkeyDateTime.Today.Year + 1; yil++)
                yillar.Add(yil);

            model.Yillar = new SelectList(yillar).ListWithOptionLabel();
            model.AcenteTVMKodu = aramaModel.AcenteTVMKodu;
            model.AcenteTVMUnvani = _TVMService.GetDetay(aramaModel.AcenteTVMKodu).Unvani;
            model.AcenteTVMKodu = _AktifKullaniciService.TVMKodu;
            // model.KayitTarihi = aramaModel.KayitTarihi;

            return View(model);
        }

        //Kaydet Güncelle butonun olacak
        public ActionResult Ekle(PoliceUretimHedefPlanlananModel model)
        {
            Neosinerji.BABOnlineTP.Database.Models.PoliceUretimHedefPlanlanan plan;

            int _TVMKodu = _AktifKullaniciService.TVMKodu;
            IBransService _BransService = DependencyResolver.Current.GetService<IBransService>();
            List<Bran> bransListesi = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BransListe = bransListesi;
            bool guncelle = false;
            bool responseEkle = false;

            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            int bagliOlduguTVMKodu = _TVMService.GetDetay(_TVMKodu).BagliOlduguTVMKodu;
            bool tali = false;
            if (bagliOlduguTVMKodu != -9999)
            {
                tali = true;
            }

            List<Neosinerji.BABOnlineTP.Database.Models.PoliceUretimHedefPlanlanan> kayitVarmi = new List<Database.Models.PoliceUretimHedefPlanlanan>();
            kayitVarmi = _PoliceUretimHedefPlanlananService.GetPoliceUretimHedefPlanlananListe(model.AcenteTVMKodu, model.Year);
            string mesaj = string.Empty;
            if (kayitVarmi.Count > 0)
            {

                int sayIndex = 0;

                foreach (var item in model.policeUretimHedefPlanlananListe)
                {
                    int sayac = 0;
                    Neosinerji.BABOnlineTP.Database.Models.PoliceUretimHedefPlanlanan guncellenecekKayit;
                    foreach (var itemguncel in kayitVarmi)
                    {
                        if (sayIndex != sayac)
                        {
                            sayac++;
                            continue;
                        }

                        guncellenecekKayit = new Database.Models.PoliceUretimHedefPlanlanan();
                        guncellenecekKayit = _PoliceUretimHedefPlanlananService.GetPoliceUretimHedefPlan(itemguncel.Id);
                        if (tali)
                        {
                            guncellenecekKayit.TVMKodu = bagliOlduguTVMKodu;
                            guncellenecekKayit.TVMKoduTali = _TVMKodu;
                        }
                        else if (model.AcenteTVMKodu != _TVMKodu)
                        {
                            guncellenecekKayit.TVMKodu = _TVMKodu;
                            guncellenecekKayit.TVMKoduTali = model.AcenteTVMKodu;
                        }
                        else
                        {
                            guncellenecekKayit.TVMKodu = _TVMKodu;
                            guncellenecekKayit.TVMKoduTali = null;
                        }

                        guncellenecekKayit.PoliceAdedi1 = item.OcakAdedi;
                        guncellenecekKayit.Prim1 = item.OcakPrim;
                        guncellenecekKayit.PoliceAdedi2 = item.SubatAdedi;
                        guncellenecekKayit.Prim2 = item.SubatPrim;
                        guncellenecekKayit.PoliceAdedi3 = item.MartAdedi;
                        guncellenecekKayit.Prim3 = item.MartPrim;
                        guncellenecekKayit.PoliceAdedi4 = item.NisanAdedi;
                        guncellenecekKayit.Prim4 = item.NisanPrim;
                        guncellenecekKayit.PoliceAdedi5 = item.MayisAdedi;
                        guncellenecekKayit.Prim5 = item.MayisPrim;
                        guncellenecekKayit.PoliceAdedi6 = item.HaziranAdedi;
                        guncellenecekKayit.Prim6 = item.HaziranPrim;
                        guncellenecekKayit.PoliceAdedi7 = item.TemmuzAdedi;
                        guncellenecekKayit.Prim7 = item.TemmuzPrim;
                        guncellenecekKayit.PoliceAdedi8 = item.AgustosAdedi;
                        guncellenecekKayit.Prim8 = item.AgustosPrim;
                        guncellenecekKayit.PoliceAdedi9 = item.EylulAdedi;
                        guncellenecekKayit.Prim9 = item.EylulPrim;
                        guncellenecekKayit.PoliceAdedi10 = item.EkimAdedi;
                        guncellenecekKayit.Prim10 = item.EkimPrim;
                        guncellenecekKayit.PoliceAdedi11 = item.KasimAdedi;
                        guncellenecekKayit.Prim11 = item.KasimPrim;
                        guncellenecekKayit.PoliceAdedi12 = item.AralikAdedi;
                        guncellenecekKayit.Prim12 = item.AralikPrim;

                        guncellenecekKayit.BransKodu = item.BransKodu;
                        guncellenecekKayit.Donem = model.Year;
                        guncellenecekKayit.GuncellemeTarihi = TurkeyDateTime.Today;
                        guncellenecekKayit.KayitTarihi = itemguncel.KayitTarihi;
                        guncelle = _PoliceUretimHedefPlanlananService.UpdatePoliceUretimHedefPlanlanan(guncellenecekKayit);
                        sayIndex++;
                        break;
                    }

                }
                mesaj = babonline.Updatedplans;
            }
            else if (model.policeUretimHedefPlanlananListe != null)
            {

                foreach (var item in model.policeUretimHedefPlanlananListe)
                {
                    Neosinerji.BABOnlineTP.Database.Models.PoliceUretimHedefPlanlanan planEkle;
                    planEkle = new Database.Models.PoliceUretimHedefPlanlanan();

                    if (tali)
                    {
                        planEkle.TVMKodu = bagliOlduguTVMKodu;
                        planEkle.TVMKoduTali = _TVMKodu;
                    }
                    else if (model.AcenteTVMKodu != _TVMKodu)
                    {
                        planEkle.TVMKodu = _TVMKodu;
                        planEkle.TVMKoduTali = model.AcenteTVMKodu;
                    }
                    else
                    {
                        planEkle.TVMKodu = _TVMKodu;
                        planEkle.TVMKoduTali = null;
                    }
                    planEkle.BransKodu = item.BransKodu;

                    planEkle.PoliceAdedi1 = item.OcakAdedi;
                    planEkle.Prim1 = item.OcakPrim;
                    planEkle.PoliceAdedi2 = item.SubatAdedi;
                    planEkle.Prim2 = item.SubatPrim;
                    planEkle.PoliceAdedi3 = item.MartAdedi;
                    planEkle.Prim3 = item.MartPrim;
                    planEkle.PoliceAdedi4 = item.NisanAdedi;
                    planEkle.Prim4 = item.NisanPrim;
                    planEkle.PoliceAdedi5 = item.MayisAdedi;
                    planEkle.Prim5 = item.MayisPrim;
                    planEkle.PoliceAdedi6 = item.HaziranAdedi;
                    planEkle.Prim6 = item.HaziranPrim;
                    planEkle.PoliceAdedi7 = item.TemmuzAdedi;
                    planEkle.Prim7 = item.TemmuzPrim;
                    planEkle.PoliceAdedi8 = item.AgustosAdedi;
                    planEkle.Prim8 = item.AgustosPrim;
                    planEkle.PoliceAdedi9 = item.EylulAdedi;
                    planEkle.Prim9 = item.EylulPrim;
                    planEkle.PoliceAdedi10 = item.EkimAdedi;
                    planEkle.Prim10 = item.EkimPrim;
                    planEkle.PoliceAdedi11 = item.KasimAdedi;
                    planEkle.Prim11 = item.KasimPrim;
                    planEkle.PoliceAdedi12 = item.AralikAdedi;
                    planEkle.Prim12 = item.AralikPrim;

                    planEkle.Donem = model.Year;
                    planEkle.GuncellemeTarihi = TurkeyDateTime.Today;
                    planEkle.KayitTarihi = TurkeyDateTime.Today;
                    responseEkle = _PoliceUretimHedefPlanlananService.CreatePoliceUretimHedefPlanlanan(planEkle);
                    mesaj = babonline.addedplans;

                }


            }

            else
            {
                mesaj = "Lütfen Zorunlu Alanları Doldurunuz.";
            }

            return Json(new { sum = mesaj });

        }

    }
}



