using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI.DASK;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using Neosinerji.BABOnlineTP.Business.AEGON;
using Neosinerji.BABOnlineTP.Business.aegon;
using System.Web;
using System.Linq.Expressions;
using Neosinerji.BABOnlineTP.Database.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TeklifService : ITeklifService
    {
        ITeklifContext _TeklifContext;
        IParametreContext _ParametreContext;
        IMusteriContext _MusteriContext;
        ITUMContext _TUMContext;
        ITVMContext _TVMContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMService _TVMService;
        IMusteriService _MusteriService;
        ILogService _LogService;
        IProcedureContext _ProcedurContext;
        ICRContext _CRContext;

        public TeklifService()
        {

        }

        [InjectionConstructor]
        public TeklifService(ITeklifContext teklifContext, IParametreContext parametreContext, ITVMContext tvmContext, IMusteriContext musteriContext, ITUMContext tumContext, IProcedureContext procedurContext, ICRContext CRContext)
        {
            _TeklifContext = teklifContext;
            _ParametreContext = parametreContext;
            _MusteriContext = musteriContext;
            _TUMContext = tumContext;
            _TVMContext = tvmContext;
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
            _ProcedurContext = procedurContext;
            _CRContext = CRContext;
        }

        public ITeklif Create(ITeklif teklif)
        {
            teklif.GenelBilgiler.KayitTarihi = TurkeyDateTime.Now;
            if (teklif.GenelBilgiler.TeklifNo == 0)
            {
                TVMDetay tvm = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == teklif.GenelBilgiler.TVMKodu).FirstOrDefault();

                if (tvm != null && tvm.ProjeKodu == TVMProjeKodlari.Aegon)
                {
                    teklif.TeklifNo = _TeklifContext.YeniTeklifNo(NeosinerjiTVM.AegonTVMKodu);
                }
                else if (tvm != null && tvm.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    teklif.TeklifNo = _TeklifContext.YeniTeklifNo(NeosinerjiTVM.MapfreeTVMKodu);
                }
                else
                {
                    if (tvm.Profili == TVMProfilleri.Sube)
                    {
                        teklif.TeklifNo = _TeklifContext.YeniTeklifNo(tvm.BagliOlduguTVMKodu);
                    }
                    else
                        teklif.TeklifNo = _TeklifContext.YeniTeklifNo(teklif.GenelBilgiler.TVMKodu);
                }
            }

            teklif.GenelBilgiler = _TeklifContext.TeklifGenelRepository.Create(teklif.GenelBilgiler);
            teklif.GenelBilgiler.TeklifSigortaEttirens.Add(teklif.SigortaEttiren);

            foreach (TeklifSigortali item in teklif.Sigortalilar)
            {
                teklif.GenelBilgiler.TeklifSigortalis.Add(item);
            }
            if (teklif.Arac.PlakaNo != null)
            {
                if (teklif.SaveArac())
                {
                    teklif.GenelBilgiler.TeklifAracs.Add(teklif.Arac);
                }
            }


            if (teklif.SaveAracEkSoru())
            {
                foreach (TeklifAracEkSoru item in teklif.AracEkSorular)
                {
                    teklif.GenelBilgiler.TeklifAracEkSorus.Add(item);
                }
            }

            if (teklif.SaveRizikoAdresi())
            {
                teklif.GenelBilgiler.TeklifRizikoAdresis.Add(teklif.RizikoAdresi);
            }

            if (teklif.SaveSorular())
            {
                foreach (TeklifSoru item in teklif.Sorular)
                {
                    teklif.GenelBilgiler.TeklifSorus.Add(item);
                }
            }
            if (teklif.SaveWebServisCevaplar())
            {
                foreach (TeklifWebServisCevap item in teklif.WebServisCevaplar)
                {
                    teklif.GenelBilgiler.TeklifWebServisCevaps.Add(item);
                }
            }
            if (teklif.SaveTeminatlar())
            {
                foreach (TeklifTeminat item in teklif.Teminatlar)
                {
                    teklif.GenelBilgiler.TeklifTeminats.Add(item);
                }
            }
            if (teklif.SaveVergiler())
            {
                foreach (TeklifVergi item in teklif.Vergiler)
                {
                    teklif.GenelBilgiler.TeklifVergis.Add(item);
                }
            }

            if (teklif.SaveOdemePlani())
            {
                foreach (TeklifOdemePlani item in teklif.OdemePlani)
                {
                    teklif.GenelBilgiler.TeklifOdemePlanis.Add(item);
                }
            }

            if (teklif.SaveLog())
            {
                foreach (WEBServisLog item in teklif.Log)
                {
                    teklif.GenelBilgiler.WEBServisLogs.Add(item);
                }
            }

            _TeklifContext.Commit();

            return teklif;
        }

        public void UpdateGenelBilgiler(TeklifGenel teklifGenel)
        {
            _TeklifContext.TeklifGenelRepository.Update(teklifGenel);
            _TeklifContext.Commit();
        }

        public void UpdateTeklif(ITeklif teklif)
        {
            List<TeklifTeminat> teminatlar = teklif.GenelBilgiler.TeklifTeminats.ToList<TeklifTeminat>();
            foreach (TeklifTeminat item in teminatlar)
            {
                _TeklifContext.TeklifTeminatRepository.Delete(item);
            }
            List<TeklifVergi> vergiler = teklif.GenelBilgiler.TeklifVergis.ToList<TeklifVergi>();
            foreach (TeklifVergi item in vergiler)
            {
                _TeklifContext.TeklifVergiRepository.Delete(item);
            }
            _TeklifContext.Commit();
            if (teklif.OdemePlani.Count > 0)
            {
                List<TeklifOdemePlani> odemeler = teklif.GenelBilgiler.TeklifOdemePlanis.ToList<TeklifOdemePlani>();
                foreach (TeklifOdemePlani item in odemeler)
                {
                    _TeklifContext.TeklifOdemePlaniRepository.Delete(item);
                }
                _TeklifContext.Commit();
            }


            teklif.GenelBilgiler.KayitTarihi = TurkeyDateTime.Now;
            _TeklifContext.TeklifGenelRepository.Update(teklif.GenelBilgiler);

            if (teklif.SaveTeminatlar())
            {
                foreach (TeklifTeminat item in teklif.Teminatlar)
                {
                    teklif.GenelBilgiler.TeklifTeminats.Add(item);
                }
            }
            if (teklif.SaveVergiler())
            {
                foreach (TeklifVergi item in teklif.Vergiler)
                {
                    teklif.GenelBilgiler.TeklifVergis.Add(item);
                }
            }
            if (teklif.SaveLog())
            {
                foreach (WEBServisLog item in teklif.Log)
                {
                    teklif.GenelBilgiler.WEBServisLogs.Add(item);
                }
            }
            if (teklif.OdemePlani.Count() > 0)
            {
                foreach (TeklifOdemePlani item in teklif.OdemePlani)
                {
                    teklif.GenelBilgiler.TeklifOdemePlanis.Add(item);
                }
            }

            _TeklifContext.Commit();
        }

        /// <summary>
        /// Teklif kaydını veritabanından alır.
        /// 
        /// Exceptions:
        ///     TeklifBulunamadiException : teklif bulunamadi
        /// </summary>
        /// <param name="teklifId"></param>
        /// <returns></returns>
        public ITeklif GetTeklif(int teklifId)
        {
            IAktifKullaniciService _Aktifkullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            TeklifGenel teklifGenel = new TeklifGenel();
            if (_Aktifkullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                _Aktifkullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            {
                teklifGenel = _TeklifContext.TeklifGenelRepository.FindById(teklifId);
            }
            else
            {
                if (_Aktifkullanici.ProjeKodu == TVMProjeKodlari.Mapfre &&
                    (_Aktifkullanici.TVMKodu == 107 || _Aktifkullanici.MapfreBolge))
                {
                    var tvmler = _MusteriContext.TVMDetayRepository.Filter(f => f.Kodu == 107 || f.BagliOlduguTVMKodu == 107);
                    if (_Aktifkullanici.MapfreBolge)
                    {
                        int BolgeKodu = _MusteriContext.TVMDetayRepository.All()
                                                       .Where(w => w.Kodu == _Aktifkullanici.TVMKodu)
                                                       .Select(s => s.BolgeKodu)
                                                       .FirstOrDefault();

                        tvmler = tvmler.Where(w => w.BolgeKodu == BolgeKodu);
                    }
                    else if (_Aktifkullanici.MapfreMerkezAcente)
                    {
                        tvmler = tvmler.Where(w => w.Kodu == _Aktifkullanici.TVMKodu || w.GrupKodu == _Aktifkullanici.TVMKodu);
                    }

                    IQueryable<TeklifGenel> allTeklif = _TeklifContext.TeklifGenelRepository.All();

                    teklifGenel = (from tvm in tvmler
                                   join t in allTeklif on tvm.Kodu equals t.TVMKodu
                                   where t.TeklifId == teklifId
                                   select t).FirstOrDefault();
                }
                else
                {
                    IQueryable<TVMDetay> yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _Aktifkullanici.TVMKodu ||
                                                                                                    s.BagliOlduguTVMKodu == _Aktifkullanici.TVMKodu);
                    IQueryable<TeklifGenel> allTeklif = _TeklifContext.TeklifGenelRepository.All();

                    teklifGenel = (from tvm in yetkiliTvmler
                                   join t in allTeklif on tvm.Kodu equals t.TVMKodu
                                   where t.TeklifId == teklifId
                                   select t).FirstOrDefault();
                }
            }

            if (teklifGenel == null)
            {
                throw new TeklifBulunamadiException();
            }

            ITeklif teklif = new Teklif(teklifGenel);
            teklif.SigortaEttiren = teklifGenel.TeklifSigortaEttirens.FirstOrDefault();
            teklif.Sigortalilar = teklifGenel.TeklifSigortalis.ToList<TeklifSigortali>();
            teklif.Arac = teklifGenel.TeklifAracs.FirstOrDefault();
            teklif.AracEkSorular = teklifGenel.TeklifAracEkSorus.ToList<TeklifAracEkSoru>();
            teklif.Sorular = teklifGenel.TeklifSorus.ToList<TeklifSoru>();
            teklif.Teminatlar = teklifGenel.TeklifTeminats.ToList<TeklifTeminat>();
            teklif.Vergiler = teklifGenel.TeklifVergis.ToList<TeklifVergi>();
            teklif.WebServisCevaplar = teklifGenel.TeklifWebServisCevaps.ToList<TeklifWebServisCevap>();
            teklif.RizikoAdresi = teklifGenel.TeklifRizikoAdresis.FirstOrDefault();

            List<WEBServisLog> logs = teklifGenel.WEBServisLogs.ToList<WEBServisLog>();
            foreach (var item in logs)
                teklif.Log.Add(item);

            return teklif;
        }

        public TeklifGenel GetTeklifGenel(int teklifId)
        {
            IAktifKullaniciService _Aktifkullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            TeklifGenel teklifGenel = new TeklifGenel();
            if (_Aktifkullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _Aktifkullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                teklifGenel = _TeklifContext.TeklifGenelRepository.FindById(teklifId);
            else
            {
                IQueryable<TVMDetay> yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _Aktifkullanici.TVMKodu ||
                                                                                                s.BagliOlduguTVMKodu == _Aktifkullanici.TVMKodu);
                IQueryable<TeklifGenel> allTeklif = _TeklifContext.TeklifGenelRepository.All();

                teklifGenel = (from tvm in yetkiliTvmler
                               join t in allTeklif on tvm.Kodu equals t.TVMKodu
                               where t.TeklifId == teklifId
                               select t).FirstOrDefault();
            }
            return teklifGenel;
        }

        public TeklifGenel GetTeklifGenel(int teklifNo, int tvmKodu, int tumKodu)
        {
            TeklifGenel teklifGenel = new TeklifGenel();

            teklifGenel = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklifNo && s.TVMKodu == tvmKodu && s.TUMKodu == tumKodu).FirstOrDefault();

            return teklifGenel;
        }

        public ITeklif GetAnaTeklif(int teklifNo, int tvmkodu)
        {
            TeklifGenel teklifGenel = _TeklifContext.TeklifGenelRepository.Find(s => s.TeklifNo == teklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmkodu);

            if (teklifGenel != null)
            {
                ITeklif teklif = new Teklif(teklifGenel);
                teklif.SigortaEttiren = teklifGenel.TeklifSigortaEttirens.FirstOrDefault();
                teklif.Sigortalilar = teklifGenel.TeklifSigortalis.ToList<TeklifSigortali>();
                TeklifArac arac = teklifGenel.TeklifAracs.FirstOrDefault();
                if (arac == null)
                {
                    TeklifGenel tklf = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklifGenel.TeklifNo && s.TUMKodu == 0).FirstOrDefault();
                    if (tklf != null)
                    {
                        arac = _TeklifContext.TeklifAracRepository.Find(s => s.TeklifId == tklf.TeklifId);
                    }
                }
                teklif.Arac = arac;
                teklif.Sorular = teklifGenel.TeklifSorus.ToList<TeklifSoru>();
                teklif.Teminatlar = teklifGenel.TeklifTeminats.ToList<TeklifTeminat>();
                teklif.Vergiler = teklifGenel.TeklifVergis.ToList<TeklifVergi>();
                teklif.WebServisCevaplar = teklifGenel.TeklifWebServisCevaps.ToList<TeklifWebServisCevap>();
                teklif.RizikoAdresi = teklifGenel.TeklifRizikoAdresis.FirstOrDefault();

                return teklif;
            }

            return null;
        }

        public List<ITeklif> GetTeklifler(int[] teklifIdList)
        {
            List<TeklifGenel> teklifLer = _TeklifContext.TeklifGenelRepository.Filter(f => teklifIdList.Contains(f.TeklifId)).ToList<TeklifGenel>();

            List<ITeklif> result = new List<ITeklif>();
            foreach (TeklifGenel teklifGenel in teklifLer)
            {
                ITeklif teklif = new Teklif(teklifGenel);
                result.Add(teklif);
            }

            return result;
        }

        public List<ITeklif> GetTeklifListe(int teklifNo, int tvmkodu)
        {
            var teklifGenelListe = _TeklifContext.TeklifGenelRepository
                                                 .Filter(f => f.TeklifNo == teklifNo && f.TUMKodu > 0 && f.TVMKodu == tvmkodu)
                                                 .ToList<TeklifGenel>();

            List<ITeklif> teklifler = new List<ITeklif>();
            foreach (var item in teklifGenelListe)
            {
                ITeklif teklif = new Teklif(item);
                teklif.Teminatlar = item.TeklifTeminats.ToList<TeklifTeminat>();

                teklifler.Add(teklif);
            }

            return teklifler;
        }

        public List<TeklifTUMDetayPartialModel> GetAllListTeklif(int teklifid)
        {
            List<TeklifTUMDetayPartialModel> model = new List<TeklifTUMDetayPartialModel>();
            TeklifGenel anaTeklif = _TeklifContext.TeklifGenelRepository.FindById(teklifid);
            IQueryable<TeklifGenel> teklifs = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == anaTeklif.TeklifNo && s.TVMKodu == anaTeklif.TVMKodu);
            IQueryable<TUMDetay> tums = _TUMContext.TUMDetayRepository.All();
            IQueryable<Urun> uruns = _ParametreContext.UrunRepository.All();

            var listte = from t in teklifs
                         join tum in tums on t.TUMKodu equals tum.Kodu
                         join u in uruns on t.UrunKodu equals u.UrunKodu
                         select new
                         {
                             TeklifId = t.TeklifId,
                             TeklifNo = t.TeklifNo,
                             TumKodu = t.TUMKodu,
                             TumUnvani = tum.Unvani,
                             UrunKodu = u.UrunKodu,
                             UrunAdi = u.UrunAdi,
                             TumTeklifNo = t.TUMTeklifNo,
                             TumPoliceNo = t.TUMPoliceNo,
                             BrutPrim = t.BrutPrim,
                             Basarili = t.Basarili,
                             TeklifDurumKodu = t.TeklifDurumKodu,
                             Otorizasyon = t.Otorizasyon
                         };


            foreach (var item in listte)
            {
                TeklifTUMDetayPartialModel mdl = new TeklifTUMDetayPartialModel();
                mdl.TUMKodu = item.TumKodu;
                mdl.TUMUnvani = item.TumUnvani;//TUMImgUrlAdres.GetTUMKisaIsim(item.TumKodu);
                mdl.TeklifNo = item.TeklifNo;
                mdl.TUMTeklifNo = item.TumTeklifNo;
                mdl.TUMPoliceNo = item.TumPoliceNo;
                mdl.UrunKodu = item.UrunKodu;
                mdl.UrunAdi = item.UrunAdi;
                mdl.TeklifDurumKodu = item.TeklifDurumKodu;
                mdl.BrutPrim = item.BrutPrim;
                mdl.Otorizasyon = item.Otorizasyon.HasValue && item.Otorizasyon.Value == 1;

                if (item.TeklifDurumKodu == TeklifDurumlari.Police)
                    mdl.PoliceURL = TeklifSayfaAdresleri.PoliceAdres(item.UrunKodu) + item.TeklifId;
                else
                    mdl.TeklifURL = TeklifSayfaAdresleri.DetayAdres(anaTeklif.UrunKodu) + anaTeklif.TeklifId;

                if ((item.Basarili.HasValue && item.Basarili == false) || mdl.Otorizasyon)
                {
                    IsDurumDetay detay = _TeklifContext.IsDurumDetayRepository.Filter(s => s.ReferansId == item.TeklifId).FirstOrDefault();
                    if (detay != null && !String.IsNullOrEmpty(detay.HataMesaji))
                    {
                        string[] hatalar = detay.HataMesaji.Split('|');
                        if (hatalar.Length > 0)
                        {
                            mdl.Hatalar = new List<string>();
                            foreach (var hata in hatalar)
                                if (!String.IsNullOrEmpty(hata) && hata != "\r\n")
                                    mdl.Hatalar.Add(hata);
                        }
                    }
                }

                model.Add(mdl);
            }

            return model;
        }

        public IsDurum GetIsDurumu(int isId)
        {
            return _TeklifContext.IsDurumRepository.FindById(isId);
        }

        public IsDurumDetay GetIsDurumDetay(int referansId)
        {
            return _TeklifContext.IsDurumDetayRepository.All().Where(w => w.ReferansId == referansId).FirstOrDefault();
        }

        public int ToplamTeklif()
        {
            return _TeklifContext.TeklifGenelRepository.All().Count();
        }

        public int ToplamTeklif(int tvmKodu)
        {
            return _TeklifContext.TeklifGenelRepository.Filter(s => s.TVMKodu == tvmKodu).Count();
        }

        public List<TeklifAramaTableModel> PagedList(TeklifListe arama, out int totalRowCount, bool teklifMi)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            
            List<TeklifAramaTableModel> listeModel = new List<TeklifAramaTableModel>();
            // if (_Aktif == null) { return listeModel; }

            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.All();
            IQueryable<TeklifGenel> TUMTeklifler = _TeklifContext.TeklifGenelRepository.All();

            IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
            List<TUMDetay> tumler = _TUMContext.TUMDetayRepository.All().ToList<TUMDetay>();

            if (_Aktif.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                TUMTeklifler = TUMTeklifler.Where(w => w.TUMKodu == TeklifUretimMerkezleri.MAPFRE);

                if (String.IsNullOrEmpty(arama.TeklifNo))
                {
                    if (arama.BaslangisTarihi.HasValue)
                    {
                        teklifler = teklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);
                        TUMTeklifler = TUMTeklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);
                    }
                    if (arama.BitisTarihi.HasValue)
                    {
                        teklifler = teklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);
                        TUMTeklifler = TUMTeklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);
                    }
                }
            }
            else
            {
                if (arama.BaslangisTarihi.HasValue)
                {
                    teklifler = teklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);
                }

                if (arama.BitisTarihi.HasValue)
                {
                    teklifler = teklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);
                }
            }

            // === Poliçe için filitreleme === //
            if (!teklifMi)
            {
                if (_Aktif.ProjeKodu == TVMProjeKodlari.Aegon)
                {
                    teklifler = teklifler.Where(s => s.TUMKodu != 0 && s.TeklifDurumKodu == TeklifDurumlari.Police);
                }
                else
                {
                    teklifler = teklifler.Where(s => s.TUMKodu != 0 && s.TUMPoliceNo != null && s.TeklifDurumKodu == TeklifDurumlari.Police);
                }


                if (arama.TUMKodu.HasValue)
                    teklifler = teklifler.Where(s => s.TUMKodu == arama.TUMKodu.Value);

                if (!String.IsNullOrEmpty(arama.PoliceNo))
                    teklifler = teklifler.Where(s => s.TUMPoliceNo == arama.PoliceNo);

                // T ile başlayan teklif numarası girildi ise TUMTeklifNo alanında arama yapacağız
                if (_Aktif.ProjeKodu == TVMProjeKodlari.Mapfre && !String.IsNullOrEmpty(arama.TeklifNo) && arama.TeklifNo.StartsWith("T"))
                {
                    string tempTeklifNo = arama.TeklifNo;
                    teklifler = teklifler.Where(w => w.TUMTeklifNo == tempTeklifNo);

                    arama.TeklifNo = String.Empty;
                }
            }
            else
            {
                // T ile başlayan teklif numarası girildi ise TUMTeklifNo alanında arama yapacağız
                if (_Aktif.ProjeKodu == TVMProjeKodlari.Mapfre && !String.IsNullOrEmpty(arama.TeklifNo) && arama.TeklifNo.StartsWith("T"))
                {
                    arama.TUMKodu = TeklifUretimMerkezleri.MAPFRE;
                }

                if (arama.TUMKodu.HasValue && arama.TUMKodu.Value > 0)
                {
                    IQueryable<TeklifGenel> TUMTeklifleri = _TeklifContext.TeklifGenelRepository.All().Where(s => s.TUMKodu == arama.TUMKodu.Value);

                    // T ile başlayan teklif numarası girildi ise TUMTeklifNo alanında arama yapacağız
                    if (_Aktif.ProjeKodu == TVMProjeKodlari.Mapfre && !String.IsNullOrEmpty(arama.TeklifNo) && arama.TeklifNo.StartsWith("T"))
                    {
                        string tempTeklifNo = arama.TeklifNo;
                        TUMTeklifler = TUMTeklifler.Where(w => w.TUMTeklifNo == tempTeklifNo);

                        arama.TeklifNo = String.Empty;
                    }

                    teklifler = teklifler.Where(s => s.TUMKodu == 0);
                }
                else { teklifler = teklifler.Where(s => s.TUMKodu == 0); }
            }

            if (!String.IsNullOrEmpty(arama.TeklifNo))
            {
                int teklifNo = 0;
                if (int.TryParse(arama.TeklifNo, out teklifNo))
                {
                    teklifler = teklifler.Where(w => w.TeklifNo == teklifNo);
                }
            }

            //TVM KODU
            if (arama.TVMKodu.HasValue && arama.TVMKodu.Value != 0)
            {
                bool yetkilimi = _TVMService.KullaniciTvmyiGormeyeYetkiliMi(arama.TVMKodu.Value);
                if (yetkilimi)
                {
                    teklifler = teklifler.Where(w => w.TVMKodu == arama.TVMKodu.Value);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TVMKodu == arama.TVMKodu.Value);
                }
                else
                {
                    teklifler = teklifler.Where(w => w.TVMKodu == _Aktif.TVMKodu);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TVMKodu == _Aktif.TVMKodu);
                }
            }
            else
            {
                if (_Aktif.TVMKodu == NeosinerjiTVM.AegonTVMKodu)
                {
                    IQueryable<TVMDetay> tvmler = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == _Aktif.TVMKodu || s.Kodu == _Aktif.TVMKodu);

                    teklifler = from t in teklifler
                                join tvm in tvmler on t.TVMKodu equals tvm.Kodu
                                select t;
                }
                else
                {
                    teklifler = teklifler.Where(w => w.TVMKodu == _Aktif.TVMKodu);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TVMKodu == _Aktif.TVMKodu);
                }
            }


            if (arama.UrunKodu.HasValue)
            {
                if (arama.UrunKodu.Value > 0)
                {
                    teklifler = teklifler.Where(w => w.UrunKodu == arama.UrunKodu.Value);
                    TUMTeklifler = TUMTeklifler.Where(w => w.UrunKodu == arama.UrunKodu.Value);
                }
            }

            if (arama.HazirlayanKodu.HasValue)
            {
                if (arama.HazirlayanKodu.Value > 0)
                {
                    teklifler = teklifler.Where(w => w.TVMKullaniciKodu == arama.HazirlayanKodu.Value);
                    TUMTeklifler = TUMTeklifler.Where(w => w.TVMKullaniciKodu == arama.HazirlayanKodu.Value);
                }
            }

            if (arama.TeklifDurumu.HasValue)
            {
                if (arama.TeklifDurumu.Value > 0)
                {
                    switch (arama.TeklifDurumu.Value)
                    {
                        case 2:
                            teklifler = teklifler.Where(w => w.GecerlilikBitisTarihi >= TurkeyDateTime.Now);
                            break;
                        case 3:
                            teklifler = teklifler.Where(w => w.GecerlilikBitisTarihi < TurkeyDateTime.Now);
                            break;
                        default:
                            break;
                    }
                }
            }


            // ==== Müşteri filitrelemesi ==== //
            if (arama.MusteriKodu.HasValue && arama.TVMKodu.HasValue)
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(arama.MusteriKodu.Value);

                if (musteri != null)
                {
                    IQueryable<TeklifSigortaEttiren> sigortaEttiren = _TeklifContext.TeklifSigortaEttirenRepository.Filter(s => s.MusteriKodu == musteri.MusteriKodu);

                    teklifler = from se in sigortaEttiren
                                join t in teklifler on se.TeklifId equals t.TeklifId
                                select t;

                    TUMTeklifler = from se in sigortaEttiren
                                   join t in TUMTeklifler on se.TeklifId equals t.TeklifId
                                   select t;
                }
            }

            if (teklifMi && _Aktif.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                teklifler = (from t in teklifler
                             join tt in TUMTeklifler on t.TeklifNo equals tt.TeklifNo
                             select t);
            }

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = teklifler.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            //Mapfre sigorta teklif listesi
            if (teklifMi && _Aktif.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                var sigortaEttirenTablo = _TeklifContext.TeklifSigortaEttirenRepository.All();

                listeModel = (from t in teklifler
                              join tt in TUMTeklifler on t.TeklifNo equals tt.TeklifNo
                              join ts in sigortaEttirenTablo on t.TeklifId equals ts.TeklifId
                              select new TeklifAramaTableModel()
                              {
                                  TeklifId = t.TeklifId,
                                  TeklifNo = t.TeklifNo,
                                  TUMTeklifNo = tt.TUMTeklifNo,
                                  TVMKodu = t.TVMKodu,
                                  TVMUnvani = t.TVMDetay.Unvani,
                                  TVMKullaniciKodu = t.TVMKullaniciKodu,
                                  TVMKullaniciAdSoyad = t.TVMKullanicilar.Adi + " " + t.TVMKullanicilar.Soyadi,
                                  UrunKodu = t.UrunKodu,
                                  TanzimTarihi = t.TanzimTarihi,
                                  PdfURL = t.PDFDosyasi,
                                  KayitTarihi = t.KayitTarihi,
                                  Otorizasyon = t.Otorizasyon.HasValue && t.Otorizasyon.Value == 1,
                                  MusteriAdSoyad = ts.MusteriGenelBilgiler.AdiUnvan + " " + ts.MusteriGenelBilgiler.SoyadiUnvan,
                                  MusteriKodu = ts.MusteriKodu
                              })
                             .OrderByDescending(o => o.TeklifId)
                             .Skip(excludedRows)
                             .Take(arama.PageSize)
                             .ToList();

                foreach (var item in listeModel)
                {
                    item.UrunAdi = _ParametreContext.UrunRepository.FindById(item.UrunKodu).UrunAdi;
                }

                return listeModel;
            }

            var list = (from k in teklifler
                        select k).OrderByDescending(o => o.TeklifId)
                       .Skip(excludedRows)
                       .Take(arama.PageSize)
                       .ToList();

            foreach (var item in list)
            {
                TeklifAramaTableModel model = new TeklifAramaTableModel();

                model.TeklifId = item.TeklifId;
                model.TeklifNo = item.TeklifNo;
                model.TUMTeklifNo = item.TUMTeklifNo;
                model.TVMKodu = item.TVMKodu;
                model.TVMUnvani = item.TVMDetay.Unvani;
                model.TVMKullaniciKodu = item.TVMKullaniciKodu;
                model.TVMKullaniciAdSoyad = item.TVMKullanicilar.Adi + " " + item.TVMKullanicilar.Soyadi;
                var res = _TVMService.GetDetay((int)item.KaydiEKleyenTVMKodu);
                model.KaydiEKleyenTVMKodu = item.KaydiEKleyenTVMKodu.ToString() + " " + res.Unvani;
                model.UrunKodu = item.UrunKodu;
                model.UrunAdi = _ParametreContext.UrunRepository.FindById(item.UrunKodu).UrunAdi;
                model.TanzimTarihi = item.TanzimTarihi;
                model.PdfURL = item.PDFDosyasi;
                model.KayitTarihi = item.KayitTarihi;
                model.Otorizasyon = item.Otorizasyon.HasValue && item.Otorizasyon.Value == 1;

                if (!teklifMi)
                {
                    TeklifGenel Anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == item.TeklifNo &
                                                                                             s.TUMKodu == 0 &
                                                                                             s.TVMKodu == item.TVMKodu).FirstOrDefault();
                    if (Anateklif != null)
                    {
                        model.AnaTeklifId = Anateklif.TeklifId;
                        model.AnaTeklifPDF = Anateklif.PDFDosyasi;
                    }

                    model.TUMPoliceNo = item.TUMPoliceNo;
                    model.PoliceBitisTarihi = item.BitisTarihi;
                    TUMDetay tum = tumler.Where(s => s.Kodu == item.TUMKodu).FirstOrDefault();
                    if (tum != null)
                        model.TUMUnvani = tum.Unvani;

                    model.PdfURL = item.PDFPolice;

                    var ingilizcePdf = item.TeklifDokumans.FirstOrDefault();
                    if (ingilizcePdf != null)
                    {
                        model.İngilizcePdfURL = ingilizcePdf.DokumanURL;
                    }

                    //AEGON URUNLERI
                    if (_Aktif.ProjeKodu == TVMProjeKodlari.Aegon)
                    {
                        TeklifProvizyon provizyon = item.TeklifProvizyons.FirstOrDefault(s => s.OnayKodu != null);
                        if (provizyon != null)
                        {
                            model.OzelAlan = String.Format("Onay Kodu = {0}", provizyon.OnayKodu);
                        }
                    }
                }


                // === Müşteri bilgileri alınıyor === //
                TeklifSigortaEttiren musteri = item.TeklifSigortaEttirens.Where(s => s.TeklifId == item.TeklifId).FirstOrDefault();
                if (musteri != null)
                {
                    model.MusteriAdSoyad = musteri.MusteriGenelBilgiler.AdiUnvan + " " + musteri.MusteriGenelBilgiler.SoyadiUnvan;
                    model.MusteriKodu = musteri.MusteriKodu;
                }

                listeModel.Add(model);
            }

            return listeModel;
        }

        public List<TeklifAramaTableModel1> PagedList1(TeklifListe1 arama, out int totalRowCount, bool teklifMi)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            bool pTeklifMi = teklifMi;
            bool pMapfreMi = _Aktif.ProjeKodu == TVMProjeKodlari.Mapfre ? true : false;
            bool pMapfreTeklifKontrol = false;
            if (pMapfreMi && !string.IsNullOrEmpty(arama.TeklifNo) && arama.TeklifNo.StartsWith("T"))
                pMapfreTeklifKontrol = true;
            DateTime? pBaslangicTarihi = arama.BaslangisTarihi;
            DateTime? pBitisTarihi = arama.BitisTarihi;
            string pKulProjeKodu = _Aktif.ProjeKodu;
            int? pTUMKodu = arama.TUMKodu;
            string pPoliceNo = arama.PoliceNo;
            string pTeklifNo = arama.TeklifNo;
            int? pTVMKodu = arama.TVMKodu;
            bool pTVMYetkilimi = _TVMService.KullaniciTvmyiGormeyeYetkiliMi(arama.TVMKodu.Value);
            int pKulTVMKodu = _Aktif.TVMKodu;
            int? pUrunKodu = arama.UrunKodu;
            int? pHazirlayanKodu = arama.HazirlayanKodu;
            int? pTeklifDurumu = arama.TeklifDurumu;
            int? pMusteriKodu = arama.MusteriKodu;
            DateTime pTDNow = TurkeyDateTime.Now;
            int pPageSize = arama.PageSize <= 0 ? 10 : arama.PageSize;
            int pPage = arama.Page;
            return _ProcedurContext.PoliceAra(pTeklifMi, pMapfreMi, pMapfreTeklifKontrol, pBaslangicTarihi, pBitisTarihi, pKulProjeKodu, pTUMKodu, pPoliceNo, pTeklifNo, pTVMKodu, pTVMYetkilimi, pKulTVMKodu, pUrunKodu, pHazirlayanKodu
                , pTeklifDurumu, pMusteriKodu, pTDNow, pPageSize, pPage, out totalRowCount);
        }

        //---Karşılaştırmalı Ortam TeklifAra
        public List<TeklifAraProcedureModel> TeklifAraPageList(TeklifAraListe arama, out int totalRowCount)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();


            if (arama.PageSize <= 0) arama.PageSize = 10;
            if (arama.Page <= 0) arama.Page = 1;
            int excludedRows = (arama.Page - 1) * arama.PageSize;

            arama.skip = excludedRows;
            arama.take = arama.PageSize;

            List<TeklifAraProcedureModel> raporlist = _TUMContext.TeklifAra_Getir(arama.BaslangisTarihi, arama.BitisTarihi, arama.TeklifNo, arama.TVMKodu, arama.MusteriKodu, arama.UrunKodu, arama.HazirlayanKodu, arama.TUMKodu,
                                               arama.TeklifDurumu, _Aktif.TVMKodu, arama.skip, arama.take);

            totalRowCount = 0;

            foreach (var item in raporlist)
            {
                totalRowCount = item.Total_Rows;
            }

            return raporlist;

        }

        //---Aegon TeklifAra
        public List<AegonTeklifAraProcedureModel> Aegon_TeklifAraPageList(AegonTeklifAraListe arama, out int totalRowCount)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();


            if (arama.PageSize <= 0) arama.PageSize = 10;
            if (arama.Page <= 0) arama.Page = 1;
            int excludedRows = (arama.Page - 1) * arama.PageSize;

            arama.skip = excludedRows;
            arama.take = arama.PageSize;

            List<AegonTeklifAraProcedureModel> raporlist = _TUMContext.Aegon_TeklifAra_Getir(arama.BaslangisTarihi, arama.BitisTarihi, arama.TeklifNo, arama.TVMKodu, arama.MusteriKodu, arama.UrunKodu, arama.HazirlayanKodu, _Aktif.TVMKodu, arama.skip, arama.take);

            totalRowCount = 0;

            foreach (var item in raporlist)
            {
                totalRowCount = item.Total_Rows;
            }

            return raporlist;

        }

        //---Mapfre TeklifAra
        public List<MapfreTeklifAraProcedureModel> Mapfre_TeklifAraPageList(MapfreTeklifAraListe arama, out int totalRowCount)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();


            if (arama.PageSize <= 0) arama.PageSize = 10;
            if (arama.Page <= 0) arama.Page = 1;
            int excludedRows = (arama.Page - 1) * arama.PageSize;

            arama.skip = excludedRows;
            arama.take = arama.PageSize;

            List<MapfreTeklifAraProcedureModel> raporlist = _TUMContext.Mapfre_TeklifAra_Getir(arama.BaslangisTarihi, arama.BitisTarihi, arama.TeklifNo, arama.TUMTeklifNo, arama.TVMKodu, arama.MusteriKodu, arama.UrunKodu, arama.HazirlayanKodu, arama.TeklifDurumu, _Aktif.TVMKodu, arama.skip, arama.take);

            totalRowCount = 0;

            foreach (var item in raporlist)
            {
                totalRowCount = item.Total_Rows;
            }

            return raporlist;

        }

        public List<TeklifAramaTableModel> TeklifRaporuPageList(TeklifListe arama, out int totalRowCount, bool teklifMi)
        {
            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.All();
            IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
            List<TUMDetay> tumler = _TUMContext.TUMDetayRepository.All().ToList<TUMDetay>();

            if (arama.BaslangisTarihi.HasValue)
                teklifler = teklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);

            if (arama.BitisTarihi.HasValue)
                teklifler = teklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);



            List<TeklifAramaTableModel> model = new List<TeklifAramaTableModel>();

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = teklifler.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            return model;
        }

        public List<TeklifOzelDetay> GetMusteriTeklifleri(int MusteriKodu, DateTime? baslangicTarihi, DateTime? bitisTarihi, byte tarihTipi)
        {
            IQueryable<TeklifSigortaEttiren> sigortaEttiren = _TeklifContext.TeklifSigortaEttirenRepository.Filter(s => s.MusteriKodu == MusteriKodu);
            IQueryable<Urun> urunler = _ParametreContext.UrunRepository.All();
            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMKodu == 0);

            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-1);
            else
                baslangicTarihi = baslangicTarihi.Value.AddDays(-1);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now;
            else
                bitisTarihi = bitisTarihi.Value.AddDays(1);


            if (tarihTipi == 1)
                teklifler = teklifler.Where(s => s.TanzimTarihi > baslangicTarihi && s.TanzimTarihi < bitisTarihi);
            else if (tarihTipi == 2)
                teklifler = teklifler.Where(s => s.BaslamaTarihi > baslangicTarihi && s.BaslamaTarihi < bitisTarihi);
            else if (tarihTipi == 3)
                teklifler = teklifler.Where(s => s.BitisTarihi > baslangicTarihi && s.BitisTarihi < bitisTarihi);

            var list = (from u in urunler
                        join t in teklifler on u.UrunKodu equals t.UrunKodu
                        join s in sigortaEttiren on t.TeklifId equals s.TeklifId
                        select new
                        {
                            u.UrunKodu,
                            u.UrunAdi,
                            t.TeklifNo,
                            t.TanzimTarihi,
                            t.TVMKodu,
                            t.TeklifId,
                            s.MusteriGenelBilgiler.AdiUnvan,
                            s.MusteriGenelBilgiler.SoyadiUnvan,
                            t.TVMDetay.Unvani,
                            t.TVMKullanicilar.Adi,
                            t.TVMKullanicilar.Soyadi,
                            t.KaydiEKleyenTVMKodu,
                            t.GecerlilikBitisTarihi,
                            //t.KaydedenTVMUnvani,
                            t.PDFDosyasi,
                            t.TUMTeklifNo,
                            t.TUMPoliceNo,
                            t.BaslamaTarihi,
                            t.BitisTarihi
                        }).OrderByDescending(o => o.TeklifId);

            List<TeklifOzelDetay> model = new List<TeklifOzelDetay>();
            foreach (var item in list)
            {
                TeklifOzelDetay teklif = new TeklifOzelDetay();

                teklif.TUMPoliceNo = item.TUMPoliceNo;
                teklif.BaslangicTarihi = item.BaslamaTarihi;
                teklif.BitisTarihi = item.BitisTarihi;
                teklif.TanzimTarihi = item.TanzimTarihi;
                teklif.TeklifNo = item.TeklifNo;
                teklif.TeklifId = item.TeklifId;
                teklif.TVMUnvani = item.Unvani;
                //teklif.KaydiEkleyenTVMKodu = item.KaydiEKleyenTVMKodu;
                teklif.KaydedenTVMUnvani = item.Unvani;
                teklif.MusteriAdiSoyadi = item.AdiUnvan + " " + item.SoyadiUnvan;
                teklif.UrunAdi = item.UrunAdi;
                teklif.TVMKullaniciAdSoyad = item.Adi + " " + item.Soyadi;
                teklif.UrunKodu = item.UrunKodu;
                teklif.GecerlilikBitisTarihi = item.GecerlilikBitisTarihi;
                teklif.Aktif = item.GecerlilikBitisTarihi > TurkeyDateTime.Today;
                teklif.DetayAdres = TeklifSayfaAdresleri.DetayAdres(item.UrunKodu);
                teklif.EkleAdres = TeklifSayfaAdresleri.EkleAdres(item.UrunKodu);
                teklif.PDFDosyasi = item.PDFDosyasi;
                teklif.TUMTeklifNo = item.TUMTeklifNo;
                model.Add(teklif);
            }
            return model;
        }

        public List<TeklifOzelDetay> GetMusteriPoliceleri(int MusteriKodu, DateTime? baslangicTarihi, DateTime? bitisTarihi, byte tarihTipi)
        {
            IQueryable<TeklifSigortaEttiren> sigortaEttiren = _TeklifContext.TeklifSigortaEttirenRepository.Filter(s => s.MusteriKodu == MusteriKodu);
            IQueryable<Urun> urunler = _ParametreContext.UrunRepository.All();
            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo != null && s.TeklifDurumKodu == 2 && s.TUMKodu != 0);
            IQueryable<TUMDetay> tumler = _TUMContext.TUMDetayRepository.All();

            if (!baslangicTarihi.HasValue)
                baslangicTarihi = TurkeyDateTime.Now.AddYears(-1);
            else
                baslangicTarihi = baslangicTarihi.Value.AddDays(-1);

            if (!bitisTarihi.HasValue)
                bitisTarihi = TurkeyDateTime.Now;
            else
                bitisTarihi = bitisTarihi.Value.AddDays(1);

            if (tarihTipi == 1)
                teklifler = teklifler.Where(s => s.TanzimTarihi > baslangicTarihi && s.TanzimTarihi < bitisTarihi);
            else if (tarihTipi == 2)
                teklifler = teklifler.Where(s => s.BaslamaTarihi > baslangicTarihi && s.BaslamaTarihi < bitisTarihi);
            else if (tarihTipi == 3)
                teklifler = teklifler.Where(s => s.BitisTarihi > baslangicTarihi && s.BitisTarihi < bitisTarihi);


            var list = (from u in urunler
                        join t in teklifler on u.UrunKodu equals t.UrunKodu
                        join s in sigortaEttiren on t.TeklifId equals s.TeklifId
                        join tum in tumler on t.TUMKodu equals tum.Kodu
                        select new
                        {
                            u.UrunKodu,
                            u.UrunAdi,
                            t.TeklifNo,
                            t.TanzimTarihi,
                            t.TVMKodu,
                            t.TeklifId,
                            s.MusteriGenelBilgiler.AdiUnvan,
                            s.MusteriGenelBilgiler.SoyadiUnvan,
                            t.TVMDetay.Unvani,
                            t.TVMKullanicilar.Adi,
                            t.TVMKullanicilar.Soyadi,
                            t.GecerlilikBitisTarihi,
                            t.PDFPolice,
                            TUMUnvani = tum.Unvani,
                            t.TUMTeklifNo,
                            t.TUMPoliceNo,
                            t.BaslamaTarihi,
                            t.BitisTarihi
                        }).OrderByDescending(o => o.TeklifId);

            List<TeklifOzelDetay> model = new List<TeklifOzelDetay>();
            foreach (var item in list)
            {
                TeklifOzelDetay police = new TeklifOzelDetay();

                police.BaslangicTarihi = item.BaslamaTarihi;
                police.BitisTarihi = item.BitisTarihi;
                police.TanzimTarihi = item.TanzimTarihi;
                police.TeklifNo = item.TeklifNo;
                police.TeklifId = item.TeklifId;
                police.TVMUnvani = item.Unvani;
                police.MusteriAdiSoyadi = item.AdiUnvan + " " + item.SoyadiUnvan;
                police.UrunAdi = item.UrunAdi;
                police.TVMKullaniciAdSoyad = item.Adi + " " + item.Soyadi;
                police.UrunKodu = item.UrunKodu;
                police.GecerlilikBitisTarihi = item.GecerlilikBitisTarihi;
                police.Aktif = item.GecerlilikBitisTarihi > TurkeyDateTime.Today;
                police.DetayAdres = TeklifSayfaAdresleri.PoliceAdres(item.UrunKodu);
                police.PDFDosyasi = item.PDFPolice;

                police.TUMUnvani = item.TUMUnvani;
                police.TUMTeklifNo = item.TUMTeklifNo;
                police.TUMPoliceNo = item.TUMPoliceNo;

                model.Add(police);
            }
            return model;
        }

        public List<TeklifOtorizasyonTableModel> GetOtorizasyonTeklifler(TeklifOtorizasyonListe arama, out int totalRowCount)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();

            List<TeklifOtorizasyonTableModel> listeModel = new List<TeklifOtorizasyonTableModel>();

            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.All();
            IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
            List<TUMDetay> tumler = _TUMContext.TUMDetayRepository.All().ToList<TUMDetay>();

            teklifler = teklifler.Where(w => w.TUMKodu == TeklifUretimMerkezleri.MAPFRE &&
                                             w.TVMKodu == arama.TVMKodu &&
                                             w.Otorizasyon == 1 &&
                                             w.TeklifDurumKodu == TeklifDurumlari.Teklif);
            if (arama.BaslangisTarihi.HasValue)
                teklifler = teklifler.Where(w => w.TanzimTarihi >= arama.BaslangisTarihi.Value);

            if (arama.BitisTarihi.HasValue)
                teklifler = teklifler.Where(w => w.TanzimTarihi <= arama.BitisTarihi.Value);

            if (arama.TeklifNo.HasValue)
                teklifler = teklifler.Where(w => w.TeklifNo == arama.TeklifNo.Value);

            if (!String.IsNullOrEmpty(arama.MapfreTeklifNo))
            {
                teklifler = teklifler.Where(w => w.TUMTeklifNo == arama.MapfreTeklifNo);
            }

            totalRowCount = teklifler.Count();

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in teklifler
                        select k)
                       .OrderByDescending(o => o.TeklifId)
                       .Skip(excludedRows)
                       .Take(arama.PageSize)
                       .ToList();

            foreach (var item in list)
            {
                TeklifOtorizasyonTableModel model = new TeklifOtorizasyonTableModel();

                model.TeklifId = item.TeklifId;
                model.TeklifNo = item.TeklifNo;
                model.TUMTeklifNo = item.TUMTeklifNo;
                model.TVMKodu = item.TVMKodu;
                model.TVMUnvani = item.TVMDetay.Unvani;
                model.TVMKullaniciKodu = item.TVMKullaniciKodu;
                model.TVMKullaniciAdSoyad = item.TVMKullanicilar.Adi + " " + item.TVMKullanicilar.Soyadi;
                model.UrunKodu = item.UrunKodu;
                model.UrunAdi = _ParametreContext.UrunRepository.FindById(item.UrunKodu).UrunAdi;
                model.TanzimTarihi = item.TanzimTarihi;

                // === Müşteri bilgileri alınıyor === //
                TeklifSigortaEttiren musteri = item.TeklifSigortaEttirens.Where(s => s.TeklifId == item.TeklifId).FirstOrDefault();
                if (musteri != null)
                {
                    model.MusteriAdSoyad = musteri.MusteriGenelBilgiler.AdiUnvan + " " + musteri.MusteriGenelBilgiler.SoyadiUnvan;
                    model.MusteriKodu = musteri.MusteriKodu;
                }

                listeModel.Add(model);
            }

            return listeModel;
        }

        public int GetTeklifUrunKodu(int teklifId)
        {
            int urunkodu = _TeklifContext.TeklifGenelRepository.Find(s => s.TeklifId == teklifId).UrunKodu;
            return urunkodu;
        }

        public long GetOfflinePoliceNo(int tvmKodu, int urunKodu)
        {
            return _TeklifContext.OfflinePoliceNumara(tvmKodu, urunKodu);
        }

        public IAktifKullaniciService _Aktif { get; set; }

        public HDIDASKEskiPoliceResponse EskiPoliceSorgula(string eskiPoliceNo)
        {
            HDIDASKEskiPoliceResponse model = new HDIDASKEskiPoliceResponse();

            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            model.Durum = "0";

            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { _Aktif.TVMKodu, TeklifUretimMerkezleri.HDI });

            string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_DaskServiceURL);

            if (servisKullanici != null && !String.IsNullOrEmpty(serviceURL))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(serviceURL);
                builder.Append("?");
                builder.Append("user=" + servisKullanici.KullaniciAdi);
                builder.Append("&pwd=" + servisKullanici.Sifre);
                builder.Append("&Uygulama=DASKWEB");
                builder.Append("&IstekTipi=E");
                builder.Append("&DASKPoliceNumara=" + eskiPoliceNo);

                Uri url = new Uri(builder.ToString());

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = 60000;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string xml = reader.ReadToEnd().ToString();

                    xml = xml.Trim();

                    xml = xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "")
                         .Replace("\r\n", "");

                    XmlSerializer xs = new XmlSerializer(typeof(HDIDASKEskiPoliceResponse));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(xml);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;
                        using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8))
                        {
                            model = (HDIDASKEskiPoliceResponse)xs.Deserialize(ms);
                        }
                    }
                }
            }

            return model;
        }

        public TeklifOzelAlan TeklifOzelAlan(int teklifId)
        {
            return _TeklifContext.TeklifOzelAlan(teklifId);
        }

        public string GetTeklifWebServisCevap(int TeklifId, int webServisCevapKodu)
        {
            string result = String.Empty;

            try
            {
                TeklifWebServisCevap cevap = _TeklifContext.TeklifWebServisCevapRepository.Filter(s => s.CevapKodu == webServisCevapKodu &&
                                                                                                       s.TeklifId == TeklifId).FirstOrDefault();
                if (cevap != null)
                    result = cevap.Cevap;
            }
            catch (Exception)
            {
            }

            return result;
        }

        public int SetIlgiliTeklifId(ITeklif teklif)
        {
            int teklifNo = 0;

            if (teklif.GenelBilgiler.IlgiliTeklifId.HasValue)
            {
                TeklifGenel t = _TeklifContext.TeklifGenelRepository.FindById(teklif.GenelBilgiler.IlgiliTeklifId.Value);
                if (t != null)
                {
                    t.IlgiliTeklifId = teklif.GenelBilgiler.TeklifId;
                    t.IlgiliTeklifNo = teklif.GenelBilgiler.TeklifNo;
                    t.IlgiliTeklifUrunKodu = teklif.GenelBilgiler.UrunKodu;
                    teklifNo = t.TeklifNo;

                    _TeklifContext.TeklifGenelRepository.Update(t);
                    _TeklifContext.Commit();
                }
            }

            return teklifNo;
        }

        //TEMINATLAR
        public TeklifTeminat GetTeklifTeminat(int TeklifId, int TeminatKodu)
        {
            TeklifTeminat teminat = new TeklifTeminat();

            try
            {
                teminat = _TeklifContext.TeklifTeminatRepository.Filter(s => s.TeminatKodu == TeminatKodu && s.TeklifId == TeklifId).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            return teminat;
        }

        public TeklifTeminat GetAnaTeklifTeminat(int teklifNo, int tvmkodu, int TeminatKodu)
        {
            TeklifTeminat teminat = new TeklifTeminat();

            try
            {
                IQueryable<TeklifGenel> anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmkodu);
                IQueryable<TeklifTeminat> teminats = _TeklifContext.TeklifTeminatRepository.Filter(s => s.TeminatKodu == TeminatKodu);

                teminat = (from a in anateklif
                           join t in teminats on a.TeklifId equals t.TeklifId
                           select t).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            return teminat;
        }

        //SORULAR
        public TeklifSoru GetTeklifSoru(int TeklifId, int SoruKodu)
        {
            TeklifSoru cevap = new TeklifSoru();

            try
            {
                cevap = _TeklifContext.TeklifSoruRepository.Filter(s => s.TeklifId == TeklifId && s.SoruKodu == SoruKodu).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            return cevap;
        }

        public TeklifSoru GetAnaTeklifSoru(int teklifNo, int tvmkodu, int SoruKodu)
        {
            TeklifSoru cevap = new TeklifSoru();

            try
            {
                IQueryable<TeklifGenel> anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmkodu);
                IQueryable<TeklifSoru> sorular = _TeklifContext.TeklifSoruRepository.Filter(s => s.SoruKodu == SoruKodu);

                cevap = (from a in anateklif
                         join s in sorular on a.TeklifId equals s.TeklifId
                         select s).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            return cevap;
        }

        #region AEGON

        public string AegonOnProvizyonParaBirimi(TeklifGenel teklif)
        {
            string result = String.Empty;

            try
            {
                if (teklif != null)
                {
                    TeklifSoru cevap;

                    #region SWİTCH

                    switch (teklif.UrunKodu)
                    {
                        case UrunKodlari.TESabitPrimli:

                            cevap = GetTeklifSoru(teklif.TeklifId, TESabitPrimliSorular.ParaBirimi);

                            if (cevap != null)
                            {
                                result = AegonParaBirimleri.ParaBirimiText(cevap.Cevap);
                            }
                            break;
                        case UrunKodlari.KorunanGelecek:

                            cevap = GetTeklifSoru(teklif.TeklifId, KorunanGelecekSorular.ParaBirimi);

                            if (cevap != null)
                            {
                                result = AegonParaBirimleri.ParaBirimiText(cevap.Cevap);
                            }
                            break;
                        case UrunKodlari.OdemeGuvence:

                            cevap = GetTeklifSoru(teklif.TeklifId, OdemeGuvenceSorular.ParaBirimi);

                            if (cevap != null)
                            {
                                result = AegonParaBirimleri.ParaBirimiText(cevap.Cevap);
                            }
                            break;
                        case UrunKodlari.KritikHastalik:

                            cevap = GetTeklifSoru(teklif.TeklifId, KritikHastalikSorular.ParaBirimi);

                            if (cevap != null)
                            {
                                result = AegonParaBirimleri.ParaBirimiText(cevap.Cevap);
                            }
                            break;
                        case UrunKodlari.OdulluBirikim: result = "TL"; break; //TEK PARA BİRİMİ VAR
                        case UrunKodlari.PrimIadeli: result = "USD"; break;
                        case UrunKodlari.Egitim: result = "USD"; break;
                        case UrunKodlari.PrimIadeli2: result = "USD"; break;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }

            return result;
        }

        public decimal AegonOnProvizyonTutar(TeklifGenel teklif, out int gercekTeklifId)
        {
            decimal result = 0;
            gercekTeklifId = 0;

            try
            {
                TeklifGenel gercekTeklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklif.TeklifNo &&
                                                                                            s.TVMKodu == teklif.TVMKodu &&
                                                                                            s.TUMKodu != 0).FirstOrDefault();

                if (gercekTeklif != null && gercekTeklif.NetPrim.HasValue)
                {
                    result = gercekTeklif.NetPrim.Value;
                    gercekTeklifId = gercekTeklif.TeklifId;
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }

            return result;
        }

        public bool AegonOnProvizyonKontrol(int teklifId)
        {
            try
            {
                TeklifGenel teklif = _TeklifContext.TeklifGenelRepository.FindById(teklifId);

                if (teklif != null)
                {
                    TeklifGenel gercekTeklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == teklif.TeklifNo &&
                                                                                                s.TVMKodu == teklif.TVMKodu &&
                                                                                                s.TUMKodu != 0).FirstOrDefault();

                    if (gercekTeklif != null && gercekTeklif.TeklifDurumKodu == TeklifDurumlari.Police)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }

            return false;
        }


        //AEGON ON PROVİZYON
        public bool AegonOnProvizyon(AegonOnProvizyonRequest request, TeklifGenel teklif, out string message)
        {
            bool sonuc = false;
            message = String.Empty;
            try
            {
                #region REQUEST

                //string pDovKodx = "TL";             //--hep ‘TL’ kullanınız (ürün doviz bazlı ise tlye cevirip tutar oyle girilsin)
                //string pKKNox = "5530561021100911"; //     --burası sabit bu deger olmalıdır. Canlıya gecince sizden geliyor olacak.
                //string pSKT_Ayx = "07";             //     --burası sabit bu deger olmalıdır. Canlıya gecince sizden geliyor olacak.
                //string pSKT_Yilx = "2015";          //     --burası sabit bu deger olmalıdır. Canlıya gecince sizden geliyor olacak.
                //string pCVVx = "284";               //     --burası sabit bu deger olmalıdır. Canlıya gecince sizden geliyor olacak.


                //request.pOdemeTurux = 3;                // --hep ‘3’ olmalıdır. 
                //request.pDovKodx = pDovKodx;
                //request.pKKNox = pKKNox;
                //request.pSKT_Ayx = pSKT_Ayx;
                //request.pSKT_Yilx = pSKT_Yilx;
                //request.pCVVx = pCVVx;

                #region Tutar SET

                int gercekTeklifId = 0;

                decimal tutar = AegonOnProvizyonTutar(teklif, out gercekTeklifId);

                request.gercekTeklifId = gercekTeklifId;
                request.tutarDEC = tutar;
                request.pTutarx = tutar.ToString("N2").Replace(",", "").Replace(".", "");

                #endregion

                #endregion

                #region RESPONSE | LOG

                IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();

                WEBServisLog log = new WEBServisLog();

                log.TeklifId = request.gercekTeklifId;
                log.IstekTarihi = TurkeyDateTime.Now;
                log.IstekTipi = WebServisIstekTipleri.OnProvizyon;

                #region Request Kayıt Kredi Kartı Temizle

                //Kredi Kartı - CCV - SKT   bilgileri logda tutulmuyor.
                string krediKatriBilgisi = request.pKKNox;
                string CCV = request.pCVVx;
                string SK_AY = request.pSKT_Ayx;
                string SK_YIL = request.pSKT_Yilx;

                //Kredi Kartının ilk 6 ve son 4 hanesi tutuluyor.
                string KK_Ilk6_Son4 = String.Format("{0}-{1}", request.pKKNox.Substring(0, 6), request.pKKNox.Substring(12, 4));

                request.pKKNox = KK_Ilk6_Son4;
                request.pCVVx = String.Empty;
                request.pSKT_Ayx = String.Empty;
                request.pSKT_Yilx = String.Empty;

                log.IstekUrl = storage.UploadXml("teklif", GetXml(request, request.GetType()));

                request.pKKNox = krediKatriBilgisi;
                request.pCVVx = CCV;
                request.pSKT_Ayx = SK_AY;
                request.pSKT_Yilx = SK_YIL;

                #endregion

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.AEGON_ServiceURL);
                aegon.Service1 servis = new aegon.Service1();
                servis.Url = serviceURL;

                result result = servis.onProvizyon(request.pUrunHaymerKodux, request.pTeklifNox, request.pPartajNox, request.pBasvuruNox,
                                                       request.pOdemeTurux, request.pDovKodx, request.pTCKx, request.pKKNox, request.pSKT_Ayx, request.pSKT_Yilx,
                                                       request.pCVVx, request.pTutarx, request.pProvTarx, AegonCommon.FirmaKisaAdi);


                #endregion

                #region SUCCESS

                log.CevapTarihi = TurkeyDateTime.Now;
                log.CevapUrl = storage.UploadXml("teklif", GetXml(result, result.GetType()));

                TeklifGenel gercekTeklif = _TeklifContext.TeklifGenelRepository.FindById(request.gercekTeklifId);

                if (gercekTeklif != null)
                {
                    if (result.response == "Approved")
                    {
                        gercekTeklif.TeklifDurumKodu = TeklifDurumlari.Police;

                        TeklifProvizyon provizyon = new TeklifProvizyon();

                        _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();

                        provizyon.BasvuruNumarasi = request.pBasvuruNox;
                        provizyon.KaydedenKullanici = _Aktif.KullaniciKodu;
                        provizyon.KayitTarihi = DateTime.Now;

                        provizyon.KrediKarti_IlkAltiSonDort = KK_Ilk6_Son4;
                        provizyon.OdemeTuru = request.pOdemeTurux;
                        provizyon.OdemeyiYapanTCKN = request.pTCKx;
                        provizyon.ParaBirimi = request.pParaBirimix;
                        provizyon.PartajNo = request.pPartajNox;
                        provizyon.ProvizyonTarihi = Convert.ToDateTime(request.pProvTarx);
                        provizyon.Tutar = request.tutarDEC;


                        //ONAY KODU
                        string[] array = result.errormsg.Split('|');
                        if (array.Count() == 2 && array[1].Length > 10)
                        {
                            array[1] = array[1].Replace(" ", "");
                            provizyon.OnayKodu = array[1].Substring(10);
                            message = provizyon.OnayKodu;
                        }

                        gercekTeklif.TeklifProvizyons.Add(provizyon);

                        //Log Kayıt
                        log.BasariliBasarisiz = WebServisBasariTipleri.Basarili;
                        gercekTeklif.WEBServisLogs.Add(log);

                        _TeklifContext.TeklifGenelRepository.Update(gercekTeklif);
                        _TeklifContext.Commit();
                        sonuc = true;
                    }
                    else
                    {
                        message = result.errormsg;
                        log.BasariliBasarisiz = WebServisBasariTipleri.Basarisiz;
                        _TeklifContext.WEBServisLogRepository.Create(log);
                        _TeklifContext.Commit();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }



            return sonuc;
        }

        public AegonOnProvizyonModelDetay AegonOnProvizyonDetay(int teklifId)
        {
            TeklifGenel anaTeklif = _TeklifContext.TeklifGenelRepository.FindById(teklifId);

            AegonOnProvizyonModelDetay model = new AegonOnProvizyonModelDetay();

            if (anaTeklif != null)
            {
                TeklifGenel teklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == anaTeklif.TeklifNo &&
                                                                                      s.TVMKodu == anaTeklif.TVMKodu &&
                                                                                      s.TUMKodu == TeklifUretimMerkezleri.AEGON).FirstOrDefault();

                if (teklif != null && teklif.TeklifDurumKodu == TeklifDurumlari.Police)
                {
                    TeklifProvizyon onProvizyon = teklif.TeklifProvizyons.FirstOrDefault();

                    if (onProvizyon != null)
                    {
                        model.paraBirimi = onProvizyon.ParaBirimi;
                        model.partajNo = onProvizyon.PartajNo;
                        model.tckn = onProvizyon.OdemeyiYapanTCKN;
                        model.tutar = onProvizyon.Tutar.ToString("N2");
                        model.basvuruNo = onProvizyon.BasvuruNumarasi;
                        model.onayKodu = onProvizyon.OnayKodu;
                        model.krediKarti = String.Format("{0} {1}** **** {2}", onProvizyon.KrediKarti_IlkAltiSonDort.Substring(0, 4),
                                                                               onProvizyon.KrediKarti_IlkAltiSonDort.Substring(4, 2),
                                                                               onProvizyon.KrediKarti_IlkAltiSonDort.Substring(7, 4));
                    }
                }
            }

            return model;
        }

        public string GetXml(object request, Type type)
        {
            string istek = String.Empty;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer s = new XmlSerializer(type);
                        s.Serialize(xmlWriter, request);
                    }

                    istek = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                _LogService = DependencyResolver.Current.GetService<ILogService>();
                _LogService.Error(ex);
            }

            return istek;
        }

        #endregion

        //METLIFE İŞ TAKİP
        public TeklifSoru CreateSoru(TeklifSoru soru)
        {
            TeklifSoru dkmn = _TeklifContext.TeklifSoruRepository.Create(soru);
            _TeklifContext.Commit();

            return dkmn;
        }

        public IsTakip CreateIsTakip(IsTakip isTakip)
        {
            isTakip = _TeklifContext.IsTakipRepository.Create(isTakip);
            _TeklifContext.Commit();
            return isTakip;
        }
        public void UpdateIsTakip(IsTakip isTakip)
        {
            _TeklifContext.IsTakipRepository.Update(isTakip);
            _TeklifContext.Commit();

        }
        public IsTakip GetIsTakip(int TeklifId, int MusteriKodu)
        {
            IsTakip isTakip = _TeklifContext.IsTakipRepository.Filter(s => s.TeklifId == TeklifId && s.TVMKullaniciId == MusteriKodu).FirstOrDefault();
            return isTakip;
        }
        public IsTakip GetIsTakip(int TeklifId)
        {
            IsTakip isTakip = _TeklifContext.IsTakipRepository.Filter(s => s.TeklifId == TeklifId).FirstOrDefault();
            return isTakip;
        }
        public IsTakip IsTakipGet(int IsTakipId)
        {
            IsTakip isTakip = _TeklifContext.IsTakipRepository.Filter(s => s.IsTakipId == IsTakipId).FirstOrDefault();
            return isTakip;
        }

        public IsTakipDetay CreateIsTakipDetay(IsTakipDetay isTakipDetay)
        {
            isTakipDetay = _TeklifContext.IsTakipDetayRepository.Create(isTakipDetay);
            _TeklifContext.Commit();
            return isTakipDetay;
        }
        public void UpdateIsTakipDetay(IsTakipDetay isTakipDetay)
        {
            _TeklifContext.IsTakipDetayRepository.Update(isTakipDetay);
            _TeklifContext.Commit();

        }
        public IsTakipDetay GetIsTakipDetay(int id)
        {
            IsTakipDetay isTakipDetay = _TeklifContext.IsTakipDetayRepository.Filter(s => s.IsTakipId == id).FirstOrDefault();
            return isTakipDetay;
        }

        public IsTakipIsTipleri CreateIsTakipIsTipleri(IsTakipIsTipleri isTakipIsTipleri)
        {
            IsTakipIsTipleri IsTakipIsTipleri = _TeklifContext.IsTakipIsTipleriRepository.Create(isTakipIsTipleri);
            _TeklifContext.Commit();
            return IsTakipIsTipleri;
        }

        public IsTakipIsTipleriDetay CreateIsTakipIsTipleriDetay(IsTakipIsTipleriDetay isTakipIsTipleriDetay)
        {
            IsTakipIsTipleriDetay IsTakipIsTipleriDetay = _TeklifContext.IsTakipIsTipleriDetayRepository.Create(isTakipIsTipleriDetay);
            _TeklifContext.Commit();
            return IsTakipIsTipleriDetay;
        }

        public IsTakipSoru CreateIsTakipSoru(IsTakipSoru isTakipSoru)
        {
            isTakipSoru = _TeklifContext.IsTakipSoruRepository.Create(isTakipSoru);
            _TeklifContext.Commit();
            return isTakipSoru;
        }
        public IsTakipSoru GetIsTakipSoru(int id)
        {
            IsTakipSoru isTakipSoru = _TeklifContext.IsTakipSoruRepository.Filter(s => s.IsTakipDetayId == id).FirstOrDefault();
            return isTakipSoru;
        }
        public void DeleteIsTakipSoru(int TeklifId)
        {
            _TeklifContext.IsTakipSoruRepository.Delete(m => m.TeklifId == TeklifId);
            _TeklifContext.Commit();
        }

        public IsTakipDokuman CreateIsTakipDokuman(IsTakipDokuman isTakipDokuman)
        {
            isTakipDokuman = _TeklifContext.IsTakipDokumanRepository.Create(isTakipDokuman);
            _TeklifContext.Commit();
            return isTakipDokuman;
        }
        public IsTakipDokuman GetIsTakipDokuman(int id)
        {
            IsTakipDokuman isTakipDokuman = _TeklifContext.IsTakipDokumanRepository.Filter(s => s.IsTakipId == id).FirstOrDefault();
            return isTakipDokuman;
        }
        public void DeleteIsTakipDokuman(int isTakipDokumanId)
        {
            _TeklifContext.IsTakipDokumanRepository.Delete(m => m.IsTakipDokumanId == isTakipDokumanId);
            _TeklifContext.Commit();
        }

        public IslerimListeModel GetIslerim(int TVMKullaniciKodu)
        {

            List<IsTakip> islerim;
            IQueryable<MusteriGenelBilgiler> musteri = _MusteriContext.MusteriGenelBilgilerRepository.All();
            IQueryable<TVMKullanicilar> kullanici = _TVMContext.TVMKullanicilarRepository.All();
            IslerimListeModel model = new IslerimListeModel();
            IsTakipKullaniciGrupKullanicilari kullaniciGrup = _TeklifContext.IsTakipKullaniciGrupKullanicilariRepository.Filter(s => s.KullaniciKodu == TVMKullaniciKodu).FirstOrDefault();

            if (kullaniciGrup.IsTakipKullaniciGrupId == MetlifeKullaniciGruplari.SatisEkibi)
            {
                islerim = _TeklifContext.IsTakipRepository.Filter(s => s.Asama == 1 || s.Asama == 3).ToList();
                var isler = (from i in islerim
                             join mus in musteri on i.MusteriKodu equals mus.MusteriKodu
                             join k in kullanici on i.TVMKullaniciId equals k.KullaniciKodu
                             select new
                             {
                                 i.TVMKullaniciId,
                                 i.TeklifId,
                                 i.KayitTarihi,
                                 i.Asama,
                                 i.IsTakipId,
                                 mus.AdiUnvan,
                                 mus.SoyadiUnvan,
                                 mus.MusteriKodu,
                                 k.Adi,
                                 k.Soyadi
                             }).ToList();

                model.Items = new List<IslerimModel>();

                foreach (var item in isler)
                {
                    IslerimModel mdl = new IslerimModel();
                    mdl.IsTakipId = item.IsTakipId;
                    mdl.KayitTarihi = item.KayitTarihi;
                    mdl.MusteriAdiSoyadi = item.AdiUnvan + " " + item.SoyadiUnvan;
                    mdl.IsTipi = "Sigorta Başvuru Süreci";
                    mdl.IsTipiDetay = MetlifeIsTipleri.MetlifeIsTipleriText(item.Asama.ToString());
                    mdl.Asama = item.Asama;
                    mdl.EkleyenKullanici = item.Adi + " " + item.Soyadi;
                    mdl.TeklifId = item.TeklifId;
                    mdl.MusteriKodu = item.MusteriKodu;
                    model.Items.Add(mdl);
                }
            }
            else
            {
                islerim = _TeklifContext.IsTakipRepository.Filter(s => s.Asama == 2 || s.Asama == 4).ToList();
                var isler = (from i in islerim
                             join mus in musteri on i.MusteriKodu equals mus.MusteriKodu
                             join k in kullanici on i.TVMKullaniciId equals k.KullaniciKodu
                             select new
                             {
                                 i.TVMKullaniciId,
                                 i.TeklifId,
                                 i.KayitTarihi,
                                 i.Asama,
                                 i.IsTakipId,
                                 mus.AdiUnvan,
                                 mus.SoyadiUnvan,
                                 k.Adi,
                                 k.Soyadi
                             }).ToList();
                model.Items = new List<IslerimModel>();

                foreach (var item in isler)
                {
                    IslerimModel mdl = new IslerimModel();
                    mdl.IsTakipId = item.IsTakipId;
                    mdl.KayitTarihi = item.KayitTarihi;
                    mdl.MusteriAdiSoyadi = item.AdiUnvan + " " + item.SoyadiUnvan;
                    mdl.IsTipi = "Sigorta Başvuru Süreci";
                    mdl.IsTipiDetay = MetlifeIsTipleri.MetlifeIsTipleriText(item.Asama.ToString());
                    mdl.Asama = item.Asama;
                    mdl.EkleyenKullanici = item.Adi + " " + item.Soyadi;
                    mdl.TeklifId = item.TeklifId;
                    model.Items.Add(mdl);
                }
            }

            return model;
        }

        public OnayladiklarimListeModel GetOnayladiklarim(int TVMKullaniciKodu)
        {
            IQueryable<IsTakip> islerim = _TeklifContext.IsTakipRepository.Filter(s => s.TVMKullaniciId == TVMKullaniciKodu);
            IQueryable<TVMKullanicilar> kullanici = _TVMContext.TVMKullanicilarRepository.All();
            OnayladiklarimListeModel model = new OnayladiklarimListeModel();

            if (islerim != null)
            {
                var isler = (from id in islerim
                             join k in kullanici on id.TVMKullaniciId equals k.KullaniciKodu
                             select new
                             {
                                 id.TeklifId,
                                 id.IsTakipId,
                                 id.TVMKullaniciId,
                                 id.KayitTarihi,
                                 id.Asama,
                                 k.Adi,
                                 k.Soyadi
                             }).Distinct().ToList();

                model.Items = new List<OnayladiklarimModel>();

                foreach (var item in isler)
                {
                    OnayladiklarimModel mdl = new OnayladiklarimModel();
                    mdl.IsTakipId = item.IsTakipId;
                    mdl.KayitTarihi = item.KayitTarihi;
                    mdl.Asama = MetlifeIsTipleri.MetlifeIsTipleriText(item.Asama.ToString());
                    mdl.EkleyenKullanici = item.Adi + " " + item.Soyadi;
                    mdl.TeklifId = item.TeklifId;

                    model.Items.Add(mdl);
                }
                return model;
            }
            return null;
        }

        public List<IsTakipDokuman> GetListDokumanlar(int IsTakipId)
        {

            var dokumanlar = _TeklifContext.IsTakipDokumanRepository.Filter(s => s.IsTakipId == IsTakipId).ToList<IsTakipDokuman>();
            return dokumanlar;
        }

        public List<IsTakipDetay> GetListTarihceler(int teklifId)
        {
            IsTakip isTakip = _TeklifContext.IsTakipRepository.Filter(s => s.TeklifId == teklifId).FirstOrDefault();
            if (isTakip != null)
            {
                var tarihceler = _TeklifContext.IsTakipDetayRepository.Filter(s => s.IsTakipId == isTakip.IsTakipId).ToList<IsTakipDetay>();
                return tarihceler;
            }
            return null;
        }

        public IsTakipKullaniciGruplari GetIsTakipKullaniciGruplari(int kgid)
        {
            IsTakipKullaniciGruplari isTakipDokuman = _TeklifContext.IsTakipKullaniciGruplariRepository.Filter(s => s.KullaniciGrupId == kgid).FirstOrDefault();
            return isTakipDokuman;
        }

        public List<IsTakipDetayListeModel> IsTakipDetayPagedList(IsTakipDetayArama arama, out int totalRowCount)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            IQueryable<IsTakipDetay> isler = _TeklifContext.IsTakipDetayRepository.All();

            if (!String.IsNullOrEmpty(arama.IsNo))
            {
                int isNo = Convert.ToInt32(arama.IsNo);
                isler = isler.Where(w => w.IsTakipId == isNo);
            }

            if (!String.IsNullOrEmpty(arama.IsTipi))
            {
                int isTipi = Convert.ToInt32(arama.IsTipi);
                isler = isler.Where(w => w.IsTipiDetayId == isTipi);
            }

            if (!String.IsNullOrEmpty(arama.HareketTipi))
            {
                int hareketTipi = Convert.ToInt32(arama.HareketTipi);
                isler = isler.Where(w => w.HareketTipi == hareketTipi);
            }
            if (!String.IsNullOrEmpty(arama.BaslangicTarihi))
            {
                DateTime baslangicTarihi = Convert.ToDateTime(arama.BaslangicTarihi);
                isler = isler.Where(w => w.KayitTarihi > baslangicTarihi);
            }

            if (!String.IsNullOrEmpty(arama.BitisTarihi))
            {
                DateTime bitisTarihi = Convert.ToDateTime(arama.BitisTarihi);
                isler = isler.Where(w => w.KayitTarihi < bitisTarihi);
            }
            IQueryable<TVMKullanicilar> kullanicilar = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == 444);

            IQueryable<IsTakipDetay> query = from k in isler
                                             where k.IsTipiDetayId == 1 || k.IsTipiDetayId == 5
                                             select k;

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            query = query.OrderBy(o => o.IsTakipId).Skip(excludedRows).Take(arama.PageSize);

            List<IsTakipDetayListeModel> list = new List<IsTakipDetayListeModel>();

            foreach (var item in query)
            {
                IsTakipDetayListeModel mdl = new IsTakipDetayListeModel();

                TVMKullanicilar kullanici = kullanicilar.Where(s => s.KullaniciKodu == item.TvmKullaniciId).FirstOrDefault();
                mdl.KullaniciAdiSoyadi = kullanici.Adi + " " + kullanici.Soyadi;
                mdl.IsNo = item.IsTakipId;
                mdl.IsTipi = MetlifeIsTipleri.MetlifeIsTipleriText(item.IsTipiDetayId.ToString());
                mdl.HareketTipi = MetlifeHareketTipleri.MetlifeHareketTipleriText(item.HareketTipi.ToString());
                mdl.KayitTarihi = item.KayitTarihi.ToShortDateString();

                list.Add(mdl);

            }

            return list;
        }
        //---METLIFE İŞ TAKİP

        public List<PoliceSorguProcedurModel> PoliceSorgu(PoliceSorguListe arama)
        {
            IAktifKullaniciService _Aktifkullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            return _ProcedurContext.PoliceSorgu(_Aktifkullanici.TVMKodu, (int)arama.SorguTipi, arama.KimlikNo, arama.PlakaKodu, arama.PlakaNo, arama.TeklifMi, arama.PoliceMi);
        }

        public List<MeslekIndirimiKasko> GetMeslekList()
        {
            var list = _CRContext.MeslekIndirimiKaskoRepository.All().ToList<MeslekIndirimiKasko>();
            return list;
        }

        public CR_MeslekIndirimiKasko GetMeslekKod(string meslekKodu)
        {
            var meslek = _CRContext.CR_MeslekIndirimiKaskoRepository.All().Where(m => m.MeslekKodu == meslekKodu).FirstOrDefault();
            return meslek;
        }

        public CR_MeslekIndirimiKasko GetTUMMeslekKod(string meslekKodu, int tumKodu)
        {
            var meslek = _CRContext.CR_MeslekIndirimiKaskoRepository.All().Where(m => m.MeslekKodu == meslekKodu && m.TUMKodu == tumKodu).FirstOrDefault();
            return meslek;
        }
        public List<CR_MeslekIndirimiKasko> GetTUMMeslekList(int tumKodu)
        {
            var meslek = _CRContext.CR_MeslekIndirimiKaskoRepository.All().Where(m => m.TUMKodu == tumKodu).ToList();
            return meslek;
        }

        //Police Transfer ile gelen poliçeler için taliacente kodunu okumak için kullanılıyor

        public int GetTUMPoliceTvmKodu(string tumPoliceNo, string tumBirlikKodu, string tumUrunKodu, string tcknVkn)
        {
            int tvmKodu = 0;
            var tumDetay = _TUMContext.TUMDetayRepository.Find(m => m.BirlikKodu == tumBirlikKodu);
            var tumUrunleri = _TUMContext.TUMUrunleriRepository.Find(m => m.TUMUrunKodu == tumUrunKodu);
            if (tumDetay != null && tumUrunleri != null)
            {
                var teklif = _TeklifContext.TeklifGenelRepository.All().Where(m => m.TeklifDurumKodu == TeklifDurumlari.Police &&
                                                                                m.TUMPoliceNo == tumPoliceNo &&
                                                                                m.TUMKodu == tumDetay.Kodu &&
                                                                                m.UrunKodu == tumUrunleri.BABOnlineUrunKodu).FirstOrDefault();
                if (teklif != null)
                {
                    var sigortali = _TeklifContext.TeklifSigortaliRepository.Find(s => s.TeklifId == teklif.TeklifId);
                    if (sigortali != null)
                    {
                        if (sigortali.MusteriGenelBilgiler.KimlikNo == tcknVkn)
                        {
                            tvmKodu = teklif.KaydiEKleyenTVMKodu.HasValue? teklif.KaydiEKleyenTVMKodu.Value:0;
                        }
                    }
                }
            }
            return tvmKodu;
        }

        public List<KaskoYurticiTasiyiciKademeleri> getYuriciTasiyiciKademleri(string kullanimTarzi)
        {
            if (!String.IsNullOrEmpty(kullanimTarzi))
            {
                var parts = kullanimTarzi.Split('-');
                if (parts != null)
                {
                    string kullanimTarzi1 = parts[0].ToString();
                    string kullanimTarzi2 = parts[1].ToString();
                    var kademeler = _ParametreContext.KaskoYurticiTasiyiciKademeleriRepository.All().Where(s => s.KullanimTarziKodu == kullanimTarzi1 && s.KullanimTarziKodu2 == kullanimTarzi2).ToList<KaskoYurticiTasiyiciKademeleri>();
                    return kademeler;
                }
            }
            return null;
        }

        public List<KaskoTasinanYukKademeleri> getTasinanYukKademleri(string kullanimTarzi)
        {
            if (!String.IsNullOrEmpty(kullanimTarzi))
            {
                var parts = kullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarzi1 = parts[0].ToString();
                    string kullanimTarzi2 = parts[1].ToString();
                    var kademeler = _ParametreContext.KaskoTasinanYukKademeleriRepository.All().Where(s => s.KullanimTarziKodu == kullanimTarzi1 && s.KullanimTarziKodu2 == kullanimTarzi2).ToList<KaskoTasinanYukKademeleri>();
                    return kademeler;
                }
            }
            return null;
        }


        public bool CreateDigerSirketTeklif(TeklifDigerSirketler teklif)
        {
            bool result = false;
            try
            {
                var teklifDiger = this.getDigerTeklif(teklif.TeklifId, teklif.SigortaSirketKodu);
                if (teklifDiger != null)
                {
                    teklifDiger.TeklifId = teklif.TeklifId;
                    teklifDiger.SigortaSirketKodu = teklif.SigortaSirketKodu;
                    teklifDiger.KayitEdenKullaniciKodu = teklif.KayitEdenKullaniciKodu;
                    teklifDiger.HasarsizlikSurprim = teklif.HasarsizlikSurprim;
                    teklifDiger.BrutPrim = teklif.BrutPrim;
                    teklifDiger.KayitTarihi = teklif.KayitTarihi;
                    teklifDiger.KomisyonTutari = teklif.KomisyonTutari;
                    _TeklifContext.TeklifDigerSirketlerRepository.Update(teklifDiger);
                    _TeklifContext.Commit();
                }
                else
                {
                    _TeklifContext.TeklifDigerSirketlerRepository.Create(teklif);
                    _TeklifContext.Commit();
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                throw;
            }
            return result;
        }

        public TeklifDigerSirketler getDigerTeklif(int teklifId, int sigortaSirketKodu)
        {
            TeklifDigerSirketler teklif = _TeklifContext.TeklifDigerSirketlerRepository.Filter(s => s.TeklifId == teklifId && s.SigortaSirketKodu == sigortaSirketKodu).FirstOrDefault();
            return teklif;
        }

        public List<TeklifDigerSirketler> getDigerTeklifler(int teklifId)
        {
            List<TeklifDigerSirketler> teklifList = _TeklifContext.TeklifDigerSirketlerRepository.Filter(s => s.TeklifId == teklifId).ToList();
            return teklifList;
        }

        //Teklif Giriş Ekranında Plaka Bilgilerini sorgulamak için kullanılacak
        public TeklifArac getTeklifAracDetay(string plakaKodu, string plakaNo)
        {
            TeklifArac arac = new TeklifArac();
            arac = _TeklifContext.TeklifAracRepository.All().Where(w => w.PlakaKodu == plakaKodu && w.PlakaNo == plakaNo).FirstOrDefault();
            return arac;
        }

        public TeklifAracDetayModel getTeklifArac(string plakaKodu, string plakaNo)
        {
            TeklifAracDetayModel model = new TeklifAracDetayModel();
            model.teklifArac = _TeklifContext.TeklifAracRepository.All().Where(w => w.PlakaKodu == plakaKodu && w.PlakaNo == plakaNo).OrderByDescending(s => s.TeklifId).FirstOrDefault();
            if (model.teklifArac != null)
            {
                model.teklifSoru = _TeklifContext.TeklifSoruRepository.All().Where(w => w.TeklifId == model.teklifArac.TeklifId
                && (w.SoruKodu == KaskoSorular.Eski_Police_No
                || w.SoruKodu == KaskoSorular.Eski_Police_Yenileme_No
                || w.SoruKodu == KaskoSorular.Eski_Police_Acente_No
                || w.SoruKodu == KaskoSorular.Eski_Police_Sigorta_Sirketi
                )).ToList();
            }

            return model;
        }

        public List<KaskoHukuksalKorumaBedel> getHukuksalKorumaBedelList()
        {
            List<KaskoHukuksalKorumaBedel> list = new List<KaskoHukuksalKorumaBedel>();
            list = _TeklifContext.KaskoHukuksalKorumaBedelRepository.All().ToList();
            return list;
        }
        public decimal getHkKademesi(int id)
        {
            decimal bedel = 0;
            bedel = _TeklifContext.KaskoHukuksalKorumaBedelRepository.All().Where(w => w.Id == id).Select(s => s.Bedel).FirstOrDefault();
            return bedel;
        }

        public Cr_KaskoHukuksalKoruma getHukuksalKorumaBedel(int tumkodu, decimal bedel)
        {
            Cr_KaskoHukuksalKoruma detay = new Cr_KaskoHukuksalKoruma();
            detay = _TeklifContext.Cr_KaskoHukuksalKorumaRepository.All().Where(w => w.TUMKodu == tumkodu && (w.Bedel == bedel || w.Bedel > bedel)).OrderBy(o => o.Bedel).FirstOrDefault();
            return detay;
        }

        public TeklifSigortali getTeklifSigortali(int teklifId)
        {
            var sigortali = _TeklifContext.TeklifSigortaliRepository.All().Where(s => s.TeklifId == teklifId).FirstOrDefault();

            return sigortali;
        }
        public bool UpdateTeklifSigortali(TeklifSigortali sigortali)
        {
            try
            {
                _TeklifContext.TeklifSigortaliRepository.Update(sigortali);
                _TeklifContext.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool AddTeklifSigortali(TeklifSigortali sigortali)
        {
            try
            {
                _TeklifContext.TeklifSigortaliRepository.Create(sigortali);
                _TeklifContext.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public string HDIHukuksalKorumaKademesi(string kullanimTarziKodu, string kod2, decimal bedel)
        {
            var hdiKademe = _TeklifContext.HDIKaskoHukuksalKorumaBedelleriRepository.All().Where(w => w.KullanimTarziKodu == kullanimTarziKodu && w.Kod2 == kod2 && w.MotorluAracBedeli == bedel).FirstOrDefault();
            if (hdiKademe != null)
            {
                return hdiKademe.Kademe;
            }
            else
            {
                return "";
            }
        }

        public List<AnadoluKullanimTipSekil> GetAnadoluKullanimTipleri(string kullanimTarzi, byte SorguTipi)
        {
            List<AnadoluKullanimTipSekil> kullanimTipleri = new List<AnadoluKullanimTipSekil>();
            if (SorguTipi == AnadoluKullanimTipiSorguTipi.KullanimTarzi)
            {
                var kullanimTipi = _TeklifContext.AnadoluKullanimTipSekilRepository.All().Where(w => w.NeoOnlineKullanimTarzi == kullanimTarzi).FirstOrDefault();
                kullanimTipleri.Add(kullanimTipi);
            }
            else
            {
                kullanimTipleri = _TeklifContext.AnadoluKullanimTipSekilRepository.All().Where(w => w.KullanimTipi == kullanimTarzi).ToList();
            }
            return kullanimTipleri;
        }

        //Koru Sigorta Lilyum Ürünü
        #region Lilyum Kart
        public List<LilyumMusteriKartlari> GetMusteriLilyumKartlari(string kimlikNo)
        {
            List<LilyumMusteriKartlari> kartList = new List<LilyumMusteriKartlari>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            var musteriDetay = _MusteriService.GetMusteri(kimlikNo, _Aktif.TVMKodu);
            if (musteriDetay != null)
            {
                IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
                IQueryable<TeklifSigortali> sigortali = _TeklifContext.TeklifSigortaliRepository.Filter(s => s.MusteriKodu == musteriDetay.MusteriKodu);
                if (_Aktif.YetkiGrubu == 1188 || _Aktif.YetkiGrubu == 1168)
                {
                    IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo != null && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == _Aktif.TVMKodu);
                    IQueryable<TeklifGenel> anateklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifDurumKodu == 1 && s.TUMKodu == 0 && s.TVMKodu == _Aktif.TVMKodu);

                    var polList = (from t in teklifler
                                   join s in sigortali on t.TeklifId equals s.TeklifId
                                   join anaT in anateklifler on t.TeklifNo equals anaT.TeklifNo
                                   select new
                                   {
                                       t.TanzimTarihi,
                                       t.TVMKodu,
                                       t.TeklifId,
                                       s.MusteriGenelBilgiler.AdiUnvan,
                                       s.MusteriGenelBilgiler.SoyadiUnvan,
                                       s.MusteriGenelBilgiler.MusteriNots,
                                       anaT.TeklifSorus,
                                       t.PDFPolice,
                                       t.TUMPoliceNo,
                                       t.BaslamaTarihi,
                                       t.BitisTarihi,
                                       t.BrutPrim,
                                       t.TaksitSayisi,
                                       anaT.TeklifAracs
                                   }).OrderByDescending(o => o.TeklifId).ToList();
                    LilyumMusteriKartlari kartItem = null;
                    for (int i = polList.Count() - 1; i >= 0; i--)
                    {
                        kartItem = new LilyumMusteriKartlari();
                        kartItem.BaslangicTarihi = polList[i].BaslamaTarihi;
                        kartItem.BitisTarihi = polList[i].BitisTarihi;
                        kartItem.MusteriAdiSoyadi = polList[i].AdiUnvan + " " + polList[i].SoyadiUnvan;
                        kartItem.ReferansNo = polList[i].TUMPoliceNo;
                        kartItem.KimlikNo = kimlikNo;
                        kartItem.BrutPrim = polList[i].BrutPrim;
                        kartItem.TaksitSayisi = polList[i].TaksitSayisi;
                        string refNo = polList[i].TUMPoliceNo;
                        var getKartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.TvmKodu == _Aktif.TVMKodu && s.ReferansNo == refNo).Select(s => s.LilyumKartNo).FirstOrDefault();

                        kartItem.KartNo = getKartNo;
                        kartItem.KartValue = kartItem.ReferansNo + " / " + getKartNo;
                        var KonutAdresi = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                        var KonutIl = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                        var KonutIlce = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                        if (KonutAdresi != null)
                        {
                            kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                        }
                        if (KonutIl != null && KonutIlce != null)
                        {
                            kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                            kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                            string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                            string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                            kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                        }
                        var aracPlaka = polList[i].TeklifAracs.FirstOrDefault();
                        if (aracPlaka != null)
                        {
                            if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                            {
                                kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                            }
                        }
                        kartList.Add(kartItem);
                    }
                }
                else
                {
                    IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo != null && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == _Aktif.TVMKodu && s.TVMKullaniciKodu == _Aktif.KullaniciKodu);
                    IQueryable<TeklifGenel> anateklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifDurumKodu == 1 && s.TUMKodu == 0 && s.TVMKodu == _Aktif.TVMKodu && s.TVMKullaniciKodu == _Aktif.KullaniciKodu);

                    var polList = (from t in teklifler
                                   join s in sigortali on t.TeklifId equals s.TeklifId
                                   join anaT in anateklifler on t.TeklifNo equals anaT.TeklifNo
                                   select new
                                   {
                                       t.TanzimTarihi,
                                       t.TVMKodu,
                                       t.TeklifId,
                                       s.MusteriGenelBilgiler.AdiUnvan,
                                       s.MusteriGenelBilgiler.SoyadiUnvan,
                                       s.MusteriGenelBilgiler.MusteriNots,
                                       anaT.TeklifSorus,
                                       t.PDFPolice,
                                       t.TUMPoliceNo,
                                       t.BaslamaTarihi,
                                       t.BitisTarihi,
                                       t.BrutPrim,
                                       t.TaksitSayisi,
                                       anaT.TeklifAracs
                                   }).OrderByDescending(o => o.TeklifId).ToList();
                    LilyumMusteriKartlari kartItem = null;
                    for (int i = polList.Count() - 1; i >= 0; i--)
                    {
                        kartItem = new LilyumMusteriKartlari();
                        kartItem.BaslangicTarihi = polList[i].BaslamaTarihi;
                        kartItem.BitisTarihi = polList[i].BitisTarihi;
                        kartItem.MusteriAdiSoyadi = polList[i].AdiUnvan + " " + polList[i].SoyadiUnvan;
                        kartItem.ReferansNo = polList[i].TUMPoliceNo;
                        kartItem.KimlikNo = kimlikNo;
                        kartItem.BrutPrim = polList[i].BrutPrim;
                        kartItem.TaksitSayisi = polList[i].TaksitSayisi;
                        string refNo = polList[i].TUMPoliceNo;
                        var getKartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.TvmKodu == _Aktif.TVMKodu && s.KullaniciKodu == _Aktif.KullaniciKodu && s.ReferansNo == refNo).Select(s => s.LilyumKartNo).FirstOrDefault();

                        kartItem.KartNo = getKartNo;
                        kartItem.KartValue = kartItem.ReferansNo + " / " + getKartNo;
                        var KonutAdresi = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                        var KonutIl = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                        var KonutIlce = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                        if (KonutAdresi != null)
                        {
                            kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                        }
                        if (KonutIl != null && KonutIlce != null)
                        {
                            kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                            kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                            string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                            string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                            kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                        }
                        var aracPlaka = polList[i].TeklifAracs.FirstOrDefault();
                        if (aracPlaka != null)
                        {
                            if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                            {
                                kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                            }
                        }
                        kartList.Add(kartItem);
                    }
                }
            }


            return kartList;
        }
        public LilyumMusteriKartlari GetMusteriLilyumKartDetay(string kimlikNo, string referansNo, string tvmkodu, string kullanicikodu)
        {
            int tvmkoduInt = Convert.ToInt32(tvmkodu);
            int kullanicikoduInt = Convert.ToInt32(kullanicikodu);
            LilyumMusteriKartlari kartItem = new LilyumMusteriKartlari();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            var musteriDetay = _MusteriService.GetMusteri(kimlikNo, tvmkoduInt);
            if (musteriDetay != null && referansNo != null && referansNo != "")
            {
                IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
                TeklifGenel lilyumTeklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo == referansNo && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == tvmkoduInt && s.TVMKullaniciKodu == kullanicikoduInt).FirstOrDefault();
                TeklifGenel anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == lilyumTeklif.TeklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmkoduInt && s.TVMKullaniciKodu == kullanicikoduInt).FirstOrDefault();

                var kartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.ReferansNo == referansNo && s.TvmKodu == tvmkoduInt && s.KullaniciKodu == kullanicikoduInt).Select(s => s.LilyumKartNo).FirstOrDefault();

                kartItem.BaslangicTarihi = lilyumTeklif.BaslamaTarihi;
                kartItem.BitisTarihi = lilyumTeklif.BitisTarihi;
                kartItem.MusteriAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                kartItem.ReferansNo = referansNo;
                kartItem.KimlikNo = kimlikNo;
                kartItem.BrutPrim = lilyumTeklif.BrutPrim;
                kartItem.TaksitSayisi = lilyumTeklif.TaksitSayisi;
                kartItem.KartNo = kartNo;
                kartItem.iptal = lilyumTeklif.iptal;
                var KonutAdresi = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                var KonutIl = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                var KonutIlce = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                if (KonutAdresi != null)
                {
                    kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                }
                if (KonutIl != null && KonutIlce != null)
                {
                    kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                    kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                    string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                    string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                    kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                }
                var aracPlaka = anateklif.TeklifAracs.FirstOrDefault();
                if (aracPlaka != null)
                {
                    if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                    {
                        kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                    }
                }
            }
            else if (referansNo != null && referansNo != "")
            {
                IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
                TeklifGenel lilyumTeklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo == referansNo && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == tvmkoduInt && s.TVMKullaniciKodu == kullanicikoduInt).FirstOrDefault();
                TeklifGenel anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == lilyumTeklif.TeklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmkoduInt && s.TVMKullaniciKodu == kullanicikoduInt).FirstOrDefault();

                var kartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.ReferansNo == referansNo && s.TvmKodu == tvmkoduInt && s.KullaniciKodu == kullanicikoduInt).Select(s => s.LilyumKartNo).FirstOrDefault();

                kartItem.BaslangicTarihi = lilyumTeklif.BaslamaTarihi;
                kartItem.BitisTarihi = lilyumTeklif.BitisTarihi;
                //kartItem.MusteriAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                kartItem.MusteriAdiSoyadi = "";
                kartItem.ReferansNo = referansNo;
                kartItem.KimlikNo = kimlikNo;
                kartItem.BrutPrim = lilyumTeklif.BrutPrim;
                kartItem.TaksitSayisi = lilyumTeklif.TaksitSayisi;
                kartItem.KartNo = kartNo;
                kartItem.iptal = lilyumTeklif.iptal;
                var KonutAdresi = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                var KonutIl = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                var KonutIlce = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                if (KonutAdresi != null)
                {
                    kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                }
                if (KonutIl != null && KonutIlce != null)
                {
                    kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                    kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                    string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                    string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                    kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                }
                var aracPlaka = anateklif.TeklifAracs.FirstOrDefault();
                if (aracPlaka != null)
                {
                    if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                    {
                        kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                    }
                }
            }

            return kartItem;
        }

        public LilyumMusteriKartlari GetMusteriLilyumKartDetay(int tvmKodu, string referansNo)
        {
            LilyumMusteriKartlari kartItem = new LilyumMusteriKartlari();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();

            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            TeklifGenel lilyumTeklif = new TeklifGenel();
            TeklifGenel anateklif = new TeklifGenel();
            string kartNo = "";
            lilyumTeklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo == referansNo && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == tvmKodu).FirstOrDefault();
            if (lilyumTeklif != null)
            {
                anateklif = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifNo == lilyumTeklif.TeklifNo && s.TUMKodu == 0 && s.TVMKodu == tvmKodu).FirstOrDefault();
                kartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.ReferansNo == referansNo && s.TvmKodu == tvmKodu).Select(s => s.LilyumKartNo).FirstOrDefault();

                var teklifSigortali = anateklif.TeklifSigortalis.FirstOrDefault();
                if (teklifSigortali != null)
                {
                    var musteriDetay = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);
                    kartItem.MusteriAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                    kartItem.KimlikNo = musteriDetay.KimlikNo;
                }
                else
                {
                    kartItem.MusteriAdiSoyadi = "";
                    kartItem.KimlikNo = "";
                }

                kartItem.BaslangicTarihi = lilyumTeklif.BaslamaTarihi;
                kartItem.BitisTarihi = lilyumTeklif.BitisTarihi;

                kartItem.ReferansNo = referansNo;

                kartItem.BrutPrim = lilyumTeklif.BrutPrim;
                kartItem.TaksitSayisi = lilyumTeklif.TaksitSayisi;
                kartItem.KartNo = kartNo;
                kartItem.iptal = lilyumTeklif.iptal;
                var KonutAdresi = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                var KonutIl = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                var KonutIlce = anateklif.TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                if (KonutAdresi != null)
                {
                    kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                }
                if (KonutIl != null && KonutIlce != null)
                {
                    kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                    kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                    string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                    string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                    kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                }
                var aracPlaka = anateklif.TeklifAracs.FirstOrDefault();
                if (aracPlaka != null)
                {
                    if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                    {
                        kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                    }
                }

            }
            return kartItem;
        }

        public void LilyumKartTeminatKullanimCreate(LilyumTeminatKaydetModel kaydetModel, ref bool basarili, ref string mesaj)
        {
            try
            {
                LilyumKartTeminatKullanim lilyumKartTeminat = null;
                var teminatListesi = this.GetLilyumTeminatlar();
                for (int i = teminatListesi.Count - 1; i >= 0; i--)
                {
                    var item = teminatListesi[i];
                    lilyumKartTeminat = new LilyumKartTeminatKullanim();
                    lilyumKartTeminat.EkleyenKullanici = kaydetModel.KaydedenKullaniciKodu;
                    lilyumKartTeminat.KayitTarihi = TurkeyDateTime.Now;
                    lilyumKartTeminat.KullaniciKodu = kaydetModel.KullaniciKodu;
                    lilyumKartTeminat.TvmKodu = kaydetModel.TvmKodu;
                    lilyumKartTeminat.ReferansNo = kaydetModel.ReferansNo;
                    lilyumKartTeminat.TeminatId = item.TeminatId;
                    lilyumKartTeminat.ToplamKullanimHakkiAdet = item.KullanimHakki;
                    lilyumKartTeminat.ToplamKullanilanAdet = 0;
                    _TeklifContext.LilyumKartTeminatKullanimRepository.Create(lilyumKartTeminat);
                }
                _TeklifContext.Commit();
                basarili = true;
            }
            catch (Exception ex)
            {

                basarili = false;
                mesaj = ex.Message.ToString();
            }

        }
        public bool lilyumKartIptalEt(string referansNo)
        {
            try
            {
                TeklifGenel teklifgenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TUMPoliceNo == referansNo).FirstOrDefault();
                teklifgenel.iptal = true;
                _TeklifContext.TeklifGenelRepository.Update(teklifgenel);
                _TeklifContext.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public TeklifGenel getLilyumReferans(string referansNo)
        {
            TeklifGenel teklifgenel = new TeklifGenel();
            try
            {
                teklifgenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TUMPoliceNo == referansNo && (w.TVMKodu == 153 || w.TVMKodu == 153008)).FirstOrDefault();
                return teklifgenel;
            }
            catch (Exception ex)
            {
                return teklifgenel;
            }
        }
        public bool lilyumKartReferansGuncelle(string teklifId, string brut, string odemeSekli, string taksitSayisi, string referansNo)
        {
            try
            {
                int _teklifid = Convert.ToInt32(teklifId);
                var teklifgenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TeklifId == _teklifid && (w.TVMKodu == 153 || w.TVMKodu == 153008)).FirstOrDefault();
                teklifgenel.BrutPrim = Convert.ToDecimal(brut);
                teklifgenel.OdemeSekli = Convert.ToByte(odemeSekli);
                teklifgenel.TaksitSayisi = Convert.ToByte(taksitSayisi);
                teklifgenel.TUMPoliceNo = referansNo;
                _TeklifContext.TeklifGenelRepository.Update(teklifgenel);
                _TeklifContext.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public LilyumKartTeminatKullanim getLilyumTeminatKullanim(string referansNo)
        {
            LilyumKartTeminatKullanim teminatKul = new LilyumKartTeminatKullanim();
            try
            {
                teminatKul = _TeklifContext.LilyumKartTeminatKullanimRepository.All().Where(w => w.ReferansNo == referansNo).FirstOrDefault();
                return teminatKul;
            }
            catch (Exception ex)
            {
                return teminatKul;
            }
        }
        public List<LilyumKartTeminatKullanim> GetLilyumKullaniciTeminatKullanim(int tvmKodu, string referansNo)
        {
            List<LilyumKartTeminatKullanim> list = new List<LilyumKartTeminatKullanim>();
            if (_Aktif.YetkiGrubu == 1188 || _Aktif.YetkiGrubu == 1168)
            {
                list = _TeklifContext.LilyumKartTeminatKullanimRepository.All().Where(w => w.TvmKodu == tvmKodu && w.ReferansNo == referansNo).ToList();
            }
            else
            {
                list = _TeklifContext.LilyumKartTeminatKullanimRepository.All().Where(w => w.TvmKodu == tvmKodu && w.ReferansNo == referansNo).ToList();
            }
            return list;
        }
        public List<LilyumKartTeminatlar> GetLilyumTeminatlar()
        {
            List<LilyumKartTeminatlar> list = new List<LilyumKartTeminatlar>();
            list = _TeklifContext.LilyumKartTeminatlarRepository.All().ToList();

            return list;
        }

        public List<LilyumMusteriKartlari> GetAcenteKullaniciLilyumKartlari(int tvmKodu, string adSoyad)
        {

            List<LilyumMusteriKartlari> kartList = new List<LilyumMusteriKartlari>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            _MusteriContext = DependencyResolver.Current.GetService<IMusteriContext>();


            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo != null && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == tvmKodu);
            IQueryable<TeklifGenel> anateklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifDurumKodu == 1 && s.TUMKodu == 0 && s.TVMKodu == tvmKodu);
            //IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu && s.TVMKullaniciKodu == kullaniciKodu);
            var polList = (from t in teklifler
                           join anaT in anateklifler on t.TeklifNo equals anaT.TeklifNo
                           select new
                           {
                               t.TanzimTarihi,
                               t.TVMKodu,
                               t.TeklifId,
                               anaT.TeklifSigortalis,
                               anaT.TeklifSorus,
                               t.PDFPolice,
                               t.TUMPoliceNo,
                               t.BaslamaTarihi,
                               t.BitisTarihi,
                               t.BrutPrim,
                               t.TaksitSayisi,
                               anaT.TeklifAracs
                           }).OrderByDescending(o => o.TeklifId).ToList();
            LilyumMusteriKartlari kartItem = null;
            for (int i = polList.Count() - 1; i >= 0; i--)
            {
                kartItem = new LilyumMusteriKartlari();
                kartItem.BaslangicTarihi = polList[i].BaslamaTarihi;
                kartItem.BitisTarihi = polList[i].BitisTarihi;
                kartItem.ReferansNo = polList[i].TUMPoliceNo;
                kartItem.BrutPrim = polList[i].BrutPrim;
                kartItem.TaksitSayisi = polList[i].TaksitSayisi;

                var teklifSigortali = polList[i].TeklifSigortalis.FirstOrDefault();
                if (teklifSigortali != null)
                {
                    var musteriDetay = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);
                    if (musteriDetay != null)
                    {
                        kartItem.MusteriAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                        kartItem.KimlikNo = musteriDetay.KimlikNo;
                    }
                }

                string refNo = polList[i].TUMPoliceNo;
                var getKartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.TvmKodu == tvmKodu && s.ReferansNo == refNo).Select(s => s.LilyumKartNo).FirstOrDefault();

                kartItem.KartNo = getKartNo;
                kartItem.KartValue = kartItem.ReferansNo + " / " + getKartNo + "/" + kartItem.MusteriAdiSoyadi;
                var KonutAdresi = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                var KonutIl = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                var KonutIlce = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                if (KonutAdresi != null)
                {
                    kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                }
                if (KonutIl != null && KonutIlce != null)
                {
                    kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                    kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                    string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                    string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                    kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                }
                var aracPlaka = polList[i].TeklifAracs.FirstOrDefault();
                if (aracPlaka != null)
                {
                    if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                    {
                        kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                    }
                }
                if (kartItem.MusteriAdiSoyadi != null && adSoyad != null && adSoyad != "")
                {
                    if (kartItem.MusteriAdiSoyadi.ToLower().Replace("ı", "i").StartsWith(adSoyad.ToLower().Replace("ı", "i")))
                    {
                        kartList.Add(kartItem);
                    }
                }

            }
            return kartList;
        }


        public List<LilyumMusteriKartlari> GetAcenteKullaniciLilyumKartlariByTCKN(int tvmKodu, string tckn)
        {

            List<LilyumMusteriKartlari> kartList = new List<LilyumMusteriKartlari>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            _MusteriContext = DependencyResolver.Current.GetService<IMusteriContext>();


            IUlkeService _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            IQueryable<TeklifGenel> teklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TUMPoliceNo != null && s.TeklifDurumKodu == 2 && s.TUMKodu == TeklifUretimMerkezleri.KORU && s.TVMKodu == tvmKodu);
            IQueryable<TeklifGenel> anateklifler = _TeklifContext.TeklifGenelRepository.Filter(s => s.TeklifDurumKodu == 1 && s.TUMKodu == 0 && s.TVMKodu == tvmKodu);
            //IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu && s.TVMKullaniciKodu == kullaniciKodu);
            var polList = (from t in teklifler
                           join anaT in anateklifler on t.TeklifNo equals anaT.TeklifNo
                           select new
                           {
                               t.TanzimTarihi,
                               t.TVMKodu,
                               t.TeklifId,
                               anaT.TeklifSigortalis,
                               anaT.TeklifSorus,
                               t.PDFPolice,
                               t.TUMPoliceNo,
                               t.BaslamaTarihi,
                               t.BitisTarihi,
                               t.BrutPrim,
                               t.TaksitSayisi,
                               anaT.TeklifAracs
                           }).OrderByDescending(o => o.TeklifId).ToList();
            LilyumMusteriKartlari kartItem = null;
            for (int i = polList.Count() - 1; i >= 0; i--)
            {
                kartItem = new LilyumMusteriKartlari();
                kartItem.BaslangicTarihi = polList[i].BaslamaTarihi;
                kartItem.BitisTarihi = polList[i].BitisTarihi;
                kartItem.ReferansNo = polList[i].TUMPoliceNo;
                kartItem.BrutPrim = polList[i].BrutPrim;
                kartItem.TaksitSayisi = polList[i].TaksitSayisi;

                var teklifSigortali = polList[i].TeklifSigortalis.FirstOrDefault();
                if (teklifSigortali != null)
                {
                    var musteriDetay = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);
                    if (musteriDetay != null)
                    {
                        kartItem.MusteriAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                        kartItem.KimlikNo = musteriDetay.KimlikNo;
                    }
                }

                string refNo = polList[i].TUMPoliceNo;
                var getKartNo = _TeklifContext.LilyumKartTeminatKullanimRepository.Filter(s => s.TvmKodu == tvmKodu && s.ReferansNo == refNo).Select(s => s.LilyumKartNo).FirstOrDefault();

                kartItem.KartNo = getKartNo;
                kartItem.KartValue = kartItem.ReferansNo + " / " + getKartNo + "/" + kartItem.MusteriAdiSoyadi;
                var KonutAdresi = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KoruLilyumIletisimAdres).FirstOrDefault();
                var KonutIl = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu).FirstOrDefault();
                var KonutIlce = polList[i].TeklifSorus.Where(s => s.SoruKodu == LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu).FirstOrDefault();
                if (KonutAdresi != null)
                {
                    kartItem.MusteriKonutAdresi = KonutAdresi.Cevap;
                }
                if (KonutIl != null && KonutIlce != null)
                {
                    kartItem.MusteriKonutIlKodu = KonutIl.Cevap;
                    kartItem.MusteriKonutIlceKodu = KonutIlce.Cevap;
                    string ilAdi = _UlkeService.GetIlAdi("TUR", KonutIl.Cevap);
                    string ilceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(KonutIlce.Cevap));
                    kartItem.MusteriKonutIlIlce = ilceAdi + "/" + ilAdi;
                }
                var aracPlaka = polList[i].TeklifAracs.FirstOrDefault();
                if (aracPlaka != null)
                {
                    if (!String.IsNullOrEmpty(aracPlaka.PlakaKodu))
                    {
                        kartItem.Plaka = aracPlaka.PlakaKodu + " " + aracPlaka.PlakaNo;
                    }
                }
                if (kartItem.KimlikNo == tckn)
                {
                    kartList.Add(kartItem);
                }
            }
            return kartList;
        }


        public void UpdateLilyumKartTeminatKullanimlari(List<LilyumKartTeminatKullanimGuncelleModel> guncelTeminatlar)
        {
            for (int i = guncelTeminatlar.Count - 1; i >= 0; i--)
            {
                if (guncelTeminatlar[i].ToplamKullanilanAdet != 0 || guncelTeminatlar[i].TeminatSonKullanilanTarihi != null || !String.IsNullOrEmpty(guncelTeminatlar[i].LilyumKartNo))
                {
                    UpdateLilyumTeminatKullanim(guncelTeminatlar[i].Id, guncelTeminatlar[i].ToplamKullanilanAdet, guncelTeminatlar[i].TeminatSonKullanilanTarihi, guncelTeminatlar[i].LilyumKartNo, guncelTeminatlar[i].LilyumReferansNo);
                }
            }
        }
        private void UpdateLilyumTeminatKullanim(int teminatId, byte teminatKullanimAdet, DateTime? teminatSonKullanimTarihi, string LilyumKartNo, string LilyumReferansNo)
        {
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            var teminatDetay = _TeklifContext.LilyumKartTeminatKullanimRepository.All().Where(w => w.TeminatId == teminatId && w.ReferansNo == LilyumReferansNo).FirstOrDefault();
            if (teminatDetay != null)
            {
                teminatDetay.ToplamKullanilanAdet = teminatKullanimAdet;
                teminatDetay.GuncellemeTarihi = TurkeyDateTime.Now;
                teminatDetay.GuncelleyenKullanici = _Aktif.KullaniciKodu;
                if (teminatSonKullanimTarihi.HasValue)
                {
                    teminatDetay.TeminatSonKullanilanTarihi = teminatSonKullanimTarihi.Value;
                }
                else
                {
                    teminatDetay.TeminatSonKullanilanTarihi = null;
                }
                if (!String.IsNullOrEmpty(LilyumKartNo))
                {
                    teminatDetay.LilyumKartNo = LilyumKartNo;
                }

                _TeklifContext.LilyumKartTeminatKullanimRepository.Update(teminatDetay);
                _TeklifContext.Commit();
            }
        }
        #endregion
        public List<int> GetTeklifID(int MusteriKodu)
        {
            var TeklifSgli = _TeklifContext.TeklifSigortaliRepository.All().Where(w => w.MusteriKodu == MusteriKodu).ToList();
            if (TeklifSgli != null)
            {
                return TeklifSgli.Select(w => w.TeklifId).ToList();
            }
            else
            {
                return null;
            }

        }
        public List<string> GetTelefonlar(List<int> teklifID)
        {
            List<string> telefonlar = new List<string>();
            for (int i = teklifID.Count - 1; i >= 0; i--)
            {
                int teklifid = teklifID[i];
                var teklifSoru = _TeklifContext.TeklifSoruRepository.All().Where(w => w.TeklifId == teklifid && w.SoruKodu == 396).FirstOrDefault();
                if (teklifSoru != null)
                {
                    telefonlar.Add(teklifSoru.Cevap);
                }
            }
            return telefonlar;
        }
        public List<string> GetAdresler(List<int> teklifID)
        {
            List<string> adresler = new List<string>();
            for (int i = teklifID.Count - 1; i >= 0; i--)
            {
                int teklifid = teklifID[i];
                var teklifSoru = _TeklifContext.TeklifSoruRepository.All().Where(w => w.TeklifId == teklifid && w.SoruKodu == 407).FirstOrDefault();
                if (teklifSoru != null)
                {
                    adresler.Add(teklifSoru.Cevap);
                }
            }
            return adresler;
        }
        public ReasurorGenel CreateReasurorGenel(ReasurorGenel reasurorGenel)
        {

            var reasuror = _TeklifContext.ReasurorGenelRepository.Create(reasurorGenel);
            _TeklifContext.Commit();
            return reasuror;
        }

        public List<PoliceDokuman> getReasurorGenelDokumanlar(int policeid)
        {
            List<PoliceDokuman> pds = new List<PoliceDokuman>();
            var reasurorGenel = _TeklifContext.ReasurorGenelRepository.All().Where(w => w.Police_ID == policeid).FirstOrDefault();

            if (reasurorGenel != null)
            {
                var teklifGenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TeklifId == reasurorGenel.Teklif_ID).FirstOrDefault();

                PoliceDokuman pd = new PoliceDokuman();

                if (teklifGenel != null)
                {
                    if (!String.IsNullOrEmpty(reasurorGenel.PdfPoliceDosyasi))
                    {
                        pd = new PoliceDokuman();
                        pd.DokumanAdi = "Credit Note 1";
                        pd.DokumanURL = reasurorGenel.PdfPoliceDosyasi;
                        pd.KayitTarihi = teklifGenel.KayitTarihi;
                        pd.TVMKullaniciKodu = teklifGenel.TVMKullanicilar.Adi+" " +teklifGenel.TVMKullanicilar.Soyadi;
                        pds.Add(pd);
                    }
                    if (!String.IsNullOrEmpty(reasurorGenel.PdfPoliceDebitNote))
                    {
                        pd = new PoliceDokuman();
                        pd.DokumanAdi = "Credit Note 2";
                        pd.DokumanURL = reasurorGenel.PdfPoliceDebitNote;
                        pd.KayitTarihi = teklifGenel.KayitTarihi;
                        pd.TVMKullaniciKodu = teklifGenel.TVMKullanicilar.Adi + " " + teklifGenel.TVMKullanicilar.Soyadi;
                        pds.Add(pd);
                    }
                    if (!String.IsNullOrEmpty(reasurorGenel.PdfPoliceCreditNote))
                    {
                        pd = new PoliceDokuman();
                        pd.DokumanAdi = "Credit Note 3";
                        pd.DokumanURL = reasurorGenel.PdfPoliceCreditNote;
                        pd.KayitTarihi = teklifGenel.KayitTarihi;
                        pd.TVMKullaniciKodu = teklifGenel.TVMKullanicilar.Adi + " " + teklifGenel.TVMKullanicilar.Soyadi;
                        pds.Add(pd);
                    }
                }
            }


            return pds;
        }

        #region Underwriters

        public bool CreateUnderwriters(List<Underwriters> Underwriters)
        {
            try
            {
                for (int i = 0; i < Underwriters.Count; i++)
                {
                    _TeklifContext.UnderwritersRepository.Create(Underwriters[i]);
                }
                _TeklifContext.Commit();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<Underwriters> getUnderwriters(string policeid, string teklifid)
        {
            int _policeid = !String.IsNullOrEmpty(policeid) ? Convert.ToInt32(policeid) : 0;
            int _teklifid = !String.IsNullOrEmpty(teklifid) ? Convert.ToInt32(teklifid) : 0;
            List<Underwriters> Underwriters = new List<Underwriters>();
            if (_policeid != 0)
            {
                Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.PoliceId == _policeid).ToList();
            }
            if (_teklifid != 0)
            {
                Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.TeklifId == _teklifid).ToList();
            }

            return Underwriters;
        }
        public bool deleteUnderwriters(string policeid, string teklifid)
        {
            bool result = false;
            int _policeid = !String.IsNullOrEmpty(policeid) ? Convert.ToInt32(policeid) : 0;
            int _teklifid = !String.IsNullOrEmpty(teklifid) ? Convert.ToInt32(teklifid) : 0;
            if (_policeid != 0)
            {
                try
                {
                    var Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.PoliceId == _policeid).FirstOrDefault();
                    if (Underwriters != null)
                    {
                        _TeklifContext.UnderwritersRepository.Delete(w => w.PoliceId == _policeid);
                        _TeklifContext.Commit();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    return result;

                }
            }
            if (_teklifid != 0)
            {
                try
                {
                    var Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                    if (Underwriters != null)
                    {
                        _TeklifContext.UnderwritersRepository.Delete(w => w.TeklifId == _teklifid);
                        _TeklifContext.Commit();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    return result;
                }
            }

            return result;
        }

        public void CreateTeklifDokuman(TeklifDokuman teklifDokuman)
        {
            _TeklifContext.TeklifDokumanRepository.Create(teklifDokuman);
            _TeklifContext.Commit();
        }


        #endregion

        public void UpdateReasurorGenel(ReasurorGenel reasurorGenel)
        {
            _TeklifContext.ReasurorGenelRepository.Update(reasurorGenel);
            _TeklifContext.Commit();
        }
        public ReasurorGenel getReasurorGenel(string policeid, string teklifid)
        {
            int _policeid = !String.IsNullOrEmpty(policeid) ? Convert.ToInt32(policeid) : 0;
            int _teklifid = !String.IsNullOrEmpty(teklifid) ? Convert.ToInt32(teklifid) : 0;
            ReasurorGenel reasuror = new ReasurorGenel();
            if (_policeid != 0)
            {
                reasuror = _TeklifContext.ReasurorGenelRepository.All().Where(w => w.Police_ID == _policeid).FirstOrDefault();
            }
            if (_teklifid != 0)
            {
                reasuror = _TeklifContext.ReasurorGenelRepository.All().Where(w => w.Teklif_ID == _teklifid).FirstOrDefault();
            }

            return reasuror;
        }
        public ITeklif getTeklifReasuror(string teklifNo, string tumKodu, int tvmKodu)
        {
            int _teklifNo = !String.IsNullOrEmpty(teklifNo) ? Convert.ToInt32(teklifNo) : 0;
            int _tumKodu = !String.IsNullOrEmpty(tumKodu) ? Convert.ToInt32(tumKodu) : 0;


            ITeklif teklif = null;

            if (_teklifNo != 0 && _tumKodu != 0)
            {
                TeklifGenel teklifGenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TeklifNo == _teklifNo && w.TUMKodu == _tumKodu && w.TVMKodu == tvmKodu).FirstOrDefault();
                teklif = new Teklif(teklifGenel);
                teklif.SigortaEttiren = teklifGenel.TeklifSigortaEttirens.FirstOrDefault();
                teklif.Sigortalilar = teklifGenel.TeklifSigortalis.ToList<TeklifSigortali>();
                teklif.Arac = teklifGenel.TeklifAracs.FirstOrDefault();
                teklif.AracEkSorular = teklifGenel.TeklifAracEkSorus.ToList<TeklifAracEkSoru>();
                teklif.Sorular = teklifGenel.TeklifSorus.ToList<TeklifSoru>();
                teklif.Teminatlar = teklifGenel.TeklifTeminats.ToList<TeklifTeminat>();
                teklif.Vergiler = teklifGenel.TeklifVergis.ToList<TeklifVergi>();
                teklif.WebServisCevaplar = teklifGenel.TeklifWebServisCevaps.ToList<TeklifWebServisCevap>();
                teklif.RizikoAdresi = teklifGenel.TeklifRizikoAdresis.FirstOrDefault();
                teklif.OdemePlani = teklifGenel.TeklifOdemePlanis.ToList();
            }


            return teklif;
        }
        public bool deleteReasurorGenel(string policeid, string teklifid)
        {
            bool result = false;
            int _policeid = !String.IsNullOrEmpty(policeid) ? Convert.ToInt32(policeid) : 0;
            int _teklifid = !String.IsNullOrEmpty(teklifid) ? Convert.ToInt32(teklifid) : 0;
            if (_policeid != 0)
            {
                try
                {
                    var reasurorTeklifid = _TeklifContext.ReasurorGenelRepository.All().Where(w => w.Police_ID == _policeid).FirstOrDefault();
                    if (reasurorTeklifid != null)
                    {
                        var teklifSigortali = _TeklifContext.TeklifSigortaliRepository.All().Where(w => w.TeklifId == reasurorTeklifid.Teklif_ID).FirstOrDefault();
                        if (teklifSigortali != null)
                        {
                            _TeklifContext.TeklifSigortaliRepository.Delete(w => w.TeklifId == reasurorTeklifid.Teklif_ID);
                            _TeklifContext.Commit();
                        }

                        var teklifSigortaettiren = _TeklifContext.TeklifSigortaEttirenRepository.All().Where(w => w.TeklifId == reasurorTeklifid.Teklif_ID).FirstOrDefault();
                        if (teklifSigortaettiren != null)
                        {
                            _TeklifContext.TeklifSigortaEttirenRepository.Delete(w => w.TeklifId == reasurorTeklifid.Teklif_ID);
                            _TeklifContext.Commit();
                        }

                        var teklifOdemePlani = _TeklifContext.TeklifOdemePlaniRepository.All().Where(w => w.TeklifId == reasurorTeklifid.Teklif_ID).FirstOrDefault();
                        if (teklifOdemePlani != null)
                        {
                            _TeklifContext.TeklifOdemePlaniRepository.Delete(w => w.TeklifId == reasurorTeklifid.Teklif_ID);
                            _TeklifContext.Commit();
                        }

                        var teklifgenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TeklifId == reasurorTeklifid.Teklif_ID).FirstOrDefault();
                        if (teklifgenel != null)
                        {
                            _TeklifContext.TeklifGenelRepository.Delete(w => w.TeklifId == reasurorTeklifid.Teklif_ID);
                            _TeklifContext.Commit();
                        }
                        var Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.PoliceId == _policeid).FirstOrDefault();
                        if (Underwriters != null)
                        {
                            _TeklifContext.UnderwritersRepository.Delete(w => w.PoliceId == _policeid);
                            _TeklifContext.Commit();
                        }
                        _TeklifContext.ReasurorGenelRepository.Delete(w => w.Police_ID == _policeid);
                        _TeklifContext.Commit();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    return result;

                }
            }
            if (_teklifid != 0)
            {
                try
                {
                    var reasurorGenel = _TeklifContext.ReasurorGenelRepository.All().Where(w => w.Teklif_ID == _teklifid).FirstOrDefault();
                    if (reasurorGenel != null)
                    {
                        var teklifSigortaEttiren = _TeklifContext.TeklifSigortaEttirenRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                        if (teklifSigortaEttiren != null)
                        {
                            _TeklifContext.TeklifSigortaEttirenRepository.Delete(w => w.TeklifId == _teklifid);
                            _TeklifContext.Commit();
                        }

                        var teklifSigortali = _TeklifContext.TeklifSigortaliRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                        if (teklifSigortali != null)
                        {
                            _TeklifContext.TeklifSigortaliRepository.Delete(w => w.TeklifId == _teklifid);
                            _TeklifContext.Commit();
                        }

                        var teklifOdemePlani = _TeklifContext.TeklifOdemePlaniRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                        if (teklifOdemePlani != null)
                        {
                            _TeklifContext.TeklifOdemePlaniRepository.Delete(w => w.TeklifId == _teklifid);
                            _TeklifContext.Commit();
                        }

                        var teklifgenel = _TeklifContext.TeklifGenelRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                        if (teklifgenel != null)
                        {
                            _TeklifContext.TeklifGenelRepository.Delete(w => w.TeklifId == _teklifid);
                            _TeklifContext.Commit();
                        }
                        var Underwriters = _TeklifContext.UnderwritersRepository.All().Where(w => w.TeklifId == _teklifid).FirstOrDefault();
                        if (Underwriters != null)
                        {
                            _TeklifContext.UnderwritersRepository.Delete(w => w.TeklifId == _teklifid);
                            _TeklifContext.Commit();
                        }
                        _TeklifContext.ReasurorGenelRepository.Delete(w => w.Teklif_ID == _teklifid);
                        _TeklifContext.Commit();
                    }
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    return result;
                }
            }

            return result;
        }
    }

}
