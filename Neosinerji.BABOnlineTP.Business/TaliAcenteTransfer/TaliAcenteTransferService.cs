using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{

    public class TaliAcenteTransferService : ITaliAcenteTransferService
    {
        ITVMContext _TVMContext;
        IParametreContext _UnitOfWork;
        IAktifKullaniciService _AktifKullanici;

        public TaliAcenteTransferService(ITVMContext tvmContext, IParametreContext unitOfWork, IAktifKullaniciService aktifKullanici)
        {
            _TVMContext = tvmContext;
            _UnitOfWork = unitOfWork;
            _AktifKullanici = aktifKullanici;
        }

        public TVMDetay GetDetay(int kodu)
        {
            return _TVMContext.TVMDetayRepository.FindById(kodu);

        }

        public List<TVMUrunYetkileri> GetListTVMUrunYetkileri(int id)
        {
            List<TVMUrunYetkileri> list = new List<TVMUrunYetkileri>();

            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            {
                list = _TVMContext.TVMUrunYetkileriRepository.Filter(m => m.TVMKodu == id).ToList<TVMUrunYetkileri>();
            }
            else
            {
                list = _TVMContext.TVMUrunYetkileriRepository.Filter(m => m.TVMKodu == id && m.Teklif == 1).ToList<TVMUrunYetkileri>();
            }
            return list;
        }

        public List<TVMWebServisKullanicilari> GetListTVMWebServisKullanicilari(int tvmKodu)
        {
            return _TVMContext.TVMWebServisKullanicilariRepository.Filter(f => f.TVMKodu == tvmKodu).ToList();
        }

        public List<TVMBolgeleri> GetListBolgeler(int tvmKodu)
        {
            return _TVMContext.TVMBolgeleriRepository.Filter(s => s.TVMKodu == tvmKodu & s.Durum == 1).ToList<TVMBolgeleri>();
        }

        public List<TVMDepartmanlar> GetListDepartmanlar(int tvmKodu)
        {
            return _TVMContext.TVMDepartmanlarRepository.Filter(s => s.TVMKodu == tvmKodu)
                                                        .ToList<TVMDepartmanlar>();
        }

        public List<TVMKullanicilar> GetListKullanicilar(int tvmKodu)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvmKodu).ToList<TVMKullanicilar>();
        }

        public List<TVMYetkiGruplari> GetListTVYetkiGruplari(int id)
        {
            List<TVMYetkiGruplari> list = new List<TVMYetkiGruplari>();

            list = _TVMContext.TVMYetkiGruplariRepository.Filter(m => m.TVMKodu == id && m.YetkiGrupAdi == "PERSONEL").ToList<TVMYetkiGruplari>();

            return list;
        }


        public TVMYetkiGruplari GetTVYetkiGruplari(int id)
        {
            TVMYetkiGruplari list = new TVMYetkiGruplari();

            list = _TVMContext.TVMYetkiGruplariRepository.Find(m => m.TVMKodu == id);

            return list;
        }

        public TVMYetkiGruplari GetTVYetkiGrup(int yetkiKodu)
        {
            TVMYetkiGruplari list = new TVMYetkiGruplari();

            list = _TVMContext.TVMYetkiGruplariRepository.Find(m => m.YetkiGrupKodu == yetkiKodu);

            return list;
        }

        public List<TVMYetkiGrupYetkileri> GetListTVYetkiGrupYetkileri(List<TVMYetkiGruplari> yetkiGrupKodu)
        {
            List<TVMYetkiGrupYetkileri> list = _TVMContext.TVMYetkiGrupYetkileriRepository.All().ToList<TVMYetkiGrupYetkileri>();
            var yetkiGrupkodlari = yetkiGrupKodu.Select(y => y.YetkiGrupKodu).ToArray();
            list = list.Where(m => yetkiGrupkodlari.Contains(m.YetkiGrupKodu)).OrderBy(k => k.YetkiGrupKodu).ToList();
            return list;
        }

        public List<TVMYetkiGrupYetkileri> GetListTVYetkiGrupYetkileri(int yetkiGrupKodu)
        {
            List<TVMYetkiGrupYetkileri> list = _TVMContext.TVMYetkiGrupYetkileriRepository.All().Where(s => s.YetkiGrupKodu == yetkiGrupKodu).ToList<TVMYetkiGrupYetkileri>();

            return list;
        }

        public TVMDetay GetSonTvm(int tvmKodu)
        {
            List<TVMDetay> tvmd = new List<TVMDetay>();
            return _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).OrderByDescending(s => s.Kodu).FirstOrDefault();

        }

        public int GetSonTvmKodu(int tvmKodu)
        {
            
            var Talivarmi= _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == tvmKodu).OrderByDescending(s => s.Kodu).FirstOrDefault();
            int yeniTvmKodu=0;
            if (Talivarmi==null)
            {
                yeniTvmKodu =Convert.ToInt32(tvmKodu.ToString() + "0000");
            }
            else
            {
                yeniTvmKodu = Talivarmi.Kodu;
            }
            return yeniTvmKodu;
        }

        public List<TaliAcenteExcel> getTaliler(string path, int tvmKodu)
        {
            List<TaliAcenteExcel> taliAcenteler = new List<TaliAcenteExcel>();
            TaliReader read = new TaliReader(path, tvmKodu);
            taliAcenteler = read.getTaliler();

            return taliAcenteler;
        }

        private int eklenenKayit = 0;
        private int varolanKayit = 0;

        private List<TVMYetkiGruplari> tvmYetkiGrup = null;

        public void Add(List<TaliAcente> taliler)
        {
            foreach (TaliAcente item in taliler)
            {
                int result = this.AddTali(item);
                if (result == 1)
                    eklenenKayit++;
                else if (result == 2)
                    varolanKayit++;
            }
        }

        private int AddTali(TaliAcente tali)
        {
            try
            {
                bool kayitVarMi = this.KayitVarMi(tali.TVMDetay.Email);
                if (!kayitVarMi)
                {
                    _TVMContext.TVMDetayRepository.Create(tali.TVMDetay);
                    _TVMContext.Commit();

                    return 1;
                }
                else return 2;


            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void taliYetkiAdd(List<TVMYetkiGrupYetkileri> taliYetkileri)
        {
            foreach (var item in taliYetkileri)
            {
                _TVMContext.TVMYetkiGrupYetkileriRepository.Create(item);
                _TVMContext.Commit();
            }

        }

        public void taliWebServisKullaniciAdd(List<TVMWebServisKullanicilari> taliWebServis)
        {
            foreach (var item in taliWebServis)
            {
                _TVMContext.TVMWebServisKullanicilariRepository.Create(item);
                _TVMContext.Commit();
            }
        }

        public void taliDepartmanlarAdd(List<TVMDepartmanlar> taliDepartmanlar)
        {
            foreach (var item in taliDepartmanlar)
            {
                _TVMContext.TVMDepartmanlarRepository.Create(item);
                _TVMContext.Commit();
            }
        }

        public void taliBolgelerAdd(List<TVMBolgeleri> taliBolgeler)
        {
            foreach (var item in taliBolgeler)
            {
                _TVMContext.TVMBolgeleriRepository.Create(item);
                _TVMContext.Commit();
            }
        }

        public void taliKullaniciAdd(TVMKullanicilar taliKullanici)
        {

            _TVMContext.TVMKullanicilarRepository.Create(taliKullanici);
            _TVMContext.Commit();

        }

        public int getBasariliKayitlar()
        {
            return this.eklenenKayit;
        }

        public int getBasarisizKayitlar()
        {
            return this.varolanKayit;
        }

        private bool KayitVarMi(string email)
        {
            TVMDetay kayit = _TVMContext.TVMDetayRepository.Find(s => s.Email == email);
            if (kayit != null) return true;
            else return false;
        }

        public bool TVMKoduVarMi(int tvmKodu)
        {
            TVMDetay kayit = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (kayit != null) return true;
            else return false;
        }

        public bool tckVarMi(string tc)
        {
           TVMKullanicilar kul = new TVMKullanicilar();

          kul=  _TVMContext.TVMKullanicilarRepository.Find(s => s.TCKN == tc);

          if (kul!=null)
          {
              return false;
          }
          return true;
        }
    }
}
