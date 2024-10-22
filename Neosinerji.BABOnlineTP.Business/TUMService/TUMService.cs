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

namespace Neosinerji.BABOnlineTP.Business
{
    public class TUMAcenteSubeVar
    {
        public const byte Yok = 0;
        public const byte Var = 1;
    }

    public class TUMService : ITUMService
    {
        ITUMContext _TUMContext;
        ITVMService _TVMService;

        public TUMService(ITUMContext tumContext, ITVMService tvmService)
        {
            _TUMContext = tumContext;
            _TVMService = tvmService;
        }

        #region TUM Service
        public DataTableList PagedList(TUMListe tumListe)
        {
            IQueryable<TUMDetay> query = _TUMContext.TUMDetayRepository.All();

            if (tumListe.Kodu.HasValue)
                query = query.Where(w => w.Kodu == tumListe.Kodu.Value);

            if (!String.IsNullOrEmpty(tumListe.Unvani))
                query = query.Where(w => w.Unvani.StartsWith(tumListe.Unvani));

            if (!String.IsNullOrEmpty(tumListe.BirlikKodu))
                query = query.Where(w => w.BirlikKodu.StartsWith(tumListe.BirlikKodu));

            int totalRowCount = 0;
            query = _TUMContext.TUMDetayRepository.Page(query, tumListe.OrderByProperty, tumListe.IsAscendingOrder, tumListe.Page, tumListe.PageSize, out totalRowCount);

            return tumListe.Prepare(query, totalRowCount);
        }

        public List<TUMDetay> GetListTUMDetay()
        {
            return _TUMContext.TUMDetayRepository.All().Where(w=>w.UygulamaKodu !=UygulamaKodlari.NeoConnect).OrderBy(s => s.Unvani).ToList();
        }
        public List<TUMDetay> GetNeoConnectTUMList()
        {
            return _TUMContext.TUMDetayRepository.All().OrderBy(s => s.Unvani).ToList();
        }

        public TUMDetay GetDetay(int kodu)
        {
            return _TUMContext.TUMDetayRepository.FindById(kodu);
        }

        public TUMDetay CreateDetay(TUMDetay detay)
        {
            detay = _TUMContext.TUMDetayRepository.Create(detay);
            _TUMContext.Commit();
            return detay;
        }

        public void UpdateDetay(TUMDetay detay)
        {
            _TUMContext.TUMDetayRepository.Update(detay);
            _TUMContext.Commit();
        }

        public string GetTumUnvan(int kodu)
        {
            string result = String.Empty;

            TUMDetay tum = _TUMContext.TUMDetayRepository.Filter(s => s.Kodu == kodu).FirstOrDefault();
            if (tum != null)
                result = tum.Unvani;

            return result;
        }

        public int GetTUMKodu(string TUMBirlikKodu)
        {
            int tumKodu = 0;
            var kod = _TUMContext.TUMDetayRepository.All().Where(s => s.BirlikKodu == TUMBirlikKodu).FirstOrDefault();
            if (kod!=null)
            {
                tumKodu = kod.Kodu;
            }
            return tumKodu;
        }
        public string GetTUMVknKodu(string  TUMBirlikKodu )
        {
            string result = String.Empty;
            TUMDetay kod = _TUMContext.TUMDetayRepository.All().Where(s => s.BirlikKodu == TUMBirlikKodu).FirstOrDefault();
            if (kod != null)
            {
                result = kod.VergiNumarasi;
            }
            return result;

        }
        #endregion

        #region TUMIPBaglanti Service
        public TUMIPBaglanti GetTUMIPBaglanti(int siraNo, int tumKodu)
        {
            TUMIPBaglanti baglanti = _TUMContext.TUMIPBaglantiRepository.Find(m => m.SiraNo == siraNo && m.TUMKodu == tumKodu);
            return baglanti;
        }

        public List<TUMIPBaglanti> GetListIPBaglanti()
        {
            return _TUMContext.TUMIPBaglantiRepository.All().ToList();
        }

