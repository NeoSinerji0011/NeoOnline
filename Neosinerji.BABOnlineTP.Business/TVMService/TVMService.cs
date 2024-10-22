using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TVMService : ITVMService
    {
        ITVMContext _TVMContext;
        IParametreContext _UnitOfWork;
        IAktifKullaniciService _AktifKullanici;
        IKomisyonContext _KomisyonContext;
        IPoliceContext _PoliceContext;


        public TVMService(ITVMContext tvmContext, IParametreContext unitOfWork, IAktifKullaniciService aktifKullanici, IKomisyonContext komisyonContext, IPoliceContext policeContext)
        {
            _TVMContext = tvmContext;
            _UnitOfWork = unitOfWork;
            _AktifKullanici = aktifKullanici;
            _KomisyonContext = komisyonContext;
            _PoliceContext = policeContext;

        }

        #region TVMService

        public DataTableList PagedList(TVMListe tvmListe)
        {
            IQueryable<TVMDetay> query;

            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                query = _TVMContext.TVMDetayRepository.All();
            else
            {
                query = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu || s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);
            }

            if (tvmListe.Kodu.HasValue)
                query = query.Where(w => w.Kodu == tvmListe.Kodu.Value);

            if (tvmListe.Tipi.HasValue)
                query = query.Where(w => w.Tipi == tvmListe.Tipi.Value);

            if (!String.IsNullOrEmpty(tvmListe.Unvani))
                query = query.Where(w => w.Unvani.StartsWith(tvmListe.Unvani));

            if (tvmListe.BagliOlduguTVMKodu.HasValue)
                query = query.Where(w => w.BagliOlduguTVMKodu == tvmListe.BagliOlduguTVMKodu.Value);

            int totalRowCount = 0;
            query = _TVMContext.TVMDetayRepository.Page(query, tvmListe.OrderByProperty, tvmListe.IsAscendingOrder, tvmListe.Page, tvmListe.PageSize, out totalRowCount);

            return tvmListe.Prepare(query, totalRowCount);
        }

        public List<TVMDetay> GetListTVMDetay()
        {
            return _TVMContext.TVMDetayRepository.All().ToList<TVMDetay>();
        }

        public List<TVMDetay> GetListTVMDetayYetkili()
        {
            List<TVMDetay> tvmList = new List<TVMDetay>();

            if (_AktifKullanici != null)
            {
                int aktifTvmKodu = _AktifKullanici.TVMKodu;

                if (aktifTvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    tvmList = _TVMContext.TVMDetayRepository.All().ToList<TVMDetay>();
                else
                {
                    if (_AktifKullanici.MerkezAcente == true)
                    {
                        tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.GrupKodu == aktifTvmKodu).OrderBy(s => s.Unvani).ToList<TVMDetay>();

                    }
                    else
                    {
                        tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == aktifTvmKodu || s.BagliOlduguTVMKodu == aktifTvmKodu)
                                                                .OrderBy(s => s.Unvani).ToList<TVMDetay>();
                    }
                }
            }

            return tvmList;
        }
        public List<TVMDetay> GetListBolgeYetkilisi()
        {
            List<TVMDetay> tvmList = new List<TVMDetay>();

            if (_AktifKullanici != null)
            {
                int aktifTvmKodu = _AktifKullanici.TVMKodu;

                if (aktifTvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                    tvmList = _TVMContext.TVMDetayRepository.All().ToList<TVMDetay>();
                else
                {
                    if (!_AktifKullanici.MerkezAcente)
                    {
                        tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == aktifTvmKodu && s.BolgeYetkilisiMi == 1).OrderBy(s => s.Unvani).ToList<TVMDetay>();

                    }
                    else
                    {
                        tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == aktifTvmKodu && s.BolgeYetkilisiMi == 1)
                                                                .OrderBy(s => s.Unvani).ToList<TVMDetay>();
                    }
                }
            }

            return tvmList;
        }

        public List<SelectListItem> GetListTVMDetayForOfflinePolice()
        {
            IQueryable<TVMDetay> tvmlist = _TVMContext.TVMDetayRepository.All();
            List<int> offlinePoliceler = _UnitOfWork.OfflinePolouseRepository.All().Select(s => s.TVMKodu).Distinct().ToList<int>();

            var listquery = from tvm in tvmlist
                            where offlinePoliceler.Contains(tvm.Kodu)
                            select tvm;

            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in listquery)
            {
                list.Add(new SelectListItem() { Value = item.Kodu.ToString(), Text = item.Unvani });
            }

            return list;
        }

        public List<TVMOzetModel> GetTVMListeKullaniciYetki(int? bagliOlduguTVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi != TVMTipleri.UzerindenIsYapilanACente);
            if (bagliOlduguTVMKodu.HasValue && bagliOlduguTVMKodu > 0)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == bagliOlduguTVMKodu);
            if (!bagliOlduguTVMKodu.HasValue)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999);

            if (!(_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                  _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi))
            {
                if (_AktifKullanici.MerkezAcente)
                {
                    tvmler = tvmler.Where(w => (w.GrupKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
                else
                {
                    tvmler = tvmler.Where(w => (w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
            }
            else
            {
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999 && (w.Tipi == TVMTipleri.Acente || w.Tipi == TVMTipleri.Banka || w.Tipi == TVMTipleri.Broker));
            }
            return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                         .OrderBy(m => m.Unvani)
                         .ToList();
        }

        public List<int> GetTVMListe(int? bagliOlduguTVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi != TVMTipleri.UzerindenIsYapilanACente);
            if (bagliOlduguTVMKodu.HasValue && bagliOlduguTVMKodu > 0)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == bagliOlduguTVMKodu);
            if (!bagliOlduguTVMKodu.HasValue)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999);

            if (!(_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                  _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi))
            {
                if (_AktifKullanici.MerkezAcente)
                {
                    tvmler = tvmler.Where(w => (w.GrupKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
                else
                {
                    tvmler = tvmler.Where(w => (w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
            }
            else
            {
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999 && (w.Tipi == TVMTipleri.Acente || w.Tipi == TVMTipleri.Banka || w.Tipi == TVMTipleri.Broker));
            }
            return tvmler.Select(s => s.Kodu).ToList();
        }

        public List<TVMOzetModel> GetTVMListeNeosinerji(int? bagliOlduguTVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi != TVMTipleri.UzerindenIsYapilanACente);
            if (bagliOlduguTVMKodu.HasValue && bagliOlduguTVMKodu > 0)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == bagliOlduguTVMKodu);
            if (!bagliOlduguTVMKodu.HasValue)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999);

            if (!(_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                  _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi))
            {
                if (_AktifKullanici.MerkezAcente)
                {
                    tvmler = tvmler.Where(w => (w.GrupKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
                else
                {
                    tvmler = tvmler.Where(w => (w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
            }
            else
            {
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu && (w.Tipi == TVMTipleri.Acente || w.Tipi == TVMTipleri.Banka || w.Tipi == TVMTipleri.Broker));
            }
            return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                         .OrderBy(m => m.Unvani)
                         .ToList();
        }

        public List<TVMOzetModel> GetNeosinerjiTVMTUMListesi(int? bagliOlduguTVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi != TVMTipleri.UzerindenIsYapilanACente);
            if (bagliOlduguTVMKodu.HasValue && bagliOlduguTVMKodu > 0)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == bagliOlduguTVMKodu);
            if (!bagliOlduguTVMKodu.HasValue)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999);

            if (!(_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                  _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi))
            {
                if (_AktifKullanici.MerkezAcente == true)
                {
                    tvmler = tvmler.Where(w => (w.GrupKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
                else
                {
                    tvmler = tvmler.Where(w => (w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu));
                }
            }
            return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                         .OrderBy(m => m.Unvani)
                         .ToList();
        }

        public List<TVMOzetModel> GetDisUretimTVMListeKullaniciYetki(int? bagliOlduguTVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi == TVMTipleri.UzerindenIsYapilanACente);
            if (bagliOlduguTVMKodu.HasValue && bagliOlduguTVMKodu > 0)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == bagliOlduguTVMKodu);
            if (!bagliOlduguTVMKodu.HasValue)
                tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == -9999);

            if (!(_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                  _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi))
            {
                if (_AktifKullanici.MerkezAcente == true)
                {
                    tvmler = tvmler.Where(w => w.GrupKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu);
                }
                else
                {
                    var tDetay = this.GetDetay(_AktifKullanici.TVMKodu);
                    if (tDetay.BagliOlduguTVMKodu == -9999)
                    {
                        tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu || w.Kodu == _AktifKullanici.TVMKodu);
                    }
                    else
                    {
                        tvmler = tvmler.Where(w => w.BagliOlduguTVMKodu == tDetay.BagliOlduguTVMKodu);
                    }
                }
            }

            return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                         .OrderBy(m => m.Unvani)
                         .ToList();
        }
        public List<ParaBirimleriModel> GetParaBirimleri()
        {
            var paraBirimleri = _TVMContext.ParaBirimleriRepository.All();
            List<ParaBirimleriModel> paraBirimleriModels = new List<ParaBirimleriModel>();

            foreach (var paraBirimi in paraBirimleri)
            {
                paraBirimleriModels.Add(new ParaBirimleriModel { Id = paraBirimi.Id.ToString().PadLeft(2, '0'), Birimi = paraBirimi.Birimi });
            }

            return paraBirimleriModels;
        }

        public List<TVMOzetModel> GetSatisKanallariListesi(int TVMKodu)
        {
            var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.Tipi != TVMTipleri.UzerindenIsYapilanACente && w.BagliOlduguTVMKodu == TVMKodu || w.Kodu == TVMKodu);
            return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                       .OrderBy(m => m.Unvani)
                       .ToList();
        }


        public List<TVMOzetModel> GetTVMListeMapfre()
        {
            if (_AktifKullanici.MapfreBolge)
            {
                var tvmBolge = _TVMContext.TVMDetayRepository.All()
                                                             .Where(w => w.Kodu == _AktifKullanici.TVMKodu)
                                                             .Select(s => s.BolgeKodu)
                                                             .FirstOrDefault();

                var tvmler = _TVMContext.TVMDetayRepository.All().Where(w => w.BagliOlduguTVMKodu == 107 &&
                                                                             w.BolgeKodu == tvmBolge);

                return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                             .OrderBy(m => m.Unvani)
                             .ToList();
            }
            else if (_AktifKullanici.MapfreMerkezAcente || _AktifKullanici.MapfreMerkez)
            {
                var tvmler = _TVMContext.TVMDetayRepository.All()
                                                           .Where(w => w.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu);

                return tvmler.Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                             .OrderBy(m => m.Unvani)
                             .ToList();
            }

            return null;
        }

        public List<TVMOzetModel> GetTVMListe(string unvan)
        {
            return _TVMContext.TVMDetayRepository
                                .All()
                                .Where(w => w.Unvani.StartsWith(unvan))
                                .OrderBy(o => o.Unvani)
                                .Take(10)
                                .Select(m => new TVMOzetModel() { Kodu = m.Kodu, Unvani = m.Unvani })
                                .ToList();
        }

        public TVMDetay GetDetay(int kodu)
        {
            return _TVMContext.TVMDetayRepository.FindById(kodu);
        }

        public TVMDetay GetDetayYetkili(int kodu)
        {
            TVMDetay tvm = new TVMDetay();

            if (_AktifKullanici != null && kodu > 0)
            {
                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                {
                    tvm = _TVMContext.TVMDetayRepository.FindById(kodu);
                }
                else
                {
                    tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _AktifKullanici.TVMKodu ||
                                                             s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu).Where(p => p.Kodu == kodu).FirstOrDefault();
                }
            }

            return tvm;
        }

        public TVMDetay CreateDetay(TVMDetay detay)
        {
            // ==== TVM Create ==== //
            detay = _TVMContext.TVMDetayRepository.Create(detay);


            // ==== TVM açıldığında o tvmye ürün yetkileri ekleniyor ==== //
            ITUMService _TUMService = DependencyResolver.Current.GetService<ITUMService>();
            List<TUMUrunleri> tumUrunleri = _TUMService.GetListUrunler();
            List<Urun> urunler = _UnitOfWork.UrunRepository.All().ToList<Urun>();

            foreach (var urun in urunler)
            {
                List<TUMUrunleri> TumUrunleriFilter = tumUrunleri.Where(s => s.BABOnlineUrunKodu == urun.UrunKodu).ToList<TUMUrunleri>();
                foreach (var tumUrun in TumUrunleriFilter)
                {
                    TVMUrunYetkileri yetki = new TVMUrunYetkileri();

                    yetki.TUMKodu = tumUrun.TUMKodu;
                    yetki.TUMUrunKodu = tumUrun.TUMUrunKodu;
                    yetki.TVMKodu = detay.Kodu;
                    yetki.BABOnlineUrunKodu = tumUrun.BABOnlineUrunKodu;

                    yetki.AcikHesapTahsilatGercek = 0;
                    yetki.AcikHesapTahsilatTuzel = 0;
                    yetki.HavaleEntegrasyon = 0;
                    yetki.KrediKartiTahsilat = 0;
                    yetki.ManuelHavale = 0;
                    yetki.Police = 0;
                    yetki.Rapor = 0;
                    yetki.Teklif = 0;

                    _TVMContext.TVMUrunYetkileriRepository.Create(yetki);
                }
            }

            // ==== Tüm işlemler başarılı ise kayıtlar tamamlanıyor ==== //
            _TVMContext.Commit();
            return detay;
        }

        public void UpdateDetay(TVMDetay detay)
        {
            _TVMContext.TVMDetayRepository.Update(detay);
            _TVMContext.Commit();
        }

        public string GetTvmUnvan(int kodu)
        {
            TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kodu);

            if (tvm != null)
                return tvm.Unvani;
            else
                return String.Empty;
        }

        public bool TvmTaliVarMi(int kodu)
        {
            TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kodu);
            if (tvm.Tipi == 2 || tvm.Tipi == 1)
            {
                if (tvm.Profili == 1 && tvm.AcentSuvbeVar == 1)
                {
                    var bagliOlanTVMsayisi = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == kodu).ToList().Count();
                    if (bagliOlanTVMsayisi > 0)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public bool MerkezAcenteMi(int kodu)
        {
            TVMDetay tvm = _TVMContext.TVMDetayRepository.FindById(kodu);

            if (tvm != null && tvm.BagliOlduguTVMKodu == -9999)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool KullaniciTvmyiGormeyeYetkiliMi(int kodu)
        {
            if (_AktifKullanici != null && kodu > 0)
            {
                List<TVMDetay> yetkiliTVMList = GetListTVMDetayYetkili();

                int sayac = 0;

                foreach (var item in yetkiliTVMList)
                {
                    if (item.Kodu == kodu)
                        sayac++;
                }

                if (sayac == 0) return false;
                else return true;
            }
            else return false;
        }

        public List<TVMDetay> GetListTvmByBolgeKodu(string BolgeKodu)
        {
            int bolgekodu;
            List<TVMDetay> list = new List<TVMDetay>();

            if (!String.IsNullOrEmpty(BolgeKodu) && int.TryParse(BolgeKodu, out bolgekodu))
            {
                list = _TVMContext.TVMDetayRepository.Filter(s => s.BolgeKodu == bolgekodu && s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu).ToList<TVMDetay>();
            }
            else
            {
                list = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == _AktifKullanici.TVMKodu).OrderBy(s => s.Unvani).ToList<TVMDetay>();
            }

            return list;
        }

        public int GetMerkezAcenteSonSatisKanaliKodu(int merkezTVMKodu)
        {
            int tvmKodu = 0;
            var kodu = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == merkezTVMKodu).OrderByDescending(s => s.Kodu).FirstOrDefault();
            if (kodu != null)
            {
                tvmKodu = kodu.Kodu + 1;
            }
            else
            {
                tvmKodu = Convert.ToInt32(merkezTVMKodu.ToString() + "0001");
            }
            return tvmKodu;
        }

        public List<TVMDetay> GetListTVMDetayPoliceTransferTali()
        {
            List<TVMDetay> tvmList = new List<TVMDetay>();

            if (_AktifKullanici != null)
            {
                int aktifTvmKodu = _AktifKullanici.TVMKodu;
                var detay = this.GetDetay(aktifTvmKodu);
                if (detay.BagliOlduguTVMKodu == -9999)
                {
                    tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == aktifTvmKodu && s.PoliceTransfer == 1).OrderBy(s => s.Unvani).ToList<TVMDetay>();
                }
                else
                {
                    tvmList = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == detay.BagliOlduguTVMKodu && s.PoliceTransfer == 1).OrderBy(s => s.Unvani).ToList<TVMDetay>();
                }
            }

            return tvmList;
        }

        public TVMDetay GetListTVMDetayInternet(int tvmKod)
        {
            return _TVMContext.TVMDetayRepository.All().Where(s => s.BagliOlduguTVMKodu == tvmKod && s.Tipi == TVMTipleri.Internet).FirstOrDefault();
        }

        #endregion

        #region TVMUrunYetki
        public List<TVMUrunYetkileriOzelModel> GetTVMUrunYetki(int tvmkodu, int urunkodu)
        {
            List<TVMUrunYetkileriOzelModel> model = new List<TVMUrunYetkileriOzelModel>();
            List<TVMUrunYetkileri> yetkiliurunler = _TVMContext.TVMUrunYetkileriRepository.Filter(w => w.TVMKodu == tvmkodu &&
                                                                                                       w.BABOnlineUrunKodu == urunkodu &&
                                                                                                       w.Teklif == 1 &&
                                                                                                       w.Police == 1).ToList();

            foreach (var item in yetkiliurunler)
            {
                TVMUrunYetkileriOzelModel mdl = new TVMUrunYetkileriOzelModel();
                mdl.TumKodu = item.TUMKodu;
                mdl.TumUnvani = TUMImgUrlAdres.GetTUMKisaIsim(item.TUMKodu);
                mdl.IMGUrl = TUMImgUrlAdres.GetTUMUrl(item.TUMKodu);
                model.Add(mdl);
            }

            return model;
        }

        public List<TVMUrunYetkileri> GetListTVMUrunYetkileri()
        {
            List<TVMUrunYetkileri> list = new List<TVMUrunYetkileri>();

            list = _TVMContext.TVMUrunYetkileriRepository.All().ToList<TVMUrunYetkileri>();

            return list;
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

        public TVMUrunYetkileri GetTVMUrunYetkisi(int tvmKodu, int babonlineUrunKodu, int tumKodu, string tumUrunKodu)
        {
            TVMUrunYetkileri urun = new TVMUrunYetkileri();
            urun = _TVMContext.TVMUrunYetkileriRepository.Filter(s => s.TVMKodu == tvmKodu &&
                                                       s.BABOnlineUrunKodu == babonlineUrunKodu &&
                                                       s.TUMUrunKodu == tumUrunKodu &&
                                                       s.TUMKodu == tumKodu)
                                                       .FirstOrDefault();


            return urun;

        }

        public TVMUrunYetkileri CreateUrunYetki(TVMUrunYetkileri tvm)
        {
            tvm = _TVMContext.TVMUrunYetkileriRepository.Create(tvm);
            _TVMContext.Commit();
            return tvm;
        }

        public void UpdateUrunYetkileri(TVMUrunYetkileri tvm)
        {
            _TVMContext.TVMUrunYetkileriRepository.Update(tvm);
            _TVMContext.Commit();
        }

        #endregion

        #region TVMIPBaglanti Service

        public TVMIPBaglanti GetTVMIPBaglanti(int siraNo, int tvmKodu)
        {
            TVMIPBaglanti baglanti = _TVMContext.TVMIPBaglantiRepository.Find(m => m.SiraNo == siraNo && m.TVMKodu == tvmKodu);
            return baglanti;
        }

        public List<TVMIPBaglanti> GetListIPBaglanti()
        {
            var ipListesi = _TVMContext.TVMIPBaglantiRepository.All().ToList<TVMIPBaglanti>();
            if (ipListesi != null)
            {
                return ipListesi;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<TVMIPBaglanti> GetListIPBaglanti(int tvmKodu, string ip)
        {

            return _TVMContext.TVMIPBaglantiRepository.Filter(s => s.TVMKodu == tvmKodu && s.BaslangicIP == ip);
        }

        public IQueryable<TVMIPBaglanti> GetListIPBaglanti(int tvmKodu)
        {
            return _TVMContext.TVMIPBaglantiRepository.Filter(s => s.TVMKodu == tvmKodu);
        }
        public IQueryable<TVMIPBaglanti> GetListIPBaglantiAnaTvm(int tvmKodu)
        {
            return _TVMContext.TVMIPBaglantiRepository.All().Where(p => p.TVMKodu.ToString().StartsWith(tvmKodu.ToString()));

        }

        public TVMIPBaglanti CreateIPBaglanti(TVMIPBaglanti baglanti)
        {
            TVMIPBaglanti bgln = _TVMContext.TVMIPBaglantiRepository.Create(baglanti);
            _TVMContext.Commit();

            return bgln;
        }

        public bool UpdateItem(TVMIPBaglanti baglanti)
        {
            _TVMContext.TVMIPBaglantiRepository.Update(baglanti);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteBaglanti(int baglantiKodu, int tvmKodu)
        {
            _TVMContext.TVMIPBaglantiRepository.Delete(m => m.SiraNo == baglantiKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListIPBaglanti(DataTableParameters<TVMIPBaglanti> baglantiList, int tvmKodu)
        {
            IQueryable<TVMIPBaglanti> query = _TVMContext.TVMIPBaglantiRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMIPBaglantiRepository.Page(query,
                                                                  baglantiList.OrderByProperty,
                                                                  baglantiList.IsAscendingOrder,
                                                                  baglantiList.Page,
                                                                  baglantiList.PageSize, out totalRowCount);
            return baglantiList.Prepare(query, totalRowCount);
        }

        public List<TVMIPBaglanti> GetTVMIPAra(string ip)
        {
            int tvmkodu = _AktifKullanici.TVMKodu;
            if (tvmkodu == 100)
            {

                return _TVMContext.TVMIPBaglantiRepository.Filter(s => s.BaslangicIP.Trim() == ip.Trim()).ToList();
            }
            else
            {
                return _TVMContext.TVMIPBaglantiRepository.Filter(s => s.BaslangicIP.Trim() == ip.Trim() && s.TVMKodu == tvmkodu).ToList();
            }
        }

        public List<TVMIPBaglanti> GetTVMIPAraByTvmKodu(int TVMKodu)
        {
            return _TVMContext.TVMIPBaglantiRepository.Filter(s => s.TVMKodu == TVMKodu).ToList();
        }
        public Boolean GetTVMIPVarMi(string ip, int tvmKodu)
        {
            var tvmIP = _TVMContext.TVMIPBaglantiRepository.Filter(s => s.BaslangicIP.Trim() == ip.Trim() && s.TVMKodu == tvmKodu).ToList();
            if (tvmIP.Count > 0)
            {
                //ip var
                return false;
            }
            else
            {
                // ip yok 
                return true;
            }
        }
        #endregion

        #region Neoconnect kadı/şifre arama
        public List<OtoLoginSigortaSirketKullanicilar> GetNeoConnectAra(int? tumkodu, int? grupKodu, int tvmKodu)
        {
            return _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(s => s.TUMKodu == tumkodu && s.GrupKodu == grupKodu && s.TVMKodu == tvmKodu).ToList();
        }
        public List<OtoLoginSigortaSirketKullanicilar> GetNeoConnectSirketAra(int? tumkodu, int tvmKodu)
        {
            return _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(s => s.TUMKodu == tumkodu && s.TVMKodu == tvmKodu).ToList();
        }
        #endregion

        #region TVMNotlar Service
        public TVMNotlar GetTVMNot(int siraNo, int tvmKodu)
        {
            TVMNotlar not = _TVMContext.TVMNotlarRepository.Find(m => m.SiraNo == siraNo && m.TVMKodu == tvmKodu);
            return not;
        }

        public IQueryable<TVMNotlar> GetListNotlar()
        {
            return _TVMContext.TVMNotlarRepository.All();
        }

        public IQueryable<TVMNotlar> GetListNotlar(int tvmKodu)
        {
            return _TVMContext.TVMNotlarRepository.Filter(s => s.TVMKodu == tvmKodu);
        }

        public TVMNotlar CreateNot(TVMNotlar not)
        {
            TVMNotlar nt = _TVMContext.TVMNotlarRepository.Create(not);
            _TVMContext.Commit();

            return nt;
        }

        public bool UpdateItem(TVMNotlar not)
        {
            _TVMContext.TVMNotlarRepository.Update(not);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteNot(int notKodu, int tvmKodu)
        {
            _TVMContext.TVMNotlarRepository.Delete(m => m.SiraNo == notKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListNot(DataTableParameters<TVMNotlar> notList, int tvmKodu)
        {
            IQueryable<TVMNotlar> query = _TVMContext.TVMNotlarRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMNotlarRepository.Page(query,
                                                                  notList.OrderByProperty,
                                                                  notList.IsAscendingOrder,
                                                                  notList.Page,
                                                                  notList.PageSize, out totalRowCount);
            return notList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TVMDokumanlar Service
        public TVMDokumanlar GetTVMDokuman(int siraNo, int tvmKodu)
        {
            TVMDokumanlar dokuman = _TVMContext.TVMDokumanlarRepository.Find(m => m.SiraNo == siraNo && m.TVMKodu == tvmKodu);
            return dokuman;
        }

        public IQueryable<TVMDokumanlar> GetListDokumanlar()
        {
            return _TVMContext.TVMDokumanlarRepository.All();
        }

        public IQueryable<TVMDokumanlar> GetListDokumanlar(int tvmKodu)
        {
            return _TVMContext.TVMDokumanlarRepository.Filter(s => s.TVMKodu == tvmKodu);
        }

        public TVMDokumanlar CreateDokuman(TVMDokumanlar dokuman)
        {
            TVMDokumanlar dkmn = _TVMContext.TVMDokumanlarRepository.Create(dokuman);
            _TVMContext.Commit();

            return dkmn;
        }

        public bool UpdateItem(TVMDokumanlar dokuman)
        {
            _TVMContext.TVMDokumanlarRepository.Update(dokuman);
            _TVMContext.Commit();

            return true;
        }

        public List<DokumanTurleri> GetListDokumanTurleri()
        {
            return _TVMContext.DokumanTurleriRepository.All().ToList<DokumanTurleri>();
        }

        public void DeleteDokuman(int dokumanKodu, int tvmKodu)
        {
            _TVMContext.TVMDokumanlarRepository.Delete(m => m.SiraNo == dokumanKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListDokuman(DataTableParameters<TVMDokumanlar> dokumanList, int tvmKodu)
        {
            IQueryable<TVMDokumanlar> query = _TVMContext.TVMDokumanlarRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMDokumanlarRepository.Page(query,
                                                                  dokumanList.OrderByProperty,
                                                                  dokumanList.IsAscendingOrder,
                                                                  dokumanList.Page,
                                                                  dokumanList.PageSize, out totalRowCount);
            return dokumanList.Prepare(query, totalRowCount);
        }

        public bool CheckedFileName(string fileName)
        {
            List<TVMDokumanlar> dokumanlar = _TVMContext.TVMDokumanlarRepository.Filter(d => d.Dokuman == fileName).ToList<TVMDokumanlar>();
            if (dokumanlar.Count > 0)
                return false;
            else
                return true;
        }
        #endregion

        #region TVMBolgeleri Service
        public TVMBolgeleri GetTVMBolge(int bolgeKodu, int tvmKodu)
        {
            TVMBolgeleri bolge = _TVMContext.TVMBolgeleriRepository.Find(m => m.TVMBolgeKodu == bolgeKodu && m.TVMKodu == tvmKodu);
            return bolge;
        }

        public List<TVMBolgeleri> GetListBolgeler()
        {
            return _TVMContext.TVMBolgeleriRepository.All().ToList();
        }

        public List<TVMBolgeleri> GetListBolgeler(int tvmKodu)
        {
            return _TVMContext.TVMBolgeleriRepository.Filter(s => s.TVMKodu == tvmKodu & s.Durum == 1).ToList<TVMBolgeleri>();
        }

        public TVMBolgeleri CreateBolge(TVMBolgeleri bolge)
        {
            TVMBolgeleri blg = _TVMContext.TVMBolgeleriRepository.Create(bolge);
            _TVMContext.Commit();

            return blg;
        }

        public bool UpdateItem(TVMBolgeleri bolge)
        {
            _TVMContext.TVMBolgeleriRepository.Update(bolge);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteBolge(int bolgeKodu, int tvmKodu)
        {
            _TVMContext.TVMBolgeleriRepository.Delete(m => m.TVMBolgeKodu == bolgeKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListBolge(DataTableParameters<TVMBolgeleri> bolgeList, int tvmKodu)
        {
            IQueryable<TVMBolgeleri> query = _TVMContext.TVMBolgeleriRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMBolgeleriRepository.Page(query,
                                                                  bolgeList.OrderByProperty,
                                                                  bolgeList.IsAscendingOrder,
                                                                  bolgeList.Page,
                                                                  bolgeList.PageSize, out totalRowCount);
            return bolgeList.Prepare(query, totalRowCount);
        }

        #endregion

        #region TVMDepartmanlar Service
        public TVMDepartmanlar GetTVMDepartman(int departmanKodu, int tvmKodu)
        {
            TVMDepartmanlar departman = _TVMContext.TVMDepartmanlarRepository.Find(m => m.DepartmanKodu == departmanKodu && m.TVMKodu == tvmKodu);
            return departman;
        }

        public List<TVMDepartmanlar> GetListDepartmanlar()
        {
            return _TVMContext.TVMDepartmanlarRepository.All().ToList();
        }

        public List<TVMDepartmanlar> GetListDepartmanlar(int tvmKodu)
        {
            return _TVMContext.TVMDepartmanlarRepository.Filter(s => s.TVMKodu == tvmKodu)
                                                        .ToList<TVMDepartmanlar>();
        }

        public TVMDepartmanlar CreateDepartman(TVMDepartmanlar departman)
        {
            TVMDepartmanlar dprtmn = _TVMContext.TVMDepartmanlarRepository.Create(departman);
            _TVMContext.Commit();

            return dprtmn;
        }

        public bool UpdateItem(TVMDepartmanlar departman)
        {
            _TVMContext.TVMDepartmanlarRepository.Update(departman);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteDepartman(int departmanKodu, int tvmKodu)
        {
            _TVMContext.TVMDepartmanlarRepository.Delete(m => m.DepartmanKodu == departmanKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListDepartman(DataTableParameters<TVMDepartmanlar> departmanList, int tvmKodu)
        {
            IQueryable<TVMDepartmanlar> query = _TVMContext.TVMDepartmanlarRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMDepartmanlarRepository.Page(query,
                                                                  departmanList.OrderByProperty,
                                                                  departmanList.IsAscendingOrder,
                                                                  departmanList.Page,
                                                                  departmanList.PageSize, out totalRowCount);
            return departmanList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TVMAcentelikleri Service
        // acentekodu=branskodu
        public TVMAcentelikleri GetTVMAcente(int acenteKodu, int tvmKodu)
        {
            TVMAcentelikleri acente = _TVMContext.TVMAcentelikleriRepository.Find(m => m.BransKodu == acenteKodu && m.TVMKodu == tvmKodu);
            return acente;
        }

        public List<TVMAcentelikleri> GetListAcenteler()
        {
            return _TVMContext.TVMAcentelikleriRepository.All().ToList();
        }

        public List<TVMAcentelikleri> GetListAcenteler(int tvmKodu)
        {
            return _TVMContext.TVMAcentelikleriRepository.Filter(s => s.TVMKodu == tvmKodu).ToList();
        }
        public List<TVMAcentelikleri> GetListTanımliBransOdemeTipleri(int tvmKodu, string tumKodu)
        {
            return _TVMContext.TVMAcentelikleriRepository.Filter(s => s.TVMKodu == tvmKodu && s.SigortaSirketKodu == tumKodu).ToList();
        }
        public List<TVMAcentelikleri> GetListTanımliBransTvmSirketOdemeTipleri(int tvmKodu, string tumKodu, int? bransKodu)
        {
            return _TVMContext.TVMAcentelikleriRepository.Filter(s => s.TVMKodu == tvmKodu && s.SigortaSirketKodu == tumKodu && s.BransKodu == bransKodu).ToList();
        }
        public TVMAcentelikleri CreateAcente(TVMAcentelikleri acente)
        {
            TVMAcentelikleri acnt = _TVMContext.TVMAcentelikleriRepository.Create(acente);
            _TVMContext.Commit();

            return acnt;
        }

        public bool UpdateItem(TVMAcentelikleri acente)
        {
            _TVMContext.TVMAcentelikleriRepository.Update(acente);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteAcente(int acenteKodu, int tvmKodu)
        {
            _TVMContext.TVMAcentelikleriRepository.Delete(m => m.BransKodu == acenteKodu && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListAcente(DataTableParameters<TVMAcentelikleri> acenteList, int tvmKodu)
        {
            IQueryable<TVMAcentelikleri> query = _TVMContext.TVMAcentelikleriRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMAcentelikleriRepository.Page(query,
                                                                  acenteList.OrderByProperty,
                                                                  acenteList.IsAscendingOrder,
                                                                  acenteList.Page,
                                                                  acenteList.PageSize, out totalRowCount);
            return acenteList.Prepare(query, totalRowCount);
        }

        public TVMDetay GetDetayByVergiNo(string vergiNo)
        {
            return _TVMContext.TVMDetayRepository.Filter(s => s.VergiNumarasi == vergiNo).FirstOrDefault();
        }

        public TVMDetay GetDetayByTCKimlikNo(string tckNo)
        {
            return _TVMContext.TVMDetayRepository.Filter(s => s.TCKN == tckNo).FirstOrDefault();
        }
        public string GetPoliceByVergiKimlikNo(string vkn, int tvmKodu)
        {
            PoliceSigortaEttiren varMi = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.VergiKimlikNo == vkn).FirstOrDefault();
            if (varMi != null)
            {
                return varMi.VergiKimlikNo;

            }
            return String.Empty;
        }
        #endregion

        #region BankaHesaplari Service
        public TVMBankaHesaplari GetTVMBankaHesap(int SiraNo, int tvmKodu)
        {
            TVMBankaHesaplari bankahesap = _TVMContext.TVMBankaHesaplariRepository.Find(m => m.SiraNo == SiraNo && m.TVMKodu == tvmKodu);
            return bankahesap;
        }

        public List<TVMBankaHesaplari> GetListTVMBankaHesaplari()
        {
            return _TVMContext.TVMBankaHesaplariRepository.All().ToList();
        }

        public List<TVMBankaHesaplari> GetListTVMBankaHesaplari(int tvmKodu)
        {
            return _TVMContext.TVMBankaHesaplariRepository.Filter(s => s.TVMKodu == tvmKodu).ToList();
        }
        public List<TVMBankaHesaplari> GetListTVMBankaHesaplari(int tvmKodu, int hesaptipi)
        {
            return _TVMContext.TVMBankaHesaplariRepository.Filter(s => s.TVMKodu == tvmKodu && s.HesapTipi == hesaptipi).ToList();
        }
        public TVMBankaHesaplari GetListTVMBankaCariHesaplari(int tvmKodu, int hesaptipi, string cariHesapNo)
        {
            TVMBankaHesaplari tvmbankacarHesaplar = _TVMContext.TVMBankaHesaplariRepository.Find(s => s.TVMKodu == tvmKodu && s.HesapTipi == hesaptipi && s.AcenteKrediKartiNo == cariHesapNo);
            return tvmbankacarHesaplar;
        }

        public bool CheckListTVMBankaCariHesaplari(int tvmKodu, int hesaptipi, string cariHesapNo)
        {
            TVMBankaHesaplari tvmbankacarHesaplar = _TVMContext.TVMBankaHesaplariRepository.Find(s => s.TVMKodu == tvmKodu && s.HesapTipi == hesaptipi && s.AcenteKrediKartiNo == cariHesapNo);
            return tvmbankacarHesaplar != null ? true : false;
        }

        public TVMBankaHesaplari CreateTVMBankaHesap(TVMBankaHesaplari bankaHesap)
        {
            TVMBankaHesaplari bnkhsp = _TVMContext.TVMBankaHesaplariRepository.Create(bankaHesap);
            _TVMContext.Commit();

            return bnkhsp;
        }

        public bool UpdateBankaHesap(TVMBankaHesaplari bankahesap)
        {
            _TVMContext.TVMBankaHesaplariRepository.Update(bankahesap);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteTVMBankaHesap(int SiraNo, int tvmKodu)
        {
            _TVMContext.TVMBankaHesaplariRepository.Delete(m => m.SiraNo == SiraNo && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListTVMBankaHesap(DataTableParameters<TVMBankaHesaplari> bankahesapList, int tvmKodu)
        {
            IQueryable<TVMBankaHesaplari> query = _TVMContext.TVMBankaHesaplariRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMBankaHesaplariRepository.Page(query,
                                                                  bankahesapList.OrderByProperty,
                                                                  bankahesapList.IsAscendingOrder,
                                                                  bankahesapList.Page,
                                                                  bankahesapList.PageSize, out totalRowCount);
            return bankahesapList.Prepare(query, totalRowCount);
        }
        #endregion

        #region IletisimYetkilileri Service
        public TVMIletisimYetkilileri GetTVMIletisimYetkili(int SiraNo, int tvmKodu)
        {
            TVMIletisimYetkilileri iletisimYetkili = _TVMContext.TVMIletisimYetkilileriRepository.Find(m => m.SiraNo == SiraNo && m.TVMKodu == tvmKodu);
            return iletisimYetkili;
        }

        public List<TVMIletisimYetkilileri> GetListTVMIletisimYetkilileri()
        {
            return _TVMContext.TVMIletisimYetkilileriRepository.All().ToList();
        }

        public List<TVMIletisimYetkilileri> GetListTVMIletisimYetkilileri(int tvmKodu)
        {
            return _TVMContext.TVMIletisimYetkilileriRepository.Filter(s => s.TVMKodu == tvmKodu).ToList();
        }

        public TVMIletisimYetkilileri CreateTVMIletisimYetkili(TVMIletisimYetkilileri iletisimYetkili)
        {
            TVMIletisimYetkilileri ltsmYtkl = _TVMContext.TVMIletisimYetkilileriRepository.Create(iletisimYetkili);
            _TVMContext.Commit();

            return ltsmYtkl;
        }

        public bool UpdateIletisimYetkili(TVMIletisimYetkilileri iletisimYetkili)
        {
            _TVMContext.TVMIletisimYetkilileriRepository.Update(iletisimYetkili);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteTVMIletisimYetkili(int SiraNo, int tvmKodu)
        {
            _TVMContext.TVMIletisimYetkilileriRepository.Delete(m => m.SiraNo == SiraNo && m.TVMKodu == tvmKodu);
            _TVMContext.Commit();
        }

        public DataTableList PagedListTVMIletisimYetkili(DataTableParameters<TVMIletisimYetkilileri> iletisimYetkilileriList, int tvmKodu)
        {
            IQueryable<TVMIletisimYetkilileri> query = _TVMContext.TVMIletisimYetkilileriRepository.Filter(s => s.TVMKodu == tvmKodu);

            int totalRowCount = 0;
            query = _TVMContext.TVMIletisimYetkilileriRepository.Page(query,
                                                                  iletisimYetkilileriList.OrderByProperty,
                                                                  iletisimYetkilileriList.IsAscendingOrder,
                                                                  iletisimYetkilileriList.Page,
                                                                  iletisimYetkilileriList.PageSize, out totalRowCount);
            return iletisimYetkilileriList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TVMKullaniciNotlar Service
        public TVMKullaniciNotlar CreateKullaniciNot(TVMKullaniciNotlar not)
        {
            TVMKullaniciNotlar NT = new TVMKullaniciNotlar();
            if (_AktifKullanici != null)
            {
                not.KullaniciKodu = _AktifKullanici.KullaniciKodu;
                not.EklemeTarihi = TurkeyDateTime.Now;

                NT = _TVMContext.TVMKullaniciNotlarRepository.Create(not);
                _TVMContext.Commit();
            }
            return NT;
        }

        public bool UpdateKullaniciNot(TVMKullaniciNotlar not)
        {
            if (_AktifKullanici != null)
            {
                _TVMContext.TVMKullaniciNotlarRepository.Update(not);
                _TVMContext.Commit();
                return true;
            }
            return false;
        }

        public List<TVMKullaniciNotlar> GetListKullaniciNotlar()
        {
            List<TVMKullaniciNotlar> notlar = new List<TVMKullaniciNotlar>();
            if (_AktifKullanici != null)
            {
                notlar = _TVMContext.TVMKullaniciNotlarRepository.Filter(s => s.KullaniciKodu == _AktifKullanici.KullaniciKodu)
                                                                                          .OrderByDescending(s => s.KullaniciNotId)
                                                                                          .ToList<TVMKullaniciNotlar>();
            }
            return notlar;
        }

        public bool DeleteKullaniciNot(int KullaniciNotId)
        {
            if (_AktifKullanici != null)
            {
                _TVMContext.TVMKullaniciNotlarRepository.Delete(s => s.KullaniciKodu == _AktifKullanici.KullaniciKodu && s.KullaniciNotId == KullaniciNotId);
                _TVMContext.Commit();
                return true;
            }
            return false;
        }

        public TVMKullaniciNotlar GetKullaniciNot(int notId)
        {
            TVMKullaniciNotlar not = new TVMKullaniciNotlar();
            if (_AktifKullanici != null)
            {
                int kullaniciId = _AktifKullanici.KullaniciKodu;
                not = _TVMContext.TVMKullaniciNotlarRepository.Filter(s => s.KullaniciNotId == notId && s.KullaniciKodu == kullaniciId).FirstOrDefault();
            }
            return not;
        }
        #endregion

        #region WebServis Kullanicilari Service
        public TVMWebServisKullanicilari GetTVMWebServisKullanicilari(int tvmKodu, int tumKodu)
        {
            TVMWebServisKullanicilari kullanici = _TVMContext.TVMWebServisKullanicilariRepository.Find(m => m.TVMKodu == tvmKodu && m.TUMKodu == tumKodu);

            return kullanici;
        }

        public List<TVMWebServisKullanicilari> GetListTVMWebServisKullanicilari(int tvmKodu)
        {
            return _TVMContext.TVMWebServisKullanicilariRepository.Filter(f => f.TVMKodu == tvmKodu).ToList();
        }

        public TVMWebServisKullanicilari CreateTVMWebServisKullanicilari(TVMWebServisKullanicilari kullanici)
        {
            kullanici = _TVMContext.TVMWebServisKullanicilariRepository.Create(kullanici);
            _TVMContext.Commit();

            return kullanici;
        }

        public bool UpdateTVMWebServisKullanicilari(TVMWebServisKullanicilari kulllanici)
        {
            _TVMContext.TVMWebServisKullanicilariRepository.Update(kulllanici);
            _TVMContext.Commit();

            return true;
        }

        public void DeleteTVMWebServisKullanicilari(int tvmKodu, int tumKodu)
        {
            _TVMContext.TVMWebServisKullanicilariRepository.Delete(m => m.TVMKodu == tvmKodu && m.TUMKodu == tumKodu);
            _TVMContext.Commit();
        }
        #endregion
        /// <summary>
        /// /neoconnect
        /// </summary>
        /// <param name="tvmKodu"></param>
        /// <param name="tumKodu"></param>
        /// <returns></returns>
        /// 

        #region NeoConnect Kullanicilari Service

        public OtoLoginSigortaSirketKullanicilar GetNeoConnectKullanicilari(int tvmKodu, int id)
        {
            OtoLoginSigortaSirketKullanicilar kullanici = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Find(m => m.TVMKodu == tvmKodu && m.Id == id);
            return kullanici;
        }

        public List<OtoLoginSigortaSirketKullanicilar> GetListNeoConnectKullanicilari(int tvmKodu)
        {
            return _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(f => f.TVMKodu == tvmKodu).ToList();
        }

        public OtoLoginSigortaSirketKullanicilar GetNeoConnectKullanici(int anaTvmKodu, int TUMKodu)
        {
            return _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(f => f.TVMKodu == anaTvmKodu && f.TUMKodu == TUMKodu).FirstOrDefault();
        }

        public List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectSirketGrupKullaniciList(int tvmKodu, string tumKodu)
        {
            var grupKullanici = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.All().Where(m => m.TvmKodu == tvmKodu && m.SirketKodu == tumKodu).ToList<NeoConnectSirketGrupKullaniciDetay>();
            return grupKullanici;
        }

        public NeoConnectSirketGrupKullaniciDetay GetNeoConnectSirketGrupKullaniciDetay(int grupKodu)
        {
            var grupKullanici = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Find(m => m.GrupKodu == grupKodu);
            return grupKullanici;
        }
        public List<NeoConnectSirketGrupKullaniciDetay> GetListNeoConnectGrupSirketleri(int? tumKodu, int tvmKodu)
        {
            return _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.All().Where(s => s.SirketKodu == tumKodu.ToString() && s.TvmKodu == tvmKodu).OrderBy(s => s.GrupAdi).ToList();
        }
        public OtoLoginSigortaSirketKullanicilar CreateNeoConnectKullanicilari(OtoLoginSigortaSirketKullanicilar kullanici)
        {
            kullanici = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Create(kullanici);
            _KomisyonContext.Commit();

            return kullanici;
        }

        public bool UpdateNeoConnectKullanicilari(OtoLoginSigortaSirketKullanicilar kulllanici)
        {
            _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Update(kulllanici);
            _KomisyonContext.Commit();

            return true;
        }

        public void DeleteNeoConnectKullanicilari(int id)
        {
            _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Delete(m => m.Id == id);
            _KomisyonContext.Commit();
        }
        #endregion

        #region NeoConnectYasakliUrl Kullanicilari Service

        public NeoConnectYasakliUrller GetNeoConnectYasakliUrlKullanicilari(int tvmKodu, int tumKodu)
        {
            NeoConnectYasakliUrller kullanici = _KomisyonContext.NeoConnectYasakliUrllerRepository.Find(m => m.TvmKodu == tvmKodu && m.SigortaSirketKodu == tumKodu);
            return kullanici;
        }

        public List<NeoConnectYasakliUrller> GetListNeoConnectYasakliUrlKullanicilari(int tvmKodu)
        {
            return _KomisyonContext.NeoConnectYasakliUrllerRepository.Filter(f => f.TvmKodu == tvmKodu).ToList();
        }

        public NeoConnectYasakliUrller CreateNeoConnectYasakliUrlKullanicilari(NeoConnectYasakliUrller kullanici)
        {
            //try
            //{

            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            kullanici = _KomisyonContext.NeoConnectYasakliUrllerRepository.Create(kullanici);
            _KomisyonContext.Commit();

            return kullanici;
        }

        public bool UpdateNeoConnectYasakliUrlKullanicilari(NeoConnectYasakliUrller kulllanici)
        {
            _KomisyonContext.NeoConnectYasakliUrllerRepository.Update(kulllanici);
            _KomisyonContext.Commit();

            return true;
        }

        public void DeleteNeoConnectYasakliUrlKullanicilari(int tvmKodu, int tumKodu)
        {
            _KomisyonContext.NeoConnectYasakliUrllerRepository.Delete(m => m.TvmKodu == tvmKodu && m.SigortaSirketKodu == tumKodu);
            _KomisyonContext.Commit();
        }
        #endregion


        #region NeoConnectTvmSirketAtama Kullanicilari Service

        public List<TVMDetay> GetAcenteList(int bagliOlduguTvmKodu)
        {
            var tvmList = _TVMContext.TVMDetayRepository.All().Where(w => w.BagliOlduguTVMKodu == bagliOlduguTvmKodu).ToList<TVMDetay>();
            return tvmList;
        }


        public NeoConnectTvmSirketYetkileri GetNeoConnectTvmSirketKullanicilari(int tvmKodu, string tumKodu)
        {
            NeoConnectTvmSirketYetkileri kullanici = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Find(m => m.TvmKodu == tvmKodu && m.TumKodu == tumKodu);
            return kullanici;
        }

        public List<NeoConnectTvmSirketYetkileri> GetListNeoConnectTvmSirketKullanicilari(int tvmKodu)
        {
            var tvmler = this.GetAcenteList(tvmKodu).Select(t => t.Kodu).ToArray();

            return _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.All().Where(f => f.TvmKodu == tvmKodu || tvmler.Contains(f.TvmKodu)).ToList();


        }

        public NeoConnectTvmSirketYetkileri CreateNeoConnectTvmSirketKullanicilari(NeoConnectTvmSirketYetkileri kullanici)
        {
            var SirketVarMi = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.All().Where(s => s.TvmKodu == kullanici.TvmKodu && s.TumKodu == kullanici.TumKodu).FirstOrDefault();
            if (SirketVarMi == null)
            {
                kullanici = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Create(kullanici);
                _KomisyonContext.Commit();

                return kullanici;
            }
            return null;

        }

        public void NeoConnectDeleteSirketSifreYetki(int id)
        {
            _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Delete(m => m.Id == id);
            _KomisyonContext.Commit();
        }


        public bool UpdateNeoConnectTvmSirketKullanicilari(NeoConnectTvmSirketYetkileri kulllanici)
        {
            //  _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Update(kulllanici);
            // _KomisyonContext.Commit();
            try
            {
                var SirketVarMi = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.All().Where(s => s.Id == kulllanici.Id).FirstOrDefault();
                if (SirketVarMi != null)
                {
                    _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Update(kulllanici);
                    _KomisyonContext.Commit();

                    return true;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public void DeleteNeoConnectTvmSirketKullanicilari(int tvmKodu, string tumKodu)
        {
            _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.Delete(m => m.TvmKodu == tvmKodu && m.TumKodu == tumKodu);
            _KomisyonContext.Commit();
        }

        #endregion

        #region TVM Harita Adres

        public TVMLocationModelList GetListTVMHarita()
        {
            TVMLocationModelList model = new TVMLocationModelList();

            List<TVMLocationModel> list = _TVMContext.TVMDetayRepository.Filter(s => !String.IsNullOrEmpty(s.Latitude) &&
                                                                                     !String.IsNullOrEmpty(s.Longitude)).Select(s => new TVMLocationModel()
                                                                                     {
                                                                                         Kod = s.Kodu,
                                                                                         Unvan = s.Unvani,
                                                                                         Telefon = s.Telefon,
                                                                                         Adres = s.Adres,
                                                                                         Lat = s.Latitude,
                                                                                         Lgn = s.Longitude
                                                                                     }).ToList<TVMLocationModel>();


            model.List = list;
            return model;
        }

        #endregion
        #region NeoConnectGrupTanımlama

        public List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectSirketGrupKullaniciDetayByTVMKodu()
        {
            List<NeoConnectSirketGrupKullaniciDetay> result = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.All().Where(w => w.TvmKodu == _AktifKullanici.TVMKodu).ToList();
            return result;
        }
        public void DeleteNeoConnectSirketGrupKullaniciDetayByTVMKodu(int id)
        {
            _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Delete(m => m.GrupKodu == id);
            _KomisyonContext.Commit();
        }
        public void CreateNeoConnectSirketGrupKullaniciDetayByTVMKodu(NeoConnectSirketGrupKullaniciDetay grup)
        {
            _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Create(grup);
            _KomisyonContext.Commit();
        }
        public void UpdateNeoConnectSirketGrupKullaniciDetayByTVMKodu(NeoConnectSirketGrupKullaniciDetay grup)
        {
            _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Update(grup);
            _KomisyonContext.Commit();
        }
        #endregion


        public bool AddTaliTVM(TVMDetay tvmDetay)
        {
            try
            {
                bool kayitVarMi = this.KayitVarMi(tvmDetay.Email);
                if (!kayitVarMi)
                {
                    _TVMContext.TVMDetayRepository.Create(tvmDetay);
                    _TVMContext.Commit();

                    return true;
                }
                else return false;


            }
            catch (Exception)
            {
                return false;
            }
        }
        public TVMDetay getAnaAcenteTvmDetay(int tvmkodu)
        {
            int anaAcenteTvmKodu = Convert.ToInt32(tvmkodu.ToString().Substring(0, 3));
            return _TVMContext.TVMDetayRepository.All().Where(w => w.Kodu == anaAcenteTvmKodu).First();
        }
        private bool KayitVarMi(string email)
        {
            TVMDetay kayit = _TVMContext.TVMDetayRepository.Find(s => s.Email == email);
            if (kayit != null) return true;
            else return false;
        }

        public int GetYetkiGrupKodu(int tvmkod)
        {
            TVMYetkiGruplari yetki = _TVMContext.TVMYetkiGruplariRepository.Filter(s => s.TVMKodu == tvmkod).OrderBy(s => s.YetkiGrupKodu).FirstOrDefault();
            if (yetki != null) return yetki.YetkiGrupKodu;
            else return 0;
        }

        public OfflineUretimPerformans GetOffilenUretimPerformans(int tvmKodu, int donemYil, string taliTvmler)
        {
            OfflineUretimPerformans model = new OfflineUretimPerformans();
            model = _TVMContext.OfflineUretim(tvmKodu, donemYil, taliTvmler);

            return model;
        }

        public int GetServisKullaniciTVMKodu(int tvmKodu)
        {
            int tvmKod = 0;
            var tvm = this.GetDetay(_AktifKullanici.TVMKodu);
            if (tvm != null)
            {
                if (tvm.BagliOlduguTVMKodu == -9999)
                {
                    var tvmdetay = this.GetListTVMDetayInternet(tvm.Kodu);
                    if (tvmdetay != null && tvmdetay.Tipi == TVMTipleri.Internet)
                    {
                        tvmKod = tvmdetay.BagliOlduguTVMKodu;
                    }
                    else
                    {
                        tvmKod = tvmKodu;
                    }
                }
                else
                {
                    var tvmdetay = this.GetListTVMDetayInternet(tvm.BagliOlduguTVMKodu);
                    if (tvmdetay != null && tvmdetay.Tipi == TVMTipleri.Internet)
                    {
                        tvmKod = tvmdetay.BagliOlduguTVMKodu;
                    }
                    else
                    {
                        tvmKod = tvm.BagliOlduguTVMKodu;
                    }
                }
            }

            return tvmKod;
        }

        public OtoLoginSigortaSirketKullanicilar GetAutoLoginKullanici(int id)
        {
            OtoLoginSigortaSirketKullanicilar result = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.All().Where(w => w.Id == id).FirstOrDefault();
            return result;
        }
        public NeoConnectTvmSirketYetkileri GetNeoConnectTvmSirketYetkileriKullanici(int id)
        {
            NeoConnectTvmSirketYetkileri result = _KomisyonContext.NeoConnectTvmSirketYetkileriRepository.All().Where(w => w.Id == id).FirstOrDefault();
            return result;
        }
        public NeoConnectYasakliUrller GetNeoConnectYasakliUrllerKullanici(int id)
        {
            NeoConnectYasakliUrller result = _KomisyonContext.NeoConnectYasakliUrllerRepository.All().Where(w => w.id == id).FirstOrDefault();
            return result;
        }

        public List<NeoConnectSirketGrupKullaniciDetay> GetNeoConnectGrupKullanicilist(int tvmKodu, string sirketKodu)
        {
            List<NeoConnectSirketGrupKullaniciDetay> result = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.All().Where(w => w.TvmKodu == tvmKodu && w.SirketKodu == sirketKodu).ToList();
            return result;
        }

        public List<NeoConnectGrupKullanicilistGuncelleSonucModel> NeoConnectGrupKullanicilistGuncelle(int tvmKodu, List<NeoConnectSirketGrupKullaniciDetay> listModel)
        {
            List<NeoConnectGrupKullanicilistGuncelleSonucModel> returnModel = new List<NeoConnectGrupKullanicilistGuncelleSonucModel>();
            NeoConnectGrupKullanicilistGuncelleSonucModel returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
            try
            {
                int sayac = 0;
                List<NeoConnectSirketGrupKullaniciDetay> grupKullanicilar = new List<NeoConnectSirketGrupKullaniciDetay>();
                if (listModel.Count > 0)
                {
                    foreach (var item in listModel)
                    {
                        if (sayac == 0)
                        {
                            grupKullanicilar = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.All().Where(w => w.TvmKodu == tvmKodu && w.SirketKodu == item.SirketKodu).ToList();
                        }
                        if (grupKullanicilar != null)
                        {

                            foreach (var itemGrup in grupKullanicilar)
                            {
                                if (item.GrupKodu == itemGrup.GrupKodu)
                                {
                                    if (!String.IsNullOrEmpty(itemGrup.KullaniciAdi) && !String.IsNullOrEmpty(itemGrup.Sifre))
                                    {
                                        if (itemGrup.KullaniciAdi != item.KullaniciAdi || itemGrup.Sifre != item.Sifre)
                                        {
                                            returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
                                            returnModelItem.grupAdi = item.GrupAdi;
                                            itemGrup.KullaniciAdi = item.KullaniciAdi;
                                            itemGrup.Sifre = item.Sifre;
                                            _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Update(itemGrup);
                                            _KomisyonContext.Commit();
                                            returnModelItem.basarili = true;
                                            returnModel.Add(returnModelItem);
                                        }
                                        else
                                        {
                                            returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
                                            returnModelItem.grupAdi = item.GrupAdi + " kayıt günceldir.";
                                            returnModelItem.basarili = false;
                                            returnModel.Add(returnModelItem);
                                        }
                                    }
                                }
                            }
                        }
                        sayac++;
                    }
                }
            }
            catch (Exception)
            {
                returnModelItem.basarili = false;
                returnModel.Add(returnModelItem);
            }

            return returnModel;
        }

        public List<OtoLoginSigortaSirketKullanicilar> NeoConnectSirketListesi(int tvmkodu)
        {
            var list = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(s => s.AltTVMKodu == tvmkodu).ToList();

            return list;
        }
        public List<OtoLoginSigortaSirketKullanicilar> NeoConnectSirketListesi()
        {
            var list = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Filter(s => s.TVMKodu == _AktifKullanici.TVMKodu && (s.AltTVMKodu == null || s.AltTVMKodu == _AktifKullanici.TVMKodu)).OrderBy(s => s.SigortaSirketAdi).ToList();

            return list;
        }
        public List<NeoConnectGrupKullanicilistGuncelleSonucModel> NeoConnectMerkezKullanicilistGuncelle(int tvmKodu, List<OtoLoginSigortaSirketKullanicilar> listModel)
        {
            List<NeoConnectGrupKullanicilistGuncelleSonucModel> returnModel = new List<NeoConnectGrupKullanicilistGuncelleSonucModel>();
            NeoConnectGrupKullanicilistGuncelleSonucModel returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
            try
            {
                int sayac = 0;
                List<OtoLoginSigortaSirketKullanicilar> grupKullanicilar = new List<OtoLoginSigortaSirketKullanicilar>();
                if (listModel.Count > 0)
                {
                    foreach (var item in listModel)
                    {
                        if (sayac == 0)
                        {
                            grupKullanicilar = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.All().Where(w => (w.TVMKodu == tvmKodu && w.AltTVMKodu == null) || (w.TVMKodu == tvmKodu && w.AltTVMKodu == tvmKodu)).ToList();
                        }
                        if (grupKullanicilar != null)
                        {
                            foreach (var itemGrup in grupKullanicilar)
                            {
                                if (item.TVMKodu == itemGrup.TVMKodu && item.TUMKodu == itemGrup.TUMKodu && item.GrupKodu == itemGrup.GrupKodu)
                                {
                                    //if (!String.IsNullOrEmpty(item.KullaniciAdi) && !String.IsNullOrEmpty(item.Sifre) && !String.IsNullOrEmpty(item.ProxyIpPort))
                                    if (!String.IsNullOrEmpty(item.KullaniciAdi) && !String.IsNullOrEmpty(item.Sifre))
                                    {
                                        if (itemGrup.KullaniciAdi != item.KullaniciAdi || itemGrup.Sifre != item.Sifre || itemGrup.ProxyIpPort != item.ProxyIpPort || itemGrup.SmsKodTelNo != item.SmsKodTelNo || itemGrup.SmsKodSecretKey1 != item.SmsKodSecretKey1)
                                        {
                                            returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
                                            itemGrup.KullaniciAdi = item.KullaniciAdi;
                                            itemGrup.Sifre = item.Sifre;
                                            itemGrup.ProxyIpPort = item.ProxyIpPort;
                                            itemGrup.SmsKodTelNo = item.SmsKodTelNo;
                                            itemGrup.SmsKodSecretKey1 = item.SmsKodSecretKey1;
                                            itemGrup.SmsKodSecretKey2 = item.SmsKodSecretKey2;
                                            _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.Update(itemGrup);
                                            _KomisyonContext.Commit();
                                            returnModelItem.basarili = true;
                                            returnModel.Add(returnModelItem);
                                        }
                                        else
                                        {
                                            returnModelItem = new NeoConnectGrupKullanicilistGuncelleSonucModel();
                                            returnModelItem.basarili = false;
                                            returnModel.Add(returnModelItem);
                                        }
                                    }

                                }
                            }
                        }
                        sayac++;
                    }
                }
            }
            catch (Exception)
            {
                returnModelItem.basarili = false;
                returnModel.Add(returnModelItem);
            }

            return returnModel;
        }
        public string GetGrupAdi(int grupkodu)
        {
            string returnVal;
            var grupAdi = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Find(s => s.GrupKodu == grupkodu);
            if (grupAdi != null)
            {
                returnVal = grupAdi.GrupAdi;
            }
            else
            {
                returnVal = "";
            }
            return returnVal;
        }

        public List<Kesintiler> GetListKesintiler(int TVMKodu)
        {
            return _TVMContext.KesintilerRepository.All().Where(w => w.TVMKodu == TVMKodu).ToList<Kesintiler>();
        }

        public List<KesintiTurleri> GetListKesintiTurleri()
        {
            return _TVMContext.KesintiTurleriRepository.All().ToList<KesintiTurleri>();
        }

        public bool CreateOdemeGirisi(Kesintiler model)
        {
            bool result;
            try
            {
                _TVMContext.KesintilerRepository.Create(model);
                _TVMContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool UpdateOdemeGirisi(Kesintiler model)
        {
            bool result;
            try
            {
                _TVMContext.KesintilerRepository.Update(model);
                _TVMContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public List<Kesintiler> GetOdemeGirisiListe(int acenteKodu, int donem)
        {

            int bagliOldugutvmKod = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == acenteKodu).BagliOlduguTVMKodu;

            if (bagliOldugutvmKod != -9999)
            {
                return _TVMContext.KesintilerRepository.All().Where(s => s.Donem == donem && (s.TVMKodu == acenteKodu || s.TVMKoduTali == acenteKodu)).ToList();

            }
            else
            {
                return _TVMContext.KesintilerRepository.All().Where(s => s.Donem == donem && s.TVMKodu == acenteKodu && s.TVMKoduTali == null).ToList();
            }

        }

        public List<Kesintiler> GetMasrafListesi(int acenteKodu, int donem)
        {
            return _TVMContext.KesintilerRepository.All().Where(s => s.Donem == donem && s.TVMKodu == acenteKodu).ToList();
        }

        public List<NeoConnectSirketGrupKullaniciDetay> GetNeoconnectGruplist()
        {
            var list = _KomisyonContext.NeoConnectSirketGrupKullaniciDetayRepository.Filter(s => s.TvmKodu == _AktifKullanici.TVMKodu).ToList();

            return list;
        }

        public Kesintiler GetOdemeGirisiList(int Id)
        {
            return _TVMContext.KesintilerRepository.All().Where(s => s.Id == Id).FirstOrDefault();
        }

        public TVMSMSKullaniciBilgi GetSmsKullaniciBilgileri(int tvmKodu)
        {
            return _TVMContext.TVMSMSKullaniciBilgiRepository.All().Where(s => s.TVMKodu == tvmKodu).FirstOrDefault();
        }

        public bool KullaniciMobilOnayKoduEkle(int tvmKodu, int kullanicikodu, string mobilOnayKodu)
        {
            bool guncellendi = false;
            try
            {
                var guncellenecekKayit = _TVMContext.TVMKullanicilarRepository.All().Where(s => s.TVMKodu == tvmKodu && s.KullaniciKodu == kullanicikodu).FirstOrDefault();
                if (guncellenecekKayit != null)
                {
                    guncellenecekKayit.MobilDogrulamaKodu = mobilOnayKodu;
                    _TVMContext.TVMKullanicilarRepository.Update(guncellenecekKayit);
                    _TVMContext.Commit();
                    guncellendi = true;
                }

            }
            catch (Exception)
            {
                guncellendi = false;
                throw;
            }


            return guncellendi;
        }

        public bool KullaniciMobilOnayKoduDogrula(int tvmKodu, int kullanicikodu, string mobilOnayKodu)
        {
            bool Onaylandi = false;
            try
            {
                var dogrula = _TVMContext.TVMKullanicilarRepository.All().Where(s => s.TVMKodu == tvmKodu && s.KullaniciKodu == kullanicikodu && s.MobilDogrulamaKodu == mobilOnayKodu).FirstOrDefault();
                if (dogrula != null)
                {
                    Onaylandi = true;

                }

            }
            catch (Exception)
            {
                Onaylandi = false;
                throw;
            }
            return Onaylandi;
        }

        public bool KullaniciMobilOnayKoduSifirla(int tvmKodu, int kullanicikodu, string mobilOnayKodu)
        {
            bool Sifirlandi = false;
            try
            {
                var silinecekKayit = _TVMContext.TVMKullanicilarRepository.All().Where(s => s.TVMKodu == tvmKodu && s.KullaniciKodu == kullanicikodu && s.MobilDogrulamaKodu == mobilOnayKodu).FirstOrDefault();
                if (silinecekKayit != null)
                {
                    silinecekKayit.MobilDogrulamaKodu = "NULL";
                    silinecekKayit.MobilDogrulamaOnaylandiMi = "true";
                    _TVMContext.TVMKullanicilarRepository.Update(silinecekKayit);
                    _TVMContext.Commit();
                    Sifirlandi = true;

                }


            }
            catch (Exception)
            {
                Sifirlandi = false;
                throw;
            }
            return Sifirlandi;
        }

        public void MobilOnayDogrulandi(int tvmKodu, int kullanicikodu)
        {
            var silinecekKayit = _TVMContext.TVMKullanicilarRepository.All().Where(s => s.TVMKodu == tvmKodu && s.KullaniciKodu == kullanicikodu).FirstOrDefault();
            try
            {
                if (silinecekKayit != null)
                {
                    silinecekKayit.MobilDogrulamaOnaylandiMi = "NULL";
                    _TVMContext.TVMKullanicilarRepository.Update(silinecekKayit);
                    _TVMContext.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NeoConnectLog> GetListNeoConnectLog(DateTime KullaniciGirisTarihi, DateTime KullaniciCikisTarihi, int tvmKodu, List<string> tumKoduList, List<int> tvmlist)
        {
            List<NeoConnectLog> returnList = new List<NeoConnectLog>();
            NeoConnectLog model = new NeoConnectLog();
            if (tvmlist != null)
            {
                var logList = _TVMContext.NeoConnectLogRepository.All().Where(s => tvmlist.Contains(s.TvmKodu.Value) && tumKoduList.Contains(s.SigortaSirketKodu) && s.KullaniciGirisTarihi > KullaniciGirisTarihi && s.KullaniciCikisTarihi <= KullaniciCikisTarihi).OrderBy(s => s.KullaniciGirisTarihi).ToList();
                if (logList.Count > 0)
                {
                    foreach (var itemLog in logList)
                    {
                        model = new NeoConnectLog();
                        model.SigortaSirketKodu = itemLog.SigortaSirketKodu;
                        model.IPAdresi = itemLog.IPAdresi;
                        model.MACAdresi = itemLog.MACAdresi;
                        model.Kullanici = itemLog.Kullanici;
                        model.SirketKullaniciAdi = itemLog.SirketKullaniciAdi;
                        model.SirketKullaniciSifresi = itemLog.SirketKullaniciSifresi;
                        model.KullaniciCikisTarihi = itemLog.KullaniciCikisTarihi;
                        model.KullaniciGirisTarihi = itemLog.KullaniciGirisTarihi;
                        model.GrupKodu = itemLog.GrupKodu;
                        returnList.Add(model);
                    }
                }

            }

            return returnList;
        }

        public bool KokpitGuncelle(int tvmKodu, int donemYil, string taliAcenteler, string branslar)
        {
            var result = _TVMContext.KokpitGuncelle(tvmKodu, donemYil, taliAcenteler, branslar);
            return result;
        }
        public bool KokpitKullaniciGuncelle(int tvmKodu, int donemYil, string kullanicikodu, string branslar)
        {
            var result = _TVMContext.KokpitKullaniciGuncelle(tvmKodu, donemYil, kullanicikodu, branslar);
            return result;
        }
        public ParaBirimleri GetParaBirimiByBirimi(string paraBirimi)
        {
            return _TVMContext.ParaBirimleriRepository.All().Where(paraBirimleri => paraBirimleri.Birimi == paraBirimi).FirstOrDefault();
        }
        public void UnicoApiKeyVarMi(int tvmKodu, out string apiKey, out string autKey)
        {
            apiKey = "";
            autKey = "";
            var UnicoKey = _TVMContext.TVMWebServisKullanicilariRepository.All().Where(w => w.TVMKodu == tvmKodu && w.TUMKodu == TeklifUretimMerkezleri.UNICO).FirstOrDefault();
            if (UnicoKey != null)
            {
                if (!String.IsNullOrEmpty(UnicoKey.KullaniciAdi2))
                {
                    apiKey = UnicoKey.KullaniciAdi2;
                    autKey = UnicoKey.Sifre2;
                }
            }
        }
    }
}

