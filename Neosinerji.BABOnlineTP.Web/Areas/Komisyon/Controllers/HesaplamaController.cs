using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Komisyon, AltMenuKodu = AltMenuler.KomisyonHesaplama, SekmeKodu = 0)]
    public class HesaplamaController : Controller
    {
        #region ServiceVariable

        IKomisyonService _KomisyonService;
        IPoliceService _PoliceService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;

        #endregion

        #region Constructor

        public HesaplamaController(IKomisyonService komisyonService, IPoliceService policeService, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
        {
            _KomisyonService = komisyonService;
            _PoliceService = policeService;
            _TVMService = TVMService;
            _AktifKullaniciService = aktifKullaniciService;
        }

        #endregion

        #region Public Method

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Liste(TaliKomisyonHesaplamaFiltreModel filtre)
        {
            try
            {
                var list = TaliAcentePoliceGenelListesi(filtre);
                return Json(new { Hata = false, Liste = list });
            }
            catch
            {
                return Json(new { Hata = true });

            }
        }

        [HttpPost]
        public ActionResult Oran(TaliKomisyonOranModel model)
        {
            return Json(new { Oran = TaliAcenteKomisyonOrani(model) });
            //return Json(new { Oran = 50 });
        }

        [HttpPost]
        public ActionResult Guncelle(TaliKomisyonHesaplamaListeModel model)
        {
            bool guncellendiMi = true;
            try
            {
                var police = _PoliceService.GetPolice(model.Id);
                police.OnaylayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                _PoliceService.UpdatePolice(police);

                int oncekiTaliKod = 0;
                if (police != null)
                {
                    oncekiTaliKod = police.TaliAcenteKodu.HasValue ? police.TaliAcenteKodu.Value : 0;
                }
                var kayit = TaliAcenteKomisyonGuncelle(model, oncekiTaliKod, out guncellendiMi);
                if (guncellendiMi)
                    return Json(new { Hata = false, Kayit = kayit });
                else
                    return Json(new { Hata = true, Kayit = kayit });
            }
            catch
            {
                return Json(new { Hata = true });
            }
        }

        #endregion

        #region Private Method

        private List<TaliKomisyonHesaplamaListeModel> TaliAcentePoliceGenelListesi(TaliKomisyonHesaplamaFiltreModel filtre)
        {
            var disKaynakList = _TVMService.GetDisUretimTVMListeKullaniciYetki(0);
            var teklifGenelList = _KomisyonService.TaliAcentePoliceGenelListesi(filtre.PoliceDurumu, filtre.TVMListe, filtre.DisKaynakListe, filtre.BransListe, filtre.SigortaSirketiListe, filtre.TarihBaslangic, filtre.TarihBitis, filtre.IptalZeylTahakkuk);
            var list = new List<TaliKomisyonHesaplamaListeModel>();
            foreach (var tg in teklifGenelList)
            {
                var lModel = new TaliKomisyonHesaplamaListeModel()
                {
                    Id = tg.PoliceId,
                    Tali = new TVMModel() { Kodu = tg.TaliAcenteKodu ?? -1 },
                    DisKaynakUnvani = tg.UretimTaliAcenteKodu.HasValue ? disKaynakList.Where(s => s.Kodu == tg.UretimTaliAcenteKodu).FirstOrDefault().Unvani : "",
                    SigortaSirketi = new SigortaSirketiModel() { Kodu = tg.SigortaSirketleri.SirketKodu, Adi = tg.SigortaSirketleri.SirketAdi },
                    Brans = new BransModel() { Kodu = tg.BransKodu ?? -1, Adi = tg.BransAdi },
                    Prim = tg.NetPrim ?? 0,
                    AlinanKomisyon = tg.Komisyon ?? 0,
                    PoliceTanzimTarihi = (tg.TanzimTarihi ?? new DateTime()).ToString("yyyy-MM-dd"),
                    PoliceNo = tg.PoliceNumarasi,
                    SigortaliUnvani = tg.PoliceSigortali.AdiUnvan + " " + tg.PoliceSigortali.SoyadiUnvan,
                    SigortaEttirenUnvani = tg.PoliceSigortaEttiren.AdiUnvan + " " + tg.PoliceSigortaEttiren.SoyadiUnvan,
                    TaliKomisyonOrani = tg.TaliKomisyonOran,
                    TaliKomisyon = tg.TaliKomisyon ?? 0
                };
                list.Add(lModel);
            }
            return list;
        }

        private TaliKomisyonHesaplamaListeModel TaliAcenteKomisyonGuncelle(TaliKomisyonHesaplamaListeModel model, int oncekiTaliKod, out bool guncellendiMi)
        {
            var disKaynakList = _TVMService.GetDisUretimTVMListeKullaniciYetki(0);
            var taliTvmKodu = model.Tali.Kodu <= 0 ? (int?)null : model.Tali.Kodu;
            var tg = _KomisyonService.TeklifGenelKomisyonGuncelle(model.Id, taliTvmKodu, oncekiTaliKod, model.TaliKomisyonOrani.Value, model.TaliKomisyon, out guncellendiMi);
            var lModel = new TaliKomisyonHesaplamaListeModel()
            {
                Id = tg.PoliceId,
                Tali = new TVMModel() { Kodu = tg.TaliAcenteKodu ?? -1 },
                DisKaynakUnvani = tg.UretimTaliAcenteKodu.HasValue ? disKaynakList.Where(s => s.Kodu == tg.UretimTaliAcenteKodu).FirstOrDefault().Unvani : "",
                SigortaSirketi = new SigortaSirketiModel() { Kodu = tg.SigortaSirketleri.SirketKodu, Adi = tg.SigortaSirketleri.SirketAdi },
                Brans = new BransModel() { Kodu = tg.BransKodu ?? -1, Adi = tg.BransAdi },
                Prim = tg.NetPrim ?? 0,
                AlinanKomisyon = tg.Komisyon ?? 0,
                PoliceTanzimTarihi = (tg.TanzimTarihi ?? new DateTime()).ToString("yyyy-MM-dd"),
                PoliceNo = tg.PoliceNumarasi,
                SigortaliUnvani = tg.PoliceSigortali.AdiUnvan + " " + tg.PoliceSigortali.SoyadiUnvan,
                SigortaEttirenUnvani = tg.PoliceSigortaEttiren.AdiUnvan + " " + tg.PoliceSigortaEttiren.SoyadiUnvan,
                TaliKomisyonOrani = tg.TaliKomisyonOran ?? 0,
                TaliKomisyon = tg.TaliKomisyon ?? 0
            };
            return lModel;
        }

        private decimal TaliAcenteKomisyonOrani(TaliKomisyonOranModel model)
        {
            return _KomisyonService.TaliAcenteKomisyonOrani(model.Tali.Kodu, model.Brans.Kodu, model.SigortaSirketi.Kodu, model.Tarih, model.policeNo);
        }

        #endregion

    }
}
