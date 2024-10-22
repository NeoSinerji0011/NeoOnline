using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business
{
    public class KullaniciService : IKullaniciService
    {
        ITVMContext _TVMContext;
        IEMailService _EMailService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _LogService;
        ITVMService _TVMService;

        public KullaniciService(ITVMContext tvmContext, IEMailService emailService)
        {
            _TVMContext = tvmContext;
            _EMailService = emailService;
        }

        public List<KullaniciListeModel> PagedList(KullaniciArama arama, out int totalRowCount)
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            // ==== Kullanıcı Neosinerji tvm sine bağlıysa sınırlama yok değilse sınırlı yetki ==== //
            IQueryable<TVMDetay> tvm;

            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                tvm = _TVMContext.TVMDetayRepository.All();
            else
            {
                if (_AktifKullanici.MerkezAcente)
                {
                    tvm = _TVMContext.TVMDetayRepository.Filter(s => s.GrupKodu == _AktifKullanici.TVMKodu || s.Kodu == _AktifKullanici.TVMKodu);
                }
                else
                {
                    tvm = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || s.Kodu == _AktifKullanici.TVMKodu);
                }
            }
            IQueryable<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.All();
            IQueryable<TVMDepartmanlar> departmanlar = _TVMContext.TVMDepartmanlarRepository.All();
            IQueryable<TVMYetkiGruplari> yetkiler = _TVMContext.TVMYetkiGruplariRepository.All();

            if (arama.TVMKodu.HasValue)
                tvm = tvm.Where(w => w.Kodu == arama.TVMKodu.Value);

            if (arama.TVMTipi.HasValue)
                tvm = tvm.Where(w => w.Tipi == arama.TVMTipi.Value);

            if (!String.IsNullOrEmpty(arama.Adi))
                kullanicilar = kullanicilar.Where(w => w.Adi.StartsWith(arama.Adi));

            if (!String.IsNullOrEmpty(arama.Soyadi))
                kullanicilar = kullanicilar.Where(w => w.Soyadi.StartsWith(arama.Soyadi));

            if (!String.IsNullOrEmpty(arama.Email))
                kullanicilar = kullanicilar.Where(w => w.Email.StartsWith(arama.Email));

            if (!String.IsNullOrEmpty(arama.TCKN))
                kullanicilar = kullanicilar.Where(w => w.TCKN == arama.TCKN);

            if (arama.Durum.HasValue)
                kullanicilar = kullanicilar.Where(w => w.Durum == arama.Durum.Value);

            if (!String.IsNullOrEmpty(arama.TeknikPersonelKodu))
                kullanicilar = kullanicilar.Where(w => w.TeknikPersonelKodu == arama.TeknikPersonelKodu);

            IQueryable<TVMKullanicilar> query = from k in kullanicilar
                                                join t in tvm on k.TVMKodu equals t.Kodu
                                                join d in departmanlar on k.DepartmanKodu equals d.DepartmanKodu
                                                join y in yetkiler on k.YetkiGrubu equals y.YetkiGrupKodu
                                                where d.TVMKodu == k.TVMKodu
                                                select k;

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in kullanicilar
                        join t in tvm on k.TVMKodu equals t.Kodu
                        join d in departmanlar on k.DepartmanKodu equals d.DepartmanKodu
                        join y in yetkiler on k.YetkiGrubu equals y.YetkiGrupKodu
                        where d.TVMKodu == k.TVMKodu
                        select new
                        {
                            k.KullaniciKodu,
                            k.TCKN,
                            k.Adi,
                            k.Soyadi,
                            k.Email,
                            k.Durum,
                            TVMUnvani = t.Unvani,
                            TVMTipi = t.Tipi,
                            DepartmanAdi = d.Adi,
                            YetkiGrupAdi = y.YetkiGrupAdi,
                            k.Gorevi,
                            k.TeknikPersonelKodu
                        })
                        .OrderBy(o => o.KullaniciKodu)
                        .Skip(excludedRows)
                        .Take(arama.PageSize)
                        .ToList();



            List<KullaniciListeModel> listeModel = new List<KullaniciListeModel>();

            foreach (var item in list)
            {
                KullaniciListeModel model = new KullaniciListeModel();
                model.KullaniciKodu = item.KullaniciKodu;
                model.TCKN = item.TCKN;
                model.Adi = item.Adi;
                model.Soyadi = item.Soyadi;
                model.TVMUnvani = item.TVMUnvani;
                model.TVMTipi = item.TVMTipi;
                model.DepartmanAdi = item.DepartmanAdi;
                model.Email = item.Email;
                model.Durum = item.Durum;
                model.YetkiGrupAdi = item.YetkiGrupAdi;
                model.Gorevi = item.Gorevi;
                model.TeknikPersonelKodu = item.TeknikPersonelKodu;
                listeModel.Add(model);
            }

            return listeModel;
        }

        public List<TVMKullanicilar> GetListKullanicilar()
        {
            return _TVMContext.TVMKullanicilarRepository.All().ToList<TVMKullanicilar>();
        }

        public List<KullaniciModelForList> GetListKullanicilarTeklifAra()
        {
            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            int kullaniciTvmKodu = aktifKullanici.TVMKodu;

            List<TVMKullanicilar> kullanicilar = new List<TVMKullanicilar>();
            kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == kullaniciTvmKodu).ToList<TVMKullanicilar>();
            List<KullaniciModelForList> model = new List<KullaniciModelForList>();

            if (kullanicilar != null)
                foreach (var item in kullanicilar)
                {
                    KullaniciModelForList mdl = new KullaniciModelForList();
                    mdl.AdiSoyadi = item.Adi + " " + item.Soyadi;
                    mdl.KullaniciKodu = item.KullaniciKodu;
                    model.Add(mdl);
                }

            return model;
        }

        public List<KullaniciModelForList> GetTVMKullanicilari(int tvmKodu)
        {
            List<TVMKullanicilar> kullanicilar = new List<TVMKullanicilar>();
            kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvmKodu).ToList<TVMKullanicilar>();
            List<KullaniciModelForList> model = new List<KullaniciModelForList>();

            if (kullanicilar != null)
                foreach (var item in kullanicilar)
                {
                    KullaniciModelForList mdl = new KullaniciModelForList();
                    mdl.AdiSoyadi = item.Adi + " " + item.Soyadi;
                    mdl.KullaniciKodu = item.KullaniciKodu;
                    model.Add(mdl);
                }

            return model;
        }
        public List<KullaniciModelForList> GetListAktifTVMKullanicilari(int tvmKodu)
        {
            List<TVMKullanicilar> kullanicilar = new List<TVMKullanicilar>();
            kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvmKodu &&s.Durum==1).ToList<TVMKullanicilar>();
            List<KullaniciModelForList> model = new List<KullaniciModelForList>();

            if (kullanicilar != null)
                foreach (var item in kullanicilar)
                {
                    KullaniciModelForList mdl = new KullaniciModelForList();
                    mdl.AdiSoyadi = item.Adi + " " + item.Soyadi;
                    mdl.KullaniciKodu = item.KullaniciKodu;
                    model.Add(mdl);
                }

            return model;
        }
        public List<KullaniciModelForList> GetListTVMKullanicilari(int tvmKodu)
        {
            List<TVMKullanicilar> kullanicilar = new List<TVMKullanicilar>();
            kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu== tvmKodu).ToList<TVMKullanicilar>();
            List<KullaniciModelForList> model = new List<KullaniciModelForList>();

            if (kullanicilar != null)
                foreach (var item in kullanicilar)
                {
                    KullaniciModelForList mdl = new KullaniciModelForList();
                    mdl.AdiSoyadi = item.Adi + " " + item.Soyadi;
                    mdl.KullaniciKodu = item.KullaniciKodu;
                    model.Add(mdl);
                }

            return model;
        }

        public List<KullaniciOzetModel> GetKullaniciOzet(int tvmKodu)
        {
            var kullaniciList = _TVMContext.TVMKullanicilarRepository.Filter(f => f.TVMKodu == tvmKodu)
                                                        .Select(m => new { m.KullaniciKodu, m.Adi, m.Soyadi })
                                                        .OrderBy(m => m.KullaniciKodu)
                                                        .ToList();

            List<KullaniciOzetModel> list = new List<KullaniciOzetModel>();
            foreach (var item in kullaniciList)
            {
                list.Add(new KullaniciOzetModel() { KullaniciKodu = item.KullaniciKodu, Adi = item.Adi, Soyadi = item.Soyadi });
            }

            return list;
        }
       
        public TVMKullanicilar GetKullaniciByEmail(string email)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(f => f.Email == email).FirstOrDefault();
        }

        public TVMKullanicilar GetKullaniciByTCKN(string tckn)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(f => f.TCKN == tckn).FirstOrDefault();
        }

        public TVMKullanicilar GetKullaniciByPartajNO(string partaj)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.TeknikPersonelKodu == partaj).FirstOrDefault();
        }

        public TVMKullanicilar GetKullanici(int kullaniciKodu)
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            TVMKullanicilar kullanici = new TVMKullanicilar();
            IQueryable<TVMDetay> YetkiliTVMler = null;
            if (_AktifKullanici != null)
            {
                if (_AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi && _AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                    return _TVMContext.TVMKullanicilarRepository.FindById(kullaniciKodu);
                else
                {
                    kullanici = _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == kullaniciKodu).FirstOrDefault();


                    TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
                    if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
                    {
                        YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == kullanici.TVMKodu ||
                                                                                s.GrupKodu == _AktifKullanici.TVMKodu);
                    }
                    else if (kullanici.TVMKodu == _AktifKullanici.TVMKodu)
                    {
                        YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu ||
                                                                                 s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);
                    }
                    else
                    {
                        YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == kullanici.TVMKodu ||
                                                                                 s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);
                    }
                    int sayac = 0;
                    if (kullanici != null)
                        foreach (var item in YetkiliTVMler)
                            if (item.Kodu == kullanici.TVMKodu)
                                sayac++;

                    if (sayac == 0)
                        return new TVMKullanicilar();
                    else return kullanici;
                }
            }

            return kullanici;
        }

        public TVMKullanicilar GetKullaniciYetkisiz(int kullaniciKodu)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == kullaniciKodu).FirstOrDefault();
        }

        public TVMKullanicilar GetKullaniciPublic(int kullaniciKodu)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == kullaniciKodu).FirstOrDefault();
        }

        public TVMKullanicilar GetMapfreKullanici(string kullaniciKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.ProjeKodu == "Mapfre");
            var kullanicilar = _TVMContext.TVMKullanicilarRepository.All().Where(w => w.TeknikPersonelKodu == kullaniciKodu);

            var kullanici = from t in tvmler
                            join k in kullanicilar on t.Kodu equals k.TVMKodu
                            select k;

            return kullanici.FirstOrDefault();
        }

        public TVMKullanicilar CreateKullanici(TVMKullanicilar kullanici)
        {
            kullanici.KayitTarihi = TurkeyDateTime.Now;
            kullanici.SifreGondermeTarihi = TurkeyDateTime.Now;

            kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.GeciciSifre;
            kullanici.HataliSifreGirisSayisi = 0;

            //Kullaniciya kayıt edildigi anda default fotograf atanıyor..
            kullanici.FotografURL = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/346/avatar.png";

            string sifre = PasswordGenerator.Generate(8);
            kullanici.Sifre = Encryption.HashPassword(sifre);

            TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == kullanici.TVMKodu).FirstOrDefault();
            if (tvm != null)
            {
                if (tvm.ProjeKodu == TVMProjeKodlari.Aegon || tvm.ProjeKodu == TVMProjeKodlari.Mapfre)
                    _EMailService.SendYeniKullaniciEMail_Update(kullanici, sifre, tvm.ProjeKodu);
                else if (tvm.ProjeKodu == TVMProjeKodlari.Lilyum)
                {
                    if (kullanici.EmailOnayKodu != null)
                    {
                        _EMailService.SendLilyumYeniKullaniciEMail(kullanici, sifre);
                    }
                    else
                    {
                        _EMailService.SendYeniKullaniciEMail(kullanici, sifre, tvm.ProjeKodu);
                    }
                }
                else
                {
                    if (kullanici.EmailOnayKodu != null)
                    {
                        _EMailService.SendSigortaliYeniKullaniciEMail(kullanici, sifre);
                    }
                    else
                    {
                        _EMailService.SendYeniKullaniciEMail(kullanici, sifre, tvm.ProjeKodu);
                    }
                }
            }

            kullanici = _TVMContext.TVMKullanicilarRepository.Create(kullanici);

            CreateKullaniciDurum(kullanici, kullanici.Durum);

            _TVMContext.Commit();

            return kullanici;
        }

        public bool CreateSifreTarihcesi(TVMKullaniciSifreTarihcesi sifreTarihi)
        {
            sifreTarihi.SifreDegistirmeNo = 1;
            if (_TVMContext.TVMKullaniciSifreTarihcesiRepository.All().Count() > 0)
            {
                int? maxSiraNo = _TVMContext.TVMKullaniciSifreTarihcesiRepository.All().Select(s => s.SifreDegistirmeNo).Max();

                if (maxSiraNo.HasValue)
                    sifreTarihi.SifreDegistirmeNo = maxSiraNo.Value + 1;
            }
            _TVMContext.TVMKullaniciSifreTarihcesiRepository.Create(sifreTarihi);

            _TVMContext.Commit();

            return true;
        }

        public void UpdateKullanici(TVMKullanicilar kullanici)
        {
            TVMKullanicilar orjinalKullanici = _TVMContext.TVMKullanicilarRepository.FindById(kullanici.KullaniciKodu);

            //Kullanıcı departmanı değişti ise kayıt güncelleniyor
            CreateKullaniciAtama(kullanici, kullanici.DepartmanKodu);

            //Orjinal kullanıcı durumu kaydedilenden farklı ise
            TVMKullaniciDurumTarihcesi durum = kullanici.TVMKullaniciDurumTarihcesis.LastOrDefault();

            if (durum != null)
                if (durum.Durum != kullanici.Durum)
                    durum.BitisTarihi = new DateTime(2049, 12, 31);

            orjinalKullanici.Gorevi = kullanici.Gorevi;
            orjinalKullanici.YetkiGrubu = kullanici.YetkiGrubu;
            orjinalKullanici.Adi = kullanici.Adi;
            orjinalKullanici.Soyadi = kullanici.Soyadi;
            orjinalKullanici.Telefon = kullanici.Telefon;
            orjinalKullanici.CepTelefon = kullanici.CepTelefon;
            orjinalKullanici.Email = kullanici.Email;
            orjinalKullanici.DepartmanKodu = kullanici.DepartmanKodu;
            orjinalKullanici.YoneticiKodu = kullanici.YoneticiKodu;
            orjinalKullanici.MTKodu = kullanici.MTKodu;
            orjinalKullanici.TeklifPoliceUretimi = kullanici.TeklifPoliceUretimi;
            orjinalKullanici.TeknikPersonelKodu = kullanici.TeknikPersonelKodu;
            orjinalKullanici.Durum = kullanici.Durum;
            orjinalKullanici.SkypeNumara = kullanici.SkypeNumara;

            _TVMContext.TVMKullanicilarRepository.Update(orjinalKullanici);
            _TVMContext.Commit();
        }

        public void CreateKullaniciAtama(TVMKullanicilar kullanici, int? yeniDepartmanKodu)
        {
            if (yeniDepartmanKodu != kullanici.DepartmanKodu)
            {
                TVMKullaniciAtama oncekiAtama = kullanici.TVMKullaniciAtamas.LastOrDefault();

                //Önceki atama kaydı
                int atamaNo = 1;
                if (oncekiAtama != null)
                {
                    oncekiAtama.BitisTarihi = TurkeyDateTime.Today;
                    atamaNo = oncekiAtama.AtamaNo + 1;
                }

                //Yeni atama kaydi
                TVMKullaniciAtama atama = new TVMKullaniciAtama();
                atama.TVMKullaniciKodu = kullanici.KullaniciKodu;
                atama.AtamaNo = atamaNo;
                atama.AtandigiDepartmanKodu = yeniDepartmanKodu.HasValue ? yeniDepartmanKodu.Value : 0;
                atama.OncekiDepartmanKodu = kullanici.DepartmanKodu.HasValue ? kullanici.DepartmanKodu.Value : 0;
                atama.BaslamaTarihi = TurkeyDateTime.Today;
                atama.BitisTarihi = new DateTime(2049, 12, 31);
                atama.AtamaTarihi = TurkeyDateTime.Today;

                kullanici.TVMKullaniciAtamas.Add(atama);
            }
        }

        public void CreateKullaniciDurum(TVMKullanicilar kullanici, byte durum)
        {
            TVMKullaniciDurumTarihcesi sonDurum = kullanici.TVMKullaniciDurumTarihcesis.LastOrDefault();

            //Önceki durum
            int siraNo = 1;
            if (sonDurum != null)
            {
                sonDurum.BitisTarihi = TurkeyDateTime.Today;
                siraNo = sonDurum.SiraNo + 1;
            }

            TVMKullaniciDurumTarihcesi yeniDurum = new TVMKullaniciDurumTarihcesi();
            yeniDurum.TVMKullaniciKodu = kullanici.KullaniciKodu;
            yeniDurum.SiraNo = siraNo;
            yeniDurum.Durum = durum;
            yeniDurum.BaslamaTarihi = TurkeyDateTime.Today;
            yeniDurum.BitisTarihi = new DateTime(2049, 12, 31);
            yeniDurum.KayitTarihi = TurkeyDateTime.Today;


            kullanici.TVMKullaniciDurumTarihcesis.Add(yeniDurum);
        }

        public bool RecoverPassword(TVMKullanicilar kullanici)
        {
            bool result = false;

            try
            {
                //TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(s => s.Email == email).FirstOrDefault();

                if (kullanici != null)
                {
                    TVMKullaniciSifremiUnuttum sifreYenileme = new TVMKullaniciSifremiUnuttum();
                    sifreYenileme.EskiSifre = kullanici.Sifre;
                    sifreYenileme.SendDate = TurkeyDateTime.Now;
                    sifreYenileme.Status = KullaniciSifremiUnuttumTipleri.LinkGonderildi;

                    Uri uri = HttpContext.Current.Request.Url;

                    string guid = Guid.NewGuid().ToString();
                    string link = String.Empty;


                    sifreYenileme.PasswordVerificationToken_ = guid;

                    TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == kullanici.TVMKodu).FirstOrDefault();

                    switch (tvm.ProjeKodu)
                    {
                        case TVMProjeKodlari.Aegon:
                            link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], ("Aegon/SifreSifirla/" + guid));
                            result = _EMailService.SendSifreYenileLink_Update(kullanici, link, tvm.ProjeKodu);
                            break;
                        case TVMProjeKodlari.Mapfre:
                            link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], ("Mapfre/SifreSifirla/" + guid));
                            result = _EMailService.SendSifreYenileLink_Update(kullanici, link, tvm.ProjeKodu);
                            break;
                        default:
                            link = String.Format("{0}://{1}/{2}", uri.Scheme, HttpContext.Current.Request.Headers["Host"], ("Account/SifreSifirla/" + guid));
                            _EMailService.SendSifreYenileLink(kullanici, link);
                            break;
                    }


                    kullanici.TVMKullaniciSifremiUnuttums.Add(sifreYenileme);

                    _TVMContext.TVMKullanicilarRepository.Update(kullanici);

                    _TVMContext.Commit();

                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        public bool RecoverPasswordMapfre(TVMKullanicilar kullanici)
        {
            bool result = false;

            try
            {
                if (kullanici != null && !String.IsNullOrEmpty(kullanici.TeknikPersonelKodu))
                {
                    kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.GeciciSifre;
                    kullanici.SifreTarihi = null;
                    kullanici.HataliSifreGirisSayisi = 0;
                    kullanici.HataliSifreGirisTarihi = null;
                    kullanici.Sifre = Encryption.HashPassword(kullanici.TeknikPersonelKodu);
                    _TVMContext.Commit();

                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
                result = false;
            }

            return result;
        }

        public bool ResetPassword(string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                TVMKullaniciSifremiUnuttum sifremiUnuttum = _TVMContext.TVMKullaniciSifremiUnuttumRepository.Filter(s => s.PasswordVerificationToken_ == token)
                                                                                                            .FirstOrDefault();

                if (sifremiUnuttum != null && sifremiUnuttum.Status == KullaniciSifremiUnuttumTipleri.LinkGonderildi)
                {
                    if (sifremiUnuttum.SendDate > TurkeyDateTime.Now.AddMinutes(-20))
                    {
                        TVMKullanicilar kullanici = sifremiUnuttum.TVMKullanicilar;

                        kullanici.SifreGondermeTarihi = TurkeyDateTime.Now;
                        kullanici.SifreDurumKodu = KullaniciSifreDurumTipleri.GeciciSifre;
                        kullanici.SifreTarihi = null;

                        string sifre = PasswordGenerator.Generate(8);
                        kullanici.Sifre = Encryption.HashPassword(sifre);

                        sifremiUnuttum.YeniSifre = kullanici.Sifre;
                        sifremiUnuttum.Status = KullaniciSifremiUnuttumTipleri.SifreResetlendi;
                        sifremiUnuttum.ResetDate = TurkeyDateTime.Now;

                        //Email Gonderiliyor..
                        TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == kullanici.TVMKodu).FirstOrDefault();

                        if (tvm != null)
                        {
                            switch (tvm.ProjeKodu)
                            {
                                case TVMProjeKodlari.Aegon:
                                    _EMailService.SendSifreYenileEMail_Update(kullanici, sifre, tvm.ProjeKodu);
                                    break;
                                case TVMProjeKodlari.Mapfre:
                                    _EMailService.SendSifreYenileEMail_Update(kullanici, sifre, tvm.ProjeKodu);
                                    break;
                                default:
                                    _EMailService.SendSifreYenileEMail(kullanici, sifre);
                                    break;
                            }
                        }


                        _TVMContext.TVMKullanicilarRepository.Update(kullanici);

                        _TVMContext.Commit();

                        return true;
                    }
                }
            }
            return false;
        }

        public bool KullaniciEkleTest(string tckn)
        {
            if (!String.IsNullOrEmpty(tckn))
            {
                TVMKullanicilar kullaniciTckn = GetKullaniciByTCKN(tckn);
                if (kullaniciTckn == null)
                    return true;
                else return false;
            }
            else return false;
        }

        public AcenteKullanicilariModel GetListAcenteKullanicilari()
        {
            AcenteKullanicilariModel model = new AcenteKullanicilariModel();
            model.list = new List<KullaniciSkypeModel>();

            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            if (aktifKullanici != null)
            {
                List<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == aktifKullanici.TVMKodu &&
                                                                                                       s.KullaniciKodu != aktifKullanici.KullaniciKodu &&
                                                                                                       s.SkypeNumara != null).ToList<TVMKullanicilar>();

                foreach (var item in kullanicilar)
                {
                    KullaniciSkypeModel mdl = new KullaniciSkypeModel();

                    mdl.Adi = item.Adi;
                    mdl.Soyadi = item.Soyadi;
                    mdl.AdiSoyadi = item.Adi + " " + item.Soyadi;
                    mdl.FotoURL = item.FotografURL;
                    mdl.SkypeNumarasi = item.SkypeNumara;
                    mdl.KullaniciKodu = item.KullaniciKodu;
                    mdl.Telefon = item.CepTelefon;
                    TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == item.TVMKodu).FirstOrDefault();
                    if (tvm != null)
                        mdl.TVMUnvani = tvm.Unvani;
                    TVMYetkiGruplari yetki = _TVMContext.TVMYetkiGruplariRepository.Filter(s => s.YetkiGrupKodu == item.YetkiGrubu).FirstOrDefault();
                    if (yetki != null)
                        mdl.Yetki = yetki.YetkiGrupAdi;

                    model.list.Add(mdl);
                }
            }

            return model;
        }

        public bool KullaniciYetkiKontrolu(int id)
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            if (id > 0 && _AktifKullanici != null)
            {
                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    return true;

                TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.FindById(id);
                int bagliolduguTvmKodu = _AktifKullanici.TVMKodu;
                TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
                if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
                {
                    bagliolduguTvmKodu = tvmDetay.BagliOlduguTVMKodu;
                }


                IQueryable<TVMDetay> YetkiliTVMler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu ||
                                                                                                s.BagliOlduguTVMKodu == bagliolduguTvmKodu);

                int sayac = 0;

                foreach (var item in YetkiliTVMler)
                    if (item.Kodu == kullanici.TVMKodu)
                        sayac++;

                if (sayac == 0)
                    return false;
                else return true;
            }

            return false;
        }

        //Mapfre Kullanıcı  
        public MapfreKullanici CreateMapfreKullanici(MapfreKullanici MapfreKullanici)
        {
            MapfreKullanici = _TVMContext.MapfreKullaniciRepository.Create(MapfreKullanici);
            _TVMContext.Commit();
            return MapfreKullanici;
        }

        public List<MapfreKullaniciListeModel> MapfrePagedList(MapfreKullaniciArama arama, out int totalRowCount)
        {
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            IQueryable<MapfreKullanici> kullanicilar = _TVMContext.MapfreKullaniciRepository.All();

            if (!String.IsNullOrEmpty(arama.KullaniciAdi))
                kullanicilar = kullanicilar.Where(w => w.KullaniciAdi == arama.KullaniciAdi);

            if (!String.IsNullOrEmpty(arama.Partaj))
                kullanicilar = kullanicilar.Where(w => w.AnaPartaj == arama.Partaj);


            IQueryable<TVMBolgeleri> bolgeleri = _TVMContext.TVMBolgeleriRepository.Filter(s => s.TVMKodu == NeosinerjiTVM.MapfreeTVMKodu);

            IQueryable<MapfreKullanici> query = from k in kullanicilar
                                                select k;

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            query = query.OrderBy(o => o.TVMUnvan).Skip(excludedRows).Take(arama.PageSize);

            List<MapfreKullaniciListeModel> list = new List<MapfreKullaniciListeModel>();

            foreach (var item in query)
            {
                MapfreKullaniciListeModel mdl = new MapfreKullaniciListeModel();

                int bolgeKodu;

                if (int.TryParse(item.Bolge, out bolgeKodu))
                {
                    TVMBolgeleri bolge = bolgeleri.Where(s => s.TVMBolgeKodu == bolgeKodu).FirstOrDefault();
                    mdl.Bolge = bolge.BolgeAdi;
                }
                mdl.KullaniciId = item.MapfeKullaniciId;
                mdl.TVMUnvan = item.TVMUnvan;
                mdl.AnaPartaj = item.AnaPartaj;
                mdl.TaliPartaj = item.TaliPartaj;
                mdl.KullaniciAdi = item.KullaniciAdi;
                mdl.Email = item.EMail;
                mdl.Olusturuldu = item.Olusturuldu.HasValue ? item.Olusturuldu.Value : false;
                mdl.OlusturulduText = "";

                list.Add(mdl);
            }

            return list;
        }

        public TVMKullanicilar GetMapfreKullaniciByKullaniciAdi(string KullaniciAdi)
        {
            IQueryable<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.All();
            IQueryable<TVMDetay> tvmler = _TVMContext.TVMDetayRepository.All();

            var kullanici = (from k in kullanicilar
                             join t in tvmler on k.TVMKodu equals t.Kodu
                             where t.BagliOlduguTVMKodu == NeosinerjiTVM.MapfreeTVMKodu && k.TeknikPersonelKodu == KullaniciAdi
                             select k).FirstOrDefault();

            return kullanici;
        }

        public bool MapfreKullaniciEkleTest(string kullaniciAdi)
        {
            if (!String.IsNullOrEmpty(kullaniciAdi))
            {
                TVMKullanicilar Kullanici = GetMapfreKullaniciByKullaniciAdi(kullaniciAdi);
                if (Kullanici == null)
                    return true;
                else return false;
            }
            else return false;
        }

        public void MapfreKulaniciOlustur(int mapfeKullaniciId, string sifre)
        {
            _TVMContext.spMapfreKullanici(mapfeKullaniciId, sifre);
        }

        public string KullaniciSifreSuresiKontrol(int aktifTvmKodu, int aktifKullaniciKodu)
        {
            string message = String.Empty;
            try
            {
                TVMKullaniciSifreTarihcesi sifreTarihcesi = new TVMKullaniciSifreTarihcesi();
                sifreTarihcesi = _TVMContext.TVMKullaniciSifreTarihcesiRepository.All().Where(s => s.TVMKodu == aktifTvmKodu && s.TVMKullaniciKodu == aktifKullaniciKodu).OrderByDescending(s => s.DegistirmeTarihi).FirstOrDefault();
                if (sifreTarihcesi != null)
                {
                    string systemDate = string.Format("{0:dd/MM/yyyy hh:mm:ss}", TurkeyDateTime.Today);
                    string kullaniciSifreTarihi = string.Format("{0:dd/MM/yyyy hh:mm:ss}", sifreTarihcesi.DegistirmeTarihi.AddDays(90));

                    TimeSpan ts = (Convert.ToDateTime(kullaniciSifreTarihi) - Convert.ToDateTime(systemDate));

                    if (ts.Days <= 0)
                    {
                        message = "Şifrenizin geçerlilik süresi dolmuştur. Güvenliğiniz için şifrenizi değiştiriniz.";
                    }
                    else if (ts.Days <= 10 && ts.Days > 0)
                    {
                        message = "Şifrenizin geçerlilik süresinin dolmasına " + ts.Days + " gün kalmıştır. Güvenliğiniz için şifrenizi bu süre içerisinde değiştiriniz.";
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                return message;
            }
        }

        public TVMKullanicilar GetKullaniciEmailKontorl(string EmailKontrolKodu)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(s => s.EmailOnayKodu == EmailKontrolKodu).FirstOrDefault();
        }
        public TVMKullanicilar GetKullaniciByTCKNEmail(string tckn, string email)
        {
            return _TVMContext.TVMKullanicilarRepository.Filter(f => f.TCKN == tckn || f.Email==email).FirstOrDefault();
        }
        public bool KullaniciVarmi(string tckn, string email)
        {
            if (!(String.IsNullOrEmpty(tckn) && String.IsNullOrEmpty(email)))
            {
                TVMKullanicilar kullanici = GetKullaniciByTCKNEmail(tckn, email);
                if (kullanici == null)
                    return false;
                else return true;
            }
            else return false;
        }
    }
}
