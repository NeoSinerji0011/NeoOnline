using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Neosinerji.BABOnlineTP.Business.TaliPolice
{
    public class TaliPoliceService : ITaliPoliceService
    {
        IKomisyonContext _KomisyonContext;
        ITVMContext _TVMContext;
        IPoliceContext _PoliceContext;

        public TaliPoliceService(IKomisyonContext komisyonContext, ITVMContext tvmContext, IPoliceContext policeContext)
        {
            _KomisyonContext = komisyonContext;
            _TVMContext = tvmContext;
            _PoliceContext = policeContext;
        }

        public bool CreatePoliceTaliAcenteler(PoliceTaliAcenteler model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceTaliAcentelerRepository.Create(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool CreatePoliceTaliAcenteRapor(PoliceTaliAcenteRapor model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceTaliAcenteRaporRepository.Create(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePoliceTaliAcenteler(PoliceTaliAcenteler model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceTaliAcentelerRepository.Update(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool UpdatePoliceTaliAcenteRapor(PoliceTaliAcenteRapor model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceTaliAcenteRaporRepository.Update(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool DeletePoliceTaliAcenteler(int id)
        {
            bool result;
            try
            {
                var taliPolice = _KomisyonContext.PoliceTaliAcentelerRepository.FindById(id);
                _KomisyonContext.PoliceTaliAcentelerRepository.Delete(s => s.Id == id);
                _KomisyonContext.Commit();

                if (taliPolice != null)
                {
                    PoliceTaliAcenteRapor taliRapor = new PoliceTaliAcenteRapor();
                    taliRapor = GetPoliceTaliAcenteRapor(taliPolice.TVMKodu.Value, taliPolice.KayitTarihi_.Value);
                    taliRapor.GuncellemeTarihi = TurkeyDateTime.Today;
                    taliRapor.Police_EkAdedi = taliRapor.Police_EkAdedi - 1;
                    if (taliRapor.Police_EkAdedi == 0)
                    {
                        taliRapor.UretimVAR_YOK = 0;
                    }
                    else
                    {
                        taliRapor.UretimVAR_YOK = 1;
                    }

                    _KomisyonContext.PoliceTaliAcenteRaporRepository.Update(taliRapor);
                    _KomisyonContext.Commit();

                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteListe(int tvmKodu)
        {
            return _KomisyonContext.PoliceTaliAcentelerRepository.Filter(s => s.TVMKodu == tvmKodu).ToList();
        }

        public PoliceTaliAcenteler GetPoliceTaliAcente(int tvmKodu, string policeNumarasi, string tumBirlikKodu)
        {
            var policeTaliAcente = _KomisyonContext.PoliceTaliAcentelerRepository.Filter(s => s.TVMKodu == tvmKodu && s.PoliceNo == policeNumarasi && s.SigortaSirketNo_ == tumBirlikKodu).FirstOrDefault();
            return policeTaliAcente;
        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteList(int tvmKodu, string policeNumarasi, string tumBirlikKodu)
        {
            var policeTaliAcente = _KomisyonContext.PoliceTaliAcentelerRepository.Filter(s => s.TVMKodu == tvmKodu && s.PoliceNo == policeNumarasi && s.SigortaSirketNo_ == tumBirlikKodu).ToList();
            return policeTaliAcente;
        }

        public PoliceTaliAcenteler GetPoliceTaliAcente(int id)
        {
            return _KomisyonContext.PoliceTaliAcentelerRepository.FindById(id);
        }

        public PoliceTaliAcenteRapor GetPoliceTaliAcenteRapor(int tvmkodu)
        {
            return _KomisyonContext.PoliceTaliAcenteRaporRepository.Find(s => s.KayitTarihi >= DateTime.Today && s.TVMKodu == tvmkodu);
        }

        public PoliceTaliAcenteRapor GetPoliceTaliAcenteRapor(int tvmkodu, DateTime bordroKayitTarihi)
        {
            DateTime tarih1 = bordroKayitTarihi.AddDays(-1);
            DateTime tarih2 = bordroKayitTarihi.AddDays(1);

            return _KomisyonContext.PoliceTaliAcenteRaporRepository.Find(s => s.KayitTarihi > tarih1 && s.KayitTarihi < tarih2 && s.TVMKodu == tvmkodu);
        }

        public PoliceTaliAcenteRapor GetPoliceTaliAcenteRaporByDate(int tvmkodu, DateTime tarih)
        {
            return _KomisyonContext.PoliceTaliAcenteRaporRepository.Find(s => s.KayitTarihi == tarih && s.TVMKodu == tvmkodu);
        }

        public List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporGunlukListe(int tvmKodu)
        {

            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            return _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.GuncellemeTarihi >= DateTime.Today && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();

        }

        public List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporTarih(int tvmKodu, DateTime tarih)
        {
            DateTime tarih1 = tarih.AddDays(-1);
            DateTime tarih2 = tarih.AddDays(1);
            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            return _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.KayitTarihi > tarih1 && s.KayitTarihi < tarih2 && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
        }

        public List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporTarihAraligi(string[] tvmList, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            List<int> aranacakTvmler = new List<int>();
            int sayac = 0;
            foreach (var item in tvmList)
            {
                sayac++;
                aranacakTvmler.Add(Convert.ToInt32(item));
            }
            var policelerListesi = _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.KayitTarihi >= baslangicTarihi && s.KayitTarihi <= bitisTarihi && aranacakTvmler.Contains(s.TVMKodu ?? 0)).OrderBy(s => s.TVMKodu).ToList();

            return policelerListesi;
        }

        public List<PoliceTaliAcenteRapor> GetPoliceTaliAcenteRaporStringTarih(int tvmKodu, string tarih)
        {

            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            if (tarih.Length == 10)
            {
                int theYear = Convert.ToInt32(tarih.Substring(6, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(3, 2));
                int theDay = Convert.ToInt32(tarih.Substring(0, 2));
                return _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.KayitTarihi.Value.Day == theDay
                                                                                  && s.KayitTarihi.Value.Month == theMonth
                                                                                  && s.KayitTarihi.Value.Year == theYear
                                                                                  && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }
            else if (tarih.Length == 9)
            {
                int theYear = Convert.ToInt32(tarih.Substring(5, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(2, 2));
                int theDay = Convert.ToInt32(tarih.Substring(0, 1));
                return _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.KayitTarihi.Value.Day == theDay
                                                                                  && s.KayitTarihi.Value.Month == theMonth
                                                                                  && s.KayitTarihi.Value.Year == theYear
                                                                                  && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }

            else
            {
                int theYear = Convert.ToInt32(tarih.Substring(4, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(2, 1));
                int theDay = Convert.ToInt32(tarih.Substring(0, 1));
                return _KomisyonContext.PoliceTaliAcenteRaporRepository.All().Where(s => s.KayitTarihi.Value.Day == theDay
                                                                                  && s.KayitTarihi.Value.Month == theMonth
                                                                                  && s.KayitTarihi.Value.Year == theYear
                                                                                  && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }





        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteIslemTarih(int tvmKodu, DateTime basTarih)
        {
            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();
            return _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ == basTarih && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteIslemStringTarih(int tvmKodu, string tarih)
        {

            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            if (tarih.Length == 10)
            {
                int theYear = Convert.ToInt32(tarih.Substring(6, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(3, 2));
                int theDay = Convert.ToInt32(tarih.Substring(0, 2));

                return _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_.Value.Day == theDay
                                                                                    && s.KayitTarihi_.Value.Month == theMonth
                                                                                    && s.KayitTarihi_.Value.Year == theYear
                                                                                    && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }
            else if (tarih.Length == 9)
            {
                int theYear = Convert.ToInt32(tarih.Substring(5, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(2, 2));
                int theDay = Convert.ToInt32(tarih.Substring(0, 1));

                return _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_.Value.Day == theDay
                                                                                    && s.KayitTarihi_.Value.Month == theMonth
                                                                                    && s.KayitTarihi_.Value.Year == theYear
                                                                                    && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }
            else
            {
                int theYear = Convert.ToInt32(tarih.Substring(4, 4));
                int theMonth = Convert.ToInt32(tarih.Substring(2, 1));
                int theDay = Convert.ToInt32(tarih.Substring(0, 1));

                return _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_.Value.Day == theDay
                                                                                    && s.KayitTarihi_.Value.Month == theMonth
                                                                                    && s.KayitTarihi_.Value.Year == theYear
                                                                                    && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();
            }


        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteGunlukListe(int tvmKodu)
        {
            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            return _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ >= DateTime.Today && yetkiliTvmler.Contains(s.TVMKodu ?? 0)).ToList();

        }

        public PoliceTaliAcenteler TaliPoliceVarmi(string policeNo, int ekNo, string sigortaSirketi)
        {
            return _KomisyonContext.PoliceTaliAcentelerRepository.Find(s => s.PoliceNo == policeNo && s.SigortaSirketNo_ == sigortaSirketi && s.EkNo == ekNo);

        }

        public int GetTVMKodTaliPoliceler(string tumBirlikKodu, string policeNo, int ekNo)
        {

            try
            {
                var tvmKod = _KomisyonContext.PoliceTaliAcentelerRepository.Find(s => s.SigortaSirketNo_ == tumBirlikKodu && s.PoliceNo == policeNo && s.EkNo == ekNo).TVMKodu.Value;
                if (tumBirlikKodu == "045" && tvmKod != null)
                {
                    if (policeNo.Length == 13)
                    {
                        policeNo = policeNo.Remove(0, 3);
                    }
                    tvmKod = _KomisyonContext.PoliceTaliAcentelerRepository.Find(s => s.SigortaSirketNo_ == tumBirlikKodu && s.PoliceNo == policeNo && s.EkNo == ekNo).TVMKodu.Value;
                }
                if (tvmKod != null)
                {
                    return tvmKod;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }


        }

        public int GetTVMKodTaliPoliceler(string tumBirlikKodu, string policeNo)
        {
            try
            {
                var tvmKod = _KomisyonContext.PoliceTaliAcentelerRepository.Filter(s => s.SigortaSirketNo_ == tumBirlikKodu && s.PoliceNo == policeNo).FirstOrDefault();
                if (tumBirlikKodu == "045" && tvmKod == null)
                {
                    if (policeNo.Length == 16)
                    {
                        policeNo = policeNo.Remove(0, 3);
                    }
                    tvmKod = _KomisyonContext.PoliceTaliAcentelerRepository.Filter(s => s.SigortaSirketNo_ == tumBirlikKodu && s.PoliceNo == policeNo).FirstOrDefault();
                }
                if (tvmKod != null)
                {
                    return tvmKod.TVMKodu.Value;
                }
                return 0;
            }
            catch (Exception)
            {

                return 0;
            }


        }

        public List<TVMDetay> GetYetkiliTVM(int tvmKodu)
        {
            return _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).ToList();
        }

        public List<PoliceTaliAcenteler> GetPoliceTaliAcenteler(string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            List<int> aranacakTvmler = new List<int>();
            int sayac = 0;
            foreach (var item in tvmList)
            {
                sayac++;
                aranacakTvmler.Add(Convert.ToInt32(item));
            }
            List<PoliceTaliAcenteler> policelerListesi;
            if (sirketList != null && sirketList.ToList().Count > 0)
            {
                List<string> aranacakSirketler = new List<string>();
                int SirketSayac = 0;
                foreach (var item in sirketList)
                {
                    SirketSayac++;
                    aranacakSirketler.Add(item);
                }
                policelerListesi = _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ >= baslangicTarihi && s.KayitTarihi_ <= bitisTarihi && aranacakTvmler.Contains(s.TVMKodu ?? 0) && aranacakSirketler.Contains(s.SigortaSirketNo_) && s.PoliceTransferEslestimi == 0).OrderBy(s => s.TVMKodu).ToList();
            }
            else
            {
                policelerListesi = _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ >= baslangicTarihi && s.KayitTarihi_ <= bitisTarihi && aranacakTvmler.Contains(s.TVMKodu ?? 0) && s.PoliceTransferEslestimi == 0).OrderBy(s => s.TVMKodu).ToList();
            }

            return policelerListesi;
        }

        public List<PoliceTaliAcenteler> GetPoliceBordroList(string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            List<int> aranacakTvmler = new List<int>();
            int sayac = 0;
            foreach (var item in tvmList)
            {
                sayac++;
                aranacakTvmler.Add(Convert.ToInt32(item));
            }
            List<PoliceTaliAcenteler> policelerListesi;
            if (sirketList != null && sirketList.ToList().Count > 0)
            {
                List<string> aranacakSirketler = new List<string>();
                int SirketSayac = 0;
                foreach (var item in sirketList)
                {
                    SirketSayac++;
                    aranacakSirketler.Add(item);
                }
                policelerListesi = _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ >= baslangicTarihi && s.KayitTarihi_ <= bitisTarihi && aranacakTvmler.Contains(s.TVMKodu ?? 0) && aranacakSirketler.Contains(s.SigortaSirketNo_)).OrderBy(s => s.TVMKodu).ToList();
            }
            else
            {
                policelerListesi = _KomisyonContext.PoliceTaliAcentelerRepository.All().Where(s => s.KayitTarihi_ >= baslangicTarihi && s.KayitTarihi_ <= bitisTarihi && aranacakTvmler.Contains(s.TVMKodu ?? 0)).OrderBy(s => s.TVMKodu).ToList();
            }

            return policelerListesi;
        }

        public List<PoliceGenel> GetPoliceGenelEslesmeyen(int tvmKodu, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            var policelerListesi = _KomisyonContext.PoliceGenelRepository.All().Where(s => s.TanzimTarihi >= baslangicTarihi && s.TanzimTarihi <= bitisTarihi && s.TVMKodu == tvmKodu && s.TaliAcenteKodu == null).OrderBy(s => s.TVMKodu).ToList();

            return policelerListesi;
        }

        public bool CreatePaylasimliPoliceUretim(PaylasimliPoliceUretim model)
        {
            bool result;
            try
            {
                _PoliceContext.PaylasimliPoliceUretimRepository.Create(model);
                _PoliceContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public PaylasimliPoliceUretim PaylasimliPoliceUretimVarMi(int tvmkodu, int? taliTvmKodu, string policeNo, string yenilemeNo, string ekNo, string sigortaSirketi)
        {
            return _PoliceContext.PaylasimliPoliceUretimRepository.Find(s => s.TVMKodu == tvmkodu && s.TaliTVMKodu == taliTvmKodu && s.PoliceNo == policeNo && s.SigortaSirketNo == sigortaSirketi && s.YenilemeNo == yenilemeNo && s.ZeylNo == ekNo);

        }

        //Poliçe Onaylama için kullanılıyor
        public List<PoliceGenel> GetPoliceGenelListesi(int merkezTVMKodu, int tvmKodu, int? disKaynakKodu, string sigortaSirketKodu, string policeNo, string tcVkn, string Plaka)
        {
            string plakaKodu1 = null;
            string plakaNo1 = null;
            string plakaKodu2 = null;
            string plakaNo2 = null;
            string plakaKodu3 = null;
            string plakaNo3 = null;
            if (Plaka != null)
            {
                var res1 = Plaka.Trim();
                var res2 = res1.Replace(" ", String.Empty);
                Regex re = new Regex(@"[A-Za-z]");
                Match m = re.Match(res2);
                if (m.Success)
                {
                    plakaKodu1 = res2.Substring(0, m.Index);
                    int plakaUzunluk = res2.Length - m.Index;
                    plakaNo1 = res2.Substring(m.Index, plakaUzunluk).ToLower().Replace('ı', 'i');
                }
                else
                {
                    plakaKodu2 = res2.Substring(0, 2);
                    int plakaUzunluk = res2.Length - 2;
                    plakaNo2 = res2.Substring(2, plakaUzunluk).ToLower().Replace('ı', 'i');

                    plakaKodu3 = res2.Substring(0, 3);
                    plakaUzunluk = res2.Length - 3;
                    plakaNo3 = res2.Substring(3, plakaUzunluk).ToLower().Replace('ı', 'i');
                }
            }
            List<PoliceGenel> polGenelList = new List<PoliceGenel>();

            polGenelList = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == merkezTVMKodu
                                                              && ((disKaynakKodu == null && s.UretimTaliAcenteKodu == null) || (disKaynakKodu != null && s.UretimTaliAcenteKodu == disKaynakKodu))
                                                              && s.TUMBirlikKodu == sigortaSirketKodu
                                                              && s.PoliceNumarasi == policeNo).OrderBy(s => s.EkNo).ToList();
            if (polGenelList != null)
            {
                foreach (var item in polGenelList)
                {
                    if (!String.IsNullOrEmpty(item.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(item.PoliceSigortaEttiren.VergiKimlikNo))
                    {
                        item.PoliceSigortali.KimlikNo = item.PoliceSigortaEttiren.KimlikNo;
                        item.PoliceSigortali.VergiKimlikNo = item.PoliceSigortaEttiren.VergiKimlikNo;
                        item.PoliceSigortali.AdiUnvan = item.PoliceSigortaEttiren.AdiUnvan;
                        item.PoliceSigortali.SoyadiUnvan = item.PoliceSigortaEttiren.SoyadiUnvan;
                    }
                }
                if (!String.IsNullOrEmpty(plakaKodu1) && !String.IsNullOrEmpty(plakaNo1) && !String.IsNullOrEmpty(tcVkn))
                {
                    polGenelList = polGenelList.Where(s => (s.PoliceSigortali.KimlikNo != null ? s.PoliceSigortali.KimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.KimlikNo == tcVkn.Trim() || s.PoliceSigortali.VergiKimlikNo != null ? s.PoliceSigortali.VergiKimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.VergiKimlikNo == tcVkn.Trim()) && s.PoliceArac.PlakaKodu.Trim() == plakaKodu1 && s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo1).ToList();
                }
                else if (!String.IsNullOrEmpty(plakaKodu2) && !String.IsNullOrEmpty(plakaNo2) && !String.IsNullOrEmpty(tcVkn))
                {
                    polGenelList = polGenelList.Where(s => (s.PoliceSigortali.KimlikNo != null ? s.PoliceSigortali.KimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.KimlikNo == tcVkn.Trim() || s.PoliceSigortali.VergiKimlikNo != null ? s.PoliceSigortali.VergiKimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.VergiKimlikNo == tcVkn.Trim()) && (s.PoliceArac.PlakaKodu.Trim() == plakaKodu2 || s.PoliceArac.PlakaKodu.Trim() == plakaKodu3) && (s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo2 || s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo3)).ToList();
                }
                else if (!String.IsNullOrEmpty(plakaKodu1) && !String.IsNullOrEmpty(plakaNo1))
                {
                    polGenelList = polGenelList.Where(s => s.PoliceArac.PlakaKodu.Trim() == plakaKodu1 && s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo1).ToList();
                }
                else if (!String.IsNullOrEmpty(plakaKodu2) && !String.IsNullOrEmpty(plakaNo2))
                {
                    polGenelList = polGenelList.Where(s => (s.PoliceArac.PlakaKodu.Trim() == plakaKodu2 || s.PoliceArac.PlakaKodu.Trim() == plakaKodu3) && (s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo2 || s.PoliceArac.PlakaNo.Trim().ToLower().Replace('ı', 'i').Replace(" ", String.Empty) == plakaNo3)).ToList();
                }
                else if (!String.IsNullOrEmpty(tcVkn))
                {
                    polGenelList = polGenelList.Where(s => (s.PoliceSigortali.KimlikNo != null ? s.PoliceSigortali.KimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.KimlikNo == tcVkn.Trim()) || (s.PoliceSigortali.VergiKimlikNo != null ? s.PoliceSigortali.VergiKimlikNo.Trim() == tcVkn.Trim() : s.PoliceSigortali.VergiKimlikNo == tcVkn.Trim())).ToList();
                }
            }
            return polGenelList;
        }

        public List<PoliceGenel> GetHesaplanmisPoliceGenelListesi(int merkezTVMKodu, string[] tvmList, string[] sirketList, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            List<PoliceGenel> polGenelList = new List<PoliceGenel>();

            List<int> aranacakTvmler = new List<int>();

            foreach (var item in tvmList)
            {
                if (item != "multiselect-all")
                {
                    aranacakTvmler.Add(Convert.ToInt32(item));
                }
            }

            if (sirketList != null && sirketList.ToList().Count > 0)
            {
                List<string> aranacakSirketler = new List<string>();

                foreach (var item in sirketList)
                {
                    if (item != "multiselect-all")
                    {
                        aranacakSirketler.Add(item);
                    }
                }
                polGenelList = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == merkezTVMKodu
                                                                           && aranacakTvmler.Contains(s.TaliAcenteKodu ?? 0)
                                                                           && aranacakSirketler.Contains(s.TUMBirlikKodu)
                                                                           && s.BaslangicTarihi >= baslangicTarihi
                                                                           && s.BaslangicTarihi <= bitisTarihi).OrderBy(s => s.TaliAcenteKodu).ToList();
            }

            if (polGenelList != null)
            {
                foreach (var item in polGenelList)
                {
                    if (item.PoliceSigortali == null)
                    {
                        if (item.PoliceSigortaEttiren != null)
                        {
                            if (!String.IsNullOrEmpty(item.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(item.PoliceSigortaEttiren.VergiKimlikNo))
                            {
                                item.PoliceSigortali = new PoliceSigortali();
                                item.PoliceSigortali.KimlikNo = item.PoliceSigortaEttiren.KimlikNo;
                                item.PoliceSigortali.VergiKimlikNo = item.PoliceSigortaEttiren.VergiKimlikNo;
                                item.PoliceSigortali.AdiUnvan = item.PoliceSigortaEttiren.AdiUnvan;
                                item.PoliceSigortali.SoyadiUnvan = item.PoliceSigortaEttiren.SoyadiUnvan;
                            }
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(item.PoliceSigortali.KimlikNo) && String.IsNullOrEmpty(item.PoliceSigortali.VergiKimlikNo))
                        {
                            if (item.PoliceSigortaEttiren != null)
                            {
                                if (!String.IsNullOrEmpty(item.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(item.PoliceSigortaEttiren.VergiKimlikNo))
                                {
                                    item.PoliceSigortali.KimlikNo = item.PoliceSigortaEttiren.KimlikNo;
                                    item.PoliceSigortali.VergiKimlikNo = item.PoliceSigortaEttiren.VergiKimlikNo;
                                    item.PoliceSigortali.AdiUnvan = item.PoliceSigortaEttiren.AdiUnvan;
                                    item.PoliceSigortali.SoyadiUnvan = item.PoliceSigortaEttiren.SoyadiUnvan;
                                }
                            }
                        }
                    }
                }
            }
            return polGenelList;
        }
    }
}
