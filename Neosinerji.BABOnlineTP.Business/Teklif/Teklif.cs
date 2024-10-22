using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;

namespace Neosinerji.BABOnlineTP.Business
{
    public class Teklif : ITeklif
    {
        public Teklif()
        {
        }

        public Teklif(TeklifGenel teklifGenel)
        {
            this.GenelBilgiler = teklifGenel;
            this.TeklifNo = this.GenelBilgiler.TeklifNo;
        }

        public static Teklif Create(int urunKodu, int tvmKodu, int kullaniciKodu, int sigortaEttirenKodu, int kaydiEkleyenTVMKodu, int kaydiEkleyenTVMKullaniciKodu)
        {
            Teklif teklif = new Teklif();

            #region Genel Bilgiler
            TeklifGenel genelBilgiler = new TeklifGenel();
            genelBilgiler.TVMKodu = tvmKodu;
            genelBilgiler.TeklifNo = 0;
            genelBilgiler.TeklifRevizyonNo = 0;
            genelBilgiler.TUMKodu = 0;
            genelBilgiler.TVMKullaniciKodu = kullaniciKodu;
            genelBilgiler.UrunKodu = urunKodu;
            genelBilgiler.TeklifDurumKodu = TeklifDurumlari.Teklif;
            genelBilgiler.OdemePlaniAlternatifKodu = OdemePlaniAlternatifKodlari.Yok;
            genelBilgiler.TanzimTarihi = TurkeyDateTime.Today;
            genelBilgiler.BaslamaTarihi = TurkeyDateTime.Today;
            genelBilgiler.BitisTarihi = TurkeyDateTime.Today.AddYears(1);
            genelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
            genelBilgiler.BrutPrim = 0;
            genelBilgiler.ToplamVergi = 0;
            genelBilgiler.OdemeSekli = OdemeSekilleri.Yok;
            genelBilgiler.OdemeTipi = OdemeTipleri.Yok;
            genelBilgiler.TaksitSayisi = 0;
            genelBilgiler.DovizTL = DovizTLTipleri.Yok;
            genelBilgiler.DovizKodu = String.Empty;
            genelBilgiler.DovizKurBedeli = 0;
            genelBilgiler.GecikmeZammiYuzdesi = 0;
            genelBilgiler.HasarsizlikIndirimYuzdesi = 0;
            genelBilgiler.PlakaIndirimYuzdesi = 0;
            genelBilgiler.HasarSurprimYuzdesi = 0;
            genelBilgiler.ToplamIndirimTutari = 0;
            genelBilgiler.ToplamSurprimTutari = 0;
            genelBilgiler.ToplamKomisyon = 0;

            //Aktif kullanıcı başkası adına teklif/poliçe girişi yapıyor ise kullanıcı bilgileri bu alanda tutulacak
            genelBilgiler.KaydiEKleyenTVMKodu = kaydiEkleyenTVMKodu;
            genelBilgiler.KaydiEKleyenTVMKullaniciKodu = kaydiEkleyenTVMKullaniciKodu;

            teklif.GenelBilgiler = genelBilgiler;

            #endregion

            #region Sigorta Ettiren
            TeklifSigortaEttiren sigortaEttiren = new TeklifSigortaEttiren();
            sigortaEttiren.TeklifId = genelBilgiler.TeklifId;
            sigortaEttiren.SiraNo = 1;
            sigortaEttiren.MusteriKodu = sigortaEttirenKodu;

            teklif.SigortaEttiren = sigortaEttiren;
            #endregion

            return teklif;
        }

        public virtual void Hesapla(ITeklif teklif)
        {
        }

        public virtual void Policelestir(Odeme odeme)
        {
        }

        public virtual void PolicePDF()
        {
        }

        public virtual Hashtable BilgilendirmeFormu(string formName)
        {
            throw new Exception("Ürüne ait bilgilendirme formu bulunmuyor.");
        }

        //public void SendMuhasebe()
        //{
        //    if (this.GenelBilgiler.TeklifDurumKodu == TeklifDurumlari.Police)
        //    {
        //        TVMDetay tvm = this.GenelBilgiler.TVMDetay;

        //        //Muhasebe kullanıyorsa poliçe bilgisi muhasebeye gönderiliyor.
        //        if (tvm != null && tvm.MuhasebeEntegrasyon.HasValue && tvm.MuhasebeEntegrasyon.Value)
        //        {
        //            IPoliceToXML _PoliceToXML = DependencyResolver.Current.GetService<IPoliceToXML>();
        //            _PoliceToXML.SendPoliceToMuhasebe(this);
        //        }
        //    }
        //}

