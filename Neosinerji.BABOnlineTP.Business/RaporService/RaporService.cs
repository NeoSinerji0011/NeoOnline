using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business
{
    public class RaporService : IRaporService
    {
        ITeklifContext _TeklifContext;
        ITVMContext _TVMContext;
        ITUMContext _TUMContext;
        IParametreContext _ParameterContext;
        IMusteriContext _MusteriContext;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        IProcedureContext _ProcedureContext;
        IPoliceContext _PoliceContext;
        IGorevTakipContext _gorevTakipContext;

        public RaporService(ITeklifContext teklifContext, ITVMContext tVMContext, ITUMContext tumContext, IParametreContext parameterContext, IMusteriContext musteriContext, IAktifKullaniciService akifKullanici, IGorevTakipContext gorevTakipContext)
        {
            _TeklifContext = teklifContext;
            _TVMContext = tVMContext;
            _TUMContext = tumContext;
            _ParameterContext = parameterContext;
            _MusteriContext = musteriContext;
            _AktifKullanici = akifKullanici;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _ProcedureContext = DependencyResolver.Current.GetService<IProcedureContext>();
            _PoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
            _gorevTakipContext = gorevTakipContext;
        }

        //________________ Liste Pager Methodlari ________________

        public List<SubeSatisRaporProcedureModel> SubeSatisPagedList(SubeSatisListe arama, out int totalRowCount)
        {
            List<SubeSatisRaporProcedureModel> raporsonuc = _TUMContext.SubeRapor_Getir(arama.PoliceTarihi.Value,
                                                                            arama.BaslangicTarihi,
                                                                            arama.BitisTarihi,
                                                                            arama.UrunList,
                                                                            arama.BransList,
                                                                            arama.DovizTL.Value,
                                                                            arama.TahIpt.Value,
                                                                             _AktifKullanici.TVMKodu);

            if (arama.PageSize <= 0) arama.PageSize = 10;
            totalRowCount = raporsonuc.Count();
            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;
            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<SubeSatisRaporProcedureModel>();
        }

        public List<MTSatisRaporProcedureModel> MtSatisPagedList(MtSatisListe arama, out int totalRowCount)
        {
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }
            else
                arama.Subeler = RaporYetkiKontrol(arama.Subeler);



            List<MTSatisRaporProcedureModel> raporsonuc = _TUMContext.MTRapor_Getir(arama.PoliceTarihi.Value,
                                                                         arama.BaslangicTarihi,
                                                                         arama.BitisTarihi,
                                                                         arama.UrunList,
                                                                         arama.BransList,
                                                                         arama.Subeler,
                                                                         arama.OdemeSekli,
                                                                         arama.OdemeTipi);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                       .Take(arama.PageSize)
                       .ToList();


            return list.ToList<MTSatisRaporProcedureModel>();
        }

        public List<PoliceRaporProcedureModel> PoliceRaporPagedList(PoliceRaporListe arama, out int totalRowCount)
        {
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }
            else
                arama.Subeler = RaporYetkiKontrol(arama.Subeler);

            List<PoliceRaporProcedureModel> raporsonuc = _TUMContext.PoliceRapor_Getir(arama.PoliceTarihi.Value,
                                                                             arama.BaslangicTarihi,
                                                                             arama.BitisTarihi,
                                                                             arama.UrunList,
                                                                             arama.BransList,
                                                                             arama.Subeler,
                                                                             arama.PoliceNo,
                                                                             arama.OdemeSekli,
                                                                             arama.OdemeTipi);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<PoliceRaporProcedureModel>();
        }


        public List<PoliceListesiOfflineRaporProcedureModel> PoliceListesiOfflineRaporGetir(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                  string sigortaSirketleriArray, string bransKoduArray,
                                                  string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {
            return _TUMContext.PoliceListesiOfflineRaporGetir(tvmKodu, policeTarihi, baslangicTarihi, bitisTarihi, subelerArray,
                                                   sigortaSirketleriArray, bransKoduArray,
                                                   PoliceNo, OdemeSekli, OdemeTipi, anaTVMKodu, uretimTvmlerArray);
        }

        public List<ReasurorPoliceListesiProcedureModel> ReasurorPoliceListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                                  string sigortaSirketleriArray, string bransKoduArray,
                                                  string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {
            return _TUMContext.GetReasurorPoliceListesi(tvmKodu, policeTarihi, baslangicTarihi, bitisTarihi, subelerArray,
                                                   sigortaSirketleriArray, bransKoduArray,
                                                   PoliceNo, OdemeSekli, OdemeTipi, anaTVMKodu, uretimTvmlerArray);
        }
        public List<ReasurorTeklifListesiProcedureModel> ReasurorTeklifListesi(int tvmKodu, byte? policeTarihi, DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray,
                                          string sigortaSirketleriArray, string bransKoduArray,
                                          string PoliceNo, byte? OdemeSekli, byte? OdemeTipi, int anaTVMKodu, string uretimTvmlerArray)
        {
            return _TUMContext.GetReasurorTeklifListesi(tvmKodu, policeTarihi, baslangicTarihi, bitisTarihi, subelerArray,
                                                   sigortaSirketleriArray, bransKoduArray,
                                                   PoliceNo, OdemeSekli, OdemeTipi, anaTVMKodu, uretimTvmlerArray);
        }
        public List<KartListesiProcedureModel> KartListesiRaporGetir(DateTime? baslangicTarihi, DateTime? bitisTarihi, string subelerArray, byte? OdemeSekli, int anaTVMKodu, string ad, string soyad, string tckn, int durum)
        {
            return _TUMContext.KartListesiRaporuGetir(baslangicTarihi, bitisTarihi, subelerArray,
                                                   OdemeSekli, anaTVMKodu, ad, soyad, tckn, durum);
        }
        public List<PoliceUretimRaporProcedureModel> PoliceUretimRaporPagedList(PoliceUretimRaporListe arama, int tvmKodu, out int totalRowCount)
        {


            List<PoliceUretimRaporProcedureModel> raporsonuc = _TUMContext.PoliceUretimRapor_Getir(arama.BasTarihi, arama.BitTarihi,
                                                                             arama.SigortaSirketleri,
                                                                             arama.BransList,
                                                                             arama.TVMList, arama.DisUretimTVMList, tvmKodu);

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<PoliceUretimRaporProcedureModel>();
        }

        public List<PoliceUretimRaporProcedureModelTali> PoliceUretimRaporTaliPagedList(PoliceUretimRaporListeTali arama, int tvmKodu, out int totalRowCount)
        {


            List<PoliceUretimRaporProcedureModelTali> raporsonuc = _TUMContext.PoliceUretimRaporTali_Getir(arama.Ay, arama.Yil,
                                                                             arama.SigortaSirketleri,
                                                                             arama.BransList,
                                                                             arama.TVMList, tvmKodu);

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<PoliceUretimRaporProcedureModelTali>();
        }

        public List<PoliceHedefRaporModel> PoliceHedefGerceklesenRapor(int tvmKodu, int talitvmKodu, int yil)
        {
            return _TUMContext.PoliceHedefRaporGerceklesen(tvmKodu, talitvmKodu, yil);
        }

        public List<PoliceUretimIcmalRaporProcedureModel> PoliceIcmalRaporGetir(int tvmKodu, byte Merkez, int ay, int yil, string SigortaSirketleriListe, string bransKoduListe, string tvmlerListe, string disKaynakLsite, int raporTip)
        {
            return _TUMContext.PoliceIcmalRaporGetir(tvmKodu, Merkez, ay, yil, SigortaSirketleriListe, bransKoduListe, tvmlerListe, disKaynakLsite, raporTip);
        }

        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string SigortaSirketleriArray, string TcVkn, string tvmlerListe, int RaporTipi)
        {
            return _TUMContext.PoliceTahsilatRaporGetir(MerkezAcenteMi, MerkezAcenteKodu, BaslangicTarihi, BitisTarihi, SigortaSirketleriArray, TcVkn, tvmlerListe, RaporTipi);
        }

        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatMusteriGrupKoduRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string MusteriGrupKodu, int RaporTipi)
        {
            MusteriGrupKodu = MusteriGrupKodu.ToLower().Replace('ı', 'i');
            List<PoliceTahsilatRaporProcedureModel> result = new List<PoliceTahsilatRaporProcedureModel>();
            var musterigenelbilgileri = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMMusteriKodu.ToLower().Trim().StartsWith(MusteriGrupKodu) && s.TVMKodu == _AktifKullanici.TVMKodu).ToList();
            var musteriData = musterigenelbilgileri.Select(x => x.KimlikNo).Distinct().ToList();

            //for (int i = 0; i < musterigenelbilgileri.Count(); i++)
            //{
            //    var temp= _TUMContext.PoliceTahsilatMusteriGrupKoduRaporGetir(MerkezAcenteMi, MerkezAcenteKodu, BaslangicTarihi, BitisTarihi, musterigenelbilgileri[i].KimlikNo, RaporTipi);
            //    if (temp.Count > 0)
            //    {
            //        result.InsertRange(result.Count(), temp);
            //    }
            //}
            foreach (var item in musteriData)
            {
                var temp = _TUMContext.PoliceTahsilatMusteriGrupKoduRaporGetir(MerkezAcenteMi, MerkezAcenteKodu, BaslangicTarihi, BitisTarihi, item, RaporTipi);
                if (temp.Count > 0)
                {
                    result.InsertRange(result.Count(), temp);
                }
            } 
            return result;
        }
        public List<PoliceTahsilatRaporProcedureModel> PoliceTahsilatDisKaynakRaporGetir(byte MerkezAcenteMi, int MerkezAcenteKodu, DateTime? BaslangicTarihi, DateTime? BitisTarihi, string disKaynakListe, int RaporTipi)
        {
            return _TUMContext.PoliceTahsilatDisKaynakRaporGetir(MerkezAcenteMi, MerkezAcenteKodu, BaslangicTarihi, BitisTarihi, disKaynakListe, RaporTipi);
        }
        public List<VadeTakipRaporuProcedureModel> VadeTakipRaporuPagedList(VadeTakipRaporuListe arama, out int totalRowCount)
        {
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }
            else
                arama.Subeler = RaporYetkiKontrol(arama.Subeler);

            List<VadeTakipRaporuProcedureModel> raporsonuc = _TUMContext.SP_VadeTakipRaporu_Deneme(arama.BaslangicTarihi,
                                                                                    arama.BitisTarihi,
                                                                                    arama.UrunList,
                                                                                    arama.BransList,
                                                                                    arama.Subeler,
                                                                                    arama.TarihTipi
                                                                                    );


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<VadeTakipRaporuProcedureModel>();
        }

        public List<KrediliHayatPoliceRaporProcedureModel> KrediliHayatPoliceRaporPagedList(KrediliHayatPoliceRaporListe arama, out int totalRowCount)
        {
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }
            else
                arama.Subeler = RaporYetkiKontrol(arama.Subeler);

            List<KrediliHayatPoliceRaporProcedureModel> raporsonuc = _TUMContext.KrediliHayatPoliceRapor_Getir(arama.PoliceTarihi.Value,
                                                                                                    arama.DovizTL,
                                                                                                    arama.BaslangicTarihi,
                                                                                                    arama.BitisTarihi,
                                                                                                    arama.Subeler,
                                                                                                    arama.PoliceNo,
                                                                                                    arama.OdemeSekli,
                                                                                                    arama.OdemeTipi);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<KrediliHayatPoliceRaporProcedureModel>();
        }

        public List<TeklifRaporuProcedureModel> TeklifRaporuPagedList(TeklifRaporuListe arama, out int totalRowCount)
        {
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }
            else
                arama.Subeler = RaporYetkiKontrol(arama.Subeler);

            List<TeklifRaporuProcedureModel> raporsonuc = _TUMContext.TeklifRaporu_Getir(arama.PoliceTarihi.Value,
                                                                                arama.BaslangicTarihi,
                                                                                arama.BitisTarihi,
                                                                                arama.Subeler,
                                                                                arama.BransList,
                                                                                arama.UrunList,
                                                                                arama.DovizTL,
                                                                                arama.TeklifNo);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();


            return list.ToList<TeklifRaporuProcedureModel>();
        }

        public List<OzetRaporProcedureModel> OzetRaporPagedList(OzetRaporListe arama, out int totalRowCount)
        {
            List<OzetRaporProcedureModel> raporsonuc = _TUMContext.OzetRapor_Getir(arama.PoliceTarihi.Value,
                                                                        arama.BaslangicTarihi,
                                                                        arama.BitisTarihi,
                                                                        arama.BransList,
                                                                        arama.UrunList,
                                                                        arama.DovizTL,
                                                                        arama.aramakriteri,
                                                                        arama.asildeger,
                                                                        arama.data2,
                                                                        arama.data3,
                                                                        _AktifKullanici.TVMKodu,
                                                                        _AktifKullanici.YetkiGrubu);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                       .Take(arama.PageSize)
                       .ToList();


            return list.ToList<OzetRaporProcedureModel>();
        }

        public List<OzetRaporProcedureModel> OzetRaporPagedListTest(OzetRaporProcedureRequestModel data)
        {
            List<OzetRaporProcedureModel> raporsonuc = new List<OzetRaporProcedureModel>();

            #region Check Parameter



            #endregion

            return raporsonuc;
        }

        public List<AracSigortalariIstatistikRaporuProcedureModel> AracSigortaliIstatistikRaporPagedList(AracSigortalariIstatistikRaporuListe arama, out int totalRowCount)
        {
            List<AracSigortalariIstatistikRaporuProcedureModel> raporsonuc = new List<AracSigortalariIstatistikRaporuProcedureModel>();

            if (arama.TVMKodu.HasValue)
                if (!_TVMService.KullaniciTvmyiGormeyeYetkiliMi(arama.TVMKodu.Value))
                {
                    totalRowCount = 0;
                    return raporsonuc;
                }

            raporsonuc = _TUMContext.AracSigortalariIstatistikRaporu_Getir(arama.TVMKodu,
                                                                            arama.PoliceTarihi.Value,
                                                                            arama.BaslangicTarihi,
                                                                            arama.BitisTarihi,
                                                                            arama.Urun,
                                                                            arama.DovizTL,
                                                                            arama.customvalue,
                                                                            arama.kodu,
                                                                            arama.data2,
                                                                            arama.sorguturu);


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                       .Take(arama.PageSize)
                       .ToList();


            return list.ToList<AracSigortalariIstatistikRaporuProcedureModel>();
        }

        ////Genel Rapor
        //public List<GenelRaporProcedureModel> GenelRaporPagedList(GenelRaporListe arama, out int totalRowCount)
        //{
        //    if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi) { }

        //    List<GenelRaporProcedureModel> raporsonuc = _TUMContext.GenelRapor_Getir(arama.TVMKodu,arama.ProjeKodu
        //      ); 

        //    totalRowCount = raporsonuc.Count();
        //    var list = (from k in raporsonuc
        //                select k)
        //               .ToList();

        //    return list.ToList<GenelRaporProcedureModel>();
        //}

        // ==== Service Methods ====//
        public OfflinePolouse CreateOfflinePolice(OfflinePolouse offlinePolice)
        {
            OfflinePolouse police = _ParameterContext.OfflinePolouseRepository.Create(offlinePolice);
            _ParameterContext.Commit();
            return police;
        }

        private string RaporYetkiKontrol(string eskiSubeler)
        {
            int AktifTvmKodu = _AktifKullanici.TVMKodu;
            List<int> tvmKodlari = new List<int>();
            int sayac = 0;
            string yeniSubeler = "";
            List<TVMDetay> yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == AktifTvmKodu || s.BagliOlduguTVMKodu == AktifTvmKodu)
                                                                         .ToList<TVMDetay>();


            foreach (var item in yetkiliTvmler)
                tvmKodlari.Add(item.Kodu);

            if (String.IsNullOrEmpty(eskiSubeler) || eskiSubeler.Contains("multiselect-all"))
            {
                foreach (var item in tvmKodlari)
                {
                    yeniSubeler += item.ToString();
                    sayac++;
                    if (sayac < tvmKodlari.Count)
                        yeniSubeler += ",";
                }
            }
            else
            {
                string[] subekodlari = eskiSubeler.Split(',');
                if (subekodlari.Length > 0)
                {
                    foreach (var item in subekodlari)
                    {
                        if (tvmKodlari.Contains(Convert.ToInt32(item)))
                            yeniSubeler += item;
                        sayac++;
                        if (sayac < subekodlari.Length)
                            yeniSubeler += ",";
                    }
                }
            }

            return yeniSubeler;
        }

        public string PoliceNoKontrol(string policeNo, int tvmkodu)
        {
            int adet = _ParameterContext.OfflinePolouseRepository.Filter(s => s.PoliceNo == policeNo && s.TVMKodu == tvmkodu).Count();
            if (adet > 0) return "Bu poliçe daha önce eklenmiş";

            return "";
        }

        public PoliceHaritaModel GetPoliceHarita(string[] policeList)
        {
            PoliceHaritaModel model = new PoliceHaritaModel();

            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _AktifKullanici.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
            { }

            List<TeklifGenel> teklifList = _TeklifContext.TeklifGenelRepository.Filter(s => policeList.Contains(s.TUMPoliceNo)).ToList<TeklifGenel>();

            List<TeklifGenel> Policeler = teklifList.Where(s => s.UrunKodu == UrunKodlari.DogalAfetSigortasi_Deprem ||
                                                                s.UrunKodu == UrunKodlari.KonutSigortasi_Paket ||
                                                                s.UrunKodu == UrunKodlari.IsYeri
                                                                ).ToList<TeklifGenel>();

            foreach (var item in Policeler)
            {

            }

            return model;
        }

        #region Mapfre Rapor Metod
        public List<MapfreBolgeUretimModel> MapfreBolgeUretimRapor(DateTime BaslangicTarih, DateTime BitisTarih, int? BolgeKodu, bool Acenteler)
        {
            return _ProcedureContext.MapfreBolgeUretimRapor(BaslangicTarih, BitisTarih, BolgeKodu, Acenteler);
        }
        #endregion

        #region Aegon Rapor Method

        public List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporuPagedList(AegonTeklifRaporuListe arama, out int totalRowCount)
        {
            arama.Subeler = AegonRaporYetkiKontrol(arama.Subeler, arama.BolgeKodu);

            if (arama.PageSize <= 0) arama.PageSize = 10;
            if (arama.Page <= 0) arama.Page = 1;
            int excludedRows = (arama.Page - 1) * arama.PageSize;

            arama.skip = excludedRows;
            arama.take = arama.PageSize;

            List<AegonTeklifRaporuProcedureModel> raporsonuc = _TUMContext.AegonTeklifRaporu_Getir(arama.TeklifTarihi,
                                                                                                    arama.BaslangicTarihi,
                                                                                                    arama.BitisTarihi,
                                                                                                    arama.Subeler,
                                                                                                    arama.Urunler,
                                                                                                    arama.ParaBirimi,
                                                                                                    arama.TeklifNo,
                                                                                                    arama.YillikMin,
                                                                                                    arama.YillikMax,
                                                                                                    arama.skip,
                                                                                                    arama.take);


            totalRowCount = 0;

            foreach (var item in raporsonuc)
            {
                item.RiskDegerlendirmeSonucu = TetkikDuzenle(item.RiskDegerlendirmeSonucu);
                totalRowCount = item.Total_Rows;
            }

            return raporsonuc;
        }

        private string AegonRaporYetkiKontrol(string eskiSubeler, string BolgeKodu)
        {
            int AktifTvmKodu = _AktifKullanici.TVMKodu;
            List<int> tvmKodlari = new List<int>();
            int sayac = 0;
            string yeniSubeler = "";
            List<TVMDetay> yetkiliTvmler;
            int bolgekodu;

            //Aegon kullanicisi için bir kısıtlama yok.
            if (_AktifKullanici.TVMKodu == NeosinerjiTVM.AegonTVMKodu && String.IsNullOrEmpty(eskiSubeler))
            {
                return "";
            }


            if (!String.IsNullOrEmpty(BolgeKodu) && int.TryParse(BolgeKodu, out bolgekodu))
            {
                yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == AktifTvmKodu || s.BagliOlduguTVMKodu == AktifTvmKodu && s.BolgeKodu == bolgekodu)
                                                                         .ToList<TVMDetay>();
            }
            else
            {
                yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == AktifTvmKodu || s.BagliOlduguTVMKodu == AktifTvmKodu).ToList<TVMDetay>();
            }



            foreach (var item in yetkiliTvmler)
                tvmKodlari.Add(item.Kodu);

            if (String.IsNullOrEmpty(eskiSubeler))
            {
                foreach (var item in tvmKodlari)
                {
                    yeniSubeler += item.ToString();
                    sayac++;
                    if (sayac < tvmKodlari.Count)
                        yeniSubeler += ",";
                }
            }
            else
            {
                string[] subekodlari = eskiSubeler.Split(',');
                if (subekodlari.Length > 0)
                {
                    foreach (var item in subekodlari)
                    {
                        if (tvmKodlari.Contains(Convert.ToInt32(item)))
                            yeniSubeler += item;
                        sayac++;
                        if (sayac < subekodlari.Length)
                            yeniSubeler += ",";
                    }
                }
            }

            return yeniSubeler;
        }

        private string TetkikDuzenle(string tetkik)
        {
            if (!String.IsNullOrEmpty(tetkik))
            {
                int index = tetkik.IndexOf(".");
                if (index != -1)
                {
                    string ilk = String.Empty;
                    if (tetkik.Length > index)
                        ilk = tetkik.Substring(0, index + 1);


                    string son = String.Empty;
                    if (tetkik.Length > index + 2)
                        son = tetkik.Substring(index + 3);

                    tetkik = ilk + " \n " + son;
                }

                tetkik = "<a href='javascript:;' class='popovers' data-trigger='hover' data-placement='bottom' data-content='" + tetkik + "'>Tetkik</a>";
            }

            return tetkik;
        }

        private string TetkikDuzenleForExcel(string tetkik)
        {
            if (!String.IsNullOrEmpty(tetkik))
            {
                int index = tetkik.IndexOf(".");
                if (index != -1)
                {
                    string ilk = String.Empty;
                    if (tetkik.Length > index)
                        ilk = tetkik.Substring(0, index + 1);

                    string son = String.Empty;
                    if (tetkik.Length > index + 2)
                        son = tetkik.Substring(index + 3);

                    tetkik = ilk + " \n " + son;
                }
            }

            return tetkik;
        }

        public List<AegonTeklifRaporuProcedureModel> AegonTeklifRaporuExcelAktar(AegonTeklifRaporuExcelAktar arama)
        {
            arama.Subeler = AegonRaporYetkiKontrol(arama.Subeler, arama.BolgeKodu);

            List<AegonTeklifRaporuProcedureModel> raporsonuc = _TUMContext.AegonTeklifRaporu_Getir(arama.TeklifTarihi,
                                                                                                    arama.BaslangicTarihi,
                                                                                                    arama.BitisTarihi,
                                                                                                    arama.Subeler,
                                                                                                    arama.Urunler,
                                                                                                    arama.ParaBirimi,
                                                                                                    arama.TeklifNo,
                                                                                                    arama.YillikMin,
                                                                                                    arama.YillikMax,
                                                                                                    0, 100000);

            foreach (var item in raporsonuc)
            {
                item.RiskDegerlendirmeSonucu = TetkikDuzenleForExcel(item.RiskDegerlendirmeSonucu);
            }

            return raporsonuc;
        }

        #endregion

        public ResultModelAtananIs AtananIslerCreate(List<AtananIsler> AtanacakIsler)
        {
            ResultModelAtananIs model = new ResultModelAtananIs();
            model.hataliKayitlar = new List<HataliKayit>();
            try
            {
                foreach (var item in AtanacakIsler)
                {
                    var result = AddAtanacakIs(item);
                    if (result == 1)
                    {
                        model.basariliKayitSayisi++;
                    }
                    else
                    {
                        HataliKayit hata = new HataliKayit();
                        hata.ekno = item.EkNo;
                        hata.yenilemeNo = item.YenilemeNo;
                        hata.policeNo = item.PoliceNumarasi;
                        hata.sirketKodu = item.SigortaSirketKodu;
                        model.hataliKayitlar.Add(hata);
                    }
                }
                if (model.hataliKayitlar != null)
                {
                    if (model.hataliKayitlar.Count > 0)
                    {
                        this.AtanamayanIsBeginLog(model.hataliKayitlar, model.hataliKayitlar.GetType());
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return model;
        }
        public int AddAtanacakIs(AtananIsler IsItem)
        {
            int result = 0;
            try
            {
                _gorevTakipContext.AtananIslerRepository.Create(IsItem);
                _gorevTakipContext.Commit();
                result = 1;
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        public void AtanamayanIsBeginLog(object request, Type type)
        {
            try
            {
                string istek = String.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer s = new XmlSerializer(type);

                        s.Serialize(xmlWriter, request);
                    }

                    istek = Encoding.UTF8.GetString(ms.ToArray());
                }
                IAtananIsLogStorage storage = DependencyResolver.Current.GetService<IAtananIsLogStorage>();
                string logURL = storage.UploadXml("police-yenileme", istek);
                AtananIsLog log = new AtananIsLog();
                string fileName = String.Format("policeyenilemelog{0}.pdf", System.Guid.NewGuid().ToString());
                log.KayitTarihi = TurkeyDateTime.Now;
                log.LogDosyasiURL = logURL;
                log.LogDosyasiAdi = fileName;
                log.TVMKodu = _AktifKullanici.TVMKodu;
                log.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                _gorevTakipContext.AtananIsLogRepository.Create(log);
                _gorevTakipContext.Commit();
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
        }

        public List<int> AtananIsListesi(List<int> polId)
        {
            List<int> list = new List<int>();
            list = _gorevTakipContext.AtananIslerRepository.All().Where(w => w.PoliceId != null && polId.Contains(w.PoliceId.Value)).Select(s => s.PoliceId.Value).ToList();
            return list;
        }
        public class PoliceTahsilatRaporGrupKoduModel
        {
            public int PoliceId { get; set; }
            public string PoliceNo { get; set; }
            public string SatisKanaliUnvani { get; set; }
            public string BransAdi { get; set; }
            public string TUMUrunAdi { get; set; }
            public int YenilemeNo { get; set; }
            public int EkNo { get; set; }
            public int TaksitNo { get; set; }
            public Nullable<decimal> TaksitTutari { get; set; }
            public DateTime? TaksitVadeTarihi { get; set; }
            public Nullable<decimal> OdenenTutar { get; set; }
            public Nullable<decimal> KalanTaksitTutari { get; set; }
            public int OdemTipi { get; set; }
            public string OdemeBelgeNo { get; set; }
            public DateTime? OdemeBelgeTarihi { get; set; }
            public string TahsilatiYapanKullaniciAdi { get; set; }
            public string TahsilatiYapanKullaniciSoyadi { get; set; }
            public string SigortaEttirenAdi { get; set; }
            public string SigortaEttirenSoyadi { get; set; }
            public string SigortaliAdi { get; set; }
            public string SigortaliSoyadi { get; set; }
            public string SigortaSirketi { get; set; }
            public List<PoliceTahsilatRaporGrupKoduProcedureModel> procedureTahsilatList { get; set; }
        }

        public class PoliceTahsilatRaporGrupKoduProcedureModel
        {
            public int PoliceId { get; set; }
            public string PoliceNo { get; set; }
            public string SatisKanaliUnvani { get; set; }
            public string BransAdi { get; set; }
            public string TUMUrunAdi { get; set; }
            public int YenilemeNo { get; set; }
            public int EkNo { get; set; }
            public int TaksitNo { get; set; }
            public Nullable<decimal> TaksitTutari { get; set; }
            public DateTime? TaksitVadeTarihi { get; set; }
            public Nullable<decimal> OdenenTutar { get; set; }
            public Nullable<decimal> KalanTaksitTutari { get; set; }
            public int OdemTipi { get; set; }
            public string OdemeBelgeNo { get; set; }
            public DateTime? OdemeBelgeTarihi { get; set; }
            public string TahsilatiYapanKullaniciAdi { get; set; }
            public string TahsilatiYapanKullaniciSoyadi { get; set; }
            public string SigortaEttirenAdi { get; set; }
            public string SigortaEttirenSoyadi { get; set; }
            public string SigortaliAdi { get; set; }
            public string SigortaliSoyadi { get; set; }
            public string SigortaSirketi { get; set; }
            // public virtual PoliceGenel PoliceGenel { get; set; }
        }

        public List<MusteriGenelBilgiler> TahsilatTakipRaporuGrupKoduAra(int tvmkodu, string grupKodu, DateTime baslangicTarhi, DateTime bitisTarihi, byte raportipi)
        {
            PoliceTahsilatRaporGrupKoduModel mod = new PoliceTahsilatRaporGrupKoduModel();
            //mod.procedureTahsilatList = new List<PoliceTahsilatRaporGrupKoduProcedureModel>();

            grupKodu = grupKodu.ToLower().Replace('ı', 'i');
            grupKodu.Trim();
            List<MusteriGenelBilgiler> musGrupKodu = new List<MusteriGenelBilgiler>();
            if (grupKodu.Length < 31)
            {
                musGrupKodu = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(s => s.TVMMusteriKodu.ToLower().Trim() == grupKodu && s.TVMKodu == tvmkodu).ToList<MusteriGenelBilgiler>();
                if (musGrupKodu.Count > 0)
                {
                    foreach (var itemGrupKodu in musGrupKodu)
                    {//raportipi=0 ödenmemiş,1 ödenmiş,2 kısmi ödenmiş

                        var polgenel = _PoliceContext.PoliceTahsilatRepository.All().Where(f => f.TaksitVadeTarihi >= baslangicTarhi
                                                                                    && f.TaksitVadeTarihi <= bitisTarihi
                                                                                    && f.KimlikNo == itemGrupKodu.KimlikNo).ToList();

                        foreach (var item in polgenel)
                        {

                            mod.PoliceId = item.PoliceId;


                        }
                    }
                    // musGrupKodu = musGrupKodu;
                }
            }
            return musGrupKodu;
        }

        public List<PoliceTahsilat> PolTahKimlikNoSorgula(string kimlikno)
        {
            List<PoliceTahsilat> kimlik = new List<PoliceTahsilat>();

            kimlik = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo == kimlikno).ToList();

            return kimlik;
        }

        
    }
}
