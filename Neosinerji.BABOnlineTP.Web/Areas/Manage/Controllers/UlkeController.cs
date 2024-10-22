using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim)]
    public class UlkeController : Controller
    {
        IUlkeService _ulkeService;
        IAktifKullaniciService _AktifKullanici;

        public UlkeController(IUlkeService ulkeService, IAktifKullaniciService aktifKullanici)
        {
            _ulkeService = ulkeService;
            _AktifKullanici = aktifKullanici;
        }

        public ActionResult Liste()
        {
            List<Ulke> ulkeler = _ulkeService.GetUlkeList().ToList<Ulke>();

            Mapper.CreateMap(typeof(List<Ulke>), typeof(List<UlkeModel>));
            List<UlkeModel> model = Mapper.Map<List<UlkeModel>>(ulkeler);

            return View(model);
        }

        public ActionResult Detay(string ulkeKodu)
        {
            Ulke ulke = _ulkeService.GetUlke(ulkeKodu);

            Mapper.CreateMap(typeof(Ulke), typeof(UlkeModel));
            UlkeModel model = Mapper.Map<UlkeModel>(ulke);

            return View(model);
        }

        public ActionResult Ekle()
        {
            UlkeModel model = new UlkeModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(UlkeModel model)
        {
            if (ModelState.IsValid)
            {
                Ulke ulke = new Ulke();
                ulke.UlkeKodu = model.UlkeKodu;
                ulke.UlkeAdi = model.UlkeAdi;

                ulke = _ulkeService.CreateUlke(ulke);

                return RedirectToAction("Detay", "Ulke", new { ulkeKodu = ulke.UlkeKodu });
            }

            return View(model);
        }

        public ActionResult Guncelle(string ulkeKodu)
        {
            Ulke ulke = _ulkeService.GetUlke(ulkeKodu);

            Mapper.CreateMap(typeof(Ulke), typeof(UlkeModel));
            UlkeModel model = Mapper.Map<UlkeModel>(ulke);

            return View();
        }

        [HttpPost]
        public ActionResult Guncelle(UlkeModel model)
        {
            if (ModelState.IsValid)
            {
                Ulke ulke = _ulkeService.GetUlke(model.UlkeKodu);
                ulke.UlkeAdi = model.UlkeAdi;

                _ulkeService.UpdateUlke(ulke);

                return RedirectToAction("Detay", "Ulke", new { ulkeKodu = ulke.UlkeKodu });
            }

            return View(model);
        }
    }
}