        public void Import(ITeklif teklif)
        {
            this.GenelBilgiler = new TeklifGenel();
            this.GenelBilgiler.TVMKodu = teklif.GenelBilgiler.TVMKodu;
            this.GenelBilgiler.KaydiEKleyenTVMKodu = teklif.GenelBilgiler.KaydiEKleyenTVMKodu;
            this.GenelBilgiler.KaydiEKleyenTVMKullaniciKodu = teklif.GenelBilgiler.KaydiEKleyenTVMKullaniciKodu;
            this.GenelBilgiler.TeklifNo = teklif.GenelBilgiler.TeklifNo;
            this.GenelBilgiler.TeklifRevizyonNo = teklif.GenelBilgiler.TeklifRevizyonNo;
            this.GenelBilgiler.TUMKodu = this.TUMKodu;
            this.GenelBilgiler.TVMKullaniciKodu = teklif.GenelBilgiler.TVMKullaniciKodu;
            this.GenelBilgiler.UrunKodu = teklif.GenelBilgiler.UrunKodu;
            this.GenelBilgiler.OdemePlaniAlternatifKodu = this.OdemePlaniAlternatifKodu;
            this.GenelBilgiler.TeklifDurumKodu = teklif.GenelBilgiler.TeklifDurumKodu;
            this.GenelBilgiler.TanzimTarihi = teklif.GenelBilgiler.TanzimTarihi;
            this.GenelBilgiler.BaslamaTarihi = teklif.GenelBilgiler.BaslamaTarihi;
            this.GenelBilgiler.BitisTarihi = teklif.GenelBilgiler.BitisTarihi;
            this.GenelBilgiler.GecerlilikBitisTarihi = teklif.GenelBilgiler.GecerlilikBitisTarihi;

            this.SigortaEttiren.MusteriKodu = teklif.SigortaEttiren.MusteriKodu;
            this.SigortaEttiren.SiraNo = teklif.SigortaEttiren.SiraNo;

            foreach (var item in teklif.Sigortalilar)
            {
                TeklifSigortali sigortali = new TeklifSigortali();
                sigortali.MusteriKodu = item.MusteriKodu;
                sigortali.SiraNo = item.SiraNo;

                this.Sigortalilar.Add(sigortali);
            }

            this.Teminatlar = new List<TeklifTeminat>();
            this.Vergiler = new List<TeklifVergi>();
            this.OdemePlani = new List<TeklifOdemePlani>();
        }

        public virtual void DekontPDF()
        { }

        public void AddSigortali(int sigortaliKodu)
        {
            TeklifSigortali sigortali = new TeklifSigortali();
            sigortali.TeklifId = this.GenelBilgiler.TeklifId;
            sigortali.SiraNo = this.Sigortalilar.Count() + 1;
            sigortali.MusteriKodu = sigortaliKodu;

            this.Sigortalilar.Add(sigortali);
        }

        public void AddSoru(int soruKodu, string cevap)
        {
            this.AddSoru(soruKodu, SoruCevapTipleri.Metin, cevap);
        }

        public void AddSoru(int soruKodu, bool cevap)
        {
            this.AddSoru(soruKodu, SoruCevapTipleri.EvetHayir, cevap ? "E" : "H");
        }

        public void AddSoru(int soruKodu, decimal cevap)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("tr-TR");
            string val = cevap.ToString("N2", culture);
            this.AddSoru(soruKodu, SoruCevapTipleri.Tutar, val);
        }

        public void AddSoru(int soruKodu, DateTime cevap)
        {
            this.AddSoru(soruKodu, SoruCevapTipleri.Tarih, cevap.ToString("yyyy-MM-dd"));
        }

        private void AddSoru(int soruKodu, byte cevapTipi, string cevap)
        {
            if (String.IsNullOrEmpty(cevap))
                return;

            TeklifSoru soru = new TeklifSoru();
            soru.TeklifId = this.GenelBilgiler.TeklifId;
            soru.SoruKodu = soruKodu;
            soru.CevapTipi = cevapTipi;
            soru.Cevap = cevap;

            this.Sorular.Add(soru);
        }

