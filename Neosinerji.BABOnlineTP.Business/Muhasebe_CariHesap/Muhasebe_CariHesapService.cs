using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Pdf;
using System.Web.Mvc;
using System.Diagnostics;

namespace Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap
{
    public class GelirGiderTablosuAnaModel
    {
        public Dictionary<string, GelirGiderTablosuModel> dict = new Dictionary<string, GelirGiderTablosuModel>();

        public byte AramaTip { get; set; }
        public SelectList AramaTipTipleri { get; set; }
        public byte RaporTip { get; set; }
        public SelectList RaporTipTipleri { get; set; }
        public List<SelectListItem> Donemler { get; set; }
        public int Donem { get; set; }
        public int Ay { get; set; }
        public List<SelectListItem> Aylar { get; set; }
        public Boolean AramaDurumu { get; set; }
        public DateTime baslangicTarihi { get; set; }
        public DateTime bitisTarihi { get; set; }
        public List<GelirGiderTablosuEkranModel> list = new List<GelirGiderTablosuEkranModel>();

    }

    public class GelirGiderTablosuModel
    {
        public int id { get; set; }
        public string CariHesapAdi { get; set; }
        public string paraBirimi { get; set; }
        public List<SelectListItem> CariHesapList { get; set; }

        //Liste Parametreleri
        public decimal? BorcToplam { get; set; }
        public decimal? AlacakToplam { get; set; }
        public decimal? BakiyeToplam { get; set; }
        public string CariHesapText { get; set; }

        public decimal? BorcToplamHer { get; set; }
        public decimal? AlacakToplamHer { get; set; }
        public decimal? BakiyeToplamHer { get; set; }
        public List<GelirGiderTablosuEkranModel> list = new List<GelirGiderTablosuEkranModel>();

        // 0 ise ada göre  1 ise başlangıç tarihi bitiş tarihine göre arama

    }
    public class GelirGiderTablosuEkranModel
    {
        public int id { get; set; }
        public string OdemeTipi { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public string Aciklama { get; set; }
        public string ParaBirimi { get; set; }
        public decimal? Borc { get; set; }
        public decimal? Alacak { get; set; }
        public string EvrakNo { get; set; }
        public string EvrakTipi { get; set; }
        public DateTime VadeTarihi { get; set; }
        public string BorcTipi { get; set; }
        public string CariHesapKodu { get; set; }
        public string MusteriGrupkodu { get; set; }

    }

    public class Muhasebe_CariHesapService : IMuhasebe_CariHesapService
    {
        IPoliceContext _PoliceContext;
        IMusteriContext _MusteriContext;
        IKomisyonContext _KomisyonContext;
        IAktifKullaniciService _AktifKullanici;
        IParametreContext _ParametreContext;
        IAktifKullaniciService _AktifKullaniciService;
        ITVMService _TVMService;
        IKomisyonService _KomisyonService;
        ITVMContext _TVMContext;
        IMuhasebeContext _MuhasebeContext;
        ISigortaSirketleriService _SigortaSirketleriService;
        IMusteriService _MusteriService;

        [InjectionConstructor]
        public Muhasebe_CariHesapService(IPoliceContext policeContext, IMusteriContext musteriContext, IKomisyonContext komisyonContext, IAktifKullaniciService aktifKullanici,
            IParametreContext parametreContext, IAktifKullaniciService _aktifKullaniciService, ITVMService _tVMService, IKomisyonService _komisyonService, ITVMContext tvmContext,
            IMuhasebeContext muhasebeContext, ISigortaSirketleriService sigortaSirketleriService, IMusteriService musteriService)
        {
            _PoliceContext = policeContext;
            _MusteriContext = musteriContext;
            _KomisyonContext = komisyonContext;
            _AktifKullanici = aktifKullanici;
            _ParametreContext = parametreContext;
            _AktifKullaniciService = _aktifKullaniciService;
            _KomisyonService = _komisyonService;
            _TVMService = _tVMService;
            _TVMContext = tvmContext;
            _MuhasebeContext = muhasebeContext;
            _SigortaSirketleriService = sigortaSirketleriService;
            _MusteriService = musteriService;
        }
        public class CariHesapGelirGiderListModel
        {
            public string OdemeTipi { get; set; }
            public DateTime? OdemeTarihi { get; set; }
            public string Aciklama { get; set; }
            public string ParaBirimi { get; set; }
            public decimal? Borc { get; set; }
            public decimal? Alacak { get; set; }
            public string EvrakNo { get; set; }
            public string EvrakTipi { get; set; }
            public DateTime VadeTarihi { get; set; }
            public string BorcTipi { get; set; }
            public string CariHesapAdiKodu { get; set; }
            public string MusteriGrupkodu { get; set; }
            public string kimlikNo { get; set; }
            public int id { get; set; }


        }
        #region Hesap Ekstresi

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapTcVknAra(string TcVkn, int tvmkodu, int yil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            List<int> polListId = new List<int>();
            if (TcVkn.Length == 11 || TcVkn.Length == 10)
            {
                police = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi.Year == yil).ToList<PoliceTahsilat>();
                if (police.Count > 0)
                {
                    PolGenelList.TahsilatTckVn = police;
                }
            }

            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapOdemeBelgeNoAra(string OdemeBelgeNo, int tvmkodu, int yil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            List<int> polListId = new List<int>();
            if (OdemeBelgeNo.Length <= 20)
            {
                police = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.OdemeBelgeNo == OdemeBelgeNo && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi.Year == yil).ToList<PoliceTahsilat>();
                if (police.Count > 0)
                {
                    PolGenelList.TahsilatTckVn = police;
                }
            }

            return PolGenelList;

        }


        public MuhasebeCariHesapServiceModel MuhasebeCariHesapUnvan(string Unvan, string UnvanSoyad, int tvmkodu, byte durum, int yil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttirenUnvan = new List<PoliceSigortaEttiren>();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            string AdSoyad = null;
            Unvan = Unvan.ToLower().Replace('ı', 'i');
            UnvanSoyad = UnvanSoyad.ToLower().Replace('ı', 'i');
            AdSoyad = Unvan + " " + UnvanSoyad;
            AdSoyad.Trim();
            List<int> polListId = new List<int>();
            //0 şahıs,1 firma
            if (durum == 0)
            {
                if (Unvan.Length <= 50 || UnvanSoyad.Length <= 50)
                {
                    polSigortaEttirenUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim() == Unvan
                    && s.SoyadiUnvan.Trim().ToLower() == UnvanSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != ""
                    || s.AdiUnvan.Trim().ToLower() == AdSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.BaslangicTarihi.Value.Year == yil
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != "").ToList();


                    if (polSigortaEttirenUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortaEttirenUnvan;
                    }

                }
            }
            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapFirmaUnvan(string UnvanFirma, int tvmkodu, byte durum, int yil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttirenFirmaUnvan = new List<PoliceSigortaEttiren>();

            UnvanFirma = UnvanFirma.ToLower().Replace('ı', 'i');

            UnvanFirma.Trim();
            List<int> polListId = new List<int>();
            //0 şahıs,1 firma

            if (durum == 1)
            {
                if (UnvanFirma.Length <= 150 || UnvanFirma.Length >= 3)
                {
                    polSigortaEttirenFirmaUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim().Contains(UnvanFirma)
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != ""
                    && s.PoliceGenel.BaslangicTarihi.Value.Year == yil
                    ).ToList();
                    if (polSigortaEttirenFirmaUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortaEttirenFirmaUnvan;
                    }


                }
            }