        public List<TUMIPBaglanti> GetListIPBaglanti(int tumKodu)
        {
            return _TUMContext.TUMIPBaglantiRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMIPBaglanti CreateIPBaglanti(TUMIPBaglanti baglanti)
        {
            TUMIPBaglanti bgln = _TUMContext.TUMIPBaglantiRepository.Create(baglanti);
            _TUMContext.Commit();

            return bgln;
        }

        public bool UpdateItem(TUMIPBaglanti baglanti)
        {
            _TUMContext.TUMIPBaglantiRepository.Update(baglanti);
            _TUMContext.Commit();

            return true;
        }

        public int GetTumDokumanSiraNo(int tumKodu)
        {
            int? siraNo = _TUMContext.TUMDokumanlarRepository.Filter(s => s.TUMKodu == tumKodu).Max(s => s.SiraNo);
            if (siraNo.HasValue) return siraNo.Value;
            else return 1;
        }

        public void DeleteBaglanti(int baglantiKodu, int tumKodu)
        {
            _TUMContext.TUMIPBaglantiRepository.Delete(m => m.SiraNo == baglantiKodu && m.TUMKodu == tumKodu);
            _TUMContext.Commit();
        }

        public DataTableList PagedListIPBaglanti(DataTableParameters<TUMIPBaglanti> baglantiList, int tumKodu)
        {
            IQueryable<TUMIPBaglanti> query = _TUMContext.TUMIPBaglantiRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMIPBaglantiRepository.Page(query,
                                                                  baglantiList.OrderByProperty,
                                                                  baglantiList.IsAscendingOrder,
                                                                  baglantiList.Page,
                                                                  baglantiList.PageSize, out totalRowCount);
            return baglantiList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TUMNotlar Service
        public TUMNotlar GetTUMNot(int siraNo, int tumKodu)
        {
            TUMNotlar not = _TUMContext.TUMNotlarRepository.Find(m => m.SiraNo == siraNo && m.TUMKodu == tumKodu);
            return not;
        }

        public List<TUMNotlar> GetListNotlar()
        {
            return _TUMContext.TUMNotlarRepository.All().ToList();
        }

        public List<TUMNotlar> GetListNotlar(int tumKodu)
        {
            return _TUMContext.TUMNotlarRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMNotlar CreateNot(TUMNotlar not)
        {
            TUMNotlar nt = _TUMContext.TUMNotlarRepository.Create(not);
            _TUMContext.Commit();

            return nt;
        }

        public bool UpdateItem(TUMNotlar not)
        {
            _TUMContext.TUMNotlarRepository.Update(not);
            _TUMContext.Commit();

            return true;
        }

        public void DeleteNot(int notKodu, int tumKodu)
        {
            _TUMContext.TUMNotlarRepository.Delete(m => m.SiraNo == notKodu && m.TUMKodu == tumKodu);
            _TUMContext.Commit();
        }

        public DataTableList PagedListNot(DataTableParameters<TUMNotlar> notList, int tumKodu)
        {
            IQueryable<TUMNotlar> query = _TUMContext.TUMNotlarRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMNotlarRepository.Page(query,
                                                                  notList.OrderByProperty,
                                                                  notList.IsAscendingOrder,
                                                                  notList.Page,
                                                                  notList.PageSize, out totalRowCount);
            return notList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TUMDokumanlar Service
        public TUMDokumanlar GetTUMDokuman(int siraNo, int tumKodu)
        {
            TUMDokumanlar dokuman = _TUMContext.TUMDokumanlarRepository.Find(m => m.SiraNo == siraNo && m.TUMKodu == tumKodu);
            return dokuman;
        }

        public List<TUMDokumanlar> GetListDokumanlar()
        {
            return _TUMContext.TUMDokumanlarRepository.All().ToList();
        }

        public List<TUMDokumanlar> GetListDokumanlar(int tumKodu)
        {
            return _TUMContext.TUMDokumanlarRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMDokumanlar CreateDokuman(TUMDokumanlar dokuman)
        {
            TUMDokumanlar dkmn = _TUMContext.TUMDokumanlarRepository.Create(dokuman);
            _TUMContext.Commit();

            return dkmn;
        }

        public bool UpdateItem(TUMDokumanlar dokuman)
        {
            _TUMContext.TUMDokumanlarRepository.Update(dokuman);
            _TUMContext.Commit();

            return true;
        }

        public void DeleteDokuman(int dokumanKodu, int tumKodu)
        {
            _TUMContext.TUMDokumanlarRepository.Delete(m => m.SiraNo == dokumanKodu && m.TUMKodu == tumKodu);
            _TUMContext.Commit();
        }

        public DataTableList PagedListDokuman(DataTableParameters<TUMDokumanlar> dokumanList, int tumKodu)
        {
            IQueryable<TUMDokumanlar> query = _TUMContext.TUMDokumanlarRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMDokumanlarRepository.Page(query,
                                                                  dokumanList.OrderByProperty,
                                                                  dokumanList.IsAscendingOrder,
                                                                  dokumanList.Page,
                                                                  dokumanList.PageSize, out totalRowCount);
            return dokumanList.Prepare(query, totalRowCount);
        }

        public bool CheckedFileName(string fileName)
        {
            List<TUMDokumanlar> dokumanlar = _TUMContext.TUMDokumanlarRepository.Filter(d => d.Dokuman == fileName).ToList<TUMDokumanlar>();
            if (dokumanlar.Count > 0)
                return false;
            else
                return true;
        }
        #endregion

        #region BankaHesaplari Service
        public TUMBankaHesaplari GetTUMBankaHesap(int SiraNo, int tumKodu)
        {
            TUMBankaHesaplari bankahesap = _TUMContext.TUMBankaHesaplariRepository.Find(m => m.SiraNo == SiraNo && m.TUMKodu == tumKodu);
            return bankahesap;
        }

        public List<TUMBankaHesaplari> GetListTUMBankaHesaplari()
        {
            return _TUMContext.TUMBankaHesaplariRepository.All().ToList();
        }

        public List<TUMBankaHesaplari> GetListTUMBankaHesaplari(int tumKodu)
        {
            return _TUMContext.TUMBankaHesaplariRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMBankaHesaplari CreateTUMBankaHesap(TUMBankaHesaplari bankaHesap)
        {
            TUMBankaHesaplari bnkhsp = _TUMContext.TUMBankaHesaplariRepository.Create(bankaHesap);
            _TUMContext.Commit();

            return bnkhsp;
        }

        public bool UpdateBankaHesap(TUMBankaHesaplari bankahesap)
        {
            _TUMContext.TUMBankaHesaplariRepository.Update(bankahesap);
            _TUMContext.Commit();

            return true;
        }

        public void DeleteTUMBankaHesap(int SiraNo, int tumKodu)
        {
            _TUMContext.TUMBankaHesaplariRepository.Delete(m => m.SiraNo == SiraNo && m.TUMKodu == tumKodu);
            _TUMContext.Commit();
        }

        public DataTableList PagedListTUMBankaHesap(DataTableParameters<TUMBankaHesaplari> bankahesapList, int tumKodu)
        {
            IQueryable<TUMBankaHesaplari> query = _TUMContext.TUMBankaHesaplariRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMBankaHesaplariRepository.Page(query,
                                                                  bankahesapList.OrderByProperty,
                                                                  bankahesapList.IsAscendingOrder,
                                                                  bankahesapList.Page,
                                                                  bankahesapList.PageSize, out totalRowCount);
            return bankahesapList.Prepare(query, totalRowCount);
        }
        #endregion

        #region IletisimYetkilileri Service
        public TUMIletisimYetkilileri GetTUMIletisimYetkili(int SiraNo, int tumKodu)
        {
            TUMIletisimYetkilileri iletisimYetkili = _TUMContext.TUMIletisimYetkilileriRepository.Find(m => m.SiraNo == SiraNo && m.TUMKodu == tumKodu);
            return iletisimYetkili;
        }

        public List<TUMIletisimYetkilileri> GetListTUMIletisimYetkilileri()
        {
            return _TUMContext.TUMIletisimYetkilileriRepository.All().ToList();
        }

        public List<TUMIletisimYetkilileri> GetListTUMIletisimYetkilileri(int tumKodu)
        {
            return _TUMContext.TUMIletisimYetkilileriRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMIletisimYetkilileri CreateTUMIletisimYetkili(TUMIletisimYetkilileri iletisimYetkili)
        {
            TUMIletisimYetkilileri ltsmYtkl = _TUMContext.TUMIletisimYetkilileriRepository.Create(iletisimYetkili);
            _TUMContext.Commit();

            return ltsmYtkl;
        }

        public bool UpdateIletisimYetkili(TUMIletisimYetkilileri iletisimYetkili)
        {
            _TUMContext.TUMIletisimYetkilileriRepository.Update(iletisimYetkili);
            _TUMContext.Commit();

            return true;
        }

        public void DeleteTUMIletisimYetkili(int SiraNo, int tumKodu)
        {
            _TUMContext.TUMIletisimYetkilileriRepository.Delete(m => m.SiraNo == SiraNo && m.TUMKodu == tumKodu);
            _TUMContext.Commit();
        }

        public DataTableList PagedListTUMIletisimYetkili(DataTableParameters<TUMIletisimYetkilileri> iletisimYetkilileriList, int tumKodu)
        {
            IQueryable<TUMIletisimYetkilileri> query = _TUMContext.TUMIletisimYetkilileriRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMIletisimYetkilileriRepository.Page(query,
                                                                  iletisimYetkilileriList.OrderByProperty,
                                                                  iletisimYetkilileriList.IsAscendingOrder,
                                                                  iletisimYetkilileriList.Page,
                                                                  iletisimYetkilileriList.PageSize, out totalRowCount);
            return iletisimYetkilileriList.Prepare(query, totalRowCount);
        }
        #endregion

        #region TUMUrunler Service
        public TUMUrunleri GetTUMUrun(string urunKodu, int tumKodu)
        {
            TUMUrunleri urun = _TUMContext.TUMUrunleriRepository.Find(m => m.TUMUrunKodu == urunKodu &&
                                                                      m.TUMKodu == tumKodu);
            return urun;
        }
        public TUMUrunleri GetTUMUrun(string urunKodu, int tumKodu, int BabOnlineUrunKodu)
        {
            TUMUrunleri urun = _TUMContext.TUMUrunleriRepository.Find(m => m.TUMUrunKodu == urunKodu &&
                                                                      m.TUMKodu == tumKodu &&
                                                                      m.BABOnlineUrunKodu == BabOnlineUrunKodu);
            return urun;
        }

        public List<TUMUrunleri> GetListUrunler()
        {
            return _TUMContext.TUMUrunleriRepository.All().ToList();
        }

        public List<TUMUrunleri> GetListUrunler(int tumKodu)
        {
            return _TUMContext.TUMUrunleriRepository.Filter(s => s.TUMKodu == tumKodu).ToList();
        }

        public TUMUrunleri CreateUrun(TUMUrunleri urun)
        {
            // ==== Yen bir ürün ekleniyor ==== //
            TUMUrunleri urn = _TUMContext.TUMUrunleriRepository.Create(urun);


            // ==== Yeni bir ürün eklendikten sorra o ürüne ait yetkilerde tüm tvmlere ekleniyor ==== //
            List<TVMDetay> tvmler = _TVMService.GetListTVMDetay();

            foreach (var tvm in tvmler)
            {
                TVMUrunYetkileri yetki = new TVMUrunYetkileri();
                yetki.TVMKodu = tvm.Kodu;
                yetki.TUMUrunKodu = urun.TUMUrunKodu;
                yetki.TUMKodu = urun.TUMKodu;
                yetki.BABOnlineUrunKodu = urun.BABOnlineUrunKodu;

                yetki.AcikHesapTahsilatGercek = 0;
                yetki.AcikHesapTahsilatTuzel = 0;
                yetki.HavaleEntegrasyon = 0;
                yetki.KrediKartiTahsilat = 0;
                yetki.ManuelHavale = 0;
                yetki.Police = 0;
                yetki.Rapor = 0;
                yetki.Teklif = 0;

                _TVMService.CreateUrunYetki(yetki);
            }


            // ==== Tüm işlemler başarılıysa kaydediliyor ==== //
            _TUMContext.Commit();
            return urn;
        }

        public bool UpdateItem(TUMUrunleri urun)
        {
            _TUMContext.TUMUrunleriRepository.Update(urun);
            _TUMContext.Commit();

            return true;
        }

        public void DeleteUrun(string tumUrunKodu, int tumKodu, int babOnlineUrunKodu)
        {
            // ==== Bir ürün siliniyor ==== //
            _TUMContext.TUMUrunleriRepository.Delete(m => m.TUMUrunKodu == tumUrunKodu && m.TUMKodu == tumKodu && m.BABOnlineUrunKodu == babOnlineUrunKodu);


            // ==== Ürün silindikten sonra o ürüne bağlı olan yetkilerde kaldırılıyor ==== //
            ITVMContext _TVMContext = DependencyResolver.Current.GetService<ITVMContext>();
            _TVMContext.TVMUrunYetkileriRepository.Delete(s => s.TUMKodu == tumKodu && s.TUMUrunKodu == tumUrunKodu);


            // ==== Ve tüm işlemler kaydediliyor ==== //
            _TUMContext.Commit();
            _TVMContext.Commit();
        }

        public DataTableList PagedListUrun(DataTableParameters<TUMUrunleri> urunList, int tumKodu)
        {
            IQueryable<TUMUrunleri> query = _TUMContext.TUMUrunleriRepository.Filter(s => s.TUMKodu == tumKodu);

            int totalRowCount = 0;
            query = _TUMContext.TUMUrunleriRepository.Page(query,
                                                                  urunList.OrderByProperty,
                                                                  urunList.IsAscendingOrder,
                                                                  urunList.Page,
                                                                  urunList.PageSize, out totalRowCount);
            return urunList.Prepare(query, totalRowCount);
        }
        #endregion
    }
}
