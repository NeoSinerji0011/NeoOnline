using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MenuService : IMenuService
    {
        IParametreContext _UnitOfWork;
        ITVMContext _TVMContext;
        IAktifKullaniciService _AktifKullanici;

        public MenuService(IParametreContext unitOfWork, ITVMContext tvm)
        {
            _UnitOfWork = unitOfWork;
            _TVMContext = tvm;

        }

        #region AnaMenuService Members

        public List<AnaMenu> GetAnaMenuListYetkili()
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            List<AnaMenu> list = new List<AnaMenu>();

            //Neosinerji yöneticisine kısıtlama yok ve yetkiye baglı olmadan menü isimlerinde multi-language sağlamak için yapıldı.
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            {
                int lang = 1;
                string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                if (!String.IsNullOrEmpty(language))
                    switch (language)
                    {
                        case "tr": lang = 1; break;
                        case "en": lang = 2; break;
                        case "it": lang = 3; break;
                        case "fr": lang = 4; break;
                        case "es": lang = 5; break;
                    }

                List<DilAciklama> DilAciklamaAnaMenuler = _UnitOfWork.DilAciklamaRepository.Filter(s => s.DilId == lang && s.TabloAdi == "AnaMenu").ToList();
                IQueryable<AnaMenu> anaMenuler = _UnitOfWork.AnaMenuRepository.All();
                foreach (var item in DilAciklamaAnaMenuler)
                {
                    AnaMenu anamenu = anaMenuler.Where(s => s.AnaMenuKodu == item.TabloId).FirstOrDefault();
                    if (anamenu != null)
                    {
                        anamenu.Aciklama = item.DilAciklama_1;
                        list.Add(anamenu);
                    }
                }


               

                return list;
            }
            else
            {
                List<KullaniciYetkiModel> yetkiliAnaMenuler = _AktifKullanici.Yetkiler.Where(s => s.AnaMenu == 0 && s.SekmeKodu == 0).ToList<KullaniciYetkiModel>();
                IQueryable<AnaMenu> anaMenuler = _UnitOfWork.AnaMenuRepository.All();

                foreach (var item in yetkiliAnaMenuler)
                {
                    AnaMenu anamenu = anaMenuler.Where(s => s.AnaMenuKodu == item.MenuKodu).FirstOrDefault();
                    if (anamenu != null)
                    {
                        anamenu.Aciklama = item.Aciklama;
                        list.Add(anamenu);
                    }
                }
            }

            return list;
        }

        public List<AnaMenu> GetAnaMenuList()
        {
            return _UnitOfWork.AnaMenuRepository.All().ToList<AnaMenu>();
        }

        public AnaMenu GetAnaMenu(int Id)
        {
            AnaMenu anaMenu = _UnitOfWork.AnaMenuRepository.FindById(Id);
            return anaMenu;
        }

        public AnaMenu CreateAnaMenu(AnaMenu anaMenu)
        {
            anaMenu.AnaMenuKodu = 1;
            anaMenu.SiraNumarasi = 1;

            if (_UnitOfWork.AnaMenuRepository.Count > 0)
            {
                int maxKod = _UnitOfWork.AnaMenuRepository.All().Select(s => s.AnaMenuKodu).Max();
                anaMenu.AnaMenuKodu = maxKod + 1;
                anaMenu.SiraNumarasi = (short)anaMenu.AnaMenuKodu;
            }

            AnaMenu AnaMenu = _UnitOfWork.AnaMenuRepository.Create(anaMenu);

            List<TVMYetkiGruplari> yetkiGrubu = _TVMContext.TVMYetkiGruplariRepository.All().ToList<TVMYetkiGruplari>();
            //List<TVMYetkiGrupYetkileri> yetkigurupYetkileri = _TVMContext.TVMYetkiGrupYetkileriRepository.All().ToList<TVMYetkiGrupYetkileri>();

            foreach (var item in yetkiGrubu)
            {
                TVMYetkiGrupYetkileri YeniYetki = new TVMYetkiGrupYetkileri();
                YeniYetki.YetkiGrupKodu = item.YetkiGrupKodu;
                YeniYetki.AnaMenuKodu = (short)anaMenu.AnaMenuKodu;
                YeniYetki.AltMenuKodu = 0;
                YeniYetki.SekmeKodu = 0;

                YeniYetki.YeniKayit = 0;
                YeniYetki.Gorme = 0;
                YeniYetki.Degistirme = 0;
                YeniYetki.Silme = 0;

                _TVMContext.TVMYetkiGrupYetkileriRepository.Create(YeniYetki);
            }
            _TVMContext.Commit();
            _UnitOfWork.Commit();
            return AnaMenu;
        }

        public bool UpdateAnaMenu(AnaMenu anaMenu)
        {
            _UnitOfWork.AnaMenuRepository.Update(anaMenu);
            _UnitOfWork.Commit();
            return true;
        }

        public bool CheckAltMenu(int AnaMenuKodu)
        {
            List<AltMenu> altmenuler = _UnitOfWork.AltMenuRepository.Filter(s => s.AnaMenuKodu == AnaMenuKodu).ToList<AltMenu>();
            if (altmenuler.Count == 0)
                return true;
            else
                return false;
        }

        public bool DeleteAnaMenu(int AnaMenuKodu)
        {
            _UnitOfWork.AnaMenuRepository.Delete(m => m.AnaMenuKodu == AnaMenuKodu);

            //Ana Menuye Ait Yetkiler Yetki Tablosundan Siliniyor..
            //List<TVMYetkiGrupYetkileri> yetki = _TVMContext.TVMYetkiGrupYetkileriRepository.Filter(s => s.AnaMenuKodu == AnaMenuKodu).ToList<TVMYetkiGrupYetkileri>();
            //if (yetki.Count > 0)
            _TVMContext.TVMYetkiGrupYetkileriRepository.Delete(s => s.AnaMenuKodu == AnaMenuKodu);
            _TVMContext.Commit();

            _UnitOfWork.Commit();
            return true;
        }

        #endregion

        #region IAltMenuService  Members

        public List<AltMenu> GetAltMenuListYetkili()
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            List<AltMenu> list = new List<AltMenu>();

            //Neosinerji yöneticisine kısıtlama yok ve yetkiye baglı olmadan menü isimlerinde multi-language sağlamak için yapıldı.
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            {
                int lang = 1;
                string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                if (!String.IsNullOrEmpty(language))
                    switch (language)
                    {
                        case "tr": lang = 1; break;
                        case "en": lang = 2; break;
                        case "it": lang = 3; break;
                        case "fr": lang = 4; break;
                        case "es": lang = 5; break;
                    }

                List<DilAciklama> DilAciklamaAltMenuler = _UnitOfWork.DilAciklamaRepository.Filter(s => s.DilId == lang && s.TabloAdi == "AltMenu").ToList();
                IQueryable<AltMenu> altMenuler = _UnitOfWork.AltMenuRepository.All();
                foreach (var item in DilAciklamaAltMenuler)
                {
                    AltMenu altmenu = altMenuler.Where(s => s.AnaMenuKodu == item.TabloId_2 && s.AltMenuKodu == item.TabloId).FirstOrDefault();
                    if (altmenu != null)
                    {
                        altmenu.Aciklama = item.DilAciklama_1;
                        list.Add(altmenu);
                    }
                }

                return list;
            }
            else
            {
                List<KullaniciYetkiModel> yetkiliAltMenuler = _AktifKullanici.Yetkiler.Where(s => s.AnaMenu != 0 && s.MenuKodu != 0 &&
                                                                                                  s.SekmeKodu == 0).ToList<KullaniciYetkiModel>();
                IQueryable<AltMenu> altMenuler = _UnitOfWork.AltMenuRepository.All();

                foreach (var item in yetkiliAltMenuler)
                {
                    AltMenu altmenu = altMenuler.Where(s => s.AnaMenuKodu == item.AnaMenu && s.AltMenuKodu == item.MenuKodu).FirstOrDefault();
                    if (altmenu != null)
                    {
                        altmenu.Aciklama = item.Aciklama;
                        list.Add(altmenu);
                    }
                }
            }

            return list;
        }

        public List<AltMenu> GetAltMenuList()
        {
            return _UnitOfWork.AltMenuRepository.All().ToList<AltMenu>();
        }

        public List<AltMenu> GetAltMenuList(int AnaMenuKodu)
        {
            return _UnitOfWork.AltMenuRepository.Filter(s => s.AnaMenuKodu == AnaMenuKodu).ToList<AltMenu>();
        }

        public AltMenu GetAltMenu(int altMenuKodu, int anaMenuKodu)
        {
            AltMenu altMenu = _UnitOfWork.AltMenuRepository.Filter(s => s.AltMenuKodu == altMenuKodu && s.AnaMenuKodu == anaMenuKodu).SingleOrDefault();
            if (altMenu != null)

                return altMenu;
            else
            {
                altMenu = new AltMenu();
                return altMenu;
            }
        }

        public AltMenu CreateAltMenu(AltMenu altMenu)
        {
            altMenu.SiraNumarasi = 1;
            altMenu.AltMenuKodu = 1;

            //Sıra Numarası veriliyor
            AltMenu sonEklenenAltMenu = _UnitOfWork.AltMenuRepository.Filter(s => s.AnaMenuKodu == altMenu.AnaMenuKodu)
                                                                     .OrderByDescending(s => s.AltMenuKodu).FirstOrDefault();
            if (sonEklenenAltMenu != null)
                altMenu.SiraNumarasi = (short)(sonEklenenAltMenu.SiraNumarasi + 1);

            //Benzersiz bir altmenu kodu verilmesi için son eklenen alt menü getiriliyor
            sonEklenenAltMenu = _UnitOfWork.AltMenuRepository.All().OrderByDescending(s => s.AltMenuKodu).FirstOrDefault();
            if (sonEklenenAltMenu != null)
                altMenu.AltMenuKodu = sonEklenenAltMenu.AltMenuKodu + 1;

            AltMenu AltMn = _UnitOfWork.AltMenuRepository.Create(altMenu);

            List<TVMYetkiGruplari> yetkiGrubu = _TVMContext.TVMYetkiGruplariRepository.All().ToList<TVMYetkiGruplari>();

            foreach (var item in yetkiGrubu)
            {
                TVMYetkiGrupYetkileri YeniYetki = new TVMYetkiGrupYetkileri();
                YeniYetki.YetkiGrupKodu = item.YetkiGrupKodu;
                YeniYetki.AnaMenuKodu = (short)altMenu.AnaMenuKodu;
                YeniYetki.AltMenuKodu = (short)altMenu.AltMenuKodu;
                YeniYetki.SekmeKodu = 0;

                YeniYetki.YeniKayit = 0;
                YeniYetki.Gorme = 0;
                YeniYetki.Degistirme = 0;
                YeniYetki.Silme = 0;

                _TVMContext.TVMYetkiGrupYetkileriRepository.Create(YeniYetki);
            }

            _TVMContext.Commit();
            _UnitOfWork.Commit();
            return AltMn;
        }

        public bool UpdateAltMenu(AltMenu altMenu)
        {
            _UnitOfWork.AltMenuRepository.Update(altMenu);
            _UnitOfWork.Commit();
            return true;
        }

        public bool CheckAltMenuSekme(int altmenekodu)
        {
            List<AltMenuSekme> listAltMenuSekme = _UnitOfWork.AltMenuSekmeRepository.Filter(a => a.AltMenuKodu == altmenekodu).ToList<AltMenuSekme>();
            if (listAltMenuSekme.Count == 0)
                return true;
            else
                return false;
        }

        public bool DeleteAltMenu(int anamenukodu, int altmenukodu)
        {
            //Alt  Menü menü tablosundan siliniyor.
            _UnitOfWork.AltMenuRepository.Delete(s => s.AltMenuKodu == altmenukodu && s.AnaMenuKodu == anamenukodu);

            //Alt Menuye Ait Yetkiler Yetki Tablosundan siliniyor..
            _TVMContext.TVMYetkiGrupYetkileriRepository.Delete(s => s.AltMenuKodu == altmenukodu && s.AnaMenuKodu == anamenukodu);

            _TVMContext.Commit();
            _UnitOfWork.Commit();
            return true;
        }

        #endregion

        #region IAltMenuSekmeService Members

        public List<AltMenuSekme> GetALtMenuSekmeListYetkili()
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            List<AltMenuSekme> list = new List<AltMenuSekme>();

            //Neosinerji yöneticisine kısıtlama yok ve yetkiye baglı olmadan menü isimlerinde multi-language sağlamak için yapıldı.
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            {
                int lang = 1;
                string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                if (!String.IsNullOrEmpty(language))
                    switch (language)
                    {
                        case "tr": lang = 1; break;
                        case "en": lang = 2; break;
                        case "it": lang = 3; break;
                        case "fr": lang = 4; break;
                        case "es": lang = 5; break;
                    }

                List<DilAciklama> DilAciklamaSekmeler = _UnitOfWork.DilAciklamaRepository.Filter(s => s.DilId == lang && s.TabloAdi == "AltMenuSekme").ToList();
                IQueryable<AltMenuSekme> sekmeler = _UnitOfWork.AltMenuSekmeRepository.All();
                foreach (var item in DilAciklamaSekmeler)
                {
                    AltMenuSekme sekme = sekmeler.Where(s => s.SekmeKodu == item.TabloId && s.AnaMenuKodu == item.AnaMenuKodu &&
                                                             s.AltMenuKodu == item.TabloId_2).FirstOrDefault();
                    if (sekme != null)
                    {
                        sekme.Aciklama = item.DilAciklama_1;
                        list.Add(sekme);
                    }
                }




                return list;
            }
            else
            {
                List<KullaniciYetkiModel> yetkiliSekmeler = _AktifKullanici.Yetkiler.Where(s => s.SekmeKodu != 0).ToList<KullaniciYetkiModel>();
                IQueryable<AltMenuSekme> sekmeler = _UnitOfWork.AltMenuSekmeRepository.All();

                foreach (var item in yetkiliSekmeler)
                {
                    AltMenuSekme sekme = sekmeler.Where(s => s.SekmeKodu == item.SekmeKodu && s.AnaMenuKodu == item.AnaMenu &&
                                                             s.AltMenuKodu == item.MenuKodu).FirstOrDefault();
                    if (sekme != null)
                    {
                        sekme.Aciklama = item.Aciklama;
                        list.Add(sekme);
                    }
                }
            }

            return list;
        }

        public List<AltMenuSekme> GetALtMenuSekmeList()
        {
            return _UnitOfWork.AltMenuSekmeRepository.All().ToList<AltMenuSekme>();
        }

        public List<AltMenuSekme> GetALtMenuSekmeList(int altMenuKodu, int anaMenuKodu)
        {
            return _UnitOfWork.AltMenuSekmeRepository.Filter(s => s.AltMenuKodu == altMenuKodu && s.AnaMenuKodu == anaMenuKodu).ToList<AltMenuSekme>();

        }

        public AltMenuSekme GetAltMenuSekme(int altMenuKodu, int sekmeKodu)
        {
            AltMenuSekme altMenuSekme = _UnitOfWork.AltMenuSekmeRepository.Filter(s => s.SekmeKodu == sekmeKodu && s.AltMenuKodu == altMenuKodu).First();

            return altMenuSekme;
        }

        public AltMenuSekme CreateALtMenuSekme(AltMenuSekme altMenuSekme)
        {
            altMenuSekme.SekmeKodu = 1;
            altMenuSekme.SiraNumarasi = 1;
            if (_UnitOfWork.AltMenuSekmeRepository.Count > 0)
            {
                int maxSekme = _UnitOfWork.AltMenuSekmeRepository.All().Select(s => s.SekmeKodu).Max();
                altMenuSekme.SekmeKodu = maxSekme + 1;
            }

            IQueryable<AltMenuSekme> sekmeler = _UnitOfWork.AltMenuSekmeRepository.Filter(s => s.AltMenuKodu == altMenuSekme.AltMenuKodu
                                                                                          && s.AnaMenuKodu == altMenuSekme.AnaMenuKodu);

            if (sekmeler.Count() > 0)
            {
                short maxSiraNo = sekmeler.Select(s => s.SiraNumarasi).Max();
                altMenuSekme.SiraNumarasi = (short)(maxSiraNo + 1);
            }


            AltMenuSekme AltMnSkme = _UnitOfWork.AltMenuSekmeRepository.Create(altMenuSekme);

            List<TVMYetkiGruplari> yetkiGrubu = _TVMContext.TVMYetkiGruplariRepository.All().ToList<TVMYetkiGruplari>();

            foreach (var item in yetkiGrubu)
            {
                TVMYetkiGrupYetkileri YeniYetki = new TVMYetkiGrupYetkileri();
                YeniYetki.YetkiGrupKodu = item.YetkiGrupKodu;
                YeniYetki.AnaMenuKodu = (short)altMenuSekme.AnaMenuKodu;
                YeniYetki.AltMenuKodu = (short)altMenuSekme.AltMenuKodu;
                YeniYetki.SekmeKodu = (short)altMenuSekme.SekmeKodu;

                YeniYetki.YeniKayit = 0;
                YeniYetki.Gorme = 0;
                YeniYetki.Degistirme = 0;
                YeniYetki.Silme = 0;

                _TVMContext.TVMYetkiGrupYetkileriRepository.Create(YeniYetki);
            }

            _TVMContext.Commit();
            _UnitOfWork.Commit();
            return AltMnSkme;
        }

        public bool UpdateAltMenuSekme(AltMenuSekme altMenuSekme)
        {
            _UnitOfWork.AltMenuSekmeRepository.Update(altMenuSekme);
            _UnitOfWork.Commit();
            return true;
        }

        public bool DeleteAltMenuSekme(int sekmeKodu)
        {
            _UnitOfWork.AltMenuSekmeRepository.Delete(a => a.SekmeKodu == sekmeKodu);

            //Sekme yetkileri siliniyor..
            _TVMContext.TVMYetkiGrupYetkileriRepository.Delete(s => s.SekmeKodu == sekmeKodu);
            _TVMContext.Commit();

            _UnitOfWork.Commit();
            return true;
        }

        #endregion

        #region IMenuIslemService Members

        public List<MenuIslem> GetListMenuIslem()
        {
            return _UnitOfWork.MenuIslemRepository.All().ToList<MenuIslem>();

        }
        public MenuIslem GetMenuIslem(int islemKodu)
        {
            MenuIslem menuislem = _UnitOfWork.MenuIslemRepository.FindById(islemKodu);
            return menuislem;
        }
        public MenuIslem CreateMenuIslem(MenuIslem menuIslem)
        {
            MenuIslem menuislem = _UnitOfWork.MenuIslemRepository.Create(menuIslem);
            _UnitOfWork.Commit();
            return menuislem;
        }
        public bool UpdateMenuIslem(MenuIslem menuIslem)
        {
            _UnitOfWork.MenuIslemRepository.Update(menuIslem);
            _UnitOfWork.Commit();
            return true;
        }
        public bool DeleteMenuIslem(int islemKodu)
        {
            _UnitOfWork.MenuIslemRepository.Delete(islemKodu);
            _UnitOfWork.Commit();
            return true;
        }

        #endregion

        public DataTableList PagedListAltMenuList(DataTableParameters<AltMenu> altMenuList, int anaMenuKodu)
        {
            IQueryable<AltMenu> query = _UnitOfWork.AltMenuRepository.Filter(s => s.AnaMenuKodu == anaMenuKodu);

            int totalRowCount = 0;
            query = _UnitOfWork.AltMenuRepository.Page(query,
                                                                  altMenuList.OrderByProperty,
                                                                  altMenuList.IsAscendingOrder,
                                                                  altMenuList.Page,
                                                                  altMenuList.PageSize, out totalRowCount);
            return altMenuList.Prepare(query, totalRowCount);
        }
    }
}
