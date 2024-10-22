using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class DuyuruService : IDuyuruService
    {
        ITVMContext _TVMContext;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;


        public DuyuruService(ITVMContext tvmContext, IAktifKullaniciService aktifKullanici)
        {
            _TVMContext = tvmContext;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        #region Duyurular Members

        public Duyurular CreateDuyuru(Duyurular duyuru, string[] tvmList, DuyuruDokuman dokuman)
        {
            try
            {
                if (_AktifKullanici != null && tvmList != null && duyuru != null)
                {
                    duyuru.EkleyenKullanici = _AktifKullanici.KullaniciKodu;
                    duyuru.EklemeTarihi = TurkeyDateTime.Now.Turkey();
                    List<int> AllowAddTVM = new List<int>();
                    List<TVMDetay> yetkiliTVMList = new List<TVMDetay>();


                    if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                        yetkiliTVMList = _TVMContext.TVMDetayRepository.All().ToList<TVMDetay>();
                    else
                        yetkiliTVMList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu ||
                                                                                    s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu).ToList<TVMDetay>();




                    List<int> tvmListInt = new List<int>();

                    foreach (var item in tvmList)
                    {
                        if (item != "multiselect-all")
                        {
                            int kodu;
                            if (int.TryParse(item, out kodu))
                                tvmListInt.Add(kodu);
                        }
                    }


                    foreach (var item in yetkiliTVMList)
                        if (tvmListInt.Contains(item.Kodu))
                            AllowAddTVM.Add(item.Kodu);


                    //Duyuru TVM talosuna ekleniyor..
                    if (AllowAddTVM != null)
                    {
                        foreach (var item in AllowAddTVM)
                        {
                            DuyuruTVM tvmduyuru = new DuyuruTVM();

                            tvmduyuru.TVMId = item;
                            tvmduyuru.EklemeTarihi = TurkeyDateTime.Now.Turkey();
                            tvmduyuru.EkleyenKullanici = _AktifKullanici.KullaniciKodu;

                            duyuru.DuyuruTVMs.Add(tvmduyuru);
                        }
                    }

                    //Duyuruya varsa dokuman ekleniyor..
                    if (dokuman != null)
                    {
                        dokuman.EkleyenKullanici = _AktifKullanici.KullaniciKodu;
                        dokuman.KayitTarihi = TurkeyDateTime.Now.Turkey();
                        duyuru.DuyuruDokumen.Add(dokuman);
                    }

                    //Duyuru Kaydediliyor..
                    duyuru = _TVMContext.DuyurularRepository.Create(duyuru);
                    _TVMContext.Commit();
                }

                return duyuru;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return duyuru;
            }
        }

        public Duyurular GetDuyuru(int duyuruId)
        {
            Duyurular duyuru = new Duyurular();

            if (_AktifKullanici != null)
            {
                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    duyuru = _TVMContext.DuyurularRepository.Find(s => s.DuyuruId == duyuruId);
                else
                {
                    List<Duyurular> yetkiliDuyurular = GetAllDuyuru();
                    duyuru = yetkiliDuyurular.Where(s => s.DuyuruId == duyuruId).FirstOrDefault();
                }
            }

            return duyuru;
        }

        public Duyurular GetDuyuruAnaSayfa(int duyuruId)
        {
            Duyurular duyuru = new Duyurular();

            List<DuyuruProcedureModel> duyurular = GetListDuyuruByTvmId(_AktifKullanici.TVMKodu);

            DuyuruProcedureModel duyuruPro = duyurular.Where(s => s.DuyuruId == duyuruId).FirstOrDefault();

            if (duyuruPro != null)
            {
                duyuru = _TVMContext.DuyurularRepository.Filter(s => s.DuyuruId == duyuruPro.DuyuruId).FirstOrDefault();
            }

            return duyuru;
        }

        public List<Duyurular> GetAllDuyuru()
        {
            List<Duyurular> duyurular = new List<Duyurular>();

            if (_AktifKullanici != null)
            {
                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    return _TVMContext.DuyurularRepository.All().ToList<Duyurular>();
                else
                {
                    IQueryable<Duyurular> AllDuyuru = _TVMContext.DuyurularRepository.All();
                    IQueryable<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == _AktifKullanici.TVMKodu &&
                                                                                                                 s.YetkiGrubu == _AktifKullanici.YetkiGrubu);

                    var list = from d in AllDuyuru
                               join k in kullanicilar on d.EkleyenKullanici equals k.KullaniciKodu
                               select d;

                    duyurular = list.ToList<Duyurular>();
                }
            }

            return duyurular;
        }

        public List<string> GetEkliTVMName(int duyuruId)
        {
            List<string> tvmler = new List<string>();

            IQueryable<DuyuruTVM> duyuruTVM = _TVMContext.DuyuruTVMRepository.Filter(s => s.DuyuruId == duyuruId);
            IQueryable<TVMDetay> TvmDetay = _TVMContext.TVMDetayRepository.All();

            var list = from d in duyuruTVM
                       join t in TvmDetay on d.TVMId equals t.Kodu
                       select new
                       {
                           t.Unvani
                       };

            foreach (var item in list)
            {
                tvmler.Add(item.Unvani);
            }
            return tvmler;

        }

        public bool UpdateDuyuru(Duyurular duyuru, string[] tvmList)
        {
            if (_AktifKullanici != null && tvmList.Length > 0)
            {
                List<DuyuruTVM> tvmlistesi = _TVMContext.DuyuruTVMRepository.Filter(s => s.DuyuruId == duyuru.DuyuruId).ToList<DuyuruTVM>();

                //Eski Tvmlistesi Siliniyor
                if (tvmlistesi != null && tvmlistesi.Count > 0)
                    foreach (var item in tvmlistesi)
                        _TVMContext.DuyuruTVMRepository.Delete(item.DuyuruTvmId);


                //Yeni tvm Listesi Ekleniyor...
                List<int> tvmListInt = new List<int>();

                foreach (var item in tvmList)
                    if (item != "multiselect-all")
                    {
                        int kodu;
                        if (int.TryParse(item, out kodu))
                            tvmListInt.Add(kodu);
                    }


                foreach (var item in tvmListInt)
                {
                    DuyuruTVM tvmduyuru = new DuyuruTVM();

                    tvmduyuru.DuyuruId = duyuru.DuyuruId;
                    tvmduyuru.TVMId = item;
                    tvmduyuru.EklemeTarihi = TurkeyDateTime.Now.Turkey();
                    tvmduyuru.EkleyenKullanici = _AktifKullanici.KullaniciKodu;

                    _TVMContext.DuyuruTVMRepository.Create(tvmduyuru);
                }

                duyuru.DegistirenKullanici = _AktifKullanici.KullaniciKodu;
                duyuru.DegistirmeTarihi = TurkeyDateTime.Now.Turkey();

                _TVMContext.DuyurularRepository.Update(duyuru);
                _TVMContext.Commit();

                return true;
            }
            return false;
        }

        public List<DuyuruProcedureModel> GetListDuyuruByTvmId(int tvmKodu)
        {
            List<DuyuruProcedureModel> model = new List<DuyuruProcedureModel>();
            model = _TVMContext.GetDuyuru(tvmKodu);
            return model;
        }

        public bool DeleteDuyuru(int DuyuruId)
        {
            _TVMContext.DuyuruTVMRepository.Delete(s => s.DuyuruId == DuyuruId);
            _TVMContext.DuyurularRepository.Delete(s => s.DuyuruId == DuyuruId);
            _TVMContext.Commit();
            return true;
        }

        #endregion

        #region DuyuruTVM Members
        public int[] GetAddedTVMList(int duyuruId)
        {
            List<DuyuruTVM> list = _TVMContext.DuyuruTVMRepository.Filter(s => s.DuyuruId == duyuruId).ToList<DuyuruTVM>();
            int[] dizi = new int[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                dizi[i] = list[i].TVMId;
            }

            return dizi;
        }
        #endregion

        #region DuyuruDokuman Members
        public int GetDuyuruIdByDokumanId(int duyuruDokumanId)
        {
            int duyuruId = _TVMContext.DuyuruDokumanRepository.Find(s => s.DuyuruDokumanId == duyuruDokumanId).DuyuruId;
            return duyuruId;
        }
        public List<DuyuruDokuman> GetDokumanByDuyuruId(int duyuruId)
        {
            return _TVMContext.DuyuruDokumanRepository.Filter(s => s.DuyuruId == duyuruId).ToList<DuyuruDokuman>();
        }
        public bool DeleteDokuman(int duyuruDokumanId)
        {
            _TVMContext.DuyuruDokumanRepository.Delete(s => s.DuyuruDokumanId == duyuruDokumanId);
            _TVMContext.Commit();
            return true;
        }
        #endregion
    }
}
