using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using Neosinerji.BABOnlineTP.Business.Common;
using Microsoft.Practices.Unity;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using System.Diagnostics;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap;
using Neosinerji.BABOnlineTP.Business.Tools;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class PoliceService : IPoliceService
    {
        IPoliceContext _PoliceContext;
        ITeklifContext _TeklifContext;
        IMuhasebeContext _MuhasebeContext;
        IMusteriContext _MusteriContext;
        IKomisyonContext _KomisyonContext;
        IAktifKullaniciService _AktifKullanici;
        IParametreContext _ParametreContext;
        IAktifKullaniciService _AktifKullaniciService;
        IMuhasebe_CariHesapService _Muhasebe_CariHesapService;
        ITVMService _TVMService;
        IKomisyonService _KomisyonService;
        ITVMContext _TVMContext;
        ISigortaSirketleriService _SigortaSirketleriService;
        [InjectionConstructor]
        public PoliceService(IPoliceContext policeContext, ISigortaSirketleriService sigortaSirketleriService, IMuhasebe_CariHesapService muhasebe_CariHesapService, IMuhasebeContext muhasebeContext, IMusteriContext musteriContext, IKomisyonContext komisyonContext, IAktifKullaniciService aktifKullanici, IParametreContext parametreContext, IAktifKullaniciService _aktifKullaniciService, ITVMService _tVMService, IKomisyonService _komisyonService, ITVMContext tvmContext, ITeklifContext teklifContext)
        {
            _Muhasebe_CariHesapService = muhasebe_CariHesapService;
            _PoliceContext = policeContext;
            _MuhasebeContext = muhasebeContext;
            _MusteriContext = musteriContext;
            _KomisyonContext = komisyonContext;
            _AktifKullanici = aktifKullanici;
            _ParametreContext = parametreContext;
            _AktifKullaniciService = _aktifKullaniciService;
            _KomisyonService = _komisyonService;
            _TVMService = _tVMService;
            _TVMContext = tvmContext;
            _SigortaSirketleriService = sigortaSirketleriService;
            _TeklifContext = teklifContext;


        }

        private int eklenenKayit = 0;
        private int updateKayit = 0;
        private int varolanKayit = 0;
        private int hataliEklenmeyenKayit = 0;
        List<PoliceKontrol> PoliceKontrolList = new List<PoliceKontrol>();
        List<PoliceKontrol> EklenmeyenPoliceList = new List<PoliceKontrol>();

        StringBuilder hata = new StringBuilder();
        public void Add(List<Police> policeler)
        {
            DateTime now = DateTime.Now;

            foreach (Police item in policeler)
            {
                if (item.GenelBilgiler.TaliAcenteKodu == 0)
                {
                    item.GenelBilgiler.TaliAcenteKodu = null;
                }
                int result = this.AddPolice(item);
                if (result == 1)
                    eklenenKayit++;
                else if (result == 3)
                    updateKayit++;
                else if (result == 2)
                {
                    AddMissingInstallments(item);
                    PoliceKontrol PolEklenmeyen = new PoliceKontrol();
                    PolEklenmeyen.PoliceNo = item.GenelBilgiler.PoliceNumarasi;
                    PolEklenmeyen.EkNo = item.GenelBilgiler.EkNo.Value;
                    PolEklenmeyen.YenilemeNo = item.GenelBilgiler.YenilemeNo.Value;
                    PolEklenmeyen.Hatatip = "Poliçe var";
                    PoliceKontrolList.Add(PolEklenmeyen);
                    varolanKayit++;
                }
                else if (result == 0)
                {
                    hataliEklenmeyenKayit++;
                }
                Boolean sonuc = this.AddMusteriGenelBilgisi(item.MusteriBilgiler);

            }
            //Debug.Write("Police Sayısı :" + policeler.Count);
            //Debug.Write(" Eklenen Sayısı:" + eklenenKayit);
            //Debug.WriteLine(" Hatalı Sayısı :" + hataliEklenmeyenKayit);
            //Double abc = (DateTime.Now - now).TotalMilliseconds;
            //Debug.WriteLine("Gecen sure" + abc);
        }
        public Boolean AddMusteriGenelBilgisi(List<MusteriGenelBilgiler> musteriler)
        {
            if (musteriler.Count > 0)
            {
                for (int i = 0; i < musteriler.Count; i++)
                {
                    int aktifTVMKodu = _AktifKullanici.TVMKodu;
                    string kimlikNo = musteriler[i].KimlikNo;
                    MusteriGenelBilgiler musteriVarMi = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.KimlikNo == kimlikNo && w.TVMKodu == aktifTVMKodu).FirstOrDefault();
                    if (musteriVarMi == null)
                    {
                        // müşteriGenelBilgiler de müşteri kayıtlı değil 
                        try
                        {
                            musteriler[i].TVMKodu = _AktifKullanici.TVMKodu;
                            musteriler[i].KayitTarihi = TurkeyDateTime.Now;
                            musteriler[i].TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                            musteriler[i].KimlikNo = musteriler[i].KimlikNo.Trim();
                            if (musteriler[i].KimlikNo.Count() == 10)
                            {
                                musteriler[i].MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                            }
                            else
                            {
                                musteriler[i].MusteriTipKodu = MusteriTipleri.TCMusteri;
                            }
                            _MusteriContext.MusteriGenelBilgilerRepository.Create(musteriler[i]);
                            _MusteriContext.Commit();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            return true;
        }
        public int AddPolice(Police police)
        {
            try
            {

                if (police.GenelBilgiler.TVMKodu.HasValue && !String.IsNullOrEmpty(police.GenelBilgiler.TUMBirlikKodu) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceNumarasi) &&
                                         police.GenelBilgiler.EkNo.HasValue && police.GenelBilgiler.YenilemeNo.HasValue)
                {

                    string sigortaliKimlikNo = "";
                    string sEttirenKimlikNo = "";
                    string cariSigortaEttirenKimlikNo = "";
                    string cariSigortaEttirenHesapNo = "";
                    MusteriGenelBilgiler musGenel = new MusteriGenelBilgiler();
                    MusteriAdre musAdres = new MusteriAdre();
                    MusteriTelefon musTelefon = new MusteriTelefon();
                    bool kayitVarMi = false;
                    bool cariHesapVarMi = false;
                    bool zeylMi = false;
                    bool zeylGuncellemeMi = false;
                    if (police.GenelBilgiler.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                    {
                        if (police.GenelBilgiler.EkNo > 1 || police.GenelBilgiler.BransKodu == BransListeCeviri.Dask)
                        {
                            if (police.GenelBilgiler.EkNo.ToString().Length > 4)
                            {
                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) != "1")
                                {
                                    zeylMi = true;
                                }
                            }
                            else
                            {
                                zeylMi = true;
                            }
                        }
                    }
                    else if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                    {
                        if (police.GenelBilgiler.EkNo > 0)
                        {
                            zeylMi = true;
                        }
                    }

                    if (police.GenelBilgiler.TaliAcenteKodu != null && !zeylMi)
                    {
                        kayitVarMi = this.TaliPoliceKaydiVarMi(police.GenelBilgiler.TVMKodu.Value, police.GenelBilgiler.TaliAcenteKodu.Value, police.GenelBilgiler.TUMBirlikKodu, police.GenelBilgiler.PoliceNumarasi, police.GenelBilgiler.EkNo.Value, police.GenelBilgiler.YenilemeNo.Value);
                    }
                    else if (police.GenelBilgiler.TaliAcenteKodu != null && zeylMi)
                    {
                        kayitVarMi = false;
                        zeylGuncellemeMi = true;
                    }
                    else
                    {
                        kayitVarMi = this.KayitVarMi(police.GenelBilgiler.TVMKodu.Value, police.GenelBilgiler.TUMBirlikKodu, police.GenelBilgiler.PoliceNumarasi, police.GenelBilgiler.EkNo.Value, police.GenelBilgiler.YenilemeNo.Value);
                    }
                    if (!kayitVarMi && !zeylGuncellemeMi)
                    {
                        var zeylinPolicesiOnaylanmisMi = this.ZeylinPolicesiOnaylanmisMi(police.GenelBilgiler.TVMKodu.Value, police.GenelBilgiler.TUMBirlikKodu, police.GenelBilgiler.PoliceNumarasi, police.GenelBilgiler.YenilemeNo.Value);
                        if (zeylinPolicesiOnaylanmisMi != null)
                        {
                            police.GenelBilgiler.TaliAcenteKodu = zeylinPolicesiOnaylanmisMi.TaliAcenteKodu;
                            police.GenelBilgiler.TaliKomisyonGuncellemeTarihi = TurkeyDateTime.Now;
                            police.GenelBilgiler.TaliKomisyonGuncelleyenKullanici = _AktifKullanici.KullaniciKodu;
                            police.GenelBilgiler.TaliKomisyonOran = zeylinPolicesiOnaylanmisMi.TaliKomisyonOran;
                            police.GenelBilgiler.TaliKomisyon = (police.GenelBilgiler.Komisyon * police.GenelBilgiler.TaliKomisyonOran) / 100;
                        }

                        #region Tckn Üretme Merkezi

                        if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                        {
                            KimlikNoUret uretTc = new KimlikNoUret();
                            uretTc = GetKimlikNoUret();
                            if (uretTc.TcknSayac != null)
                            {
                                Int64 Kimlik = 0;
                                var kimlik = uretTc;
                                Kimlik = Convert.ToInt64(kimlik.TcknSayac);
                                var aas = (Kimlik + 1).ToString();
                                uretTc.TcknSayac = aas;
                                _PoliceContext.KimlikNoUretRepository.Update(uretTc);
                                _PoliceContext.Commit();
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = (uretTc.TcknSayac).ToString();
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = (uretTc.TcknSayac).ToString();
                                foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                {
                                    item.KimlikNo = (uretTc.TcknSayac).ToString();
                                }
                            }
                        }

                        #endregion

                        _PoliceContext.PoliceGenelRepository.Create(police.GenelBilgiler);
                        _PoliceContext.Commit();

                        PoliceUretimHedefGerceklesen gerceklesenUretim = new PoliceUretimHedefGerceklesen();
                        if (police.GenelBilgiler.TaliAcenteKodu != null)
                        {
                            gerceklesenUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Find(s => s.Donem == police.GenelBilgiler.TanzimTarihi.Value.Year &&
                                                                                                            s.TVMKoduTali == police.GenelBilgiler.TaliAcenteKodu &&
                                                                                                            s.TVMKodu == _AktifKullanici.TVMKodu &&
                                                                                                            s.BransKodu == police.GenelBilgiler.BransKodu.Value);
                        }
                        else
                        {
                            gerceklesenUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Find(s => s.Donem == police.GenelBilgiler.TanzimTarihi.Value.Year &&
                                                                                                                                       s.TVMKoduTali == _AktifKullanici.TVMKodu &&
                                                                                                                                       s.TVMKodu == _AktifKullanici.TVMKodu &&
                                                                                                                                       s.BransKodu == police.GenelBilgiler.BransKodu.Value);
                        }

                        #region Gerçekleşen Üretim Güncelleme

                        if (gerceklesenUretim != null) //Güncelleme
                        {
                            //Poliçenin başlangıç Tarihi hangi aya ait ise o aya poliçe neti ekleniyor
                            var policeAy = police.GenelBilgiler.TanzimTarihi.Value.Month;
                            if (policeAy == 1) //Ocak
                            {
                                if (gerceklesenUretim.PoliceAdedi1 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi1 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi1 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi1 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi1 = 1;
                                }
                                if (gerceklesenUretim.Prim1 != null)
                                {
                                    gerceklesenUretim.Prim1 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim1 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari1 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari1 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari1 = police.GenelBilgiler.Komisyon;
                                }

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari1 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari1 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari1 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 2)//Şubat
                            {
                                if (gerceklesenUretim.PoliceAdedi2 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi2 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi2 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi2 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi2 = 1;
                                }
                                if (gerceklesenUretim.Prim2 != null)
                                {
                                    gerceklesenUretim.Prim2 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim2 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari2 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari2 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari2 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari2 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari2 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari2 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 3)//Mart
                            {
                                if (gerceklesenUretim.PoliceAdedi3 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi3 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi3 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi3 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi3 = 1;
                                }
                                if (gerceklesenUretim.Prim3 != null)
                                {
                                    gerceklesenUretim.Prim3 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim3 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari3 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari3 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari3 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari3 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari3 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari3 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 4)//Nisan
                            {
                                if (gerceklesenUretim.PoliceAdedi4 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi4 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi4 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi4 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi4 = 1;
                                }
                                if (gerceklesenUretim.Prim4 != null)
                                {
                                    gerceklesenUretim.Prim4 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim4 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari4 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari4 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari4 = police.GenelBilgiler.Komisyon;
                                }

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari4 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari4 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari4 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 5)//Mayıs
                            {
                                if (gerceklesenUretim.PoliceAdedi5 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi5 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi5 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi5 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi5 = 1;
                                }
                                if (gerceklesenUretim.Prim5 != null)
                                {
                                    gerceklesenUretim.Prim5 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim5 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari5 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari5 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari5 = police.GenelBilgiler.Komisyon;
                                }

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari5 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari5 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari5 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 6)//Haziran
                            {
                                if (gerceklesenUretim.PoliceAdedi6 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi6 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi6 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi6 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi6 = 1;
                                }
                                if (gerceklesenUretim.Prim6 != null)
                                {
                                    gerceklesenUretim.Prim6 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim6 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari6 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari6 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari6 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari6 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari6 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari6 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 7)//Temmuz
                            {
                                if (gerceklesenUretim.PoliceAdedi7 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi7 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi7 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi7 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi7 = 1;
                                }
                                if (gerceklesenUretim.Prim7 != null)
                                {
                                    gerceklesenUretim.Prim7 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim7 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari7 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari7 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari7 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari7 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari7 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari7 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 8)//Ağustos
                            {
                                if (gerceklesenUretim.PoliceAdedi8 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi8 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi8 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi8 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi8 = 1;
                                }
                                if (gerceklesenUretim.Prim8 != null)
                                {
                                    gerceklesenUretim.Prim8 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim8 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari8 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari8 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari8 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari8 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari8 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari8 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 9)//Eylül
                            {
                                if (gerceklesenUretim.PoliceAdedi9 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi9 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi9 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi9 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi9 = 1;
                                }
                                if (gerceklesenUretim.Prim9 != null)
                                {
                                    gerceklesenUretim.Prim9 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim9 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari9 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari9 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari9 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari9 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari9 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari9 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 10)//Ekim
                            {
                                if (gerceklesenUretim.PoliceAdedi10 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi10 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi10 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi10 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi10 = 1;
                                }
                                if (gerceklesenUretim.Prim10 != null)
                                {
                                    gerceklesenUretim.Prim10 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim10 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari10 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari10 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari10 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari10 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari10 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari10 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 11)//Kasım
                            {
                                if (gerceklesenUretim.PoliceAdedi11 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi11 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi11 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi11 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi11 = 1;
                                }
                                if (gerceklesenUretim.Prim11 != null)
                                {
                                    gerceklesenUretim.Prim11 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim11 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari11 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari11 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari11 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari11 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari11 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari11 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            else if (policeAy == 12)//Aralık
                            {
                                if (gerceklesenUretim.PoliceAdedi12 != null)
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi12 += 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi12 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi12 += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceAdedi12 = 1;
                                }
                                if (gerceklesenUretim.Prim12 != null)
                                {
                                    gerceklesenUretim.Prim12 += police.GenelBilgiler.NetPrim;
                                }
                                else
                                {
                                    gerceklesenUretim.Prim12 = police.GenelBilgiler.NetPrim;
                                }
                                if (gerceklesenUretim.PoliceKomisyonTutari12 != null)
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari12 += police.GenelBilgiler.Komisyon;
                                }
                                else
                                {
                                    gerceklesenUretim.PoliceKomisyonTutari12 = police.GenelBilgiler.Komisyon;
                                }
                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    if (gerceklesenUretim.VerilenKomisyonTutari12 != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari12 += police.GenelBilgiler.TaliKomisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari12 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                            }
                            _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretim);
                            _KomisyonContext.Commit();
                        }

                        #endregion

                        #region Gerçekleşen Üretim Kayıt Ekle

                        else //Tabloda acentenin hiç gerçekleşen üretimi yok ise yeni ekleniyor
                        {
                            if (gerceklesenUretim == null)
                            {
                                gerceklesenUretim = new PoliceUretimHedefGerceklesen();
                            }
                            //Poliçenin başlangıç Tarihi hangi aya ait ise o aya poliçe neti ekleniyor
                            var policeAy = police.GenelBilgiler.TanzimTarihi.Value.Month;
                            if (policeAy == 1) //Ocak
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi1 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi1 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi1 = 1;
                                    }
                                }
                                gerceklesenUretim.Prim1 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari1 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari1 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 2)//Şubat
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi2 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi2 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi2 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim2 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari2 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari2 = police.GenelBilgiler.TaliKomisyon;

                                }
                            }
                            else if (policeAy == 3)//Mart
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi3 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi3 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi3 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim3 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari3 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari3 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 4)//Nisan
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi4 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi4 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi4 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim4 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari4 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari4 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 5)//Mayıs
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi5 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi5 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi5 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim5 = police.GenelBilgiler.NetPrim;

                                gerceklesenUretim.PoliceKomisyonTutari5 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari5 = police.GenelBilgiler.TaliKomisyon;

                                }
                            }
                            else if (policeAy == 6)//Haziran
                            {

                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi6 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi6 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi6 = 1;
                                    }
                                }
                                gerceklesenUretim.Prim6 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari6 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari6 = police.GenelBilgiler.TaliKomisyon;

                                }
                            }
                            else if (policeAy == 7)//Temmuz
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi7 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi7 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi7 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim7 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari7 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari7 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 8)//Ağustos
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi8 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi8 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi8 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim8 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari8 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {

                                    gerceklesenUretim.VerilenKomisyonTutari8 = police.GenelBilgiler.TaliKomisyon;

                                }
                            }
                            else if (policeAy == 9)//Eylül
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi9 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi9 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi9 = 1;
                                    }
                                }
                                gerceklesenUretim.Prim9 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari9 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari9 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 10)//Ekim
                            {

                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi10 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi10 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi10 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim10 = police.GenelBilgiler.NetPrim;

                                gerceklesenUretim.PoliceKomisyonTutari10 = police.GenelBilgiler.Komisyon;


                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari10 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 11)//Kasım
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi11 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi11 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi11 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim11 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari11 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari11 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            else if (policeAy == 12)//Aralık
                            {
                                if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                {
                                    if (police.GenelBilgiler.EkNo == 0)
                                    {
                                        gerceklesenUretim.PoliceAdedi12 = 1;
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                    {
                                        if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                        {
                                            gerceklesenUretim.PoliceAdedi12 += 1;
                                        }
                                    }
                                    if (police.GenelBilgiler.EkNo == 1)
                                    {
                                        gerceklesenUretim.PoliceAdedi12 = 1;
                                    }
                                }

                                gerceklesenUretim.Prim12 = police.GenelBilgiler.NetPrim;
                                gerceklesenUretim.PoliceKomisyonTutari12 = police.GenelBilgiler.Komisyon;

                                if (police.GenelBilgiler.TaliAcenteKodu != null)
                                {
                                    gerceklesenUretim.VerilenKomisyonTutari12 = police.GenelBilgiler.TaliKomisyon;
                                }
                            }
                            gerceklesenUretim.TVMKodu = _AktifKullanici.TVMKodu;
                            gerceklesenUretim.TVMKoduTali = _AktifKullanici.TVMKodu;  //Poliçenin tali acentekodu belirenmemiş ise transfer yapan kullanıcının tvmkodu ile kayıt ediliyor
                            if (police.GenelBilgiler.TaliAcenteKodu != null)  //Poliçenin tali acentekodu belirenmiş ise alt acentenin kodu ile kaydediliyor
                            {
                                gerceklesenUretim.TVMKoduTali = police.GenelBilgiler.TaliAcenteKodu;
                            }
                            gerceklesenUretim.Donem = police.GenelBilgiler.TanzimTarihi.Value.Year;
                            gerceklesenUretim.BransKodu = police.GenelBilgiler.BransKodu;

                            _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretim);
                            _KomisyonContext.Commit();
                        }

                        #endregion

                        #region Müşteri KayıtEkleme


                        int tvmKodu = 0;
                        if (police.GenelBilgiler.TaliAcenteKodu != null)
                        {
                            tvmKodu = police.GenelBilgiler.TaliAcenteKodu.Value;
                        }
                        else
                        {
                            tvmKodu = police.GenelBilgiler.TVMKodu.Value;
                        }

                        if ((!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo)) || (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo)))
                        {
                            if (police.GenelBilgiler.PoliceSigortali.KimlikNo == police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo || police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo)
                            {
                                #region ikisi birbirine eşit olduğu için tek kayıt ediliyor müşteri tablosuna
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                {
                                    sigortaliKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                    Musteri = this.getMusteri(sigortaliKimlikNo, tvmKodu);
                                    string ilKodu = null;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                    {
                                        ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortali.IlAdi);
                                    }
                                    //MusteriGenelBilgiler Musteri = this.getMusteri(tvmKodu, sigortaliKimlikNo);
                                    if (Musteri != null)
                                    {
                                        var polSigortali = police.GenelBilgiler.PoliceSigortali;
                                        MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortali.Adres, polSigortali.IlKodu, polSigortali.Mahalle, polSigortali.Cadde, Convert.ToInt32(polSigortali.IlceKodu), polSigortali.Apartman, polSigortali.Sokak, polSigortali.BinaNo, polSigortali.DaireNo, Musteri.MusteriKodu);
                                        MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortali.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                                musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {
                                            int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                siraNumarasi += 1;
                                            }
                                            MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortali.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                            if (musTelefonum2 == null)
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                                musTelefon.SiraNo = siraNumarasi;
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }

                                        if (MusteriAdresim == null)
                                        {
                                            musAdres = new MusteriAdre();
                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;

                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;

                                            }
                                            if (Musteri.MusteriAdres.Count == 0)
                                            {
                                                musAdres.Varsayilan = true;
                                            }
                                            else
                                            {
                                                musAdres.Varsayilan = false;
                                            }
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                            musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                            //    musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                            musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                            Musteri.MusteriAdres.Add(musAdres);
                                        }
                                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                        _MusteriContext.Commit();
                                    }
                                    else
                                    {
                                        musGenel = new MusteriGenelBilgiler();
                                        musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.AdiUnvan) ? police.GenelBilgiler.PoliceSigortali.AdiUnvan : "..";
                                        musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortali.SoyadiUnvan : "";
                                        musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                                        musGenel.KayitTarihi = DateTime.Now;
                                        musGenel.TVMKodu = tvmKodu;
                                        musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                        musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                        musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                                        musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                        musGenel.EMail = police.GenelBilgiler.PoliceSigortali.EMail;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;
                                        }
                                        else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                                        }
                                        musAdres = new MusteriAdre();
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                        if (police.GenelBilgiler.BransKodu == 11)
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Ev;
                                        }
                                        else
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Diger;
                                        }
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                        musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                        musAdres.SiraNo = 1;
                                        musAdres.Varsayilan = true;
                                        // musAdres.MusteriKodu = Musteri.MusteriKodu;
                                        musGenel.MusteriAdres.Add(musAdres);

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        else
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = "..";
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                            musTelefon.SiraNo = 2;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                        _MusteriContext.Commit();
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                #region sigortali

                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                {
                                    sigortaliKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                    Musteri = this.getMusteri(sigortaliKimlikNo, tvmKodu);
                                    string ilKodu = null;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                    {
                                        ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortali.IlAdi);
                                    }
                                    //MusteriGenelBilgiler Musteri = this.getMusteri(tvmKodu, sigortaliKimlikNo);
                                    if (Musteri != null)
                                    {
                                        var polSigortali = police.GenelBilgiler.PoliceSigortali;
                                        MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortali.Adres, polSigortali.IlKodu, polSigortali.Mahalle, polSigortali.Cadde, Convert.ToInt32(polSigortali.IlceKodu), polSigortali.Apartman, polSigortali.Sokak, polSigortali.BinaNo, polSigortali.DaireNo, Musteri.MusteriKodu);
                                        MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortali.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                                musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {
                                            int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                siraNumarasi += 1;
                                            }
                                            MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortali.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                            if (musTelefonum2 == null)
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                                musTelefon.SiraNo = siraNumarasi;
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }

                                        if (MusteriAdresim == null)
                                        {
                                            musAdres = new MusteriAdre();
                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;

                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;

                                            }
                                            if (Musteri.MusteriAdres.Count == 0)
                                            {
                                                musAdres.Varsayilan = true;
                                            }
                                            else
                                            {
                                                musAdres.Varsayilan = false;
                                            }
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                            musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                            //    musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                            musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                            Musteri.MusteriAdres.Add(musAdres);
                                        }
                                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                        _MusteriContext.Commit();
                                    }
                                    else
                                    {
                                        musGenel = new MusteriGenelBilgiler();
                                        musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.AdiUnvan) ? police.GenelBilgiler.PoliceSigortali.AdiUnvan : "..";
                                        musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortali.SoyadiUnvan : "";
                                        musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                                        musGenel.KayitTarihi = DateTime.Now;
                                        musGenel.TVMKodu = tvmKodu;
                                        musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                        musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                        musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                                        musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                        musGenel.EMail = police.GenelBilgiler.PoliceSigortali.EMail;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;
                                        }
                                        else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                                        }
                                        musAdres = new MusteriAdre();
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                        if (police.GenelBilgiler.BransKodu == 11)
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Ev;
                                        }
                                        else
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Diger;
                                        }
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                        musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                        musAdres.SiraNo = 1;
                                        musAdres.Varsayilan = true;
                                        // musAdres.MusteriKodu = Musteri.MusteriKodu;
                                        musGenel.MusteriAdres.Add(musAdres);

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        else
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = "..";
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                            musTelefon.SiraNo = 2;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                        _MusteriContext.Commit();
                                    }
                                }
                                #endregion
                                #region s.ettiren
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                                {
                                    sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                    Musteri = this.getMusteri(sEttirenKimlikNo, tvmKodu);
                                    string ilKodu = null;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi))
                                    {
                                        ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi);
                                    }
                                    if (Musteri != null)
                                    {
                                        var polSigortaEttiren = police.GenelBilgiler.PoliceSigortaEttiren;
                                        MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortaEttiren.Adres, polSigortaEttiren.IlKodu, polSigortaEttiren.Mahalle, polSigortaEttiren.Cadde, Convert.ToInt32(polSigortaEttiren.IlceKodu), polSigortaEttiren.Apartman, polSigortaEttiren.Cadde, polSigortaEttiren.BinaNo, polSigortaEttiren.DaireNo, Musteri.MusteriKodu);

                                        MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortaEttiren.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                                musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }


                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                        {
                                            int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                siraNumarasi += 1;
                                            }
                                            MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortaEttiren.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                            if (musTelefonum2 == null)
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                                musTelefon.SiraNo = siraNumarasi;
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }
                                        if (MusteriAdresim != null)
                                        {
                                            musAdres = new MusteriAdre();
                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;
                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;
                                            }
                                            if (Musteri.MusteriAdres.Count == 0)
                                            {
                                                musAdres.Varsayilan = true;
                                            }
                                            else
                                            {
                                                musAdres.Varsayilan = false;
                                            }
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                            musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu);
                                            //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                            Musteri.MusteriAdres.Add(musAdres);
                                        }
                                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                        _MusteriContext.Commit();
                                    }
                                    else
                                    {
                                        musGenel = new MusteriGenelBilgiler();
                                        musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan : "..";
                                        musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan : "";
                                        musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi;
                                        musGenel.KayitTarihi = DateTime.Now;
                                        musGenel.TVMKodu = tvmKodu;
                                        musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                        musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet;
                                        musGenel.EMail = police.GenelBilgiler.PoliceSigortaEttiren.EMail;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;

                                        }
                                        else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;

                                        }
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        else
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = "..";
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                            musTelefon.SiraNo = 2;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        musAdres = new MusteriAdre();
                                        if (police.GenelBilgiler.BransKodu == 11)
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Ev;
                                        }
                                        else
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Diger;
                                        }
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                        musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu);
                                        musAdres.SiraNo = 1;
                                        musAdres.Varsayilan = true;
                                        musGenel.MusteriAdres.Add(musAdres);
                                        _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                        _MusteriContext.Commit();
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                            {
                                sigortaliKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                Musteri = this.getMusteri(sigortaliKimlikNo, tvmKodu);
                                string ilKodu = null;
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                {
                                    ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortali.IlAdi);
                                }
                                if (Musteri != null)
                                {
                                    var polSigortali = police.GenelBilgiler.PoliceSigortali;
                                    MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortali.Adres, polSigortali.IlKodu, polSigortali.Mahalle, polSigortali.Cadde, Convert.ToInt32(polSigortali.IlceKodu), polSigortali.Apartman, polSigortali.Sokak, polSigortali.BinaNo, polSigortali.DaireNo, Musteri.MusteriKodu);
                                    MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortali.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);

                                    if (musTelefonum == null)
                                    {
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                            musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            Musteri.MusteriTelefons.Add(musTelefon);
                                        }
                                    }


                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                    {
                                        int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            siraNumarasi += 1;
                                        }
                                        MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortali.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                        if (musTelefonum2 == null)
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                            musTelefon.SiraNo = siraNumarasi;
                                            Musteri.MusteriTelefons.Add(musTelefon);
                                        }
                                    }
                                    if (MusteriAdresim == null)
                                    {
                                        musAdres = new MusteriAdre();
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                        if (police.GenelBilgiler.BransKodu == 11)
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Ev;
                                        }
                                        else
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Diger;
                                        }
                                        if (Musteri.MusteriAdres.Count == 0)
                                        {
                                            musAdres.Varsayilan = true;
                                        }
                                        else
                                        {
                                            musAdres.Varsayilan = false;
                                        }
                                        musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                        //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                        musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                        musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                        Musteri.MusteriAdres.Add(musAdres);
                                    }
                                    _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                    _MusteriContext.Commit();
                                }
                                else
                                {
                                    musGenel = new MusteriGenelBilgiler();
                                    musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.AdiUnvan) ? police.GenelBilgiler.PoliceSigortali.AdiUnvan : "..";
                                    musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortali.SoyadiUnvan : "";
                                    musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                                    musGenel.KayitTarihi = DateTime.Now;
                                    musGenel.TVMKodu = tvmKodu;
                                    musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                    musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                                    musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                    musGenel.EMail = police.GenelBilgiler.PoliceSigortali.EMail;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                    {
                                        musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;

                                    }
                                    else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                    {
                                        musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;

                                    }
                                    musAdres = new MusteriAdre();
                                    musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                    if (police.GenelBilgiler.BransKodu == 11)
                                    {
                                        musAdres.AdresTipi = AdresTipleri.Ev;
                                    }
                                    else
                                    {
                                        musAdres.AdresTipi = AdresTipleri.Diger;
                                    }
                                    musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                    musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                    musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                    musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                    musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                    musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                    musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                    musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                    musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                    musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                    musAdres.SiraNo = 1;
                                    musAdres.Varsayilan = true;
                                    musGenel.MusteriAdres.Add(musAdres);

                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                    {
                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                        musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                        musTelefon.SiraNo = 1;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }
                                    else
                                    {
                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                        musTelefon.Numara = "..";
                                        musTelefon.SiraNo = 1;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }

                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                    {

                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                        musTelefon.SiraNo = 2;
                                        musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }


                                    _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                    _MusteriContext.Commit();
                                }
                            }
                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                            {
                                sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                Musteri = this.getMusteri(sEttirenKimlikNo, tvmKodu);
                                string ilKodu = null;
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi))
                                {
                                    ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi);
                                }
                                if (Musteri != null)
                                {
                                    //var polSigortaEttiren = police.GenelBilgiler.PoliceSigortaEttiren;
                                    //MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortaEttiren.Adres, polSigortaEttiren.IlKodu, polSigortaEttiren.Mahalle, polSigortaEttiren.Cadde, Convert.ToInt32(polSigortaEttiren.IlceKodu), polSigortaEttiren.Apartman, polSigortaEttiren.Cadde, polSigortaEttiren.BinaNo, polSigortaEttiren.DaireNo, Musteri.MusteriKodu);

                                    //MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortaEttiren.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                    //if (musTelefonum == null)
                                    //{
                                    //    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                    //    {
                                    //        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                    //        musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                    //        musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                    //        Musteri.MusteriTelefons.Add(musTelefon);
                                    //    }
                                    //}


                                    //if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                    //{
                                    //    int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                    //    if (musTelefonum == null)
                                    //    {
                                    //        siraNumarasi += 1;
                                    //    }
                                    //    MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortaEttiren.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                    //    if (musTelefonum2 == null)
                                    //    {
                                    //        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                    //        musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                    //        musTelefon.SiraNo = siraNumarasi;
                                    //        Musteri.MusteriTelefons.Add(musTelefon);
                                    //    }
                                    //}
                                    //if (MusteriAdresim == null)
                                    //{
                                    //    musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                    //    if (police.GenelBilgiler.BransKodu == 11)
                                    //    {
                                    //        musAdres.AdresTipi = AdresTipleri.Ev;
                                    //    }
                                    //    else
                                    //    {
                                    //        musAdres.AdresTipi = AdresTipleri.Diger;
                                    //    }
                                    //    if (Musteri.MusteriAdres.Count == 0)
                                    //    {
                                    //        musAdres.Varsayilan = true;
                                    //    }
                                    //    else
                                    //    {
                                    //        musAdres.Varsayilan = false;
                                    //    }
                                    //    musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                    //    musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                    //    musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                    //    musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                    //    musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                    //    musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                    //    musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                    //    musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                    //    musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                    //    musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu);
                                    //    //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                    //    musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                    //    Musteri.MusteriAdres.Add(musAdres);
                                    //}
                                    //_MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                    //_MusteriContext.Commit();
                                }
                                else
                                {
                                    musGenel = new MusteriGenelBilgiler();
                                    musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan : "..";
                                    musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan : "";
                                    musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi;
                                    musGenel.KayitTarihi = DateTime.Now;
                                    musGenel.TVMKodu = tvmKodu;
                                    musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                    musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet;

                                    musGenel.TVMMusteriKodu = police.GenelBilgiler.PoliceSigortali.MusteriGrupKodu;
                                    musGenel.EMail = police.GenelBilgiler.PoliceSigortaEttiren.EMail;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                                    {
                                        musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;

                                    }
                                    else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                                    {
                                        musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;

                                    }
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                    {
                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                        musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                        musTelefon.SiraNo = 1;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }
                                    else
                                    {
                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                        musTelefon.Numara = "..";
                                        musTelefon.SiraNo = 1;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }

                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                    {
                                        musTelefon = new MusteriTelefon();
                                        musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                        musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                        musTelefon.SiraNo = 2;
                                        musGenel.MusteriTelefons.Add(musTelefon);
                                    }
                                    musAdres = new MusteriAdre();
                                    if (police.GenelBilgiler.BransKodu == 11)
                                    {
                                        musAdres.AdresTipi = AdresTipleri.Ev;
                                    }
                                    else
                                    {
                                        musAdres.AdresTipi = AdresTipleri.Diger;
                                    }
                                    musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                    musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                    musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                    musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                    musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                    musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                    musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                    musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                    musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                    musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                    musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu);
                                    musAdres.SiraNo = 1;
                                    musAdres.Varsayilan = true;
                                    musGenel.MusteriAdres.Add(musAdres);
                                    _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                    _MusteriContext.Commit();
                                }
                            }
                        }
                        #endregion

                    }
                    else if (!kayitVarMi && zeylGuncellemeMi)
                    {
                        var polZeyli = this.GetPoliceZeyli(police.GenelBilgiler.TVMKodu.Value, police.GenelBilgiler.TUMBirlikKodu, police.GenelBilgiler.PoliceNumarasi, police.GenelBilgiler.EkNo.Value, police.GenelBilgiler.YenilemeNo.Value);
                        if (polZeyli != null)
                        {
                            polZeyli.TaliAcenteKodu = police.GenelBilgiler.TaliAcenteKodu;
                            polZeyli.TaliKomisyon = police.GenelBilgiler.TaliKomisyon;
                            polZeyli.TaliKomisyonOran = police.GenelBilgiler.TaliKomisyonOran;
                            _PoliceContext.PoliceGenelRepository.Update(polZeyli);
                            _PoliceContext.Commit();

                            _KomisyonService.PoliceUretimHedefGerceklesenGuncelle(polZeyli, 0);
                            kayitVarMi = true;
                        }
                        else
                        {
                            #region Zeyl Yok ise kaydet

                            #region Tckn Üretme Merkezi

                            if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                            {
                                KimlikNoUret uretTcc = new KimlikNoUret();
                                uretTcc = GetKimlikNoUret();
                                if (uretTcc.TcknSayac != null)
                                {
                                    Int64 Kimlik = 0;
                                    var kimlik = uretTcc;
                                    Kimlik = Convert.ToInt64(kimlik.TcknSayac);
                                    var aas = (Kimlik + 1).ToString();
                                    uretTcc.TcknSayac = aas;
                                    _PoliceContext.KimlikNoUretRepository.Update(uretTcc);
                                    _PoliceContext.Commit();
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = (uretTcc.TcknSayac).ToString();
                                    police.GenelBilgiler.PoliceSigortali.KimlikNo = (uretTcc.TcknSayac).ToString();
                                    foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                    {
                                        item.KimlikNo = (uretTcc.TcknSayac).ToString();
                                    }
                                }
                            }

                            #endregion

                            _PoliceContext.PoliceGenelRepository.Create(police.GenelBilgiler);
                            _PoliceContext.Commit();

                            PoliceUretimHedefGerceklesen gerceklesenUretim = new PoliceUretimHedefGerceklesen();
                            if (police.GenelBilgiler.TaliAcenteKodu != null)
                            {
                                gerceklesenUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Find(s => s.Donem == police.GenelBilgiler.TanzimTarihi.Value.Year &&
                                                                                                                s.TVMKoduTali == police.GenelBilgiler.TaliAcenteKodu &&
                                                                                                                s.TVMKodu == _AktifKullanici.TVMKodu &&
                                                                                                                s.BransKodu == police.GenelBilgiler.BransKodu.Value);
                            }
                            else
                            {
                                gerceklesenUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Find(s => s.Donem == police.GenelBilgiler.TanzimTarihi.Value.Year &&
                                                                                                                                           s.TVMKoduTali == _AktifKullanici.TVMKodu &&
                                                                                                                                           s.TVMKodu == _AktifKullanici.TVMKodu &&
                                                                                                                                           s.BransKodu == police.GenelBilgiler.BransKodu.Value);
                            }
                            #region Gerçekleşen Üretim Güncelleme

                            if (gerceklesenUretim != null) //Güncelleme
                            {
                                //Poliçenin başlangıç Tarihi hangi aya ait ise o aya poliçe neti ekleniyor
                                var policeAy = police.GenelBilgiler.TanzimTarihi.Value.Month;
                                if (policeAy == 1) //Ocak
                                {
                                    if (gerceklesenUretim.PoliceAdedi1 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi1 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi1 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi1 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi1 = 1;
                                    }
                                    if (gerceklesenUretim.Prim1 != null)
                                    {
                                        gerceklesenUretim.Prim1 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim1 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari1 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari1 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari1 = police.GenelBilgiler.Komisyon;
                                    }

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari1 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari1 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari1 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 2)//Şubat
                                {
                                    if (gerceklesenUretim.PoliceAdedi2 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi2 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi2 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi2 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi2 = 1;
                                    }
                                    if (gerceklesenUretim.Prim2 != null)
                                    {
                                        gerceklesenUretim.Prim2 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim2 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari2 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari2 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari2 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari2 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari2 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari2 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 3)//Mart
                                {
                                    if (gerceklesenUretim.PoliceAdedi3 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi3 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi3 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi3 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi3 = 1;
                                    }
                                    if (gerceklesenUretim.Prim3 != null)
                                    {
                                        gerceklesenUretim.Prim3 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim3 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari3 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari3 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari3 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari3 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari3 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari3 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 4)//Nisan
                                {
                                    if (gerceklesenUretim.PoliceAdedi4 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi4 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi4 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi4 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi4 = 1;
                                    }
                                    if (gerceklesenUretim.Prim4 != null)
                                    {
                                        gerceklesenUretim.Prim4 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim4 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari4 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari4 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari4 = police.GenelBilgiler.Komisyon;
                                    }

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari4 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari4 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari4 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 5)//Mayıs
                                {
                                    if (gerceklesenUretim.PoliceAdedi5 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi5 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi5 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi5 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi5 = 1;
                                    }
                                    if (gerceklesenUretim.Prim5 != null)
                                    {
                                        gerceklesenUretim.Prim5 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim5 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari5 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari5 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari5 = police.GenelBilgiler.Komisyon;
                                    }

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari5 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari5 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari5 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 6)//Haziran
                                {
                                    if (gerceklesenUretim.PoliceAdedi6 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi6 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi6 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi6 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi6 = 1;
                                    }
                                    if (gerceklesenUretim.Prim6 != null)
                                    {
                                        gerceklesenUretim.Prim6 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim6 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari6 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari6 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari6 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari6 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari6 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari6 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 7)//Temmuz
                                {
                                    if (gerceklesenUretim.PoliceAdedi7 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi7 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi7 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi7 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi7 = 1;
                                    }
                                    if (gerceklesenUretim.Prim7 != null)
                                    {
                                        gerceklesenUretim.Prim7 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim7 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari7 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari7 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari7 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari7 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari7 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari7 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 8)//Ağustos
                                {
                                    if (gerceklesenUretim.PoliceAdedi8 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi8 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi8 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi8 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi8 = 1;
                                    }
                                    if (gerceklesenUretim.Prim8 != null)
                                    {
                                        gerceklesenUretim.Prim8 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim8 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari8 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari8 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari8 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari8 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari8 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari8 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 9)//Eylül
                                {
                                    if (gerceklesenUretim.PoliceAdedi9 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi9 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi9 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi9 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi9 = 1;
                                    }
                                    if (gerceklesenUretim.Prim9 != null)
                                    {
                                        gerceklesenUretim.Prim9 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim9 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari9 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari9 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari9 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari9 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari9 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari9 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 10)//Ekim
                                {
                                    if (gerceklesenUretim.PoliceAdedi10 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi10 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi10 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi10 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi10 = 1;
                                    }
                                    if (gerceklesenUretim.Prim10 != null)
                                    {
                                        gerceklesenUretim.Prim10 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim10 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari10 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari10 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari10 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari10 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari10 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari10 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 11)//Kasım
                                {
                                    if (gerceklesenUretim.PoliceAdedi11 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi11 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi11 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi11 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi11 = 1;
                                    }
                                    if (gerceklesenUretim.Prim11 != null)
                                    {
                                        gerceklesenUretim.Prim11 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim11 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari11 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari11 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari11 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari11 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari11 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari11 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                else if (policeAy == 12)//Aralık
                                {
                                    if (gerceklesenUretim.PoliceAdedi12 != null)
                                    {
                                        if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (police.GenelBilgiler.EkNo == 0)
                                            {
                                                gerceklesenUretim.PoliceAdedi12 += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                            {
                                                if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                                {
                                                    gerceklesenUretim.PoliceAdedi12 += 1;
                                                }
                                            }
                                            if (police.GenelBilgiler.EkNo == 1)
                                            {
                                                gerceklesenUretim.PoliceAdedi12 += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceAdedi12 = 1;
                                    }
                                    if (gerceklesenUretim.Prim12 != null)
                                    {
                                        gerceklesenUretim.Prim12 += police.GenelBilgiler.NetPrim;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.Prim12 = police.GenelBilgiler.NetPrim;
                                    }
                                    if (gerceklesenUretim.PoliceKomisyonTutari12 != null)
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari12 += police.GenelBilgiler.Komisyon;
                                    }
                                    else
                                    {
                                        gerceklesenUretim.PoliceKomisyonTutari12 = police.GenelBilgiler.Komisyon;
                                    }
                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        if (gerceklesenUretim.VerilenKomisyonTutari12 != null)
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari12 += police.GenelBilgiler.TaliKomisyon;
                                        }
                                        else
                                        {
                                            gerceklesenUretim.VerilenKomisyonTutari12 = police.GenelBilgiler.TaliKomisyon;
                                        }
                                    }
                                }
                                _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(gerceklesenUretim);
                                _KomisyonContext.Commit();
                            }

                            #endregion

                            #region Gerçekleşen Üretim Kayıt Ekle

                            else //Tabloda acentenin hiç gerçekleşen üretimi yok ise yeni ekleniyor
                            {
                                if (gerceklesenUretim == null)
                                {
                                    gerceklesenUretim = new PoliceUretimHedefGerceklesen();
                                }
                                //Poliçenin başlangıç Tarihi hangi aya ait ise o aya poliçe neti ekleniyor
                                var policeAy = police.GenelBilgiler.TanzimTarihi.Value.Month;
                                if (policeAy == 1) //Ocak
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi1 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi1 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi1 = 1;
                                        }
                                    }
                                    gerceklesenUretim.Prim1 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari1 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari1 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 2)//Şubat
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi2 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi2 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi2 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim2 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari2 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari2 = police.GenelBilgiler.TaliKomisyon;

                                    }
                                }
                                else if (policeAy == 3)//Mart
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi3 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi3 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi3 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim3 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari3 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari3 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 4)//Nisan
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi4 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi4 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi4 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim4 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari4 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari4 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 5)//Mayıs
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi5 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi5 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi5 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim5 = police.GenelBilgiler.NetPrim;

                                    gerceklesenUretim.PoliceKomisyonTutari5 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari5 = police.GenelBilgiler.TaliKomisyon;

                                    }
                                }
                                else if (policeAy == 6)//Haziran
                                {

                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi6 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi6 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi6 = 1;
                                        }
                                    }
                                    gerceklesenUretim.Prim6 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari6 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari6 = police.GenelBilgiler.TaliKomisyon;

                                    }
                                }
                                else if (policeAy == 7)//Temmuz
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi7 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi7 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi7 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim7 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari7 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari7 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 8)//Ağustos
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi8 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi8 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi8 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim8 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari8 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {

                                        gerceklesenUretim.VerilenKomisyonTutari8 = police.GenelBilgiler.TaliKomisyon;

                                    }
                                }
                                else if (policeAy == 9)//Eylül
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi9 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi9 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi9 = 1;
                                        }
                                    }
                                    gerceklesenUretim.Prim9 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari9 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari9 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 10)//Ekim
                                {

                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi10 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi10 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi10 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim10 = police.GenelBilgiler.NetPrim;

                                    gerceklesenUretim.PoliceKomisyonTutari10 = police.GenelBilgiler.Komisyon;


                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari10 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 11)//Kasım
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi11 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi11 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi11 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim11 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari11 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari11 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                else if (policeAy == 12)//Aralık
                                {
                                    if (police.GenelBilgiler.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (police.GenelBilgiler.EkNo == 0)
                                        {
                                            gerceklesenUretim.PoliceAdedi12 = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (police.GenelBilgiler.BransKodu == BransListeCeviri.Dask && police.GenelBilgiler.EkNo.ToString().Length > 4)
                                        {
                                            if (police.GenelBilgiler.EkNo.ToString().Substring(4, 1) == "1")
                                            {
                                                gerceklesenUretim.PoliceAdedi12 += 1;
                                            }
                                        }
                                        if (police.GenelBilgiler.EkNo == 1)
                                        {
                                            gerceklesenUretim.PoliceAdedi12 = 1;
                                        }
                                    }

                                    gerceklesenUretim.Prim12 = police.GenelBilgiler.NetPrim;
                                    gerceklesenUretim.PoliceKomisyonTutari12 = police.GenelBilgiler.Komisyon;

                                    if (police.GenelBilgiler.TaliAcenteKodu != null)
                                    {
                                        gerceklesenUretim.VerilenKomisyonTutari12 = police.GenelBilgiler.TaliKomisyon;
                                    }
                                }
                                gerceklesenUretim.TVMKodu = _AktifKullanici.TVMKodu;
                                gerceklesenUretim.TVMKoduTali = _AktifKullanici.TVMKodu;  //Poliçenin tali acentekodu belirenmemiş ise transfer yapan kullanıcının tvmkodu ile kayıt ediliyor
                                if (police.GenelBilgiler.TaliAcenteKodu != null)  //Poliçenin tali acentekodu belirenmiş ise alt acentenin kodu ile kaydediliyor
                                {
                                    gerceklesenUretim.TVMKoduTali = police.GenelBilgiler.TaliAcenteKodu;
                                }
                                gerceklesenUretim.Donem = police.GenelBilgiler.TanzimTarihi.Value.Year;
                                gerceklesenUretim.BransKodu = police.GenelBilgiler.BransKodu;

                                _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Create(gerceklesenUretim);
                                _KomisyonContext.Commit();
                            }

                            #endregion

                            #region Müşteri KayıtEkleme


                            int tvmKodu = 0;
                            if (police.GenelBilgiler.TaliAcenteKodu != null)
                            {
                                tvmKodu = police.GenelBilgiler.TaliAcenteKodu.Value;
                            }
                            else
                            {
                                tvmKodu = police.GenelBilgiler.TVMKodu.Value;
                            }

                            if ((!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo)) || (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo)))
                            {
                                if (police.GenelBilgiler.PoliceSigortali.KimlikNo == police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo || police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo)
                                {
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                    {
                                        sigortaliKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                        MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                        Musteri = this.getMusteri(sigortaliKimlikNo, tvmKodu);
                                        string ilKodu = null;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                        {
                                            ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortali.IlAdi);
                                        }
                                        //MusteriGenelBilgiler Musteri = this.getMusteri(tvmKodu, sigortaliKimlikNo);
                                        if (Musteri != null)
                                        {
                                            var polSigortali = police.GenelBilgiler.PoliceSigortali;
                                            MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortali.Adres, polSigortali.IlKodu, polSigortali.Mahalle, polSigortali.Cadde, Convert.ToInt32(polSigortali.IlceKodu), polSigortali.Apartman, polSigortali.Sokak, polSigortali.BinaNo, polSigortali.DaireNo, Musteri.MusteriKodu);
                                            MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortali.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                                {
                                                    musTelefon = new MusteriTelefon();
                                                    musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                    musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                                    musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                    Musteri.MusteriTelefons.Add(musTelefon);
                                                }
                                            }

                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                            {
                                                int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                if (musTelefonum == null)
                                                {
                                                    siraNumarasi += 1;
                                                }
                                                MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortali.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                                if (musTelefonum2 == null)
                                                {
                                                    musTelefon = new MusteriTelefon();
                                                    musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                    musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                                    musTelefon.SiraNo = siraNumarasi;
                                                    Musteri.MusteriTelefons.Add(musTelefon);
                                                }
                                            }

                                            if (MusteriAdresim == null)
                                            {
                                                musAdres = new MusteriAdre();
                                                musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                                if (police.GenelBilgiler.BransKodu == 11)
                                                {
                                                    musAdres.AdresTipi = AdresTipleri.Ev;

                                                }
                                                else
                                                {
                                                    musAdres.AdresTipi = AdresTipleri.Diger;

                                                }
                                                if (Musteri.MusteriAdres.Count == 0)
                                                {
                                                    musAdres.Varsayilan = true;
                                                }
                                                else
                                                {
                                                    musAdres.Varsayilan = false;
                                                }
                                                musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                                musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                                musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                                musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                                musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                                musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                                musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                                musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                                musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                                musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                                //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                                musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriAdres.Add(musAdres);
                                            }
                                            _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                            _MusteriContext.Commit();
                                        }
                                        else
                                        {
                                            musGenel = new MusteriGenelBilgiler();
                                            musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.AdiUnvan) ? police.GenelBilgiler.PoliceSigortali.AdiUnvan : "..";
                                            musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortali.SoyadiUnvan : "";
                                            musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                                            musGenel.KayitTarihi = DateTime.Now;
                                            musGenel.TVMKodu = tvmKodu;
                                            musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                            musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                            musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                                            musGenel.EMail = police.GenelBilgiler.PoliceSigortali.EMail;
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                            {
                                                musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;
                                            }
                                            else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                            {
                                                musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                                            }

                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;
                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;
                                            }
                                            //                                   musAdres = new MusteriAdre();
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                            musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                            musAdres.SiraNo = 1;
                                            musAdres.Varsayilan = true;
                                            // musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musGenel.MusteriAdres.Add(musAdres);

                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                                musTelefon.SiraNo = 1;
                                                musGenel.MusteriTelefons.Add(musTelefon);
                                            }
                                            else
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = "..";
                                                musTelefon.SiraNo = 1;
                                                musGenel.MusteriTelefons.Add(musTelefon);
                                            }

                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                                musTelefon.SiraNo = 2;
                                                musGenel.MusteriTelefons.Add(musTelefon);
                                            }

                                            _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                            _MusteriContext.Commit();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                {
                                    sigortaliKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                                    MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                    Musteri = this.getMusteri(sigortaliKimlikNo, tvmKodu);
                                    string ilKodu = null;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                    {
                                        ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortali.IlAdi);
                                    }
                                    if (Musteri != null)
                                    {
                                        var polSigortali = police.GenelBilgiler.PoliceSigortali;
                                        MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortali.Adres, polSigortali.IlKodu, polSigortali.Mahalle, polSigortali.Cadde, Convert.ToInt32(polSigortali.IlceKodu), polSigortali.Apartman, polSigortali.Sokak, polSigortali.BinaNo, polSigortali.DaireNo, Musteri.MusteriKodu);
                                        MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortali.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                                musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }


                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {
                                            int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                siraNumarasi += 1;
                                            }
                                            MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortali.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                            if (musTelefonum2 == null)
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                                musTelefon.SiraNo = siraNumarasi;
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }
                                        if (MusteriAdresim == null)
                                        {
                                            musAdres = new MusteriAdre();
                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;
                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;
                                            }
                                            if (Musteri.MusteriAdres.Count == 0)
                                            {
                                                musAdres.Varsayilan = true;
                                            }
                                            else
                                            {
                                                musAdres.Varsayilan = false;
                                            }
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                            //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                            Musteri.MusteriAdres.Add(musAdres);
                                        }
                                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                        _MusteriContext.Commit();
                                    }
                                    else
                                    {
                                        musGenel = new MusteriGenelBilgiler();
                                        musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.AdiUnvan) ? police.GenelBilgiler.PoliceSigortali.AdiUnvan : "..";
                                        musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortali.SoyadiUnvan : "";
                                        musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                                        musGenel.KayitTarihi = DateTime.Now;
                                        musGenel.TVMKodu = tvmKodu;
                                        musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                        musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                        musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                                        musGenel.EMail = police.GenelBilgiler.PoliceSigortali.EMail;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;

                                        }
                                        else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;

                                        }
                                        musAdres = new MusteriAdre();
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres) ? police.GenelBilgiler.PoliceSigortali.Adres : "..";
                                        if (police.GenelBilgiler.BransKodu == 11)
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Ev;
                                        }
                                        else
                                        {
                                            musAdres.AdresTipi = AdresTipleri.Diger;
                                        }
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Apartman) ? police.GenelBilgiler.PoliceSigortali.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortali.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Mahalle) ? police.GenelBilgiler.PoliceSigortali.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde) ? police.GenelBilgiler.PoliceSigortali.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak) ? police.GenelBilgiler.PoliceSigortali.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo) ? police.GenelBilgiler.PoliceSigortali.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo) ? police.GenelBilgiler.PoliceSigortali.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortali.UlkeKodu;
                                        musAdres.PostaKodu = Convert.ToInt32(police.GenelBilgiler.PoliceSigortali.PostaKodu);
                                        musAdres.SiraNo = 1;
                                        musAdres.Varsayilan = true;
                                        musGenel.MusteriAdres.Add(musAdres);

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        else
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = "..";
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.TelefonNo))
                                        {

                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.SiraNo = 2;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }


                                        _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                        _MusteriContext.Commit();
                                    }
                                }
                                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) || !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                                {
                                    sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    MusteriGenelBilgiler Musteri = new MusteriGenelBilgiler();
                                    Musteri = this.getMusteri(sEttirenKimlikNo, tvmKodu);
                                    string ilKodu = null;
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi))
                                    {
                                        ilKodu = this.getIl(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi);
                                    }
                                    if (Musteri != null)
                                    {
                                        var polSigortaEttiren = police.GenelBilgiler.PoliceSigortaEttiren;
                                        MusteriAdre MusteriAdresim = this.getMusteriAdres(polSigortaEttiren.Adres, polSigortaEttiren.IlKodu, polSigortaEttiren.Mahalle, polSigortaEttiren.Cadde, Convert.ToInt32(polSigortaEttiren.IlceKodu), polSigortaEttiren.Apartman, polSigortaEttiren.Cadde, polSigortaEttiren.BinaNo, polSigortaEttiren.DaireNo, Musteri.MusteriKodu);

                                        MusteriTelefon musTelefonum = this.getMusteriTelefon(polSigortaEttiren.MobilTelefonNo, IletisimNumaraTipleri.Cep, Musteri.MusteriKodu);
                                        if (musTelefonum == null)
                                        {
                                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                                musTelefon.SiraNo = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }


                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                        {
                                            int siraNumarasi = this.musTelefonSiraNo(Musteri.MusteriKodu);
                                            if (musTelefonum == null)
                                            {
                                                siraNumarasi += 1;
                                            }
                                            MusteriTelefon musTelefonum2 = this.getMusteriTelefon(polSigortaEttiren.TelefonNo, IletisimNumaraTipleri.Ev, Musteri.MusteriKodu);
                                            if (musTelefonum2 == null)
                                            {
                                                musTelefon = new MusteriTelefon();
                                                musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                                musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                                musTelefon.SiraNo = siraNumarasi;
                                                Musteri.MusteriTelefons.Add(musTelefon);
                                            }
                                        }
                                        if (MusteriAdresim == null)
                                        {
                                            musAdres = new MusteriAdre();
                                            musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                            if (police.GenelBilgiler.BransKodu == 11)
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Ev;
                                            }
                                            else
                                            {
                                                musAdres.AdresTipi = AdresTipleri.Diger;
                                            }
                                            if (Musteri.MusteriAdres.Count == 0)
                                            {
                                                musAdres.Varsayilan = true;
                                            }
                                            else
                                            {
                                                musAdres.Varsayilan = false;
                                            }
                                            musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                            musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                            musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                            musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                            musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                            musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                            musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                            musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                            musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                            //musAdres.MusteriKodu = Musteri.MusteriKodu;
                                            musAdres.SiraNo = this.musAdresSiraNo(Musteri.MusteriKodu);
                                            Musteri.MusteriAdres.Add(musAdres);
                                        }
                                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                                        _MusteriContext.Commit();
                                    }
                                    else
                                    {
                                        musGenel = new MusteriGenelBilgiler();
                                        musGenel.AdiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan : "..";
                                        musGenel.SoyadiUnvan = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan) ? police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan : "";
                                        musGenel.DogumTarihi = police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi;
                                        musGenel.KayitTarihi = DateTime.Now;
                                        musGenel.TVMKodu = tvmKodu;
                                        musGenel.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                                        musGenel.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        musGenel.Cinsiyet = police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet;
                                        musGenel.EMail = police.GenelBilgiler.PoliceSigortaEttiren.EMail;
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TCMusteri;

                                        }
                                        else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                                        {
                                            musGenel.MusteriTipKodu = MusteriTipleri.TuzelMusteri;

                                        }
                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        else
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                                            musTelefon.Numara = "..";
                                            musTelefon.SiraNo = 1;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }

                                        if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo))
                                        {
                                            musTelefon = new MusteriTelefon();
                                            musTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Ev;
                                            musTelefon.Numara = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                                            musTelefon.SiraNo = 2;
                                            musGenel.MusteriTelefons.Add(musTelefon);
                                        }
                                        musAdres = new MusteriAdre();
                                        musAdres.Adres = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres) ? police.GenelBilgiler.PoliceSigortaEttiren.Adres : "..";
                                        musAdres.Apartman = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Apartman) ? police.GenelBilgiler.PoliceSigortaEttiren.Apartman : "..";
                                        musAdres.IlKodu = !String.IsNullOrEmpty(ilKodu) ? ilKodu : "34";
                                        musAdres.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu > 0 ? police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu : 449;
                                        musAdres.Mahalle = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Mahalle) ? police.GenelBilgiler.PoliceSigortaEttiren.Mahalle : "..";
                                        musAdres.Cadde = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde) ? police.GenelBilgiler.PoliceSigortaEttiren.Cadde : "..";
                                        musAdres.Sokak = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak) ? police.GenelBilgiler.PoliceSigortaEttiren.Sokak : "..";
                                        musAdres.BinaNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo) ? police.GenelBilgiler.PoliceSigortaEttiren.BinaNo : "..";
                                        musAdres.DaireNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo) ? police.GenelBilgiler.PoliceSigortaEttiren.DaireNo : "..";
                                        musAdres.UlkeKodu = police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu;
                                        musAdres.SiraNo = 1;
                                        musAdres.Varsayilan = true;
                                        musGenel.MusteriAdres.Add(musAdres);
                                        _MusteriContext.MusteriGenelBilgilerRepository.Create(musGenel);
                                        _MusteriContext.Commit();
                                    }
                                }
                            }
                            #endregion

                            #endregion
                        }

                    }
                    if (!kayitVarMi)
                    {
                        return 1;
                    }
                    else if (police.GenelBilgiler.TUMBirlikKodu == SigortaSirketiBirlikKodlari.AXASIGORTA && (police.GenelBilgiler.BransKodu == 1 || police.GenelBilgiler.BransKodu == 2))
                    {
                        PoliceGenel kayit = _PoliceContext.PoliceGenelRepository.Find(s => s.TVMKodu == police.GenelBilgiler.TVMKodu && s.TUMBirlikKodu == police.GenelBilgiler.TUMBirlikKodu &&
                                                                s.PoliceNumarasi == police.GenelBilgiler.PoliceNumarasi &&
                                                                s.EkNo == police.GenelBilgiler.EkNo &&
                                                                s.YenilemeNo == police.GenelBilgiler.YenilemeNo);
                        if (kayit != null)
                        {
                            if (kayit.PoliceArac.PlakaKodu != null && kayit.PoliceArac.PlakaKodu != string.Empty)
                            {
                                kayit.PoliceArac.PlakaKodu = police.GenelBilgiler.PoliceArac.PlakaKodu;
                                kayit.PoliceArac.PlakaNo = police.GenelBilgiler.PoliceArac.PlakaNo;
                                kayit.PoliceArac.MotorNo = police.GenelBilgiler.PoliceArac.MotorNo;
                                kayit.PoliceArac.SasiNo = police.GenelBilgiler.PoliceArac.SasiNo;
                                kayit.PoliceArac.Renk = police.GenelBilgiler.PoliceArac.Renk;
                                kayit.PoliceArac.KoltukSayisi = police.GenelBilgiler.PoliceArac.KoltukSayisi;
                                kayit.PoliceArac.TrafikCikisTarihi = police.GenelBilgiler.PoliceArac.TrafikCikisTarihi;
                                kayit.PoliceArac.TrafikTescilTarihi = police.GenelBilgiler.PoliceArac.TrafikTescilTarihi;
                                kayit.PoliceArac.SilindirHacmi = police.GenelBilgiler.PoliceArac.SilindirHacmi;
                                kayit.PoliceArac.MotorGucu = police.GenelBilgiler.PoliceArac.MotorGucu;
                                kayit.PoliceArac.TramerBelgeNo = police.GenelBilgiler.PoliceArac.TramerBelgeNo;
                                kayit.PoliceArac.TramerBelgeTarihi = police.GenelBilgiler.PoliceArac.TramerBelgeTarihi;
                                kayit.PoliceArac.TescilSeriNo = police.GenelBilgiler.PoliceArac.TescilSeriNo;
                                _PoliceContext.PoliceAracRepository.Update(kayit.PoliceArac);
                                _PoliceContext.Commit();
                                return 3;
                            }
                            else
                            {

                            }
                        }
                        return 2;
                    }
                    else return 2;
                }

                else
                    return 0;

            }
            catch (Exception ex)
            {
                PoliceKontrol PolEklenmeyen = new PoliceKontrol();
                PolEklenmeyen.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                PolEklenmeyen.EkNo = police.GenelBilgiler.EkNo.Value;
                PolEklenmeyen.YenilemeNo = police.GenelBilgiler.YenilemeNo.Value;
                PolEklenmeyen.Hatatip = "Poliçe formatı Hatalı";
                EklenmeyenPoliceList.Add(PolEklenmeyen);
                return 0;
            }

        }

        public bool UpdatePolice(PoliceGenel police)
        {
            bool result = false;
            try
            {
                _PoliceContext.PoliceGenelRepository.Update(police);
                _PoliceContext.Commit();
                result = true;
                return result;
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public void UpdateOfflinePolice(int policeId, byte Yeni_is)
        {
            bool result = false;
            try
            {
                Police police = GetPoliceById(policeId);
                police.GenelBilgiler.Yeni_is = Yeni_is;
                _PoliceContext.PoliceGenelRepository.Update(police.GenelBilgiler);
                _PoliceContext.Commit();
                result = true;
            }
            catch (Exception ex)
            {

            }
        }

        public bool UpdatePoliceReasuror(Business.Police police)
        {
            bool result = false;
            try
            {
                PoliceSigortaEttiren policeSigortaEttiren = police.GenelBilgiler.PoliceSigortaEttiren;
                _PoliceContext.PoliceSigortaEttirenRepository.Delete(policeSigortaEttiren);


                PoliceSigortali policeSigortali = police.GenelBilgiler.PoliceSigortali;
                _PoliceContext.PoliceSigortaliRepository.Delete(policeSigortali);


                List<PoliceOdemePlani> odemePlani = police.GenelBilgiler.PoliceOdemePlanis.ToList<PoliceOdemePlani>();
                foreach (PoliceOdemePlani item in odemePlani)
                {
                    _PoliceContext.PoliceOdemePlaniRepository.Delete(item);
                }

                List<PoliceTahsilat> tahsilats = police.GenelBilgiler.PoliceTahsilats.ToList<PoliceTahsilat>();
                foreach (PoliceTahsilat item in tahsilats)
                {
                    _PoliceContext.PoliceTahsilatRepository.Delete(item);
                }


                _PoliceContext.Commit();

                _PoliceContext.PoliceGenelRepository.Update(police.GenelBilgiler);

                if (police.policeTahsilat != null && police.policeTahsilat.Count() > 0)
                {
                    foreach (PoliceTahsilat item in police.policeTahsilat)
                    {
                        police.GenelBilgiler.PoliceTahsilats.Add(item);
                    }
                }

                if (police.policeOdemePlani != null && police.policeOdemePlani.Count() > 0)
                {
                    foreach (PoliceOdemePlani item in police.policeOdemePlani)
                    {
                        police.GenelBilgiler.PoliceOdemePlanis.Add(item);
                    }
                }
                if (police.policeSigortaEttiren != null)
                {
                    police.GenelBilgiler.PoliceSigortaEttiren = police.policeSigortaEttiren;
                }
                if (police.policeSigortali != null)
                {
                    police.GenelBilgiler.PoliceSigortali = police.policeSigortali;
                }

                _PoliceContext.Commit();
                result = true;
                return result;
            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }
        public bool UpdatePoliceSigortaEttiren(PoliceSigortaEttiren police)
        {
            bool result = false;
            try
            {
                //var kayitVarmi= _PoliceContext.PoliceSigortaEttirenRepository.All().Where(s=>s.PoliceId==police.PoliceId ).FirstOrDefault();
                if (police != null)
                {
                    _PoliceContext.PoliceSigortaEttirenRepository.Update(police);
                    _PoliceContext.Commit();
                    result = true;
                }

                return result;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public bool DeletePolice(Police police)
        {
            try
            {
                var muhasebeKaydiSilindiMi = this.DeletePoliceMuhasebe(police);
                if (muhasebeKaydiSilindiMi)
                {
                    _PoliceContext.PoliceSigortaliRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    _PoliceContext.PoliceSigortaEttirenRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    _PoliceContext.PoliceAracRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    var policeOdemePlani = police.GenelBilgiler.PoliceOdemePlanis;
                    if (police.GenelBilgiler.PoliceOdemePlanis != null)
                    {
                        _PoliceContext.PoliceOdemePlaniRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    }

                    if (police.GenelBilgiler.PoliceVergis != null)
                    {
                        _PoliceContext.PoliceVergiRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    }
                    if (police.GenelBilgiler.PoliceTahsilats != null)
                    {
                        _PoliceContext.PoliceTahsilatRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                    }

                    _PoliceContext.PoliceRizikoAdresiRepository.Delete(police.GenelBilgiler.PoliceRizikoAdresi);
                    _PoliceContext.PoliceGenelRepository.Delete(police.GenelBilgiler.PoliceId);

                    var uretimSilindiMi = PoliceUretimHedefGerceklesenSil(police.GenelBilgiler);
                    _PoliceContext.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool DeleteSadecePolice(Police police)
        {
            try
            {
                _PoliceContext.PoliceSigortaliRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                _PoliceContext.PoliceSigortaEttirenRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                _PoliceContext.PoliceAracRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                var policeOdemePlani = police.GenelBilgiler.PoliceOdemePlanis;
                if (police.GenelBilgiler.PoliceOdemePlanis != null)
                {
                    _PoliceContext.PoliceOdemePlaniRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                }

                if (police.GenelBilgiler.PoliceVergis != null)
                {
                    _PoliceContext.PoliceVergiRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                }
                if (police.GenelBilgiler.PoliceTahsilats != null)
                {
                    _PoliceContext.PoliceTahsilatRepository.Delete(s => s.PoliceId == police.GenelBilgiler.PoliceId);
                }

                _PoliceContext.PoliceRizikoAdresiRepository.Delete(police.GenelBilgiler.PoliceRizikoAdresi);
                _PoliceContext.PoliceGenelRepository.Delete(police.GenelBilgiler.PoliceId);

                var uretimSilindiMi = PoliceUretimHedefGerceklesenSil(police.GenelBilgiler);
                _PoliceContext.Commit();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool DeletePoliceMuhasebe(Police police)
        {
            try
            {
                string kasaHesabi = "100.01.0000000001";
                string sigortaEttirenKimlik = "120.01.";
                string sigortaSirketVkn = "320.01.";
                string sigortaSirketGelirKomisyon = "600.01.";
                string sigortaSirketGiderKomisyon = "610.01.";

                int tvmKodu = police.GenelBilgiler.TVMKodu.Value;

                string donem = "";
                string ay = "";
                decimal borc = 0;
                decimal alacak = 0;
                string evrakno = "";

                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                {
                    sigortaEttirenKimlik = sigortaEttirenKimlik + police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                }
                else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                {
                    sigortaEttirenKimlik = sigortaEttirenKimlik + police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                }
                else
                {
                    return false;
                }
                var getSirket = _SigortaSirketleriService.GetSirket(police.GenelBilgiler.TUMBirlikKodu);
                if (!String.IsNullOrEmpty(getSirket.VergiNumarasi))
                {
                    sigortaSirketVkn += getSirket.VergiNumarasi;
                    sigortaSirketGelirKomisyon += getSirket.VergiNumarasi;
                    sigortaSirketGiderKomisyon += getSirket.VergiNumarasi;
                }
                else
                {
                    return false;
                }

                if (police.GenelBilgiler.PoliceTahsilats.Count > 0)
                {
                    for (int i = 0; i < police.GenelBilgiler.PoliceTahsilats.Count; i++)
                    {
                        #region Taksiti Ödeyen Cari Hesaptan Muhasebe Kaydı Siliniyor.
                        tvmKodu = police.GenelBilgiler.TVMKodu.Value;
                        var tahsilat = police.GenelBilgiler.PoliceTahsilats.Where(w => w.PoliceId == police.GenelBilgiler.PoliceId && w.TaksitNo == (i + 1)).FirstOrDefault();
                        donem = tahsilat.TaksitVadeTarihi.Year.ToString();
                        ay = tahsilat.TaksitVadeTarihi.Month.ToString();
                        borc = tahsilat.TaksitTutari;
                        alacak = tahsilat.TaksitTutari;
                        evrakno = "";
                        string odeyenCariHesapKodu = tahsilat.CariHesapKodu;
                        if (tahsilat.OdemTipi == TahsilatOdemeTipleri.KrediKarti || tahsilat.OdemTipi == TahsilatOdemeTipleri.Havale || tahsilat.OdemTipi == TahsilatOdemeTipleri.Cek || tahsilat.OdemTipi == TahsilatOdemeTipleri.Senet)
                        {
                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);

                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak);

                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B");
                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            #endregion


                        }
                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.AcenteKrediKarti)
                        {
                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);

                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak);

                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B");
                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            #endregion
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);
                        }

                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.AcenteBireyselKrediKarti)
                        {

                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);

                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak);

                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B");
                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            #endregion
                            // _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            // _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);


                        }

                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.Nakit)
                        {


                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor


                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;
                            if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value > 0)
                            {
                                var odemeAy = tahsilat.OdemeBelgeTarihi.Value.Month.ToString();
                                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, kasaHesabi, donem, odemeAy, borc, 0);
                                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);
                                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, 0, alacak);
                                _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno + "/" + tahsilat.TaksitNo, "A");

                            }
                            else
                            {
                                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                                _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, 0);
                                _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno + "/" + tahsilat.TaksitNo, "B");
                            }

                            _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            _Muhasebe_CariHesapService.deleteCariHareket(kasaHesabi, tvmKodu, evrakno, "B");
                            #endregion

                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);


                            // _Muhasebe_CariHesapService.deleteCariHareket(odeyenCariHesapKodu, tvmKodu, evrakno, "A");
                        }

                        #endregion

                    }
                }
                if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value > 0)
                {
                    #region Sigorta Sirketi Komisyon  Gelir Muhasebeden Siliiyor

                    donem = police.GenelBilgiler.TanzimTarihi.Value.Year.ToString();
                    ay = police.GenelBilgiler.TanzimTarihi.Value.Month.ToString();
                    borc = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    alacak = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;

                    _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, 0);
                    _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B");

                    _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketGelirKomisyon, donem, ay, 0, alacak);
                    _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketGelirKomisyon, tvmKodu, evrakno, "A");
                    #endregion
                }
                else if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value < 0)
                {
                    #region Sigorta Sirketi Komisyon  Gider Muhasebeden Siliiyor

                    donem = police.GenelBilgiler.TanzimTarihi.Value.Year.ToString();
                    ay = police.GenelBilgiler.TanzimTarihi.Value.Month.ToString();
                    borc = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    alacak = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;

                    _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, 0, alacak);
                    _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");

                    _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketGiderKomisyon, donem, ay, borc, 0);
                    _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketGiderKomisyon, tvmKodu, evrakno, "B");
                    #endregion
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// devam edilecek
        /// </summary>
        /// <param name="police"></param>
        /// <returns></returns>
        public bool UpdatePoliceMuhasebe(Police police, decimal brutPrim, decimal komisyonTutari)
        {
            try
            {
                string kasaHesabi = "100.01.0000000001";
                string sigortaEttirenKimlik = "120.01.";
                string sigortaSirketVkn = "320.01.";
                string sigortaSirketGelirKomisyon = "600.01.";
                string sigortaSirketGiderKomisyon = "610.01.";

                int tvmKodu = police.GenelBilgiler.TVMKodu.Value;

                string donem = "";
                string ay = "";
                decimal borc = 0;
                decimal alacak = 0;
                string evrakno = "";

                if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                {
                    sigortaEttirenKimlik = sigortaEttirenKimlik + police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                }
                else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                {
                    sigortaEttirenKimlik = sigortaEttirenKimlik + police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                }
                else
                {
                    return false;
                }
                var getSirket = _SigortaSirketleriService.GetSirket(police.GenelBilgiler.TUMBirlikKodu);
                if (!String.IsNullOrEmpty(getSirket.VergiNumarasi))
                {
                    sigortaSirketVkn += getSirket.VergiNumarasi;
                    sigortaSirketGelirKomisyon += getSirket.VergiNumarasi;
                    sigortaSirketGiderKomisyon += getSirket.VergiNumarasi;
                }
                else
                {
                    return false;
                }

                if (police.GenelBilgiler.PoliceTahsilats.Count > 0)
                {
                    for (int i = 0; i < police.GenelBilgiler.PoliceTahsilats.Count; i++)
                    {
                        #region Taksiti Ödeyen Cari Hesaptan Muhasebe Kaydı Siliniyor.
                        tvmKodu = police.GenelBilgiler.TVMKodu.Value;
                        var tahsilat = police.GenelBilgiler.PoliceTahsilats.Where(w => w.PoliceId == police.GenelBilgiler.PoliceId && w.TaksitNo == (i + 1)).FirstOrDefault();
                        donem = tahsilat.TaksitVadeTarihi.Year.ToString();
                        ay = tahsilat.TaksitVadeTarihi.Month.ToString();
                        borc = tahsilat.TaksitTutari;
                        alacak = tahsilat.TaksitTutari;
                        evrakno = "";
                        string odeyenCariHesapKodu = tahsilat.CariHesapKodu;
                        if (tahsilat.OdemTipi == TahsilatOdemeTipleri.KrediKarti || tahsilat.OdemTipi == TahsilatOdemeTipleri.Havale || tahsilat.OdemTipi == TahsilatOdemeTipleri.Cek || tahsilat.OdemTipi == TahsilatOdemeTipleri.Senet)
                        {
                            _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak, brutPrim, komisyonTutari);

                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak, brutPrim, komisyonTutari);

                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B");  //update olarak düzeltilecek
                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            #endregion


                        }
                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.AcenteKrediKarti)
                        {
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);

                            //#region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak);

                            //evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B"); //update olarak düzeltilecek
                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            #endregion
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);
                        }

                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.AcenteBireyselKrediKarti)
                        {

                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak);

                            //#region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor

                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, alacak);

                            //evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo + "/" + tahsilat.TaksitNo;

                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B"); //update olarak düzeltilecek
                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A");
                            // #endregion
                            // _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            // _Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);


                        }

                        else if (tahsilat.OdemTipi == TahsilatOdemeTipleri.Nakit)
                        {


                            #region Sigorta Şirketinden Muhasebe Kaydı ve Carihareket tablosundaki kaydı siliniyor


                            evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;
                            if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value > 0)
                            {
                                var odemeAy = tahsilat.OdemeBelgeTarihi.Value.Month.ToString();
                                _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, kasaHesabi, donem, odemeAy, borc, 0, brutPrim, 0);
                                _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, borc, alacak, brutPrim, 0);
                                _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, 0, alacak, brutPrim, 0);
                                // _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno + "/" + tahsilat.TaksitNo, "A"); //update olarak düzeltilecek

                            }
                            else
                            {
                                _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak, brutPrim, 0);
                                _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, 0, brutPrim, 0);
                                // _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno + "/" + tahsilat.TaksitNo, "B"); //update olarak düzeltilecek
                            }

                            //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A"); //update olarak düzeltilecek
                            //_Muhasebe_CariHesapService.deleteCariHareket(kasaHesabi, tvmKodu, evrakno, "B");
                            #endregion

                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, odeyenCariHesapKodu, donem, ay, 0, alacak);
                            //_Muhasebe_CariHesapService.UpdateCariHesapBorcAlacak(tvmKodu, sigortaEttirenKimlik, donem, ay, borc, 0);


                            // _Muhasebe_CariHesapService.deleteCariHareket(odeyenCariHesapKodu, tvmKodu, evrakno, "A");
                        }

                        //#endregion

                    }
                }
                if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value > 0)
                {
                    #region Sigorta Sirketi Komisyon  Gelir Muhasebeden Siliiyor

                    donem = police.GenelBilgiler.TanzimTarihi.Value.Year.ToString();
                    ay = police.GenelBilgiler.TanzimTarihi.Value.Month.ToString();
                    borc = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    alacak = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;

                    _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, borc, 0, 0, komisyonTutari);
                    //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "B"); //update olarak düzeltilecek

                    _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketGelirKomisyon, donem, ay, 0, alacak, 0, komisyonTutari);
                    //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketGelirKomisyon, tvmKodu, evrakno, "A"); //update olarak düzeltilecek
                    #endregion
                }
                else if (police.GenelBilgiler.Komisyon.HasValue && police.GenelBilgiler.Komisyon.Value < 0)
                {
                    #region Sigorta Sirketi Komisyon  Gider Muhasebeden Siliiyor

                    donem = police.GenelBilgiler.TanzimTarihi.Value.Year.ToString();
                    ay = police.GenelBilgiler.TanzimTarihi.Value.Month.ToString();
                    borc = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    alacak = police.GenelBilgiler.Komisyon.HasValue ? police.GenelBilgiler.Komisyon.Value : 0;
                    evrakno = police.GenelBilgiler.PoliceNumarasi + "-" + police.GenelBilgiler.YenilemeNo + "-" + police.GenelBilgiler.EkNo;

                    _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketVkn, donem, ay, 0, alacak, 0, komisyonTutari);
                    // _Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketVkn, tvmKodu, evrakno, "A"); //update olarak düzeltilecek

                    _Muhasebe_CariHesapService.UpdateYeniCariHesapBorcAlacak(tvmKodu, sigortaSirketGiderKomisyon, donem, ay, borc, 0, 0, komisyonTutari);
                    //_Muhasebe_CariHesapService.deleteCariHareket(sigortaSirketGiderKomisyon, tvmKodu, evrakno, "B");//update olarak düzeltilecek
                    #endregion
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }



        public SigortaSirketleri getSirketBilgisi(string tumKodu)
        {
            return _PoliceContext.SigortaSirketleriRepository.All().Where(w => w.SirketKodu == tumKodu).First();
        }

        #region Genc Police ve Musteri Ekleme

        public void GencPolicelerAdd(GencListeler policeler)
        {
            foreach (Police item in policeler.policeListesi)
            {
                int result = this.AddPolice(item);
                if (result == 1)
                    eklenenKayit++;
                else if (result == 2)
                {
                    PoliceKontrol PolEklenmeyen = new PoliceKontrol();
                    PolEklenmeyen.PoliceNo = item.GenelBilgiler.PoliceNumarasi;
                    PolEklenmeyen.EkNo = item.GenelBilgiler.EkNo.Value;
                    PolEklenmeyen.YenilemeNo = item.GenelBilgiler.YenilemeNo.Value;
                    PoliceKontrolList.Add(PolEklenmeyen);
                    varolanKayit++;
                }
            }

            //foreach (MusteriGenelBilgiler item in policeler.musteriListesi)
            //{
            //    int result = this.AddGencMusteri(item);
            //    if (result == 1)
            //        eklenenKayit++;
            //    else if (result == 2)
            //    {
            //        //PoliceKontrol PolEklenmeyen = new PoliceKontrol();
            //        //PolEklenmeyen.PoliceNo = item.GenelBilgiler.PoliceNumarasi;
            //        //PolEklenmeyen.EkNo = item.GenelBilgiler.EkNo.Value;
            //        //PolEklenmeyen.YenilemeNo = item.GenelBilgiler.YenilemeNo.Value;
            //        //PoliceKontrolList.Add(PolEklenmeyen);
            //        //varolanKayit++;
            //    }
            //}
        }

        private int AddGencMusteri(MusteriGenelBilgiler musteri)
        {
            try
            {

                bool kayitVarMi = this.MusVarMi(musteri.TVMKodu, musteri.TVMKullaniciKodu, musteri.KimlikNo.ToString());
                if (!kayitVarMi)
                {
                    _MusteriContext.MusteriGenelBilgilerRepository.Create(musteri);
                    _MusteriContext.Commit();
                    return 1;
                }
                else return 2;


            }
            catch (Exception)
            {
                return 0;
            }
        }

        private bool MusVarMi(int tvmKodu, int tvmKullaniciKodu, string kimlikNo)
        {
            MusteriGenelBilgiler kayit = _MusteriContext.MusteriGenelBilgilerRepository.Find(s => s.TVMKodu == tvmKodu && s.TVMKullaniciKodu == tvmKullaniciKodu &&
                                                                 s.KimlikNo == kimlikNo);
            if (kayit != null) return true;
            else return false;

        }

        #endregion

        public int getBasariliKayitlar()
        {
            return this.eklenenKayit;
        }
        public int getUpdateKayit()
        {
            return this.updateKayit;
        }

        public int getVarolanKayitlar()
        {
            return this.varolanKayit;
        }
        public int getHataliEklenmeyenKayitlar()
        {
            return this.hataliEklenmeyenKayit;
        }

        //Hatalı kayıt sebebi ile eklenmeyen poliçeler
        public List<PoliceKontrol> getEklenmeyenPoliceler()
        {
            return this.EklenmeyenPoliceList;
        }

        public List<PoliceKontrol> getVarOlanPoliceler()
        {
            return this.PoliceKontrolList;
        }

        private bool KayitVarMi(int tvmKodu, string TumBirlikKodu, string PoliceNo, int EkNo, int YenilemeNo)
        {
            PoliceGenel kayit = _PoliceContext.PoliceGenelRepository.Find(s => s.TVMKodu == tvmKodu && s.TUMBirlikKodu == TumBirlikKodu &&
                                                                 s.PoliceNumarasi == PoliceNo &&
                                                                 s.EkNo == EkNo &&
                                                                 s.YenilemeNo == YenilemeNo);
            if (kayit != null) return true;
            else return false;

        }
        private bool cariHesapKayitVarMi(int tvmKodu, string cariHesapNo)
        {
            CariHesaplari cariKayit = _MuhasebeContext.CariHesaplariRepository.Find(s => s.TVMKodu == tvmKodu && s.CariHesapKodu == cariHesapNo);

            if (cariKayit != null) return true;
            else return false;

        }

        private PoliceGenel ZeylinPolicesiOnaylanmisMi(int tvmKodu, string TumBirlikKodu, string PoliceNo, int YenilemeNo)
        {
            PoliceGenel kayit = _PoliceContext.PoliceGenelRepository.Find(s => s.TVMKodu == tvmKodu && s.TUMBirlikKodu == TumBirlikKodu &&
                                                                 s.PoliceNumarasi == PoliceNo &&
                                                                 s.TaliAcenteKodu != null &&
                                                                 s.YenilemeNo == YenilemeNo);
            return kayit;

        }

        private PoliceGenel GetPoliceZeyli(int tvmKodu, string TumBirlikKodu, string PoliceNo, int EkNo, int YenilemeNo)
        {


            PoliceGenel kayit = _PoliceContext.PoliceGenelRepository.Find(s => s.TVMKodu == tvmKodu && s.TUMBirlikKodu == TumBirlikKodu &&
                                                                 s.PoliceNumarasi == PoliceNo &&
                                                                 s.EkNo == EkNo &&
                                                                 s.YenilemeNo == YenilemeNo);
            return kayit;

        }
        public KimlikNoUret GetKimlikNoUret()
        {
            KimlikNoUret kayit = _PoliceContext.KimlikNoUretRepository.All().FirstOrDefault();
            return kayit;
        }
        public List<PoliceGenel> GetGuncellenecekPoliceZeyl(int tvmKodu, string TumBirlikKodu, string PoliceNo)
        {
            List<PoliceGenel> kayit = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == tvmKodu && s.TUMBirlikKodu == TumBirlikKodu &&
                                                                 s.PoliceNumarasi == PoliceNo).ToList();
            return kayit;

        }
        private bool TaliPoliceKaydiVarMi(int tvmKodu, int taliTvmKodu, string TumBirlikKodu, string PoliceNo, int EkNo, int YenilemeNo)
        {
            PoliceGenel kayit = _PoliceContext.PoliceGenelRepository.Find(s => s.TVMKodu == tvmKodu && s.TaliAcenteKodu == taliTvmKodu && s.TUMBirlikKodu == TumBirlikKodu &&
                                                                 s.PoliceNumarasi == PoliceNo &&
                                                                 s.EkNo == EkNo &&
                                                                 s.YenilemeNo == YenilemeNo);
            if (kayit != null) return true;
            else return false;

        }

        public class PoliceKontrol
        {
            public string PoliceNo;
            public int EkNo;
            public int YenilemeNo;
            public string Hatatip;
        }

        public Police GetPoliceById(int policeId)
        {
            Police police = new Police();

            police.GenelBilgiler = _PoliceContext.PoliceGenelRepository.Find(s => s.PoliceId == policeId);
            police.GenelBilgiler.PoliceArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == policeId);
            police.GenelBilgiler.PoliceSigortali = _PoliceContext.PoliceSigortaliRepository.Find(s => s.PoliceId == policeId);
            if (!(police.GenelBilgiler.OdemeSekli == 1 && !string.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren?.KimlikNo)))
                police.GenelBilgiler.PoliceSigortaEttiren = _PoliceContext.PoliceSigortaEttirenRepository.Find(s => s.PoliceId == policeId);
            police.GenelBilgiler.PoliceRizikoAdresi = _PoliceContext.PoliceRizikoAdresiRepository.Find(s => s.PoliceId == policeId);

            return police;
        }

        #region offline poliçe ara


        public PoliceOffLineServiceModel PoliceOffLineMustNo(string MustNo, int tvmkodu, DateTime baslangic, DateTime bitis)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttiren = new List<PoliceSigortaEttiren>();
            List<MusteriGenelBilgiler> musterigenelbilgileri = new List<MusteriGenelBilgiler>();
            List<int> polListId = new List<int>();
            MustNo = MustNo.ToLower().Replace('ı', 'i');
            musterigenelbilgileri = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMMusteriKodu.ToLower().Trim().StartsWith(MustNo) && s.TVMKodu == tvmkodu).ToList();
            foreach (var item in musterigenelbilgileri)
            {
                var sigortaEttirenBilgileri = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => (s.KimlikNo == item.KimlikNo || s.VergiKimlikNo == item.KimlikNo) && s.PoliceGenel.TVMKodu == tvmkodu && s.PoliceGenel.TanzimTarihi >= baslangic && s.PoliceGenel.TanzimTarihi <= bitis).ToList();
                if (sigortaEttirenBilgileri.Count > 0)
                {
                    for (int x = 0; x < sigortaEttirenBilgileri.Count(); x++)
                    {
                        polSigortaEttiren.Add(sigortaEttirenBilgileri[x]);
                    }
                }
            }
            if (polSigortaEttiren.Count > 0)
            {
                PolGenelList.OfflineSigortaEttiren = polSigortaEttiren;
            }
            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineTvmKod(int tvmkodu, DateTime baslangic, DateTime bitis)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortaEttiren = new List<PoliceSigortaEttiren>();
            List<MusteriGenelBilgiler> musterigenelbilgileri = new List<MusteriGenelBilgiler>();
            List<int> polListId = new List<int>();

            // musterigenelbilgileri = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s =>  s.TVMKodu == tvmkodu).ToList();
            polSigortaEttiren = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => (s.PoliceGenel.TVMKodu == tvmkodu || s.PoliceGenel.TaliAcenteKodu == tvmkodu) && s.PoliceGenel.TanzimTarihi >= baslangic && s.PoliceGenel.TanzimTarihi <= bitis).ToList();
            //foreach (var item in musterigenelbilgileri)
            //{
            //    var sigortaEttirenBilgileri = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => (s.KimlikNo == item.KimlikNo || s.VergiKimlikNo == item.KimlikNo) && s.PoliceGenel.TVMKodu == tvmkodu && s.PoliceGenel.TanzimTarihi >= baslangic && s.PoliceGenel.TanzimTarihi <= bitis && s.PoliceGenel.TVMKodu==tvmkodu).ToList();
            //    if (sigortaEttirenBilgileri.Count > 0)
            //    {
            //        for (int x = 0; x < sigortaEttirenBilgileri.Count(); x++)
            //        {
            //            polSigortaEttiren.Add(sigortaEttirenBilgileri[x]);
            //        }
            //    }
            //}
            if (polSigortaEttiren.Count > 0)
            {
                PolGenelList.OfflineSigortaEttiren = polSigortaEttiren;
            }
            return PolGenelList;

        }





        public PoliceOffLineServiceModel PoliceOffLineTcVknSigortaEttiren(string TcVkn, int tvmkodu, int donem)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortalilar = new List<PoliceSigortaEttiren>();
            // List<PoliceSigortaEttiren> polSigortaEttirenler = new List<PoliceSigortaEttiren>();

            // PolGenelList.OfflineGenel = _PoliceContext.PoliceAraOfflineByKimlik(tvmkodu, TcVkn).ToList();



            List<int> polListId = new List<int>();
            if (TcVkn.Length == 11)
            {
                polSigortalilar = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.KimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
                if (polSigortalilar.Count > 0)
                {

                    PolGenelList.OfflineSigortaEttiren = polSigortalilar;


                }
            }
            else
            {
                polSigortalilar = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.VergiKimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
                if (polSigortalilar.Count > 0)
                {

                    PolGenelList.OfflineSigortaEttiren = polSigortalilar;
                }
            }

            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineTcVknSigortaEttiren(string TcVkn, int tvmkodu)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortalilar = new List<PoliceSigortaEttiren>();
            // List<PoliceSigortaEttiren> polSigortaEttirenler = new List<PoliceSigortaEttiren>();

            // PolGenelList.OfflineGenel = _PoliceContext.PoliceAraOfflineByKimlik(tvmkodu, TcVkn).ToList();



            List<int> polListId = new List<int>();
            if (TcVkn.Length == 11)
            {
                polSigortalilar = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.KimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu).ToList();
                if (polSigortalilar.Count > 0)
                {

                    PolGenelList.OfflineSigortaEttiren = polSigortalilar;


                }
            }
            else
            {
                polSigortalilar = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.VergiKimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu).ToList();
                if (polSigortalilar.Count > 0)
                {

                    PolGenelList.OfflineSigortaEttiren = polSigortalilar;
                }
            }

            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineUnvanSigortaEttiren(string Unvan, string UnvanSoyad, int tvmkodu, byte durum, int donem)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortalilarUnvan = new List<PoliceSigortaEttiren>();

            string AdSoyad = null;
            Unvan = Unvan.ToLower().Replace('ı', 'i');
            UnvanSoyad = UnvanSoyad.ToLower().Replace('ı', 'i');
            AdSoyad = Unvan + " " + UnvanSoyad;
            AdSoyad.Trim();

            //0 şahıs,1 firma
            if (durum == 0)
            {
                if (Unvan.Length <= 50 || UnvanSoyad.Length <= 50)
                {
                    polSigortalilarUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim() == Unvan
                    && s.SoyadiUnvan.Trim().ToLower() == UnvanSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != ""
                    || s.AdiUnvan.Trim().ToLower() == AdSoyad
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.KimlikNo != ""
                    && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
                    if (polSigortalilarUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortalilarUnvan;
                    }
                }

            }

            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineFirmaUnvanSigortaEttiren(string UnvanFirma, int tvmkodu, byte durum, int donem)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortaEttiren> polSigortalilarUnvan = new List<PoliceSigortaEttiren>();

            UnvanFirma = UnvanFirma.ToLower().Replace('ı', 'i');

            UnvanFirma.Trim();
            List<int> polListId = new List<int>();
            //0 şahıs,1 firma

            if (durum == 1)
            {
                if (UnvanFirma.Length <= 150 || UnvanFirma.Length >= 3)
                {
                    polSigortalilarUnvan = _PoliceContext.PoliceSigortaEttirenRepository.Filter(s => s.AdiUnvan.ToLower().Trim().Contains(UnvanFirma)
                    && s.PoliceGenel.TVMKodu == tvmkodu
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != null
                    && s.PoliceGenel.PoliceSigortaEttiren.VergiKimlikNo != ""
                    && s.PoliceGenel.TanzimTarihi.Value.Year == donem
                    ).ToList();
                    if (polSigortalilarUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortaEttiren = polSigortalilarUnvan;
                    }


                }
            }

            return PolGenelList;

        }

        #region Poliçe Ara Offline Filtre Metotları
        public PoliceOffLineServiceModel PoliceOffLinePolNo(bool merkezAcentemi, string policeNo, int tvmkodu, string donemAraligi)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();

            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            PolGenelList.OfflineGenel = _PoliceContext.PoliceGenelRepository.Filter(s => s.PoliceNumarasi == policeNo &&
            ((merkezAcentemi && s.TVMKodu == tvmkodu) || (!merkezAcentemi && s.TaliAcenteKodu == tvmkodu))
            && (s.TanzimTarihi.Value.Year == donemBaslangic || s.TanzimTarihi.Value.Year == donemBitis)).OrderByDescending(s => s.TanzimTarihi).ToList();

            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineTcVkn(bool merkezAcentemi, string TcVkn, int tvmkodu, string donemAraligi)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortali> polSigortalilar = new List<PoliceSigortali>();
            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            polSigortalilar = _PoliceContext.PoliceSigortaliRepository.Filter(s => (s.KimlikNo == TcVkn || s.VergiKimlikNo == TcVkn) &&
             ((merkezAcentemi && s.PoliceGenel.TVMKodu == tvmkodu) || (!merkezAcentemi && s.PoliceGenel.TaliAcenteKodu == tvmkodu))
            && (s.PoliceGenel.TanzimTarihi.Value.Year == donemBaslangic || s.PoliceGenel.TanzimTarihi.Value.Year == donemBitis)).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
            if (polSigortalilar.Count > 0)
            {
                PolGenelList.OfflineSigortali = polSigortalilar;
            }

            //else
            //{
            //    polSigortalilar = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.VergiKimlikNo == TcVkn && s.PoliceGenel.TVMKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
            //    if (polSigortalilar.Count > 0)
            //    {

            //        PolGenelList.OfflineSigortali = polSigortalilar;
            //    }
            //}

            //if (PolGenelList.OfflineSigortali.Count > 0)
            //{
            //    foreach (var item in PolGenelList.OfflineSigortali)
            //    {
            //        if (item.PoliceGenel.BransKodu == 1 || item.PoliceGenel.BransKodu == 2)
            //        {
            //            PolGenelList.OfflineArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == item.PoliceId);
            //            PolGenelList.OfflineAraclar.Add(PolGenelList.OfflineArac);
            //        }
            //    }
            //}


            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLinePlakaNo(bool merkezAcentemi, string plakaNo, string plakaKodu, int tvmkodu, string donemAraligi)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceArac> polArac = new List<PoliceArac>();
            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            string plk3 = plakaKodu.Trim().PadLeft(3, '0').ToString();
            string plk2 = plakaKodu.Trim().PadLeft(2, '0').ToString();
            string plk1 = plakaKodu.Trim();
            if (plk1.Length == 2)
            {
                plk1 = plk1.Substring(1, 1);
            }

            polArac = _PoliceContext.PoliceAracRepository.Filter(s => (s.PlakaKodu.Trim() == plk1 || s.PlakaKodu.Trim() == plk2 || s.PlakaKodu.Trim() == plk3)
                                                                      && s.PlakaNo.Trim() == plakaNo.Trim() &&
                                                                     ((merkezAcentemi && s.PoliceGenel.TVMKodu == tvmkodu) || (!merkezAcentemi && s.PoliceGenel.TaliAcenteKodu == tvmkodu)) &&
                                                                      (s.PoliceGenel.TanzimTarihi.Value.Year == donemBaslangic || s.PoliceGenel.TanzimTarihi.Value.Year == donemBitis)).ToList();

            if (polArac.Count > 0)
            {
                for (int i = 0; i < polArac.Count(); i++)
                {
                    PolGenelList.OfflineGenel.Add(polArac[i].PoliceGenel);
                }

            }

            //if (PolGenelList.OfflineGenel.Count > 0)
            //{
            //    foreach (var item in PolGenelList.OfflineGenel)
            //    {
            //        if (item.BransKodu == 1 || item.BransKodu == 2)
            //        {
            //            PolGenelList.OfflineArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == item.PoliceId);
            //            PolGenelList.OfflineAraclar.Add(PolGenelList.OfflineArac);
            //        }
            //    }
            //}
            return PolGenelList;
        }
        public PoliceOffLineServiceModel PoliceOffLineUnvan(bool merkezAcentemi, string Unvan, string UnvanSoyad, int tvmkodu, byte durum, string donemAraligi)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortali> polSigortalilarUnvan = new List<PoliceSigortali>();

            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            string AdSoyad = null;
            Unvan = Unvan.ToLower().Replace('ı', 'i');
            UnvanSoyad = UnvanSoyad.ToLower().Replace('ı', 'i');
            AdSoyad = Unvan + " " + UnvanSoyad;
            AdSoyad.Trim();
            //0 şahıs,1 firma
            if (durum == 0)
            {
                if (Unvan.Length <= 50 || UnvanSoyad.Length <= 50)
                {
                    polSigortalilarUnvan = _PoliceContext.PoliceSigortaliRepository.Filter(s => ((s.AdiUnvan.ToLower().Trim() == Unvan
                    && s.SoyadiUnvan.Trim().ToLower() == UnvanSoyad) || s.AdiUnvan.Trim().ToLower() == AdSoyad)
                    && ((merkezAcentemi && s.PoliceGenel.TVMKodu == tvmkodu) || (!merkezAcentemi && s.PoliceGenel.TaliAcenteKodu == tvmkodu))
                    && s.PoliceGenel.PoliceSigortali.KimlikNo != null
                    && s.PoliceGenel.PoliceSigortali.KimlikNo != ""
                    && (s.PoliceGenel.TanzimTarihi.Value.Year == donemBaslangic || s.PoliceGenel.TanzimTarihi.Value.Year == donemBitis)).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
                    if (polSigortalilarUnvan.Count > 0)
                    {
                        for (int i = 0; i < polSigortalilarUnvan.Count; i++)
                        {
                            PolGenelList.OfflineGenel.Add(polSigortalilarUnvan[i].PoliceGenel);
                        }
                    }
                }
            }
            return PolGenelList;

        }
        public PoliceOffLineServiceModel PoliceOffLineFirmaUnvan(bool merkezAcentemi, string UnvanFirma, int tvmkodu, byte durum, string donemAraligi)
        {
            PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
            List<PoliceSigortali> polSigortalilarUnvan = new List<PoliceSigortali>();
            var partsDonem = donemAraligi.Split('-');
            int donemBaslangic = Convert.ToInt32(partsDonem[0]);
            int donemBitis = Convert.ToInt32(partsDonem[1]);

            UnvanFirma = UnvanFirma.ToLower().Replace('ı', 'i');

            UnvanFirma.Trim();
            //0 şahıs,1 firma

            if (durum == 1)
            {
                if (UnvanFirma.Length <= 150 || UnvanFirma.Length >= 3)
                {
                    polSigortalilarUnvan = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.AdiUnvan.ToLower().Trim().Contains(UnvanFirma)
                    && ((merkezAcentemi && s.PoliceGenel.TVMKodu == tvmkodu) || (!merkezAcentemi && s.PoliceGenel.TaliAcenteKodu == tvmkodu))
                    && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != null
                    && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != ""
                    && (s.PoliceGenel.TanzimTarihi.Value.Year == donemBaslangic || s.PoliceGenel.TanzimTarihi.Value.Year == donemBitis)
                    ).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
                    if (polSigortalilarUnvan.Count > 0)
                    {
                        PolGenelList.OfflineSigortali = polSigortalilarUnvan;
                    }


                }
            }

            return PolGenelList;

        }

        #endregion

        #region Gereksiz Tali Kodları

        //public PoliceOffLineServiceModel PoliceOffLinePolNoTali(string policeNo, int tvmkodu, int donem)
        //{
        //    PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();

        //    List<int> polListId = new List<int>();
        //    PolGenelList.OfflineGenel = _PoliceContext.PoliceGenelRepository.Filter(s => s.PoliceNumarasi == policeNo && s.TaliAcenteKodu == tvmkodu && s.TanzimTarihi.Value.Year == donem).OrderByDescending(s => s.TanzimTarihi).ToList();

        //    foreach (var item in PolGenelList.OfflineGenel)
        //    {
        //        if (item.BransKodu == 1 || item.BransKodu == 2)
        //        {
        //            PolGenelList.OfflineArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == item.PoliceId);
        //            PolGenelList.OfflineAraclar.Add(PolGenelList.OfflineArac);
        //        }
        //    }

        //    return PolGenelList;

        //}

        //public PoliceOffLineServiceModel PoliceOffLineTcVknTali(string TcVkn, int tvmkodu, int donem)
        //{
        //    PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
        //    List<PoliceSigortali> polSigortalilar = new List<PoliceSigortali>();
        //    // List<PoliceSigortaEttiren> polSigortaEttirenler = new List<PoliceSigortaEttiren>();

        //    // PolGenelList.OfflineGenel = _PoliceContext.PoliceAraOfflineByKimlik(tvmkodu, TcVkn).ToList();



        //    List<int> polListId = new List<int>();
        //    if (TcVkn.Length == 11)
        //    {
        //        polSigortalilar = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.KimlikNo == TcVkn && s.PoliceGenel.TaliAcenteKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
        //        if (polSigortalilar.Count > 0)
        //        {

        //            PolGenelList.OfflineSigortali = polSigortalilar;


        //        }
        //    }
        //    else
        //    {
        //        polSigortalilar = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.VergiKimlikNo == TcVkn && s.PoliceGenel.TaliAcenteKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
        //        if (polSigortalilar.Count > 0)
        //        {

        //            PolGenelList.OfflineSigortali = polSigortalilar;
        //        }
        //    }

        //    //if (PolGenelList.OfflineSigortali.Count > 0)
        //    //{
        //    //    foreach (var item in PolGenelList.OfflineSigortali)
        //    //    {
        //    //        if (item.PoliceGenel.BransKodu == 1 || item.PoliceGenel.BransKodu == 2)
        //    //        {
        //    //            PolGenelList.OfflineArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == item.PoliceId);
        //    //            PolGenelList.OfflineAraclar.Add(PolGenelList.OfflineArac);
        //    //        }
        //    //    }
        //    //}


        //    return PolGenelList;

        //}
        //public PoliceOffLineServiceModel PoliceOffLineUnvanTali(string Unvan, string UnvanSoyad, int tvmkodu, byte durum, int donem)
        //{
        //    PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
        //    List<PoliceSigortali> polSigortalilarUnvan = new List<PoliceSigortali>();
        //    string AdSoyad = null;
        //    Unvan = Unvan.ToLower().Replace('ı', 'i');
        //    UnvanSoyad = UnvanSoyad.ToLower().Replace('ı', 'i');
        //    AdSoyad = Unvan + " " + UnvanSoyad;
        //    AdSoyad.Trim();
        //    List<int> polListId = new List<int>();
        //    //0 şahıs,1 firma
        //    if (durum == 0)
        //    {
        //        if (Unvan.Length <= 50 || UnvanSoyad.Length <= 50)
        //        {
        //            polSigortalilarUnvan = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.AdiUnvan.ToLower().Trim() == Unvan
        //            && s.SoyadiUnvan.Trim().ToLower() == UnvanSoyad
        //            && s.PoliceGenel.TVMKodu == tvmkodu
        //            && s.PoliceGenel.PoliceSigortali.KimlikNo != null
        //            && s.PoliceGenel.PoliceSigortali.KimlikNo != ""
        //            || s.AdiUnvan.Trim().ToLower() == AdSoyad
        //            && s.PoliceGenel.TVMKodu == tvmkodu
        //            && s.PoliceGenel.PoliceSigortali.KimlikNo != null
        //            && s.PoliceGenel.PoliceSigortali.KimlikNo != ""
        //            && s.PoliceGenel.TanzimTarihi.Value.Year == donem).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
        //            if (polSigortalilarUnvan.Count > 0)
        //            {
        //                PolGenelList.OfflineSigortali = polSigortalilarUnvan;
        //            }

        //        }

        //    }


        //    return PolGenelList;

        //}
        //public PoliceOffLineServiceModel PoliceOffLineFirmaUnvanTali(string UnvanFirma, int tvmkodu, byte durum, int donem)
        //{
        //    PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
        //    List<PoliceSigortali> polSigortalilarUnvan = new List<PoliceSigortali>();
        //    string AdSoyad = null;

        //    UnvanFirma = UnvanFirma.ToLower().Replace('ı', 'i');
        //    AdSoyad.Trim();
        //    List<int> polListId = new List<int>();
        //    //0 şahıs,1 firma

        //    if (durum == 1)
        //    {
        //        if (UnvanFirma.Length <= 150 || UnvanFirma.Length >= 3)
        //        {
        //            polSigortalilarUnvan = _PoliceContext.PoliceSigortaliRepository.Filter(s => s.AdiUnvan.ToLower().Trim().Contains(UnvanFirma)
        //            && s.PoliceGenel.TVMKodu == tvmkodu
        //            && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != null
        //            && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != ""
        //            || s.AdiUnvan.Trim().ToLower().Contains(AdSoyad)
        //            && s.PoliceGenel.TVMKodu == tvmkodu
        //            && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != null
        //            && s.PoliceGenel.PoliceSigortali.VergiKimlikNo != ""
        //            && s.PoliceGenel.TanzimTarihi.Value.Year == donem).OrderByDescending(s => s.PoliceGenel.TanzimTarihi).ToList();
        //            if (polSigortalilarUnvan.Count > 0)
        //            {
        //                PolGenelList.OfflineSigortali = polSigortalilarUnvan;
        //            }


        //        }
        //    }


        //    return PolGenelList;

        //}

        //public PoliceOffLineServiceModel PoliceOffLinePlakaNoTali(string plakaNo, string plakaKodu, int tvmkodu, int donem)
        //{
        //    PoliceOffLineServiceModel PolGenelList = new PoliceOffLineServiceModel();
        //    List<PoliceArac> polArac = new List<PoliceArac>();
        //    List<int> polListId = new List<int>();

        //    polArac = _PoliceContext.PoliceAracRepository.Filter(s => s.PlakaKodu == plakaKodu && s.PlakaNo == plakaNo && s.PoliceGenel.TaliAcenteKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
        //    if (polArac.Count == 0)
        //    {
        //        plakaKodu = "0" + plakaKodu;
        //        polArac = _PoliceContext.PoliceAracRepository.Filter(s => s.PlakaKodu == plakaKodu && s.PlakaNo == plakaNo && s.PoliceGenel.TaliAcenteKodu == tvmkodu && s.PoliceGenel.TanzimTarihi.Value.Year == donem).ToList();
        //    }
        //    if (polArac.Count > 0)
        //    {
        //        foreach (var item in polArac)
        //        {
        //            PolGenelList.OfflineGenel.Add(_PoliceContext.PoliceGenelRepository.Find(s => s.PoliceId == item.PoliceId));

        //        }
        //    }


        //    if (PolGenelList.OfflineGenel.Count > 0)
        //    {
        //        foreach (var item in PolGenelList.OfflineGenel)
        //        {
        //            if (item.BransKodu == 1 || item.BransKodu == 2)
        //            {
        //                PolGenelList.OfflineArac = _PoliceContext.PoliceAracRepository.Find(s => s.PoliceId == item.PoliceId);
        //                PolGenelList.OfflineAraclar.Add(PolGenelList.OfflineArac);
        //            }
        //        }
        //    }

        //    return PolGenelList;

        //}

        #endregion
        #endregion

        #region tahsilat kayıt

        public bool CreatePoliceTahsilat(PoliceTahsilat model)
        {
            bool result;
            try
            {
                _PoliceContext.PoliceTahsilatRepository.Create(model);
                _PoliceContext.Commit();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        public bool UpdatePoliceTahsilat(PoliceTahsilat model)
        {
            bool result = false;
            try
            {
                var polTahDetay = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.TahsilatId == model.TahsilatId).FirstOrDefault();
                if (polTahDetay != null)
                {
                    polTahDetay.OdenenTutar = model.OdenenTutar;
                    polTahDetay.KalanTaksitTutari = model.KalanTaksitTutari;
                    polTahDetay.KayitTarihi = TurkeyDateTime.Today.Date;
                    polTahDetay.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                    polTahDetay.OdemTipi = Convert.ToInt32(model.OdemTipi);
                    polTahDetay.OdemeBelgeNo = model.OdemeBelgeNo;
                    polTahDetay.OdemeBelgeTarihi = model.OdemeBelgeTarihi;
                    polTahDetay.Dekont_EvrakNo = model.Dekont_EvrakNo;
                    polTahDetay.CariHesapKodu = model.CariHesapKodu;
                    polTahDetay.OtomatikTahsilatiKkMi = model.OtomatikTahsilatiKkMi;
                    polTahDetay.KaydiEkleyenKullaniciKodu = model.KaydiEkleyenKullaniciKodu;
                    _PoliceContext.PoliceTahsilatRepository.Update(polTahDetay);
                    _PoliceContext.Commit();
                    result = true;
                }

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public PoliceOdemePlani GetPoliceOdemePlani(int policeId, int taksitSayisi)
        {
            PoliceOdemePlani police = new PoliceOdemePlani();


            police = _PoliceContext.PoliceOdemePlaniRepository.Find(s => s.PoliceId == policeId && s.TaksitNo == taksitSayisi);

            return police;
        }
        public PoliceOdemePlani GetTopluTahsilatPoliceOdemePlani(int policeId)
        {
            PoliceOdemePlani police = new PoliceOdemePlani();


            police = _PoliceContext.PoliceOdemePlaniRepository.Find(s => s.PoliceId == policeId);

            return police;
        }
        public PoliceTahsilat GetPoliceTahsilat(int policeId, int taksitSayisi)
        {
            PoliceTahsilat police = new PoliceTahsilat();


            police = _PoliceContext.PoliceTahsilatRepository.Find(s => s.PoliceId == policeId && s.TaksitNo == taksitSayisi);

            return police;
        }
        #endregion

        public class PoliceOffLineServiceModel
        {
            public List<PoliceGenel> OfflineGenel { get; set; }
            public List<PoliceArac> OfflineAraclar { get; set; }
            public PoliceArac OfflineArac { get; set; }
            public List<PoliceSigortali> OfflineSigortali { get; set; }
            public List<PoliceSigortaEttiren> OfflineSigortaEttiren { get; set; }

            public PoliceOffLineServiceModel()
            {

                OfflineAraclar = new List<PoliceArac>();
                OfflineGenel = new List<PoliceGenel>();
                OfflineSigortali = new List<PoliceSigortali>();
            }
        }

        public PoliceGenel getPolice(int? tvmKodu, string sigortaSirketiKodu, string policeNo, int ekNo)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TaliAcenteKodu == tvmKodu
                                                                   && s.TUMBirlikKodu == sigortaSirketiKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo).FirstOrDefault();

            return pol;
        }



        public PoliceGenel getManuelPolice(string sigortaSirketiKodu, string policeNo, int ekNo, int yenilemeNo, int bransKodu)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == _AktifKullanici.TVMKodu
                                                                   && s.TUMBirlikKodu == sigortaSirketiKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo
                                                                   && s.YenilemeNo == yenilemeNo
                                                                   && s.BransKodu == bransKodu
                                                                   && s.Durum == PoliceDurumlari.ManuelGiris).FirstOrDefault();

            return pol;
        }
        public PoliceGenel getManuelPolice(string policeNo, int ekNo, int yenilemeNo)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == _AktifKullanici.TVMKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo
                                                                   && s.YenilemeNo == yenilemeNo
                                                                   && s.Durum == PoliceDurumlari.ManuelGiris).FirstOrDefault();

            return pol;
        }
        public PoliceGenel getOfflinePolice(string sigortaSirketiKodu, string policeNo, int ekNo, int yenilemeNo, int bransKodu)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == _AktifKullanici.TVMKodu
                                                                   && s.TUMBirlikKodu == sigortaSirketiKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo
                                                                   && s.YenilemeNo == yenilemeNo
                                                                   && s.BransKodu == bransKodu
                                                                   && s.Durum == PoliceDurumlari.Hepsi).FirstOrDefault();

            return pol;
        }

        public PoliceGenel getZeylinPolicesi(int tvmKodu, string sigortaSirketiKodu, string policeNo, int ekNo)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == tvmKodu
                                                                   && s.TUMBirlikKodu == sigortaSirketiKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo
                                                                   && s.TaliAcenteKodu != null).FirstOrDefault();

            return pol;
        }

        public PoliceGenel GetPolice(int policeId)
        {
            PoliceGenel police = new PoliceGenel();
            police = _PoliceContext.PoliceGenelRepository.Find(s => s.PoliceId == policeId);

            return police;
        }

        private bool AddMissingInstallments(Police police)
        {
            var actualPolice = _PoliceContext.PoliceGenelRepository.All().Where(p => p.TVMKodu == police.GenelBilgiler.TVMKodu
                                                                                && p.PoliceNumarasi == police.GenelBilgiler.PoliceNumarasi
                                                                                && p.EkNo == police.GenelBilgiler.EkNo
                                                                                && p.YenilemeNo == police.GenelBilgiler.YenilemeNo).FirstOrDefault();

            if (actualPolice.PoliceTahsilats.Count != police.GenelBilgiler.PoliceTahsilats.Count)
            {

                #region PoliceTahsilat
                var tahsilats = _PoliceContext.PoliceTahsilatRepository.All().Where(t => t.PoliceId == actualPolice.PoliceId).ToList();
                foreach (var tahsilat in tahsilats)
                {
                    _PoliceContext.PoliceTahsilatRepository.Delete(tahsilat);
                }
                _PoliceContext.Commit();

                for (int i = actualPolice.PoliceTahsilats.Count; i <= police.GenelBilgiler.PoliceTahsilats.Count - 1; i++)
                {
                    var tahsilat = police.GenelBilgiler.PoliceTahsilats.ElementAt(i);
                    tahsilat.OdenenTutar = 0;
                    actualPolice.PoliceTahsilats.Add(tahsilat);
                }
                #endregion

                #region PoliceOdemePlani
                var odemePlanis = _PoliceContext.PoliceOdemePlaniRepository.All().Where(o => o.PoliceId == actualPolice.PoliceId).ToList();
                foreach (var odemePlani in odemePlanis)
                {
                    _PoliceContext.PoliceOdemePlaniRepository.Delete(odemePlani);
                }
                _PoliceContext.Commit();

                for (int i = actualPolice.PoliceOdemePlanis.Count; i <= police.GenelBilgiler.PoliceOdemePlanis.Count - 1; i++)
                {
                    var odemePlani = police.GenelBilgiler.PoliceOdemePlanis.ElementAt(i);
                    actualPolice.PoliceOdemePlanis.Add(odemePlani);
                }
                #endregion
                _PoliceContext.PoliceGenelRepository.Update(actualPolice);
                _PoliceContext.Commit();
                return true;
            }

            return false;
        }

        public PoliceGenel getTahsilatPolice(string sigortaSirketiKodu, string policeNo, int ekNo)
        {
            PoliceGenel pol = new PoliceGenel();
            pol = _PoliceContext.PoliceGenelRepository.All().Where(s => s.TUMBirlikKodu == sigortaSirketiKodu
                                                                   && s.PoliceNumarasi == policeNo
                                                                   && s.EkNo == ekNo).FirstOrDefault();

            return pol;
        }

        public PoliceGenel TaliAcnteKomisyonGuncelle(PoliceGenel police, out bool guncellendiMi)
        {
            guncellendiMi = false;
            try
            {
                _KomisyonContext.PoliceGenelRepository.Update(police);
                _KomisyonContext.Commit();
                guncellendiMi = true;

                string sigortaliKimlikNo = "";
                string sEttirenKimlikNo = "";
                int tvmKodu = 0;
                int musTvmKodu = 0;
                if (police.TaliAcenteKodu != null && police.UretimTaliAcenteKodu != null)
                {
                    tvmKodu = police.TaliAcenteKodu.Value;
                }
                else if (police.TaliAcenteKodu != null && police.UretimTaliAcenteKodu == null)
                {
                    tvmKodu = police.TaliAcenteKodu.Value;
                }

                if (!String.IsNullOrEmpty(police.PoliceSigortali.KimlikNo) || !String.IsNullOrEmpty(police.PoliceSigortali.VergiKimlikNo))
                {
                    sigortaliKimlikNo = !String.IsNullOrEmpty(police.PoliceSigortali.KimlikNo) ? police.PoliceSigortali.KimlikNo : police.PoliceSigortali.VergiKimlikNo;

                    int KontrolTVMKod = _TVMService.GetDetay(tvmKodu).BagliOlduguTVMKodu;
                    if (KontrolTVMKod == -9999)
                    {
                        musTvmKodu = tvmKodu;
                    }
                    else
                    {
                        musTvmKodu = KontrolTVMKod;
                    }
                    MusteriGenelBilgiler Musteri = this.getMusteriler(sigortaliKimlikNo, musTvmKodu);
                    if (Musteri != null)
                    {
                        Musteri.TVMKodu = tvmKodu;
                        var kullanici = this.GetMusteriTVMKullanicilarByTVMKodu(tvmKodu);
                        if (kullanici != null)
                        {
                            Musteri.TVMKullaniciKodu = kullanici.KullaniciKodu;
                        }
                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                        _MusteriContext.Commit();
                    }
                }
                else
                {
                    sEttirenKimlikNo = !String.IsNullOrEmpty(police.PoliceSigortaEttiren.KimlikNo) ? police.PoliceSigortaEttiren.KimlikNo : police.PoliceSigortaEttiren.VergiKimlikNo;
                    int KontrolTVMKod = _TVMService.GetDetay(tvmKodu).BagliOlduguTVMKodu;
                    if (KontrolTVMKod == -9999)
                    {
                        musTvmKodu = tvmKodu;
                    }
                    else
                    {
                        musTvmKodu = KontrolTVMKod;
                    }
                    MusteriGenelBilgiler Musteri = this.getMusteriler(sEttirenKimlikNo, musTvmKodu);
                    if (Musteri != null)
                    {
                        Musteri.TVMKodu = tvmKodu;
                        var kullanici = this.GetMusteriTVMKullanicilarByTVMKodu(tvmKodu);
                        if (kullanici != null)
                        {
                            Musteri.TVMKullaniciKodu = kullanici.KullaniciKodu;
                        }
                        _MusteriContext.MusteriGenelBilgilerRepository.Update(Musteri);
                        _MusteriContext.Commit();
                    }
                }

                return police;
            }
            catch (Exception)
            {
                guncellendiMi = false;
                return police;
            }
        }

        public bool GetPoliceUretimHedefGerceklesen(PoliceGenel police)
        {
            PoliceUretimHedefGerceklesen gerceklesenUretimPolice = null;
            PoliceUretimHedefGerceklesen gerceklesenUretimTaliGuncelle = null;

            bool guncellendiMi = false;
            bool uretimYeniMiEkleniyor = false;
            int taliKodu = 0;
            try
            {
                if (police != null)
                {
                    if (police.TaliAcenteKodu != null)
                    {
                        taliKodu = police.TaliAcenteKodu.Value;
                    }
                    //else if (police.TaliAcenteKodu == null && police.UretimTaliAcenteKodu != null)
                    //{
                    //    taliKodu = police.UretimTaliAcenteKodu.Value;
                    //}

                    if (police.TanzimTarihi.HasValue && police.BransKodu != BransListeCeviri.TanimsizBransKodu)
                    {
                        gerceklesenUretimPolice = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == police.TVMKodu &&
                                                                                      s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();

                        gerceklesenUretimTaliGuncelle = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == taliKodu &&
                                                                                     s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();

                    }

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
                                if (gerceklesenUretimTaliGuncelle.Prim1.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 0;
                                    }
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
                                    gerceklesenUretimPolice.PoliceAdedi1 = gerceklesenUretimPolice.PoliceAdedi1.HasValue ? (gerceklesenUretimPolice.PoliceAdedi1.Value - 1) : gerceklesenUretimPolice.PoliceAdedi1;
                                    if (gerceklesenUretimTaliGuncelle.Prim1.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi1 = gerceklesenUretimTaliGuncelle.PoliceAdedi1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceAdedi1.Value + 1 : gerceklesenUretimTaliGuncelle.PoliceAdedi1;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi1 = gerceklesenUretimPolice.PoliceAdedi1.HasValue ? (gerceklesenUretimPolice.PoliceAdedi1.Value - 1) : gerceklesenUretimPolice.PoliceAdedi1.Value;

                                if (gerceklesenUretimTaliGuncelle.Prim1.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi1 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi1 = gerceklesenUretimTaliGuncelle.PoliceAdedi1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceAdedi1.Value + 1 : gerceklesenUretimTaliGuncelle.PoliceAdedi1;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi1 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari1 = gerceklesenUretimPolice.PoliceKomisyonTutari1.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari1.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari1;
                            gerceklesenUretimPolice.Prim1 = gerceklesenUretimPolice.Prim1.HasValue ? (gerceklesenUretimPolice.Prim1.Value - police.NetPrim) : gerceklesenUretimPolice.Prim1;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim1 = gerceklesenUretimTaliGuncelle.Prim1.HasValue ? gerceklesenUretimTaliGuncelle.Prim1.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari1 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari1 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim2.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim2.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi2 = gerceklesenUretimTaliGuncelle.PoliceAdedi2.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi2.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi2;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi2 = gerceklesenUretimPolice.PoliceAdedi2.HasValue ? (gerceklesenUretimPolice.PoliceAdedi2.Value - 1) : gerceklesenUretimPolice.PoliceAdedi2;

                                if (gerceklesenUretimTaliGuncelle.Prim2.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi2 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi2 = gerceklesenUretimTaliGuncelle.PoliceAdedi2.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi2.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi2;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi2 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari2 = gerceklesenUretimPolice.PoliceKomisyonTutari2.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari2.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari2;
                            gerceklesenUretimPolice.Prim2 = gerceklesenUretimPolice.Prim2.HasValue ? (gerceklesenUretimPolice.Prim2.Value - police.NetPrim) : gerceklesenUretimPolice.Prim2;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim2 = gerceklesenUretimTaliGuncelle.Prim2.HasValue ? gerceklesenUretimTaliGuncelle.Prim2.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari2 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari2 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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
                                if (gerceklesenUretimTaliGuncelle.Prim3.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 0;
                                    }
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
                                    if (gerceklesenUretimTaliGuncelle.Prim3.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi3 = gerceklesenUretimTaliGuncelle.PoliceAdedi3.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi3.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi3;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi3 = gerceklesenUretimPolice.PoliceAdedi3.HasValue ? (gerceklesenUretimPolice.PoliceAdedi3.Value - 1) : gerceklesenUretimPolice.PoliceAdedi3;

                                if (gerceklesenUretimTaliGuncelle.Prim3.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi3 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi3 = gerceklesenUretimTaliGuncelle.PoliceAdedi3.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi3.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi3;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi3 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari3 = gerceklesenUretimPolice.PoliceKomisyonTutari3.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari3.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari3;
                            gerceklesenUretimPolice.Prim3 = gerceklesenUretimPolice.Prim3.HasValue ? (gerceklesenUretimPolice.Prim3.Value - police.NetPrim) : gerceklesenUretimPolice.Prim3;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim3 = gerceklesenUretimTaliGuncelle.Prim3.HasValue ? gerceklesenUretimTaliGuncelle.Prim3.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari3 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari3 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim4.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim4.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi4 = gerceklesenUretimTaliGuncelle.PoliceAdedi4.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi4.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi4;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi4 = gerceklesenUretimPolice.PoliceAdedi4.HasValue ? (gerceklesenUretimPolice.PoliceAdedi4.Value - 1) : gerceklesenUretimPolice.PoliceAdedi4;

                                if (gerceklesenUretimTaliGuncelle.Prim4.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi4 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi4 = gerceklesenUretimTaliGuncelle.PoliceAdedi4.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi4.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi4;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi4 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari4 = gerceklesenUretimPolice.PoliceKomisyonTutari4.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari4.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari4;
                            gerceklesenUretimPolice.Prim4 = gerceklesenUretimPolice.Prim4.HasValue ? (gerceklesenUretimPolice.Prim4.Value - police.NetPrim) : gerceklesenUretimPolice.Prim4;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim4 = gerceklesenUretimTaliGuncelle.Prim4.HasValue ? gerceklesenUretimTaliGuncelle.Prim4.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari4 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari4 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim5.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim5.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi5 = gerceklesenUretimTaliGuncelle.PoliceAdedi5.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi5.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi5;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi5 = gerceklesenUretimPolice.PoliceAdedi5.HasValue ? (gerceklesenUretimPolice.PoliceAdedi5.Value - 1) : gerceklesenUretimPolice.PoliceAdedi5;

                                if (gerceklesenUretimTaliGuncelle.Prim5.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi5 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi5 = gerceklesenUretimTaliGuncelle.PoliceAdedi5.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi5.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi5;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi5 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari5 = gerceklesenUretimPolice.PoliceKomisyonTutari5.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari5.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari5;
                            gerceklesenUretimPolice.Prim5 = gerceklesenUretimPolice.Prim5.HasValue ? (gerceklesenUretimPolice.Prim5.Value - police.NetPrim) : gerceklesenUretimPolice.Prim5;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim5 = gerceklesenUretimTaliGuncelle.Prim5.HasValue ? gerceklesenUretimTaliGuncelle.Prim5.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari5 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari5 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim6.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;

                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim6.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;

                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi6 = gerceklesenUretimTaliGuncelle.PoliceAdedi6.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi6.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi6;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi6 = gerceklesenUretimPolice.PoliceAdedi6.HasValue ? (gerceklesenUretimPolice.PoliceAdedi6.Value - 1) : gerceklesenUretimPolice.PoliceAdedi6;

                                if (gerceklesenUretimTaliGuncelle.Prim6.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;

                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi6 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi6 = gerceklesenUretimTaliGuncelle.PoliceAdedi6.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi6.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi6;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi6 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari6 = gerceklesenUretimPolice.PoliceKomisyonTutari6.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari6.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari6;
                            gerceklesenUretimPolice.Prim6 = gerceklesenUretimPolice.Prim6.HasValue ? (gerceklesenUretimPolice.Prim6.Value - police.NetPrim) : gerceklesenUretimPolice.Prim6;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim6 = gerceklesenUretimTaliGuncelle.Prim6.HasValue ? gerceklesenUretimTaliGuncelle.Prim6.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari6 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari6 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim7.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim7.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi7 = gerceklesenUretimTaliGuncelle.PoliceAdedi7.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi7.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi7;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi7 = gerceklesenUretimPolice.PoliceAdedi7.HasValue ? (gerceklesenUretimPolice.PoliceAdedi7.Value - 1) : gerceklesenUretimPolice.PoliceAdedi7;

                                if (gerceklesenUretimTaliGuncelle.Prim7.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi7 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi7 = gerceklesenUretimTaliGuncelle.PoliceAdedi7.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi7.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi7;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi7 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari7 = gerceklesenUretimPolice.PoliceKomisyonTutari7.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari7.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari7;
                            gerceklesenUretimPolice.Prim7 = gerceklesenUretimPolice.Prim7.HasValue ? (gerceklesenUretimPolice.Prim7.Value - police.NetPrim) : gerceklesenUretimPolice.Prim7;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim7 = gerceklesenUretimTaliGuncelle.Prim7.HasValue ? gerceklesenUretimTaliGuncelle.Prim7.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari7 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari7 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim8.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                                }
                            }
                            else if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                            {
                                if (police.EkNo.ToString().Substring(4, 1) == "1")
                                {
                                    gerceklesenUretimPolice.PoliceAdedi8 = gerceklesenUretimPolice.PoliceAdedi8.HasValue ? (gerceklesenUretimPolice.PoliceAdedi8.Value - 1) : gerceklesenUretimPolice.PoliceAdedi8;

                                    if (gerceklesenUretimTaliGuncelle.Prim8.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi8 = gerceklesenUretimPolice.PoliceAdedi8.HasValue ? (gerceklesenUretimPolice.PoliceAdedi8.Value - 1) : gerceklesenUretimPolice.PoliceAdedi8;

                                if (gerceklesenUretimTaliGuncelle.Prim8.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi8 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi8 = gerceklesenUretimTaliGuncelle.PoliceAdedi8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi8.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi8;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi8 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari8 = gerceklesenUretimPolice.PoliceKomisyonTutari8.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari8.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari8;
                            gerceklesenUretimPolice.Prim8 = gerceklesenUretimPolice.Prim8.HasValue ? (gerceklesenUretimPolice.Prim8.Value - police.NetPrim) : gerceklesenUretimPolice.Prim8;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8.Value + police.Komisyon) : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8.HasValue ? (gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8.Value + police.TaliKomisyon) : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim8 = gerceklesenUretimTaliGuncelle.Prim8.HasValue ? gerceklesenUretimTaliGuncelle.Prim8.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari8 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari8 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim9.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim9.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi9 = gerceklesenUretimTaliGuncelle.PoliceAdedi9.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi9.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi9;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi9 = gerceklesenUretimPolice.PoliceAdedi9.HasValue ? (gerceklesenUretimPolice.PoliceAdedi9.Value - 1) : gerceklesenUretimPolice.PoliceAdedi9;

                                if (gerceklesenUretimTaliGuncelle.Prim9.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi9 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi9 = gerceklesenUretimTaliGuncelle.PoliceAdedi9.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi9.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi9;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi9 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari9 = gerceklesenUretimPolice.PoliceKomisyonTutari9.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari9.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari9;
                            gerceklesenUretimPolice.Prim9 = gerceklesenUretimPolice.Prim9.HasValue ? (gerceklesenUretimPolice.Prim9.Value - police.NetPrim) : gerceklesenUretimPolice.Prim9;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim9 = gerceklesenUretimTaliGuncelle.Prim9.HasValue ? gerceklesenUretimTaliGuncelle.Prim9.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari9 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari9 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim10.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim10.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi10 = gerceklesenUretimTaliGuncelle.PoliceAdedi10.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi10.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi10;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi10 = gerceklesenUretimPolice.PoliceAdedi10.HasValue ? (gerceklesenUretimPolice.PoliceAdedi10.Value - 1) : gerceklesenUretimPolice.PoliceAdedi10;

                                if (gerceklesenUretimTaliGuncelle.Prim10.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi10 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi10 = gerceklesenUretimTaliGuncelle.PoliceAdedi10.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi10.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi10;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi10 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari10 = gerceklesenUretimPolice.PoliceKomisyonTutari10.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari10.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari10;
                            gerceklesenUretimPolice.Prim10 = gerceklesenUretimPolice.Prim10.HasValue ? (gerceklesenUretimPolice.Prim10.Value - police.NetPrim) : gerceklesenUretimPolice.Prim10;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim10 = gerceklesenUretimTaliGuncelle.Prim10.HasValue ? gerceklesenUretimTaliGuncelle.Prim10.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari10 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari10 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim11.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim11.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi11 = gerceklesenUretimTaliGuncelle.PoliceAdedi11.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi11.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi11;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi11 = gerceklesenUretimPolice.PoliceAdedi11.HasValue ? (gerceklesenUretimPolice.PoliceAdedi11.Value - 1) : gerceklesenUretimPolice.PoliceAdedi11;

                                if (gerceklesenUretimTaliGuncelle.Prim11.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi11 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi11 = gerceklesenUretimTaliGuncelle.PoliceAdedi11.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi11.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi11;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi11 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari11 = gerceklesenUretimPolice.PoliceKomisyonTutari11.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari11.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari11;
                            gerceklesenUretimPolice.Prim11 = gerceklesenUretimPolice.Prim11.HasValue ? (gerceklesenUretimPolice.Prim11.Value - police.NetPrim) : gerceklesenUretimPolice.Prim11;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim11 = gerceklesenUretimTaliGuncelle.Prim11.HasValue ? gerceklesenUretimTaliGuncelle.Prim11.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari11 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari11 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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

                                if (gerceklesenUretimTaliGuncelle.Prim12.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 0;
                                    }
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

                                    if (gerceklesenUretimTaliGuncelle.Prim12.HasValue)
                                    {
                                        uretimYeniMiEkleniyor = false;
                                        if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 == null)
                                        {
                                            gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 0;
                                        }
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi12 = gerceklesenUretimTaliGuncelle.PoliceAdedi12.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi12.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi12;
                                    }
                                    else
                                    {
                                        uretimYeniMiEkleniyor = true;
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 1;
                                    }
                                }
                            }
                            else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                            {
                                gerceklesenUretimPolice.PoliceAdedi12 = gerceklesenUretimPolice.PoliceAdedi12.HasValue ? (gerceklesenUretimPolice.PoliceAdedi12.Value - 1) : gerceklesenUretimPolice.PoliceAdedi12;

                                if (gerceklesenUretimTaliGuncelle.Prim12.HasValue)
                                {
                                    uretimYeniMiEkleniyor = false;
                                    if (gerceklesenUretimTaliGuncelle.PoliceAdedi12 == null)
                                    {
                                        gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 0;
                                    }
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi12 = gerceklesenUretimTaliGuncelle.PoliceAdedi12.HasValue ? (gerceklesenUretimTaliGuncelle.PoliceAdedi12.Value + 1) : gerceklesenUretimTaliGuncelle.PoliceAdedi12;
                                }
                                else
                                {
                                    uretimYeniMiEkleniyor = true;
                                    gerceklesenUretimTaliGuncelle.PoliceAdedi12 = 1;
                                }
                            }
                            gerceklesenUretimPolice.PoliceKomisyonTutari12 = gerceklesenUretimPolice.PoliceKomisyonTutari12.HasValue ? (gerceklesenUretimPolice.PoliceKomisyonTutari12.Value - police.Komisyon) : gerceklesenUretimPolice.PoliceKomisyonTutari12;
                            gerceklesenUretimPolice.Prim12 = gerceklesenUretimPolice.Prim12.HasValue ? (gerceklesenUretimPolice.Prim12.Value - police.NetPrim) : gerceklesenUretimPolice.Prim12;

                            if (!uretimYeniMiEkleniyor)
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12 = gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12.HasValue ? gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12.Value + police.Komisyon : police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12 = gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12.HasValue ? gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12.Value + police.TaliKomisyon : police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
                                gerceklesenUretimTaliGuncelle.Prim12 = gerceklesenUretimTaliGuncelle.Prim12.HasValue ? gerceklesenUretimTaliGuncelle.Prim12.Value + police.NetPrim : police.NetPrim;
                            }
                            else
                            {
                                gerceklesenUretimTaliGuncelle.PoliceKomisyonTutari12 = police.Komisyon;
                                gerceklesenUretimTaliGuncelle.VerilenKomisyonTutari12 = police.TaliKomisyon;
                                gerceklesenUretimTaliGuncelle.TVMKoduTali = taliKodu;
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
                }
            }
            //catch (Exception)
            //{

            //    guncellendiMi = false;
            //    throw;
            //}

            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                }
                throw;
            }
            return guncellendiMi;
        }

        public bool PoliceUretimHedefGerceklesenSil(PoliceGenel police)
        {
            bool uretimHedefGuncellendiMi = false;
            PoliceUretimHedefGerceklesen TVMUretim = new PoliceUretimHedefGerceklesen();
            try
            {
                if (!police.TaliAcenteKodu.HasValue)
                {
                    TVMUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == police.TVMKodu &&
                                                                                               s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();
                }
                else
                {
                    TVMUretim = _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Filter(s => s.TVMKodu == police.TVMKodu && s.TVMKoduTali == police.TaliAcenteKodu &&
                                                                                               s.Donem == police.TanzimTarihi.Value.Year && s.BransKodu == police.BransKodu).FirstOrDefault();

                }
                if (TVMUretim != null)
                {
                    #region OCAK
                    if (police.TanzimTarihi.Value.Month == 1)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi1 != null)
                            {
                                TVMUretim.PoliceAdedi1 = TVMUretim.PoliceAdedi1.HasValue ? (TVMUretim.PoliceAdedi1.Value - 1) : TVMUretim.PoliceAdedi1;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi1 != null)
                            {
                                TVMUretim.PoliceAdedi1 = TVMUretim.PoliceAdedi1.HasValue ? (TVMUretim.PoliceAdedi1.Value - 1) : TVMUretim.PoliceAdedi1;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {

                                if (TVMUretim.PoliceAdedi1 != null)
                                {
                                    TVMUretim.PoliceAdedi1 = TVMUretim.PoliceAdedi1.HasValue ? (TVMUretim.PoliceAdedi1.Value - 1) : TVMUretim.PoliceAdedi1;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari1 = TVMUretim.PoliceKomisyonTutari1.HasValue ? (TVMUretim.PoliceKomisyonTutari1.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari1 = TVMUretim.VerilenKomisyonTutari1.HasValue ? (TVMUretim.VerilenKomisyonTutari1.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim1 = TVMUretim.Prim1.HasValue ? (TVMUretim.Prim1.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region ŞUBAT
                    if (police.TanzimTarihi.Value.Month == 2)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi2 != null)
                            {
                                TVMUretim.PoliceAdedi2 = TVMUretim.PoliceAdedi2.HasValue ? (TVMUretim.PoliceAdedi2.Value - 1) : TVMUretim.PoliceAdedi2;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi2 != null)
                            {
                                TVMUretim.PoliceAdedi2 = TVMUretim.PoliceAdedi2.HasValue ? (TVMUretim.PoliceAdedi2.Value - 1) : TVMUretim.PoliceAdedi2;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {

                                if (TVMUretim.PoliceAdedi2 != null)
                                {
                                    TVMUretim.PoliceAdedi2 = TVMUretim.PoliceAdedi2.HasValue ? (TVMUretim.PoliceAdedi2.Value - 1) : TVMUretim.PoliceAdedi2;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari2 = TVMUretim.PoliceKomisyonTutari2.HasValue ? (TVMUretim.PoliceKomisyonTutari2.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari2 = TVMUretim.VerilenKomisyonTutari2.HasValue ? (TVMUretim.VerilenKomisyonTutari2.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim2 = TVMUretim.Prim2.HasValue ? (TVMUretim.Prim2.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region MART
                    if (police.TanzimTarihi.Value.Month == 3)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi3 != null)
                            {
                                TVMUretim.PoliceAdedi3 = TVMUretim.PoliceAdedi3.HasValue ? (TVMUretim.PoliceAdedi3.Value - 1) : TVMUretim.PoliceAdedi3;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi3 != null)
                            {
                                TVMUretim.PoliceAdedi3 = TVMUretim.PoliceAdedi3.HasValue ? (TVMUretim.PoliceAdedi3.Value - 1) : TVMUretim.PoliceAdedi3;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {

                                if (TVMUretim.PoliceAdedi3 != null)
                                {
                                    TVMUretim.PoliceAdedi3 = TVMUretim.PoliceAdedi3.HasValue ? (TVMUretim.PoliceAdedi3.Value - 1) : TVMUretim.PoliceAdedi3;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari3 = TVMUretim.PoliceKomisyonTutari3.HasValue ? (TVMUretim.PoliceKomisyonTutari3.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari3 = TVMUretim.VerilenKomisyonTutari3.HasValue ? (TVMUretim.VerilenKomisyonTutari3.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim3 = TVMUretim.Prim3.HasValue ? (TVMUretim.Prim3.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region NİSAN
                    if (police.TanzimTarihi.Value.Month == 4)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi4 != null)
                            {
                                TVMUretim.PoliceAdedi4 = TVMUretim.PoliceAdedi4.HasValue ? (TVMUretim.PoliceAdedi4.Value - 1) : TVMUretim.PoliceAdedi4;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi4 != null)
                            {
                                TVMUretim.PoliceAdedi4 = TVMUretim.PoliceAdedi4.HasValue ? (TVMUretim.PoliceAdedi4.Value - 1) : TVMUretim.PoliceAdedi4;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi4 != null)
                                {
                                    TVMUretim.PoliceAdedi4 = TVMUretim.PoliceAdedi4.HasValue ? (TVMUretim.PoliceAdedi4.Value - 1) : TVMUretim.PoliceAdedi4;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari4 = TVMUretim.PoliceKomisyonTutari4.HasValue ? (TVMUretim.PoliceKomisyonTutari4.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari4 = TVMUretim.VerilenKomisyonTutari4.HasValue ? (TVMUretim.VerilenKomisyonTutari4.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim4 = TVMUretim.Prim4.HasValue ? (TVMUretim.Prim4.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region MAYIS
                    if (police.TanzimTarihi.Value.Month == 5)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi5 != null)
                            {
                                TVMUretim.PoliceAdedi5 = TVMUretim.PoliceAdedi5.HasValue ? (TVMUretim.PoliceAdedi5.Value - 1) : TVMUretim.PoliceAdedi5;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi5 != null)
                            {
                                TVMUretim.PoliceAdedi5 = TVMUretim.PoliceAdedi5.HasValue ? (TVMUretim.PoliceAdedi5.Value - 1) : TVMUretim.PoliceAdedi5;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi5 != null)
                                {
                                    TVMUretim.PoliceAdedi5 = TVMUretim.PoliceAdedi5.HasValue ? (TVMUretim.PoliceAdedi5.Value - 1) : TVMUretim.PoliceAdedi5;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari5 = TVMUretim.PoliceKomisyonTutari5.HasValue ? (TVMUretim.PoliceKomisyonTutari5.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari5 = TVMUretim.VerilenKomisyonTutari5.HasValue ? (TVMUretim.VerilenKomisyonTutari5.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim5 = TVMUretim.Prim5.HasValue ? (TVMUretim.Prim5.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region HAZİRAN
                    if (police.TanzimTarihi.Value.Month == 6)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi6 != null)
                            {
                                TVMUretim.PoliceAdedi6 = TVMUretim.PoliceAdedi6.HasValue ? (TVMUretim.PoliceAdedi6.Value - 1) : TVMUretim.PoliceAdedi6;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi6 != null)
                            {
                                TVMUretim.PoliceAdedi6 = TVMUretim.PoliceAdedi6.HasValue ? (TVMUretim.PoliceAdedi6.Value - 1) : TVMUretim.PoliceAdedi6;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi6 != null)
                                {
                                    TVMUretim.PoliceAdedi6 = TVMUretim.PoliceAdedi6.HasValue ? (TVMUretim.PoliceAdedi6.Value - 1) : TVMUretim.PoliceAdedi6;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari6 = TVMUretim.PoliceKomisyonTutari6.HasValue ? (TVMUretim.PoliceKomisyonTutari6.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari6 = TVMUretim.VerilenKomisyonTutari6.HasValue ? (TVMUretim.VerilenKomisyonTutari6.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim6 = TVMUretim.Prim6.HasValue ? (TVMUretim.Prim6.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region TEMMUZ
                    if (police.TanzimTarihi.Value.Month == 7)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi7 != null)
                            {
                                TVMUretim.PoliceAdedi7 = TVMUretim.PoliceAdedi7.HasValue ? (TVMUretim.PoliceAdedi7.Value - 1) : TVMUretim.PoliceAdedi7;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi7 != null)
                            {
                                TVMUretim.PoliceAdedi7 = TVMUretim.PoliceAdedi7.HasValue ? (TVMUretim.PoliceAdedi7.Value - 1) : TVMUretim.PoliceAdedi7;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi7 != null)
                                {
                                    TVMUretim.PoliceAdedi7 = TVMUretim.PoliceAdedi7.HasValue ? (TVMUretim.PoliceAdedi7.Value - 1) : TVMUretim.PoliceAdedi7;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari7 = TVMUretim.PoliceKomisyonTutari7.HasValue ? (TVMUretim.PoliceKomisyonTutari7.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari7 = TVMUretim.VerilenKomisyonTutari7.HasValue ? (TVMUretim.VerilenKomisyonTutari7.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim7 = TVMUretim.Prim7.HasValue ? (TVMUretim.Prim7.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region AĞUSTOS
                    if (police.TanzimTarihi.Value.Month == 8)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi8 != null)
                            {
                                TVMUretim.PoliceAdedi8 = TVMUretim.PoliceAdedi8.HasValue ? (TVMUretim.PoliceAdedi8.Value - 1) : TVMUretim.PoliceAdedi8;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi8 != null)
                            {
                                TVMUretim.PoliceAdedi8 = TVMUretim.PoliceAdedi8.HasValue ? (TVMUretim.PoliceAdedi8.Value - 1) : TVMUretim.PoliceAdedi8;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi8 != null)
                                {
                                    TVMUretim.PoliceAdedi8 = TVMUretim.PoliceAdedi8.HasValue ? (TVMUretim.PoliceAdedi8.Value - 1) : TVMUretim.PoliceAdedi8;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari8 = TVMUretim.PoliceKomisyonTutari8.HasValue ? (TVMUretim.PoliceKomisyonTutari8.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari8 = TVMUretim.VerilenKomisyonTutari8.HasValue ? (TVMUretim.VerilenKomisyonTutari8.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim8 = TVMUretim.Prim8.HasValue ? (TVMUretim.Prim8.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region EYLÜL
                    if (police.TanzimTarihi.Value.Month == 9)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi9 != null)
                            {
                                TVMUretim.PoliceAdedi9 = TVMUretim.PoliceAdedi9.HasValue ? (TVMUretim.PoliceAdedi9.Value - 1) : TVMUretim.PoliceAdedi9;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi9 != null)
                            {
                                TVMUretim.PoliceAdedi9 = TVMUretim.PoliceAdedi9.HasValue ? (TVMUretim.PoliceAdedi9.Value - 1) : TVMUretim.PoliceAdedi9;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi9 != null)
                                {
                                    TVMUretim.PoliceAdedi9 = TVMUretim.PoliceAdedi9.HasValue ? (TVMUretim.PoliceAdedi9.Value - 1) : TVMUretim.PoliceAdedi9;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari9 = TVMUretim.PoliceKomisyonTutari9.HasValue ? (TVMUretim.PoliceKomisyonTutari9.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari9 = TVMUretim.VerilenKomisyonTutari9.HasValue ? (TVMUretim.VerilenKomisyonTutari9.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim9 = TVMUretim.Prim9.HasValue ? (TVMUretim.Prim9.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region EKİM
                    if (police.TanzimTarihi.Value.Month == 10)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi10 != null)
                            {
                                TVMUretim.PoliceAdedi10 = TVMUretim.PoliceAdedi10.HasValue ? (TVMUretim.PoliceAdedi10.Value - 1) : TVMUretim.PoliceAdedi10;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi10 != null)
                            {
                                TVMUretim.PoliceAdedi10 = TVMUretim.PoliceAdedi10.HasValue ? (TVMUretim.PoliceAdedi10.Value - 1) : TVMUretim.PoliceAdedi10;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi10 != null)
                                {
                                    TVMUretim.PoliceAdedi10 = TVMUretim.PoliceAdedi10.HasValue ? (TVMUretim.PoliceAdedi10.Value - 1) : TVMUretim.PoliceAdedi10;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari10 = TVMUretim.PoliceKomisyonTutari10.HasValue ? (TVMUretim.PoliceKomisyonTutari10.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari10 = TVMUretim.VerilenKomisyonTutari10.HasValue ? (TVMUretim.VerilenKomisyonTutari10.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim10 = TVMUretim.Prim10.HasValue ? (TVMUretim.Prim10.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region KASIM
                    if (police.TanzimTarihi.Value.Month == 11)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi11 != null)
                            {
                                TVMUretim.PoliceAdedi11 = TVMUretim.PoliceAdedi11.HasValue ? (TVMUretim.PoliceAdedi11.Value - 1) : TVMUretim.PoliceAdedi11;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi11 != null)
                            {
                                TVMUretim.PoliceAdedi11 = TVMUretim.PoliceAdedi11.HasValue ? (TVMUretim.PoliceAdedi11.Value - 1) : TVMUretim.PoliceAdedi11;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi11 != null)
                                {
                                    TVMUretim.PoliceAdedi11 = TVMUretim.PoliceAdedi11.HasValue ? (TVMUretim.PoliceAdedi11.Value - 1) : TVMUretim.PoliceAdedi11;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari11 = TVMUretim.PoliceKomisyonTutari11.HasValue ? (TVMUretim.PoliceKomisyonTutari11.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari11 = TVMUretim.VerilenKomisyonTutari11.HasValue ? (TVMUretim.VerilenKomisyonTutari11.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim11 = TVMUretim.Prim11.HasValue ? (TVMUretim.Prim11.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    #region ARALIK
                    if (police.TanzimTarihi.Value.Month == 12)
                    {
                        if (police.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 1)
                        {
                            if (TVMUretim.PoliceAdedi12 != null)
                            {
                                TVMUretim.PoliceAdedi12 = TVMUretim.PoliceAdedi12.HasValue ? (TVMUretim.PoliceAdedi12.Value - 1) : TVMUretim.PoliceAdedi12;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.EkNo == 0)
                        {
                            if (TVMUretim.PoliceAdedi12 != null)
                            {
                                TVMUretim.PoliceAdedi12 = TVMUretim.PoliceAdedi12.HasValue ? (TVMUretim.PoliceAdedi12.Value - 1) : TVMUretim.PoliceAdedi12;
                            }
                        }
                        else if (police.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && police.BransKodu == BransListeCeviri.Dask && police.EkNo.ToString().Length > 4)
                        {
                            if (police.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                if (TVMUretim.PoliceAdedi12 != null)
                                {
                                    TVMUretim.PoliceAdedi12 = TVMUretim.PoliceAdedi12.HasValue ? (TVMUretim.PoliceAdedi12.Value - 1) : TVMUretim.PoliceAdedi12;
                                }
                            }
                        }
                        TVMUretim.PoliceKomisyonTutari12 = TVMUretim.PoliceKomisyonTutari12.HasValue ? (TVMUretim.PoliceKomisyonTutari12.Value - police.Komisyon) : police.Komisyon;
                        TVMUretim.VerilenKomisyonTutari12 = TVMUretim.VerilenKomisyonTutari12.HasValue ? (TVMUretim.VerilenKomisyonTutari12.Value - police.TaliKomisyon) : police.TaliKomisyon;
                        TVMUretim.Prim12 = TVMUretim.Prim12.HasValue ? (TVMUretim.Prim12.Value - police.NetPrim) : police.NetPrim;
                    }
                    #endregion

                    _KomisyonContext.PoliceUretimHedefGerceklesenRepository.Update(TVMUretim);
                    _KomisyonContext.Commit();
                    uretimHedefGuncellendiMi = true;
                }
                else
                {
                    uretimHedefGuncellendiMi = false;
                }
            }
            catch (Exception)
            {
                uretimHedefGuncellendiMi = false;
            }
            return uretimHedefGuncellendiMi;
        }

        #region müsteri kayıt için metodlar

        public MusteriGenelBilgiler getMusteri(string kimlikNo, int tvmKodu)
        {
            MusteriGenelBilgiler mus = new MusteriGenelBilgiler();
            mus = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(s => s.KimlikNo == kimlikNo
                                                                               && s.TVMKodu == tvmKodu
                                                                                                          ).FirstOrDefault();

            return mus;
        }

        public MusteriGenelBilgiler getMusteriler(string kimlikNo, int tvmKodu)
        {
            var musList = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(s => s.KimlikNo == kimlikNo
                                                                                 && s.TVMKodu == tvmKodu
                                                                                                            ).ToList();
            if (musList != null)
            {
                for (int i = 0; i < musList.Count; i++)
                {
                    if (i == 0)
                    {
                        return musList[i];
                    }
                }
            }


            return null;

        }

        public TVMKullanicilar GetMusteriTVMKullanicilarByTVMKodu(int tvmKodu)
        {
            var musTvm = _TVMContext.TVMKullanicilarRepository.Filter(s => s.TVMKodu == tvmKodu).FirstOrDefault();

            return musTvm;
        }

        public string getIl(string ilAdi)
        {

            Il ilAdiConvertKodu = new Il();
            ilAdi = ilAdi.Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G');
            ilAdiConvertKodu = _ParametreContext.IlRepository.All().Where(s => s.IlAdi == ilAdi).FirstOrDefault();
            if (ilAdiConvertKodu != null)
            {
                return ilAdiConvertKodu.IlKodu;
            }
            else
            {
                return "";
            }

        }

        public MusteriAdre getMusteriAdres(string adres, string ilKodu, string mahalle, string cadde, int ilceKodu, string apartman, string sokak, string binaNo, string daireNo, int musteriKodu)
        {
            MusteriAdre musGenelAdres = new MusteriAdre();
            musGenelAdres = _MusteriContext.MusteriAdreRepository.All().Where(s => s.Adres == adres
                                                                          && s.IlKodu == ilKodu
                                                                          && s.Mahalle == mahalle
                                                                          && s.Cadde == cadde
                                                                          && s.IlceKodu == ilceKodu
                                                                          && s.Apartman == apartman
                                                                              && s.Sokak == sokak
                                                                              && s.BinaNo == binaNo
                                                                              && s.DaireNo == daireNo
                                                                          && s.MusteriKodu == musteriKodu
                                                                                         ).FirstOrDefault();

            return musGenelAdres;

        }

        public MusteriTelefon getMusteriTelefon(string numara, int iletsimNumaraTipi, int musteriKodu)
        {
            MusteriTelefon musTelefon = new MusteriTelefon();
            musTelefon = _MusteriContext.MusteriTelefonRepository.All().Where(s => s.Numara == numara
                                                                          && s.IletisimNumaraTipi == iletsimNumaraTipi
                                                                          && s.MusteriKodu == musteriKodu).FirstOrDefault();
            return musTelefon;

        }

        public int musAdresSiraNo(int musteriKodu)
        {
            var adres = _MusteriContext.MusteriAdreRepository.All().Where(s => s.MusteriKodu == musteriKodu).OrderByDescending(s => s.SiraNo).FirstOrDefault();
            if (adres != null)
            {
                return adres.SiraNo + 1;
            }
            else
            {
                return 0;
            }
        }

        public int musTelefonSiraNo(int musteriKodu)
        {
            var telefon = _MusteriContext.MusteriTelefonRepository.All().Where(s => s.MusteriKodu == musteriKodu).OrderByDescending(s => s.SiraNo).FirstOrDefault();
            if (telefon != null)
            {
                return telefon.SiraNo + 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        public List<OtoLoginSigortaSirketKullanicilar> getOtoLoginSigortaSirketKullanicilar(int tvmkodu)
        {
            List<OtoLoginSigortaSirketKullanicilar> getOtoLogin = new List<OtoLoginSigortaSirketKullanicilar>();
            getOtoLogin = _KomisyonContext.OtoLoginSigortaSirketKullanicilarRepository.All().Where(s => s.TVMKodu == tvmkodu

                                                                                         ).ToList();

            return getOtoLogin;

        }

        public bool PoliceKimlikGuncelle(int policeId, string kimlikNo)
        {
            bool guncellendiMi = false;
            try
            {
                var police = _PoliceContext.PoliceGenelRepository.All().Where(s => s.PoliceId == policeId).FirstOrDefault();
                if (police != null)
                {
                    if (kimlikNo.Length == 10)
                    {

                        police.PoliceSigortaEttiren.VergiKimlikNo = kimlikNo;
                        if (police.PoliceSigortali.AdiUnvan == police.PoliceSigortaEttiren.AdiUnvan)
                        {
                            police.PoliceSigortali.VergiKimlikNo = kimlikNo;
                            foreach (var item in police.PoliceTahsilats)
                            {
                                item.KimlikNo = kimlikNo;
                            }
                        }
                    }
                    else
                    {
                        police.PoliceSigortaEttiren.KimlikNo = kimlikNo;
                        if (police.PoliceSigortali.AdiUnvan == police.PoliceSigortaEttiren.AdiUnvan)
                        {
                            police.PoliceSigortali.KimlikNo = kimlikNo;
                            foreach (var item in police.PoliceTahsilats)
                            {
                                item.KimlikNo = kimlikNo;
                            }
                        }
                    }
                    _PoliceContext.PoliceGenelRepository.Update(police);
                    _PoliceContext.Commit();
                    guncellendiMi = true;
                }
            }
            catch (Exception)
            {
                guncellendiMi = false;
            }

            return guncellendiMi;
        }
        public bool policeOdemePlaniIsOdemeTipiGuncelle(int policeId, string odemeTipi)
        {
            bool guncellendiMi = false;
            try
            {
                var police = _PoliceContext.PoliceOdemePlaniRepository.All().Where(s => s.PoliceId == policeId).FirstOrDefault();

                if (police != null)
                {

                    List<PoliceOdemePlani> polOdemePlaniList = new List<PoliceOdemePlani>();
                    polOdemePlaniList = police.PoliceGenel.PoliceOdemePlanis.Where(s => s.PoliceId == policeId).ToList<PoliceOdemePlani>();
                    if (polOdemePlaniList != null)
                    {
                        foreach (var item in polOdemePlaniList)
                        {
                            item.OdemeTipi = Convert.ToByte(odemeTipi);
                            police.PoliceGenel.PoliceOdemePlanis.Add(item);
                            _PoliceContext.PoliceOdemePlaniRepository.Update(police);
                            _PoliceContext.Commit();
                        }

                        guncellendiMi = true;
                    }

                }
            }
            catch (Exception)
            {
                guncellendiMi = false;
            }
            return guncellendiMi;
        }

        public bool OdemeTipiGuncelle(int policeId, string odemeTipi)
        {
            bool guncellendiMi = false;
            try
            {
                var police = _PoliceContext.PoliceOdemePlaniRepository.All().Where(s => s.PoliceId == policeId).FirstOrDefault();
                var polTah = _PoliceContext.PoliceTahsilatRepository.All().Where(s => s.PoliceId == policeId).FirstOrDefault();

                if (police != null)
                {

                    List<PoliceOdemePlani> polOdemePlaniList = new List<PoliceOdemePlani>();
                    polOdemePlaniList = police.PoliceGenel.PoliceOdemePlanis.Where(s => s.PoliceId == policeId).ToList<PoliceOdemePlani>();
                    if (polOdemePlaniList != null)
                    {
                        foreach (var item in polOdemePlaniList)
                        {
                            item.OdemeTipi = Convert.ToByte(odemeTipi);
                            police.PoliceGenel.PoliceOdemePlanis.Add(item);
                            _PoliceContext.PoliceOdemePlaniRepository.Update(police);
                            _PoliceContext.Commit();
                        }

                        guncellendiMi = true;
                    }

                }
                if (polTah != null)
                {
                    List<PoliceTahsilat> polTahiList = new List<PoliceTahsilat>();
                    polTahiList = police.PoliceGenel.PoliceTahsilats.Where(s => s.PoliceId == policeId).ToList<PoliceTahsilat>();
                    if (polTahiList != null)
                    {
                        foreach (var item2 in polTahiList)
                        {
                            if (item2.OdemTipi == 2 && Convert.ToByte(odemeTipi) != 2)
                            {
                                item2.OtomatikTahsilatiKkMi = null;
                            }
                            item2.OdemTipi = Convert.ToByte(odemeTipi);
                            police.PoliceGenel.PoliceTahsilats.Add(item2);
                            _PoliceContext.PoliceOdemePlaniRepository.Update(police);
                            police.PoliceGenel.PoliceTahsilats.Add(item2);
                            _PoliceContext.PoliceTahsilatRepository.Update(polTah);
                            _PoliceContext.Commit();
                        }

                        guncellendiMi = true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
                guncellendiMi = false;
            }

            return guncellendiMi;
        }

        public List<Database.Models.IsTipleri> getListIsTipleri()
        {
            IGorevTakipContext _gorevTakipContext = DependencyResolver.Current.GetService<IGorevTakipContext>();
            List<Database.Models.IsTipleri> isTipListesi = _gorevTakipContext.IsTipleriRepository.All().Where(w => w.Durum == 1).ToList();
            return isTipListesi;
        }

        public PoliceArac getPoliceAracDetay(string plakaKodu, string plakaNo)
        {
            PoliceArac arac = new PoliceArac();
            arac = _PoliceContext.PoliceAracRepository.All().Where(w => w.PlakaKodu.Trim() == plakaKodu && w.PlakaNo.Trim() == plakaNo).OrderByDescending(o => o.PoliceId).FirstOrDefault();
            return arac;
        }
        public Tuple<string, string, string, string, string> GetTCKNBilgileriByTCKN(string kimlikNo)
        {
            var policeSigortali = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.KimlikNo == kimlikNo && w.TVMKodu == _AktifKullanici.TVMKodu).FirstOrDefault();
            return new Tuple<string, string, string, string, string>(policeSigortali.AdiUnvan, policeSigortali.SoyadiUnvan, policeSigortali.MusteriTelefons.FirstOrDefault().Numara, policeSigortali.MusteriAdres.FirstOrDefault().Adres, policeSigortali.KimlikNo);
        }
        public List<MusteriGenelBilgiler> GetTCKNBilgileriByAdSoyad(string ad, string soyad)
        {
            var policeSigortali = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.AdiUnvan == ad && w.SoyadiUnvan == soyad && w.TVMKodu == _AktifKullanici.TVMKodu).ToList();
            return policeSigortali;
        }
        public MusteriBilgiModel getMusteriDetay(string ad, string soyad)
        {
            MusteriBilgiModel model = new MusteriBilgiModel();
            var tvmListesi = _TVMService.GetTVMListe(_AktifKullanici.TVMKodu);
            tvmListesi.Add(_AktifKullanici.TVMKodu);
            var musteriBilgi = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => tvmListesi.Contains(w.TVMKodu) && w.AdiUnvan.Contains(ad) && w.SoyadiUnvan.Contains(soyad)).FirstOrDefault();
            if (musteriBilgi != null)
            {
                var adresDetay = musteriBilgi.MusteriAdres.Where(w => w.Varsayilan == true).FirstOrDefault();
                if (adresDetay != null)
                {
                    model.Adres = adresDetay.Adres;
                }
                model.kimlikNo = !String.IsNullOrEmpty(musteriBilgi.KimlikNo) ? musteriBilgi.KimlikNo : "";
            }
            else
            {
                model.HataMesaji = "Müşteri Kaydı bulunamadı";
            }
            return model;
        }

        public SigortaSirketleri getSigortaSirketi(string sirketKodu)
        {
            return _SigortaSirketleriService.GetSirket(sirketKodu.Trim());
        }
        public List<CariHesaplari> getSirketler()
        {
            var carihesaptipi = "320.01.";
            return _MuhasebeContext.CariHesaplariRepository.All().Where(w => w.CariHesapTipi.Trim() == carihesaptipi && w.TVMKodu == _AktifKullanici.TVMKodu).ToList();
        }
        public List<PoliceDokuman> getPoliceDokumanlar(int policeId)
        {
            return _PoliceContext.PoliceDokumanRepository.All().Where(w => w.Police_ID == policeId).ToList();
        }

        public bool createPoliceDokuman(PoliceDokuman model)
        {
            try
            {
                _PoliceContext.PoliceDokumanRepository.Create(model);
                _PoliceContext.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public PoliceYaslandirmaTablosuModel getBrokerPoliceRapor(PoliceYaslandirmaTablosuModel model)
        {
            model.policeYaslandirmaTablosuPolice = new List<PoliceYaslandirmaTablosuPolice>();
            PoliceYaslandirmaTablosuPolice policeYaslandirmaTablosuModel;
            PoliceYaslandirmaTablosuModelItem policeYaslandirmaTablosuModelItem;
            var policelist = _PoliceContext.PoliceGenelRepository.All().Where(x => x.TVMKodu == _AktifKullanici.TVMKodu && x.TanzimTarihi >= model.BaslangicTarihi && x.TanzimTarihi <= model.BitisTarihi).ToList();
            PoliceTahsilat policeTahsilat;
            List<PoliceTahsilat> policeTahsilats;
            List<PoliceOdemePlani> odemePlanis;
            List<Underwriters> underwriters;
            PoliceSigortaEttiren policeSigortaEttiren;
            PoliceSigortali policeSigortali;
            bool odendimi = false;
            decimal ToplamPrim = 0;
            decimal ToplamOdenen = 0;
            decimal ToplamKalan = 0;
            decimal ToplamYurtDisiBrokerKomisyon = 0;
            decimal ToplamAlinanKomisyon = 0;
            decimal ToplamKalanKomisyon = 0;
            List<string> ssliste = new List<string>();
            if (model.SigortaSirketleriSelectList != null)
            {
                foreach (var item in model.SigortaSirketleriSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        ssliste.Add(item);
                    }
                }

            }
            List<CariHareketleri> cariHareketleris = null; decimal taksitTutari = 0;
            List<PoliceYaslandirmaTablosuModelItem> tempUWCariHareketList = null;
            foreach (var itemPolice in policelist)
            {
                cariHareketleris = _Muhasebe_CariHesapService.YaslandirmaTablosuCariHesaplari(itemPolice.PoliceNumarasi + "-" + itemPolice.YenilemeNo + "-" + itemPolice.EkNo);
                if (ssliste.Count > 0)
                {//tumbirlik filtresi
                    if (!ssliste.Contains(itemPolice.TUMBirlikKodu))
                        continue;
                }
                policeSigortaEttiren = _PoliceContext.PoliceSigortaEttirenRepository.All().Where(x => x.PoliceId == itemPolice.PoliceId).FirstOrDefault();
                policeSigortali = _PoliceContext.PoliceSigortaliRepository.All().Where(x => x.PoliceId == itemPolice.PoliceId).FirstOrDefault();
                var tempsatiskanali = _SigortaSirketleriService.GetSigortaBilgileri(itemPolice.TUMBirlikKodu);
                policeYaslandirmaTablosuModel = new PoliceYaslandirmaTablosuPolice();
                policeYaslandirmaTablosuModel.PoliceNumarasi = itemPolice.PoliceNumarasi;
                policeYaslandirmaTablosuModel.ParaBirimi = itemPolice.ParaBirimi;
                policeYaslandirmaTablosuModel.SigortaSirketi = tempsatiskanali.SirketAdi;
                policeYaslandirmaTablosuModel.MuhasebeIslimi = itemPolice.MuhasebeyeAktarildiMi == 2 ? true : false;

                policeYaslandirmaTablosuModel.SatisKanali = _AktifKullanici.TVMUnvani;//sorulacak
                policeYaslandirmaTablosuModel.SigortaEttirenAdSoyad = policeSigortaEttiren.AdiUnvan + " " + policeSigortaEttiren.SoyadiUnvan;
                policeYaslandirmaTablosuModel.SigortaliAdSoyad = policeSigortali.AdiUnvan + " " + policeSigortali.SoyadiUnvan;
                ToplamPrim = 0;
                ToplamOdenen = 0;
                ToplamKalan = 0;
                ToplamYurtDisiBrokerKomisyon = 0;
                ToplamAlinanKomisyon = 0;
                ToplamKalanKomisyon = 0;
                policeTahsilats = _PoliceContext.PoliceTahsilatRepository.All().Where(x => x.PoliceId == itemPolice.PoliceId).ToList();
                odemePlanis = _PoliceContext.PoliceOdemePlaniRepository.All().Where(x => x.PoliceId == itemPolice.PoliceId).ToList();
                underwriters = _TeklifContext.UnderwritersRepository.All().Where(x => x.PoliceId.Value == itemPolice.PoliceId).ToList();
                //model.policeYaslandirmaTablosuPolice.Add(policeYaslandirmaTablosuModel);
                foreach (var itemUw in underwriters)
                {
                    var tempuwkod = "[" + itemUw.UnderwriterKodu + "]";
                    var cariHareketVarmi = cariHareketleris.Where(x => x.EvrakNo.Contains(tempuwkod)).ToList();
                    tempUWCariHareketList = new List<PoliceYaslandirmaTablosuModelItem>();
                    foreach (var item in cariHareketVarmi)
                    {
                        var temp2 = item.Aciklama.Split('-').Where(x => x.Contains("Taksit")).FirstOrDefault();
                        if (temp2 != null)
                        {
                            temp2 = temp2.Replace("Taksit", "").Replace(".", "");
                            if (int.TryParse(temp2, out int tempTaksitNo))
                                tempUWCariHareketList.Add(new PoliceYaslandirmaTablosuModelItem { TaksitNo = tempTaksitNo });
                        }
                    }
                    foreach (var itemOdeme in odemePlanis)
                    {

                        policeYaslandirmaTablosuModelItem = new PoliceYaslandirmaTablosuModelItem();

                        policeTahsilat = policeTahsilats.Where(x => x.PoliceId == itemOdeme.PoliceId && x.TaksitNo == itemOdeme.TaksitNo).FirstOrDefault();
                        odendimi = policeTahsilat.KalanTaksitTutari == 0 ? true : false;
                        if (odendimi)
                        {
                            policeYaslandirmaTablosuModelItem.Odendimi = true;
                            if (model.RaporTipi == "1")
                                continue;
                        }
                        else
                        {
                            if (model.RaporTipi == "2")
                                continue;
                        }
                        policeYaslandirmaTablosuModelItem.ReasurorAd = itemUw.UnderwriterAdi;
                        policeYaslandirmaTablosuModelItem.TaksitNo = itemOdeme.TaksitNo;
                        policeYaslandirmaTablosuModelItem.TaksitTarihi = itemOdeme.VadeTarihi.Value;
                        ////uw prim odenen kalan baslangic
                        var taksit = Utils.TaksitDuzenle(itemUw.UnderwriterPrim.Value, odemePlanis.Count);
                        policeYaslandirmaTablosuModelItem.Odenen = 0;
                        policeYaslandirmaTablosuModelItem.Kalan = 0;
                        if (itemOdeme.TaksitNo == odemePlanis.Count)//son taksit kontrol
                            taksitTutari = taksit[1];
                        else
                            taksitTutari = taksit[0];
                        policeYaslandirmaTablosuModelItem.Prim = taksitTutari;
                        if (odendimi)
                            policeYaslandirmaTablosuModelItem.Odenen = taksitTutari;
                        else
                            policeYaslandirmaTablosuModelItem.Kalan = taksitTutari;
                        #region eski hali
                        //if (itemOdeme.TaksitNo == odemePlanis.Count)//son taksit kontrol
                        //{
                        //    policeYaslandirmaTablosuModelItem.Prim = taksit[1];
                        //    if (odendimi)
                        //        policeYaslandirmaTablosuModelItem.Odenen = taksit[1];
                        //    else
                        //        policeYaslandirmaTablosuModelItem.Kalan = taksit[1];

                        //}
                        //else
                        //{
                        //    policeYaslandirmaTablosuModelItem.Prim = taksit[0];
                        //    if (odendimi)
                        //        policeYaslandirmaTablosuModelItem.Odenen = taksit[0];
                        //    else
                        //        policeYaslandirmaTablosuModelItem.Kalan = taksit[0];
                        //}
                        #endregion
                        ////uw prim odenen kalan son
                        ////uw komisyon odenen kalan baslangic
                        taksit = Utils.TaksitDuzenle(itemUw.UnderwriterKomisyon.Value, odemePlanis.Count);
                        policeYaslandirmaTablosuModelItem.AlinanKomisyon = 0;
                        
                        if (itemOdeme.TaksitNo == odemePlanis.Count)//son taksit kontrol
                            taksitTutari = taksit[1];
                        else
                            taksitTutari = taksit[0];
                        policeYaslandirmaTablosuModelItem.YurtDisiBrokerKomisyon = taksitTutari;
                        policeYaslandirmaTablosuModelItem.KalanKomisyon = taksitTutari;
                        if (tempUWCariHareketList.Count > 0)
                        {
                            var taksitnovarmi = tempUWCariHareketList.Where(x => x.TaksitNo == itemOdeme.TaksitNo).FirstOrDefault();
                            if (taksitnovarmi != null)
                            {
                                policeYaslandirmaTablosuModelItem.AlinanKomisyon = taksitTutari;
                                policeYaslandirmaTablosuModelItem.KalanKomisyon = 0;

                            }
                        }
                        //else
                        //{
                        //    //if (odendimi)
                        //    //    policeYaslandirmaTablosuModelItem.AlinanKomisyon = taksitTutari;
                        //    //else
                        //    //    policeYaslandirmaTablosuModelItem.KalanKomisyon = taksitTutari;
                        //    policeYaslandirmaTablosuModelItem.KalanKomisyon = taksitTutari;
                        //}
                        #region eski hali
                        //if (itemOdeme.TaksitNo == odemePlanis.Count)//son taksit kontrol
                        //{
                        //    policeYaslandirmaTablosuModelItem.YurtDisiBrokerKomisyon = taksit[1];
                        //    if (odendimi)
                        //        policeYaslandirmaTablosuModelItem.AlinanKomisyon = taksit[1];
                        //    else
                        //        policeYaslandirmaTablosuModelItem.KalanKomisyon = taksit[1];

                        //}
                        //else
                        //{
                        //    policeYaslandirmaTablosuModelItem.YurtDisiBrokerKomisyon = taksit[0];
                        //    if (odendimi)
                        //        policeYaslandirmaTablosuModelItem.AlinanKomisyon = taksit[0];
                        //    else
                        //        policeYaslandirmaTablosuModelItem.KalanKomisyon = taksit[0];
                        //}
                        #endregion
                        ////uw komisyon odenen kalan son

                        ToplamPrim += policeYaslandirmaTablosuModelItem.Prim;
                        ToplamOdenen += policeYaslandirmaTablosuModelItem.Odenen;
                        ToplamKalan += policeYaslandirmaTablosuModelItem.Kalan;
                        ToplamYurtDisiBrokerKomisyon += policeYaslandirmaTablosuModelItem.YurtDisiBrokerKomisyon;
                        ToplamAlinanKomisyon += policeYaslandirmaTablosuModelItem.AlinanKomisyon;
                        ToplamKalanKomisyon += policeYaslandirmaTablosuModelItem.KalanKomisyon;

                        policeYaslandirmaTablosuModel.PoliceYaslandirmaTablosuList.Add(policeYaslandirmaTablosuModelItem);
                    }
                }
                policeYaslandirmaTablosuModelItem = new PoliceYaslandirmaTablosuModelItem();
                policeYaslandirmaTablosuModelItem.ToplamPrim = ToplamPrim;
                policeYaslandirmaTablosuModelItem.ToplamOdenen = ToplamOdenen;
                policeYaslandirmaTablosuModelItem.ToplamKalan = ToplamKalan;
                policeYaslandirmaTablosuModelItem.ToplamYurtDisiBrokerKomisyon = ToplamYurtDisiBrokerKomisyon;
                policeYaslandirmaTablosuModelItem.ToplamAlinanKomisyon = ToplamAlinanKomisyon;
                policeYaslandirmaTablosuModelItem.ToplamKalanKomisyon = ToplamKalanKomisyon;
                policeYaslandirmaTablosuModel.PoliceYaslandirmaTablosuList.Add(policeYaslandirmaTablosuModelItem);

                model.policeYaslandirmaTablosuPolice.Add(policeYaslandirmaTablosuModel);
            }

            return model;
        }
        public TeklifPoliceListesiUWDetay getBrokerPoliceTeklifListesiUWDetayli(TeklifPoliceListesiUWDetay model)
        {
            ReasurorPoliceListesiProcedureModel reasurorPoliceListesiProcedureModel;

            var policelist = model.procedurePoliceOfflineList;
            model.procedurePoliceOfflineList = new List<ReasurorPoliceListesiProcedureModel>();
            //model.TeklifPoliceUWToplamItem = new List<TeklifPoliceParaBirimiToplamItem>();
            //var policeidlist = policelist.Select(x => x.PoliceId).ToList();
            PoliceTahsilat policeTahsilat;
            List<PoliceTahsilat> policeTahsilats;
            List<PoliceOdemePlani> odemePlanis;
            List<Underwriters> underwriters = null;
            PoliceSigortaEttiren policeSigortaEttiren;
            PoliceSigortali policeSigortali;
            ReasurorGenel reasurorGenel;
            bool odendimi = false;
            decimal ToplamPrim = 0, ToplamPrimTL = 0;
            decimal ToplamOdenen = 0, ToplamOdenenTL = 0;
            decimal ToplamKalan = 0, ToplamKalanTL = 0;
            decimal ToplamYurtDisiBrokerKomisyon = 0, ToplamYurtDisiBrokerKomisyonTL = 0;
            decimal ToplamAlinanKomisyon = 0, ToplamAlinanKomisyonTL = 0;
            decimal ToplamDisKomisyon = 0, ToplamDisKomisyonTL = 0;
            int index;
            foreach (var itemPolice in policelist)
            {
                reasurorPoliceListesiProcedureModel = new ReasurorPoliceListesiProcedureModel();
                reasurorPoliceListesiProcedureModel = itemPolice;
                ToplamPrim = 0; ToplamPrimTL = 0;
                ToplamAlinanKomisyon = 0; ToplamAlinanKomisyonTL = 0;
                ToplamDisKomisyon = 0; ToplamDisKomisyonTL = 0;
                //underwriters = _TeklifContext.UnderwritersRepository.All().Where(x => x.PoliceId.Value == itemPolice.PoliceId).ToList();
                var tempuw = model.Underwriters.Where(x => x.PoliceId == itemPolice.PoliceId).FirstOrDefault();
                if (tempuw != null)
                {
                    underwriters = tempuw.Underwriters;
                }
                else
                {
                    continue;
                }
                //if (underwriters == null || underwriters.Count == 0)
                //{
                //    var temp = model.TeklifPoliceUWToplamItem.Where(x => x.ParaBirimi == itemPolice.ParaBirimi).FirstOrDefault();
                //    if (temp != null)
                //        model.TeklifPoliceUWToplamItem.Remove(temp);
                //    continue;
                //} 
                index = 0;
                foreach (var itemUw in underwriters)
                {
                    reasurorPoliceListesiProcedureModel = new ReasurorPoliceListesiProcedureModel();
                    if (index == 0)
                    {
                        reasurorPoliceListesiProcedureModel.SEUnvan = itemPolice.SEUnvan;
                        reasurorPoliceListesiProcedureModel.SliUnvan = itemPolice.SliUnvan;
                        reasurorPoliceListesiProcedureModel.PoliceNumarasi = itemPolice.PoliceNumarasi;
                        reasurorPoliceListesiProcedureModel.BransAdi = itemPolice.BransAdi;
                        reasurorPoliceListesiProcedureModel.TcknVkn = itemPolice.TcknVkn;
                        reasurorPoliceListesiProcedureModel.EkNo = itemPolice.EkNo;
                        reasurorPoliceListesiProcedureModel.YenilemeNo = itemPolice.YenilemeNo;
                    }
                    reasurorPoliceListesiProcedureModel.Aciklama = itemPolice.Aciklama;
                    reasurorPoliceListesiProcedureModel.BaslangicTarihi = itemPolice.BaslangicTarihi;
                    reasurorPoliceListesiProcedureModel.BitisTarihi = itemPolice.BitisTarihi;
                    reasurorPoliceListesiProcedureModel.BransKodu = itemPolice.BransKodu;
                    reasurorPoliceListesiProcedureModel.Bsmv = itemPolice.Bsmv;
                    reasurorPoliceListesiProcedureModel.BsmvTL = itemPolice.BsmvTL;
                    reasurorPoliceListesiProcedureModel.DisUretimTvmKodu = itemPolice.DisUretimTvmKodu;
                    reasurorPoliceListesiProcedureModel.DisUretimTvmUnvani = itemPolice.DisUretimTvmUnvani;
                    reasurorPoliceListesiProcedureModel.DovizKur = itemPolice.DovizKur;
                    reasurorPoliceListesiProcedureModel.FrontingSigortaSirketiKomisyon = itemPolice.FrontingSigortaSirketiKomisyon;
                    reasurorPoliceListesiProcedureModel.FrontingSigortaSirketiKomisyonTL = itemPolice.FrontingSigortaSirketiKomisyonTL;
                    reasurorPoliceListesiProcedureModel.OdemeSekli = itemPolice.OdemeSekli;
                    reasurorPoliceListesiProcedureModel.OdemeTipi = itemPolice.OdemeTipi;
                    reasurorPoliceListesiProcedureModel.ParaBirimi = itemPolice.ParaBirimi;
                    reasurorPoliceListesiProcedureModel.PdfPoliceCreditNote = itemPolice.PdfPoliceCreditNote;
                    reasurorPoliceListesiProcedureModel.PdfPoliceDebitNote = itemPolice.PdfPoliceDebitNote;
                    reasurorPoliceListesiProcedureModel.PdfPoliceDosyasi = itemPolice.PdfPoliceDosyasi;
                    reasurorPoliceListesiProcedureModel.PoliceId = itemPolice.PoliceId;
                    reasurorPoliceListesiProcedureModel.PsliIlAdi = itemPolice.PsliIlAdi;
                    reasurorPoliceListesiProcedureModel.PsliIlceAdi = itemPolice.PsliIlceAdi;
                    reasurorPoliceListesiProcedureModel.SatisKanaliKomisyon = itemPolice.SatisKanaliKomisyon;
                    reasurorPoliceListesiProcedureModel.SatisKanaliKomisyonTL = itemPolice.SatisKanaliKomisyonTL;

                    reasurorPoliceListesiProcedureModel.TaksitSayisi = itemPolice.TaksitSayisi;
                    reasurorPoliceListesiProcedureModel.TaliKodu = itemPolice.TaliKodu;
                    reasurorPoliceListesiProcedureModel.TaliUnvani = itemPolice.TaliUnvani;
                    reasurorPoliceListesiProcedureModel.TanzimTarihi = itemPolice.TanzimTarihi;

                    reasurorPoliceListesiProcedureModel.TeminatTutari = itemPolice.TeminatTutari;
                    reasurorPoliceListesiProcedureModel.TeminatTutariTL = itemPolice.TeminatTutariTL;
                    reasurorPoliceListesiProcedureModel.TUMBirlikKodu = itemPolice.TUMBirlikKodu;
                    reasurorPoliceListesiProcedureModel.TUMUrunAdi = itemPolice.TUMUrunAdi;
                    reasurorPoliceListesiProcedureModel.TUMUrunKodu = itemPolice.TUMUrunKodu;
                    reasurorPoliceListesiProcedureModel.TvmDetayKodu = itemPolice.TvmDetayKodu;
                    reasurorPoliceListesiProcedureModel.TvmDetayUnvani = itemPolice.TvmDetayUnvani;
                    reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyon = itemPolice.YurtdisiAlinanKomisyon;
                    reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyonTL = itemPolice.YurtdisiAlinanKomisyonTL;
                    reasurorPoliceListesiProcedureModel.YurtdisiBrokerNetPrim = itemPolice.YurtdisiBrokerNetPrim;
                    reasurorPoliceListesiProcedureModel.YurtdisiBrokerNetPrimTL = itemPolice.YurtdisiBrokerNetPrimTL;
                    reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyon = itemPolice.YurtdisiDisKaynakKomisyon;
                    reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyonTL = itemPolice.YurtdisiDisKaynakKomisyonTL;
                    reasurorPoliceListesiProcedureModel.YurtdisiNetPrim = itemPolice.YurtdisiNetPrim;
                    reasurorPoliceListesiProcedureModel.YurtdisiNetPrimTL = itemPolice.YurtdisiNetPrimTL;
                    reasurorPoliceListesiProcedureModel.YurtdisiPrim = itemPolice.YurtdisiPrim;
                    reasurorPoliceListesiProcedureModel.YurtdisiPrimTL = itemPolice.YurtdisiPrimTL;
                    reasurorPoliceListesiProcedureModel.YurticiAlinanKomisyon = itemPolice.YurticiAlinanKomisyon;
                    reasurorPoliceListesiProcedureModel.YurticiAlinanKomisyonTL = itemPolice.YurticiAlinanKomisyonTL;
                    reasurorPoliceListesiProcedureModel.YurticiBrutPrim = itemPolice.YurticiBrutPrim;
                    reasurorPoliceListesiProcedureModel.YurticiBrutPrimTL = itemPolice.YurticiBrutPrimTL;
                    reasurorPoliceListesiProcedureModel.YurticiNetPrim = itemPolice.YurticiNetPrim;
                    reasurorPoliceListesiProcedureModel.YurticiNetPrimTL = itemPolice.YurticiNetPrimTL;
                    reasurorPoliceListesiProcedureModel.SirketAdi = itemUw.UnderwriterAdi;
                    //reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyon = reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyon * itemUw.UnderwriterKomisyonOrani / 100;
                    //reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyonTL = reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyonTL * itemUw.UnderwriterKomisyonOrani / 100;
                    //reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyon = reasurorPoliceListesiProcedureModel.YurtdisiNetPrim * itemUw.UnderwriterKomisyonOrani / 100;
                    //reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyonTL = reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyonTL * itemUw.UnderwriterKomisyonOrani / 100;

                    reasurorPoliceListesiProcedureModel.YurtdisiNetPrim = itemPolice.YurtdisiNetPrim * itemUw.UnderwriterPayOrani / 100;
                    reasurorPoliceListesiProcedureModel.YurtdisiNetPrimTL = itemPolice.YurtdisiNetPrimTL * itemUw.UnderwriterPayOrani / 100;
                    reasurorPoliceListesiProcedureModel.UWKomisyon = itemUw.UnderwriterKomisyon;
                    reasurorPoliceListesiProcedureModel.UWPrim = itemUw.UnderwriterPrim;

                    ToplamPrim += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiNetPrim);
                    ToplamPrimTL += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiNetPrimTL);
                    ToplamAlinanKomisyon += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyon);
                    ToplamAlinanKomisyonTL += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyonTL);
                    ToplamDisKomisyon += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyon);
                    ToplamDisKomisyonTL += Convert.ToDecimal(reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyonTL);

                    model.procedurePoliceOfflineList.Add(reasurorPoliceListesiProcedureModel);
                    index++;
                }

                reasurorPoliceListesiProcedureModel = new ReasurorPoliceListesiProcedureModel();
                reasurorPoliceListesiProcedureModel.PoliceNumarasi = itemPolice.PoliceNumarasi;
                reasurorPoliceListesiProcedureModel.YenilemeNo = itemPolice.YenilemeNo;
                reasurorPoliceListesiProcedureModel.EkNo = itemPolice.EkNo;
                reasurorPoliceListesiProcedureModel.ParaBirimi = itemPolice.ParaBirimi;
                reasurorPoliceListesiProcedureModel.Aciklama = "ToplamSatır";
                reasurorPoliceListesiProcedureModel.SirketAdi = "P.Toplam";

                reasurorPoliceListesiProcedureModel.YurtdisiNetPrim = ToplamPrim;
                reasurorPoliceListesiProcedureModel.YurtdisiNetPrimTL = ToplamPrimTL;
                reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyon = ToplamAlinanKomisyon;
                reasurorPoliceListesiProcedureModel.YurtdisiAlinanKomisyonTL = ToplamAlinanKomisyonTL;
                reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyon = ToplamDisKomisyon;
                reasurorPoliceListesiProcedureModel.YurtdisiDisKaynakKomisyonTL = ToplamDisKomisyonTL;

                model.procedurePoliceOfflineList.Add(reasurorPoliceListesiProcedureModel);
                if (model.TeklifPoliceUWToplamItem.Where(x => x.ParaBirimi == itemPolice.ParaBirimi).FirstOrDefault() == null)
                {
                    TeklifPoliceParaBirimiToplamItem teklifPoliceParaBirimiToplamItem = new TeklifPoliceParaBirimiToplamItem();
                    teklifPoliceParaBirimiToplamItem.ParaBirimi = itemPolice.ParaBirimi;
                    if (itemPolice.ParaBirimi.Trim() != "TL")
                    {
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiAlinanKomisyon = ToplamAlinanKomisyon;
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiDisKaynakKomisyon = ToplamDisKomisyon;
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiNetPrim = ToplamPrim;
                    }
                    else
                    {
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiAlinanKomisyon = ToplamAlinanKomisyonTL;
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiDisKaynakKomisyon = ToplamDisKomisyonTL;
                        teklifPoliceParaBirimiToplamItem.ToplamYurtdisiNetPrim = ToplamPrimTL;
                    }
                    model.TeklifPoliceUWToplamItem.Add(teklifPoliceParaBirimiToplamItem);
                }
                else
                {
                    var tempteklifPoliceParaBirimiToplamItem = model.TeklifPoliceUWToplamItem.Where(x => x.ParaBirimi == itemPolice.ParaBirimi).FirstOrDefault();
                    if (itemPolice.ParaBirimi.Trim() != "TL")
                    {
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiAlinanKomisyon += ToplamAlinanKomisyon;
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiDisKaynakKomisyon += ToplamDisKomisyon;
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiNetPrim += ToplamPrim;
                    }
                    else
                    {
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiAlinanKomisyon += ToplamAlinanKomisyonTL;
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiDisKaynakKomisyon += ToplamDisKomisyonTL;
                        tempteklifPoliceParaBirimiToplamItem.ToplamYurtdisiNetPrim += ToplamPrimTL;
                    }
                    //model.TeklifPoliceUWToplamItem.Add(teklifPoliceParaBirimiToplamItem);
                }

                //model.policeYaslandirmaTablosuPolice.Add(policeYaslandirmaTablosuModel);
            }

            return model;
        }


    }
}
