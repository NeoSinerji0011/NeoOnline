using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class YetkiService : IYetkiService
    {
        ITVMContext _TVMContext;

        public YetkiService(ITVMContext tvm)
        {
            _TVMContext = tvm;
        }

        #region YetkiGrup Members

        public string GetYetkiGrupAdi(int yetkiGrupKodu)
        {
            TVMYetkiGruplari yetki = _TVMContext.TVMYetkiGruplariRepository.Find(s => s.YetkiGrupKodu == yetkiGrupKodu);
            if (yetki != null)
                return yetki.YetkiGrupAdi;
            else
                return "";
        }

        public TVMYetkiGruplari GetYetkiGrup(int yetkiGrupKodu)
        {
            return _TVMContext.TVMYetkiGruplariRepository.FindById(yetkiGrupKodu);
        }
        public TVMYetkiGruplari GetYetkiGrupKontrolu(int yetkiGrupKodu,int tvmkodu)
        {
            return _TVMContext.TVMYetkiGruplariRepository.Find(s=>s.YetkiGrupKodu==yetkiGrupKodu && s.TVMKodu==tvmkodu);
           
        }
        public List<YetkiListeServiceModel> GetListYetkiGrup(int? kullaniciKodu)
        {
            // ==== Aktif kullanıcının tvm bilgisi alınıyor ==== //
            IAktifKullaniciService aktifService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            int aktifTVMKodu = aktifService.TVMKodu;

            List<YetkiListeServiceModel> model = new List<YetkiListeServiceModel>();
            IQueryable<TVMDetay> tvmlist = _TVMContext.TVMDetayRepository.All();
            IQueryable<TVMYetkiGruplari> yetkiGruplari = _TVMContext.TVMYetkiGruplariRepository.All();


            // ==== Neosinerji için herhangi bir kısıtlama yok ==== //
            if (aktifTVMKodu != NeosinerjiTVM.NeosinerjiTVMKodu)
            {
                List<TVMDetay> baglitvmlist = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == aktifTVMKodu || s.Kodu == aktifTVMKodu).ToList();

                List<int> tvmKodlari = new List<int>();

                foreach (var item in baglitvmlist)
                {
                    tvmKodlari.Add(item.Kodu);
                }

                yetkiGruplari = from y in yetkiGruplari
                                where tvmKodlari.Contains(y.TVMKodu)
                                select y;
            }

            var list = from yet in yetkiGruplari
                       join tvm in tvmlist on yet.TVMKodu equals tvm.Kodu
                       select new { TVMKodu = tvm.Kodu, TVMUnvani = tvm.Unvani, YetkiKodu = yet.YetkiGrupKodu, YetkiAdi = yet.YetkiGrupAdi };

            foreach (var item in list)
            {
                YetkiListeServiceModel mdl = new YetkiListeServiceModel();
                mdl.TVMKodu = item.TVMKodu;
                mdl.TVMUnvani = item.TVMUnvani;
                mdl.YetkiGrupAdi = item.YetkiAdi;
                mdl.YetkiGrupKodu = item.YetkiKodu;
                model.Add(mdl);
            }
            return model;
        }

        public List<YetkiListeServiceModel> GetListYetkiGrupByTVMKodu(int tvmKodu)
        {
            List<YetkiListeServiceModel> model = new List<YetkiListeServiceModel>();
            IQueryable<TVMYetkiGruplari> yetkiGruplari = _TVMContext.TVMYetkiGruplariRepository.Filter(s => s.TVMKodu == tvmKodu);
            IQueryable<TVMDetay> tvmlist = _TVMContext.TVMDetayRepository.All();

            var list = from yet in yetkiGruplari
                       join tvm in tvmlist on yet.TVMKodu equals tvm.Kodu
                       select new { TVMKodu = tvm.Kodu, TVMUnvani = tvm.Unvani, YetkiKodu = yet.YetkiGrupKodu, YetkiAdi = yet.YetkiGrupAdi };

            foreach (var item in list)
            {
                YetkiListeServiceModel mdl = new YetkiListeServiceModel();
                mdl.TVMKodu = item.TVMKodu;
                mdl.TVMUnvani = item.TVMUnvani;
                mdl.YetkiGrupAdi = item.YetkiAdi;
                mdl.YetkiGrupKodu = item.YetkiKodu;
                model.Add(mdl);
            }
            return model;
        }

        public List<TVMYetkiGruplari> GetListYetkiGrup(int TVMKodu)
        {
            List<TVMYetkiGruplari> list = new List<TVMYetkiGruplari>();

            TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == TVMKodu).FirstOrDefault();

            if (tvm != null)
            {
                list = _TVMContext.TVMYetkiGruplariRepository.Filter(s => s.TVMKodu == tvm.Kodu || s.TVMKodu == tvm.BagliOlduguTVMKodu).ToList<TVMYetkiGruplari>();
            }

            if (tvm.BagliOlduguTVMKodu > 0)
            {
                list = list.Where(w => !w.YetkiSeviyesi.Value).ToList<TVMYetkiGruplari>();
            }

            return list;
        }

        public TVMYetkiGruplari CreateYetkiGrubu(TVMYetkiGruplari yetkiGrubu)
        {
            yetkiGrubu = _TVMContext.TVMYetkiGruplariRepository.Create(yetkiGrubu);
            _TVMContext.Commit();
            return yetkiGrubu;
        }

        public void UpdateYetkiGrubu(TVMYetkiGruplari yetkiGrubu)
        {
            _TVMContext.TVMYetkiGruplariRepository.Update(yetkiGrubu);
            _TVMContext.Commit();
        }

        public bool CheckAuthorityYetkiGrup(int yetkiGrupKodu)
        {
            IAktifKullaniciService _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            if (_AktifKullanici != null)
            {
                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    return true;

                IQueryable<TVMDetay> yetkiliTVMList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu ||
                                                                                                 s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);

                TVMYetkiGruplari yetkiGrubu = _TVMContext.TVMYetkiGruplariRepository.Filter(s => s.YetkiGrupKodu == yetkiGrupKodu).FirstOrDefault();

                if (yetkiGrubu != null)
                {
                    foreach (var item in yetkiliTVMList)
                        if (item.Kodu == yetkiGrubu.TVMKodu) return true;
                }
            }

            return false;
        }

        #endregion

        #region YetkiGrupYetki Members


        public List<TVMYetkiGrupYetkileri> GetListYetkiGrupYetki(int yetkiGrupkodu)
        {
            return _TVMContext.TVMYetkiGrupYetkileriRepository.Filter(s => s.YetkiGrupKodu == yetkiGrupkodu).ToList<TVMYetkiGrupYetkileri>();
        }

        public TVMYetkiGrupYetkileri CreateYetkiGrupYetkileri(TVMYetkiGrupYetkileri yetkiGrupYetkileri)
        {
            yetkiGrupYetkileri = _TVMContext.TVMYetkiGrupYetkileriRepository.Create(yetkiGrupYetkileri);
            _TVMContext.Commit();
            return yetkiGrupYetkileri;
        }

        public void UpdateYetkiGrupYetkileri(TVMYetkiGrupYetkileri yetkiGrupYetkileri)
        {
            _TVMContext.TVMYetkiGrupYetkileriRepository.Update(yetkiGrupYetkileri);
            _TVMContext.Commit();
        }

        #endregion

        public List<TVMYetkiGrupSablonu> GetListTVMYetkiGrupSablonu(int yetkiGrupkodu)
        {
            var list= _TVMContext.TVMYetkiGrupSablonuRepository.All().Where(s => s.YetkiGrupKodu == yetkiGrupkodu).ToList<TVMYetkiGrupSablonu>();

            return list;
        }
    }
}