            return PolGenelList;
        }

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapSatisKanaliAra(int tvmkodu, List<int> tvmlist, int ay, int yil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceGenel> satisKanaliAra = new List<PoliceGenel>();
            satisKanaliAra = _PoliceContext.PoliceGenelRepository.All().Where(s => tvmlist.Contains(s.TaliAcenteKodu.Value) && s.BaslangicTarihi.Value.Month == ay && s.BaslangicTarihi.Value.Year == yil).ToList();

            if (satisKanaliAra.Count > 0)
            {
                PolGenelList.OfflineGenel = satisKanaliAra;
            }
            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapGrupKoduAra(int tvmkodu, string grupKodu, int yil)
        {
            grupKodu = grupKodu.ToLower().Replace('ı', 'i');
            grupKodu.Trim();
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<MusteriGenelBilgiler> musGrupKodu = new List<MusteriGenelBilgiler>();
            if (grupKodu.Length < 31)
            {
                musGrupKodu = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(s => s.TVMMusteriKodu.ToLower().Trim() == grupKodu
                && s.TVMKodu == tvmkodu
                && s.KayitTarihi.Year == yil).ToList<MusteriGenelBilgiler>();
                if (musGrupKodu.Count > 0)
                {
                    PolGenelList.OfflineMusteriGenel = musGrupKodu;
                }
            }

            return PolGenelList;
        }

        public MuhasebeCariHesapServiceModel MuhasebeCariHesapTarihtenAra(int tvmkodu, DateTime baslangicTarhi, DateTime bitisTarihi)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceGenel> ikiTarihAraligi = new List<PoliceGenel>();
            ikiTarihAraligi = _PoliceContext.PoliceGenelRepository.All().Where(s => s.BaslangicTarihi > baslangicTarhi && s.BitisTarihi <= bitisTarihi).OrderBy(s => s.BaslangicTarihi).ToList();
            if (ikiTarihAraligi.Count > 0)
            {
                PolGenelList.OfflineGenel = ikiTarihAraligi;
            }

            return PolGenelList;
        }

        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapTcVknAra(string TcVkn, int tvmkodu, int devirYil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            List<int> polListId = new List<int>();
            if (TcVkn.Length == 11 || TcVkn.Length == 10)
            {
                police = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi.Year == devirYil).ToList<PoliceTahsilat>();
                if (police.Count > 0)
                {
                    PolGenelList.TahsilatTckVn = police;
                }
            }

            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapOdemeBelgeNoAra(string OdemeBelgeNo, int tvmkodu, int devirYil)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            List<int> polListId = new List<int>();
            if (OdemeBelgeNo.Length <= 20)
            {
                police = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.OdemeBelgeNo == OdemeBelgeNo && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi.Year == devirYil).ToList<PoliceTahsilat>();
                if (police.Count > 0)
                {
                    PolGenelList.TahsilatTckVn = police;
                }
            }

            return PolGenelList;

        }


        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapUnvan(string Unvan, string UnvanSoyad, int tvmkodu, byte durum)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttirenUnvan = new List<PoliceSigortaEttiren>();
            List<PoliceTahsilat> police = new List<PoliceTahsilat>();

            string AdSoyad = null;
            Unvan = Unvan.ToLower().Replace('ı', 'i');
            UnvanSoyad = UnvanSoyad.ToLower().Replace('ı', 'i');
            AdSoyad = Unvan + " " + UnvanSoyad;
            AdSoyad.Trim();
            List<int> polListId = new List<int>();
            //0 şahıs,1 firma
            if (durum == 0)
            {
                if (Unvan.Length <= 50 || UnvanSoyad.Length <= 50)
                {
                    polSigortaEttirenUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim() == Unvan
                    && s.SoyadiUnvan.Trim().ToLower() == UnvanSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != ""
                    || s.AdiUnvan.Trim().ToLower() == AdSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != "").ToList();
                    if (polSigortaEttirenUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortaEttirenUnvan;
                    }

                }

            }

            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapFirmaUnvan(string UnvanFirma, int tvmkodu, byte durum)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttirenFirmaUnvan = new List<PoliceSigortaEttiren>();

            UnvanFirma = UnvanFirma.ToLower().Replace('ı', 'i');

            UnvanFirma.Trim();
            List<int> polListId = new List<int>();
            //0 şahıs,1 firma

            if (durum == 1)
            {
                if (UnvanFirma.Length <= 150 || UnvanFirma.Length >= 3)
                {
                    polSigortaEttirenFirmaUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim().Contains(UnvanFirma)
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != ""
                    ).ToList();
                    if (polSigortaEttirenFirmaUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortaEttirenFirmaUnvan;
                    }


                }
            }

            return PolGenelList;
        }

        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapSatisKanaliAra(int tvmkodu, List<int> tvmlist)
        {
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<PoliceGenel> satisKanaliAra = new List<PoliceGenel>();
            satisKanaliAra = _PoliceContext.PoliceGenelRepository.All().Where(s => tvmlist.Contains(s.TaliAcenteKodu.Value)).ToList();

            if (satisKanaliAra.Count > 0)
            {
                PolGenelList.OfflineGenel = satisKanaliAra;
            }
            return PolGenelList;

        }

        public MuhasebeCariHesapServiceModel DevirMuhasebeCariHesapGrupKoduAra(int tvmkodu, string grupKodu)
        {
            grupKodu = grupKodu.ToLower().Replace('ı', 'i');
            grupKodu.Trim();
            MuhasebeCariHesapServiceModel PolGenelList = new MuhasebeCariHesapServiceModel();
            List<MusteriGenelBilgiler> musGrupKodu = new List<MusteriGenelBilgiler>();
            if (grupKodu.Length < 31)
            {
                musGrupKodu = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(s => s.TVMMusteriKodu.ToLower().Trim() == grupKodu && s.TVMKodu == tvmkodu).ToList<MusteriGenelBilgiler>();
                if (musGrupKodu.Count > 0)
                {
                    PolGenelList.OfflineMusteriGenel = musGrupKodu;
                }
            }

            return PolGenelList;
        }

        public class MuhasebeCariHesapServiceModel
        {
            public List<PoliceGenel> OfflineGenel { get; set; }
            public List<PoliceArac> OfflineAraclar { get; set; }
            public PoliceArac OfflineArac { get; set; }
            public List<PoliceTahsilat> TahsilatTckVn { get; set; }
            public List<PoliceSigortaEttiren> OfflineSigortaEttiren { get; set; }
            public List<PoliceSigortali> OfflineSigortali { get; set; }
            public List<MusteriGenelBilgiler> OfflineMusteriGenel { get; set; }

            public MuhasebeCariHesapServiceModel()
            {
                OfflineSigortali = new List<PoliceSigortali>();
                OfflineAraclar = new List<PoliceArac>();
                OfflineGenel = new List<PoliceGenel>();
                TahsilatTckVn = new List<PoliceTahsilat>();
                OfflineSigortaEttiren = new List<PoliceSigortaEttiren>();
                OfflineMusteriGenel = new List<MusteriGenelBilgiler>();
            }
        }

        #endregion

        #region Cari Hareket İşlemleri

        public List<CariHesaplari> getCariHesaplar()
        {
            List<CariHesaplari> list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.TVMKodu == _AktifKullanici.TVMKodu).ToList();
            return list;
        }

        public CariHareketleri CariHareketEkle(CariHareketleri hareket)
        {
            try
            {
                CariHareketleri cariHareket = _MuhasebeContext.CariHareketleriRepository.Create(hareket);
                _MuhasebeContext.Commit();

                #region CariHesapBorcAlacak tablosuna kayıt atılıyor

                this.CariBorcAlacakEkle(hareket);

                #endregion
                return cariHareket;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public CariHareketleri PolTahCariHareketEkle(CariHareketleri hareket)
        {
            try
            {
                CariHareketleri cariHareket = _MuhasebeContext.CariHareketleriRepository.Create(hareket);
                _MuhasebeContext.Commit();

                return cariHareket;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int CariBorcAlacakEkle(CariHareketleri hareket)
        {
            try
            {
                var borcAlacakVarMi = this.getCariHesapBA(hareket.CariHesapKodu, hareket.TVMKodu, hareket.CariHareketTarihi.Value.Year);
                if (borcAlacakVarMi != null)
                {
                    this.SetBorcAlacak(borcAlacakVarMi, hareket.CariHareketTarihi.Value, hareket.Tutar, hareket.BorcAlacakTipi);
                    _MuhasebeContext.CariHesapBorcAlacakRepository.Update(borcAlacakVarMi);
                    _MuhasebeContext.Commit();
                }
                else
                {
                    borcAlacakVarMi = new CariHesapBorcAlacak();
                    borcAlacakVarMi.TVMKodu = hareket.TVMKodu;
                    borcAlacakVarMi.KimlikNo = "";
                    borcAlacakVarMi.KayitTarihi = TurkeyDateTime.Now;
                    borcAlacakVarMi.Donem = hareket.CariHareketTarihi.Value.Year;
                    borcAlacakVarMi.CariHesapKodu = hareket.CariHesapKodu;
                    this.SetBorcAlacak(borcAlacakVarMi, hareket.CariHareketTarihi.Value, hareket.Tutar, hareket.BorcAlacakTipi);
                    _MuhasebeContext.CariHesapBorcAlacakRepository.Create(borcAlacakVarMi);
                    _MuhasebeContext.Commit();
                }

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public CariHareketleri getCariHareketDetay(int id)
        {
            CariHareketleri hareketDetay = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.Id == id).FirstOrDefault();
            return hareketDetay;
        }
        public bool deleteCariHareket(string carihesapkodu, int TVMKodu, string evrakNo, string borcAlacakTipi)
        {
            try
            {
                _MuhasebeContext.CariHareketleriRepository.Delete(w => w.TVMKodu == TVMKodu && w.CariHesapKodu == carihesapkodu && w.EvrakNo == evrakNo && w.BorcAlacakTipi == borcAlacakTipi);
                _MuhasebeContext.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public CariHesapBorcAlacak getCariHesapBA(string cariHesapKodu, int tvmKodu, int donem)
        {
            CariHesapBorcAlacak borcAlacak = new CariHesapBorcAlacak();
            borcAlacak = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(w => w.CariHesapKodu == cariHesapKodu
            && w.TVMKodu == tvmKodu && w.Donem == donem
            ).FirstOrDefault();
            return borcAlacak;
        }

        public List<CariEvrakTipleri> getCariEvrakTipler()
        {
            List<CariEvrakTipleri> list = _MuhasebeContext.CariEvrakTipleriRepository.All().ToList();
            return list;
        }
        public string getCariEvrakTip(int kodu)
        {
            string evrakTipi = "";
            var evrakTip = _MuhasebeContext.CariEvrakTipleriRepository.All().Where(w => w.Kodu == kodu).FirstOrDefault();
            if (evrakTip != null)
            {
                evrakTipi = evrakTip.Aciklama;
            }
            return evrakTipi;
        }
        public string getCariOdemeTip(int kodu)
        {
            string odemeTipi = "";
            var odemeTip = _MuhasebeContext.CariOdemeTipleriRepository.All().Where(w => w.Kodu == kodu).FirstOrDefault();
            if (odemeTip != null)
            {
                odemeTipi = odemeTip.Aciklama;
            }
            return odemeTipi;
        }

        public class CariHesapHareketListModel
        {
            public string OdemeTipi { get; set; }
            public DateTime? OdemeTarihi { get; set; }
            public string Aciklama { get; set; }
            public string ParaBirimi { get; set; }
            public decimal? Borc { get; set; }
            public decimal? Alacak { get; set; }
            public string EvrakNo { get; set; }
            public string EvrakTipi { get; set; }
            public DateTime VadeTarihi { get; set; }
            public string BorcTipi { get; set; }
            public string CariHesapAdiKodu { get; set; }
            public string MusteriGrupkodu { get; set; }
            public string kimlikNo { get; set; }
            public int id { get; set; }


        }
        public List<CariHesapHareketListModel> CariHesapHareketKontrolListesi(string hesapkodu, int tvmkodu, int donem, string musteriGrupKodu, byte aramaTipi, Boolean aramaDurumu, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            var evrakTipleri = _MuhasebeContext.CariEvrakTipleriRepository.All();
            var odemeTipleri = _MuhasebeContext.CariOdemeTipleriRepository.All();
            List<CariHareketleri> CariHareketler = new List<CariHareketleri>();
            if (aramaTipi == 1 && aramaDurumu) //Müşteri Grup Kodu
            {
                if (!String.IsNullOrEmpty(musteriGrupKodu))
                {
                    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value.Year == donem && w.MusteriGrupKodu.StartsWith(musteriGrupKodu)).ToList();
                    var musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMMusteriKodu == musteriGrupKodu && w.TVMKodu == tvmkodu).Select(s => s.KimlikNo).ToList();

                }
            }
            else if (aramaTipi == 0 && aramaDurumu)
            {
                if (!String.IsNullOrEmpty(hesapkodu))
                {
                    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.CariHesapKodu == hesapkodu && w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value.Year == donem).ToList();
                }
            }
            else if (!aramaDurumu)
            {
                if (!String.IsNullOrEmpty(baslangicTarihi.ToShortDateString()) && !String.IsNullOrEmpty(bitisTarihi.ToShortDateString()))
                {
                    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value >= baslangicTarihi && w.CariHareketTarihi.Value <= bitisTarihi).ToList();
                }
            }
            List<CariHesapHareketListModel> list = new List<CariHesapHareketListModel>();
            CariHesapHareketListModel listItem = new CariHesapHareketListModel();
            if (CariHareketler != null && CariHareketler.Count > 0)
            {
                foreach (var item in CariHareketler)
                {
                    listItem = new CariHesapHareketListModel();
                    listItem.Aciklama = item.Aciklama;

                    if (item.BorcAlacakTipi == "A")
                    {
                        listItem.Alacak = item.Tutar;
                        listItem.Borc = 0;
                    }
                    else if (item.BorcAlacakTipi == "B")
                    {
                        listItem.Borc = item.Tutar;
                        listItem.Alacak = 0;
                    }
                    listItem.BorcTipi = item.BorcAlacakTipi;
                    listItem.EvrakNo = item.EvrakNo;
                    var evrakDetay = evrakTipleri.Where(w => w.Kodu == item.EvrakTipi).FirstOrDefault();
                    if (item.OdemeTipi != null)
                    {
                        var odemeDetay = odemeTipleri.Where(w => w.Kodu == item.OdemeTipi).FirstOrDefault();
                        listItem.OdemeTipi = odemeDetay == null ? "" : odemeDetay.Aciklama;
                    }
                    else
                    {
                        listItem.OdemeTipi = "";
                    }

                    listItem.MusteriGrupkodu = item.MusteriGrupKodu;
                    if (evrakDetay != null)
                    {
                        listItem.EvrakTipi = evrakDetay.Aciklama;
                    }
                    else
                    {
                        listItem.EvrakTipi = "";
                    }

                    listItem.OdemeTarihi = item.OdemeTarihi;
                    listItem.VadeTarihi = item.CariHareketTarihi.HasValue ? item.CariHareketTarihi.Value : TurkeyDateTime.Now;
                    listItem.ParaBirimi = item.DovizTipi;
                    listItem.CariHesapAdiKodu = item.CariHesapKodu;
                    listItem.id = item.Id;
                    list.Add(listItem);
                }
            }

            //if (list != null && list.Count > 0)
            //{
            //    list = list.OrderBy(w => w.VadeTarihi).ToList();
            //}
            return list;
        }


        public List<CariHesapGelirGiderListModel> CariHesapGelirGiderListesi(int tvmkodu, string CariHesapTip, int donem, DateTime baslangicTarihi, DateTime bitisTarihi, int aramaTip)
        {
            var evrakTipleri = _MuhasebeContext.CariEvrakTipleriRepository.All();
            var odemeTipleri = _MuhasebeContext.CariOdemeTipleriRepository.All();
            List<CariHareketleri> CariHareketler = new List<CariHareketleri>();

            //if (aramaTip == 0 )
            //{
            //    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value.Year == donem && (w.CariHesapKodu.StartsWith(CariHesapTip))).ToList();
            //}

            if (aramaTip == 0 || aramaTip == 1 || aramaTip == 2)
            {
                CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value >= baslangicTarihi && w.CariHareketTarihi.Value <= bitisTarihi && (w.CariHesapKodu.StartsWith(CariHesapTip))).ToList();
            }





            List<CariHesapGelirGiderListModel> list = new List<CariHesapGelirGiderListModel>();
            CariHesapGelirGiderListModel listItem = new CariHesapGelirGiderListModel();
            if (CariHareketler != null && CariHareketler.Count > 0)
            {
                foreach (var item in CariHareketler)
                {
                    listItem = new CariHesapGelirGiderListModel();
                    listItem.Aciklama = item.Aciklama;

                    if (item.BorcAlacakTipi == "A")
                    {
                        listItem.Alacak = item.Tutar;
                        listItem.Borc = 0;
                    }
                    else if (item.BorcAlacakTipi == "B")
                    {
                        listItem.Borc = item.Tutar;
                        listItem.Alacak = 0;
                    }
                    listItem.BorcTipi = item.BorcAlacakTipi;
                    listItem.EvrakNo = item.EvrakNo;
                    var evrakDetay = evrakTipleri.Where(w => w.Kodu == item.EvrakTipi).FirstOrDefault();
                    if (item.OdemeTipi != null)
                    {
                        var odemeDetay = odemeTipleri.Where(w => w.Kodu == item.OdemeTipi).FirstOrDefault();
                        listItem.OdemeTipi = odemeDetay == null ? "" : odemeDetay.Aciklama;
                    }
                    else
                    {
                        listItem.OdemeTipi = "";
                    }

                    listItem.MusteriGrupkodu = item.MusteriGrupKodu;
                    if (evrakDetay != null)
                    {
                        listItem.EvrakTipi = evrakDetay.Aciklama;
                    }
                    else
                    {
                        listItem.EvrakTipi = "";
                    }

                    listItem.OdemeTarihi = item.OdemeTarihi;
                    listItem.VadeTarihi = item.CariHareketTarihi.HasValue ? item.CariHareketTarihi.Value : TurkeyDateTime.Now;
                    listItem.ParaBirimi = item.DovizTipi;

                    listItem.CariHesapAdiKodu = item.CariHesapKodu;
                    listItem.id = item.Id;
                    list.Add(listItem);
                }
            }

            //if (list != null && list.Count > 0)
            //{
            //    list = list.OrderBy(w => w.VadeTarihi).ToList();
            //}
            return list;
        }
        public string GelirGiderCreatePDF(GelirGiderTablosuAnaModel model)
        {
            PDFHelper pdf = null;
            try
            {
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                string template = PdfTemplates.GetTemplate(rootPath + "Content/templates/", PdfTemplates.GELIR_GIDER);

                pdf = new PDFHelper("NeoOnline", "GELİR GİDER TABLOSU - İKİ TARİH ARALIĞI", "CARİ HESAP EKSTRESİ", 8, rootPath + "Content/fonts/");

                // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                pdf.SetPageEventHelper(new PDFCustomEventHelper());
                pdf.Rotate();
                PDFParser parser = new PDFParser(template, pdf);

                string cariHesapAdi = "";
                string donem = "";
                if (model.dict.Count > 0)
                {
                    foreach (var cariHesap in model.dict.ToList())
                    {
                        string fiyatSatirTemplate = parser.GetTemplate("EkstreSatiri");
                        decimal yuruyenEkstreBakiye = 0;
                        foreach (var cariHareket in cariHesap.Value.list)
                        {
                            #region satırlar
                            string listSatir = String.Empty;

                            listSatir = listSatir.Replace("$CariHesapKodu$", cariHesap.Key);
                            listSatir = listSatir.Replace("$HesapAdı$", cariHesap.Value.CariHesapAdi);

                            listSatir = listSatir.Replace("$ParaBirimi$", cariHesap.Value.paraBirimi);
                            listSatir = listSatir.Replace("$Borc$", cariHesap.Value.BorcToplam.HasValue ? cariHesap.Value.BorcToplam.Value.ToString("N2") : "");
                            listSatir = listSatir.Replace("$Alacak$", cariHesap.Value.AlacakToplam.HasValue ? cariHesap.Value.AlacakToplam.Value.ToString("N2") : "");
                            yuruyenEkstreBakiye += cariHareket.Borc ?? 0 - cariHareket.Alacak ?? 0;
                            listSatir = listSatir.Replace("$Bakiye$", yuruyenEkstreBakiye.ToString("N2"));
                            if (yuruyenEkstreBakiye < 0)
                            {
                                listSatir = listSatir.Replace("$BA$", "A");
                            }
                            else
                            {
                                listSatir = listSatir.Replace("$BA$", "B");
                            }
                            parser.AppendToPlaceHolder("EkstreSatirlari", listSatir);
                            #endregion

                            #region tablo
                            string cariHesapTemplate = parser.GetTemplate("CariHesapTablo");

                            var hesapBilgi = this.GetCariHesapAdi(cariHesap.Key);
                            cariHesapAdi = hesapBilgi.Item1 + " " + hesapBilgi.Item2;
                            cariHesapTemplate = cariHesapTemplate.Replace("$CariHesapAdi$", cariHesapAdi);

                            donem = model.Donem.ToString();
                            cariHesapTemplate = cariHesapTemplate.Replace("$Donem$", "DÖNEM -" + donem);
                            cariHesapTemplate = cariHesapTemplate.Replace("$ListeAdi$", "CARİ HESAP EKSTRESİ");
                            cariHesapTemplate = cariHesapTemplate.Replace("$TVMUnvani$", _AktifKullanici.TVMUnvani);

                            var ekstreBorcToplam = cariHesap.Value.list.Select(s => s.Borc).Sum();
                            var ekstreAlacakToplam = cariHesap.Value.list.Select(s => s.Alacak).Sum();
                            var ekstreBakiyeToplam = ekstreBorcToplam - ekstreAlacakToplam;
                            cariHesapTemplate = cariHesapTemplate.Replace("$CEBorcToplam$", ekstreBorcToplam.HasValue ? ekstreBorcToplam.Value.ToString("N2") : "");
                            cariHesapTemplate = cariHesapTemplate.Replace("$CEAlacakToplam$", ekstreAlacakToplam.HasValue ? ekstreAlacakToplam.Value.ToString("N2") : "");
                            cariHesapTemplate = cariHesapTemplate.Replace("$CEBakiyeToplam$", ekstreBakiyeToplam.HasValue ? ekstreBakiyeToplam.Value.ToString("N2") : "");
                            if (ekstreBakiyeToplam < 0)
                            {
                                cariHesapTemplate = cariHesapTemplate.Replace("$CEBAGenel$", "A");
                            }
                            else
                            {
                                cariHesapTemplate = cariHesapTemplate.Replace("$CEBAGenel$", "B");
                            }
                            parser.AppendToPlaceHolder("CariHesapTablolari", cariHesapTemplate);
                            parser.ReplacePlaceHolder("EkstreSatirlari", "");
                            #endregion
                        }
                    }
                }

                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();
                ICariHesapEkstrePDFStorage storage = DependencyResolver.Current.GetService<ICariHesapEkstrePDFStorage>();
                string dosyaAdi = cariHesapAdi;
                string fileName = String.Format(dosyaAdi + "_" + donem + "-Dönemi_GelirGiderTablosu_{0}.pdf", System.Guid.NewGuid().ToString());
                string url = storage.UploadFile(fileName, fileData);
                if (!String.IsNullOrEmpty(url))
                {
                    return url;
                }
                else
                {
                    return "hata";
                }
            }
            catch (Exception ex)
            {
                return "hata";
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }


        public bool cariHareketVarMi()
        {
            //var  _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.Id == id).FirstOrDefault();
            return false;
        }

        public List<CariOdemeTipleri> getCariOdemeTipleriList()
        {
            List<CariOdemeTipleri> list = new List<CariOdemeTipleri>();
            list = _MuhasebeContext.CariOdemeTipleriRepository.All().ToList();
            return list;
        }
        #endregion

        #region Cari Hesap İşlemleri

        public int CariHesapEkle(CariHesaplari hesap)
        {
            try
            {
                _MuhasebeContext.CariHesaplariRepository.Create(hesap);
                _MuhasebeContext.Commit();

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int CariHesapBorcAlacakKayitEkle(CariHesapBorcAlacak borcAlacakEkle)
        {
            try
            {
                borcAlacakEkle.KimlikNo = borcAlacakEkle.KimlikNo.Trim();
                _MuhasebeContext.CariHesapBorcAlacakRepository.Create(borcAlacakEkle);
                _MuhasebeContext.Commit();
                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public CariHesapBorcAlacak SetBorcAlacak(CariHesapBorcAlacak borcAlacak, DateTime hareketTarihi, decimal Tutar, string BorcAlacakTipi)
        {

            var ay = hareketTarihi.Month;
            switch (ay)
            {
                case 1:
                    {
                        if (BorcAlacakTipi == "B")
                        {

                            if (borcAlacak.Borc1 == null)
                            {
                                borcAlacak.Borc1 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc1 += Tutar;
                            }

                        }
                        else
                        {
                            if (borcAlacak.Alacak1 == null)
                            {
                                borcAlacak.Alacak1 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak1 += Tutar;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc2 == null)
                            {
                                borcAlacak.Borc2 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc2 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak2 == null)
                            {
                                borcAlacak.Alacak2 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak2 += Tutar;
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc3 == null)
                            {
                                borcAlacak.Borc3 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc3 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak3 == null)
                            {
                                borcAlacak.Alacak3 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak3 += Tutar;
                            }
                        }
                    }
                    break;
                case 4:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc4 == null)
                            {
                                borcAlacak.Borc4 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc4 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak4 == null)
                            {
                                borcAlacak.Alacak4 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak4 += Tutar;
                            }
                        }
                    }
                    break;
                case 5:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc5 == null)
                            {
                                borcAlacak.Borc5 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc5 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak5 == null)
                            {
                                borcAlacak.Alacak5 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak5 += Tutar;
                            }
                        }
                    }
                    break;
                case 6:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc6 == null)
                            {
                                borcAlacak.Borc6 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc6 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak6 == null)
                            {
                                borcAlacak.Alacak6 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak6 += Tutar;
                            }
                        }
                    }
                    break;
                case 7:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc7 == null)
                            {
                                borcAlacak.Borc7 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc7 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak7 == null)
                            {
                                borcAlacak.Alacak7 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak7 += Tutar;
                            }
                        }

                    }
                    break;
                case 8:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc8 == null)
                            {
                                borcAlacak.Borc8 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc8 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak8 == null)
                            {
                                borcAlacak.Alacak8 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak8 += Tutar;
                            }
                        }

                    }
                    break;

                case 9:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc9 == null)
                            {
                                borcAlacak.Borc9 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc9 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak9 == null)
                            {
                                borcAlacak.Alacak9 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak9 += Tutar;
                            }
                        }
                    }
                    break;
                case 10:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc10 == null)
                            {
                                borcAlacak.Borc10 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc10 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak10 == null)
                            {
                                borcAlacak.Alacak10 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak10 += Tutar;
                            }
                        }
                    }
                    break;
                case 11:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc11 == null)
                            {
                                borcAlacak.Borc11 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc11 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak11 == null)
                            {
                                borcAlacak.Alacak11 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak11 += Tutar;
                            }
                        }
                    }
                    break;
                case 12:
                    {
                        if (BorcAlacakTipi == "B")
                        {
                            if (borcAlacak.Borc12 == null)
                            {
                                borcAlacak.Borc12 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Borc12 += Tutar;
                            }
                        }
                        else
                        {
                            if (borcAlacak.Alacak12 == null)
                            {
                                borcAlacak.Alacak12 = Tutar;
                            }
                            else
                            {
                                borcAlacak.Alacak12 += Tutar;
                            }
                        }
                    }
                    break;

            }

            return borcAlacak;
        }
        public CariHesaplari GetCariDetayYetkili(int id)
        {
            CariHesaplari detay = new CariHesaplari();
            if (id != null)
            {
                detay = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapId == id).FirstOrDefault();
            }
            return detay;
        }
        public List<CariHesaplari> GetCariHesapList(string CariHesapAdi)
        {
            List<CariHesaplari> hesapList = new List<CariHesaplari>();
            if (!String.IsNullOrEmpty(CariHesapAdi))
            {
                //CariHesapAdi += "%";
                CariHesapAdi = CariHesapAdi.ToLower().Replace('ı', 'i').Trim();
                hesapList = _MuhasebeContext.CariHesaplariRepository.All().Where(shop => shop.Unvan.ToLower().Trim().StartsWith(CariHesapAdi) && shop.TVMKodu == _AktifKullanici.TVMKodu).ToList();
                if (hesapList != null)
                {
                    foreach (var item in hesapList)
                    {
                        item.Unvan = item.Unvan + " " + item.CariHesapKodu;
                    }
                }

            }
            return hesapList;
        }
        public List<CariHesaplari> GetCariKasaHesapList(string kasaHesabiKodu)
        {
            List<CariHesaplari> hesapList = new List<CariHesaplari>();
            if (!String.IsNullOrEmpty(kasaHesabiKodu))
            {
                hesapList = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapTipi == kasaHesabiKodu && s.TVMKodu == _AktifKullanici.TVMKodu).ToList();

            }
            return hesapList;
        }
        public List<CariHesaplari> GetCariHesapAcenteBankaPosKasaHesapList(string acenteKK)
        {
            List<CariHesaplari> hesapList = new List<CariHesaplari>();
            if (!String.IsNullOrEmpty(acenteKK))
            {
                hesapList = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapKodu == acenteKK && s.TVMKodu == _AktifKullanici.TVMKodu).ToList();

            }
            return hesapList;
        }
        public Tuple<string, string> GetCariHesapAdi(string cariHesapKodu)
        {
            string cariHesabınKodu = "";
            string cariHesapUnvani = "";
            if (!String.IsNullOrEmpty(cariHesapKodu))
            {
                try
                {
                    var hesap = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapKodu == cariHesapKodu && s.TVMKodu == _AktifKullanici.TVMKodu).FirstOrDefault();//HESAP BULUNAMADIGI ZAMAN HESAP OLUŞTURULACAK SEKİLDE DÜZENLEME YAPILACAK
                    cariHesapUnvani = hesap.Unvan;
                    cariHesabınKodu = hesap.CariHesapKodu;
                }
                catch (Exception)
                {
                    cariHesabınKodu = "";
                    cariHesapUnvani = "";
                }


            }
            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }
        public Tuple<string, string> GetCariHesapAdiByTCKN(string TCKN)
        {
            string cariHesabınKodu = "";
            string cariHesapUnvani = "";
            if (!String.IsNullOrEmpty(TCKN))
            {
                var hesap = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.KimlikNo == TCKN && s.TVMKodu == _AktifKullanici.TVMKodu).FirstOrDefault();

                if (hesap != null)
                {
                    cariHesapUnvani = hesap.Unvan;
                    cariHesabınKodu = hesap.CariHesapKodu;
                    return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
                }


            }
            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }

        public Tuple<string, string> GetCariHesapByVKN(string VKN, int policeId)
        {
            string cariHesabınKodu = "";
            string cariHesapUnvani = "";
            var aktarilanPolice = _PoliceContext.PoliceGenelRepository.FindById(policeId);


            if (!String.IsNullOrEmpty(VKN))
            {

                var policeParaBirimi = _TVMContext.ParaBirimleriRepository.All().Where(paraBirimi => paraBirimi.Birimi == aktarilanPolice.ParaBirimi).FirstOrDefault();
                string policeParaBirimiKodu = policeParaBirimi.Id.ToString().PadLeft(2, '0');
                var anaTvmKodu = Convert.ToInt32(aktarilanPolice.TVMDetay.Kodu.ToString().Substring(0, 3));
                var hesap = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.KimlikNo == VKN && s.TVMKodu == anaTvmKodu && s.CariHesapTipi == "120." + policeParaBirimiKodu + ".").FirstOrDefault();

                //Cari hesap varsa olan kaydın unvanını ve kodunu döndür.
                if (hesap != null && hesap.CariHesapTipi.Contains(policeParaBirimiKodu))
                {
                    cariHesapUnvani = hesap.Unvan;
                    cariHesabınKodu = hesap.CariHesapKodu;
                    return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
                }
                // Yoksa Cari hesap ve mizan tablosunu(CariHesapBorcAlacak) oluştur ve cari hesap unvanini ve kodunu döndür.
                else
                {
                    var TVMKodu = aktarilanPolice.TVMDetay.Kodu;
                    var TVMUnvani = aktarilanPolice.TVMDetay.Unvani;

                    int anaAcenteTvmKodu = Convert.ToInt32(TVMKodu.ToString().Substring(0, 3));

                    try
                    {
                        CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                        CariHesaplari cariHesapEkle = new CariHesaplari();
                        cariHesapEkle.TVMKodu = anaAcenteTvmKodu;
                        cariHesapEkle.TVMUnvani = TVMUnvani;
                        cariHesapEkle.UlkeKodu = aktarilanPolice.PoliceSigortaEttiren.UlkeKodu;
                        cariHesapEkle.IlKodu = aktarilanPolice.PoliceSigortaEttiren.IlKodu;
                        cariHesapEkle.IlceKodu = aktarilanPolice.PoliceSigortaEttiren.IlceKodu.HasValue ? aktarilanPolice.PoliceSigortaEttiren.IlceKodu.Value : 0;
                        cariHesapEkle.KayitTarihi = DateTime.Now;
                        cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                        cariHesapEkle.Adres = aktarilanPolice.PoliceSigortaEttiren.Adres;
                        if (!String.IsNullOrEmpty(aktarilanPolice.PoliceSigortaEttiren.AdiUnvan))
                        {
                            cariHesapEkle.Unvan = aktarilanPolice.PoliceSigortaEttiren.AdiUnvan.Trim();
                        }
                        if (!String.IsNullOrEmpty(aktarilanPolice.PoliceSigortaEttiren.SoyadiUnvan))
                        {
                            cariHesapEkle.Unvan += " " + aktarilanPolice.PoliceSigortaEttiren.SoyadiUnvan.Trim();
                        }
                        cariHesapEkle.Unvan += String.Format(" ({0})", aktarilanPolice.ParaBirimi);
                        cariHesapEkle.Telefon1 = aktarilanPolice.PoliceSigortaEttiren.TelefonNo;
                        cariHesapEkle.CepTel = aktarilanPolice.PoliceSigortaEttiren.MobilTelefonNo;
                        cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                        cariHesapEkle.Email = aktarilanPolice.PoliceSigortaEttiren.EMail;
                        cariHesapEkle.PostaKodu = aktarilanPolice.PoliceSigortaEttiren.PostaKodu.HasValue ? aktarilanPolice.PoliceSigortaEttiren.PostaKodu.Value : 0;
                        cariHesapEkle.KimlikNo = VKN;
                        cariHesapEkle.CariHesapTipi = "120." + policeParaBirimiKodu + ".";
                        cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + VKN;
                        var musteriDetay = _MusteriService.GetMusteri(VKN, TVMKodu);
                        if (musteriDetay != null)
                        {
                            if (!String.IsNullOrEmpty(musteriDetay.TVMMusteriKodu))
                            {
                                cariHesapEkle.MusteriGrupKodu = musteriDetay.TVMMusteriKodu;
                            }
                        }

                        var musteriHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                        _MuhasebeContext.Commit();

                        //Mizan Tablosu oluşturma
                        if (musteriHesap.CariHesapKodu != null)
                        {
                            cariHesapBorcAlacakEkle.CariHesapKodu = musteriHesap.CariHesapKodu;
                            cariHesapBorcAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                            cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                            cariHesapBorcAlacakEkle.KimlikNo = VKN;
                            cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                            cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                            cariHesapBorcAlacakEkle.Alacak1 = 0;
                            cariHesapBorcAlacakEkle.Borc1 = 0;
                            cariHesapBorcAlacakEkle.Alacak2 = 0;
                            cariHesapBorcAlacakEkle.Borc2 = 0;
                            cariHesapBorcAlacakEkle.Alacak3 = 0;
                            cariHesapBorcAlacakEkle.Borc3 = 0;
                            cariHesapBorcAlacakEkle.Alacak4 = 0;
                            cariHesapBorcAlacakEkle.Borc4 = 0;
                            cariHesapBorcAlacakEkle.Alacak5 = 0;
                            cariHesapBorcAlacakEkle.Borc5 = 0;
                            cariHesapBorcAlacakEkle.Alacak6 = 0;
                            cariHesapBorcAlacakEkle.Borc6 = 0;
                            cariHesapBorcAlacakEkle.Alacak7 = 0;
                            cariHesapBorcAlacakEkle.Borc7 = 0;
                            cariHesapBorcAlacakEkle.Alacak8 = 0;
                            cariHesapBorcAlacakEkle.Borc8 = 0;
                            cariHesapBorcAlacakEkle.Alacak9 = 0;
                            cariHesapBorcAlacakEkle.Borc9 = 0;
                            cariHesapBorcAlacakEkle.Alacak10 = 0;
                            cariHesapBorcAlacakEkle.Borc10 = 0;
                            cariHesapBorcAlacakEkle.Alacak11 = 0;
                            cariHesapBorcAlacakEkle.Borc11 = 0;
                            cariHesapBorcAlacakEkle.Alacak12 = 0;
                            cariHesapBorcAlacakEkle.Borc12 = 0;
                            cariHesapBorcAlacakEkle.KimlikNo = cariHesapBorcAlacakEkle.KimlikNo.Trim();
                            _MuhasebeContext.CariHesapBorcAlacakRepository.Create(cariHesapBorcAlacakEkle);
                            _MuhasebeContext.Commit();

                        }
                        cariHesapUnvani = cariHesapEkle.Unvan;
                        cariHesabınKodu = cariHesapEkle.CariHesapKodu;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
                }

            }
            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }

        public Tuple<string, string> GetBankaHesapByVKN(string VKN, int policeId)
        {
            string cariHesabınKodu = "";
            string cariHesapUnvani = "";
            var aktarilanPolice = _PoliceContext.PoliceGenelRepository.FindById(policeId);
            VKN = aktarilanPolice.TVMDetay.VergiNumarasi;

            if (!String.IsNullOrEmpty(VKN))
            {
                var hesap = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.KimlikNo == VKN && s.TVMKodu == aktarilanPolice.TVMDetay.Kodu).FirstOrDefault();
                //Cari hesap varsa olan kaydın unvanını ve kodunu döndür.
                if (hesap != null)
                {
                    cariHesapUnvani = hesap.Unvan;
                    cariHesabınKodu = hesap.CariHesapKodu;
                    return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
                }
                // Yoksa Cari hesap ve mizan tablosunu(CariHesapBorcAlacak) oluştur ve cari hesap unvanini ve kodunu döndür.
                else
                {
                    var TVMKodu = aktarilanPolice.TVMDetay.Kodu;
                    var TVMUnvani = aktarilanPolice.TVMDetay.Unvani;

                    try
                    {
                        CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                        CariHesaplari cariHesapEkle = new CariHesaplari();
                        cariHesapEkle.TVMKodu = TVMKodu;
                        cariHesapEkle.TVMUnvani = TVMUnvani;
                        cariHesapEkle.UlkeKodu = aktarilanPolice.TVMDetay.UlkeKodu;
                        cariHesapEkle.IlKodu = aktarilanPolice.TVMDetay.IlKodu;
                        cariHesapEkle.IlceKodu = aktarilanPolice.TVMDetay.IlceKodu.HasValue ? aktarilanPolice.TVMDetay.IlceKodu.Value : 0;
                        cariHesapEkle.KayitTarihi = DateTime.Now;
                        cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                        cariHesapEkle.Adres = aktarilanPolice.TVMDetay.Adres;
                        if (!String.IsNullOrEmpty(aktarilanPolice.TVMDetay.Unvani))
                        {
                            cariHesapEkle.Unvan = aktarilanPolice.TVMDetay.Unvani;
                        }


                        cariHesapEkle.Telefon1 = aktarilanPolice.TVMDetay.Telefon;
                        cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                        cariHesapEkle.Email = aktarilanPolice.TVMDetay.Email;
                        cariHesapEkle.KimlikNo = VKN;
                        cariHesapEkle.CariHesapTipi = "100.01.";
                        cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + VKN;
                        var musteriDetay = _MusteriService.GetMusteri(VKN, TVMKodu);
                        if (musteriDetay != null)
                        {
                            if (!String.IsNullOrEmpty(musteriDetay.TVMMusteriKodu))
                            {
                                cariHesapEkle.MusteriGrupKodu = musteriDetay.TVMMusteriKodu;
                            }
                        }

                        var musteriHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                        _MuhasebeContext.Commit();

                        //Mizan Tablosu oluşturma
                        if (musteriHesap.CariHesapKodu != null)
                        {
                            cariHesapBorcAlacakEkle.CariHesapKodu = musteriHesap.CariHesapKodu;
                            cariHesapBorcAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                            cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                            cariHesapBorcAlacakEkle.KimlikNo = VKN;
                            cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                            cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                            cariHesapBorcAlacakEkle.Alacak1 = 0;
                            cariHesapBorcAlacakEkle.Borc1 = 0;
                            cariHesapBorcAlacakEkle.Alacak2 = 0;
                            cariHesapBorcAlacakEkle.Borc2 = 0;
                            cariHesapBorcAlacakEkle.Alacak3 = 0;
                            cariHesapBorcAlacakEkle.Borc3 = 0;
                            cariHesapBorcAlacakEkle.Alacak4 = 0;
                            cariHesapBorcAlacakEkle.Borc4 = 0;
                            cariHesapBorcAlacakEkle.Alacak5 = 0;
                            cariHesapBorcAlacakEkle.Borc5 = 0;
                            cariHesapBorcAlacakEkle.Alacak6 = 0;
                            cariHesapBorcAlacakEkle.Borc6 = 0;
                            cariHesapBorcAlacakEkle.Alacak7 = 0;
                            cariHesapBorcAlacakEkle.Borc7 = 0;
                            cariHesapBorcAlacakEkle.Alacak8 = 0;
                            cariHesapBorcAlacakEkle.Borc8 = 0;
                            cariHesapBorcAlacakEkle.Alacak9 = 0;
                            cariHesapBorcAlacakEkle.Borc9 = 0;
                            cariHesapBorcAlacakEkle.Alacak10 = 0;
                            cariHesapBorcAlacakEkle.Borc10 = 0;
                            cariHesapBorcAlacakEkle.Alacak11 = 0;
                            cariHesapBorcAlacakEkle.Borc11 = 0;
                            cariHesapBorcAlacakEkle.Alacak12 = 0;
                            cariHesapBorcAlacakEkle.Borc12 = 0;
                            cariHesapBorcAlacakEkle.KimlikNo = cariHesapBorcAlacakEkle.KimlikNo.Trim();
                            _MuhasebeContext.CariHesapBorcAlacakRepository.Create(cariHesapBorcAlacakEkle);
                            _MuhasebeContext.Commit();

                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
                }

            }
            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }


        public CariHesaplari GetCariHesap(string CariHesapKodu)
        {
            var hesap = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapKodu == CariHesapKodu && s.TVMKodu == _AktifKullanici.TVMKodu).FirstOrDefault();
            return hesap;
        }

        public Tuple<string, string> CreateCariHesap(string cariHesapKodu, string TCKN, string unvan, string adres, string ulkeKodu, string ilKodu, int? ilceKodu, string telefon, string cepTel, string email, int? postaKodu, string MusteriGrupKodu)
        {
            string cariHesabınKodu = "";
            string cariHesapUnvani = "";

            CariHesaplari hesap = new CariHesaplari();
            CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
            try
            {
                int ilceKodu2 = 0;
                int postaKodu2 = 0;
                if (ilceKodu == null)
                    ilceKodu2 = 0;
                else
                    ilceKodu2 = (int)ilceKodu;
                if (postaKodu == null)
                    postaKodu2 = 0;
                else
                    postaKodu2 = (int)postaKodu;
                hesap.Unvan = unvan;
                hesap.KimlikNo = TCKN;
                hesap.CariHesapKodu = "120.01." + TCKN;
                hesap.Adres = adres;
                hesap.CariHesapTipi = "120.01.";
                hesap.TVMKodu = _AktifKullanici.TVMKodu;
                hesap.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                hesap.UlkeKodu = ulkeKodu;
                if (ulkeKodu == null)
                    hesap.UlkeKodu = "TUR";
                hesap.IlKodu = ilKodu;
                hesap.IlceKodu = ilceKodu2;
                hesap.KayitTarihi = DateTime.Now;
                hesap.GuncellemeTarihi = DateTime.Now;
                hesap.Telefon1 = telefon;
                hesap.CepTel = cepTel;
                hesap.Email = email;
                hesap.PostaKodu = postaKodu2;
                hesap.MusteriGrupKodu = MusteriGrupKodu;
                hesap.BilgiNotu = "Poliçe Tahsilat ekranından yaratıldı.";

                //Mükerrer kayıt oluşmamamsı için Cari kod eklimi kontrolü yapılıyor
                if (this.GetCariHesap(hesap.CariHesapKodu) == null)
                {
                    _MuhasebeContext.CariHesaplariRepository.Create(hesap);
                    _MuhasebeContext.Commit();

                    if (hesap.CariHesapKodu != null)
                    {
                        cariHesapBorcAlacakEkle.CariHesapKodu = hesap.CariHesapKodu;
                        cariHesapBorcAlacakEkle.CariHesapId = hesap.CariHesapId;
                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapBorcAlacakEkle.KimlikNo = hesap.KimlikNo;
                        cariHesapBorcAlacakEkle.TVMKodu = hesap.TVMKodu;
                        cariHesapBorcAlacakEkle.Donem = Convert.ToInt32(TurkeyDateTime.Now.Year);
                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                        cariHesapBorcAlacakEkle.Borc1 = 0;
                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                        cariHesapBorcAlacakEkle.Borc2 = 0;
                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                        cariHesapBorcAlacakEkle.Borc3 = 0;
                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                        cariHesapBorcAlacakEkle.Borc4 = 0;
                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                        cariHesapBorcAlacakEkle.Borc5 = 0;
                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                        cariHesapBorcAlacakEkle.Borc6 = 0;
                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                        cariHesapBorcAlacakEkle.Borc7 = 0;
                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                        cariHesapBorcAlacakEkle.Borc8 = 0;
                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                        cariHesapBorcAlacakEkle.Borc9 = 0;
                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                        cariHesapBorcAlacakEkle.Borc10 = 0;
                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                        cariHesapBorcAlacakEkle.Borc11 = 0;
                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                        cariHesapBorcAlacakEkle.Borc12 = 0;
                        CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);

                        cariHesapBorcAlacakEkle.CariHesapKodu = hesap.CariHesapKodu;
                        cariHesapBorcAlacakEkle.CariHesapId = hesap.CariHesapId;
                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapBorcAlacakEkle.KimlikNo = hesap.KimlikNo;
                        cariHesapBorcAlacakEkle.TVMKodu = hesap.TVMKodu;
                        cariHesapBorcAlacakEkle.Donem = Convert.ToInt32(TurkeyDateTime.Now.Year + 1);
                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                        cariHesapBorcAlacakEkle.Borc1 = 0;
                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                        cariHesapBorcAlacakEkle.Borc2 = 0;
                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                        cariHesapBorcAlacakEkle.Borc3 = 0;
                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                        cariHesapBorcAlacakEkle.Borc4 = 0;
                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                        cariHesapBorcAlacakEkle.Borc5 = 0;
                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                        cariHesapBorcAlacakEkle.Borc6 = 0;
                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                        cariHesapBorcAlacakEkle.Borc7 = 0;
                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                        cariHesapBorcAlacakEkle.Borc8 = 0;
                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                        cariHesapBorcAlacakEkle.Borc9 = 0;
                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                        cariHesapBorcAlacakEkle.Borc10 = 0;
                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                        cariHesapBorcAlacakEkle.Borc11 = 0;
                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                        cariHesapBorcAlacakEkle.Borc12 = 0;
                        CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                    }
                }
                cariHesapUnvani = hesap.Unvan;
                cariHesabınKodu = hesap.CariHesapKodu;
            }
            catch (Exception)
            {
            }

            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }

        public Tuple<string, string> UpdateCariHesap(CariHesaplari carihesap)
        {

            string cariHesapUnvani = "";
            string cariHesabınKodu = "";
            try
            {
                _MuhasebeContext.CariHesaplariRepository.Update(carihesap);
                _MuhasebeContext.Commit();
                cariHesapUnvani = carihesap.Unvan;
                cariHesabınKodu = carihesap.CariHesapKodu;
            }
            catch (Exception)
            {
            }

            return new Tuple<string, string>(cariHesapUnvani, cariHesabınKodu);
        }
        public CariHesapEkstreListModel CariHesapEkstre(string hesapkodu, int tvmkodu, string musteriGrupKodu, byte aramaTipi, string donemAraligi, DateTime baslangic, DateTime bitis, int mizanTipi, int pdfTipi)
        {
            DateTime defaultTarih = new DateTime();
            bool GunAySorgu = false;
            if (baslangic != defaultTarih && bitis != defaultTarih)
            {
                GunAySorgu = true;
            }
            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            //int donem = bitis.Year;
            CariHesapEkstreListModel model = new CariHesapEkstreListModel();
            var evrakTipleri = _MuhasebeContext.CariEvrakTipleriRepository.All();
            List<CariHareketleri> CariHareketler = new List<CariHareketleri>();
            List<PoliceTahsilat> polTahsilatlar = new List<PoliceTahsilat>();
            List<PoliceGenel> polGenelList = new List<PoliceGenel>();
            string tcVkn = String.Empty;
            string carHesapKod = String.Empty;
            if (!String.IsNullOrEmpty(hesapkodu))
            {
                var parts = hesapkodu.Split('.');
                if (parts != null && parts.Count() == 3)
                {
                    tcVkn = parts[2];
                    carHesapKod = parts[0];
                }
            }

            if (aramaTipi == 1) //Müşteri Grup Kodu
            {
                if (!String.IsNullOrEmpty(musteriGrupKodu))
                {
                    musteriGrupKodu = musteriGrupKodu.ToLower().Replace('ı', 'i').Trim();
                    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All()
                        .Where(w => w.TVMKodu == tvmkodu &&
                        ((GunAySorgu == true && w.CariHareketTarihi.Value >= baslangic && w.CariHareketTarihi.Value <= bitis) ||
                        (GunAySorgu == false && (w.CariHareketTarihi.Value.Year == donemBaslangic || w.CariHareketTarihi.Value.Year == donemBitis))) &&
                        w.MusteriGrupKodu.ToLower().Trim()
                        .StartsWith(musteriGrupKodu)).ToList();

                    var musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMMusteriKodu.ToLower().Trim().StartsWith(musteriGrupKodu) && w.TVMKodu == tvmkodu).Select(s => s.KimlikNo).ToList();
                    if (musteriler.Count > 0)
                    {
                        polGenelList = _PoliceContext.PoliceGenelRepository.All()
                            .Where(w => w.TVMKodu == tvmkodu && w.BrutPrim != 0 &&
                            ((GunAySorgu == true && w.TanzimTarihi.Value >= baslangic && w.TanzimTarihi.Value <= bitis) ||
                            (GunAySorgu == false && (w.TanzimTarihi.Value.Year == donemBaslangic || w.TanzimTarihi.Value.Year == donemBitis))) &&
                             ((w.PoliceSigortaEttiren.KimlikNo != null && musteriler.Contains(w.PoliceSigortaEttiren.KimlikNo.Trim())) || (w.PoliceSigortaEttiren.VergiKimlikNo != null &&
                       musteriler.Contains(w.PoliceSigortaEttiren.VergiKimlikNo.Trim())))).ToList();

                        polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All()
                            .Where(s => musteriler.Contains(s.KimlikNo.Trim()) && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu &&
                            ((GunAySorgu == true && s.TaksitVadeTarihi >= baslangic && s.TaksitVadeTarihi <= bitis) ||
                            (GunAySorgu == false && (s.TaksitVadeTarihi.Year == donemBaslangic || s.TaksitVadeTarihi.Year == donemBitis)))).ToList();
                        //for (int i = 0; i < polGenelList.Count(); i++)
                        //{
                        //    for (int j = 0; j < polGenelList[i].PoliceTahsilats.Count(); j++)
                        //    {
                        //        var polT = polGenelList[i].PoliceTahsilats;
                        //        polTahsilatlar = polT.ToList();
                        //    }

                        //}



                        //if (_AktifKullanici.TVMKodu == NeosinerjiTVM.CrasTVMKodu)
                        //{
                        //}
                        //else
                        //{
                        //    polGenelList = _PoliceContext.PoliceGenelRepository.All()
                        //        .Where(w => w.TVMKodu == tvmkodu && w.BrutPrim != 0 && 
                        //        (w.TanzimTarihi != null && w.TanzimTarihi.Value <= bitis && baslangic <= w.TanzimTarihi.Value) && 
                        //        musteriler.Contains(w.PoliceSigortaEttiren.KimlikNo.Trim())).ToList();

                        //    polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All().Where(s => musteriler.Contains(s.KimlikNo.Trim()) && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi <= bitis && baslangic <= s.TaksitVadeTarihi).ToList();

                        //}
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(hesapkodu))
                {
                    #region Crass Sigortaya Özel 
                    //if (_AktifKullanici.TVMKodu == NeosinerjiTVM.CrasTVMKodu)
                    //{
                    //    int yil = 2018;
                    //    if (donem < yil)
                    //    {
                    //        CariHareketler = new List<CariHareketleri>();
                    //        polTahsilatlar = new List<PoliceTahsilat>();
                    //        polGenelList = new List<PoliceGenel>();
                    //        //CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.CariHesapKodu == hesapkodu && w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value.Month > ay && w.CariHareketTarihi.Value.Year > donem).ToList();
                    //        //polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo.Trim()== tcVkn && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi.Month > ay && s.TaksitVadeTarihi.Year > donem).ToList();

                    //        //polGenelList = _PoliceContext.PoliceGenelRepository.All().Where(w => w.TVMKodu == tvmkodu &&
                    //        //w.BrutPrim != 0 && (w.TanzimTarihi != null && w.TanzimTarihi.Value.Month > ay && w.TanzimTarihi.Value.Year > donem) &&
                    //        //(w.PoliceSigortaEttiren.KimlikNo.Trim() == tcVkn || w.PoliceSigortaEttiren.VergiKimlikNo.Trim() == tcVkn)).ToList();
                    //    }
                    //    else
                    //    {
                    //        CariHareketler = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.CariHesapKodu.Trim() == hesapkodu.Trim() && w.TVMKodu == tvmkodu && w.CariHareketTarihi.Value <= bitis && baslangic <= w.CariHareketTarihi.Value).ToList();
                    //        if (carHesapKod == "120")
                    //        {
                    //            polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo.Trim() == tcVkn && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu && (s.OdemTipi != CariOdemeTipKodu.AcenteKrediKarti && s.OdemTipi != CariOdemeTipKodu.AcenteBireyselKart) && s.TaksitVadeTarihi <= bitis && baslangic <= s.TaksitVadeTarihi).ToList();

                    //        }
                    //        else
                    //        {
                    //            polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.KimlikNo.Trim() == tcVkn && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu && s.TaksitVadeTarihi <= bitis && baslangic <= s.TaksitVadeTarihi).ToList();

                    //        }
                    //        polGenelList = _PoliceContext.PoliceGenelRepository.All().Where(w => w.TVMKodu == tvmkodu &&
                    //        w.BrutPrim != 0 && (w.TanzimTarihi != null && w.TanzimTarihi.Value <= bitis && baslangic <= w.TanzimTarihi.Value) &&
                    //         ((w.PoliceSigortaEttiren.KimlikNo != null && w.PoliceSigortaEttiren.KimlikNo.Trim() == tcVkn) || (w.PoliceSigortaEttiren.VergiKimlikNo != null && w.PoliceSigortaEttiren.VergiKimlikNo.Trim() == tcVkn))).ToList();
                    //    }
                    //}
                    //else
                    //{  //}
                    #endregion

                    CariHareketler = _MuhasebeContext.CariHareketleriRepository.All()
                        .Where(w => w.CariHesapKodu.Trim() == hesapkodu.Trim() && w.TVMKodu == tvmkodu &&
                        ((GunAySorgu == true && w.CariHareketTarihi.Value >= baslangic && w.CariHareketTarihi.Value <= bitis) ||
                        (GunAySorgu == false && (w.CariHareketTarihi.Value.Year == donemBaslangic || w.CariHareketTarihi.Value.Year == donemBitis)))).ToList();
                    if (carHesapKod == "120")
                    {
                        polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All()
                            .Where(s => s.KimlikNo.Trim() == tcVkn && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu &&
                            (s.OdemTipi != CariOdemeTipKodu.AcenteKrediKarti && s.OdemTipi != CariOdemeTipKodu.AcenteBireyselKart) &&
                            ((GunAySorgu == true && s.TaksitVadeTarihi >= baslangic && s.TaksitVadeTarihi <= bitis) ||
                            (GunAySorgu == false && (s.TaksitVadeTarihi.Year == donemBaslangic || s.TaksitVadeTarihi.Year == donemBitis)))).ToList();
                    }
                    else
                    {
                        polTahsilatlar = _PoliceContext.PoliceTahsilatRepository.All()
                            .Where(s => s.KimlikNo.Trim() == tcVkn && s.BrutPrim != 0 && s.PoliceGenel.TVMKodu == tvmkodu &&
                            ((GunAySorgu == true && s.TaksitVadeTarihi >= baslangic && s.TaksitVadeTarihi <= bitis) ||
                            (GunAySorgu == false && (s.TaksitVadeTarihi.Year == donemBaslangic || s.TaksitVadeTarihi.Year == donemBitis)))).ToList();

                    }

                    polGenelList = _PoliceContext.PoliceGenelRepository.All()
                       .Where(w => w.TVMKodu == tvmkodu && w.BrutPrim != 0 &&
                       ((GunAySorgu == true && w.TanzimTarihi.Value >= baslangic && w.TanzimTarihi.Value <= bitis) ||
                       (GunAySorgu == false && (w.TanzimTarihi.Value.Year == donemBaslangic || w.TanzimTarihi.Value.Year == donemBitis))) &&
                       ((w.PoliceSigortaEttiren.KimlikNo != null && w.PoliceSigortaEttiren.KimlikNo.Trim() == tcVkn) || (w.PoliceSigortaEttiren.VergiKimlikNo != null &&
                       w.PoliceSigortaEttiren.VergiKimlikNo.Trim() == tcVkn))).ToList();

                }
            }
            List<CariHesapEkstreModel> list = new List<CariHesapEkstreModel>();
            CariHesapEkstreModel listItem = new CariHesapEkstreModel();
            if (CariHareketler != null && CariHareketler.Count > 0)
            {
                foreach (var item in CariHareketler)
                {
                    listItem = new CariHesapEkstreModel();
                    listItem.Aciklama = item.Aciklama;
                    listItem.Adi = "";
                    listItem.Soyad = "";

                    if (item.BorcAlacakTipi == "A")
                    {
                        listItem.Alacak = item.Tutar;
                        listItem.Borc = 0;
                    }
                    else if (item.BorcAlacakTipi == "B")
                    {
                        listItem.Borc = item.Tutar;
                        listItem.Alacak = 0;
                    }
                    listItem.Bakiye = listItem.Borc - listItem.Alacak;
                    listItem.BorcTipi = item.BorcAlacakTipi;
                    listItem.EvrakNo = item.EvrakNo;
                    var evrakDetay = evrakTipleri.Where(w => w.Kodu == item.EvrakTipi).FirstOrDefault();
                    listItem.EvrakTipi = evrakDetay != null ? evrakDetay.Aciklama : "";
                    listItem.MusteriGrupKodu = item.MusteriGrupKodu;
                    listItem.OdemeTarihi = item.OdemeTarihi;

                    listItem.VadeTarihi = item.CariHareketTarihi.HasValue ? item.CariHareketTarihi.Value : TurkeyDateTime.Now;

                    if (!listItem.OdemeTarihi.HasValue)
                    {
                        listItem.OdemeTarihi = listItem.VadeTarihi;
                    }

                    listItem.ParaBirimi = item.DovizTipi;
                    list.Add(listItem);
                }
            }


            bool isYurtdisiBroker = false;
            if (polGenelList.Count > 0)
            {
                isYurtdisiBroker = _TVMService.getAnaAcenteTvmDetay(polGenelList.First().TVMKodu.Value).Tipi == 1;
            }

            if (polGenelList != null && polGenelList.Count > 0 && !isYurtdisiBroker)
            {
                var GroupList = polGenelList.GroupBy(ac => new
                {
                    ac.PoliceId

                })
                                           .Select(ac => new
                                           {
                                               PoliceId = ac.Key.PoliceId,
                                               polList = ac.ToList()
                                           }).OrderBy(s => s.PoliceId).ToList();
                for (int i = 0; i < GroupList.Count(); i++)
                {
                    var item = GroupList[i].polList.FirstOrDefault();
                    //var odemetipigroup = item.PoliceTahsilats.FirstOrDefault().OdemTipi;

                    string musteriGrupAdi = "";
                    var musteriDetay = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.KimlikNo == item.PoliceSigortaEttiren.KimlikNo && w.TVMKodu == tvmkodu && w.TVMMusteriKodu != null).Select(s => s.TVMMusteriKodu).FirstOrDefault();
                    if (!String.IsNullOrEmpty(musteriDetay))
                    {
                        musteriGrupAdi = musteriDetay;
                    }
                    listItem = new CariHesapEkstreModel();
                    if (item.BrutPrim < 0)
                    {
                        listItem.Alacak = item.BrutPrim * -1;
                        listItem.Borc = 0;
                    }
                    else
                    {
                        listItem.Borc = item.BrutPrim;
                        listItem.Alacak = 0;
                    }

                    listItem.Bakiye = listItem.Borc - listItem.Alacak;
                    listItem.VadeTarihi = item.TanzimTarihi.Value;
                    listItem.OdemeTarihi = item.TanzimTarihi.Value;
                    listItem.ParaBirimi = item.ParaBirimi;
                    listItem.MusteriGrupKodu = musteriGrupAdi;
                    listItem.PoliceId = item.PoliceId;
                    listItem.EvrakNo = item.PoliceNumarasi + " - " + item.YenilemeNo + " - " + item.EkNo;


                    if (item.BransKodu == 1 || item.BransKodu == 2)
                    {
                        if (item.PoliceArac == null)

                            listItem.Aciklama =
                                                item.BransAdi + "-" + item.SigortaSirketleri.SirketAdi + "- " + item.PoliceSigortali.AdiUnvan + "- " +
                                                item.PoliceSigortali.SoyadiUnvan;
                        else

                            listItem.Aciklama =
                                            item.BransAdi + "-" + item.SigortaSirketleri.SirketAdi + "- " + item.PoliceSigortali.AdiUnvan + "- " +
                                            item.PoliceSigortali.SoyadiUnvan + " -" + item.PoliceArac.PlakaKodu + " " + item.PoliceArac.PlakaNo;
                    }
                    else
                    {

                        listItem.Aciklama =
                            item.BransAdi + " - " + item.SigortaSirketleri.SirketAdi + " - " + item.PoliceSigortali.AdiUnvan + "  " +
                            item.PoliceSigortali.SoyadiUnvan + " - " + item.PoliceSigortali.IlAdi + " / " + item.PoliceSigortali.IlceAdi;

                    }

                    list.Add(listItem);

                    var odenenTahsilat = item.PoliceTahsilats.Where(w => w.OdenenTutar != 0 &&
                        ((carHesapKod == "120" && (w.OdemTipi != CariOdemeTipKodu.AcenteKrediKarti && w.OdemTipi != CariOdemeTipKodu.AcenteBireyselKart)) || carHesapKod != "120")).ToList();
                    decimal toplamOdenen = 0;
                    if (odenenTahsilat != null)
                    {
                        toplamOdenen = odenenTahsilat.Select(c => c.OdenenTutar).Sum();
                        DateTime sonVadeTarihi;
                        DateTime? sonOdemeTarihi;
                        byte sonOdemeTipi = 0;
                        var taksitDetay = odenenTahsilat.OrderByDescending(w => w.TaksitVadeTarihi).FirstOrDefault();


                        if (taksitDetay != null)
                        {
                            sonVadeTarihi = taksitDetay.TaksitVadeTarihi;
                            sonOdemeTarihi = taksitDetay.OdemeBelgeTarihi;
                            sonOdemeTipi = (byte)taksitDetay.OdemTipi;
                            if (!String.IsNullOrEmpty(musteriDetay))
                            {
                                musteriGrupAdi = musteriDetay;
                            }
                            listItem = new CariHesapEkstreModel();
                            if (toplamOdenen < 0)
                            {
                                listItem.Borc = toplamOdenen * -1;
                                listItem.Alacak = 0;
                            }
                            else
                            {
                                listItem.Borc = 0;
                                listItem.Alacak = toplamOdenen;
                            }
                            listItem.Bakiye = listItem.Borc - listItem.Alacak;
                            listItem.EvrakNo = taksitDetay.Dekont_EvrakNo;
                            listItem.VadeTarihi = sonVadeTarihi;
                            listItem.MusteriGrupKodu = musteriGrupAdi;
                            if (sonOdemeTarihi.HasValue)
                            {
                                listItem.OdemeTarihi = sonOdemeTarihi.Value;
                            }
                            else
                            {
                                listItem.OdemeTarihi = listItem.VadeTarihi;
                            }
                            listItem.OdemeTipi = OdemeTipleri.OdemeTipi(sonOdemeTipi);

                            listItem.ParaBirimi = item.ParaBirimi;
                            if (item.BransKodu == 1 || item.BransKodu == 2)
                            {
                                if (item.PoliceArac == null)

                                    listItem.Aciklama = item.PoliceNumarasi + "/" + item.YenilemeNo + "/" + item.EkNo + " - " +
                                                    item.BransAdi + " - Ödenen Tks. Ad.: " + odenenTahsilat.Count + " - " + item.SigortaSirketleri.SirketAdi + "- " +
                                                    item.PoliceSigortali.AdiUnvan + "- " + item.PoliceSigortali.SoyadiUnvan;
                                else

                                    listItem.Aciklama = item.PoliceNumarasi + "/" + item.YenilemeNo + "/" + item.EkNo + " - " +
                                                        item.BransAdi + " - Ödenen Tks. Ad.: " + odenenTahsilat.Count + " - " + item.SigortaSirketleri.SirketAdi + "- " +
                                                        item.PoliceSigortali.AdiUnvan + "- " + item.PoliceSigortali.SoyadiUnvan + " -" +
                                                        item.PoliceArac.PlakaKodu + " " + item.PoliceArac.PlakaNo;

                            }
                            else
                            {
                                listItem.Aciklama = item.PoliceNumarasi + "/" + item.YenilemeNo + "/" + item.EkNo + " - " +
                                                item.BransAdi + " - Ödenen Tks. Ad.: " + odenenTahsilat.Count + " - " + item.SigortaSirketleri.SirketAdi
                                                + " - " + item.PoliceSigortali.AdiUnvan + "  " + item.PoliceSigortali.SoyadiUnvan + " - " +
                                                    item.PoliceSigortali.IlAdi + " / " + item.PoliceSigortali.IlceAdi;
                            }

                            listItem.Adi = "";
                            listItem.Soyad = "";
                            listItem.PoliceId = item.PoliceId;
                            list.Add(listItem);

                        }
                    }
                }
            }

            List<PoliceTahsilat> farkliOdenenTaksitler = new List<PoliceTahsilat>();
            if (polTahsilatlar != null && polTahsilatlar.Count > 0)
            {
                var GroupList = polTahsilatlar.GroupBy(ac => new
                {
                    ac.PoliceId
                })
                                            .Select(ac => new
                                            {
                                                PoliceId = ac.Key.PoliceId,
                                                polTahsilatlar = ac.ToList()
                                            }).OrderBy(s => s.PoliceId).ToList();

                for (int i = 0; i < GroupList.Count(); i++)
                {
                    farkliOdenenTaksitler = new List<PoliceTahsilat>();
                    var odenenTksList = GroupList[i].polTahsilatlar.Where(w => w.OdenenTutar != 0 &&
                            ((GunAySorgu == true && w.TaksitVadeTarihi >= baslangic && w.TaksitVadeTarihi <= bitis) ||
                            (GunAySorgu == false && (w.TaksitVadeTarihi.Year == donemBaslangic || w.TaksitVadeTarihi.Year == donemBitis))) &&
                            ((carHesapKod == "120" && (w.OdemTipi != CariOdemeTipKodu.AcenteKrediKarti && w.OdemTipi != CariOdemeTipKodu.AcenteBireyselKart)) || carHesapKod != "120")).ToList();

                    if (odenenTksList != null)
                    {
                        decimal toplamFarkliOdenen = 0;
                        foreach (var item in odenenTksList)
                        {
                            var farkliOdenen = list.Where(w => w.PoliceId == item.PoliceId).FirstOrDefault();
                            if (farkliOdenen == null)
                            {
                                toplamFarkliOdenen += item.OdenenTutar;
                                farkliOdenenTaksitler.Add(item);
                            }
                        }
                        if (farkliOdenenTaksitler != null && farkliOdenenTaksitler.Count() > 0)
                        {
                            DateTime sonVadeTarihi;
                            DateTime? sonOdemeTarihi;
                            byte sonOdemeTipi = 0;
                            var taksitDetay = farkliOdenenTaksitler.OrderByDescending(w => w.TaksitVadeTarihi).FirstOrDefault();

                            if (taksitDetay != null)
                            {
                                sonVadeTarihi = taksitDetay.TaksitVadeTarihi;
                                sonOdemeTarihi = taksitDetay.OdemeBelgeTarihi;
                                sonOdemeTipi = (byte)taksitDetay.OdemTipi;
                                string musteriGrupAdi = "";
                                var musteriDetay = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.KimlikNo == taksitDetay.KimlikNo && w.TVMKodu == tvmkodu && w.TVMMusteriKodu != null).Select(s => s.TVMMusteriKodu).FirstOrDefault();
                                if (!String.IsNullOrEmpty(musteriDetay))
                                {
                                    musteriGrupAdi = musteriDetay;
                                }
                                listItem = new CariHesapEkstreModel();
                                if (toplamFarkliOdenen < 0)
                                {
                                    listItem.Borc = toplamFarkliOdenen * -1;
                                    listItem.Alacak = 0;
                                }
                                else
                                {
                                    listItem.Borc = 0;
                                    listItem.Alacak = toplamFarkliOdenen;
                                }
                                listItem.Bakiye = listItem.Borc - listItem.Alacak;
                                listItem.EvrakNo = taksitDetay.OdemeBelgeNo;
                                listItem.VadeTarihi = sonVadeTarihi;
                                listItem.MusteriGrupKodu = musteriGrupAdi;
                                if (sonOdemeTarihi.HasValue)
                                {
                                    listItem.OdemeTarihi = sonOdemeTarihi.Value;
                                }
                                else
                                {
                                    listItem.OdemeTarihi = listItem.VadeTarihi;
                                }
                                listItem.OdemeTipi = OdemeTipleri.OdemeTipi(sonOdemeTipi);

                                listItem.ParaBirimi = taksitDetay.PoliceGenel.ParaBirimi;
                                if (taksitDetay.PoliceGenel.BransKodu == 1 || taksitDetay.PoliceGenel.BransKodu == 2)
                                {

                                    listItem.Aciklama = taksitDetay.PoliceGenel.PoliceNumarasi + "/" + taksitDetay.PoliceGenel.YenilemeNo + "/" + taksitDetay.PoliceGenel.EkNo + " - " +
                                                        taksitDetay.PoliceGenel.BransAdi + " - Ödenen Tks. Ad.: " + farkliOdenenTaksitler.Count + " - " + taksitDetay.PoliceGenel.SigortaSirketleri.SirketAdi + "- " +
                                                        taksitDetay.PoliceGenel.PoliceSigortali.AdiUnvan + "- " + taksitDetay.PoliceGenel.PoliceSigortali.SoyadiUnvan + " -" +
                                                        taksitDetay.PoliceGenel.PoliceArac.PlakaKodu + " " + taksitDetay.PoliceGenel.PoliceArac.PlakaNo;

                                }
                                else
                                {
                                    listItem.Aciklama = taksitDetay.PoliceGenel.PoliceNumarasi + "/" + taksitDetay.PoliceGenel.YenilemeNo + "/" + taksitDetay.PoliceGenel.EkNo + " - " +
                                                    taksitDetay.PoliceGenel.BransAdi + " - Ödenen Tks. Ad.: " + farkliOdenenTaksitler.Count + " - " + taksitDetay.PoliceGenel.SigortaSirketleri.SirketAdi
                                                    + " - " + taksitDetay.PoliceGenel.PoliceSigortali.AdiUnvan + "  " + taksitDetay.PoliceGenel.PoliceSigortali.SoyadiUnvan + " - " +
                                                        taksitDetay.PoliceGenel.PoliceSigortali.IlAdi + " / " + taksitDetay.PoliceGenel.PoliceSigortali.IlceAdi;
                                }

                                listItem.Adi = "";
                                listItem.Soyad = "";
                                if (!isYurtdisiBroker)
                                {
                                    list.Add(listItem);
                                }

                            }
                        }
                    }
                }
            }

            if (list != null && list.Count > 0)
            {
                list = list.OrderByDescending(w => w.OdemeTarihi).ToList();
            }
            model.ekstreList = list;
            #region Cari Hesap Özet
            if (mizanTipi == 1) // 0 = Detaylı, 1 = Özet
            {
                var nakitItems = new List<CariHesapEkstreModel>();
                foreach (var ekstre in list)
                {
                    nakitItems.Add(ekstre);
                }

                nakitItems.RemoveAll(x => x.OdemeTipi != "Nakit");
                list.RemoveAll(x => x.OdemeTipi == "Nakit");
                var groupedNakitItems = nakitItems.GroupBy(x => x.OdemeTarihi);

                var mergedNakitsModel = new List<CariHesapEkstreModel>();

                foreach (var group in groupedNakitItems)
                {
                    var mergedEkstre = new CariHesapEkstreModel();
                    mergedEkstre.Borc = 0;
                    mergedEkstre.Alacak = 0;
                    mergedEkstre.Bakiye = 0;
                    mergedEkstre.Aciklama = "Toplu Poliçe Ödeme";
                    mergedEkstre.OdemeTarihi = group.Key;
                    mergedEkstre.OdemeTipi = group.First().OdemeTipi;
                    mergedEkstre.ParaBirimi = group.First().ParaBirimi;
                    mergedEkstre.VadeTarihi = group.First().VadeTarihi;
                    mergedEkstre.EvrakNo = group.First().EvrakNo;
                    mergedEkstre.MusteriGrupKodu = group.First().MusteriGrupKodu;
                    mergedEkstre.Adi = group.First().Adi;
                    mergedEkstre.Soyad = group.First().Soyad;

                    foreach (var ekstre in group)
                    {
                        mergedEkstre.Borc += ekstre.Borc;
                        mergedEkstre.Alacak += ekstre.Alacak;
                        mergedEkstre.Bakiye += ekstre.Bakiye;
                    }
                    mergedNakitsModel.Add(mergedEkstre);
                }

                foreach (var mergedEkstre in mergedNakitsModel)
                {
                    list.Add(mergedEkstre);
                }
            }
            #endregion


            model.cariHesap = this.GetBorcAlacak(donemBaslangic, hesapkodu);

            if (list.Count > 0)
            {
                model.PDFURL = this.CariHesapCreatePDF(model, hesapkodu, musteriGrupKodu, pdfTipi);
            }
            return model;
        }

        public string CariHesapCreatePDF(CariHesapEkstreListModel model, string hesapKodu, string musteriGrupKodu, int pdfTipi)
        {
            PDFHelper pdf = null;
            if (pdfTipi == 0)
            {
                try
                {
                    string rootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                    string template = PdfTemplates.GetTemplate(rootPath + "Content/templates/", PdfTemplates.CARI_HESAP);

                    pdf = new PDFHelper("NeoOnline", "CARİ HESAP EKSTRESİ", "CARİ HESAP EKSTRESİ", 8, rootPath + "Content/fonts/");

                    // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                    pdf.SetPageEventHelper(new PDFCustomEventHelper());
                    pdf.Rotate();
                    PDFParser parser = new PDFParser(template, pdf);

                    string cariHesapAdi = "";
                    string donem = "";
                    int k = model.ekstreList.Count - 1;

                    if (model.ekstreList.Count > 0)
                    {
                        donem = model.ekstreList[k].VadeTarihi.Year.ToString();
                        parser.SetVariable("$Donem$", "DÖNEM -" + donem);
                        parser.SetVariable("$ListeAdi$", "CARİ HESAP EKSTRESİ");
                        parser.SetVariable("$TVMUnvani$", _AktifKullanici.TVMUnvani);

                        if (!String.IsNullOrEmpty(hesapKodu))
                        {
                            var hesapBilgi = this.GetCariHesapAdi(hesapKodu);
                            cariHesapAdi = hesapBilgi.Item1 + " " + hesapBilgi.Item2;
                            parser.SetVariable("$CariHesapAdi$", cariHesapAdi);
                        }

                        if (!String.IsNullOrEmpty(musteriGrupKodu))
                        {
                            cariHesapAdi = musteriGrupKodu;
                            parser.SetVariable("$CariHesapAdi$", musteriGrupKodu);
                        }
                    }
                    string fiyatSatirTemplate = parser.GetTemplate("EkstreSatiri");
                    decimal yuruyenEkstreBakiye = 0;

                    for (int i = model.ekstreList.Count - 1; i >= 0; i--)
                    {
                        var item = model.ekstreList[i];
                        string listSatir = String.Empty;
                        listSatir = fiyatSatirTemplate.Replace("$VadeTarihi$", item.VadeTarihi.ToString("dd.MM.yyyy"));
                        listSatir = listSatir.Replace("$EvrakTarihi$", item.OdemeTarihi.HasValue ? item.OdemeTarihi.Value.ToString("dd.MM.yyyy") : "");
                        listSatir = listSatir.Replace("$EvrakOdemeTipi$", item.EvrakTipi + " " + item.OdemeTipi);
                        listSatir = listSatir.Replace("$EvrakNo$", item.EvrakNo);
                        listSatir = listSatir.Replace("$Aciklama$", item.Aciklama);
                        listSatir = listSatir.Replace("$ParaBirimi$", item.ParaBirimi);
                        listSatir = listSatir.Replace("$Borc$", item.Borc.HasValue ? item.Borc.Value.ToString("N2") : "");
                        listSatir = listSatir.Replace("$Alacak$", item.Alacak.HasValue ? item.Alacak.Value.ToString("N2") : "");
                        yuruyenEkstreBakiye += item.Bakiye.HasValue ? item.Bakiye.Value : 0;

                        if (yuruyenEkstreBakiye < 0)
                        {
                            yuruyenEkstreBakiye = yuruyenEkstreBakiye * -1;
                            listSatir = listSatir.Replace("$Bakiye$", yuruyenEkstreBakiye.ToString("N2"));
                            yuruyenEkstreBakiye = yuruyenEkstreBakiye * -1;
                        }
                        else
                        {
                            listSatir = listSatir.Replace("$Bakiye$", yuruyenEkstreBakiye.ToString("N2"));
                        }

                        if (yuruyenEkstreBakiye < 0)
                        {
                            listSatir = listSatir.Replace("$BA$", "A");
                        }
                        else
                        {
                            listSatir = listSatir.Replace("$BA$", "B");
                        }


                        parser.AppendToPlaceHolder("EkstreSatirlari", listSatir);
                    }
                    if (model.ekstreList.Count > 0)
                    {

                        var ekstreBorcToplam = model.ekstreList.Select(s => s.Borc).Sum();
                        var ekstreAlacakToplam = model.ekstreList.Select(s => s.Alacak).Sum();
                        var ekstreBakiyeToplam = ekstreBorcToplam - ekstreAlacakToplam;
                        ekstreBakiyeToplam = ekstreBakiyeToplam * -1;

                        if (ekstreBorcToplam < 0)
                        {
                            ekstreBorcToplam = ekstreBorcToplam * -1;
                            parser.SetVariable("$CEBorcToplam$", ekstreBorcToplam.Value.ToString("N2"));
                            ekstreBorcToplam = ekstreBorcToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEBorcToplam$", ekstreBorcToplam.Value.ToString("N2"));
                        }
                        if (ekstreAlacakToplam < 0)
                        {
                            ekstreAlacakToplam = ekstreAlacakToplam * -1;
                            parser.SetVariable("$CEAlacakToplam$", ekstreAlacakToplam.Value.ToString("N2"));
                            ekstreAlacakToplam = ekstreAlacakToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEAlacakToplam$", ekstreAlacakToplam.Value.ToString("N2"));
                        }
                        if (ekstreBakiyeToplam < 0)
                        {
                            ekstreBakiyeToplam = ekstreBakiyeToplam * -1;
                            parser.SetVariable("$CEBakiyeToplam$", ekstreBakiyeToplam.Value.ToString("N2"));
                            ekstreBakiyeToplam = ekstreBakiyeToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEBakiyeToplam$", ekstreBakiyeToplam.Value.ToString("N2"));
                        }
                        if (ekstreBakiyeToplam < 0)
                        {
                            parser.SetVariable("$CEBAGenel$", "A");
                        }
                        else
                        {
                            parser.SetVariable("$CEBAGenel$", "B");
                        }

                    }

                    if (model.ekstreList.Count > 0)
                    {
                        #region Cari Hesap Özeti (Aylık)
                        var chba = model.cariHesap;
                        decimal yuruyenBakiye = 0;
                        decimal borcToplam = 0;

                        if (chba != null)
                        {
                            parser.SetVariable("$Donem2$", "DÖNEM -" + donem);
                            parser.SetVariable("$ListeAdi2$", "AYLIK CARİ HESAP ÖZETİ");
                            parser.SetVariable("$CariHesapAdi2$", cariHesapAdi);

                            #region Tutarlar Boş ise sıfır ekleniyor

                            chba.Borc1 = chba.Borc1.HasValue ? chba.Borc1.Value : 0;
                            chba.Alacak1 = chba.Alacak1.HasValue ? chba.Alacak1.Value : 0;

                            chba.Borc2 = chba.Borc2.HasValue ? chba.Borc2.Value : 0;
                            chba.Alacak2 = chba.Alacak2.HasValue ? chba.Alacak2.Value : 0;

                            chba.Borc3 = chba.Borc3.HasValue ? chba.Borc3.Value : 0;
                            chba.Alacak3 = chba.Alacak3.HasValue ? chba.Alacak3.Value : 0;

                            chba.Borc4 = chba.Borc4.HasValue ? chba.Borc4.Value : 0;
                            chba.Alacak4 = chba.Alacak4.HasValue ? chba.Alacak4.Value : 0;

                            chba.Borc5 = chba.Borc5.HasValue ? chba.Borc5.Value : 0;
                            chba.Alacak5 = chba.Alacak5.HasValue ? chba.Alacak5.Value : 0;

                            chba.Borc6 = chba.Borc6.HasValue ? chba.Borc6.Value : 0;
                            chba.Alacak6 = chba.Alacak6.HasValue ? chba.Alacak6.Value : 0;

                            chba.Borc7 = chba.Borc7.HasValue ? chba.Borc7.Value : 0;
                            chba.Alacak7 = chba.Alacak7.HasValue ? chba.Alacak7.Value : 0;

                            chba.Borc8 = chba.Borc8.HasValue ? chba.Borc8.Value : 0;
                            chba.Alacak8 = chba.Alacak8.HasValue ? chba.Alacak8.Value : 0;

                            chba.Borc9 = chba.Borc9.HasValue ? chba.Borc9.Value : 0;
                            chba.Alacak9 = chba.Alacak9.HasValue ? chba.Alacak9.Value : 0;

                            chba.Borc10 = chba.Borc10.HasValue ? chba.Borc10.Value : 0;
                            chba.Alacak10 = chba.Alacak10.HasValue ? chba.Alacak10.Value : 0;

                            chba.Borc11 = chba.Borc11.HasValue ? chba.Borc11.Value : 0;
                            chba.Alacak11 = chba.Alacak11.HasValue ? chba.Alacak11.Value : 0;

                            chba.Borc12 = chba.Borc12.HasValue ? chba.Borc12.Value : 0;
                            chba.Alacak12 = chba.Alacak12.HasValue ? chba.Alacak12.Value : 0;


                            #endregion

                            #region OCAK 
                            if (chba.Borc1 < 0)
                            {
                                chba.Borc1 = chba.Borc1 * -1;
                                parser.SetVariable("$OcakTutar$", chba.Borc1.HasValue ? chba.Borc1.Value.ToString("N2") : "");
                                chba.Borc1 = chba.Borc1 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$OcakTutar$", chba.Borc1.HasValue ? chba.Borc1.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$OcakOdenen$", chba.Alacak1.HasValue ? chba.Alacak1.Value.ToString("N2") : "");

                            var Borc = chba.Borc1.Value - chba.Alacak1.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc1.Value - chba.Alacak1.Value) < 0)
                            {
                                parser.SetVariable("$OcakBorc$", ((chba.Borc1.Value - chba.Alacak1.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$OcakBorc$", (chba.Borc1.Value - chba.Alacak1.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$OcakBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$OcakBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region ŞUBAT 
                            if (chba.Borc2 < 0)
                            {
                                chba.Borc2 = chba.Borc2 * -1;
                                parser.SetVariable("$SubatTutar$", chba.Borc2.HasValue ? chba.Borc2.Value.ToString("N2") : "");
                                chba.Borc2 = chba.Borc2 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$SubatTutar$", chba.Borc2.HasValue ? chba.Borc2.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$SubatOdenen$", chba.Alacak2.HasValue ? chba.Alacak2.Value.ToString("N2") : "");

                            Borc = chba.Borc2.Value - chba.Alacak2.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc2.Value - chba.Alacak2.Value) < 0)
                            {
                                parser.SetVariable("$SubatBorc$", ((chba.Borc2.Value - chba.Alacak2.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$SubatBorc$", (chba.Borc2.Value - chba.Alacak2.Value).ToString("N2"));
                            }
                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$SubatBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$SubatBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region MART 
                            if (chba.Borc3 < 0)
                            {
                                chba.Borc3 = chba.Borc3 * -1;
                                parser.SetVariable("$MartTutar$", chba.Borc3.HasValue ? chba.Borc3.Value.ToString("N2") : "");
                                chba.Borc3 = chba.Borc3 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$MartTutar$", chba.Borc3.HasValue ? chba.Borc3.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$MartOdenen$", chba.Alacak3.HasValue ? chba.Alacak3.Value.ToString("N2") : "");

                            Borc = chba.Borc3.Value - chba.Alacak3.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((Borc = chba.Borc3.Value - chba.Alacak3.Value) < 0)
                            {
                                parser.SetVariable("$MartBorc$", ((chba.Borc3.Value - chba.Alacak3.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$MartBorc$", (chba.Borc3.Value - chba.Alacak3.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$MartBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$MartBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region NİSAN 
                            if (chba.Borc4 < 0)
                            {
                                chba.Borc4 = chba.Borc4 * -1;
                                parser.SetVariable("$NisanTutar$", chba.Borc4.HasValue ? chba.Borc4.Value.ToString("N2") : "");
                                chba.Borc4 = chba.Borc4 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$NisanTutar$", chba.Borc4.HasValue ? chba.Borc4.Value.ToString("N2") : "");

                            }
                            parser.SetVariable("$NisanOdenen$", chba.Alacak4.HasValue ? chba.Alacak4.Value.ToString("N2") : "");

                            Borc = chba.Borc4.Value - chba.Alacak4.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc4.Value - chba.Alacak4.Value) < 0)
                            {
                                parser.SetVariable("$NisanBorc$", ((chba.Borc4.Value - chba.Alacak4.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$NisanBorc$", (chba.Borc4.Value - chba.Alacak4.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$NisanBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$NisanBakiye$", yuruyenBakiye.ToString("N2"));
                            }
                            #endregion

                            #region MAYIS 
                            if (chba.Borc5 < 0)
                            {
                                chba.Borc5 = chba.Borc5 * -1;
                                parser.SetVariable("$MayisTutar$", chba.Borc5.HasValue ? chba.Borc5.Value.ToString("N2") : "");
                                chba.Borc5 = chba.Borc5 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$MayisTutar$", chba.Borc5.HasValue ? chba.Borc5.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$MayisOdenen$", chba.Alacak5.HasValue ? chba.Alacak5.Value.ToString("N2") : "");

                            Borc = chba.Borc5.Value - chba.Alacak5.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc5.Value - chba.Alacak5.Value) < 0)
                            {
                                parser.SetVariable("$MayisBorc$", ((chba.Borc5.Value - chba.Alacak5.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$MayisBorc$", (chba.Borc5.Value - chba.Alacak5.Value).ToString("N2"));
                            }
                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$MayisBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$MayisBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region HAZİRAN 
                            if (chba.Borc6 < 0)
                            {
                                chba.Borc6 = chba.Borc6 * -1;
                                parser.SetVariable("$HaziranTutar$", chba.Borc6.HasValue ? chba.Borc6.Value.ToString("N2") : "");
                                chba.Borc6 = chba.Borc6 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$HaziranTutar$", chba.Borc6.HasValue ? chba.Borc6.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$HaziranOdenen$", chba.Alacak6.HasValue ? chba.Alacak6.Value.ToString("N2") : "");

                            Borc = chba.Borc6.Value - chba.Alacak6.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc6.Value - chba.Alacak6.Value) < 0)
                            {
                                parser.SetVariable("$HaziranBorc$", ((chba.Borc6.Value - chba.Alacak6.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$HaziranBorc$", (chba.Borc6.Value - chba.Alacak6.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$HaziranBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$HaziranBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region TEMMUZ 
                            if (chba.Borc7 < 0)
                            {
                                chba.Borc7 = chba.Borc7 * -1;
                                parser.SetVariable("$TemmuzTutar$", chba.Borc7.HasValue ? chba.Borc7.Value.ToString("N2") : "");
                                chba.Borc7 = chba.Borc7 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$TemmuzTutar$", chba.Borc7.HasValue ? chba.Borc7.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$TemmuzOdenen$", chba.Alacak7.HasValue ? chba.Alacak7.Value.ToString("N2") : "");

                            Borc = chba.Borc7.Value - chba.Alacak7.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc7.Value - chba.Alacak7.Value) < 0)
                            {
                                parser.SetVariable("$TemmuzBorc$", ((chba.Borc7.Value - chba.Alacak7.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$TemmuzBorc$", ((chba.Borc7.Value - chba.Alacak7.Value) * -1).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$TemmuzBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$TemmuzBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region AĞUSTOS 
                            if (chba.Borc8 < 0)
                            {
                                chba.Borc8 = chba.Borc8 * -1;
                                parser.SetVariable("$AgustosTutar$", chba.Borc8.HasValue ? chba.Borc8.Value.ToString("N2") : "");
                                chba.Borc8 = chba.Borc8 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$AgustosTutar$", chba.Borc8.HasValue ? chba.Borc8.Value.ToString("N2") : "");
                            }
                            parser.SetVariable("$AgustosOdenen$", chba.Alacak8.HasValue ? chba.Alacak8.Value.ToString("N2") : "");

                            Borc = chba.Borc8.Value - chba.Alacak8.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc8.Value - chba.Alacak8.Value) < 0)
                            {
                                parser.SetVariable("$AgustosBorc$", ((chba.Borc8.Value - chba.Alacak8.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$AgustosBorc$", (chba.Borc8.Value - chba.Alacak8.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$AgustosBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$AgustosBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region EYLÜL 
                            if (chba.Borc9 < 0)
                            {
                                chba.Borc9 = chba.Borc9 * -1;
                                parser.SetVariable("$EylulTutar$", chba.Borc9.HasValue ? chba.Borc9.Value.ToString("N2") : "");
                                chba.Borc9 = chba.Borc9 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$EylulTutar$", chba.Borc9.HasValue ? chba.Borc9.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$EylulOdenen$", chba.Alacak9.HasValue ? chba.Alacak9.Value.ToString("N2") : "");

                            Borc = chba.Borc9.Value - chba.Alacak9.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc9.Value - chba.Alacak9.Value) < 0)
                            {
                                parser.SetVariable("$EylulBorc$", ((chba.Borc9.Value - chba.Alacak9.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$EylulBorc$", (chba.Borc9.Value - chba.Alacak9.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$EylulBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$EylulBakiye$", yuruyenBakiye.ToString("N2"));
                            }
                            #endregion

                            #region EKİM 
                            if (chba.Borc10 < 0)
                            {
                                chba.Borc10 = chba.Borc10 * -1;
                                parser.SetVariable("$EkimTutar$", chba.Borc10.HasValue ? chba.Borc10.Value.ToString("N2") : "");
                                chba.Borc10 = chba.Borc10 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$EkimTutar$", chba.Borc10.HasValue ? chba.Borc10.Value.ToString("N2") : "");

                            }

                            parser.SetVariable("$EkimOdenen$", chba.Alacak10.HasValue ? chba.Alacak10.Value.ToString("N2") : "");

                            Borc = chba.Borc10.Value - chba.Alacak10.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc10.Value - chba.Alacak10.Value) < 0)
                            {
                                parser.SetVariable("$EkimBorc$", ((chba.Borc10.Value - chba.Alacak10.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$EkimBorc$", (chba.Borc10.Value - chba.Alacak10.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$EkimBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$EkimBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region KASIM 
                            if (chba.Borc11 < 0)
                            {
                                chba.Borc11 = chba.Borc11 * -1;
                                parser.SetVariable("$KasimTutar$", chba.Borc11.HasValue ? chba.Borc11.Value.ToString("N2") : "");
                                chba.Borc11 = chba.Borc11 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$KasimTutar$", chba.Borc11.HasValue ? chba.Borc11.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$KasimOdenen$", chba.Alacak11.HasValue ? chba.Alacak11.Value.ToString("N2") : "");

                            Borc = chba.Borc11.Value - chba.Alacak11.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc11.Value - chba.Alacak11.Value) < 0)
                            {
                                parser.SetVariable("$KasimBorc$", ((chba.Borc11.Value - chba.Alacak11.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$KasimBorc$", (chba.Borc11.Value - chba.Alacak11.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$KasimBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$KasimBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region ARALIK 
                            if (chba.Borc12 < 0)
                            {
                                chba.Borc12 = chba.Borc12 * -1;
                                parser.SetVariable("$AralikTutar$", chba.Borc12.HasValue ? chba.Borc12.Value.ToString("N2") : "");
                                chba.Borc12 = chba.Borc12 * -1;
                            }
                            else
                            {
                                parser.SetVariable("$AralikTutar$", chba.Borc12.HasValue ? chba.Borc12.Value.ToString("N2") : "");
                            }

                            parser.SetVariable("$AralikOdenen$", chba.Alacak12.HasValue ? chba.Alacak12.Value.ToString("N2") : "");

                            Borc = chba.Borc12.Value - chba.Alacak12.Value;
                            yuruyenBakiye += Borc;
                            borcToplam += Borc;
                            if ((chba.Borc12.Value - chba.Alacak12.Value) < 0)
                            {
                                parser.SetVariable("$AralikBorc$", ((chba.Borc12.Value - chba.Alacak12.Value) * -1).ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$AralikBorc$", (chba.Borc12.Value - chba.Alacak12.Value).ToString("N2"));
                            }

                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$AralikBakiye$", yuruyenBakiye.ToString("N2"));
                                yuruyenBakiye = yuruyenBakiye * -1;
                            }
                            else
                            {
                                parser.SetVariable("$AralikBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion

                            #region Toplam Borç

                            decimal toplamBorc = 0;
                            toplamBorc += chba.Borc1.Value;
                            toplamBorc += chba.Borc2.Value;
                            toplamBorc += chba.Borc3.Value;
                            toplamBorc += chba.Borc4.Value;
                            toplamBorc += chba.Borc5.Value;
                            toplamBorc += chba.Borc6.Value;
                            toplamBorc += chba.Borc7.Value;
                            toplamBorc += chba.Borc8.Value;
                            toplamBorc += chba.Borc9.Value;
                            toplamBorc += chba.Borc10.Value;
                            toplamBorc += chba.Borc11.Value;
                            toplamBorc += chba.Borc12.Value;

                            #endregion

                            #region Toplam Alacak

                            decimal toplamAlacak = 0;
                            toplamAlacak += chba.Alacak1.Value;
                            toplamAlacak += chba.Alacak2.Value;
                            toplamAlacak += chba.Alacak3.Value;
                            toplamAlacak += chba.Alacak4.Value;
                            toplamAlacak += chba.Alacak5.Value;
                            toplamAlacak += chba.Alacak6.Value;
                            toplamAlacak += chba.Alacak7.Value;
                            toplamAlacak += chba.Alacak8.Value;
                            toplamAlacak += chba.Alacak9.Value;
                            toplamAlacak += chba.Alacak10.Value;
                            toplamAlacak += chba.Alacak11.Value;
                            toplamAlacak += chba.Alacak12.Value;


                            #endregion

                            #region Genel Toplam
                            if (toplamBorc < 0)
                            {
                                toplamBorc = toplamBorc * -1;
                                parser.SetVariable("$OdenecekToplam$", toplamBorc.ToString("N2"));

                            }
                            else
                            {
                                parser.SetVariable("$OdenecekToplam$", toplamBorc.ToString("N2"));
                            }
                            if (toplamAlacak < 0)
                            {
                                toplamAlacak = toplamAlacak * -1;
                                parser.SetVariable("$OdenenToplam$", toplamAlacak.ToString("N2"));

                            }
                            else
                            {
                                parser.SetVariable("$OdenenToplam$", toplamAlacak.ToString("N2"));
                            }
                            if (borcToplam < 0)
                            {
                                borcToplam = borcToplam * -1;
                                parser.SetVariable("$BorcToplam$", borcToplam.ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$BorcToplam$", borcToplam.ToString("N2"));
                            }
                            if (yuruyenBakiye < 0)
                            {
                                yuruyenBakiye = yuruyenBakiye * -1;
                                parser.SetVariable("$ToplamBakiye$", yuruyenBakiye.ToString("N2"));
                            }
                            else
                            {
                                parser.SetVariable("$ToplamBakiye$", yuruyenBakiye.ToString("N2"));
                            }

                            #endregion
                            #endregion

                        }
                        else
                        {
                            parser.SetVariable("$Donem2$", "DÖNEM -" + donem);
                            parser.SetVariable("$ListeAdi2$", "AYLIK CARİ HESAP ÖZETİ");
                            parser.SetVariable("$CariHesapAdi2$", cariHesapAdi);

                            parser.SetVariable("$OcakTutar$", "0");
                            parser.SetVariable("$OcakOdenen$", "0");
                            parser.SetVariable("$OcakBorc$", "0");
                            parser.SetVariable("$OcakBakiye$", "0");

                            parser.SetVariable("$SubatTutar$", "0");
                            parser.SetVariable("$SubatOdenen$", "0");
                            parser.SetVariable("$SubatBorc$", "0");
                            parser.SetVariable("$SubatBakiye$", "0");

                            parser.SetVariable("$MartTutar$", "0");
                            parser.SetVariable("$MartOdenen$", "0");
                            parser.SetVariable("$MartBorc$", "0");
                            parser.SetVariable("$MartBakiye$", "0");

                            parser.SetVariable("$NisanTutar$", "0");
                            parser.SetVariable("$NisanOdenen$", "0");
                            parser.SetVariable("$NisanBorc$", "0");
                            parser.SetVariable("$NisanBakiye$", "0");

                            parser.SetVariable("$MayisTutar$", "0");
                            parser.SetVariable("$MayisOdenen$", "0");
                            parser.SetVariable("$MayisBorc$", "0");
                            parser.SetVariable("$MayisBakiye$", "0");

                            parser.SetVariable("$HaziranTutar$", "0");
                            parser.SetVariable("$HaziranOdenen$", "0");
                            parser.SetVariable("$HaziranBorc$", "0");
                            parser.SetVariable("$HaziranBakiye$", "0");

                            parser.SetVariable("$TemmuzTutar$", "0");
                            parser.SetVariable("$TemmuzOdenen$", "0");
                            parser.SetVariable("$TemmuzBorc$", "0");
                            parser.SetVariable("$TemmuzBakiye$", "0");

                            parser.SetVariable("$AgustosTutar$", "0");
                            parser.SetVariable("$AgustosOdenen$", "0");
                            parser.SetVariable("$AgustosBorc$", "0");
                            parser.SetVariable("$AgustosBakiye$", "0");

                            parser.SetVariable("$EylulTutar$", "0");
                            parser.SetVariable("$EylulOdenen$", "0");
                            parser.SetVariable("$EylulBorc$", "0");
                            parser.SetVariable("$EylulBakiye$", "0");

                            parser.SetVariable("$EkimTutar$", "0");
                            parser.SetVariable("$EkimOdenen$", "0");
                            parser.SetVariable("$EkimBorc$", "0");
                            parser.SetVariable("$EkimBakiye$", "0");

                            parser.SetVariable("$KasimTutar$", "0");
                            parser.SetVariable("$KasimOdenen$", "0");
                            parser.SetVariable("$KasimBorc$", "0");
                            parser.SetVariable("$KasimBakiye$", "0");

                            parser.SetVariable("$AralikTutar$", "0");
                            parser.SetVariable("$AralikOdenen$", "0");
                            parser.SetVariable("$AralikBorc$", "0");
                            parser.SetVariable("$AralikBakiye$", "0");

                            parser.SetVariable("$OdenecekToplam$", "0");
                            parser.SetVariable("$OdenenToplam$", "0");
                            parser.SetVariable("$BorcToplam$", "0");
                            parser.SetVariable("$ToplamBakiye$", "0");
                        }
                    }
                    parser.Parse();
                    pdf.Close();

                    byte[] fileData = pdf.GetFileBytes();
                    ICariHesapEkstrePDFStorage storage = DependencyResolver.Current.GetService<ICariHesapEkstrePDFStorage>();
                    string dosyaAdi = cariHesapAdi;
                    string fileName = String.Format(dosyaAdi + "_" + donem + "-Dönemi_CariHesapEkstresi_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile(fileName, fileData);
                    if (!String.IsNullOrEmpty(url))
                    {
                        return url;
                    }
                    else
                    {
                        return "hata";
                    }
                }
                catch (Exception ex)
                {
                    return "hata";
                }
                finally
                {
                    if (pdf != null)
                        pdf.Dispose();
                }
            }
            else
            {
                try
                {
                    string rootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                    string template = PdfTemplates.GetTemplate(rootPath + "Content/templates/", PdfTemplates.CARI_HESAP1);

                    pdf = new PDFHelper("NeoOnline", "CARİ HESAP EKSTRESİ", "CARİ HESAP EKSTRESİ", 8, rootPath + "Content/fonts/");

                    // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                    pdf.SetPageEventHelper(new PDFCustomEventHelper());
                    pdf.Rotate();
                    PDFParser parser = new PDFParser(template, pdf);

                    string cariHesapAdi = "";
                    string donem = "";
                    int k = model.ekstreList.Count - 1;

                    if (model.ekstreList.Count > 0)
                    {
                        donem = model.ekstreList[k].VadeTarihi.Year.ToString();
                        parser.SetVariable("$Donem$", "DÖNEM -" + donem);
                        parser.SetVariable("$ListeAdi$", "CARİ HESAP EKSTRESİ");
                        parser.SetVariable("$TVMUnvani$", _AktifKullanici.TVMUnvani);

                        if (!String.IsNullOrEmpty(hesapKodu))
                        {
                            var hesapBilgi = this.GetCariHesapAdi(hesapKodu);
                            cariHesapAdi = hesapBilgi.Item1 + " " + hesapBilgi.Item2;
                            parser.SetVariable("$CariHesapAdi$", cariHesapAdi);
                        }

                        if (!String.IsNullOrEmpty(musteriGrupKodu))
                        {
                            cariHesapAdi = musteriGrupKodu;
                            parser.SetVariable("$CariHesapAdi$", musteriGrupKodu);
                        }
                    }
                    string fiyatSatirTemplate = parser.GetTemplate("EkstreSatiri");
                    decimal yuruyenEkstreBakiye = 0;

                    for (int i = model.ekstreList.Count - 1; i >= 0; i--)
                    {
                        var item = model.ekstreList[i];
                        string listSatir = String.Empty;
                        listSatir = fiyatSatirTemplate.Replace("$VadeTarihi$", item.VadeTarihi.ToString("dd.MM.yyyy"));
                        listSatir = listSatir.Replace("$EvrakTarihi$", item.OdemeTarihi.HasValue ? item.OdemeTarihi.Value.ToString("dd.MM.yyyy") : "");
                        listSatir = listSatir.Replace("$EvrakOdemeTipi$", item.EvrakTipi + " " + item.OdemeTipi);
                        listSatir = listSatir.Replace("$EvrakNo$", item.EvrakNo);
                        listSatir = listSatir.Replace("$Aciklama$", item.Aciklama);
                        listSatir = listSatir.Replace("$ParaBirimi$", item.ParaBirimi);
                        listSatir = listSatir.Replace("$Borc$", item.Borc.HasValue ? item.Borc.Value.ToString("N2") : "");
                        listSatir = listSatir.Replace("$Alacak$", item.Alacak.HasValue ? item.Alacak.Value.ToString("N2") : "");
                        yuruyenEkstreBakiye += item.Bakiye.HasValue ? item.Bakiye.Value : 0;

                        if (yuruyenEkstreBakiye < 0)
                        {
                            yuruyenEkstreBakiye = yuruyenEkstreBakiye * -1;
                            listSatir = listSatir.Replace("$Bakiye$", yuruyenEkstreBakiye.ToString("N2"));
                            yuruyenEkstreBakiye = yuruyenEkstreBakiye * -1;
                        }
                        else
                        {
                            listSatir = listSatir.Replace("$Bakiye$", yuruyenEkstreBakiye.ToString("N2"));
                        }

                        if (yuruyenEkstreBakiye < 0)
                        {
                            listSatir = listSatir.Replace("$BA$", "A");
                        }
                        else
                        {
                            listSatir = listSatir.Replace("$BA$", "B");
                        }


                        parser.AppendToPlaceHolder("EkstreSatirlari", listSatir);
                    }
                    if (model.ekstreList.Count > 0)
                    {

                        var ekstreBorcToplam = model.ekstreList.Select(s => s.Borc).Sum();
                        var ekstreAlacakToplam = model.ekstreList.Select(s => s.Alacak).Sum();
                        var ekstreBakiyeToplam = ekstreBorcToplam - ekstreAlacakToplam;
                        ekstreBakiyeToplam = ekstreBakiyeToplam * -1;

                        if (ekstreBorcToplam < 0)
                        {
                            ekstreBorcToplam = ekstreBorcToplam * -1;
                            parser.SetVariable("$CEBorcToplam$", ekstreBorcToplam.Value.ToString("N2"));
                            ekstreBorcToplam = ekstreBorcToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEBorcToplam$", ekstreBorcToplam.Value.ToString("N2"));
                        }
                        if (ekstreAlacakToplam < 0)
                        {
                            ekstreAlacakToplam = ekstreAlacakToplam * -1;
                            parser.SetVariable("$CEAlacakToplam$", ekstreAlacakToplam.Value.ToString("N2"));
                            ekstreAlacakToplam = ekstreAlacakToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEAlacakToplam$", ekstreAlacakToplam.Value.ToString("N2"));
                        }
                        if (ekstreBakiyeToplam < 0)
                        {
                            ekstreBakiyeToplam = ekstreBakiyeToplam * -1;
                            parser.SetVariable("$CEBakiyeToplam$", ekstreBakiyeToplam.Value.ToString("N2"));
                            ekstreBakiyeToplam = ekstreBakiyeToplam * -1;
                        }
                        else
                        {
                            parser.SetVariable("$CEBakiyeToplam$", ekstreBakiyeToplam.Value.ToString("N2"));
                        }
                        if (ekstreBakiyeToplam < 0)
                        {
                            parser.SetVariable("$CEBAGenel$", "A");
                        }
                        else
                        {
                            parser.SetVariable("$CEBAGenel$", "B");
                        }

                    }

                    parser.Parse();
                    pdf.Close();

                    byte[] fileData = pdf.GetFileBytes();
                    ICariHesapEkstrePDFStorage storage = DependencyResolver.Current.GetService<ICariHesapEkstrePDFStorage>();
                    string dosyaAdi = cariHesapAdi;
                    string fileName = String.Format(dosyaAdi + "_" + donem + "-Dönemi_CariHesapEkstresi_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile(fileName, fileData);
                    if (!String.IsNullOrEmpty(url))
                    {
                        return url;
                    }
                    else
                    {
                        return "hata";
                    }
                }
                catch (Exception ex)
                {
                    return "hata";
                }
                finally
                {
                    if (pdf != null)
                        pdf.Dispose();
                }
            }

        }
        public class CariHesapEkstreModel
        {
            public int PoliceId { get; set; }
            public string OdemeTipi { get; set; }
            public DateTime? OdemeTarihi { get; set; }
            public string Aciklama { get; set; }
            public string ParaBirimi { get; set; }
            public decimal? Borc { get; set; }
            public decimal? Alacak { get; set; }
            public decimal? Bakiye { get; set; }
            public string EvrakNo { get; set; }
            public string EvrakTipi { get; set; }
            public DateTime VadeTarihi { get; set; }
            public string BorcTipi { get; set; }
            public string Adi { get; set; }
            public string Soyad { get; set; }
            public string MusteriGrupKodu { get; set; }
        }
        public CariHesaplari GetCariDetayGuncelleme(int id)
        {
            CariHesaplari detay = new CariHesaplari();
            //if (id != null)
            //{
            //    detay = _MuhasebeContext.CariHesaplariRepository.Filter(s => s.Id == id).FirstOrDefault();
            //}
            return detay;
        }
        public CariHesaplari GetCariKodu(string cariHesapKodu)
        {
            CariHesaplari list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapKodu == cariHesapKodu).FirstOrDefault();
            return list;
        }
        public List<CariHesaplari> getCariHesapListesiByUnvan(string unvan, int tvmKodu)
        {
            unvan = unvan.ToLower().Replace('ı', 'i');
            unvan.Trim();
            List<CariHesaplari> list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.Unvan.ToLower().Trim().StartsWith(unvan) && s.TVMKodu == tvmKodu).ToList();
            return list;
        }
        public List<CariHesaplari> getCariHesapListesiByMusteriGrupKodu(string musteriGrupKodu, int tvmKodu)
        {
            musteriGrupKodu = musteriGrupKodu.ToLower().Replace('ı', 'i');
            musteriGrupKodu.Trim();
            List<CariHesaplari> list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.MusteriGrupKodu.ToLower().Trim().StartsWith(musteriGrupKodu) && s.TVMKodu == tvmKodu).ToList();
            return list;
        }
        public List<CariHesaplari> getCariHesapListesiByCariHesapKodu(string cariHesapKodu, int tvmKodu)
        {
            List<CariHesaplari> list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.CariHesapKodu.StartsWith(cariHesapKodu) && s.TVMKodu == tvmKodu).ToList();
            return list;
        }
        public List<CariHesaplari> getCariHesapListesiAll(int tvmKodu)
        {

            List<CariHesaplari> list = _MuhasebeContext.CariHesaplariRepository.All().Where(s => s.TVMKodu == tvmKodu && !s.CariHesapKodu.StartsWith("120")).OrderBy(s => s.CariHesapKodu).ToList();


            return list;


        }
        public int CariHesapGuncelle(CariHesaplari hesap)
        {
            try
            {
                _MuhasebeContext.CariHesaplariRepository.Update(hesap);
                _MuhasebeContext.Commit();

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public void DeleteCariHareket(int id)
        {
            _MuhasebeContext.CariHareketleriRepository.Delete(m => m.Id == id);
            _KomisyonContext.Commit();
        }
        public void DeleteCariHesap(int id)
        {
            _MuhasebeContext.CariHesaplariRepository.Delete(m => m.CariHesapId == id);
            _MuhasebeContext.Commit();
        }
        public void DeleteCariHesapBorcAlacak(int id)
        {
            _MuhasebeContext.CariHesapBorcAlacakRepository.Delete(m => m.Id == id);
            _MuhasebeContext.Commit();
        }
        void fnk_YeniCariHesapBorcAlacak(CariHesapBorcAlacak kayit, string cariHesapKodu, decimal brutPrim, decimal komisyonTutari, int ay)
        {
            if (ay == 1)
            {
                switch (cariHesapKodu.Substring(0, 3))
                {
                    case "100":
                        kayit.Borc1 += brutPrim;
                        break;
                    case "120":
                        kayit.Alacak1 += brutPrim;
                        kayit.Borc1 += brutPrim;
                        break;
                    case "320":
                        kayit.Alacak1 += brutPrim;
                        kayit.Borc1 += komisyonTutari;
                        break;
                    case "600":
                        kayit.Alacak1 += komisyonTutari;
                        break;
                    case "610":
                        kayit.Borc1 += komisyonTutari;
                        break;
                    default:
                        break;
                }
            }
            if (ay == 2)
            {
                switch (cariHesapKodu.Substring(0, 3))
                {
                    case "100":
                        kayit.Borc2 += brutPrim;
                        break;
                    case "120":
                        kayit.Alacak2 += brutPrim;
                        kayit.Borc2 += brutPrim;
                        break;
                    case "320":
                        kayit.Alacak2 += brutPrim;
                        kayit.Borc2 += komisyonTutari;
                        break;
                    case "600":
                        kayit.Alacak2 += komisyonTutari;
                        break;
                    case "610":
                        kayit.Borc2 += komisyonTutari;
                        break;
                    default:
                        break;
                }
            }
            if (ay == 3)
            {
                switch (cariHesapKodu.Substring(0, 3))
                {
                    case "100":
                        kayit.Borc3 += brutPrim;
                        break;
                    case "120":
                        kayit.Alacak3 += brutPrim;
                        kayit.Borc3 += brutPrim;
                        break;
                    case "320":
                        kayit.Alacak3 += brutPrim;
                        kayit.Borc3 += komisyonTutari;
                        break;
                    case "600":
                        kayit.Alacak3 += komisyonTutari;
                        break;
                    case "610":
                        kayit.Borc3 += komisyonTutari;
                        break;
                    default:
                        break;
                }
            }
        }
        //edit
        public void UpdateYeniCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak, decimal brutPrim, decimal komisyonTutari)
        {
            int donemInt = Convert.ToInt32(donem);
            int ayInt = Convert.ToInt32(ay);
            var kayit = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(m => m.TVMKodu == TVMKodu && m.CariHesapKodu == cariHesapKodu && m.Donem == donemInt).FirstOrDefault();

            if (ayInt == 1)
            {
                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak1 = kayit.Alacak1 - alacak;
                kayit.Borc1 = kayit.Borc1 - borc;
                fnk_YeniCariHesapBorcAlacak(kayit, cariHesapKodu, brutPrim, komisyonTutari, ayInt);
            }
            else if (ayInt == 2)
            {
                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak2 = kayit.Alacak2 - alacak;
                kayit.Borc2 = kayit.Borc2 - borc;
                fnk_YeniCariHesapBorcAlacak(kayit, cariHesapKodu, brutPrim, komisyonTutari, ayInt);

            }
            else if (ayInt == 3)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak3 = kayit.Alacak3 - alacak;
                kayit.Borc3 = kayit.Borc3 - borc;
                fnk_YeniCariHesapBorcAlacak(kayit, cariHesapKodu, brutPrim, komisyonTutari, ayInt);

            }
            else if (ayInt == 4)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak4 = kayit.Alacak4 - alacak;
                kayit.Borc4 = kayit.Borc4 - borc;
                fnk_YeniCariHesapBorcAlacak(kayit, cariHesapKodu, brutPrim, komisyonTutari, ayInt);


            }
            else if (ayInt == 5)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak5 = kayit.Alacak5 - alacak;
                kayit.Borc5 = kayit.Borc5 - borc;
                fnk_YeniCariHesapBorcAlacak(kayit, cariHesapKodu, brutPrim, komisyonTutari, ayInt);


            }
            else if (ayInt == 6)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak6 = kayit.Alacak6 - alacak;
                kayit.Borc6 = kayit.Borc6 - borc;


            }
            else if (ayInt == 7)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak7 = kayit.Alacak7 - alacak;
                kayit.Borc7 = kayit.Borc7 - borc;


            }
            else if (ayInt == 8)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak8 = kayit.Alacak8 - alacak;
                kayit.Borc8 = kayit.Borc8 - borc;


            }
            else if (ayInt == 9)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak9 = kayit.Alacak9 - alacak;
                kayit.Borc9 = kayit.Borc9 - borc;


            }
            else if (ayInt == 10)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak10 = kayit.Alacak10 - alacak;
                kayit.Borc10 = kayit.Borc10 - borc;


            }
            else if (ayInt == 11)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak11 = kayit.Alacak11 - alacak;
                kayit.Borc11 = kayit.Borc11 - borc;


            }
            else if (ayInt == 12)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak12 = kayit.Alacak12 - alacak;
                kayit.Borc12 = kayit.Borc12 - borc;


            }
            kayit.GuncellemeTarihi = DateTime.Now;
            _MuhasebeContext.CariHesapBorcAlacakRepository.Update(kayit);
            _MuhasebeContext.Commit();

        }


        public void UpdateCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak)
        {
            int donemInt = Convert.ToInt32(donem);
            int ayInt = Convert.ToInt32(ay);
            var kayit = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(m => m.TVMKodu == TVMKodu && m.CariHesapKodu == cariHesapKodu && m.Donem == donemInt).FirstOrDefault();

            if (ayInt == 1)
            {
                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak1 = kayit.Alacak1 - alacak;
                kayit.Borc1 = kayit.Borc1 - borc;
            }
            else if (ayInt == 2)
            {
                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak2 = kayit.Alacak2 - alacak;
                kayit.Borc2 = kayit.Borc2 - borc;
            }
            else if (ayInt == 3)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak3 = kayit.Alacak3 - alacak;
                kayit.Borc3 = kayit.Borc3 - borc;

            }
            else if (ayInt == 4)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak4 = kayit.Alacak4 - alacak;
                kayit.Borc4 = kayit.Borc4 - borc;


            }
            else if (ayInt == 5)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak5 = kayit.Alacak5 - alacak;
                kayit.Borc5 = kayit.Borc5 - borc;


            }
            else if (ayInt == 6)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak6 = kayit.Alacak6 - alacak;
                kayit.Borc6 = kayit.Borc6 - borc;


            }
            else if (ayInt == 7)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak7 = kayit.Alacak7 - alacak;
                kayit.Borc7 = kayit.Borc7 - borc;


            }
            else if (ayInt == 8)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak8 = kayit.Alacak8 - alacak;
                kayit.Borc8 = kayit.Borc8 - borc;


            }
            else if (ayInt == 9)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak9 = kayit.Alacak9 - alacak;
                kayit.Borc9 = kayit.Borc9 - borc;


            }
            else if (ayInt == 10)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak10 = kayit.Alacak10 - alacak;
                kayit.Borc10 = kayit.Borc10 - borc;


            }
            else if (ayInt == 11)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak11 = kayit.Alacak11 - alacak;
                kayit.Borc11 = kayit.Borc11 - borc;


            }
            else if (ayInt == 12)
            {

                if (alacak < 0 || borc < 0)
                {
                    alacak = alacak * -1;
                    borc = borc * -1;
                }
                kayit.Alacak12 = kayit.Alacak12 - alacak;
                kayit.Borc12 = kayit.Borc12 - borc;


            }
            kayit.GuncellemeTarihi = DateTime.Now;
            _MuhasebeContext.CariHesapBorcAlacakRepository.Update(kayit);
            _MuhasebeContext.Commit();

        }


        public void UpdatePolTahCariHesapBorcAlacak(int TVMKodu, string cariHesapKodu, string donem, string ay, decimal borc, decimal alacak)
        {
            int donemInt = Convert.ToInt32(donem);
            int ayInt = Convert.ToInt32(ay);
            try
            {
                var kayit = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(m => m.TVMKodu == TVMKodu && m.CariHesapKodu == cariHesapKodu && m.Donem == donemInt).FirstOrDefault();
                if (kayit != null && kayit.Donem == donemInt)
                {
                    if (ayInt == 1)
                    {
                        kayit.Alacak1 += alacak;
                        kayit.Borc1 += borc;
                    }
                    else if (ayInt == 2)
                    {
                        kayit.Alacak2 += alacak;
                        kayit.Borc2 += borc;
                    }
                    else if (ayInt == 3)
                    {
                        kayit.Alacak3 += alacak;
                        kayit.Borc3 += borc;
                    }
                    else if (ayInt == 4)
                    {
                        kayit.Alacak4 += alacak;
                        kayit.Borc4 += borc;
                    }
                    else if (ayInt == 5)
                    {
                        kayit.Alacak5 += alacak;
                        kayit.Borc5 += borc;
                    }
                    else if (ayInt == 6)
                    {
                        kayit.Alacak6 += alacak;
                        kayit.Borc6 += borc;
                    }
                    else if (ayInt == 7)
                    {
                        kayit.Alacak7 += alacak;
                        kayit.Borc7 += borc;
                    }
                    else if (ayInt == 8)
                    {
                        kayit.Alacak8 += alacak;
                        kayit.Borc8 += borc;
                    }
                    else if (ayInt == 9)
                    {
                        kayit.Alacak9 += alacak;
                        kayit.Borc9 += borc;
                    }
                    else if (ayInt == 10)
                    {
                        kayit.Alacak10 += alacak;
                        kayit.Borc10 += borc;
                    }
                    else if (ayInt == 11)
                    {
                        kayit.Alacak11 += alacak;
                        kayit.Borc11 += borc;
                    }
                    else if (ayInt == 12)
                    {
                        kayit.Alacak12 += alacak;
                        kayit.Borc12 += borc;
                    }
                    kayit.GuncellemeTarihi = DateTime.Now;
                    _MuhasebeContext.CariHesapBorcAlacakRepository.Update(kayit);
                    _MuhasebeContext.Commit();
                }
                if (kayit == null)
                {
                    #region cari borc alacak ekle
                    CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();

                    if (cariHesapKodu != null)
                    {
                        int donemYoksa = Convert.ToInt32(donem);
                        int ayYoksa = Convert.ToInt32(ay);
                        var cariHesapVarMi = getCariHesapBilgi(TVMKodu, cariHesapKodu);
                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapKodu;
                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapVarMi.KimlikNo;
                        cariHesapBorcAlacakEkle.TVMKodu = TVMKodu;
                        cariHesapBorcAlacakEkle.CariHesapId = cariHesapVarMi.CariHesapId;
                        cariHesapBorcAlacakEkle.Donem = Convert.ToInt32(donem);
                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                        cariHesapBorcAlacakEkle.Borc1 = 0;
                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                        cariHesapBorcAlacakEkle.Borc2 = 0;
                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                        cariHesapBorcAlacakEkle.Borc3 = 0;
                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                        cariHesapBorcAlacakEkle.Borc4 = 0;
                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                        cariHesapBorcAlacakEkle.Borc5 = 0;
                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                        cariHesapBorcAlacakEkle.Borc6 = 0;
                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                        cariHesapBorcAlacakEkle.Borc7 = 0;
                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                        cariHesapBorcAlacakEkle.Borc8 = 0;
                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                        cariHesapBorcAlacakEkle.Borc9 = 0;
                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                        cariHesapBorcAlacakEkle.Borc10 = 0;
                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                        cariHesapBorcAlacakEkle.Borc11 = 0;
                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                        cariHesapBorcAlacakEkle.Borc12 = 0;
                        _MuhasebeContext.CariHesapBorcAlacakRepository.Create(cariHesapBorcAlacakEkle);
                        _MuhasebeContext.Commit();
                        var kayitYoksa = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(m => m.TVMKodu == TVMKodu && m.CariHesapKodu == cariHesapKodu && m.Donem == donemYoksa).FirstOrDefault();
                        if (ayYoksa == 1)
                        {
                            kayitYoksa.Alacak1 += alacak;
                            kayitYoksa.Borc1 += borc;
                        }
                        else if (ayYoksa == 2)
                        {
                            kayitYoksa.Alacak2 += alacak;
                            kayitYoksa.Borc2 += borc;
                        }
                        else if (ayYoksa == 3)
                        {
                            kayitYoksa.Alacak3 += alacak;
                            kayitYoksa.Borc3 += borc;
                        }
                        else if (ayYoksa == 4)
                        {
                            kayitYoksa.Alacak4 += alacak;
                            kayitYoksa.Borc4 += borc;
                        }
                        else if (ayYoksa == 5)
                        {
                            kayitYoksa.Alacak5 += alacak;
                            kayitYoksa.Borc5 += borc;
                        }
                        else if (ayYoksa == 6)
                        {
                            kayitYoksa.Alacak6 += alacak;
                            kayitYoksa.Borc6 += borc;
                        }
                        else if (ayYoksa == 7)
                        {
                            kayitYoksa.Alacak7 += alacak;
                            kayitYoksa.Borc7 += borc;
                        }
                        else if (ayYoksa == 8)
                        {
                            kayitYoksa.Alacak8 += alacak;
                            kayitYoksa.Borc8 += borc;
                        }
                        else if (ayYoksa == 9)
                        {
                            kayitYoksa.Alacak9 += alacak;
                            kayitYoksa.Borc9 += borc;
                        }
                        else if (ayYoksa == 10)
                        {
                            kayitYoksa.Alacak10 += alacak;
                            kayitYoksa.Borc10 += borc;
                        }
                        else if (ayYoksa == 11)
                        {
                            kayitYoksa.Alacak11 += alacak;
                            kayitYoksa.Borc11 += borc;
                        }
                        else if (ayYoksa == 12)
                        {
                            kayitYoksa.Alacak12 += alacak;
                            kayitYoksa.Borc12 += borc;
                        }
                        kayitYoksa.GuncellemeTarihi = DateTime.Now;
                        _MuhasebeContext.CariHesapBorcAlacakRepository.Update(kayitYoksa);
                        _MuhasebeContext.Commit();
                    }
                    #endregion

                }

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        #endregion

        #region Cari Hesap Borç Alacak

        public CariHesapBorcalacakPeocedureModels GetBorcAlacak(int Donem, string CariHesapKodu)
        {
            CariHesapBorcalacakPeocedureModels borcAlacak = _MuhasebeContext.BorcAlacak_Getir(_AktifKullanici.TVMKodu, Donem, CariHesapKodu);

            return borcAlacak;
        }
        public class CariHesapEkstreListModel
        {
            public string PDFURL { get; set; }
            public List<CariHesapEkstreModel> ekstreList = new List<CariHesapEkstreModel>();
            public CariHesapBorcalacakPeocedureModels cariHesap = new CariHesapBorcalacakPeocedureModels();
        }

        #endregion

        #region Cari Aktarım İşlemleri
        //public bool CariHesapVarMi(string tcVkn,int tvmKodu)
        //{
        //    var cariHesap = _MuhasebeContext.CariHesaplariRepository.All().Where(w=>w.KimlikNo== tcVkn &&w.TVMKodu==tvmKodu ).FirstOrDefault();
        //    if (cariHesap!=null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
        public List<CariHesapBAReturnModel> CariAktarimIslemleri(int tvmkodu, DateTime tanzimBaslangic, DateTime tanzimBitis)
        {
            List<CariHesapBAReturnModel> returnList = new List<CariHesapBAReturnModel>();
            var policeLsit = _PoliceContext.PoliceGenelRepository.All().Where(w => w.TVMKodu == tvmkodu && w.TanzimTarihi >= tanzimBaslangic &&
            w.TanzimTarihi <= tanzimBitis && w.CariHareketKayitTarihi == null).ToList();


            List<CariHesapBorcAlacakServiceModel> BAList = new List<CariHesapBorcAlacakServiceModel>();
            CariHesapBorcAlacakServiceModel borcAlacak = new CariHesapBorcAlacakServiceModel();
            CariHareketleri carHareket = new CariHareketleri();
            CariHareketKaydetModel model = new CariHareketKaydetModel();
            List<CariHareketKaydetModel> carHareketList = new List<CariHareketKaydetModel>();

            if (policeLsit.Count > 0)
            {
                foreach (var item in policeLsit)
                {
                    if (item.PoliceTahsilats.Count > 0 || (item.Komisyon.HasValue && item.Komisyon.Value != 0))
                    {
                        model = new CariHareketKaydetModel();

                        string tcVkn = String.Empty;
                        bool musteriHesapVarMi = false;
                        bool sirketHesapVarMi = false;
                        bool sirketKomGelirHesapVarMi = false;
                        bool sirketKomGiderHesapVarMi = false;
                        string sirketHesapKodu = String.Empty;
                        string sirketVkn = String.Empty;
                        string sirketKomGelirHesapKodu = String.Empty;
                        string sirketKomGiderHesapKodu = String.Empty;
                        string musteriHesapKodu = String.Empty;
                        if (!String.IsNullOrEmpty(item.PoliceSigortaEttiren.KimlikNo))
                        {
                            if (item.PoliceSigortaEttiren.KimlikNo.Length == 11)
                            {
                                tcVkn = item.PoliceSigortaEttiren.KimlikNo;
                            }
                        }
                        if (!String.IsNullOrEmpty(item.PoliceSigortaEttiren.VergiKimlikNo))
                        {
                            if (item.PoliceSigortaEttiren.VergiKimlikNo.Length == 10)
                            {
                                tcVkn = item.PoliceSigortaEttiren.VergiKimlikNo.Trim();
                            }
                        }
                        if (!String.IsNullOrEmpty(tcVkn))
                        {

                            CariHesaplari musteriHesap = new CariHesaplari();
                            musteriHesapKodu = "120.01." + tcVkn;
                            musteriHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, musteriHesapKodu);
                            if (musteriHesap != null)
                            {
                                musteriHesapVarMi = true;
                            }
                            CariHesaplari sirketHesap = new CariHesaplari();
                            CariHesaplari sirketKomGelirHesap = new CariHesaplari();
                            CariHesaplari sirketKomGiderHesap = new CariHesaplari();

                            var getSirket = _SigortaSirketleriService.GetSirket(item.TUMBirlikKodu);
                            if (getSirket != null)
                            {
                                sirketHesapKodu = "320.01." + getSirket.VergiNumarasi;
                                sirketHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketHesapKodu);
                                if (sirketHesap != null)
                                {
                                    sirketHesapVarMi = true;
                                }
                                sirketKomGelirHesapKodu = "600.01." + getSirket.VergiNumarasi;
                                sirketKomGelirHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketKomGelirHesapKodu);
                                if (sirketKomGelirHesap != null)
                                {
                                    sirketKomGelirHesapVarMi = true;
                                }
                                sirketKomGiderHesapKodu = "610.01." + getSirket.VergiNumarasi;
                                sirketKomGiderHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketKomGiderHesapKodu);
                                if (sirketKomGiderHesap != null)
                                {
                                    sirketKomGiderHesapVarMi = true;
                                }
                            }

                            if (!musteriHesapVarMi)
                            {
                                try
                                {
                                    CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesaplari cariHesapEkle = new CariHesaplari();
                                    cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                    cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                    cariHesapEkle.UlkeKodu = item.PoliceSigortaEttiren.UlkeKodu;
                                    cariHesapEkle.IlKodu = item.PoliceSigortaEttiren.IlKodu;
                                    cariHesapEkle.IlceKodu = item.PoliceSigortaEttiren.IlceKodu.HasValue ? item.PoliceSigortaEttiren.IlceKodu.Value : 0;
                                    cariHesapEkle.KayitTarihi = DateTime.Now;
                                    cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                    cariHesapEkle.Adres = item.PoliceSigortaEttiren.Adres;
                                    cariHesapEkle.Unvan = item.PoliceSigortaEttiren.AdiUnvan + item.PoliceSigortaEttiren.SoyadiUnvan;
                                    cariHesapEkle.Telefon1 = item.PoliceSigortaEttiren.TelefonNo;
                                    cariHesapEkle.CepTel = item.PoliceSigortaEttiren.MobilTelefonNo;
                                    cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                                    cariHesapEkle.Email = item.PoliceSigortaEttiren.EMail;
                                    cariHesapEkle.PostaKodu = item.PoliceSigortaEttiren.PostaKodu.HasValue ? item.PoliceSigortaEttiren.PostaKodu.Value : 0;
                                    cariHesapEkle.KimlikNo = tcVkn;
                                    cariHesapEkle.CariHesapTipi = "120.01.";
                                    cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + tcVkn;
                                    var musteriDetay = _MusteriService.GetMusteri(tcVkn, tvmkodu);
                                    if (musteriDetay != null)
                                    {
                                        if (!String.IsNullOrEmpty(musteriDetay.TVMMusteriKodu))
                                        {
                                            cariHesapEkle.MusteriGrupKodu = musteriDetay.TVMMusteriKodu;
                                        }
                                    }

                                    musteriHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                    _MuhasebeContext.Commit();
                                    if (musteriHesap.CariHesapKodu != null)
                                    {
                                        cariHesapBorcAlacakEkle.CariHesapKodu = musteriHesapKodu;
                                        cariHesapBorcAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                        cariHesapBorcAlacakEkle.KimlikNo = tcVkn;
                                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                        cariHesapBorcAlacakEkle.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                                        cariHesapBorcAlacakEkle.Borc1 = 0;
                                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                                        cariHesapBorcAlacakEkle.Borc2 = 0;
                                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                                        cariHesapBorcAlacakEkle.Borc3 = 0;
                                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                                        cariHesapBorcAlacakEkle.Borc4 = 0;
                                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                                        cariHesapBorcAlacakEkle.Borc5 = 0;
                                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                                        cariHesapBorcAlacakEkle.Borc6 = 0;
                                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                                        cariHesapBorcAlacakEkle.Borc7 = 0;
                                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                                        cariHesapBorcAlacakEkle.Borc8 = 0;
                                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                                        cariHesapBorcAlacakEkle.Borc9 = 0;
                                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                                        cariHesapBorcAlacakEkle.Borc10 = 0;
                                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                                        cariHesapBorcAlacakEkle.Borc11 = 0;
                                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                                        cariHesapBorcAlacakEkle.Borc12 = 0;
                                        var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                        if (hesapEkleReturn == 0)
                                        {
                                            CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                            rModel.Basarili = false;
                                            rModel.CariHesapKodu = musteriHesapKodu;
                                            rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                            rModel.PoliceNo = item.PoliceNumarasi;
                                            rModel.YenilemeNo = item.YenilemeNo;
                                            rModel.EkNo = item.EkNo;
                                            rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                            rModel.TvmKodu = item.TVMKodu;
                                            rModel.Mesaj = "Yeni açılan müşteri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                            returnList.Add(rModel);
                                        }
                                    }
                                    musteriHesapVarMi = true;
                                }
                                catch (Exception ex)
                                {
                                    CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                    rModel.Basarili = false;
                                    rModel.CariHesapKodu = musteriHesapKodu;
                                    rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                    rModel.PoliceNo = item.PoliceNumarasi;
                                    rModel.YenilemeNo = item.YenilemeNo;
                                    rModel.EkNo = item.EkNo;
                                    rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                    rModel.TvmKodu = item.TVMKodu;
                                    rModel.Mesaj = "Müşteri Hesap Ekleme Hata:" + ex.ToString();
                                    returnList.Add(rModel);
                                }
                            }
                            if (!sirketHesapVarMi)
                            {
                                try
                                {
                                    CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesaplari cariHesapEkle = new CariHesaplari();
                                    cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                    cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                    cariHesapEkle.KayitTarihi = DateTime.Now;
                                    cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                    cariHesapEkle.Unvan = getSirket.SirketAdi;
                                    cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";

                                    if (getSirket != null)
                                    {
                                        cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                    }
                                    getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                    cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                    cariHesapEkle.CariHesapTipi = "320.01.";
                                    cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;
                                    cariHesapEkle.KomisyonGelirleriMuhasebeKodu = "600.01." + getSirket.VergiNumarasi;
                                    cariHesapEkle.SatisIadeleriMuhasebeKodu = "610.01." + getSirket.VergiNumarasi;
                                    sirketHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                    _MuhasebeContext.Commit();
                                    if (sirketHesap.CariHesapKodu != null)
                                    {
                                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                        cariHesapBorcAlacakEkle.CariHesapId = sirketHesap.CariHesapId;
                                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                        cariHesapBorcAlacakEkle.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                                        cariHesapBorcAlacakEkle.Borc1 = 0;
                                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                                        cariHesapBorcAlacakEkle.Borc2 = 0;
                                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                                        cariHesapBorcAlacakEkle.Borc3 = 0;
                                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                                        cariHesapBorcAlacakEkle.Borc4 = 0;
                                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                                        cariHesapBorcAlacakEkle.Borc5 = 0;
                                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                                        cariHesapBorcAlacakEkle.Borc6 = 0;
                                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                                        cariHesapBorcAlacakEkle.Borc7 = 0;
                                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                                        cariHesapBorcAlacakEkle.Borc8 = 0;
                                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                                        cariHesapBorcAlacakEkle.Borc9 = 0;
                                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                                        cariHesapBorcAlacakEkle.Borc10 = 0;
                                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                                        cariHesapBorcAlacakEkle.Borc11 = 0;
                                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                                        cariHesapBorcAlacakEkle.Borc12 = 0;
                                        var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                        if (hesapEkleReturn == 0)
                                        {
                                            CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                            rModel.Basarili = false;
                                            rModel.CariHesapKodu = musteriHesapKodu;
                                            rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                            rModel.PoliceNo = item.PoliceNumarasi;
                                            rModel.YenilemeNo = item.YenilemeNo;
                                            rModel.EkNo = item.EkNo;
                                            rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                            rModel.TvmKodu = item.TVMKodu;
                                            rModel.Mesaj = "Yeni açılan sigorta şirketi hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                            returnList.Add(rModel);
                                        }
                                    }
                                    sirketHesapVarMi = true;
                                }
                                catch (Exception ex)
                                {
                                    CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                    rModel.Basarili = false;
                                    rModel.CariHesapKodu = sirketHesapKodu;
                                    rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                    rModel.PoliceNo = item.PoliceNumarasi;
                                    rModel.YenilemeNo = item.YenilemeNo;
                                    rModel.EkNo = item.EkNo;
                                    rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                    rModel.TvmKodu = item.TVMKodu;
                                    rModel.Mesaj = "Sigorta Şirketi Hesap Ekleme Hata:" + ex.ToString();
                                    returnList.Add(rModel);
                                }
                            }
                            if (!sirketKomGelirHesapVarMi)
                            {
                                try
                                {
                                    CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesaplari cariHesapEkle = new CariHesaplari();
                                    cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                    cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                    cariHesapEkle.KayitTarihi = DateTime.Now;
                                    cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                    cariHesapEkle.Unvan = getSirket.SirketAdi;
                                    cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                                    if (getSirket != null)
                                    {
                                        cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                    }
                                    getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                    cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                    cariHesapEkle.CariHesapTipi = "600.01.";
                                    cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;

                                    sirketKomGelirHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                    _MuhasebeContext.Commit();
                                    if (sirketKomGelirHesap.CariHesapKodu != null)
                                    {
                                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                        cariHesapBorcAlacakEkle.CariHesapId = sirketKomGelirHesap.CariHesapId;
                                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                        cariHesapBorcAlacakEkle.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                                        cariHesapBorcAlacakEkle.Borc1 = 0;
                                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                                        cariHesapBorcAlacakEkle.Borc2 = 0;
                                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                                        cariHesapBorcAlacakEkle.Borc3 = 0;
                                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                                        cariHesapBorcAlacakEkle.Borc4 = 0;
                                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                                        cariHesapBorcAlacakEkle.Borc5 = 0;
                                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                                        cariHesapBorcAlacakEkle.Borc6 = 0;
                                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                                        cariHesapBorcAlacakEkle.Borc7 = 0;
                                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                                        cariHesapBorcAlacakEkle.Borc8 = 0;
                                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                                        cariHesapBorcAlacakEkle.Borc9 = 0;
                                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                                        cariHesapBorcAlacakEkle.Borc10 = 0;
                                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                                        cariHesapBorcAlacakEkle.Borc11 = 0;
                                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                                        cariHesapBorcAlacakEkle.Borc12 = 0;
                                        var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                        if (hesapEkleReturn == 0)
                                        {
                                            CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                            rModel.Basarili = false;
                                            rModel.CariHesapKodu = musteriHesapKodu;
                                            rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                            rModel.PoliceNo = item.PoliceNumarasi;
                                            rModel.YenilemeNo = item.YenilemeNo;
                                            rModel.EkNo = item.EkNo;
                                            rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                            rModel.TvmKodu = item.TVMKodu;
                                            rModel.Mesaj = "Yeni açılan sigorta şirketi komisyon gelirleri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                            returnList.Add(rModel);
                                        }
                                    }
                                    sirketKomGelirHesapVarMi = true;

                                }
                                catch (Exception ex)
                                {
                                    CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                    rModel.Basarili = false;
                                    rModel.CariHesapKodu = sirketKomGelirHesapKodu;
                                    rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                    rModel.PoliceNo = item.PoliceNumarasi;
                                    rModel.YenilemeNo = item.YenilemeNo;
                                    rModel.EkNo = item.EkNo;
                                    rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                    rModel.TvmKodu = item.TVMKodu;
                                    rModel.Mesaj = "Sigorta Şirketi Komisyon Gelir Hesabı Ekleme Hata:" + ex.ToString();
                                    returnList.Add(rModel);
                                }
                            }
                            if (!sirketKomGiderHesapVarMi)
                            {
                                try
                                {
                                    CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesaplari cariHesapEkle = new CariHesaplari();
                                    cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                    cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                    cariHesapEkle.KayitTarihi = DateTime.Now;
                                    cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                    cariHesapEkle.Unvan = getSirket.SirketAdi;
                                    cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";

                                    if (getSirket != null)
                                    {
                                        cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                    }
                                    getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                    cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                    cariHesapEkle.CariHesapTipi = "610.01.";
                                    cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;

                                    sirketKomGiderHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                    _MuhasebeContext.Commit();
                                    if (sirketKomGiderHesap.CariHesapKodu != null)
                                    {
                                        cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                        cariHesapBorcAlacakEkle.CariHesapId = sirketKomGiderHesap.CariHesapId;
                                        cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                        cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                        cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                        cariHesapBorcAlacakEkle.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                        cariHesapBorcAlacakEkle.Alacak1 = 0;
                                        cariHesapBorcAlacakEkle.Borc1 = 0;
                                        cariHesapBorcAlacakEkle.Alacak2 = 0;
                                        cariHesapBorcAlacakEkle.Borc2 = 0;
                                        cariHesapBorcAlacakEkle.Alacak3 = 0;
                                        cariHesapBorcAlacakEkle.Borc3 = 0;
                                        cariHesapBorcAlacakEkle.Alacak4 = 0;
                                        cariHesapBorcAlacakEkle.Borc4 = 0;
                                        cariHesapBorcAlacakEkle.Alacak5 = 0;
                                        cariHesapBorcAlacakEkle.Borc5 = 0;
                                        cariHesapBorcAlacakEkle.Alacak6 = 0;
                                        cariHesapBorcAlacakEkle.Borc6 = 0;
                                        cariHesapBorcAlacakEkle.Alacak7 = 0;
                                        cariHesapBorcAlacakEkle.Borc7 = 0;
                                        cariHesapBorcAlacakEkle.Alacak8 = 0;
                                        cariHesapBorcAlacakEkle.Borc8 = 0;
                                        cariHesapBorcAlacakEkle.Alacak9 = 0;
                                        cariHesapBorcAlacakEkle.Borc9 = 0;
                                        cariHesapBorcAlacakEkle.Alacak10 = 0;
                                        cariHesapBorcAlacakEkle.Borc10 = 0;
                                        cariHesapBorcAlacakEkle.Alacak11 = 0;
                                        cariHesapBorcAlacakEkle.Borc11 = 0;
                                        cariHesapBorcAlacakEkle.Alacak12 = 0;
                                        cariHesapBorcAlacakEkle.Borc12 = 0;
                                        var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                        if (hesapEkleReturn == 0)
                                        {
                                            CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                            rModel.Basarili = false;
                                            rModel.CariHesapKodu = musteriHesapKodu;
                                            rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                            rModel.PoliceNo = item.PoliceNumarasi;
                                            rModel.YenilemeNo = item.YenilemeNo;
                                            rModel.EkNo = item.EkNo;
                                            rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                            rModel.TvmKodu = item.TVMKodu;
                                            rModel.Mesaj = "Yeni açılan sigorta şirketi komisyon iadeleri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                            returnList.Add(rModel);
                                        }
                                    }
                                    sirketKomGiderHesapVarMi = true;
                                }
                                catch (Exception ex)
                                {
                                    CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                    rModel.Basarili = false;
                                    rModel.CariHesapKodu = sirketKomGiderHesapKodu;
                                    rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                                    rModel.PoliceNo = item.PoliceNumarasi;
                                    rModel.YenilemeNo = item.YenilemeNo;
                                    rModel.EkNo = item.EkNo;
                                    rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                                    rModel.TvmKodu = item.TVMKodu;
                                    rModel.Mesaj = "Sigorta Şirketi Komisyon İade Hesabı Ekleme Hata:" + ex.ToString();
                                    returnList.Add(rModel);
                                }
                            }

                            if (musteriHesapVarMi && sirketHesapVarMi && sirketKomGelirHesapVarMi)
                            {
                                foreach (var itemTah in item.PoliceTahsilats)
                                {
                                    #region Tahakkuk

                                    if (itemTah.TaksitTutari > 0)
                                    {
                                        #region Müşteri Hesabına Borç Yazılıyor
                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = tcVkn;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = musteriHesapKodu;
                                        borcAlacak.CariHesapId = musteriHesap.CariHesapId;
                                        borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                        borcAlacak.PoliceId = itemTah.PoliceId;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 1: borcAlacak.Borc1 = itemTah.TaksitTutari; break;
                                            case 2: borcAlacak.Borc2 = itemTah.TaksitTutari; break;
                                            case 3: borcAlacak.Borc3 = itemTah.TaksitTutari; break;
                                            case 4: borcAlacak.Borc4 = itemTah.TaksitTutari; break;
                                            case 5: borcAlacak.Borc5 = itemTah.TaksitTutari; break;
                                            case 6: borcAlacak.Borc6 = itemTah.TaksitTutari; break;
                                            case 7: borcAlacak.Borc7 = itemTah.TaksitTutari; break;
                                            case 8: borcAlacak.Borc8 = itemTah.TaksitTutari; break;
                                            case 9: borcAlacak.Borc9 = itemTah.TaksitTutari; break;
                                            case 10: borcAlacak.Borc10 = itemTah.TaksitTutari; break;
                                            case 11: borcAlacak.Borc11 = itemTah.TaksitTutari; break;
                                            case 12: borcAlacak.Borc12 = itemTah.TaksitTutari; break;
                                            default:
                                                break;
                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion                                       

                                        #region Sigorta Şirketi Hesabına Alacak Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "A";
                                        carHareket.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                        carHareket.CariHesapKodu = sirketHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo + "-" + itemTah.TaksitNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = itemTah.TaksitTutari;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }

                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketHesapKodu;
                                        borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                        borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                        borcAlacak.PoliceId = itemTah.PoliceId;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 1: borcAlacak.Alacak1 = itemTah.TaksitTutari; break;
                                            case 2: borcAlacak.Alacak2 = itemTah.TaksitTutari; break;
                                            case 3: borcAlacak.Alacak3 = itemTah.TaksitTutari; break;
                                            case 4: borcAlacak.Alacak4 = itemTah.TaksitTutari; break;
                                            case 5: borcAlacak.Alacak5 = itemTah.TaksitTutari; break;
                                            case 6: borcAlacak.Alacak6 = itemTah.TaksitTutari; break;
                                            case 7: borcAlacak.Alacak7 = itemTah.TaksitTutari; break;
                                            case 8: borcAlacak.Alacak8 = itemTah.TaksitTutari; break;
                                            case 9: borcAlacak.Alacak9 = itemTah.TaksitTutari; break;
                                            case 10: borcAlacak.Alacak10 = itemTah.TaksitTutari; break;
                                            case 11: borcAlacak.Alacak11 = itemTah.TaksitTutari; break;
                                            case 12: borcAlacak.Alacak12 = itemTah.TaksitTutari; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);

                                        #endregion



                                    }
                                    #endregion

                                    #region İptal

                                    if (itemTah.TaksitTutari < 0)
                                    {
                                        var tutarCarpan = -1;
                                        #region Müşteri Hesabına Borç Yazılıyor
                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = tcVkn;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = musteriHesap.CariHesapKodu;
                                        borcAlacak.CariHesapId = musteriHesap.CariHesapId;
                                        borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                        borcAlacak.PoliceId = itemTah.PoliceId;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 1: borcAlacak.Alacak1 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 2: borcAlacak.Alacak2 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 3: borcAlacak.Alacak3 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 4: borcAlacak.Alacak4 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 5: borcAlacak.Alacak5 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 6: borcAlacak.Alacak6 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 7: borcAlacak.Alacak7 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 8: borcAlacak.Alacak8 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 9: borcAlacak.Alacak9 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 10: borcAlacak.Alacak10 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 11: borcAlacak.Alacak11 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 12: borcAlacak.Alacak12 = itemTah.TaksitTutari * tutarCarpan; break;
                                            default:
                                                break;
                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion

                                        #region Sigorta Şirketi Hesabına Boç Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "B";
                                        carHareket.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                        carHareket.CariHesapKodu = sirketHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo + "-" + itemTah.TaksitNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.PrimIadesi;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = itemTah.TaksitTutari * tutarCarpan;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }
                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketHesapKodu;
                                        borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                        borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                        borcAlacak.PoliceId = itemTah.PoliceId;

                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 1: borcAlacak.Borc1 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 2: borcAlacak.Borc2 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 3: borcAlacak.Borc3 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 4: borcAlacak.Borc4 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 5: borcAlacak.Borc5 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 6: borcAlacak.Borc6 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 7: borcAlacak.Borc7 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 8: borcAlacak.Borc8 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 9: borcAlacak.Borc9 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 10: borcAlacak.Borc10 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 11: borcAlacak.Borc11 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 12: borcAlacak.Borc12 = itemTah.TaksitTutari * tutarCarpan; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);

                                        #endregion



                                    }
                                    #endregion

                                }
                                #region Komisyon Tahakkuk

                                if (item.Komisyon.HasValue)
                                {
                                    if (item.Komisyon.Value > 0)
                                    {
                                        #region Sigorta Şirketi Hesabına Borc Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "B";
                                        carHareket.CariHareketTarihi = item.TanzimTarihi;
                                        carHareket.CariHesapKodu = sirketHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonTahakkuk;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = item.Komisyon.Value;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }
                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketHesapKodu;
                                        borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                        borcAlacak.Donem = item.TanzimTarihi.Value.Year;
                                        borcAlacak.PoliceId = item.PoliceId;
                                        switch (item.TanzimTarihi.Value.Month)
                                        {
                                            case 1: borcAlacak.Borc1 = item.Komisyon; break;
                                            case 2: borcAlacak.Borc2 = item.Komisyon; break;
                                            case 3: borcAlacak.Borc3 = item.Komisyon; break;
                                            case 4: borcAlacak.Borc4 = item.Komisyon; break;
                                            case 5: borcAlacak.Borc5 = item.Komisyon; break;
                                            case 6: borcAlacak.Borc6 = item.Komisyon; break;
                                            case 7: borcAlacak.Borc7 = item.Komisyon; break;
                                            case 8: borcAlacak.Borc8 = item.Komisyon; break;
                                            case 9: borcAlacak.Borc9 = item.Komisyon; break;
                                            case 10: borcAlacak.Borc10 = item.Komisyon; break;
                                            case 11: borcAlacak.Borc11 = item.Komisyon; break;
                                            case 12: borcAlacak.Borc12 = item.Komisyon; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion

                                        #region Sigorta Şirketi Komisyon Gelir Hesabına Alacak Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "A";
                                        carHareket.CariHareketTarihi = item.TanzimTarihi;
                                        carHareket.CariHesapKodu = sirketKomGelirHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonTahakkuk;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = item.Komisyon.Value;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }
                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketKomGelirHesapKodu;
                                        borcAlacak.CariHesapId = sirketKomGelirHesap.CariHesapId;

                                        borcAlacak.Donem = item.TanzimTarihi.Value.Year;
                                        borcAlacak.PoliceId = item.PoliceId;
                                        switch (item.TanzimTarihi.Value.Month)
                                        {
                                            case 1: borcAlacak.Alacak1 = item.Komisyon; break;
                                            case 2: borcAlacak.Alacak2 = item.Komisyon; break;
                                            case 3: borcAlacak.Alacak3 = item.Komisyon; break;
                                            case 4: borcAlacak.Alacak4 = item.Komisyon; break;
                                            case 5: borcAlacak.Alacak5 = item.Komisyon; break;
                                            case 6: borcAlacak.Alacak6 = item.Komisyon; break;
                                            case 7: borcAlacak.Alacak7 = item.Komisyon; break;
                                            case 8: borcAlacak.Alacak8 = item.Komisyon; break;
                                            case 9: borcAlacak.Alacak9 = item.Komisyon; break;
                                            case 10: borcAlacak.Alacak10 = item.Komisyon; break;
                                            case 11: borcAlacak.Alacak11 = item.Komisyon; break;
                                            case 12: borcAlacak.Alacak12 = item.Komisyon; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion
                                    }
                                }

                                #endregion

                                #region Komisyon İptal
                                int carpan = -1;
                                if (item.Komisyon.HasValue)
                                {
                                    if (item.Komisyon.Value < 0)
                                    {
                                        #region Sigorta Şirketi Hesabına Alacak Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "A";
                                        carHareket.CariHareketTarihi = item.TanzimTarihi;
                                        carHareket.CariHesapKodu = sirketHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonIadesi;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = item.Komisyon.Value * carpan;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }
                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketHesapKodu;
                                        borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                        borcAlacak.Donem = item.TanzimTarihi.Value.Year;
                                        borcAlacak.PoliceId = item.PoliceId;
                                        switch (item.TanzimTarihi.Value.Month)
                                        {
                                            case 1: borcAlacak.Alacak1 = item.Komisyon * carpan; break;
                                            case 2: borcAlacak.Alacak2 = item.Komisyon * carpan; break;
                                            case 3: borcAlacak.Alacak3 = item.Komisyon * carpan; break;
                                            case 4: borcAlacak.Alacak4 = item.Komisyon * carpan; break;
                                            case 5: borcAlacak.Alacak5 = item.Komisyon * carpan; break;
                                            case 6: borcAlacak.Alacak6 = item.Komisyon * carpan; break;
                                            case 7: borcAlacak.Alacak7 = item.Komisyon * carpan; break;
                                            case 8: borcAlacak.Alacak8 = item.Komisyon * carpan; break;
                                            case 9: borcAlacak.Alacak9 = item.Komisyon * carpan; break;
                                            case 10: borcAlacak.Alacak10 = item.Komisyon * carpan; break;
                                            case 11: borcAlacak.Alacak11 = item.Komisyon * carpan; break;
                                            case 12: borcAlacak.Alacak12 = item.Komisyon * carpan; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion

                                        #region Sigorta Şirketi Komisyon Gider Hesabına Alacak Yazılıyor
                                        carHareket = new CariHareketleri();
                                        carHareket.Aciklama = "Neosinerji-Otomatik Yaratıldı.";
                                        carHareket.BorcAlacakTipi = "B";
                                        carHareket.CariHareketTarihi = item.TanzimTarihi;
                                        carHareket.CariHesapKodu = sirketKomGiderHesapKodu;
                                        carHareket.DovizKuru = item.DovizKur;
                                        carHareket.DovizTipi = item.ParaBirimi;
                                        carHareket.EvrakNo = item.PoliceNumarasi + "-" + item.YenilemeNo + "-" + item.EkNo;
                                        carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonIadesi;
                                        carHareket.KayitTarihi = TurkeyDateTime.Now;
                                        carHareket.Tutar = item.Komisyon.Value * carpan;
                                        carHareket.TVMKodu = item.TVMKodu.HasValue ? item.TVMKodu.Value : 0;
                                        if (carHareket.DovizTipi != "TL")
                                        {
                                            if (carHareket.DovizKuru.HasValue)
                                            {
                                                carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                            }
                                        }
                                        model.carHareketList.Add(carHareket);

                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmkodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketKomGiderHesapKodu;
                                        borcAlacak.CariHesapId = sirketKomGiderHesap.CariHesapId;
                                        borcAlacak.Donem = item.TanzimTarihi.Value.Year;
                                        borcAlacak.PoliceId = item.PoliceId;
                                        switch (item.TanzimTarihi.Value.Month)
                                        {
                                            case 1: borcAlacak.Borc1 = item.Komisyon * carpan; break;
                                            case 2: borcAlacak.Borc2 = item.Komisyon * carpan; break;
                                            case 3: borcAlacak.Borc3 = item.Komisyon * carpan; break;
                                            case 4: borcAlacak.Borc4 = item.Komisyon * carpan; break;
                                            case 5: borcAlacak.Borc5 = item.Komisyon * carpan; break;
                                            case 6: borcAlacak.Borc6 = item.Komisyon * carpan; break;
                                            case 7: borcAlacak.Borc7 = item.Komisyon * carpan; break;
                                            case 8: borcAlacak.Borc8 = item.Komisyon * carpan; break;
                                            case 9: borcAlacak.Borc9 = item.Komisyon * carpan; break;
                                            case 10: borcAlacak.Borc10 = item.Komisyon * carpan; break;
                                            case 11: borcAlacak.Borc11 = item.Komisyon * carpan; break;
                                            case 12: borcAlacak.Borc12 = item.Komisyon * carpan; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);
                                        #endregion
                                    }
                                }
                                #endregion
                            }

                            model.ekNo = item.EkNo;
                            model.yenilemeNo = item.YenilemeNo;
                            model.polNo = item.PoliceNumarasi;
                            model.sigortaSirketKodu = item.TUMBirlikKodu;
                            carHareketList.Add(model);
                        }
                        else
                        {
                            CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                            rModel.Basarili = false;
                            rModel.CariHesapKodu = musteriHesapKodu;
                            rModel.Donem = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.Year : 0;
                            rModel.PoliceNo = item.PoliceNumarasi;
                            rModel.YenilemeNo = item.YenilemeNo;
                            rModel.EkNo = item.EkNo;
                            rModel.SigortaSirketKodu = item.TUMBirlikKodu;
                            rModel.TvmKodu = item.TVMKodu;
                            rModel.Mesaj = "Müşteri kimlik numarası bulunamadı, poliçe cari hesaba aktarılmadı.";
                            returnList.Add(rModel);
                        }
                    }
                }
            }
            if (carHareketList.Count > 0)
            {
                borcAlacak = new CariHesapBorcAlacakServiceModel();
                borcAlacak.cariHareketList = carHareketList;
                BAList.Add(borcAlacak);
            }
            if (BAList.Count > 0)
            {
                returnList = this.CariHesapBorcAlacakListUpdate(BAList);
            }

            return returnList;
        }

        public List<CariHesapBAReturnModel> CariHesapBorcAlacakListUpdate(List<CariHesapBorcAlacakServiceModel> list)
        {
            List<CariHesapBAReturnModel> modelList = new List<CariHesapBAReturnModel>();
            if (list.Count > 0)
            {
                var cariHareketler = list.Where(w => w.cariHareketList.Count > 0).ToList();
                if (cariHareketler.Count > 0)
                {
                    foreach (var item in cariHareketler)
                    {
                        foreach (var itemLog in item.cariHareketList)
                        {
                            foreach (var itemHar in itemLog.carHareketList)
                            {
                                var otoHareketKayit = this.OtoCariHeraketEkle(itemLog, itemHar);
                                modelList.Add(otoHareketKayit);
                            }
                        }
                    }
                }
                list = list.Where(w => w.cariHareketList.Count == 0).ToList();
                foreach (var item in list)
                {
                    var CariHesapBAResult = this.CariHesapBAUpdate(item);
                    modelList.Add(CariHesapBAResult);
                }
            }
            return modelList;
        }
        public CariHesapBAReturnModel OtoCariHeraketEkle(CariHareketKaydetModel logModel, CariHareketleri item)
        {
            CariHesapBAReturnModel model = new CariHesapBAReturnModel();
            try
            {
                var hareket = _MuhasebeContext.CariHareketleriRepository.Create(item);
                _MuhasebeContext.Commit();
                model.Basarili = true;
                //model = new CariHesapBAReturnModel();
                //model.Basarili = true;
                //model.CariHesapKodu = item.CariHesapKodu;
                //model.Donem = item.CariHareketTarihi.HasValue ? item.CariHareketTarihi.Value.Year : 0;
                //model.Id = hareket.Id;
                //model.Mesaj = "Cari Hareket eklendi.";
                return model;
            }
            catch (Exception ex)
            {
                model = new CariHesapBAReturnModel();
                model.Basarili = false;
                model.CariHesapKodu = item.CariHesapKodu;
                model.Donem = item.CariHareketTarihi.HasValue ? item.CariHareketTarihi.Value.Year : 0;
                model.PoliceNo = logModel.polNo;
                model.YenilemeNo = logModel.yenilemeNo;
                model.EkNo = logModel.ekNo;
                model.SigortaSirketKodu = logModel.sigortaSirketKodu;
                model.TvmKodu = item.TVMKodu;
                model.Mesaj = "Cari Hareket Ekleme Hata:" + ex.ToString();
                return model;
            }

        }
        public CariHesapBAReturnModel CariHesapBAUpdate(CariHesapBorcAlacakServiceModel item)
        {
            CariHesapBAReturnModel model = new CariHesapBAReturnModel();
            var CariHesap = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(w => w.Donem == item.Donem && w.CariHesapKodu == item.CariHesapKodu && w.TVMKodu == item.TVMKodu).FirstOrDefault();
            if (CariHesap != null)
            {
                var police = _PoliceContext.PoliceGenelRepository.All().Where(w => w.PoliceId == item.PoliceId).FirstOrDefault();
                try
                {
                    #region Alacaklar

                    if (item.Alacak1.HasValue)
                    {
                        CariHesap.Alacak1 += item.Alacak1.Value;
                    }
                    else if (item.Alacak2.HasValue)
                    {
                        CariHesap.Alacak2 += item.Alacak2.Value;
                    }
                    else if (item.Alacak3.HasValue)
                    {
                        CariHesap.Alacak3 += item.Alacak3.Value;
                    }
                    else if (item.Alacak4.HasValue)
                    {
                        CariHesap.Alacak4 += item.Alacak4.Value;
                    }
                    if (item.Alacak5.HasValue)
                    {
                        CariHesap.Alacak5 += item.Alacak5.Value;
                    }
                    else if (item.Alacak6.HasValue)
                    {
                        CariHesap.Alacak6 += item.Alacak6.Value;
                    }
                    else if (item.Alacak7.HasValue)
                    {
                        CariHesap.Alacak7 += item.Alacak7.Value;
                    }
                    else if (item.Alacak8.HasValue)
                    {
                        CariHesap.Alacak8 += item.Alacak8.Value;
                    }
                    else if (item.Alacak9.HasValue)
                    {
                        CariHesap.Alacak9 += item.Alacak9.Value;
                    }
                    else if (item.Alacak10.HasValue)
                    {
                        CariHesap.Alacak10 += item.Alacak10.Value;
                    }
                    else if (item.Alacak11.HasValue)
                    {
                        CariHesap.Alacak11 += item.Alacak11.Value;
                    }
                    else if (item.Alacak12.HasValue)
                    {
                        CariHesap.Alacak12 += item.Alacak12.Value;
                    }

                    #endregion

                    #region Borçlar

                    if (item.Borc1.HasValue)
                    {
                        CariHesap.Borc1 += item.Borc1.Value;
                    }
                    else if (item.Borc2.HasValue)
                    {
                        CariHesap.Borc2 += item.Borc2.Value;
                    }
                    else if (item.Borc3.HasValue)
                    {
                        CariHesap.Borc3 += item.Borc3.Value;
                    }
                    else if (item.Borc4.HasValue)
                    {
                        CariHesap.Borc4 += item.Borc4.Value;
                    }
                    if (item.Borc5.HasValue)
                    {
                        CariHesap.Borc5 += item.Borc5.Value;
                    }
                    else if (item.Borc6.HasValue)
                    {
                        CariHesap.Borc6 += item.Borc6.Value;
                    }
                    else if (item.Borc7.HasValue)
                    {
                        CariHesap.Borc7 += item.Borc7.Value;
                    }
                    else if (item.Borc8.HasValue)
                    {
                        CariHesap.Borc8 += item.Borc8.Value;
                    }
                    else if (item.Borc9.HasValue)
                    {
                        CariHesap.Borc9 += item.Borc9.Value;
                    }
                    else if (item.Borc10.HasValue)
                    {
                        CariHesap.Borc10 += item.Borc10.Value;
                    }
                    else if (item.Borc11.HasValue)
                    {
                        CariHesap.Borc11 += item.Borc11.Value;
                    }
                    else if (item.Borc12.HasValue)
                    {
                        CariHesap.Borc12 += item.Borc12.Value;
                    }

                    #endregion

                    _MuhasebeContext.CariHesapBorcAlacakRepository.Update(CariHesap);
                    _MuhasebeContext.Commit();


                    police.CariHareketKayitTarihi = TurkeyDateTime.Now;
                    _PoliceContext.PoliceGenelRepository.Update(police);
                    _PoliceContext.Commit();

                    //model.Basarili = true;
                    //model.CariHesapKodu = item.CariHesapKodu;
                    //model.Donem = item.Donem;
                    //model.Id = CariHesap.Id;
                    //model.Mesaj = "Kayıt güncellendi.";
                    model.Basarili = true;
                    return model;
                }
                catch (Exception ex)
                {
                    model.Basarili = false;
                    model.CariHesapKodu = item.CariHesapKodu;
                    model.Donem = item.Donem;
                    model.PoliceNo = police != null ? police.PoliceNumarasi : null;
                    model.YenilemeNo = police != null ? police.YenilemeNo : null;
                    model.EkNo = police != null ? police.EkNo : null;
                    model.SigortaSirketKodu = police != null ? police.TUMBirlikKodu : null;
                    model.TvmKodu = police != null ? police.TVMKodu : _AktifKullanici.TVMKodu;
                    if (ex.Message.Length > 450)
                    {
                        model.Mesaj = "Cari Hesap Borç Alacak Güncelleme Hata:" + ex.Message.ToString().Substring(0, 450);
                    }
                    else
                    {
                        model.Mesaj = "Cari Hesap Borç Alacak Güncelleme Hata:" + ex.Message.ToString();
                    }
                    return model;
                }
            }
            else
            {
                var police = _PoliceContext.PoliceGenelRepository.All().Where(w => w.PoliceId == item.PoliceId).FirstOrDefault();
                try
                {
                    CariHesap = new CariHesapBorcAlacak();
                    CariHesap.Donem = item.Donem;
                    CariHesap.KayitTarihi = TurkeyDateTime.Now;
                    CariHesap.CariHesapKodu = item.CariHesapKodu;
                    CariHesap.CariHesapId = item.CariHesapId;
                    CariHesap.KimlikNo = item.KimlikNo;
                    CariHesap.TVMKodu = item.TVMKodu;

                    #region Alacaklar

                    CariHesap.Alacak1 = item.Alacak1.HasValue ? item.Alacak1 : 0;
                    CariHesap.Alacak2 = item.Alacak2.HasValue ? item.Alacak2 : 0;
                    CariHesap.Alacak3 = item.Alacak3.HasValue ? item.Alacak3 : 0;
                    CariHesap.Alacak4 = item.Alacak4.HasValue ? item.Alacak4 : 0;
                    CariHesap.Alacak5 = item.Alacak5.HasValue ? item.Alacak5 : 0;
                    CariHesap.Alacak6 = item.Alacak6.HasValue ? item.Alacak6 : 0;
                    CariHesap.Alacak7 = item.Alacak7.HasValue ? item.Alacak7 : 0;
                    CariHesap.Alacak8 = item.Alacak8.HasValue ? item.Alacak8 : 0;
                    CariHesap.Alacak9 = item.Alacak9.HasValue ? item.Alacak9 : 0;
                    CariHesap.Alacak10 = item.Alacak10.HasValue ? item.Alacak10 : 0;
                    CariHesap.Alacak11 = item.Alacak11.HasValue ? item.Alacak11 : 0;
                    CariHesap.Alacak12 = item.Alacak12.HasValue ? item.Alacak12 : 0;

                    #endregion

                    #region Borçlar
                    CariHesap.Borc1 = item.Borc1.HasValue ? item.Borc1 : 0;
                    CariHesap.Borc2 = item.Borc2.HasValue ? item.Borc2 : 0;
                    CariHesap.Borc3 = item.Borc3.HasValue ? item.Borc3 : 0;
                    CariHesap.Borc4 = item.Borc4.HasValue ? item.Borc4 : 0;
                    CariHesap.Borc5 = item.Borc5.HasValue ? item.Borc5 : 0;
                    CariHesap.Borc6 = item.Borc6.HasValue ? item.Borc6 : 0;
                    CariHesap.Borc7 = item.Borc7.HasValue ? item.Borc7 : 0;
                    CariHesap.Borc8 = item.Borc8.HasValue ? item.Borc8 : 0;
                    CariHesap.Borc9 = item.Borc9.HasValue ? item.Borc9 : 0;
                    CariHesap.Borc10 = item.Borc10.HasValue ? item.Borc10 : 0;
                    CariHesap.Borc11 = item.Borc11.HasValue ? item.Borc11 : 0;
                    CariHesap.Borc12 = item.Borc12.HasValue ? item.Borc12 : 0;

                    #endregion

                    var cariBA = _MuhasebeContext.CariHesapBorcAlacakRepository.Create(CariHesap);
                    _MuhasebeContext.Commit();

                    police.CariHareketKayitTarihi = TurkeyDateTime.Now;
                    _PoliceContext.PoliceGenelRepository.Update(police);
                    _PoliceContext.Commit();
                    model.Basarili = true;
                    //model.Basarili = true;
                    //model.CariHesapKodu = item.CariHesapKodu;
                    //model.Donem = item.Donem;
                    //model.Id = cariBA.Id;
                    //model.Mesaj = "Kayıt eklendi.";
                    return model;
                }
                catch (Exception ex)
                {
                    model.Basarili = false;
                    model.CariHesapKodu = item.CariHesapKodu;
                    model.Donem = item.Donem;
                    model.PoliceNo = police != null ? police.PoliceNumarasi : null;
                    model.YenilemeNo = police != null ? police.YenilemeNo : null;
                    model.EkNo = police != null ? police.EkNo : null;
                    model.SigortaSirketKodu = police != null ? police.TUMBirlikKodu : null;
                    model.TvmKodu = police != null ? police.TVMKodu : _AktifKullanici.TVMKodu;
                    if (ex.Message.Length > 450)
                    {
                        model.Mesaj = "Cari Hesap Borç Alacak Ekleme Hata:" + ex.Message.ToString().Substring(0, 450);
                    }
                    else
                    {
                        model.Mesaj = "Cari Hesap Borç Alacak Ekleme Hata:" + ex.Message.ToString();
                    }
                    return model;
                }
            }

        }

        public void CariAktarimLogAdd(List<CariHesapBAReturnModel> list)
        {
            CariAktarimLog logItem = new CariAktarimLog();
            try
            {
                foreach (var item in list)
                {
                    if (!item.Basarili)
                    {
                        logItem.Basarili = 0;
                    }
                    logItem.CariHesapKodu = item.CariHesapKodu;
                    logItem.CariHesapUnvani = item.CariHesapAdi;
                    logItem.Donem = item.Donem;
                    logItem.EkNo = item.EkNo.HasValue ? item.EkNo.ToString() : "";
                    logItem.Mesaj = item.Mesaj;
                    logItem.TvmKodu = item.TvmKodu;
                    logItem.TvmUnvan = item.TvmUnvan;
                    logItem.YenilemeNo = item.YenilemeNo.HasValue ? item.YenilemeNo.ToString() : "";
                    logItem.KayitTarihi = TurkeyDateTime.Now;
                    _MuhasebeContext.CariAktarimLogRepository.Create(logItem);
                    _MuhasebeContext.Commit();
                }
            }
            catch (Exception)
            {

            }
        }

        public class CariHesapBAReturnModel
        {
            public bool Basarili { get; set; }
            public string Mesaj { get; set; }
            public string PoliceNo { get; set; }
            public string SigortaSirketKodu { get; set; }
            public string SigortaSirketUnvan { get; set; }
            public int? YenilemeNo { get; set; }
            public int? EkNo { get; set; }
            public string CariHesapKodu { get; set; }
            public string CariHesapAdi { get; set; }
            public int Donem { get; set; }
            public int? TvmKodu { get; set; }
            public string TvmUnvan { get; set; }

        }

        public class CariHareketKaydetModel
        {
            public List<CariHareketleri> carHareketList = new List<CariHareketleri>();
            public string polNo { get; set; }
            public int? yenilemeNo { get; set; }
            public int? ekNo { get; set; }
            public string sigortaSirketKodu { get; set; }
        }

        public CariHesaplari getCariHesapBilgi(int tvmKodu, string cariHesapKodu)
        {
            CariHesaplari cariHesaplari = new CariHesaplari();
            cariHesaplari = _MuhasebeContext.CariHesaplariRepository.All().Where(w => w.TVMKodu == tvmKodu && w.CariHesapKodu == cariHesapKodu).FirstOrDefault();

            return cariHesaplari;
        }

        #endregion

        #region Toplu Poliçe tahsilat kapatma

        public List<PoliceGenel> GetTopluPoliceTahsilatList(DateTime BaslangicTarihi, DateTime BitisTarihi, int tvmKodu, List<string> tumKoduList, List<int> tvmlist)
        {
            var getList = _PoliceContext.PoliceGenelRepository.All().Where(s => tvmlist.Contains(s.TVMKodu.Value) && tumKoduList.Contains(s.TUMBirlikKodu) && s.TanzimTarihi <= BaslangicTarihi && s.TanzimTarihi <= s.TanzimTarihi).ToList();

            return getList;
        }

        #endregion

        #region Poliçeleri CH Otomatik Aktar
        public List<CariHesapBAReturnModel> CariAktarimIslemleri(int tvmKodu)
        {
            List<CariHesapBAReturnModel> returnList = new List<CariHesapBAReturnModel>();
            DateTime tanzimBaslangicTarihi = new DateTime(2017, 05, 01);
            DateTime tanzimBitisTarihi = new DateTime(2017, 05, 30);
            var aktarilanPolice = _PoliceContext.PoliceGenelRepository.All().Where(w => w.TVMKodu == tvmKodu && w.CariHareketKayitTarihi == null &&
                                                                              w.TanzimTarihi >= tanzimBaslangicTarihi &&
                                                                              w.TanzimTarihi <= tanzimBitisTarihi &&
                                                                              w.TUMBirlikKodu == "047" &&
                                                                              (w.BrutPrim != 0 || (w.Komisyon != null && w.Komisyon != 0))
                                                                              ).OrderByDescending(s => s.PoliceId).FirstOrDefault();

            if (aktarilanPolice == null)
            {
                string bitti = "";
            }
            List<CariHesapBorcAlacakServiceModel> BAList = new List<CariHesapBorcAlacakServiceModel>();
            CariHesapBorcAlacakServiceModel borcAlacak = new CariHesapBorcAlacakServiceModel();
            CariHareketleri carHareket = new CariHareketleri();
            CariHareketKaydetModel model = new CariHareketKaydetModel();
            List<CariHareketKaydetModel> carHareketList = new List<CariHareketKaydetModel>();

            if (aktarilanPolice != null)
            {

                if (aktarilanPolice.PoliceTahsilats.Count > 0 || (aktarilanPolice.Komisyon.HasValue && aktarilanPolice.Komisyon.Value != 0))
                {
                    model = new CariHareketKaydetModel();

                    string tcVkn = String.Empty;
                    bool musteriHesapVarMi = false;
                    bool sirketHesapVarMi = false;
                    bool sirketKomGelirHesapVarMi = false;
                    bool sirketKomGiderHesapVarMi = false;
                    string sirketHesapKodu = String.Empty;
                    string sirketVkn = String.Empty;
                    string sirketKomGelirHesapKodu = String.Empty;
                    string sirketKomGiderHesapKodu = String.Empty;
                    string musteriHesapKodu = String.Empty;
                    if (!String.IsNullOrEmpty(aktarilanPolice.PoliceSigortaEttiren.KimlikNo))
                    {
                        if (aktarilanPolice.PoliceSigortaEttiren.KimlikNo.Length == 11)
                        {
                            tcVkn = aktarilanPolice.PoliceSigortaEttiren.KimlikNo;
                        }
                    }
                    if (!String.IsNullOrEmpty(aktarilanPolice.PoliceSigortaEttiren.VergiKimlikNo))
                    {
                        if (aktarilanPolice.PoliceSigortaEttiren.VergiKimlikNo.Length == 10)
                        {
                            tcVkn = aktarilanPolice.PoliceSigortaEttiren.VergiKimlikNo.Trim();
                        }
                    }
                    if (!String.IsNullOrEmpty(tcVkn))
                    {

                        CariHesaplari musteriHesap = new CariHesaplari();
                        musteriHesapKodu = "120.01." + tcVkn;
                        musteriHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, musteriHesapKodu);
                        if (musteriHesap != null)
                        {
                            musteriHesapVarMi = true;
                        }
                        CariHesaplari sirketHesap = new CariHesaplari();
                        CariHesaplari sirketKomGelirHesap = new CariHesaplari();
                        CariHesaplari sirketKomGiderHesap = new CariHesaplari();

                        var getSirket = _SigortaSirketleriService.GetSirket(aktarilanPolice.TUMBirlikKodu);
                        if (getSirket != null)
                        {
                            sirketHesapKodu = "320.01." + getSirket.VergiNumarasi;
                            sirketHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketHesapKodu);
                            if (sirketHesap != null)
                            {
                                sirketHesapVarMi = true;
                            }
                            sirketKomGelirHesapKodu = "600.01." + getSirket.VergiNumarasi;
                            sirketKomGelirHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketKomGelirHesapKodu);
                            if (sirketKomGelirHesap != null)
                            {
                                sirketKomGelirHesapVarMi = true;
                            }
                            sirketKomGiderHesapKodu = "610.01." + getSirket.VergiNumarasi;
                            sirketKomGiderHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketKomGiderHesapKodu);
                            if (sirketKomGiderHesap != null)
                            {
                                sirketKomGiderHesapVarMi = true;
                            }
                        }

                        if (!musteriHesapVarMi)
                        {
                            try
                            {
                                CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                CariHesaplari cariHesapEkle = new CariHesaplari();
                                cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                cariHesapEkle.UlkeKodu = aktarilanPolice.PoliceSigortaEttiren.UlkeKodu;
                                cariHesapEkle.IlKodu = aktarilanPolice.PoliceSigortaEttiren.IlKodu;
                                cariHesapEkle.IlceKodu = aktarilanPolice.PoliceSigortaEttiren.IlceKodu.HasValue ? aktarilanPolice.PoliceSigortaEttiren.IlceKodu.Value : 0;
                                cariHesapEkle.KayitTarihi = DateTime.Now;
                                cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                cariHesapEkle.Adres = aktarilanPolice.PoliceSigortaEttiren.Adres;
                                cariHesapEkle.Unvan = aktarilanPolice.PoliceSigortaEttiren.AdiUnvan + aktarilanPolice.PoliceSigortaEttiren.SoyadiUnvan;
                                cariHesapEkle.Telefon1 = aktarilanPolice.PoliceSigortaEttiren.TelefonNo;
                                cariHesapEkle.CepTel = aktarilanPolice.PoliceSigortaEttiren.MobilTelefonNo;
                                cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                                cariHesapEkle.Email = aktarilanPolice.PoliceSigortaEttiren.EMail;
                                cariHesapEkle.PostaKodu = aktarilanPolice.PoliceSigortaEttiren.PostaKodu.HasValue ? aktarilanPolice.PoliceSigortaEttiren.PostaKodu.Value : 0;
                                cariHesapEkle.KimlikNo = tcVkn;
                                cariHesapEkle.CariHesapTipi = "120.01.";
                                cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + tcVkn;
                                var musteriDetay = _MusteriService.GetMusteri(tcVkn, tvmKodu);
                                if (musteriDetay != null)
                                {
                                    if (!String.IsNullOrEmpty(musteriDetay.TVMMusteriKodu))
                                    {
                                        cariHesapEkle.MusteriGrupKodu = musteriDetay.TVMMusteriKodu;
                                    }
                                }

                                musteriHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                _MuhasebeContext.Commit();
                                if (musteriHesap.CariHesapKodu != null)
                                {
                                    cariHesapBorcAlacakEkle.CariHesapKodu = musteriHesapKodu;
                                    cariHesapBorcAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                                    cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                    cariHesapBorcAlacakEkle.KimlikNo = tcVkn;
                                    cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                    cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                    cariHesapBorcAlacakEkle.Alacak1 = 0;
                                    cariHesapBorcAlacakEkle.Borc1 = 0;
                                    cariHesapBorcAlacakEkle.Alacak2 = 0;
                                    cariHesapBorcAlacakEkle.Borc2 = 0;
                                    cariHesapBorcAlacakEkle.Alacak3 = 0;
                                    cariHesapBorcAlacakEkle.Borc3 = 0;
                                    cariHesapBorcAlacakEkle.Alacak4 = 0;
                                    cariHesapBorcAlacakEkle.Borc4 = 0;
                                    cariHesapBorcAlacakEkle.Alacak5 = 0;
                                    cariHesapBorcAlacakEkle.Borc5 = 0;
                                    cariHesapBorcAlacakEkle.Alacak6 = 0;
                                    cariHesapBorcAlacakEkle.Borc6 = 0;
                                    cariHesapBorcAlacakEkle.Alacak7 = 0;
                                    cariHesapBorcAlacakEkle.Borc7 = 0;
                                    cariHesapBorcAlacakEkle.Alacak8 = 0;
                                    cariHesapBorcAlacakEkle.Borc8 = 0;
                                    cariHesapBorcAlacakEkle.Alacak9 = 0;
                                    cariHesapBorcAlacakEkle.Borc9 = 0;
                                    cariHesapBorcAlacakEkle.Alacak10 = 0;
                                    cariHesapBorcAlacakEkle.Borc10 = 0;
                                    cariHesapBorcAlacakEkle.Alacak11 = 0;
                                    cariHesapBorcAlacakEkle.Borc11 = 0;
                                    cariHesapBorcAlacakEkle.Alacak12 = 0;
                                    cariHesapBorcAlacakEkle.Borc12 = 0;
                                    var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                    if (hesapEkleReturn == 0)
                                    {
                                        CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                        rModel.Basarili = false;
                                        rModel.CariHesapKodu = musteriHesapKodu;
                                        rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                        rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                        rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                        rModel.EkNo = aktarilanPolice.EkNo;
                                        rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                        rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                        rModel.Mesaj = "Yeni açılan müşteri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                        returnList.Add(rModel);
                                    }
                                }
                                musteriHesapVarMi = true;
                            }
                            catch (Exception ex)
                            {
                                CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                rModel.Basarili = false;
                                rModel.CariHesapKodu = musteriHesapKodu;
                                rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                rModel.EkNo = aktarilanPolice.EkNo;
                                rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                rModel.Mesaj = "Müşteri Hesap Ekleme Hata:" + ex.ToString();
                                returnList.Add(rModel);
                            }
                        }
                        if (!sirketHesapVarMi)
                        {
                            try
                            {
                                CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                CariHesaplari cariHesapEkle = new CariHesaplari();
                                cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                cariHesapEkle.KayitTarihi = DateTime.Now;
                                cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                cariHesapEkle.Unvan = getSirket.SirketAdi;
                                cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";

                                if (getSirket != null)
                                {
                                    cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                }
                                getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                cariHesapEkle.CariHesapTipi = "320.01.";
                                cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;
                                cariHesapEkle.KomisyonGelirleriMuhasebeKodu = "600.01." + getSirket.VergiNumarasi;
                                cariHesapEkle.SatisIadeleriMuhasebeKodu = "610.01." + getSirket.VergiNumarasi;
                                sirketHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                _MuhasebeContext.Commit();
                                if (sirketHesap.CariHesapKodu != null)
                                {
                                    cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                    cariHesapBorcAlacakEkle.CariHesapId = sirketHesap.CariHesapId;
                                    cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                    cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                    cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                    cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                    cariHesapBorcAlacakEkle.Alacak1 = 0;
                                    cariHesapBorcAlacakEkle.Borc1 = 0;
                                    cariHesapBorcAlacakEkle.Alacak2 = 0;
                                    cariHesapBorcAlacakEkle.Borc2 = 0;
                                    cariHesapBorcAlacakEkle.Alacak3 = 0;
                                    cariHesapBorcAlacakEkle.Borc3 = 0;
                                    cariHesapBorcAlacakEkle.Alacak4 = 0;
                                    cariHesapBorcAlacakEkle.Borc4 = 0;
                                    cariHesapBorcAlacakEkle.Alacak5 = 0;
                                    cariHesapBorcAlacakEkle.Borc5 = 0;
                                    cariHesapBorcAlacakEkle.Alacak6 = 0;
                                    cariHesapBorcAlacakEkle.Borc6 = 0;
                                    cariHesapBorcAlacakEkle.Alacak7 = 0;
                                    cariHesapBorcAlacakEkle.Borc7 = 0;
                                    cariHesapBorcAlacakEkle.Alacak8 = 0;
                                    cariHesapBorcAlacakEkle.Borc8 = 0;
                                    cariHesapBorcAlacakEkle.Alacak9 = 0;
                                    cariHesapBorcAlacakEkle.Borc9 = 0;
                                    cariHesapBorcAlacakEkle.Alacak10 = 0;
                                    cariHesapBorcAlacakEkle.Borc10 = 0;
                                    cariHesapBorcAlacakEkle.Alacak11 = 0;
                                    cariHesapBorcAlacakEkle.Borc11 = 0;
                                    cariHesapBorcAlacakEkle.Alacak12 = 0;
                                    cariHesapBorcAlacakEkle.Borc12 = 0;
                                    var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                    if (hesapEkleReturn == 0)
                                    {
                                        CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                        rModel.Basarili = false;
                                        rModel.CariHesapKodu = musteriHesapKodu;
                                        rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                        rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                        rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                        rModel.EkNo = aktarilanPolice.EkNo;
                                        rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                        rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                        rModel.Mesaj = "Yeni açılan sigorta şirketi hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                        returnList.Add(rModel);
                                    }
                                }
                                sirketHesapVarMi = true;
                            }
                            catch (Exception ex)
                            {
                                CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                rModel.Basarili = false;
                                rModel.CariHesapKodu = sirketHesapKodu;
                                rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                rModel.EkNo = aktarilanPolice.EkNo;
                                rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                rModel.Mesaj = "Sigorta Şirketi Hesap Ekleme Hata:" + ex.ToString();
                                returnList.Add(rModel);
                            }
                        }
                        if (!sirketKomGelirHesapVarMi)
                        {
                            try
                            {
                                CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                CariHesaplari cariHesapEkle = new CariHesaplari();
                                cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                cariHesapEkle.KayitTarihi = DateTime.Now;
                                cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                cariHesapEkle.Unvan = getSirket.SirketAdi;
                                cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";
                                if (getSirket != null)
                                {
                                    cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                }
                                getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                cariHesapEkle.CariHesapTipi = "600.01.";
                                cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;

                                sirketKomGelirHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                _MuhasebeContext.Commit();
                                if (sirketKomGelirHesap.CariHesapKodu != null)
                                {
                                    cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                    cariHesapBorcAlacakEkle.CariHesapId = sirketKomGelirHesap.CariHesapId;
                                    cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                    cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                    cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                    cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                    cariHesapBorcAlacakEkle.Alacak1 = 0;
                                    cariHesapBorcAlacakEkle.Borc1 = 0;
                                    cariHesapBorcAlacakEkle.Alacak2 = 0;
                                    cariHesapBorcAlacakEkle.Borc2 = 0;
                                    cariHesapBorcAlacakEkle.Alacak3 = 0;
                                    cariHesapBorcAlacakEkle.Borc3 = 0;
                                    cariHesapBorcAlacakEkle.Alacak4 = 0;
                                    cariHesapBorcAlacakEkle.Borc4 = 0;
                                    cariHesapBorcAlacakEkle.Alacak5 = 0;
                                    cariHesapBorcAlacakEkle.Borc5 = 0;
                                    cariHesapBorcAlacakEkle.Alacak6 = 0;
                                    cariHesapBorcAlacakEkle.Borc6 = 0;
                                    cariHesapBorcAlacakEkle.Alacak7 = 0;
                                    cariHesapBorcAlacakEkle.Borc7 = 0;
                                    cariHesapBorcAlacakEkle.Alacak8 = 0;
                                    cariHesapBorcAlacakEkle.Borc8 = 0;
                                    cariHesapBorcAlacakEkle.Alacak9 = 0;
                                    cariHesapBorcAlacakEkle.Borc9 = 0;
                                    cariHesapBorcAlacakEkle.Alacak10 = 0;
                                    cariHesapBorcAlacakEkle.Borc10 = 0;
                                    cariHesapBorcAlacakEkle.Alacak11 = 0;
                                    cariHesapBorcAlacakEkle.Borc11 = 0;
                                    cariHesapBorcAlacakEkle.Alacak12 = 0;
                                    cariHesapBorcAlacakEkle.Borc12 = 0;
                                    var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                    if (hesapEkleReturn == 0)
                                    {
                                        CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                        rModel.Basarili = false;
                                        rModel.CariHesapKodu = musteriHesapKodu;
                                        rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                        rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                        rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                        rModel.EkNo = aktarilanPolice.EkNo;
                                        rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                        rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                        rModel.Mesaj = "Yeni açılan sigorta şirketi komisyon gelirleri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                        returnList.Add(rModel);
                                    }
                                }
                                sirketKomGelirHesapVarMi = true;

                            }
                            catch (Exception ex)
                            {
                                CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                rModel.Basarili = false;
                                rModel.CariHesapKodu = sirketKomGelirHesapKodu;
                                rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                rModel.EkNo = aktarilanPolice.EkNo;
                                rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                rModel.Mesaj = "Sigorta Şirketi Komisyon Gelir Hesabı Ekleme Hata:" + ex.ToString();
                                returnList.Add(rModel);
                            }
                        }
                        if (!sirketKomGiderHesapVarMi)
                        {
                            try
                            {
                                CariHesapBorcAlacak cariHesapBorcAlacakEkle = new CariHesapBorcAlacak();
                                CariHesaplari cariHesapEkle = new CariHesaplari();
                                cariHesapEkle.TVMKodu = _AktifKullanici.TVMKodu;
                                cariHesapEkle.TVMUnvani = _AktifKullanici.TVMUnvani;
                                cariHesapEkle.KayitTarihi = DateTime.Now;
                                cariHesapEkle.GuncellemeTarihi = DateTime.Now;
                                cariHesapEkle.Unvan = getSirket.SirketAdi;
                                cariHesapEkle.BilgiNotu = "Neosinerji-Otomatik Yaratildi";

                                if (getSirket != null)
                                {
                                    cariHesapEkle.VergiDairesi = getSirket.VergiDairesi;
                                }
                                getSirket.VergiNumarasi = getSirket.VergiNumarasi.Trim();
                                cariHesapEkle.KimlikNo = getSirket.VergiNumarasi;
                                cariHesapEkle.CariHesapTipi = "610.01.";
                                cariHesapEkle.CariHesapKodu = cariHesapEkle.CariHesapTipi + getSirket.VergiNumarasi;

                                sirketKomGiderHesap = _MuhasebeContext.CariHesaplariRepository.Create(cariHesapEkle);
                                _MuhasebeContext.Commit();
                                if (sirketKomGiderHesap.CariHesapKodu != null)
                                {
                                    cariHesapBorcAlacakEkle.CariHesapKodu = cariHesapEkle.CariHesapKodu;
                                    cariHesapBorcAlacakEkle.CariHesapId = sirketKomGiderHesap.CariHesapId;
                                    cariHesapBorcAlacakEkle.KayitTarihi = TurkeyDateTime.Now;
                                    cariHesapBorcAlacakEkle.KimlikNo = cariHesapEkle.KimlikNo;
                                    cariHesapBorcAlacakEkle.TVMKodu = cariHesapEkle.TVMKodu;
                                    cariHesapBorcAlacakEkle.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                    cariHesapBorcAlacakEkle.Alacak1 = 0;
                                    cariHesapBorcAlacakEkle.Borc1 = 0;
                                    cariHesapBorcAlacakEkle.Alacak2 = 0;
                                    cariHesapBorcAlacakEkle.Borc2 = 0;
                                    cariHesapBorcAlacakEkle.Alacak3 = 0;
                                    cariHesapBorcAlacakEkle.Borc3 = 0;
                                    cariHesapBorcAlacakEkle.Alacak4 = 0;
                                    cariHesapBorcAlacakEkle.Borc4 = 0;
                                    cariHesapBorcAlacakEkle.Alacak5 = 0;
                                    cariHesapBorcAlacakEkle.Borc5 = 0;
                                    cariHesapBorcAlacakEkle.Alacak6 = 0;
                                    cariHesapBorcAlacakEkle.Borc6 = 0;
                                    cariHesapBorcAlacakEkle.Alacak7 = 0;
                                    cariHesapBorcAlacakEkle.Borc7 = 0;
                                    cariHesapBorcAlacakEkle.Alacak8 = 0;
                                    cariHesapBorcAlacakEkle.Borc8 = 0;
                                    cariHesapBorcAlacakEkle.Alacak9 = 0;
                                    cariHesapBorcAlacakEkle.Borc9 = 0;
                                    cariHesapBorcAlacakEkle.Alacak10 = 0;
                                    cariHesapBorcAlacakEkle.Borc10 = 0;
                                    cariHesapBorcAlacakEkle.Alacak11 = 0;
                                    cariHesapBorcAlacakEkle.Borc11 = 0;
                                    cariHesapBorcAlacakEkle.Alacak12 = 0;
                                    cariHesapBorcAlacakEkle.Borc12 = 0;
                                    var hesapEkleReturn = this.CariHesapBorcAlacakKayitEkle(cariHesapBorcAlacakEkle);
                                    if (hesapEkleReturn == 0)
                                    {
                                        CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                        rModel.Basarili = false;
                                        rModel.CariHesapKodu = musteriHesapKodu;
                                        rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                        rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                        rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                        rModel.EkNo = aktarilanPolice.EkNo;
                                        rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                        rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                        rModel.Mesaj = "Yeni açılan sigorta şirketi komisyon iadeleri hesabının cari hesap borç alacak kaydı yaratılırken bir hata oluştu.";
                                        returnList.Add(rModel);
                                    }
                                }
                                sirketKomGiderHesapVarMi = true;
                            }
                            catch (Exception ex)
                            {
                                CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                                rModel.Basarili = false;
                                rModel.CariHesapKodu = sirketKomGiderHesapKodu;
                                rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                                rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                                rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                                rModel.EkNo = aktarilanPolice.EkNo;
                                rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                                rModel.TvmKodu = aktarilanPolice.TVMKodu;
                                rModel.Mesaj = "Sigorta Şirketi Komisyon İade Hesabı Ekleme Hata:" + ex.ToString();
                                returnList.Add(rModel);
                            }
                        }

                        if (musteriHesapVarMi && sirketHesapVarMi && sirketKomGelirHesapVarMi)
                        {
                            foreach (var itemTah in aktarilanPolice.PoliceTahsilats)
                            {
                                #region Tahakkuk

                                if (itemTah.TaksitTutari > 0)
                                {
                                    #region Müşteri Hesabına Borç Yazılıyor
                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = tcVkn;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = musteriHesapKodu;
                                    borcAlacak.CariHesapId = musteriHesap.CariHesapId;
                                    borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                    borcAlacak.PoliceId = itemTah.PoliceId;
                                    switch (itemTah.TaksitVadeTarihi.Month)
                                    {
                                        case 1: borcAlacak.Borc1 = itemTah.TaksitTutari; break;
                                        case 2: borcAlacak.Borc2 = itemTah.TaksitTutari; break;
                                        case 3: borcAlacak.Borc3 = itemTah.TaksitTutari; break;
                                        case 4: borcAlacak.Borc4 = itemTah.TaksitTutari; break;
                                        case 5: borcAlacak.Borc5 = itemTah.TaksitTutari; break;
                                        case 6: borcAlacak.Borc6 = itemTah.TaksitTutari; break;
                                        case 7: borcAlacak.Borc7 = itemTah.TaksitTutari; break;
                                        case 8: borcAlacak.Borc8 = itemTah.TaksitTutari; break;
                                        case 9: borcAlacak.Borc9 = itemTah.TaksitTutari; break;
                                        case 10: borcAlacak.Borc10 = itemTah.TaksitTutari; break;
                                        case 11: borcAlacak.Borc11 = itemTah.TaksitTutari; break;
                                        case 12: borcAlacak.Borc12 = itemTah.TaksitTutari; break;
                                        default:
                                            break;
                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion

                                    #region Sigorta Şirketi Hesabına Alacak Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "A";
                                    carHareket.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                    carHareket.CariHesapKodu = sirketHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo + "-" + itemTah.TaksitNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.PrimTahakkuk;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = itemTah.TaksitTutari;
                                    carHareket.OdemeTipi = itemTah.OdemTipi;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    carHareket.OdemeTipi = itemTah.OdemTipi;
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketHesapKodu;
                                    borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                    borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                    borcAlacak.PoliceId = itemTah.PoliceId;
                                    switch (itemTah.TaksitVadeTarihi.Month)
                                    {
                                        case 1: borcAlacak.Alacak1 = itemTah.TaksitTutari; break;
                                        case 2: borcAlacak.Alacak2 = itemTah.TaksitTutari; break;
                                        case 3: borcAlacak.Alacak3 = itemTah.TaksitTutari; break;
                                        case 4: borcAlacak.Alacak4 = itemTah.TaksitTutari; break;
                                        case 5: borcAlacak.Alacak5 = itemTah.TaksitTutari; break;
                                        case 6: borcAlacak.Alacak6 = itemTah.TaksitTutari; break;
                                        case 7: borcAlacak.Alacak7 = itemTah.TaksitTutari; break;
                                        case 8: borcAlacak.Alacak8 = itemTah.TaksitTutari; break;
                                        case 9: borcAlacak.Alacak9 = itemTah.TaksitTutari; break;
                                        case 10: borcAlacak.Alacak10 = itemTah.TaksitTutari; break;
                                        case 11: borcAlacak.Alacak11 = itemTah.TaksitTutari; break;
                                        case 12: borcAlacak.Alacak12 = itemTah.TaksitTutari; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);

                                    #endregion

                                    #region Müşteri/S.Şirket Hesabına Alacak Yazılıyor pol transferde odemetipi 2 ise

                                    CariHesapBorcAlacak cariHesapBorcSsirketiAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesapBorcAlacak cariHesapBorcSEttirenAlacakEkle = new CariHesapBorcAlacak();
                                    CariHareketleri cariHaraketler = new CariHareketleri();
                                    if (itemTah.OdemTipi == 2 && itemTah.OtomatikTahsilatiKkMi == 1)
                                    {
                                        #region S.ettiren cari alacak ekleme
                                        //foreach (var itemOdemeTipiIki in musteriHesap)
                                        //{
                                        cariHesapBorcSEttirenAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                                        cariHesapBorcSEttirenAlacakEkle.CariHesapKodu = musteriHesap.CariHesapKodu;
                                        cariHesapBorcSEttirenAlacakEkle.TVMKodu = musteriHesap.TVMKodu;
                                        //}
                                        cariHesapBorcSEttirenAlacakEkle.KayitTarihi = DateTime.Now;
                                        cariHesapBorcSEttirenAlacakEkle.KimlikNo = musteriHesap.KimlikNo;
                                        cariHesapBorcSEttirenAlacakEkle.Donem = itemTah.TaksitVadeTarihi.Year;
                                        cariHesapBorcSEttirenAlacakEkle.GuncellemeTarihi = DateTime.Now;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 01:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak1 = itemTah.TaksitTutari;
                                                break;
                                            case 02:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak2 = itemTah.TaksitTutari;
                                                break;
                                            case 03:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak3 = itemTah.TaksitTutari;
                                                break;
                                            case 04:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak4 = itemTah.TaksitTutari;
                                                break;
                                            case 05:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak5 = itemTah.TaksitTutari;
                                                break;
                                            case 06:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak6 = itemTah.TaksitTutari;
                                                break;
                                            case 07:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak7 = itemTah.TaksitTutari;
                                                break;
                                            case 08:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak8 = itemTah.TaksitTutari;
                                                break;
                                            case 09:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak9 = itemTah.TaksitTutari;
                                                break;
                                            case 10:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak10 = itemTah.TaksitTutari;
                                                break;
                                            case 11:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak11 = itemTah.TaksitTutari;
                                                break;
                                            case 12:
                                                cariHesapBorcSEttirenAlacakEkle.Alacak12 = itemTah.TaksitTutari;
                                                break;

                                            default:
                                                break;
                                        }
                                        UpdatePolTahCariHesapBorcAlacak(cariHesapBorcSEttirenAlacakEkle.TVMKodu, cariHesapBorcSEttirenAlacakEkle.CariHesapKodu, itemTah.TaksitVadeTarihi.Year.ToString(), itemTah.TaksitVadeTarihi.Month.ToString(), 0, itemTah.TaksitTutari);

                                        #endregion

                                        #region sigorta şirketı cari borc ekle   
                                        //sirketHesapKodu = "320.01." + getSirket.VergiNumarasi;
                                        //sirketHesap = this.getCariHesapBilgi(_AktifKullanici.TVMKodu, sirketHesapKodu);
                                        //foreach (var itemOdemeTipiIkiSSirketi in sirketHesap)
                                        //{
                                        cariHesapBorcSsirketiAlacakEkle.CariHesapId = sirketHesap.CariHesapId;
                                        cariHesapBorcSsirketiAlacakEkle.CariHesapKodu = sirketHesap.CariHesapKodu;
                                        cariHesapBorcSsirketiAlacakEkle.TVMKodu = sirketHesap.TVMKodu;
                                        cariHesapBorcSsirketiAlacakEkle.KimlikNo = sirketHesap.KimlikNo;
                                        cariHesapBorcSsirketiAlacakEkle.GuncellemeTarihi = sirketHesap.GuncellemeTarihi;
                                        //}
                                        cariHesapBorcSsirketiAlacakEkle.Donem = itemTah.TaksitVadeTarihi.Year;
                                        cariHesapBorcSsirketiAlacakEkle.GuncellemeTarihi = DateTime.Now;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 01:
                                                cariHesapBorcSsirketiAlacakEkle.Borc1 = itemTah.TaksitTutari;
                                                break;
                                            case 02:
                                                cariHesapBorcSsirketiAlacakEkle.Borc2 = itemTah.TaksitTutari;
                                                break;
                                            case 03:
                                                cariHesapBorcSsirketiAlacakEkle.Borc3 = itemTah.TaksitTutari;
                                                break;
                                            case 04:
                                                cariHesapBorcSsirketiAlacakEkle.Borc4 = itemTah.TaksitTutari;
                                                break;
                                            case 05:
                                                cariHesapBorcSsirketiAlacakEkle.Borc5 = itemTah.TaksitTutari;
                                                break;
                                            case 06:
                                                cariHesapBorcSsirketiAlacakEkle.Borc6 = itemTah.TaksitTutari;
                                                break;
                                            case 07:
                                                cariHesapBorcSsirketiAlacakEkle.Borc7 = itemTah.TaksitTutari;
                                                break;
                                            case 08:
                                                cariHesapBorcSsirketiAlacakEkle.Borc8 = itemTah.TaksitTutari;
                                                break;
                                            case 09:
                                                cariHesapBorcSsirketiAlacakEkle.Borc9 = itemTah.TaksitTutari;
                                                break;
                                            case 10:
                                                cariHesapBorcSsirketiAlacakEkle.Borc10 = itemTah.TaksitTutari;
                                                break;
                                            case 11:
                                                cariHesapBorcSsirketiAlacakEkle.Borc11 = itemTah.TaksitTutari;
                                                break;
                                            case 12:
                                                cariHesapBorcSsirketiAlacakEkle.Borc12 = itemTah.TaksitTutari;
                                                break;

                                            default:
                                                break;
                                        }
                                        UpdatePolTahCariHesapBorcAlacak(cariHesapBorcSsirketiAlacakEkle.TVMKodu, cariHesapBorcSsirketiAlacakEkle.CariHesapKodu, itemTah.TaksitVadeTarihi.Year.ToString(), itemTah.TaksitVadeTarihi.Month.ToString(), itemTah.TaksitTutari, 0);
                                        #region cari haraket ekleme

                                        cariHaraketler.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                        cariHaraketler.DovizKuru = aktarilanPolice.DovizKur;
                                        cariHaraketler.DovizTipi = aktarilanPolice.ParaBirimi;
                                        if (cariHaraketler.DovizTipi != "TL")
                                        {
                                            cariHaraketler.DovizTutari = aktarilanPolice.DovizKur * cariHaraketler.Tutar;
                                        }
                                        cariHaraketler.OdemeTipi = itemTah.OdemTipi;
                                        cariHaraketler.EvrakTipi = CariEvrakTipKodu.PrimTahsilat;
                                        if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                        {
                                            cariHaraketler.Aciklama = "Tahsilat - " + " " +
                                                                aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                                aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + " - " + "Sistem";
                                        }
                                        else
                                        {

                                            cariHaraketler.Aciklama = "Tahsilat - " + " " +
                                                aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                                aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + " - " + "Sistem";

                                        }
                                        cariHaraketler.Tutar = itemTah.TaksitTutari;
                                        cariHaraketler.GuncellemeTarihi = DateTime.Now;
                                        cariHaraketler.OdemeTarihi = itemTah.TaksitVadeTarihi;
                                        cariHaraketler.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                        //foreach (var itemSigortasirketCari in sirketHesap)
                                        //{
                                        cariHaraketler.CariHesapKodu = sirketHesap.CariHesapKodu;
                                        //}
                                        cariHaraketler.BorcAlacakTipi = "B";
                                        cariHaraketler.KayitTarihi = DateTime.Now;
                                        cariHaraketler.TVMKodu = aktarilanPolice.TVMKodu.Value;
                                        PolTahCariHareketEkle(cariHaraketler);

                                        #endregion
                                        #endregion
                                    }
                                    #endregion

                                }
                                #endregion

                                #region İptal

                                if (itemTah.TaksitTutari < 0)
                                {
                                    var tutarCarpan = -1;
                                    #region Müşteri Hesabına Borç Yazılıyor
                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = tcVkn;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = musteriHesap.CariHesapKodu;
                                    borcAlacak.CariHesapId = musteriHesap.CariHesapId;
                                    borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                    borcAlacak.PoliceId = itemTah.PoliceId;
                                    switch (itemTah.TaksitVadeTarihi.Month)
                                    {
                                        case 1: borcAlacak.Alacak1 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 2: borcAlacak.Alacak2 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 3: borcAlacak.Alacak3 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 4: borcAlacak.Alacak4 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 5: borcAlacak.Alacak5 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 6: borcAlacak.Alacak6 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 7: borcAlacak.Alacak7 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 8: borcAlacak.Alacak8 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 9: borcAlacak.Alacak9 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 10: borcAlacak.Alacak10 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 11: borcAlacak.Alacak11 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 12: borcAlacak.Alacak12 = itemTah.TaksitTutari * tutarCarpan; break;
                                        default:
                                            break;
                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion

                                    #region Sigorta Şirketi Hesabına Boç Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "B";
                                    carHareket.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                    carHareket.CariHesapKodu = sirketHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo + "-" + itemTah.TaksitNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.PrimIadesi;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = itemTah.TaksitTutari * tutarCarpan;
                                    carHareket.OdemeTipi = itemTah.OdemTipi;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketHesapKodu;
                                    borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                    borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                    borcAlacak.PoliceId = itemTah.PoliceId;

                                    switch (itemTah.TaksitVadeTarihi.Month)
                                    {
                                        case 1: borcAlacak.Borc1 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 2: borcAlacak.Borc2 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 3: borcAlacak.Borc3 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 4: borcAlacak.Borc4 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 5: borcAlacak.Borc5 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 6: borcAlacak.Borc6 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 7: borcAlacak.Borc7 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 8: borcAlacak.Borc8 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 9: borcAlacak.Borc9 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 10: borcAlacak.Borc10 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 11: borcAlacak.Borc11 = itemTah.TaksitTutari * tutarCarpan; break;
                                        case 12: borcAlacak.Borc12 = itemTah.TaksitTutari * tutarCarpan; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);

                                    #endregion

                                    #region Müşteri/S.Şirket Hesabına Alacak Yazılıyor pol transferde odemetipi 2 ise

                                    CariHesapBorcAlacak cariHesapBorcSsirketiAlacakEkle = new CariHesapBorcAlacak();
                                    CariHesapBorcAlacak cariHesapBorcSEttirenAlacakEkle = new CariHesapBorcAlacak();
                                    CariHareketleri cariHaraketler = new CariHareketleri();
                                    if (itemTah.OdemTipi == 2 && itemTah.OtomatikTahsilatiKkMi == 1)
                                    {
                                        #region S.ettiren cari alacak ekleme
                                        //foreach (var itemOdemeTipiIki in musteriHesap)
                                        //{
                                        cariHesapBorcSEttirenAlacakEkle.CariHesapId = musteriHesap.CariHesapId;
                                        cariHesapBorcSEttirenAlacakEkle.CariHesapKodu = musteriHesap.CariHesapKodu;
                                        cariHesapBorcSEttirenAlacakEkle.TVMKodu = musteriHesap.TVMKodu;
                                        //}
                                        cariHesapBorcSEttirenAlacakEkle.KayitTarihi = DateTime.Now;
                                        cariHesapBorcSEttirenAlacakEkle.KimlikNo = musteriHesap.KimlikNo;
                                        cariHesapBorcSEttirenAlacakEkle.Donem = itemTah.TaksitVadeTarihi.Year;
                                        cariHesapBorcSEttirenAlacakEkle.GuncellemeTarihi = DateTime.Now;
                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 01:
                                                cariHesapBorcSEttirenAlacakEkle.Borc1 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 02:
                                                cariHesapBorcSEttirenAlacakEkle.Borc2 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 03:
                                                cariHesapBorcSEttirenAlacakEkle.Borc3 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 04:
                                                cariHesapBorcSEttirenAlacakEkle.Borc4 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 05:
                                                cariHesapBorcSEttirenAlacakEkle.Borc5 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 06:
                                                cariHesapBorcSEttirenAlacakEkle.Borc6 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 07:
                                                cariHesapBorcSEttirenAlacakEkle.Borc7 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 08:
                                                cariHesapBorcSEttirenAlacakEkle.Borc8 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 09:
                                                cariHesapBorcSEttirenAlacakEkle.Borc9 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 10:
                                                cariHesapBorcSEttirenAlacakEkle.Borc10 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 11:
                                                cariHesapBorcSEttirenAlacakEkle.Borc11 = itemTah.TaksitTutari * tutarCarpan;
                                                break;
                                            case 12:
                                                cariHesapBorcSEttirenAlacakEkle.Borc12 = itemTah.TaksitTutari * tutarCarpan;
                                                break;

                                            default:
                                                break;
                                        }
                                        UpdatePolTahCariHesapBorcAlacak(cariHesapBorcSEttirenAlacakEkle.TVMKodu, cariHesapBorcSEttirenAlacakEkle.CariHesapKodu, itemTah.TaksitVadeTarihi.Year.ToString(), itemTah.TaksitVadeTarihi.Month.ToString(), itemTah.TaksitTutari * tutarCarpan, 0);

                                        #endregion

                                        #region sigorta şirketı cari borc ekle   

                                        //cariHesapBorcSsirketiAlacakEkle.CariHesapId = sirketHesap.CariHesapId;
                                        //cariHesapBorcSsirketiAlacakEkle.CariHesapKodu = sirketHesap.CariHesapKodu;
                                        //cariHesapBorcSsirketiAlacakEkle.TVMKodu = sirketHesap.TVMKodu;
                                        //cariHesapBorcSsirketiAlacakEkle.KimlikNo = sirketHesap.KimlikNo;
                                        //cariHesapBorcSsirketiAlacakEkle.GuncellemeTarihi = sirketHesap.GuncellemeTarihi;

                                        //cariHesapBorcSsirketiAlacakEkle.Donem = itemTah.TaksitVadeTarihi.Year;
                                        //cariHesapBorcSsirketiAlacakEkle.GuncellemeTarihi = DateTime.Now;
                                        //switch (itemTah.TaksitVadeTarihi.Month)
                                        //{
                                        //    case 01:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak1 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 02:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak2 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 03:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak3 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 04:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak4 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 05:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak5 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 06:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak6 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 07:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak7 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 08:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak8 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 09:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak9 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 10:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak10 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 11:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak11 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;
                                        //    case 12:
                                        //        cariHesapBorcSsirketiAlacakEkle.Alacak12 = itemTah.TaksitTutari * tutarCarpan;
                                        //        break;

                                        //    default:
                                        //        break;
                                        //}
                                        //UpdatePolTahCariHesapBorcAlacak(cariHesapBorcSsirketiAlacakEkle.TVMKodu, cariHesapBorcSsirketiAlacakEkle.CariHesapKodu, itemTah.TaksitVadeTarihi.Year.ToString(), itemTah.TaksitVadeTarihi.Month.ToString(),0, itemTah.TaksitTutari * tutarCarpan);
                                        borcAlacak = new CariHesapBorcAlacakServiceModel();
                                        borcAlacak.TVMKodu = tvmKodu;
                                        borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                        borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                        borcAlacak.CariHesapKodu = sirketHesapKodu;
                                        borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                        borcAlacak.Donem = itemTah.TaksitVadeTarihi.Year;
                                        borcAlacak.PoliceId = itemTah.PoliceId;

                                        switch (itemTah.TaksitVadeTarihi.Month)
                                        {
                                            case 1: borcAlacak.Alacak1 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 2: borcAlacak.Alacak2 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 3: borcAlacak.Alacak3 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 4: borcAlacak.Alacak4 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 5: borcAlacak.Alacak5 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 6: borcAlacak.Alacak6 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 7: borcAlacak.Alacak7 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 8: borcAlacak.Alacak8 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 9: borcAlacak.Alacak9 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 10: borcAlacak.Alacak10 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 11: borcAlacak.Alacak11 = itemTah.TaksitTutari * tutarCarpan; break;
                                            case 12: borcAlacak.Alacak12 = itemTah.TaksitTutari * tutarCarpan; break;
                                            default:
                                                break;

                                        }
                                        BAList.Add(borcAlacak);

                                        #region cari haraket ekleme

                                        cariHaraketler.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                        cariHaraketler.DovizKuru = aktarilanPolice.DovizKur;
                                        cariHaraketler.DovizTipi = aktarilanPolice.ParaBirimi;
                                        cariHaraketler.OdemeTipi = itemTah.OdemTipi;
                                        cariHaraketler.EvrakTipi = CariEvrakTipKodu.TahsilatIptal;
                                        if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                        {
                                            cariHaraketler.Aciklama = "Tahsilat İptal - " + " " +
                                                                aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                                aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + " - " + "Sistem";
                                        }
                                        else
                                        {

                                            cariHaraketler.Aciklama = "Tahsilat İptal - " + " " +
                                                aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                                aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + " - " + "Sistem";

                                        }
                                        cariHaraketler.Tutar = itemTah.TaksitTutari * tutarCarpan;
                                        if (cariHaraketler.DovizTipi != "TL")
                                        {
                                            cariHaraketler.DovizTutari = aktarilanPolice.DovizKur * cariHaraketler.Tutar;
                                        }
                                        cariHaraketler.GuncellemeTarihi = DateTime.Now;
                                        cariHaraketler.OdemeTarihi = itemTah.TaksitVadeTarihi;
                                        cariHaraketler.CariHareketTarihi = itemTah.TaksitVadeTarihi;
                                        //foreach (var itemSigortasirketCari in sirketHesap)
                                        //{
                                        cariHaraketler.CariHesapKodu = sirketHesap.CariHesapKodu;
                                        //}
                                        cariHaraketler.BorcAlacakTipi = "A";
                                        cariHaraketler.KayitTarihi = DateTime.Now;
                                        cariHaraketler.TVMKodu = aktarilanPolice.TVMKodu.Value;
                                        PolTahCariHareketEkle(cariHaraketler);

                                        #endregion
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion

                            }
                            #region Komisyon Tahakkuk

                            if (aktarilanPolice.Komisyon.HasValue)
                            {
                                if (aktarilanPolice.Komisyon.Value > 0)
                                {
                                    #region Sigorta Şirketi Hesabına Borc Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "B";
                                    carHareket.CariHareketTarihi = aktarilanPolice.TanzimTarihi;
                                    carHareket.CariHesapKodu = sirketHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonTahakkuk;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = aktarilanPolice.Komisyon.Value;
                                    carHareket.OdemeTipi = aktarilanPolice.PoliceOdemePlanis.Count > 0 ? aktarilanPolice.PoliceOdemePlanis.FirstOrDefault().OdemeTipi.Value : CariOdemeTipKodu.Havale;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketHesapKodu;
                                    borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                    borcAlacak.Donem = aktarilanPolice.TanzimTarihi.Value.Year;
                                    borcAlacak.PoliceId = aktarilanPolice.PoliceId;
                                    switch (aktarilanPolice.TanzimTarihi.Value.Month)
                                    {
                                        case 1: borcAlacak.Borc1 = aktarilanPolice.Komisyon; break;
                                        case 2: borcAlacak.Borc2 = aktarilanPolice.Komisyon; break;
                                        case 3: borcAlacak.Borc3 = aktarilanPolice.Komisyon; break;
                                        case 4: borcAlacak.Borc4 = aktarilanPolice.Komisyon; break;
                                        case 5: borcAlacak.Borc5 = aktarilanPolice.Komisyon; break;
                                        case 6: borcAlacak.Borc6 = aktarilanPolice.Komisyon; break;
                                        case 7: borcAlacak.Borc7 = aktarilanPolice.Komisyon; break;
                                        case 8: borcAlacak.Borc8 = aktarilanPolice.Komisyon; break;
                                        case 9: borcAlacak.Borc9 = aktarilanPolice.Komisyon; break;
                                        case 10: borcAlacak.Borc10 = aktarilanPolice.Komisyon; break;
                                        case 11: borcAlacak.Borc11 = aktarilanPolice.Komisyon; break;
                                        case 12: borcAlacak.Borc12 = aktarilanPolice.Komisyon; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion

                                    #region Sigorta Şirketi Komisyon Gelir Hesabına Alacak Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "A";
                                    carHareket.CariHareketTarihi = aktarilanPolice.TanzimTarihi;
                                    carHareket.CariHesapKodu = sirketKomGelirHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonTahakkuk;
                                    carHareket.OdemeTipi = aktarilanPolice.PoliceOdemePlanis.Count > 0 ? aktarilanPolice.PoliceOdemePlanis.FirstOrDefault().OdemeTipi.Value : CariOdemeTipKodu.Havale;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = aktarilanPolice.Komisyon.Value;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketKomGelirHesapKodu;
                                    borcAlacak.CariHesapId = sirketKomGelirHesap.CariHesapId;

                                    borcAlacak.Donem = aktarilanPolice.TanzimTarihi.Value.Year;
                                    borcAlacak.PoliceId = aktarilanPolice.PoliceId;
                                    switch (aktarilanPolice.TanzimTarihi.Value.Month)
                                    {
                                        case 1: borcAlacak.Alacak1 = aktarilanPolice.Komisyon; break;
                                        case 2: borcAlacak.Alacak2 = aktarilanPolice.Komisyon; break;
                                        case 3: borcAlacak.Alacak3 = aktarilanPolice.Komisyon; break;
                                        case 4: borcAlacak.Alacak4 = aktarilanPolice.Komisyon; break;
                                        case 5: borcAlacak.Alacak5 = aktarilanPolice.Komisyon; break;
                                        case 6: borcAlacak.Alacak6 = aktarilanPolice.Komisyon; break;
                                        case 7: borcAlacak.Alacak7 = aktarilanPolice.Komisyon; break;
                                        case 8: borcAlacak.Alacak8 = aktarilanPolice.Komisyon; break;
                                        case 9: borcAlacak.Alacak9 = aktarilanPolice.Komisyon; break;
                                        case 10: borcAlacak.Alacak10 = aktarilanPolice.Komisyon; break;
                                        case 11: borcAlacak.Alacak11 = aktarilanPolice.Komisyon; break;
                                        case 12: borcAlacak.Alacak12 = aktarilanPolice.Komisyon; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion
                                }
                            }

                            #endregion

                            #region Komisyon İptal
                            int carpan = -1;
                            if (aktarilanPolice.Komisyon.HasValue)
                            {
                                if (aktarilanPolice.Komisyon.Value < 0)
                                {
                                    #region Sigorta Şirketi Hesabına Alacak Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "A";
                                    carHareket.CariHareketTarihi = aktarilanPolice.TanzimTarihi;
                                    carHareket.CariHesapKodu = sirketHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonIadesi;
                                    carHareket.OdemeTipi = aktarilanPolice.PoliceOdemePlanis.Count > 0 ? aktarilanPolice.PoliceOdemePlanis.FirstOrDefault().OdemeTipi.Value : CariOdemeTipKodu.Havale;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = aktarilanPolice.Komisyon.Value * carpan;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketHesapKodu;
                                    borcAlacak.CariHesapId = sirketHesap.CariHesapId;
                                    borcAlacak.Donem = aktarilanPolice.TanzimTarihi.Value.Year;
                                    borcAlacak.PoliceId = aktarilanPolice.PoliceId;
                                    switch (aktarilanPolice.TanzimTarihi.Value.Month)
                                    {
                                        case 1: borcAlacak.Alacak1 = aktarilanPolice.Komisyon * carpan; break;
                                        case 2: borcAlacak.Alacak2 = aktarilanPolice.Komisyon * carpan; break;
                                        case 3: borcAlacak.Alacak3 = aktarilanPolice.Komisyon * carpan; break;
                                        case 4: borcAlacak.Alacak4 = aktarilanPolice.Komisyon * carpan; break;
                                        case 5: borcAlacak.Alacak5 = aktarilanPolice.Komisyon * carpan; break;
                                        case 6: borcAlacak.Alacak6 = aktarilanPolice.Komisyon * carpan; break;
                                        case 7: borcAlacak.Alacak7 = aktarilanPolice.Komisyon * carpan; break;
                                        case 8: borcAlacak.Alacak8 = aktarilanPolice.Komisyon * carpan; break;
                                        case 9: borcAlacak.Alacak9 = aktarilanPolice.Komisyon * carpan; break;
                                        case 10: borcAlacak.Alacak10 = aktarilanPolice.Komisyon * carpan; break;
                                        case 11: borcAlacak.Alacak11 = aktarilanPolice.Komisyon * carpan; break;
                                        case 12: borcAlacak.Alacak12 = aktarilanPolice.Komisyon * carpan; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion

                                    #region Sigorta Şirketi Komisyon Gider Hesabına Alacak Yazılıyor
                                    carHareket = new CariHareketleri();
                                    if (aktarilanPolice.BransKodu == 1 || aktarilanPolice.BransKodu == 2)
                                    {
                                        carHareket.Aciklama = aktarilanPolice.BransAdi + "-" + aktarilanPolice.SigortaSirketleri.SirketAdi + "- " + aktarilanPolice.PoliceSigortali.AdiUnvan + "- " +
                                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " -" + aktarilanPolice.PoliceArac.PlakaKodu + " " + aktarilanPolice.PoliceArac.PlakaNo + "-Sistem";
                                    }
                                    else
                                    {

                                        carHareket.Aciklama = aktarilanPolice.BransAdi + " - " + aktarilanPolice.SigortaSirketleri.SirketAdi + " - " + aktarilanPolice.PoliceSigortali.AdiUnvan + "  " +
                                            aktarilanPolice.PoliceSigortali.SoyadiUnvan + " - " + aktarilanPolice.PoliceSigortali.IlAdi + " / " + aktarilanPolice.PoliceSigortali.IlceAdi + "-Sistem";

                                    }
                                    carHareket.BorcAlacakTipi = "B";
                                    carHareket.CariHareketTarihi = aktarilanPolice.TanzimTarihi;
                                    carHareket.CariHesapKodu = sirketKomGiderHesapKodu;
                                    carHareket.DovizKuru = aktarilanPolice.DovizKur;
                                    carHareket.DovizTipi = aktarilanPolice.ParaBirimi;
                                    carHareket.EvrakNo = aktarilanPolice.PoliceNumarasi + "-" + aktarilanPolice.YenilemeNo + "-" + aktarilanPolice.EkNo;
                                    carHareket.EvrakTipi = CariEvrakTipKodu.KomisyonIadesi;
                                    carHareket.KayitTarihi = TurkeyDateTime.Now;
                                    carHareket.Tutar = aktarilanPolice.Komisyon.Value * carpan;
                                    carHareket.OdemeTipi = aktarilanPolice.PoliceOdemePlanis.Count > 0 ? aktarilanPolice.PoliceOdemePlanis.FirstOrDefault().OdemeTipi.Value : CariOdemeTipKodu.Havale;
                                    carHareket.TVMKodu = aktarilanPolice.TVMKodu.HasValue ? aktarilanPolice.TVMKodu.Value : 0;
                                    if (carHareket.DovizTipi != "TL")
                                    {
                                        if (carHareket.DovizKuru.HasValue)
                                        {
                                            carHareket.DovizTutari = carHareket.DovizKuru * carHareket.Tutar;
                                        }
                                    }
                                    model.carHareketList.Add(carHareket);

                                    borcAlacak = new CariHesapBorcAlacakServiceModel();
                                    borcAlacak.TVMKodu = tvmKodu;
                                    borcAlacak.KimlikNo = getSirket.VergiNumarasi;
                                    borcAlacak.KayitTarihi = TurkeyDateTime.Now;
                                    borcAlacak.CariHesapKodu = sirketKomGiderHesapKodu;
                                    borcAlacak.CariHesapId = sirketKomGiderHesap.CariHesapId;
                                    borcAlacak.Donem = aktarilanPolice.TanzimTarihi.Value.Year;
                                    borcAlacak.PoliceId = aktarilanPolice.PoliceId;
                                    switch (aktarilanPolice.TanzimTarihi.Value.Month)
                                    {
                                        case 1: borcAlacak.Borc1 = aktarilanPolice.Komisyon * carpan; break;
                                        case 2: borcAlacak.Borc2 = aktarilanPolice.Komisyon * carpan; break;
                                        case 3: borcAlacak.Borc3 = aktarilanPolice.Komisyon * carpan; break;
                                        case 4: borcAlacak.Borc4 = aktarilanPolice.Komisyon * carpan; break;
                                        case 5: borcAlacak.Borc5 = aktarilanPolice.Komisyon * carpan; break;
                                        case 6: borcAlacak.Borc6 = aktarilanPolice.Komisyon * carpan; break;
                                        case 7: borcAlacak.Borc7 = aktarilanPolice.Komisyon * carpan; break;
                                        case 8: borcAlacak.Borc8 = aktarilanPolice.Komisyon * carpan; break;
                                        case 9: borcAlacak.Borc9 = aktarilanPolice.Komisyon * carpan; break;
                                        case 10: borcAlacak.Borc10 = aktarilanPolice.Komisyon * carpan; break;
                                        case 11: borcAlacak.Borc11 = aktarilanPolice.Komisyon * carpan; break;
                                        case 12: borcAlacak.Borc12 = aktarilanPolice.Komisyon * carpan; break;
                                        default:
                                            break;

                                    }
                                    BAList.Add(borcAlacak);
                                    #endregion
                                }
                            }
                            #endregion
                        }

                        model.ekNo = aktarilanPolice.EkNo;
                        model.yenilemeNo = aktarilanPolice.YenilemeNo;
                        model.polNo = aktarilanPolice.PoliceNumarasi;
                        model.sigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                        carHareketList.Add(model);
                    }
                    else
                    {
                        CariHesapBAReturnModel rModel = new CariHesapBAReturnModel();
                        rModel.Basarili = false;
                        rModel.CariHesapKodu = musteriHesapKodu;
                        rModel.Donem = aktarilanPolice.TanzimTarihi.HasValue ? aktarilanPolice.TanzimTarihi.Value.Year : 0;
                        rModel.PoliceNo = aktarilanPolice.PoliceNumarasi;
                        rModel.YenilemeNo = aktarilanPolice.YenilemeNo;
                        rModel.EkNo = aktarilanPolice.EkNo;
                        rModel.SigortaSirketKodu = aktarilanPolice.TUMBirlikKodu;
                        rModel.TvmKodu = aktarilanPolice.TVMKodu;
                        rModel.Mesaj = "Müşteri kimlik numarası bulunamadı, poliçe cari hesaba aktarılmadı.";
                        returnList.Add(rModel);
                    }
                }

            }
            if (carHareketList.Count > 0)
            {
                borcAlacak = new CariHesapBorcAlacakServiceModel();
                borcAlacak.cariHareketList = carHareketList;
                BAList.Add(borcAlacak);
            }
            if (BAList.Count > 0)
            {
                returnList = this.CariHesapBorcAlacakListUpdate(BAList);
            }

            return returnList;
        }

        #endregion

        #region cari hesap mizanı
        public List<CariHesapBorcAlacak> CariHesapBorcAlacakGetirByCariHesapKodu(int Donem, string CariHesapKodu)
        {
            List<CariHesapBorcAlacak> list = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(s => s.CariHesapKodu.Trim().StartsWith(CariHesapKodu) && s.TVMKodu == _AktifKullanici.TVMKodu && s.Donem == Donem).ToList();

            foreach (var item in list)
            {
                if (item.Alacak1 == null) item.Alacak1 = 0;
                if (item.Alacak2 == null) item.Alacak2 = 0;
                if (item.Alacak3 == null) item.Alacak3 = 0;
                if (item.Alacak4 == null) item.Alacak4 = 0;
                if (item.Alacak5 == null) item.Alacak5 = 0;
                if (item.Alacak6 == null) item.Alacak6 = 0;
                if (item.Alacak7 == null) item.Alacak7 = 0;
                if (item.Alacak8 == null) item.Alacak8 = 0;
                if (item.Alacak9 == null) item.Alacak9 = 0;
                if (item.Alacak10 == null) item.Alacak10 = 0;
                if (item.Alacak11 == null) item.Alacak11 = 0;
                if (item.Alacak12 == null) item.Alacak12 = 0;

                if (item.Borc1 == null) item.Borc1 = 0;
                if (item.Borc2 == null) item.Borc2 = 0;
                if (item.Borc3 == null) item.Borc3 = 0;
                if (item.Borc4 == null) item.Borc4 = 0;
                if (item.Borc5 == null) item.Borc5 = 0;
                if (item.Borc6 == null) item.Borc6 = 0;
                if (item.Borc7 == null) item.Borc7 = 0;
                if (item.Borc8 == null) item.Borc8 = 0;
                if (item.Borc9 == null) item.Borc9 = 0;
                if (item.Borc10 == null) item.Borc10 = 0;
                if (item.Borc11 == null) item.Borc11 = 0;
                if (item.Borc12 == null) item.Borc12 = 0;
            }


            return list;
        }

        public List<CariHesapBorcAlacak> CariHesapBorcAlacakGetirByCariHesapKodu(string CariHesapKodu)
        {
            List<CariHesapBorcAlacak> list = _MuhasebeContext.CariHesapBorcAlacakRepository.All().Where(s => s.CariHesapKodu.Trim().StartsWith(CariHesapKodu) && s.TVMKodu == _AktifKullanici.TVMKodu).ToList();
            return list;
        }
        #endregion


        #region policeleri Cariye Aktar
        public int PoliceleriCariyeAktarma(MuhasebeAktarimKonfigurasyon model)
        {
            try
            {
                int siraNo = 1;
                MuhasebeAktarimKonfigurasyon MuhasebeKonfig = _MuhasebeContext.MuhasebeAktarimKonfigurasyonRepository.All().OrderByDescending(m => m.SiraNo).FirstOrDefault();
                if (MuhasebeKonfig != null)
                {
                    siraNo = MuhasebeKonfig.SiraNo + 1;
                }
                model.SiraNo = siraNo;
                MuhasebeAktarimKonfigurasyon policeCariAktar = _MuhasebeContext.MuhasebeAktarimKonfigurasyonRepository.Create(model);
                _MuhasebeContext.Commit();
                return policeCariAktar.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public double GetMuhasebeAktarimYuzdesi(int konfigurasyonId)
        {
            MuhasebeAktarimKonfigurasyon konfigurasyon = _MuhasebeContext.MuhasebeAktarimKonfigurasyonRepository.All().Where(konfig => konfig.Id == konfigurasyonId).FirstOrDefault();
            return konfigurasyon.AktarimYuzdesi.HasValue ? konfigurasyon.AktarimYuzdesi.Value : 0;
        }


        public List<MuhasebeAktarimKonfigurasyon> GetMuhasebeAktarimListesi()
        {
            var liste = _MuhasebeContext.MuhasebeAktarimKonfigurasyonRepository.All().Where(s => s.TvmKodu == _AktifKullanici.TVMKodu).ToList();
            return liste;
        }

        #endregion
        public List<MusteriPolice> GetMusteriPoliceleri(int tvmKodu, string kimlikNo)
        {
            List<MusteriPolice> polList = new List<MusteriPolice>();
            MusteriPolice musteriPolice = null;
            int yil = TurkeyDateTime.Now.Year;
            var policeler = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == tvmKodu && s.BrutPrim != 0 &&
                            (s.PoliceSigortali.KimlikNo == kimlikNo || s.PoliceSigortali.VergiKimlikNo == kimlikNo) && (s.TanzimTarihi.Value.Year == yil || s.TanzimTarihi.Value.Year == (yil - 1))
                            )
                            .GroupBy(g => new { g.PoliceId, g.PoliceNumarasi, g.YenilemeNo, g.EkNo, g.BrutPrim })
                            .Select(s => new { list = s.ToList() }).ToList();
            if (policeler != null)
            {
                for (int i = 0; i < policeler.Count(); i++)
                {
                    for (int j = 0; j < policeler[i].list.Count(); j++)
                    {
                        musteriPolice = new MusteriPolice()
                        {
                            BrutPrim = policeler[i].list[j].BrutPrim,
                            PoliceNumarasi = policeler[i].list[j].PoliceNumarasi,
                            YenilemeNo = policeler[i].list[j].YenilemeNo,
                            Ekno = policeler[i].list[j].EkNo,
                            OdemTipiKKveyaBKKmi = policeler[i].list[j].PoliceTahsilats.Where(w => w.OdemTipi == 5 || w.OdemTipi == 9).Count() > 0 ? true : false
                        };
                        polList.Add(musteriPolice);
                    }
                }
            }
            if (polList != null)
            {

            }

            return polList;
        }
        public List<CariHareketleri> getCarihareketForPoliceTahsilatRaporu(string TCKNVKN, string EvrakNo)
        {
            var carikodu = "120.01." + TCKNVKN;
            List<CariHareketleri> CariHareket = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.CariHesapKodu == carikodu && w.EvrakNo.Contains(EvrakNo) && w.TVMKodu == _AktifKullanici.TVMKodu).ToList();
            //if(CariHareket.Count>0)
            //{
            //    for (int i = CariHareket.Count-1; i>=0 ; i--)
            //    {
            //        if(CariHareket[i])
            //        toplamOdenenTutar += CariHareket[i].Tutar;
            //    }
            //}
            return CariHareket;
        }
        public List<CariHareketleri> YaslandirmaTablosuCariHesaplari(string PoliceYenilemeEkNo)
        {
            List<CariHareketleri> CariHareket = _MuhasebeContext.CariHareketleriRepository.All().Where(w => w.Aciklama.Contains(PoliceYenilemeEkNo) && w.TVMKodu == _AktifKullanici.TVMKodu && w.OdemeTarihi != null).ToList();

            return CariHareket;
        }

        public List<PoliceTahsilat> getPoliceTahsilat(string policeNo, string zeyilNo, int? yenilemeNo)
        {
            List<PoliceTahsilat> policeler = new List<PoliceTahsilat>();
            var policeGen = _PoliceContext.PoliceGenelRepository.All().Where(w => w.PoliceNumarasi == policeNo && w.EkNo.ToString() == zeyilNo && w.YenilemeNo == yenilemeNo && w.TVMKodu == _AktifKullanici.TVMKodu).FirstOrDefault();
            if (policeGen != null)
            {
                policeler = _PoliceContext.PoliceTahsilatRepository.All().Where(w => w.PoliceId == policeGen.PoliceId).OrderByDescending(w => w.TaksitNo).ToList();
            }
            return policeler;
        }
    }
}