        public string ReadSoru(int soruKodu, string defaultValue)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                return soru.Cevap;
            }

            return defaultValue;
        }

        public string ReadSoruNullableString(int soruKodu)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                return soru.Cevap;
            }
            else return null;
        }


        public bool ReadSoru(int soruKodu, bool defaultValue)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                return soru.Cevap == "E" ? true : false;
            }

            return defaultValue;
        }


        public bool? ReadSoruNullableBool(int soruKodu)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                return soru.Cevap == "E" ? true : false;
            }
            else return null;
        }

        public decimal ReadSoru(int soruKodu, decimal defaultValue)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                if (!String.IsNullOrEmpty(soru.Cevap))
                {
                    decimal result = Decimal.Zero;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("tr-TR");
                    if (decimal.TryParse(soru.Cevap, System.Globalization.NumberStyles.Number, culture, out result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        public int? ReadSoruNullableInt(int soruKodu)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                if (!String.IsNullOrEmpty(soru.Cevap))
                {
                    int result;
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("tr-TR");
                    if (int.TryParse(soru.Cevap, System.Globalization.NumberStyles.Number, culture, out result))
                    {
                        return result;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }

        public DateTime ReadSoru(int soruKodu, DateTime defaultValue)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                if (!String.IsNullOrEmpty(soru.Cevap) && soru.Cevap.Length == 10)
                {
                    string[] parts = soru.Cevap.Split('-');
                    if (parts.Length == 3)
                    {
                        int year = Convert.ToInt32(parts[0]);
                        int month = Convert.ToInt32(parts[1]);
                        int day = Convert.ToInt32(parts[2]);

                        return new DateTime(year, month, day);
                    }
                }
            }

            return defaultValue;
        }

        public DateTime? ReadSoruNullableDateTime(int soruKodu)
        {
            TeklifSoru soru = this.Sorular.FirstOrDefault(f => f.SoruKodu == soruKodu);

            if (soru != null)
            {
                if (!String.IsNullOrEmpty(soru.Cevap) && soru.Cevap.Length == 10)
                {
                    string[] parts = soru.Cevap.Split('-');
                    if (parts.Length == 3)
                    {
                        int year = Convert.ToInt32(parts[0]);
                        int month = Convert.ToInt32(parts[1]);
                        int day = Convert.ToInt32(parts[2]);

                        return new DateTime(year, month, day);
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }


        public void AddWebServisCevap(int cevapKodu, string cevap)
        {
            this.AddWebServisCevap(cevapKodu, SoruCevapTipleri.Metin, cevap);
        }

        public void AddWebServisCevap(int cevapKodu, bool cevap)
        {
            this.AddWebServisCevap(cevapKodu, SoruCevapTipleri.EvetHayir, cevap ? "E" : "H");
        }

        public void AddWebServisCevap(int cevapKodu, int cevap)
        {
            string val = cevap.ToString();
            this.AddWebServisCevap(cevapKodu, SoruCevapTipleri.Metin, val);
        }

        public void AddWebServisCevap(int cevapKodu, decimal cevap)
        {
            string val = cevap.ToString("N2");
            this.AddWebServisCevap(cevapKodu, SoruCevapTipleri.Tutar, val);
        }

        public void AddWebServisCevap(int cevapKodu, DateTime cevap)
        {
            this.AddWebServisCevap(cevapKodu, SoruCevapTipleri.Tarih, cevap.ToString("yyyy-MM-dd"));
        }

        private void AddWebServisCevap(int cevapKodu, byte cevapTipi, string cevap)
        {
            if (String.IsNullOrEmpty(cevap))
                return;

            TeklifWebServisCevap c = new TeklifWebServisCevap();
            c.TeklifId = this.GenelBilgiler.TeklifId;
            c.CevapKodu = cevapKodu;
            c.CevapTipi = cevapTipi;
            c.Cevap = cevap;

            this.WebServisCevaplar.Add(c);
        }

        public string ReadWebServisCevap(int cevapKodu, string defaultValue)
        {
            TeklifWebServisCevap cevap = this.WebServisCevaplar.FirstOrDefault(f => f.CevapKodu == cevapKodu);

            if (cevap != null)
            {
                return cevap.Cevap;
            }

            return defaultValue;
        }

        public int ReadWebServisCevap(int cevapKodu, int defaultValue)
        {
            TeklifWebServisCevap cevap = this.WebServisCevaplar.FirstOrDefault(f => f.CevapKodu == cevapKodu);

            if (cevap != null)
            {
                return Convert.ToInt32(cevap.Cevap);
            }

            return defaultValue;
        }

        public bool ReadWebServisCevap(int cevapKodu, bool defaultValue)
        {
            TeklifWebServisCevap cevap = this.WebServisCevaplar.FirstOrDefault(f => f.CevapKodu == cevapKodu);

            if (cevap != null)
            {
                return cevap.Cevap == "E" ? true : false;
            }

            return defaultValue;
        }

        public decimal ReadWebServisCevap(int cevapKodu, decimal defaultValue)
        {
            TeklifWebServisCevap cevap = this.WebServisCevaplar.FirstOrDefault(f => f.CevapKodu == cevapKodu);

            if (cevap != null)
            {
                if (!String.IsNullOrEmpty(cevap.Cevap))
                {
                    decimal result = Decimal.Zero;
                    if (decimal.TryParse(cevap.Cevap, out result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        public DateTime ReadWebServisCevap(int cevapKodu, DateTime defaultValue)
        {
            TeklifWebServisCevap cevap = this.WebServisCevaplar.FirstOrDefault(f => f.CevapKodu == cevapKodu);

            if (cevap != null)
            {
                if (!String.IsNullOrEmpty(cevap.Cevap) && cevap.Cevap.Length == 10)
                {
                    string[] parts = cevap.Cevap.Split('-');
                    if (parts.Length == 3)
                    {
                        int year = Convert.ToInt32(parts[0]);
                        int month = Convert.ToInt32(parts[1]);
                        int day = Convert.ToInt32(parts[2]);

                        return new DateTime(year, month, day);
                    }

                }
            }

            return defaultValue;
        }

        public void AddAracEkSoru(int tumKodu, string soruTipi, string soruKodu, string aciklama, decimal bedel, decimal fiyat)
        {
            TeklifAracEkSoru soru = new TeklifAracEkSoru();
            soru.TeklifId = this.GenelBilgiler.TeklifId;
            soru.TUMKodu = tumKodu;
            soru.SoruTipi = soruTipi;
            soru.SoruKodu = soruKodu;
            soru.Aciklama = aciklama;
            soru.Bedel = bedel;
            soru.Fiyat = fiyat;

            this.AracEkSorular.Add(soru);
        }

        public void AddVergi(int vergiKodu, decimal tutar)
        {
            TeklifVergi vergi = new TeklifVergi();
            vergi.TeklifId = this.GenelBilgiler.TeklifId;
            vergi.VergiKodu = vergiKodu;
            vergi.VergiTutari = tutar;

            this.Vergiler.Add(vergi);
        }

        public void AddTeminat(int teminatKodu, decimal tutar, decimal vergi, decimal netprim, decimal brutprim, int adet)
        {
            TeklifTeminat teminat = new TeklifTeminat();
            teminat.TeklifId = this.GenelBilgiler.TeklifId;
            teminat.TeminatKodu = teminatKodu;
            teminat.TeminatBedeli = tutar;
            teminat.TeminatVergi = vergi;
            teminat.TeminatNetPrim = netprim;
            teminat.TeminatBrutPrim = brutprim;
            teminat.Adet = adet;

            this.Teminatlar.Add(teminat);
        }

        public void AddOdemePlani(int taksitNo, DateTime vade, decimal tutar, byte odemeTipi)
        {
            TeklifOdemePlani odeme = new TeklifOdemePlani();
            odeme.TaksitNo = taksitNo;
            odeme.VadeTarihi = vade;
            odeme.TaksitTutari = tutar;
            odeme.OdemeTipi = odemeTipi;

            this.OdemePlani.Add(odeme);
        }

        public void AddOdemePlaniALL(ITeklif teklif)
        {
            if (this.GenelBilgiler.BrutPrim.HasValue && this.GenelBilgiler.TaksitSayisi.HasValue)
            {
                decimal taksit = this.GenelBilgiler.BrutPrim.Value / Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi);
                decimal taksitFraction = taksit - decimal.Floor(taksit);
                decimal taksit1 = decimal.Floor(taksit) + (taksitFraction * Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi));
                decimal taksit2 = decimal.Floor(taksit);

                DateTime taksitTarihi = this.GenelBilgiler.BaslamaTarihi;
                for (int i = 0; i < Convert.ToInt32(this.GenelBilgiler.TaksitSayisi); i++)
                {
                    if (i == 0)
                        this.AddOdemePlani(i + 1, taksitTarihi, taksit1, teklif.GenelBilgiler.OdemeTipi ?? 0);
                    else
                        this.AddOdemePlani(i + 1, taksitTarihi, taksit2, teklif.GenelBilgiler.OdemeTipi ?? 0);

                    taksitTarihi = taksitTarihi.AddMonths(1);
                }
            }
        }

        public void ResetOdemePlani()
        {
            ITeklifContext _TeklifContext = DependencyResolver.Current.GetService<ITeklifContext>();

            _TeklifContext.TeklifOdemePlaniRepository.Delete(s => s.TeklifId == this.GenelBilgiler.TeklifId);
        }

        public void AddHata(string hataMesaji)
        {
            this.Hatalar.Add(hataMesaji);
        }

        public void AddBilgiMesaji(string bilgiMesajlari)
        {
            this.BilgiMesajlari.Add(bilgiMesajlari);
        }

        public void BeginLog(string istek, byte istekTipi)
        {
            this._AktifLog = new WEBServisLog();
            _AktifLog.IstekTarihi = TurkeyDateTime.Now;
            _AktifLog.IstekTipi = istekTipi;

            this._AktifIstek = istek;
        }

        public void BeginLog(object request, Type type, byte istekTipi)
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

                this.BeginLog(istek, istekTipi);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
        }

        public void EndLog(string cevap, bool basarili)
        {
            if (this._AktifLog == null)
                return;

            IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();

            this._AktifLog.CevapTarihi = TurkeyDateTime.Now;
            this._AktifLog.BasariliBasarisiz = basarili ? WebServisBasariTipleri.Basarili : WebServisBasariTipleri.Basarisiz;

            string istekURL = storage.UploadXml("teklif", this._AktifIstek);
            string cevapURL = storage.UploadXml("teklif", cevap);

            this._AktifLog.IstekUrl = istekURL;
            this._AktifLog.CevapUrl = cevapURL;

            this.Log.Add(this._AktifLog);

            this._AktifLog = null;
        }

        public void EndLog(object response, bool basarili, Type type)
        {
            try
            {
                string cevap = String.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer s = new XmlSerializer(type);
                        s.Serialize(xmlWriter, response);
                    }

                    cevap = Encoding.UTF8.GetString(ms.ToArray());
                }

                this.EndLog(cevap, basarili);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
            }
        }

        public bool SaveArac()
        {
            return this._Arac != null;
        }

        public bool SaveAracEkSoru()
        {
            return this._AracEkSorular != null;
        }

        public bool SaveRizikoAdresi()
        {
            return this._RizikoAdresi != null;
        }

        public bool SaveSorular()
        {
            return this._Sorular != null && this._Sorular.Count > 0;
        }

        public bool SaveWebServisCevaplar()
        {
            return this._WebServisCevaplar != null && this._WebServisCevaplar.Count > 0;
        }

        public bool SaveTeminatlar()
        {
            return this._Teminatlar != null && this._Teminatlar.Count > 0;
        }

        public bool SaveVergiler()
        {
            return this._Vergiler != null && this._Vergiler.Count > 0;
        }

        public bool SaveOdemePlani()
        {
            return this._OdemePlani != null && this._OdemePlani.Count > 0;
        }

        public bool SaveLog()
        {
            return this._Log != null && this._Log.Count > 0;
        }

        public int UrunKodu
        {
            get
            {
                return this.GenelBilgiler.UrunKodu;
            }
        }

        public int TeklifNo
        {
            get
            {
                return this.GenelBilgiler.TeklifNo;
            }
            set
            {
                this.GenelBilgiler.TeklifNo = value;
            }
        }

        public virtual int TUMKodu
        {
            get
            {
                if (this.GenelBilgiler != null)
                    return this.GenelBilgiler.TUMKodu;

                return 0;
            }
        }

        private int _OdemePlaniAlternatifKodu;
        public int OdemePlaniAlternatifKodu
        {
            get
            {
                if (_OdemePlaniAlternatifKodu == 0 && this.GenelBilgiler != null)
                    return this.GenelBilgiler.OdemePlaniAlternatifKodu;

                return _OdemePlaniAlternatifKodu;
            }
            set
            {
                if (this.GenelBilgiler != null)
                    this.GenelBilgiler.OdemePlaniAlternatifKodu = value;

                _OdemePlaniAlternatifKodu = value;
            }
        }

        public bool Basarili
        {
            get
            {
                if (this._Hatalar == null)
                    return true;

                return this.Hatalar.Count == 0;
            }
        }

        private TeklifGenel _GenelBilgiler;
        public TeklifGenel GenelBilgiler
        {
            get
            {
                if (_GenelBilgiler == null)
                    _GenelBilgiler = new TeklifGenel();

                return _GenelBilgiler;
            }
            set
            {
                _GenelBilgiler = value;
            }
        }

        private TeklifSigortaEttiren _SigortaEttiren;
        public TeklifSigortaEttiren SigortaEttiren
        {
            get
            {
                if (_SigortaEttiren == null)
                    _SigortaEttiren = new TeklifSigortaEttiren();

                return _SigortaEttiren;
            }
            set
            {
                _SigortaEttiren = value;
            }
        }

        private List<TeklifSigortali> _Sigortalilar;
        public List<TeklifSigortali> Sigortalilar
        {
            get
            {
                if (_Sigortalilar == null)
                    _Sigortalilar = new List<TeklifSigortali>();

                return _Sigortalilar;
            }
            set
            {
                _Sigortalilar = value;
            }
        }

        private TeklifArac _Arac;
        public TeklifArac Arac
        {
            get
            {
                if (_Arac == null)
                {
                    _Arac = new TeklifArac();
                    _Arac.TeklifId = this.GenelBilgiler.TeklifId;
                    _Arac.SiraNo = 1;
                }

                return _Arac;
            }
            set
            {
                _Arac = value;
            }
        }

        private TeklifRizikoAdresi _RizikoAdresi;
        public TeklifRizikoAdresi RizikoAdresi
        {
            get
            {
                if (_RizikoAdresi == null)
                    _RizikoAdresi = new TeklifRizikoAdresi();

                return _RizikoAdresi;
            }
            set
            {
                _RizikoAdresi = value;
            }
        }

        private List<TeklifAracEkSoru> _AracEkSorular;
        public List<TeklifAracEkSoru> AracEkSorular
        {
            get
            {
                if (_AracEkSorular == null)
                    _AracEkSorular = new List<TeklifAracEkSoru>();

                return _AracEkSorular;
            }
            set
            {
                _AracEkSorular = value;
            }
        }

        private List<TeklifSoru> _Sorular;
        public List<TeklifSoru> Sorular
        {
            get
            {
                if (_Sorular == null)
                    _Sorular = new List<TeklifSoru>();

                return _Sorular;
            }
            set
            {
                _Sorular = value;
            }
        }

        private List<TeklifWebServisCevap> _WebServisCevaplar;
        public List<TeklifWebServisCevap> WebServisCevaplar
        {
            get
            {
                if (_WebServisCevaplar == null)
                    _WebServisCevaplar = new List<TeklifWebServisCevap>();

                return _WebServisCevaplar;
            }
            set
            {
                _WebServisCevaplar = value;
            }
        }

        private List<TeklifTeminat> _Teminatlar;
        public List<TeklifTeminat> Teminatlar
        {
            get
            {
                if (_Teminatlar == null)
                    _Teminatlar = new List<TeklifTeminat>();

                return _Teminatlar;
            }
            set
            {
                _Teminatlar = value;
            }
        }

        private List<TeklifVergi> _Vergiler;
        public List<TeklifVergi> Vergiler
        {
            get
            {
                if (_Vergiler == null)
                    _Vergiler = new List<TeklifVergi>();

                return _Vergiler;
            }
            set
            {
                _Vergiler = value;
            }
        }

        private List<TeklifOdemePlani> _OdemePlani;
        public List<TeklifOdemePlani> OdemePlani
        {
            get
            {
                if (_OdemePlani == null)
                    _OdemePlani = new List<TeklifOdemePlani>();

                return _OdemePlani;
            }
            set
            {
                _OdemePlani = value;
            }
        }

        private WEBServisLog _AktifLog;
        private string _AktifIstek;

        private List<WEBServisLog> _Log;
        public List<WEBServisLog> Log
        {
            get
            {
                if (_Log == null)
                    _Log = new List<WEBServisLog>();

                return _Log;
            }
        }

        private List<string> _Hatalar;
        public List<string> Hatalar
        {
            get
            {
                if (_Hatalar == null)
                    _Hatalar = new List<string>();

                return _Hatalar;
            }
        }

        private List<string> _BilgiMesajlari;
        public List<string> BilgiMesajlari
        {
            get
            {
                if (_BilgiMesajlari == null)
                    _BilgiMesajlari = new List<string>();

                return _Hatalar;
            }
        }

        #region IP

        public string ClientIPNo { get; set; }
        public void SetClientIPAdres(string ipadres)
        {
            ClientIPNo = ipadres;
        }

        //ANADOLUKasko.cs de Client ip yi okumak için kullanılıyor.
        public string SetClientIPAdress(string ipadres)
        {
            ClientIPNo = ipadres;
            return ClientIPNo;
        }

      
        #endregion


    }
   
}