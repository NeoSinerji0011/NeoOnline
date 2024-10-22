using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Areas.OdemeGirisi.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.IO;


namespace Neosinerji.BABOnlineTP.Web.Areas.OdemeGirisi.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.MasrafGiderGirisi, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class OdemeGirisiController : Controller
    {
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        IKesintiTransferService _KesintiTransferService;
        public OdemeGirisiController(ITVMService tvmService, IAktifKullaniciService aktifKullanici, IKesintiTransferService kesintiTransferService)
            : base()
        {
            _AktifKullanici = aktifKullanici;
            _TVMService = tvmService;
            _KesintiTransferService = kesintiTransferService;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.MasrafGiderGirisi, SekmeKodu = 0)]
        public ActionResult Ekle()
        {
            OdemeGirisiModel model = new OdemeGirisiModel();
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year - 1; yil <= TurkeyDateTime.Today.Year; yil++)
                yillar.Add(yil);

            model.Donemler = new SelectList(yillar).ListWithOptionLabel();
            model.AcenteTVMKodu = _AktifKullanici.TVMKodu;
            model.odemelerGirisiListe = new List<OdemeGirisiList>();
            model.IslemTipi = 0;
            model.Islemler = new SelectList(Islemler.MasrafGirisiIslemTipleri(), "Value", "Text", model.IslemTipi);
            var kesintiTurleri = _TVMService.GetListKesintiTurleri();
            model.KesintiTurleri = kesintiTurleri;
            if (kesintiTurleri != null)
            {
                OdemeGirisiList odemelerGirisiListItem = new OdemeGirisiList();
                foreach (var item in kesintiTurleri)
                {
                    odemelerGirisiListItem = new OdemeGirisiList();
                    odemelerGirisiListItem.KesintiTuruKodu = item.KesintiKodu;
                    odemelerGirisiListItem.KesintiTuruAdi = item.KesintiAciklamasi;
                    model.odemelerGirisiListe.Add(odemelerGirisiListItem);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.MasrafGiderGirisi, SekmeKodu = 0)]
        public ActionResult Ekle(OdemeGirisiList aramaModel)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            OdemeGirisiModel model = new OdemeGirisiModel();
            model.Islemler = new SelectList(Islemler.MasrafGirisiIslemTipleri(), "Value", "Text", model.IslemTipi);
            var kesintiTurleri = _TVMService.GetListKesintiTurleri();
            model.KesintiTurleri = kesintiTurleri;

            var odemeGirisi = _TVMService.GetOdemeGirisiListe(aramaModel.AcenteTVMKodu, aramaModel.Donem);

            OdemeGirisiList odemeLsit;
            model.odemelerGirisiListe = new List<OdemeGirisiList>();
            if (odemeGirisi.Count == 0)
            {
                foreach (var item in kesintiTurleri)
                {
                    odemeLsit = new OdemeGirisiList();
                    odemeLsit.KesintiTuruKodu = item.KesintiKodu;
                    odemeLsit.KesintiTuruAdi = item.KesintiAciklamasi;
                    model.odemelerGirisiListe.Add(odemeLsit);
                }
            }
            else
            {
                foreach (var item in odemeGirisi)
                {
                    odemeLsit = new OdemeGirisiList();
                    odemeLsit.KesintiTuruKodu = item.KesintiKodu;
                    odemeLsit.OcakBorc = item.Borc1.HasValue ? item.Borc1.Value.ToString() : "";
                    odemeLsit.OcakAlacak = item.Alacak1.HasValue ? item.Alacak1.Value.ToString() : "";
                    odemeLsit.SubatBorc = item.Borc2.HasValue ? item.Borc2.Value.ToString() : "";
                    odemeLsit.SubatAlacak = item.Alacak2.HasValue ? item.Alacak2.Value.ToString() : "";
                    odemeLsit.MartBorc = item.Borc3.HasValue ? item.Borc3.Value.ToString() : "";
                    odemeLsit.MartAlacak = item.Alacak3.HasValue ? item.Alacak3.Value.ToString() : "";
                    odemeLsit.NisanBorc = item.Borc4.HasValue ? item.Borc4.Value.ToString() : "";
                    odemeLsit.NisanAlacak = item.Alacak4.HasValue ? item.Alacak4.Value.ToString() : "";
                    odemeLsit.MayisBorc = item.Borc5.HasValue ? item.Borc5.Value.ToString() : "";
                    odemeLsit.MayisAlacak = item.Alacak5.HasValue ? item.Alacak5.Value.ToString() : "";
                    odemeLsit.HaziranBorc = item.Borc6.HasValue ? item.Borc6.Value.ToString() : "";
                    odemeLsit.HaziranAlacak = item.Alacak6.HasValue ? item.Alacak6.Value.ToString() : "";
                    odemeLsit.TemmuzBorc = item.Borc7.HasValue ? item.Borc7.Value.ToString() : "";
                    odemeLsit.TemmuzAlacak = item.Alacak7.HasValue ? item.Alacak7.Value.ToString() : "";
                    odemeLsit.AgustosBorc = item.Borc8.HasValue ? item.Borc8.Value.ToString() : "";
                    odemeLsit.AgustosAlacak = item.Alacak8.HasValue ? item.Alacak8.Value.ToString() : "";
                    odemeLsit.EylulBorc = item.Borc9.HasValue ? item.Borc9.Value.ToString() : "";
                    odemeLsit.EylulAlacak = item.Alacak9.HasValue ? item.Alacak9.Value.ToString() : "";
                    odemeLsit.EkimBorc = item.Borc10.HasValue ? item.Borc10.Value.ToString() : "";
                    odemeLsit.EkimAlacak = item.Alacak10.HasValue ? item.Alacak10.Value.ToString() : "";
                    odemeLsit.KasimBorc = item.Borc11.HasValue ? item.Borc11.Value.ToString() : "";
                    odemeLsit.KasimAlacak = item.Alacak11.HasValue ? item.Alacak11.Value.ToString() : "";
                    odemeLsit.AralikBorc = item.Borc12.HasValue ? item.Borc12.Value.ToString() : "";
                    odemeLsit.AralikAlacak = item.Alacak12.HasValue ? item.Alacak12.Value.ToString() : "";
                    odemeLsit.KayitTarihi = Convert.ToDateTime(item.KayitTarihi);
                    odemeLsit.GuncellemeTarihi = Convert.ToDateTime(item.GuncellemeTarihi);
                    odemeLsit.Donem = item.Donem;
                    var kesintiDetay = kesintiTurleri.Where(s => s.KesintiKodu == item.KesintiKodu).FirstOrDefault();
                    if (kesintiDetay != null)
                    {
                        odemeLsit.KesintiTuruAdi = kesintiDetay.KesintiAciklamasi;
                    }
                    else
                    {
                        odemeLsit.KesintiTuruAdi = "";
                    }
                    odemeLsit.KayitId = item.Id;
                    model.odemelerGirisiListe.Add(odemeLsit);
                }
            }


            List<int> donemler = new List<int>();
            for (int donem = TurkeyDateTime.Today.Year - 1; donem <= TurkeyDateTime.Today.Year; donem++)
                donemler.Add(donem);

            model.Donemler = new SelectList(donemler).ListWithOptionLabel();
            model.AcenteTVMKodu = aramaModel.AcenteTVMKodu;
            model.AcenteTVMUnvani = _TVMService.GetDetay(aramaModel.AcenteTVMKodu).Unvani;
            model.AcenteTVMKodu = _AktifKullanici.TVMKodu;
            // model.KayitTarihi = aramaModel.KayitTarihi; 
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            return View(model);
        }

        public ActionResult KaydetGuncelle(OdemeGirisiModel model)
        {
            int _TVMKodu = _AktifKullanici.TVMKodu;
            string mesaj = string.Empty;
            bool guncelle = false;
            bool responseEkle = false;

            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            int bagliOlduguTVMKodu = _TVMService.GetDetay(_TVMKodu).BagliOlduguTVMKodu;
            bool tali = false;

            if (bagliOlduguTVMKodu != -9999)
            {
                tali = true;
            }
            var kesintiListesi = _TVMService.GetMasrafListesi(_AktifKullanici.TVMKodu, model.Donem);

            if (model.IslemTipi == 1)
            {
                if (model.tvmList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.tvmList)
                    {
                        if (item != "multiselect-all" && item != "")
                        {
                            liste.Add(item);
                        }
                    }
                    model.TVMListe = String.Empty;
                    if (liste.Count > 0)
                    {
                        for (int i = 0; i < liste.Count; i++)
                        {
                            if (i != liste.Count - 1)
                                model.TVMListe = model.TVMListe + liste[i] + ",";
                            else model.TVMListe = model.TVMListe + liste[i];

                            foreach (var item in model.odemelerGirisiListe)
                            {
                                var kesintiKayitListesiVarMi = kesintiListesi.Where(s => s.TVMKoduTali == Convert.ToInt32(liste[i]) && s.Donem == model.Donem).ToList();

                                if (kesintiKayitListesiVarMi.Count > 0)
                                {

                                    foreach (var itemKesinti in kesintiKayitListesiVarMi)
                                    {
                                        if (item.KesintiTuruKodu == itemKesinti.KesintiKodu)
                                        {
                                            itemKesinti.Borc1 = Convert.ToDecimal(item.OcakBorc);
                                            itemKesinti.Alacak1 = Convert.ToDecimal(item.OcakAlacak);
                                            itemKesinti.Borc2 = Convert.ToDecimal(item.SubatBorc);
                                            itemKesinti.Alacak2 = Convert.ToDecimal(item.SubatAlacak);
                                            itemKesinti.Borc3 = Convert.ToDecimal(item.MartBorc);
                                            itemKesinti.Alacak3 = Convert.ToDecimal(item.MartAlacak);
                                            itemKesinti.Borc4 = Convert.ToDecimal(item.NisanBorc);
                                            itemKesinti.Alacak4 = Convert.ToDecimal(item.NisanAlacak);
                                            itemKesinti.Borc5 = Convert.ToDecimal(item.MayisBorc);
                                            itemKesinti.Alacak5 = Convert.ToDecimal(item.MayisAlacak);
                                            itemKesinti.Borc6 = Convert.ToDecimal(item.HaziranBorc);
                                            itemKesinti.Alacak6 = Convert.ToDecimal(item.HaziranAlacak);
                                            itemKesinti.Borc7 = Convert.ToDecimal(item.TemmuzBorc);
                                            itemKesinti.Alacak7 = Convert.ToDecimal(item.TemmuzAlacak);
                                            itemKesinti.Borc8 = Convert.ToDecimal(item.AgustosBorc);
                                            itemKesinti.Alacak8 = Convert.ToDecimal(item.AgustosAlacak);
                                            itemKesinti.Borc9 = Convert.ToDecimal(item.EylulBorc);
                                            itemKesinti.Alacak9 = Convert.ToDecimal(item.EylulAlacak);
                                            itemKesinti.Borc10 = Convert.ToDecimal(item.EkimBorc);
                                            itemKesinti.Alacak10 = Convert.ToDecimal(item.EkimAlacak);
                                            itemKesinti.Borc11 = Convert.ToDecimal(item.KasimBorc);
                                            itemKesinti.Alacak11 = Convert.ToDecimal(item.KasimAlacak);
                                            itemKesinti.Borc12 = Convert.ToDecimal(item.AralikBorc);
                                            itemKesinti.Alacak12 = Convert.ToDecimal(item.AralikAlacak);
                                            itemKesinti.Donem = model.Donem;
                                            itemKesinti.GuncellemeTarihi = TurkeyDateTime.Today;
                                            guncelle = _TVMService.UpdateOdemeGirisi(itemKesinti);
                                            mesaj = "Kayıtlar güncellendi.";
                                        }

                                    }
                                }
                                else
                                {
                                    Neosinerji.BABOnlineTP.Database.Models.Kesintiler kesintiEkle;
                                    kesintiEkle = new Database.Models.Kesintiler();

                                    if (tali)
                                    {
                                        kesintiEkle.TVMKodu = bagliOlduguTVMKodu;
                                        kesintiEkle.TVMKoduTali = Convert.ToInt32(liste[i]);
                                    }
                                    else if (Convert.ToInt32(liste[i]) != _TVMKodu)
                                    {
                                        kesintiEkle.TVMKodu = _TVMKodu;
                                        kesintiEkle.TVMKoduTali = Convert.ToInt32(liste[i]);
                                    }
                                    else
                                    {
                                        kesintiEkle.TVMKodu = _TVMKodu;
                                        kesintiEkle.TVMKoduTali = null;
                                    }
                                    kesintiEkle.KesintiKodu = item.KesintiTuruKodu;

                                    kesintiEkle.Borc1 = Convert.ToDecimal(item.OcakBorc);
                                    kesintiEkle.Alacak1 = Convert.ToDecimal(item.OcakAlacak);
                                    kesintiEkle.Borc2 = Convert.ToDecimal(item.SubatBorc);
                                    kesintiEkle.Alacak2 = Convert.ToDecimal(item.SubatAlacak);
                                    kesintiEkle.Borc3 = Convert.ToDecimal(item.MartBorc);
                                    kesintiEkle.Alacak3 = Convert.ToDecimal(item.MartAlacak);
                                    kesintiEkle.Borc4 = Convert.ToDecimal(item.NisanBorc);
                                    kesintiEkle.Alacak4 = Convert.ToDecimal(item.NisanAlacak);
                                    kesintiEkle.Borc5 = Convert.ToDecimal(item.MayisBorc);
                                    kesintiEkle.Alacak5 = Convert.ToDecimal(item.MayisAlacak);
                                    kesintiEkle.Borc6 = Convert.ToDecimal(item.HaziranBorc);
                                    kesintiEkle.Alacak6 = Convert.ToDecimal(item.HaziranAlacak);
                                    kesintiEkle.Borc7 = Convert.ToDecimal(item.TemmuzBorc);
                                    kesintiEkle.Alacak7 = Convert.ToDecimal(item.TemmuzAlacak);
                                    kesintiEkle.Borc8 = Convert.ToDecimal(item.AgustosBorc);
                                    kesintiEkle.Alacak8 = Convert.ToDecimal(item.AgustosAlacak);
                                    kesintiEkle.Borc9 = Convert.ToDecimal(item.EylulBorc);
                                    kesintiEkle.Alacak9 = Convert.ToDecimal(item.EylulAlacak);
                                    kesintiEkle.Borc10 = Convert.ToDecimal(item.EkimBorc);
                                    kesintiEkle.Alacak10 = Convert.ToDecimal(item.EkimAlacak);
                                    kesintiEkle.Borc11 = Convert.ToDecimal(item.KasimBorc);
                                    kesintiEkle.Alacak11 = Convert.ToDecimal(item.KasimAlacak);
                                    kesintiEkle.Borc12 = Convert.ToDecimal(item.AralikBorc);
                                    kesintiEkle.Alacak12 = Convert.ToDecimal(item.AralikAlacak);

                                    kesintiEkle.Donem = model.Donem;
                                    kesintiEkle.GuncellemeTarihi = TurkeyDateTime.Today;
                                    kesintiEkle.KayitTarihi = TurkeyDateTime.Today;
                                    responseEkle = _TVMService.CreateOdemeGirisi(kesintiEkle);
                                    mesaj = "Kayıtlar eklendi.";
                                }
                            }
                        }
                    }
                    else
                    {
                        model.TVMListe = String.Empty;
                    }
                }
                else
                {
                    // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                    model.TVMListe = String.Empty;
                }
            }
            else if (model.IslemTipi == 0)
            {
                if (String.IsNullOrEmpty(model.TVMListe))
                {
                    List<Neosinerji.BABOnlineTP.Database.Models.Kesintiler> kayitVarmi = new List<Database.Models.Kesintiler>();
                    kayitVarmi = _TVMService.GetOdemeGirisiListe(model.AcenteTVMKodu, model.Donem);
                    mesaj = string.Empty;
                    if (kayitVarmi.Count > 0)
                    {
                        int sayIndex = 0;

                        foreach (var item in model.odemelerGirisiListe)
                        {
                            int sayac = 0;
                            Neosinerji.BABOnlineTP.Database.Models.Kesintiler guncellenecekKayit;
                            foreach (var itemguncel in kayitVarmi)
                            {
                                if (sayIndex != sayac)
                                {
                                    sayac++;
                                    continue;
                                }

                                guncellenecekKayit = new Database.Models.Kesintiler();
                                guncellenecekKayit = _TVMService.GetOdemeGirisiList(itemguncel.Id);
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
                                guncellenecekKayit.Borc1 = Convert.ToDecimal(item.OcakBorc);
                                guncellenecekKayit.Alacak1 = Convert.ToDecimal(item.OcakAlacak);
                                guncellenecekKayit.Borc2 = Convert.ToDecimal(item.SubatBorc);
                                guncellenecekKayit.Alacak2 = Convert.ToDecimal(item.SubatAlacak);
                                guncellenecekKayit.Borc3 = Convert.ToDecimal(item.MartBorc);
                                guncellenecekKayit.Alacak3 = Convert.ToDecimal(item.MartAlacak);
                                guncellenecekKayit.Borc4 = Convert.ToDecimal(item.NisanBorc);
                                guncellenecekKayit.Alacak4 = Convert.ToDecimal(item.NisanAlacak);
                                guncellenecekKayit.Borc5 = Convert.ToDecimal(item.MayisBorc);
                                guncellenecekKayit.Alacak5 = Convert.ToDecimal(item.MayisAlacak);
                                guncellenecekKayit.Borc6 = Convert.ToDecimal(item.HaziranBorc);
                                guncellenecekKayit.Alacak6 = Convert.ToDecimal(item.HaziranAlacak);
                                guncellenecekKayit.Borc7 = Convert.ToDecimal(item.TemmuzBorc);
                                guncellenecekKayit.Alacak7 = Convert.ToDecimal(item.TemmuzAlacak);
                                guncellenecekKayit.Borc8 = Convert.ToDecimal(item.AgustosBorc);
                                guncellenecekKayit.Alacak8 = Convert.ToDecimal(item.AgustosAlacak);
                                guncellenecekKayit.Borc9 = Convert.ToDecimal(item.EylulBorc);
                                guncellenecekKayit.Alacak9 = Convert.ToDecimal(item.EylulAlacak);
                                guncellenecekKayit.Borc10 = Convert.ToDecimal(item.EkimBorc);
                                guncellenecekKayit.Alacak10 = Convert.ToDecimal(item.EkimAlacak);
                                guncellenecekKayit.Borc11 = Convert.ToDecimal(item.KasimBorc);
                                guncellenecekKayit.Alacak11 = Convert.ToDecimal(item.KasimAlacak);
                                guncellenecekKayit.Borc12 = Convert.ToDecimal(item.AralikBorc);
                                guncellenecekKayit.Alacak12 = Convert.ToDecimal(item.AralikAlacak);

                                guncellenecekKayit.KesintiKodu = item.KesintiTuruKodu;
                                guncellenecekKayit.Donem = model.Donem;
                                guncellenecekKayit.GuncellemeTarihi = TurkeyDateTime.Today;
                                guncellenecekKayit.KayitTarihi = itemguncel.KayitTarihi;
                                guncelle = _TVMService.UpdateOdemeGirisi(guncellenecekKayit);
                                sayIndex++;
                                break;
                            }
                        }
                        mesaj = babonline.CostExpenseEntryUpdate;
                    }
                    else if (model.odemelerGirisiListe != null)
                    {
                        foreach (var item in model.odemelerGirisiListe)
                        {
                            Neosinerji.BABOnlineTP.Database.Models.Kesintiler kesintiEkle;
                            kesintiEkle = new Database.Models.Kesintiler();

                            if (tali)
                            {
                                kesintiEkle.TVMKodu = bagliOlduguTVMKodu;
                                kesintiEkle.TVMKoduTali = _TVMKodu;
                            }
                            else if (model.AcenteTVMKodu != _TVMKodu)
                            {
                                kesintiEkle.TVMKodu = _TVMKodu;
                                kesintiEkle.TVMKoduTali = model.AcenteTVMKodu;
                            }
                            else
                            {
                                kesintiEkle.TVMKodu = _TVMKodu;
                                kesintiEkle.TVMKoduTali = null;
                            }
                            kesintiEkle.KesintiKodu = item.KesintiTuruKodu;

                            kesintiEkle.Borc1 = Convert.ToDecimal(item.OcakBorc);
                            kesintiEkle.Alacak1 = Convert.ToDecimal(item.OcakAlacak);
                            kesintiEkle.Borc2 = Convert.ToDecimal(item.SubatBorc);
                            kesintiEkle.Alacak2 = Convert.ToDecimal(item.SubatAlacak);
                            kesintiEkle.Borc3 = Convert.ToDecimal(item.MartBorc);
                            kesintiEkle.Alacak3 = Convert.ToDecimal(item.MartAlacak);
                            kesintiEkle.Borc4 = Convert.ToDecimal(item.NisanBorc);
                            kesintiEkle.Alacak4 = Convert.ToDecimal(item.NisanAlacak);
                            kesintiEkle.Borc5 = Convert.ToDecimal(item.MayisBorc);
                            kesintiEkle.Alacak5 = Convert.ToDecimal(item.MayisAlacak);
                            kesintiEkle.Borc6 = Convert.ToDecimal(item.HaziranBorc);
                            kesintiEkle.Alacak6 = Convert.ToDecimal(item.HaziranAlacak);
                            kesintiEkle.Borc7 = Convert.ToDecimal(item.TemmuzBorc);
                            kesintiEkle.Alacak7 = Convert.ToDecimal(item.TemmuzAlacak);
                            kesintiEkle.Borc8 = Convert.ToDecimal(item.AgustosBorc);
                            kesintiEkle.Alacak8 = Convert.ToDecimal(item.AgustosAlacak);
                            kesintiEkle.Borc9 = Convert.ToDecimal(item.EylulBorc);
                            kesintiEkle.Alacak9 = Convert.ToDecimal(item.EylulAlacak);
                            kesintiEkle.Borc10 = Convert.ToDecimal(item.EkimBorc);
                            kesintiEkle.Alacak10 = Convert.ToDecimal(item.EkimAlacak);
                            kesintiEkle.Borc11 = Convert.ToDecimal(item.KasimBorc);
                            kesintiEkle.Alacak11 = Convert.ToDecimal(item.KasimAlacak);
                            kesintiEkle.Borc12 = Convert.ToDecimal(item.AralikBorc);
                            kesintiEkle.Alacak12 = Convert.ToDecimal(item.AralikAlacak);

                            kesintiEkle.Donem = model.Donem;
                            kesintiEkle.GuncellemeTarihi = TurkeyDateTime.Today;
                            kesintiEkle.KayitTarihi = TurkeyDateTime.Today;
                            responseEkle = _TVMService.CreateOdemeGirisi(kesintiEkle);
                            mesaj = babonline.CostExpenseEntryAdd;
                        }
                    }
                    else
                    {
                        mesaj = "Lütfen Zorunlu Alanları Doldurunuz.";
                    }
                }
            }

            return Json(new { sum = mesaj });
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.KesintiTransfer, SekmeKodu = 0)]
        public ActionResult KesintiTransfer()
        {
            OdemeGirisiTransferModel model = new OdemeGirisiTransferModel();
            List<SelectListItem> listYillar = new List<SelectListItem>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 2016; yil--)
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
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.KesintiTransfer, SekmeKodu = 0)]
        public ActionResult KesintiTransfer(OdemeGirisiTransferModel model, HttpPostedFileBase file)
        {
            string mesaj = "";
            try
            {
                if (ModelState.IsValid && file != null && file.ContentLength > 0)
                {
                    string path = String.Empty;

                    path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
                    file.SaveAs(path);

                    var kesintiler = _KesintiTransferService.getKesintiler(path, _AktifKullanici.TVMKodu, model.Ay, model.Yil);
                    foreach (var item in kesintiler)
                    {
                        bool responseEkle = _TVMService.CreateOdemeGirisi(item);
                        mesaj = "Kesintiler eklendi.";
                    }
                }
                else
                {
                    mesaj = "Lütfen girdiğiniz bilgileri kontrol ediniz.";
                }
            }
            catch (Exception ex)
            {
                mesaj = "İşlem sırasında bir hata oluştu." + ex.ToString();
                throw;
            }
            return Json(new { sum = mesaj });
        }
    }
}
