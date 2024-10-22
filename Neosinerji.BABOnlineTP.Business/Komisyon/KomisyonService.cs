using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Komisyon
{
    public class KomisyonService : IKomisyonService
    {
        #region Variable

        IKomisyonContext _KomisyonContext;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        IPoliceContext _PoliceContext;
        #endregion

        #region Constructor

        public KomisyonService(IKomisyonContext komisyonContext, IAktifKullaniciService aktifKullanici, ITVMService tvmService, IPoliceContext policeContext)
        {
            _KomisyonContext = komisyonContext;
            _AktifKullanici = aktifKullanici;
            _TVMService = tvmService;
            _PoliceContext = policeContext;
        }

        #endregion

        #region Implement Method

        public List<TaliAcenteKomisyonOrani> TaliAcenteKomisyonListesi(List<TVMModel> taliAcenteler, List<TVMModel> disKaynak, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime gecerlilikBaslangicTarihi, int sayfa, int adet, out int toplam)
        {
            var aktifTVMKodu = _AktifKullanici.TVMKodu;
            var _taliAcenteler = taliAcenteler.Select(t => t.Kodu).ToArray();
            var _disKaynaklar = disKaynak.Select(t => t.Kodu).ToArray();
            var _branslar = branslar.Select(b => b.Kodu).ToArray();
            var _sigortaSirketleri = sigortaSirketleri.Select(s => s.Kodu).ToArray();
            toplam = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o => o.TVMKodu == aktifTVMKodu
                        && (_taliAcenteler.Contains(o.TaliTVMKodu ?? 0)
                        || _disKaynaklar.Contains(o.TaliTVMKodu ?? 0))
                        && ((taliDisKaynakKodu != 0 && o.DisKaynakKodu == taliDisKaynakKodu) || (taliDisKaynakKodu == 0 && o.DisKaynakKodu == null))
                        && _branslar.Contains(o.BransKodu ?? 0)
                        && _sigortaSirketleri.Contains(o.SigortaSirketKodu)
                        && o.GecirlilikBaslangicTarihi >= gecerlilikBaslangicTarihi
                        && o.MinUretim == null
                        && o.MaxUretim == null
                        ).Count();

            int _skip = sayfa * adet >= toplam ? 0 : sayfa * adet;
            var list = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o => o.TVMKodu == aktifTVMKodu
                        && (_taliAcenteler.Contains(o.TaliTVMKodu ?? 0)
                        || _disKaynaklar.Contains(o.TaliTVMKodu ?? 0))
                       && ((taliDisKaynakKodu != 0 && o.DisKaynakKodu == taliDisKaynakKodu) || (taliDisKaynakKodu == 0 && o.DisKaynakKodu == null))
                        && _branslar.Contains(o.BransKodu ?? 0)
                        && _sigortaSirketleri.Contains(o.SigortaSirketKodu)
                        && o.GecirlilikBaslangicTarihi >= gecerlilikBaslangicTarihi
                        && o.MinUretim == null
                        && o.MaxUretim == null
                        ).OrderBy(o => o.TaliTVMKodu).ThenBy(o => o.BransKodu).ThenBy(o => o.SigortaSirketKodu).ThenByDescending(o => o.GecirlilikBaslangicTarihi)
                        .Skip(_skip)
                        .Take(adet)
                        .ToList();
            return list;
        }

        public List<TaliAcenteKomisyonOrani> TaliAcenteKademeliKomisyonListesi(List<TVMModel> taliAcenteler, List<TVMModel> disKaynak, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, int gecerliYil)
        {
            var aktifTVMKodu = _AktifKullanici.TVMKodu;
            var _taliAcenteler = taliAcenteler.Select(t => t.Kodu).ToArray();
            var _disKaynaklar = disKaynak.Select(t => t.Kodu).ToArray();
            var _branslar = branslar.Select(b => b.Kodu).ToArray();
            var _sigortaSirketleri = sigortaSirketleri.Select(s => s.Kodu).ToArray();
            var _gecerlilikBaslangicTarihi = new DateTime(gecerliYil, 1, 1);
            var list = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o => o.TVMKodu == aktifTVMKodu
                        && (_taliAcenteler.Contains(o.TaliTVMKodu ?? 0)
                        || _disKaynaklar.Contains(o.TaliTVMKodu ?? 0))
                          && ((taliDisKaynakKodu != 0 && o.DisKaynakKodu == taliDisKaynakKodu) || (taliDisKaynakKodu == 0 && o.DisKaynakKodu == null))
                        && _branslar.Contains(o.BransKodu ?? 0)
                        && _sigortaSirketleri.Contains(o.SigortaSirketKodu)
                        && o.GecirlilikBaslangicTarihi == _gecerlilikBaslangicTarihi
                        && o.MinUretim != null
                        && o.MaxUretim != null)
                        .OrderBy(o => o.TaliTVMKodu)
                        .ThenBy(o => o.BransKodu)
                        .ThenBy(o => o.SigortaSirketKodu)
                        .ThenByDescending(o => o.GecirlilikBaslangicTarihi)
                        .ToList();
            return list;
        }

        public bool TaliAcenteKomisyonListesiOlustur(List<TVMModel> taliAcenteler, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime gecerlilikBaslangicTarihi, decimal oran)
        {
            var sonuc = true;
            try
            {
                var aktifTVMKodu = _AktifKullanici.TVMKodu;
                var aktifTVMUnvani = _AktifKullanici.TVMUnvani;
                var kayitTarihi = DateTime.Now;
                var kayitKullanici = _AktifKullanici.KullaniciKodu;
                foreach (var tali in taliAcenteler)
                {
                    foreach (var brans in branslar)
                    {
                        foreach (var sirket in sigortaSirketleri)
                        {
                            var kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Find(o =>
                                o.TVMKodu == aktifTVMKodu &&
                                o.TaliTVMKodu == tali.Kodu &&
                                  ((taliDisKaynakKodu != 0 && o.DisKaynakKodu == taliDisKaynakKodu) || (taliDisKaynakKodu == 0 && o.DisKaynakKodu == null)) &&
                                o.SigortaSirketKodu == sirket.Kodu &&
                                o.BransKodu == brans.Kodu &&
                                o.GecirlilikBaslangicTarihi == gecerlilikBaslangicTarihi);
                            if (kayit != null)
                            {
                                kayit.KomisyonOran = oran;
                                kayit.GuncellemeTarihi = DateTime.Now;
                                kayit.GuncellemeKullaniciKodu = kayitKullanici;
                                _KomisyonContext.TaliAcenteKomisyonOraniRepository.Update(kayit);
                            }
                            else
                            {
                                kayit = new TaliAcenteKomisyonOrani()
                                {
                                    TVMKodu = aktifTVMKodu,
                                    TaliTVMKodu = tali.Kodu,
                                    SigortaSirketKodu = sirket.Kodu,
                                    BransKodu = brans.Kodu,
                                    GecirlilikBaslangicTarihi = gecerlilikBaslangicTarihi,
                                    KomisyonOran = oran,
                                    KayitTarihi = kayitTarihi,
                                    KayitKullaniciKodu = kayitKullanici
                                };
                                if (taliDisKaynakKodu != 0)
                                {
                                    kayit.DisKaynakKodu = taliDisKaynakKodu;
                                }
                                _KomisyonContext.TaliAcenteKomisyonOraniRepository.Create(kayit);
                            }
                        }
                    }
                }
                _KomisyonContext.Commit();
            }
            catch (Exception )
            {
                sonuc = false;
            }
            return sonuc;
        }

        public bool TaliAcenteKademeliKomisyonListesiOlustur(List<TVMModel> taliAcenteler, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, int gecerliYil, List<KomisyonKademeModel> kademeModel)
        {
            var sonuc = true;
            try
            {
                var aktifTVMKodu = _AktifKullanici.TVMKodu;
                var aktifTVMUnvani = _AktifKullanici.TVMUnvani;
                var kayitTarihi = DateTime.Now;
                var kayitKullanici = _AktifKullanici.KullaniciKodu;

                foreach (var tali in taliAcenteler)
                {
                    foreach (var brans in branslar)
                    {
                        foreach (var sirket in sigortaSirketleri)
                        {
                            var kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o =>
                                o.TVMKodu == aktifTVMKodu &&
                                o.TaliTVMKodu == tali.Kodu &&
                                  ((taliDisKaynakKodu != 0 && o.DisKaynakKodu == taliDisKaynakKodu) || (taliDisKaynakKodu == 0 && o.DisKaynakKodu == null)) &&
                                o.SigortaSirketKodu == sirket.Kodu &&
                                o.BransKodu == brans.Kodu &&
                                o.GecirlilikBaslangicTarihi == new DateTime(gecerliYil, 1, 1) &&
                                o.MinUretim != null &&
                                o.MaxUretim != null
                                ).ToList();
                            int kademeIndex = 0;
                            if (kayit.Count > kademeModel.Count())
                            {
                                for (int i = kayit.Count; i > kademeModel.Count; i--)
                                {
                                    _KomisyonContext.TaliAcenteKomisyonOraniRepository.Delete(kayit[i - 1]);
                                }
                            }
                            foreach (var kademe in kademeModel)
                            {
                                if (kademeIndex < kayit.Count())
                                {
                                    kayit[kademeIndex].MinUretim = kademe.MinUretim;
                                    kayit[kademeIndex].MaxUretim = kademe.MaxUretim;
                                    kayit[kademeIndex].KomisyonOran = kademe.Oran;
                                    kayit[kademeIndex].GuncellemeTarihi = DateTime.Now;
                                    kayit[kademeIndex].GuncellemeKullaniciKodu = kayitKullanici;
                                    _KomisyonContext.TaliAcenteKomisyonOraniRepository.Update(kayit[kademeIndex]);
                                }
                                else
                                {
                                    var _kayit = new TaliAcenteKomisyonOrani()
                                    {
                                        TVMKodu = aktifTVMKodu,
                                        TaliTVMKodu = tali.Kodu,
                                        SigortaSirketKodu = sirket.Kodu,
                                        BransKodu = brans.Kodu,
                                        GecirlilikBaslangicTarihi = new DateTime(gecerliYil, 1, 1),
                                        KomisyonOran = kademe.Oran,
                                        KayitTarihi = kayitTarihi,
                                        KayitKullaniciKodu = kayitKullanici,
                                        MinUretim = kademe.MinUretim,
                                        MaxUretim = kademe.MaxUretim
                                    };
                                    if (taliDisKaynakKodu != 0)
                                    {
                                        _kayit.DisKaynakKodu = taliDisKaynakKodu;
                                    }
                                    _KomisyonContext.TaliAcenteKomisyonOraniRepository.Create(_kayit);
                                }
                                kademeIndex++;
                            }
                        }
                    }
                }
                _KomisyonContext.Commit();
            }
            catch (Exception)
            {
                sonuc = false;
            }
            return sonuc;
        }

        public TaliAcenteKomisyonOrani TaliAcenteKomisyonGuncelle(int komisyonOranId, DateTime gecerlilikBaslangicTarihi, decimal oran, out bool guncellendiMi)
        {
            guncellendiMi = true;
            var kayit = new TaliAcenteKomisyonOrani();
            try
            {
                kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Find(o => o.KomisyonOranId == komisyonOranId);
                kayit.GecirlilikBaslangicTarihi = gecerlilikBaslangicTarihi;
                kayit.KomisyonOran = oran;
                kayit.GuncellemeTarihi = DateTime.Now;
                kayit.GuncellemeKullaniciKodu = _AktifKullanici.TVMKodu;
                _KomisyonContext.TaliAcenteKomisyonOraniRepository.Update(kayit);
                _KomisyonContext.Commit();
                return kayit;
            }
            catch (Exception)
            {
                guncellendiMi = false;
            }
            return kayit;
        }

        public TaliAcenteKomisyonOrani TaliAcenteKademeliKomisyonGuncelle(int komisyonOranId, int gecerliYil, List<KomisyonKademeModel> kademeModel, out bool guncellendiMi)
        {
            guncellendiMi = true;
            var _kayit = new TaliAcenteKomisyonOrani();
            try
            {
                var kayitKullanici = _AktifKullanici.KullaniciKodu;
                var kayitTarihi = DateTime.Now;

                _kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Find(
                    o => o.KomisyonOranId == komisyonOranId);
                var kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o =>
                             o.TVMKodu == _kayit.TVMKodu &&
                             o.TaliTVMKodu == _kayit.TaliTVMKodu &&
                             o.DisKaynakKodu == _kayit.DisKaynakKodu &&
                             o.SigortaSirketKodu == _kayit.SigortaSirketKodu &&
                             o.BransKodu == _kayit.BransKodu &&
                             o.GecirlilikBaslangicTarihi == _kayit.GecirlilikBaslangicTarihi &&
                             o.MinUretim != null &&
                             o.MaxUretim != null
                             ).ToList();
                int kademeIndex = 0;
                if (kayit.Count > kademeModel.Count())
                {
                    for (int i = kayit.Count; i > kademeModel.Count; i--)
                    {
                        _KomisyonContext.TaliAcenteKomisyonOraniRepository.Delete(kayit[i - 1]);
                    }
                }
                foreach (var kademe in kademeModel)
                {
                    if (kademeIndex < kayit.Count())
                    {
                        kayit[kademeIndex].GecirlilikBaslangicTarihi = new DateTime(gecerliYil, 1, 1);
                        kayit[kademeIndex].MinUretim = kademe.MinUretim;
                        kayit[kademeIndex].MaxUretim = kademe.MaxUretim;
                        kayit[kademeIndex].KomisyonOran = kademe.Oran;
                        kayit[kademeIndex].GuncellemeTarihi = DateTime.Now;
                        kayit[kademeIndex].GuncellemeKullaniciKodu = kayitKullanici;
                        _KomisyonContext.TaliAcenteKomisyonOraniRepository.Update(kayit[kademeIndex]);
                    }
                    else
                    {
                        var nkayit = new TaliAcenteKomisyonOrani()
                        {
                            TVMKodu = _kayit.TVMKodu,
                            TaliTVMKodu = _kayit.TaliTVMKodu,
                            DisKaynakKodu = _kayit.DisKaynakKodu,
                            SigortaSirketKodu = _kayit.SigortaSirketKodu,
                            BransKodu = _kayit.BransKodu,
                            GecirlilikBaslangicTarihi = new DateTime(gecerliYil, 1, 1),
                            KomisyonOran = kademe.Oran,
                            KayitTarihi = kayitTarihi,
                            KayitKullaniciKodu = kayitKullanici,
                            MinUretim = kademe.MinUretim,
                            MaxUretim = kademe.MaxUretim
                        };
                        _KomisyonContext.TaliAcenteKomisyonOraniRepository.Create(nkayit);
                    }
                    kademeIndex++;
                }
                _KomisyonContext.Commit();
            }
            catch (Exception)
            {
                guncellendiMi = false;
            }

            return _kayit;
        }

        public List<PoliceGenel> TaliAcentePoliceGenelListesi(TaliKomisyonHesaplamaPoliceDurumu durum, List<TVMModel> taliAcenteler, List<TVMModel> disKaynaklar, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime tarihBaslangic, DateTime tarihBitis, PrimTipleri iptalZeylTahakkuk)
        {
            var aktifTVMKodu = _AktifKullanici.TVMKodu;
            var _taliAcenteler = taliAcenteler.Select(t => t.Kodu).ToArray();
            var _disKaynaklar = disKaynaklar.Select(t => t.Kodu).ToArray();
            var _branslar = branslar.Select(b => b.Kodu).ToArray();
            var _sigortaSirketleri = sigortaSirketleri.Select(s => s.Kodu).ToArray();

            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
            if (tvmDetay != null && tvmDetay.BolgeYetkilisiMi == 1)
            {
                aktifTVMKodu = tvmDetay.BagliOlduguTVMKodu;
            }

            if (durum == TaliKomisyonHesaplamaPoliceDurumu.Hesaplanmamis)
            {
                if (iptalZeylTahakkuk == PrimTipleri.IptalZeyilleri)
                {
                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                p => p.TVMKodu == aktifTVMKodu
                    && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                    && p.TaliAcenteKodu == null
                    && p.TaliKomisyon == null
                    && _branslar.Contains(p.BransKodu ?? 0)
                    && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                    && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                    && p.NetPrim <= 0
                    ).ToList();
                    return list;
                }
                else if (iptalZeylTahakkuk == PrimTipleri.Tahakkuk)
                {
                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                                    p => p.TVMKodu == aktifTVMKodu
                                        && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                                        && p.TaliAcenteKodu == null
                                        && p.TaliKomisyon == null
                                        && _branslar.Contains(p.BransKodu ?? 0)
                                        && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                                        && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                                        && p.NetPrim > 0
                                        ).ToList();
                    return list;
                }
                else
                {
                    return null;
                }

            }
            else if (durum == TaliKomisyonHesaplamaPoliceDurumu.Hesaplanmis)
            {
                if (iptalZeylTahakkuk == PrimTipleri.IptalZeyilleri)
                {
                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                  p => p.TVMKodu == aktifTVMKodu
                      && _taliAcenteler.Contains(p.TaliAcenteKodu ?? 0)
                      && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                      && p.TaliKomisyon != null
                      && _branslar.Contains(p.BransKodu ?? 0)
                      && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                      && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                      && p.NetPrim <= 0
                      ).ToList();
                    return list;
                }
                else if (iptalZeylTahakkuk == PrimTipleri.Tahakkuk)
                {
                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                 p => p.TVMKodu == aktifTVMKodu
                     && _taliAcenteler.Contains(p.TaliAcenteKodu ?? 0)
                     && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                     && p.TaliKomisyon != null
                     && _branslar.Contains(p.BransKodu ?? 0)
                     && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                     && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                     && p.NetPrim > 0
                     ).ToList();
                    return list;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (iptalZeylTahakkuk == PrimTipleri.IptalZeyilleri)
                {
                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                            p => p.TVMKodu == aktifTVMKodu
                                && _taliAcenteler.Contains(p.TaliAcenteKodu ?? 0)
                                 && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                                && _branslar.Contains(p.BransKodu ?? 0)
                                && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                                && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                                && p.NetPrim <= 0
                                ).ToList();
                    return list;
                }
                else if (iptalZeylTahakkuk == PrimTipleri.Tahakkuk)
                {

                    var list = _KomisyonContext.PoliceGenelRepository.Filter(
                            p => p.TVMKodu == aktifTVMKodu
                                && _taliAcenteler.Contains(p.TaliAcenteKodu ?? 0)
                                && (_disKaynaklar.Count() > 0 ? _disKaynaklar.Contains(p.UretimTaliAcenteKodu ?? 0) : p.UretimTaliAcenteKodu == null)
                                && _branslar.Contains(p.BransKodu ?? 0)
                                && _sigortaSirketleri.Contains(p.TUMBirlikKodu)
                                && p.TanzimTarihi >= tarihBaslangic && p.TanzimTarihi <= tarihBitis
                                && p.NetPrim > 0
                                ).ToList();
                    return list;
                }
                else
                {
                    return null;
                }
            }
        }

        public decimal TaliAcenteKomisyonOrani(int taliTVMKodu, int bransKodu, string sigortaSirketKodu, DateTime gecerlilikTarihi, string policeNo)
        {

            PoliceGenel polGenel = new PoliceGenel();
            int ekno = 0;
            if (sigortaSirketKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA) //Allianz Sigorta Poliçesi İse
            {
                ekno = 1;
            }


            polGenel = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TUMBirlikKodu == sigortaSirketKodu
                                                              && s.PoliceNumarasi == policeNo
                                                              && s.EkNo == ekno).FirstOrDefault();

            if (polGenel == null)
            {
                polGenel = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TUMBirlikKodu == sigortaSirketKodu
                                                             && s.PoliceNumarasi == policeNo).FirstOrDefault();
            }

            if (polGenel != null)
            {
                //if (polGenel.TaliKomisyonOran.HasValue)
                //{
                //    var oran = 0;
                //    oran = Convert.ToInt32(polGenel.TaliKomisyonOran.Value);
                //    return oran;
                //}
                //else
                //{
                decimal GerceklesenTaliUretim = 0;
                GerceklesenTaliUretim = this.PoliceTVMGerceklesen(taliTVMKodu, gecerlilikTarihi.Year, bransKodu);
                TaliAcenteKomisyonOrani taliKomisyonOrani = new TaliAcenteKomisyonOrani();

                var orans = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(
                        o => o.TaliTVMKodu == taliTVMKodu
                            && (polGenel.UretimTaliAcenteKodu.HasValue ? o.DisKaynakKodu == polGenel.UretimTaliAcenteKodu : o.DisKaynakKodu == null)
                            && o.BransKodu == bransKodu
                            && o.SigortaSirketKodu == sigortaSirketKodu
                            && o.GecirlilikBaslangicTarihi <= gecerlilikTarihi
                        ).OrderByDescending(o => o.GecirlilikBaslangicTarihi).ToList();

                if (orans.Count > 1)
                {
                    foreach (var item in orans)
                    {
                        if (item.MaxUretim != null && item.MinUretim != null)
                        {
                            if (GerceklesenTaliUretim > 0 && orans.Count > 0)
                            {
                                taliKomisyonOrani = orans.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                            }
                            return taliKomisyonOrani == null ? 0 : (taliKomisyonOrani.KomisyonOran ?? 0);
                        }
                        else
                        {
                            var oran = orans.OrderByDescending(s => s.GecirlilikBaslangicTarihi).FirstOrDefault();
                            return oran == null ? 0 : (oran.KomisyonOran ?? 0);
                        }
                    }
                }
                else
                {
                    var oran = orans.OrderByDescending(s => s.GecirlilikBaslangicTarihi).FirstOrDefault();
                    return oran == null ? 0 : (oran.KomisyonOran ?? 0);
                }
            }
            //}

            return 0;
        }

        public PoliceGenel TeklifGenelKomisyonGuncelle(int teklifGenelId, int? taliTVMKodu, int oncekiTaliKodu, decimal komisyonOrani, decimal komisyon, out bool guncellendiMi)
        {
            guncellendiMi = true;
            var kayit = new PoliceGenel();
            try
            {
                kayit = _KomisyonContext.PoliceGenelRepository.Find(p => p.PoliceId == teklifGenelId);
                kayit.TaliAcenteKodu = taliTVMKodu;
                if (komisyonOrani>100)
                {
                    if (komisyonOrani.ToString().Length==4)
                    {
                        komisyonOrani = komisyonOrani / 100;
                    }
                    if (komisyonOrani.ToString().Length ==3 )
                    {
                        komisyonOrani = komisyonOrani / 10;
                    }
                }

                kayit.TaliKomisyonOran = komisyonOrani;
                kayit.TaliKomisyon = komisyon;
                kayit.TaliKomisyonGuncellemeTarihi = DateTime.Now;
                kayit.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                _KomisyonContext.PoliceGenelRepository.Update(kayit);
                _KomisyonContext.Commit();

                bool gerceklesenHedefGuncellendiMi = PoliceUretimHedefGerceklesenGuncelle(kayit, oncekiTaliKodu);
                if (!gerceklesenHedefGuncellendiMi)
                {
                    guncellendiMi = false;
                }

                if (komisyon > 0)
                {
                    // p.PoliceId != teklifGenelId zeyl listesine poliçe gelmesin diye bu kontrol eklendi
                    var policeninZeyliVarMi = _KomisyonContext.PoliceGenelRepository.All().Where(p => p.PoliceNumarasi == kayit.PoliceNumarasi && p.TVMKodu == kayit.TVMKodu && p.TUMBirlikKodu == kayit.TUMBirlikKodu && p.PoliceId!= teklifGenelId).ToList();
                    foreach (var itemZeyl in policeninZeyliVarMi)
                    {
                        itemZeyl.TaliAcenteKodu = taliTVMKodu;
                        itemZeyl.TaliKomisyonOran = komisyonOrani;
                        itemZeyl.TaliKomisyon = (itemZeyl.Komisyon * komisyonOrani) / 100;
                        itemZeyl.TaliKomisyonGuncellemeTarihi = DateTime.Now;
                        itemZeyl.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                        _KomisyonContext.PoliceGenelRepository.Update(itemZeyl);
                        _KomisyonContext.Commit();

                        bool gerceklesenUretimHedefGuncellendiMi = PoliceUretimHedefGerceklesenGuncelle(itemZeyl, oncekiTaliKodu);
                        if (!gerceklesenUretimHedefGuncellendiMi)
                        {
                            guncellendiMi = false;
                        }
                    }
                }
                return kayit;
            }
            catch (Exception)
            {
                guncellendiMi = false;
            }
            return kayit;
        }

        public List<TaliAcenteKomisyonOrani> GetTaliAcenteKomisyon(int anaTVMKodu, int taliTVMKodu, int? disKaynakKodu, string TUMBirlikKodu, int bransKodu)
        {
            var komisyonOranlari = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o => o.TVMKodu == anaTVMKodu &&
                                                                      ((taliTVMKodu == 0 && o.TaliTVMKodu == null) || (taliTVMKodu != 0 && o.TaliTVMKodu == taliTVMKodu)) &&
                                                                      (((disKaynakKodu == 0 || disKaynakKodu==null) && o.DisKaynakKodu == null) || (disKaynakKodu != null && o.DisKaynakKodu == disKaynakKodu)) &&
                                                                      o.SigortaSirketKodu == TUMBirlikKodu &&
                                                                      o.BransKodu == bransKodu).OrderByDescending(o => o.GecirlilikBaslangicTarihi).ToList();
            List<TaliAcenteKomisyonOrani> listOran = new List<TaliAcenteKomisyonOrani>();
            if (komisyonOranlari != null)
            {
                TaliAcenteKomisyonOrani newOran = new TaliAcenteKomisyonOrani();
                for (int i = 0; i < komisyonOranlari.Count(); i++)
                {
                    if (komisyonOranlari[0].GecirlilikBaslangicTarihi == komisyonOranlari[i].GecirlilikBaslangicTarihi)
                    {
                        newOran = new TaliAcenteKomisyonOrani();
                        newOran.TVMKodu = komisyonOranlari[i].TVMKodu;
                        newOran.BransKodu = komisyonOranlari[i].BransKodu;
                        newOran.GecirlilikBaslangicTarihi = komisyonOranlari[i].GecirlilikBaslangicTarihi;
                        newOran.GuncellemeKullaniciKodu = komisyonOranlari[i].GuncellemeKullaniciKodu;
                        newOran.GuncellemeTarihi = komisyonOranlari[i].GuncellemeTarihi;
                        newOran.KayitKullaniciKodu = komisyonOranlari[i].KayitKullaniciKodu;
                        newOran.KayitTarihi = komisyonOranlari[i].KayitTarihi;
                        //newOran.KomisyonOrani = komisyonOranlari[i].KomisyonOrani;
                        newOran.KomisyonOran = komisyonOranlari[i].KomisyonOran;
                        newOran.KomisyonOranId = komisyonOranlari[i].KomisyonOranId;
                        newOran.MaxUretim = komisyonOranlari[i].MaxUretim;
                        newOran.MinUretim = komisyonOranlari[i].MinUretim;
                        newOran.SigortaSirketKodu = komisyonOranlari[i].SigortaSirketKodu;
                        newOran.TaliTVMKodu = komisyonOranlari[i].TaliTVMKodu;
                        newOran.DisKaynakKodu = komisyonOranlari[i].DisKaynakKodu;
                        listOran.Add(newOran);
                    }
                }
            }
            return listOran;
        }

        public List<TaliAcenteKomisyonOrani> TaliAcenteKademeliKomisyonListesi(int komisyonOranId)
        {
            var _kayit = new TaliAcenteKomisyonOrani();
            try
            {
                _kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Find(
                    o => o.KomisyonOranId == komisyonOranId);
                var kayit = _KomisyonContext.TaliAcenteKomisyonOraniRepository.Filter(o =>
                             o.TVMKodu == _kayit.TVMKodu &&
                             o.TaliTVMKodu == _kayit.TaliTVMKodu &&
                             o.SigortaSirketKodu == _kayit.SigortaSirketKodu &&
                             o.BransKodu == _kayit.BransKodu &&
                             o.GecirlilikBaslangicTarihi == _kayit.GecirlilikBaslangicTarihi &&
                             o.MinUretim != null &&
                             o.MaxUretim != null
                             ).OrderBy(o => o.MinUretim).ToList();
                return kayit;
            }
            catch
            {
                return new List<TaliAcenteKomisyonOrani>();
            }
        }

        public decimal PoliceTVMGerceklesen(int tvmKoduTali, int donem, int bransKodu)
        {
            decimal taliToplamUretim = 0;
            PoliceUretimHedefGerceklesen taliUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Find(s => s.TVMKoduTali == tvmKoduTali &&
                                                                                          s.Donem == donem &&
                                                                                          s.BransKodu == bransKodu);

            if (taliUretim != null)
            {
                taliToplamUretim = (taliUretim.Prim1.HasValue ? taliUretim.Prim1.Value : 0) + (taliUretim.Prim2.HasValue ? taliUretim.Prim2.Value : 0) +
                              (taliUretim.Prim3.HasValue ? taliUretim.Prim3.Value : 0) + (taliUretim.Prim4.HasValue ? taliUretim.Prim4.Value : 0) +
                              (taliUretim.Prim5.HasValue ? taliUretim.Prim5.Value : 0) + (taliUretim.Prim6.HasValue ? taliUretim.Prim6.Value : 0) +
                              (taliUretim.Prim7.HasValue ? taliUretim.Prim7.Value : 0) + (taliUretim.Prim8.HasValue ? taliUretim.Prim8.Value : 0) +
                              (taliUretim.Prim9.HasValue ? taliUretim.Prim9.Value : 0) + (taliUretim.Prim10.HasValue ? taliUretim.Prim10.Value : 0) +
                              (taliUretim.Prim11.HasValue ? taliUretim.Prim11.Value : 0) + (taliUretim.Prim12.HasValue ? taliUretim.Prim12.Value : 0);
            }

            return taliToplamUretim;
        }

        public bool PoliceUretimHedefGerceklesenGuncelle(PoliceGenel police, int oncekiTaliKodu)
        {
            bool guncellendiMi = true;
            bool uretimYeniMiEkleniyor = false;
            PoliceUretimHedefGerceklesen gerceklesenUretimPolice = new PoliceUretimHedefGerceklesen();
            PoliceUretimHedefGerceklesen gerceklesenUretimTaliGuncelle = new PoliceUretimHedefGerceklesen();

            if (oncekiTaliKodu > 0)
            {
                gerceklesenUretimPolice = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == oncekiTaliKodu &&
                                                                                   s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();
            }
            else
            {
                gerceklesenUretimPolice = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == police.TVMKodu &&
                                                                                  s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();
            }

            gerceklesenUretimTaliGuncelle = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == police.TaliAcenteKodu &&
                                                                         s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();

            #region Gerceklesen Uretim Guncelleniyor
            if (gerceklesenUretimPolice != null)
            {
                if (gerceklesenUretimTaliGuncelle == null)
                {
                    gerceklesenUretimTaliGuncelle = new PoliceUretimHedefGerceklesen();
                    uretimYeniMiEkleniyor = true;
                }
                // Tali acentekodu belli olan ve komisyon bilgileri güncellenen poliçe hangi aya ait ise Policeüretim hedef gerceklesen tablosundan siliniyor
                if (police.TanzimTarihi.Value.Month == 1) //Ocak
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi1 = gerceklesenUretimPolice.PoliceAdedi1.HasValue ? (gerceklesenUretimPolice.PoliceAdedi1.Value - 1) : gerceklesenUretimPolice.PoliceAdedi1;
                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi1 = gerceklesenUretimTaliGuncelle.PoliceAdedi1.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi1.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi1;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi1 = gerceklesenUretimPolice.PoliceAdedi1.HasValue ? (gerceklesenUretimPolice.PoliceAdedi1.Value - 1) : gerceklesenUretimPolice.PoliceAdedi1.Value;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi1 = gerceklesenUretimTaliGuncelle.PoliceAdedi1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceAdedi1.Value + 1 : gerceklesenUretimTaliGuncelle.PoliceAdedi1;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi1 = gerceklesenUretimPolice.PoliceAdedi1.HasValue ? (gerceklesenUretimPolice.PoliceAdedi1.Value - 1) : gerceklesenUretimPolice.PoliceAdedi1.Value;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi1 = gerceklesenUretimTaliGuncelle.PoliceAdedi1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceAdedi1.Value + 1 : gerceklesenUretimTaliGuncelle.PoliceAdedi1;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari1 = gerceklesenUretimPolice.PoliceKomisyonTutari1.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari1.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari1;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari1 = gerceklesenUretimPolice.VerilenKomisyonTutari1.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari1.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari1;
                    }
                    gerceklesenUretimPolice.Prim1 = gerceklesenUretimPolice.Prim1.HasValue ? (gerceklesenUretimPolice.Prim1.Value - police.NetPrim) : gerceklesenUretimPolice.Prim1;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim1 = gerceklesenUretimTaliGuncelle.Prim1.HasValue ? gerceklesenUretimTaliGuncelle.Prim1.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim1 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }

                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 2)//Şubat
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi2 = gerceklesenUretimPolice.PoliceAdedi2.HasValue ? (gerceklesenUretimPolice.PoliceAdedi2.Value - 1) : gerceklesenUretimPolice.PoliceAdedi2;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi2 = gerceklesenUretimTaliGuncelle.PoliceAdedi2.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi2.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi2;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi2 = gerceklesenUretimPolice.PoliceAdedi2.HasValue ? (gerceklesenUretimPolice.PoliceAdedi2.Value - 1) : gerceklesenUretimPolice.PoliceAdedi2;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi2 = gerceklesenUretimTaliGuncelle.PoliceAdedi2.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi2.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi2;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi2 = gerceklesenUretimPolice.PoliceAdedi2.HasValue ? (gerceklesenUretimPolice.PoliceAdedi2.Value - 1) : gerceklesenUretimPolice.PoliceAdedi2;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi2 = gerceklesenUretimTaliGuncelle.PoliceAdedi2.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi2.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi2;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari2 = gerceklesenUretimPolice.PoliceKomisyonTutari2.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari2.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari2;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari2 = gerceklesenUretimPolice.VerilenKomisyonTutari2.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari2.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari2;
                    }
                    gerceklesenUretimPolice.Prim2 = gerceklesenUretimPolice.Prim2.HasValue ? (gerceklesenUretimPolice.Prim2.Value - police.NetPrim) : gerceklesenUretimPolice.Prim2;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim2 = gerceklesenUretimTaliGuncelle.Prim2.HasValue ? gerceklesenUretimTaliGuncelle.Prim2.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim2 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 3)//Mart
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi3 = gerceklesenUretimPolice.PoliceAdedi3.HasValue ? (gerceklesenUretimPolice.PoliceAdedi3.Value - 1) : gerceklesenUretimPolice.PoliceAdedi3;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi3 = gerceklesenUretimTaliGuncelle.PoliceAdedi3.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi3.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi3;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi3 = gerceklesenUretimPolice.PoliceAdedi3.HasValue ? (gerceklesenUretimPolice.PoliceAdedi3.Value - 1) : gerceklesenUretimPolice.PoliceAdedi3;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi3 = gerceklesenUretimTaliGuncelle.PoliceAdedi3.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi3.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi3;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi3 = gerceklesenUretimPolice.PoliceAdedi3.HasValue ? (gerceklesenUretimPolice.PoliceAdedi3.Value - 1) : gerceklesenUretimPolice.PoliceAdedi3;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi3 = gerceklesenUretimTaliGuncelle.PoliceAdedi3.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi3.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi3;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari3 = gerceklesenUretimPolice.PoliceKomisyonTutari3.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari3.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari3;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari3 = gerceklesenUretimPolice.VerilenKomisyonTutari3.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari3.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari3;
                    }
                    gerceklesenUretimPolice.Prim3 = gerceklesenUretimPolice.Prim3.HasValue ? (gerceklesenUretimPolice.Prim3.Value - police.NetPrim) : gerceklesenUretimPolice.Prim3;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim3 = gerceklesenUretimTaliGuncelle.Prim3.HasValue ? gerceklesenUretimTaliGuncelle.Prim3.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim3 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 4)//Nisan
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi4 = gerceklesenUretimPolice.PoliceAdedi4.HasValue ? (gerceklesenUretimPolice.PoliceAdedi4.Value - 1) : gerceklesenUretimPolice.PoliceAdedi4;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi4 = gerceklesenUretimTaliGuncelle.PoliceAdedi4.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi4.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi4;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi4 = gerceklesenUretimPolice.PoliceAdedi4.HasValue ? (gerceklesenUretimPolice.PoliceAdedi4.Value - 1) : gerceklesenUretimPolice.PoliceAdedi4;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi4 = gerceklesenUretimTaliGuncelle.PoliceAdedi4.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi4.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi4;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi4 = gerceklesenUretimPolice.PoliceAdedi4.HasValue ? (gerceklesenUretimPolice.PoliceAdedi4.Value - 1) : gerceklesenUretimPolice.PoliceAdedi4;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi4 = gerceklesenUretimTaliGuncelle.PoliceAdedi4.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi4.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi4;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari4 = gerceklesenUretimPolice.PoliceKomisyonTutari4 - police.Komisyon;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari4 = gerceklesenUretimPolice.VerilenKomisyonTutari4.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari4.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari4;
                    }
                    gerceklesenUretimPolice.Prim4 = gerceklesenUretimPolice.Prim4.HasValue ? (gerceklesenUretimPolice.Prim4.Value - police.NetPrim) : gerceklesenUretimPolice.Prim4;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim4 = gerceklesenUretimTaliGuncelle.Prim4.HasValue ? gerceklesenUretimTaliGuncelle.Prim4.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim4 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 5)//Mayıs
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi5 = gerceklesenUretimPolice.PoliceAdedi5.HasValue ? (gerceklesenUretimPolice.PoliceAdedi5.Value - 1) : gerceklesenUretimPolice.PoliceAdedi5;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi5 = gerceklesenUretimTaliGuncelle.PoliceAdedi5.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi5.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi5;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi5 = gerceklesenUretimPolice.PoliceAdedi5.HasValue ? (gerceklesenUretimPolice.PoliceAdedi5.Value - 1) : gerceklesenUretimPolice.PoliceAdedi5;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi5 = gerceklesenUretimTaliGuncelle.PoliceAdedi5.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi5.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi5;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi5 = gerceklesenUretimPolice.PoliceAdedi5.HasValue ? (gerceklesenUretimPolice.PoliceAdedi5.Value - 1) : gerceklesenUretimPolice.PoliceAdedi5;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi5 = gerceklesenUretimTaliGuncelle.PoliceAdedi5.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi5.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi5;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari5 = gerceklesenUretimPolice.PoliceKomisyonTutari5.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari5.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari5;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari5 = gerceklesenUretimPolice.VerilenKomisyonTutari5.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari5.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari5;
                    }
                    gerceklesenUretimPolice.Prim5 = gerceklesenUretimPolice.Prim5.HasValue ? (gerceklesenUretimPolice.Prim5.Value - police.NetPrim) : gerceklesenUretimPolice.Prim5;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim5 = gerceklesenUretimTaliGuncelle.Prim5.HasValue ? gerceklesenUretimTaliGuncelle.Prim5.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim5 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 6)//Haziran
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi6 = gerceklesenUretimPolice.PoliceAdedi6.HasValue ? (gerceklesenUretimPolice.PoliceAdedi6.Value - 1) : gerceklesenUretimPolice.PoliceAdedi6;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi6 = gerceklesenUretimTaliGuncelle.PoliceAdedi6.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi6.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi6;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi6 = gerceklesenUretimPolice.PoliceAdedi6.HasValue ? (gerceklesenUretimPolice.PoliceAdedi6.Value - 1) : gerceklesenUretimPolice.PoliceAdedi6;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi6 = gerceklesenUretimTaliGuncelle.PoliceAdedi6.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi6.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi6;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi6 = gerceklesenUretimPolice.PoliceAdedi6.HasValue ? (gerceklesenUretimPolice.PoliceAdedi6.Value - 1) : gerceklesenUretimPolice.PoliceAdedi6;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi6 = gerceklesenUretimTaliGuncelle.PoliceAdedi6.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi6.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi6;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari6 = gerceklesenUretimPolice.PoliceKomisyonTutari6.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari6.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari6;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari6 = gerceklesenUretimPolice.VerilenKomisyonTutari6.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari6.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari6;
                    }
                    gerceklesenUretimPolice.Prim6 = gerceklesenUretimPolice.Prim6.HasValue ? (gerceklesenUretimPolice.Prim6.Value - police.NetPrim) : gerceklesenUretimPolice.Prim6;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim6 = gerceklesenUretimTaliGuncelle.Prim6.HasValue ? gerceklesenUretimTaliGuncelle.Prim6.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim6 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 7)//Temmuz
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi7 = gerceklesenUretimPolice.PoliceAdedi7.HasValue ? (gerceklesenUretimPolice.PoliceAdedi7.Value - 1) : gerceklesenUretimPolice.PoliceAdedi7;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi7 = gerceklesenUretimTaliGuncelle.PoliceAdedi7.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi7.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi7;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi7 = gerceklesenUretimPolice.PoliceAdedi7.HasValue ? (gerceklesenUretimPolice.PoliceAdedi7.Value - 1) : gerceklesenUretimPolice.PoliceAdedi7;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi7 = gerceklesenUretimTaliGuncelle.PoliceAdedi7.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi7.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi7;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi7 = gerceklesenUretimPolice.PoliceAdedi7.HasValue ? (gerceklesenUretimPolice.PoliceAdedi7.Value - 1) : gerceklesenUretimPolice.PoliceAdedi7;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi7 = gerceklesenUretimTaliGuncelle.PoliceAdedi7.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi7.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi7;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari7 = gerceklesenUretimPolice.PoliceKomisyonTutari7.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari7.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari7;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari7 = gerceklesenUretimPolice.VerilenKomisyonTutari7.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari7.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari7;
                    }
                    gerceklesenUretimPolice.Prim7 = gerceklesenUretimPolice.Prim7.HasValue ? (gerceklesenUretimPolice.Prim7.Value - police.NetPrim) : gerceklesenUretimPolice.Prim7;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim7 = gerceklesenUretimTaliGuncelle.Prim7.HasValue ? gerceklesenUretimTaliGuncelle.Prim7.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim7 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 8)//Ağustos
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi8 = gerceklesenUretimPolice.PoliceAdedi8.HasValue ? (gerceklesenUretimPolice.PoliceAdedi8.Value - 1) : gerceklesenUretimPolice.PoliceAdedi8;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi8 = gerceklesenUretimPolice.PoliceAdedi8.HasValue ? (gerceklesenUretimPolice.PoliceAdedi8.Value - 1) : gerceklesenUretimPolice.PoliceAdedi8;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 != null && !uretimYeniMiEkleniyor)
                        {
                            gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                        }
                        else
                        {
                            gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi8 = gerceklesenUretimPolice.PoliceAdedi8.HasValue ? (gerceklesenUretimPolice.PoliceAdedi8.Value - 1) : gerceklesenUretimPolice.PoliceAdedi8;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 != null && !uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari8 = gerceklesenUretimPolice.PoliceKomisyonTutari8.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari8.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari8;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari8 = gerceklesenUretimPolice.VerilenKomisyonTutari8.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari8.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari8;
                    }
                    gerceklesenUretimPolice.Prim8 = gerceklesenUretimPolice.Prim8.HasValue ? (gerceklesenUretimPolice.Prim8.Value - police.NetPrim) : gerceklesenUretimPolice.Prim8;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim8 = gerceklesenUretimTaliGuncelle.Prim8.HasValue ? gerceklesenUretimTaliGuncelle.Prim8.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim8 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 9)//Eylül
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi9 = gerceklesenUretimPolice.PoliceAdedi9.HasValue ? (gerceklesenUretimPolice.PoliceAdedi9.Value - 1) : gerceklesenUretimPolice.PoliceAdedi9;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi9 = gerceklesenUretimTaliGuncelle.PoliceAdedi9.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi9.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi9;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi9 = gerceklesenUretimPolice.PoliceAdedi9.HasValue ? (gerceklesenUretimPolice.PoliceAdedi9.Value - 1) : gerceklesenUretimPolice.PoliceAdedi9;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi9 = gerceklesenUretimTaliGuncelle.PoliceAdedi9.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi9.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi9;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi9 = gerceklesenUretimPolice.PoliceAdedi9.HasValue ? (gerceklesenUretimPolice.PoliceAdedi9.Value - 1) : gerceklesenUretimPolice.PoliceAdedi9;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi9 = gerceklesenUretimTaliGuncelle.PoliceAdedi9.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi9.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi9;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari9 = gerceklesenUretimPolice.PoliceKomisyonTutari9.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari9.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari9;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari9 = gerceklesenUretimPolice.VerilenKomisyonTutari9.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari9.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari9;
                    }
                    gerceklesenUretimPolice.Prim9 = gerceklesenUretimPolice.Prim9.HasValue ? (gerceklesenUretimPolice.Prim9.Value - police.NetPrim) : gerceklesenUretimPolice.Prim9;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim9 = gerceklesenUretimTaliGuncelle.Prim9.HasValue ? gerceklesenUretimTaliGuncelle.Prim9.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim9 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 10)//Ekim
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi10 = gerceklesenUretimPolice.PoliceAdedi10.HasValue ? (gerceklesenUretimPolice.PoliceAdedi10.Value - 1) : gerceklesenUretimPolice.PoliceAdedi10;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi10 = gerceklesenUretimTaliGuncelle.PoliceAdedi10.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi10.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi10;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi10 = gerceklesenUretimPolice.PoliceAdedi10.HasValue ? (gerceklesenUretimPolice.PoliceAdedi10.Value - 1) : gerceklesenUretimPolice.PoliceAdedi10;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi10 = gerceklesenUretimTaliGuncelle.PoliceAdedi10.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi10.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi10;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi10 = gerceklesenUretimPolice.PoliceAdedi10.HasValue ? (gerceklesenUretimPolice.PoliceAdedi10.Value - 1) : gerceklesenUretimPolice.PoliceAdedi10;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi10 = gerceklesenUretimTaliGuncelle.PoliceAdedi10.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi10.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi10;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari10 = gerceklesenUretimPolice.PoliceKomisyonTutari10.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari10.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari10;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari10 = gerceklesenUretimPolice.VerilenKomisyonTutari10.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari10.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari10;
                    }
                    gerceklesenUretimPolice.Prim10 = gerceklesenUretimPolice.Prim10.HasValue ? (gerceklesenUretimPolice.Prim10.Value - police.NetPrim) : gerceklesenUretimPolice.Prim10;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim10 = gerceklesenUretimTaliGuncelle.Prim10.HasValue ? gerceklesenUretimTaliGuncelle.Prim10.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim10 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 11)//Kasım
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi11 = gerceklesenUretimPolice.PoliceAdedi11.HasValue ? (gerceklesenUretimPolice.PoliceAdedi11.Value - 1) : gerceklesenUretimPolice.PoliceAdedi11;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi11 = gerceklesenUretimTaliGuncelle.PoliceAdedi11.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi11.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi11;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi11 = gerceklesenUretimPolice.PoliceAdedi11.HasValue ? (gerceklesenUretimPolice.PoliceAdedi11.Value - 1) : gerceklesenUretimPolice.PoliceAdedi11;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi11 = gerceklesenUretimTaliGuncelle.PoliceAdedi11.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi11.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi11;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi11 = gerceklesenUretimPolice.PoliceAdedi11.HasValue ? (gerceklesenUretimPolice.PoliceAdedi11.Value - 1) : gerceklesenUretimPolice.PoliceAdedi11;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi11 = gerceklesenUretimTaliGuncelle.PoliceAdedi11.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi11.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi11;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari11 = gerceklesenUretimPolice.PoliceKomisyonTutari11.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari11.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari11;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari11 = gerceklesenUretimPolice.VerilenKomisyonTutari11.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari11.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari11;
                    }
                    gerceklesenUretimPolice.Prim11 = gerceklesenUretimPolice.Prim11.HasValue ? (gerceklesenUretimPolice.Prim11.Value - police.NetPrim) : gerceklesenUretimPolice.Prim11;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim11 = gerceklesenUretimTaliGuncelle.Prim11.HasValue ? gerceklesenUretimTaliGuncelle.Prim11.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim11 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
                else if (police.TanzimTarihi.Value.Month == 12)//Aralık
                {
                    if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                    {
                        gerceklesenUretimPolice.PoliceAdedi12 = gerceklesenUretimPolice.PoliceAdedi12.HasValue ? (gerceklesenUretimPolice.PoliceAdedi12.Value - 1) : gerceklesenUretimPolice.PoliceAdedi12;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi12 = gerceklesenUretimTaliGuncelle.PoliceAdedi12.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi12.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi12;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                    {
                        gerceklesenUretimPolice.PoliceAdedi12 = gerceklesenUretimPolice.PoliceAdedi12.HasValue ? (gerceklesenUretimPolice.PoliceAdedi12.Value - 1) : gerceklesenUretimPolice.PoliceAdedi12;

                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 != null)
                        {
                            uretimYeniMiEkleniyor = false;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi12 = gerceklesenUretimTaliGuncelle.PoliceAdedi12.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi12.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi12;
                        }
                        else
                        {
                            uretimYeniMiEkleniyor = true;
                            gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 1;
                        }
                    }
                    else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                    {
                        if (police.EkNo.ToString().Substring(4, 1) == "1")
                        {
                            gerceklesenUretimPolice.PoliceAdedi12 = gerceklesenUretimPolice.PoliceAdedi12.HasValue ? (gerceklesenUretimPolice.PoliceAdedi12.Value - 1) : gerceklesenUretimPolice.PoliceAdedi12;

                            if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 != null)
                            {
                                uretimYeniMiEkleniyor = false;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi12 = gerceklesenUretimTaliGuncelle.PoliceAdedi12.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi12.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi12;
                            }
                            else
                            {
                                uretimYeniMiEkleniyor = true;
                                gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 1;
                            }
                        }
                    }
                    gerceklesenUretimPolice.PoliceKomisyonTutari12 = gerceklesenUretimPolice.PoliceKomisyonTutari12.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari12.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari12;
                    if (oncekiTaliKodu > 0)
                    {
                        gerceklesenUretimPolice.VerilenKomisyonTutari12 = gerceklesenUretimPolice.VerilenKomisyonTutari12.HasValue ? (gerceklesenUretimPolice.VerilenKomisyonTutari12.Value - police.TaliKomisyon) : gerceklesenUretimPolice.VerilenKomisyonTutari12;
                    }
                    gerceklesenUretimPolice.Prim12 = gerceklesenUretimPolice.Prim12.HasValue ? (gerceklesenUretimPolice.Prim12.Value - police.NetPrim) : gerceklesenUretimPolice.Prim12;

                    if (!uretimYeniMiEkleniyor)
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12.Value + police.Komisyon : police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12.Value + police.TaliKomisyon : police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim12 = gerceklesenUretimTaliGuncelle.Prim12.HasValue ? gerceklesenUretimTaliGuncelle.Prim12.Value + police.NetPrim : police.NetPrim;
                    }
                    else
                    {
                        gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12 = police.Komisyon;
                        gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12 = police.TaliKomisyon;
                        gerceklesenUretimTaliGuncelle.TVMKoduTali = police.TaliAcenteKodu;
                        gerceklesenUretimTaliGuncelle.Prim12 = police.NetPrim;
                        gerceklesenUretimTaliGuncelle.BransKodu = police.BransKodu;
                        gerceklesenUretimTaliGuncelle.Donem = police.TanzimTarihi.Value.Year;
                        gerceklesenUretimTaliGuncelle.TVMKodu = police.TVMKodu.HasValue ? police.TVMKodu.Value : 0;
                    }

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimPolice);
                    _KomisyonContext.Commit();
                    if (!uretimYeniMiEkleniyor)
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    else
                    {
                        _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretimTaliGuncelle);
                        _KomisyonContext.Commit();
                    }
                    guncellendiMi = true;
                }
            }
            #endregion

            return guncellendiMi;
        }

        //public bool GetGecerliYilTaliKomisyonVarMi(int gecerliYil, DateTime gecerliYil2,int taliAcenteKodu, bool kademeliMi)
        //{
        //    bool returnVal = false;
        //    if (kademeliMi)
        //    {
        //        DateTime date = new DateTime(gecerliYil, 1, 1);
        //       var result= _KomisyonContext.TaliAcenteKomisyonOraniRepository.All().Where(w => w.GecirlilikBaslangicTarihi == date && w.TaliTVMKodu == taliAcenteKodu);
        //       if (result!=null)
        //       {
        //           returnVal = true;
        //       }
        //       else
        //       {
        //           returnVal = false;
        //       }
        //    }
        //    else
        //    {
        //        var result = _KomisyonContext.TaliAcenteKomisyonOraniRepository.All().Where(w => w.GecirlilikBaslangicTarihi == gecerliYil2 && w.TaliTVMKodu == taliAcenteKodu);
        //        if (result != null)
        //        {
        //            returnVal = true;
        //        }
        //        else
        //        {
        //            returnVal = false;
        //        }
        //    }

        //    return false;
        //}
        #endregion
    }
}
