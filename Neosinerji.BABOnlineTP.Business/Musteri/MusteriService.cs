using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Service;
using System.Globalization;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Paritus;
using RestSharp.Extensions;
using AutoMapper.Internal;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MusteriService : IMusteriService
    {
        IMusteriContext _MusteriContext;
        ITVMContext _TVMContext;
        IParametreContext _UlkeContext;
        ITanimService _TanimService;
        IAktifKullaniciService _Aktif;
        IKonfigurasyonService _KonfigurasyonService;
        IParametreContext _ParameterContext;

        public MusteriService(IMusteriContext musteriContext, ITVMContext tmvContext, IParametreContext unitOfWork, ITanimService tanim)
        {
            _TanimService = tanim;
            _MusteriContext = musteriContext;
            _TVMContext = tmvContext;
            _UlkeContext = unitOfWork;
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
            _ParameterContext = DependencyResolver.Current.GetService<IParametreContext>();
        }


        //Müşteri
        #region MusteriGenelBİlgiler Members
        public MusteriGenelBilgiler GetMusteri(int MusteriKodu)
        {
            if (_Aktif.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu &&
                _Aktif.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                return _MusteriContext.MusteriGenelBilgilerRepository.FindById(MusteriKodu);
            else
            {
                IQueryable<MusteriGenelBilgiler> musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
                IQueryable<TVMDetay> yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _Aktif.TVMKodu || s.BagliOlduguTVMKodu == _Aktif.TVMKodu || s.GrupKodu == _Aktif.TVMKodu);

                if (_Aktif.ProjeKodu == TVMProjeKodlari.Mapfre &&
                   (_Aktif.MapfreBolge))
                {
                    if (_Aktif.MapfreBolge)
                    {
                        int bolgeKodu = _TVMContext.TVMDetayRepository.All().Where(w => w.Kodu == _Aktif.TVMKodu)
                                                                            .Select(s => s.BolgeKodu)
                                                                            .FirstOrDefault();

                        yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == 107 &&
                                                                                   s.BolgeKodu == bolgeKodu);
                    }
                    else if (_Aktif.MapfreMerkezAcente)
                    {
                        yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.BagliOlduguTVMKodu == 107 &&
                                                                                   (s.Kodu == _Aktif.TVMKodu || s.GrupKodu == _Aktif.TVMKodu));
                    }
                }

                var musteri = (from m in musteriler
                               join t in yetkiliTvmler on m.TVMKodu equals t.Kodu
                               where m.MusteriKodu == MusteriKodu
                               select m).FirstOrDefault();
                if (musteri == null)
                {
                    musteri = (from m in musteriler
                               where m.MusteriKodu == MusteriKodu
                               select m).FirstOrDefault();
                }

                return musteri;
            }
        }

        public MusteriGenelBilgiler GetMusteri(string KimlikNo, int TVMKodu)
        {
            var musteri = _MusteriContext.MusteriGenelBilgilerRepository
                                     .Filter(s => s.KimlikNo == KimlikNo &&
                                                 s.TVMKodu == TVMKodu).FirstOrDefault();
            return musteri;
            //if (musteri != null)
            //{
            //    return musteri;
            //}
            //else
            //{
            //    var musteriler = _MusteriContext.MusteriGenelBilgilerRepository
            //                         .Filter(s => s.KimlikNo == KimlikNo).FirstOrDefault();
            //    return musteriler;
            //}
        }

        public void UpdateMusteriAdi(MusteriGenelBilgiler bilgi)
        {
            var getMusteri = this.GetMusteri(bilgi.MusteriKodu);
            if (getMusteri != null)
            {
                getMusteri.AdiUnvan = bilgi.AdiUnvan;
            }
            _MusteriContext.MusteriGenelBilgilerRepository.Update(getMusteri);
            _MusteriContext.Commit();
        }


        public MusteriGenelBilgiler GetMusteriTeklifFor(string KimlikNo, int TVMKodu)
        {
            return _MusteriContext.MusteriGenelBilgilerRepository
                                      .Filter(s => s.KimlikNo == KimlikNo &&
                                                s.TVMKodu == TVMKodu).FirstOrDefault();
        }

        public List<MusteriFinderOzetModel> GetMusteriListByTvmKodu(int TVMKodu)
        {
            var musteriList = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == TVMKodu)
                                                            .Select(m => new { m.MusteriKodu, m.AdiUnvan, m.SoyadiUnvan })
                                                            .OrderBy(m => m.MusteriKodu)
                                                            .ToList();

            List<MusteriFinderOzetModel> list = new List<MusteriFinderOzetModel>();
            foreach (var item in musteriList)
            {
                list.Add(new MusteriFinderOzetModel() { MusteriKodu = item.MusteriKodu, Adi = item.AdiUnvan, Soyadi = item.SoyadiUnvan });
            }

            return list;
        }

        public int ToplamMusteriSayisi(int tvmKodu)
        {
            return _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu).Count();
        }

        public int ToplamMusteriSayisi()
        {
            return _MusteriContext.MusteriGenelBilgilerRepository.All().Count();
        }

        public List<MusteriGenelBilgiler> GetSon5Musteri(int tvmKodu)
        {
            return _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu).
                                                                  OrderByDescending(s => s.MusteriKodu).Take(5).
                                                                  ToList<MusteriGenelBilgiler>();
        }

        public MusteriGenelBilgiler CreateMusteri(MusteriGenelBilgiler musteri, MusteriAdre adres, MusteriTelefon telefon)
        {
            musteri.KayitTarihi = TurkeyDateTime.Now;
            musteri = _MusteriContext.MusteriGenelBilgilerRepository.Create(musteri);
            adres.Varsayilan = true;
            musteri.MusteriAdres.Add(adres);
            if (telefon != null && !String.IsNullOrEmpty(telefon.Numara))
                musteri.MusteriTelefons.Add(telefon);

            _MusteriContext.Commit();
            return musteri;
        }

        public MusteriGenelBilgiler CreateMusteri(MusteriGenelBilgiler musteri)
        {
            musteri.KayitTarihi = TurkeyDateTime.Now;
            musteri = _MusteriContext.MusteriGenelBilgilerRepository.Create(musteri);
            _MusteriContext.Commit();
            return musteri;
        }

        public bool UpdateMusteri(MusteriGenelBilgiler musteriGenelBilgiler)
        {
            _MusteriContext.MusteriGenelBilgilerRepository.Update(musteriGenelBilgiler);
            _MusteriContext.Commit();
            return true;
        }

        public bool DeleteMusteri(int Id)
        {
            MusteriGenelBilgiler musteri = _MusteriContext.MusteriGenelBilgilerRepository.FindById(Id);

            if (musteri != null)
            {
                _MusteriContext.MusteriGenelBilgilerRepository.Delete(musteri);
                _MusteriContext.MusteriTelefonRepository.Delete(s => s.MusteriKodu == Id);
                _MusteriContext.MusteriNotRepository.Delete(s => s.MusteriKodu == Id);
                _MusteriContext.MusteriDokumanRepository.Delete(s => s.MusteriKodu == Id);
                _MusteriContext.MusteriAdreRepository.Delete(s => s.MusteriKodu == Id);
                _MusteriContext.Commit();

                return true;
            }

            return false;
        }

        public ParitusAdresModel GetParitusAdres(string adres)
        {
            ParitusAdresModel model = new ParitusAdresModel();

            if (!String.IsNullOrEmpty(adres))
            {
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.Paritus_ServiceURL);
                string apiKey = _KonfigurasyonService.GetKonfigDeger(Konfig.Paritus_ApiKey);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                serviceURL +
                "?id=1" +
                "&address=" + adres +
                "&apikey=" + apiKey);

                request.Method = "GET";
                request.Accept = "application/xml";

                ParitusAdresSorgulamaResponse response = new ParitusAdresSorgulamaResponse();

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    Stream data = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(data);

                    string xml = reader.ReadToEnd();

                    xml = xml.Replace("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?>", "")
                             .Replace("\r\n", "");

                    XmlSerializer xs = new XmlSerializer(typeof(ParitusAdresSorgulamaResponse));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(xml);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;
                        using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8))
                        {
                            response = (ParitusAdresSorgulamaResponse)xs.Deserialize(ms);
                        }
                    }
                }

                if (response != null)
                {
                    if (response.streetHits != null)
                    {
                        if (response.streetHits.Count() == 0)
                        {
                            model.Durum = ParitusAdresSorgulamaDurum.YanlizIlIlce;
                        }
                        else if (response.streetHits.Count() == 1)
                        {
                            model.Durum = ParitusAdresSorgulamaDurum.TekliAdes;
                        }
                        else if (response.streetHits.Count() > 1)
                        {
                            model.Durum = ParitusAdresSorgulamaDurum.CokluAdres;
                            model.CokluAdres = new List<string>();
                            foreach (var item in response.streetHits)
                            {
                                model.CokluAdres.Add(item.original);
                            }
                        }
                    }

                    if (response.parsedAddress != null)
                    {
                        if (!String.IsNullOrEmpty(response.parsedAddress.cityCode))
                        {
                            Il il = _UlkeContext.IlRepository.Filter(s => s.IlKodu == response.parsedAddress.cityCode).FirstOrDefault();
                            if (il != null)
                            {
                                model.IlKodu = il.IlKodu;
                                if (!String.IsNullOrEmpty(response.parsedAddress.town))
                                {
                                    string ilceadi = response.parsedAddress.town.ToUpper();
                                    Ilce ilce = _UlkeContext.IlceRepository.Filter(s => s.IlKodu == il.IlKodu && s.IlceAdi == ilceadi).FirstOrDefault();
                                    if (ilce != null)
                                        model.IlceKodu = ilce.IlceKodu;
                                }
                            }
                        }

                        model.Mahalle = response.parsedAddress.quarter;
                        model.PostaKodu = response.parsedAddress.zipCode;
                        model.BinaNo = response.parsedAddress.houseNumber;
                        model.FullAdres = response.parsedAddress.original;

                        model.Blok = "";
                        model.Kat = "";
                        model.IsMerkezi = "";
                        model.Undefined = "";

                        if (response.parsedAddress.tokens != null)
                        {
                            foreach (var item in response.parsedAddress.tokens)
                            {
                                switch (item.tokenType)
                                {
                                    case "STREET": model.Sokak = item.text; break;
                                    case "QUARTER": model.Mahalle = item.text; break;
                                    case "MAINSTREET": model.Cadde = item.text; break;
                                    case "HOUSENUMBER": model.BinaNo = item.text; break;
                                    case "UNIT": model.DaireNo = item.text; break;
                                    case "ZIP": model.PostaKodu = item.text; break;
                                    case "UNDEFINED": model.Undefined = item.text; break;
                                    case "APARTMENT": model.Apartman = item.text; break;
                                    case "COMMERCIALCOMPLEX": model.IsMerkezi = item.text; break;
                                    case "BLOCK": model.Blok = item.text; break;
                                    case "FLOOR": model.Kat = item.text; break;
                                }
                            }
                        }

                        model.VerificationScore = response.verificationScore;
                        model.Latitude = response.latitude;
                        model.Longitude = response.longitude;

                        model.uavtStreetCode = response.parsedAddress.uavtStreetCode;
                        model.uavtBuildingCode = response.parsedAddress.uavtAddressCode;
                        model.uavtAddressCode = response.parsedAddress.uavtAddressCode;
                    }
                }
                else
                {
                    model.Durum = ParitusAdresSorgulamaDurum.Basarisiz;
                }

            }

            return model;
        }

        public void MusteriEnlemBoylamGetir()
        {
            IParitusService _paritus = DependencyResolver.Current.GetService<IParitusService>();
            IUlkeService _Ulke = DependencyResolver.Current.GetService<IUlkeService>();

            List<MusteriAdre> adresler = _MusteriContext.MusteriAdreRepository.Filter(s => s.Latitude == null && s.Longitude == null).ToList<MusteriAdre>();

            foreach (var adres in adresler)
            {
                ParitusAdresSorgulamaRequest request = new ParitusAdresSorgulamaRequest();

                if (!String.IsNullOrEmpty(adres.Cadde))
                    request.address += adres.Cadde + " cad. ";

                if (!String.IsNullOrEmpty(adres.Mahalle))
                    request.address += adres.Mahalle + " mah. ";

                if (!String.IsNullOrEmpty(adres.Sokak))
                    request.address += adres.Sokak + " ";

                if (!String.IsNullOrEmpty(adres.Semt))
                    request.address += adres.Semt + " ";

                if (!String.IsNullOrEmpty(adres.IlKodu))
                {
                    string iladi = _Ulke.GetIlAdi(adres.UlkeKodu, adres.IlKodu);
                    if (!String.IsNullOrEmpty(iladi))
                        request.address += iladi + " ";
                }

                if (adres.IlceKodu.HasValue)
                {
                    string ilceadi = _Ulke.GetIlceAdi(adres.IlceKodu.Value);
                    if (!String.IsNullOrEmpty(ilceadi))
                        request.address += ilceadi + " ";
                }

                request.address += adres.PostaKodu.ToString();

                ParitusAdresModel model = _paritus.GetParitusAdres(request);

                if (model != null && !String.IsNullOrEmpty(model.Latitude) && !String.IsNullOrEmpty(model.Longitude))
                {
                    adres.Latitude = model.Latitude;
                    adres.Longitude = model.Longitude;
                    _MusteriContext.MusteriAdreRepository.Update(adres);
                }
                _MusteriContext.Commit();
            }



        }

        #endregion

        #region MusteriAdres Members
        //Musteriye Ayit Tum Adresleri Getiren method
        public List<MusteriAdre> GetMusteriAdresleri(int musteriKodu)
        {
            return _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriKodu).ToList<MusteriAdre>();
        }
        public MusteriAdre GetAdres(int musteriKodu, int siraNo)
        {
            return _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriKodu && s.SiraNo == siraNo).First();
        }
        public MusteriAdre GetDefaultAdres(int musteriKodu)
        {
            return _MusteriContext.MusteriAdreRepository.Find(s => s.MusteriKodu == musteriKodu && s.Varsayilan == true);
        }
        public MusteriAdre CreateMusteriAdres(MusteriAdre musteriAdres)
        {
            int? maxSiraNo = _MusteriContext.MusteriAdreRepository.All().Where(f => f.MusteriKodu == musteriAdres.MusteriKodu).Max(m => (int?)m.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            musteriAdres.SiraNo = siraNo;

            //Musteri adresi varsayılan ise diğerleri değiştiriliyor..
            List<MusteriAdre> adresleri = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriAdres.MusteriKodu).ToList<MusteriAdre>();

            if (musteriAdres.Varsayilan == true)
            {
                foreach (var item in adresleri)
                {
                    item.Varsayilan = false;
                    _MusteriContext.MusteriAdreRepository.Update(item);
                }
            }

            musteriAdres = _MusteriContext.MusteriAdreRepository.Create(musteriAdres);
            _MusteriContext.Commit();
            return musteriAdres;
        }
        public bool DeleteAdres(int musteriKodu, int siraNo)
        {
            //Kontrol edilecek
            MusteriAdre adres = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriKodu && s.SiraNo == siraNo).First();
            List<MusteriAdre> adresleri = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriKodu).ToList<MusteriAdre>();
            if (adresleri.Count > 1)
            {
                if (adres.Varsayilan == true)
                {
                    MusteriAdre adr = adresleri.Where(s => s.SiraNo != adres.SiraNo).First();
                    adr.Varsayilan = true;
                    _MusteriContext.MusteriAdreRepository.Update(adr);
                }
                _MusteriContext.MusteriAdreRepository.Delete(adres);
                _MusteriContext.Commit();
                return true;
            }
            return false;
        }
        public bool UpdateAdres(MusteriAdre adres)
        {
            int adresSayisi = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == adres.MusteriKodu).Count();

            MusteriAdre irtibatAdresi = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == adres.MusteriKodu &&
                                                                   s.SiraNo != adres.SiraNo &&
                                                                   s.Varsayilan == true).SingleOrDefault();

            //Adres tek oldugu icin varsayılanı false yapılamaz...
            if (adresSayisi == 1 && adres.Varsayilan == false)
            {
                adres.Varsayilan = true;
                _MusteriContext.MusteriAdreRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }


            //Adres sayısı birden fazla ve varsayılanı true olan başka adres yoksa adres update edilebilir fakat başka bir adres irtibat adresi yapılır..
            if (adresSayisi > 1 && irtibatAdresi != null && adres.Varsayilan == true)
            {
                irtibatAdresi.Varsayilan = false;
                _MusteriContext.MusteriAdreRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }

            //Adres sayısı birden fazla ve guncelllenmek istenen adres irtibat adresiyse varsayılan değeri false yapılamaz
            if (adresSayisi > 1 && irtibatAdresi == null && adres.Varsayilan == false)
            {
                MusteriAdre degisicekAdres = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == adres.MusteriKodu &&
                                                                                          s.SiraNo != adres.SiraNo &&
                                                                                          s.Varsayilan == false).First();
                degisicekAdres.Varsayilan = true;
                _MusteriContext.MusteriAdreRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }
            _MusteriContext.MusteriAdreRepository.Update(adres);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region MusteriTelefon Members

        //Musteriye ait tum kayıtlı telefonlari getiren method
        public List<MusteriTelefon> GetMusteriTelefon(int musteriKodu)
        {
            return _MusteriContext.MusteriTelefonRepository
                                  .Filter(s => s.MusteriKodu == musteriKodu)
                                  .OrderByDescending(o => o.SiraNo)
                                  .ToList<MusteriTelefon>();
        }
        public MusteriTelefon GetTelefon(int musteriKodu, int siraNo)
        {
            return _MusteriContext.MusteriTelefonRepository.Filter(s => s.MusteriKodu == musteriKodu && s.SiraNo == siraNo).FirstOrDefault();
        }
        public MusteriTelefon CreateTelefon(MusteriTelefon musteriTelefon)
        {
            int? maxSiraNo = _MusteriContext.MusteriTelefonRepository.All().Where(f => f.MusteriKodu == musteriTelefon.MusteriKodu).Max(m => (int?)m.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            musteriTelefon.SiraNo = siraNo;

            musteriTelefon = _MusteriContext.MusteriTelefonRepository.Create(musteriTelefon);
            _MusteriContext.Commit();
            return musteriTelefon;
        }
        public bool DeleteTelefon(int musteriKodu, int siraNo)
        {
            _MusteriContext.MusteriTelefonRepository.Delete(s => s.SiraNo == siraNo && s.MusteriKodu == musteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool UpdateTelefon(MusteriTelefon telefon)
        {
            _MusteriContext.MusteriTelefonRepository.Update(telefon);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region MusteriDokuman Members

        //Musteriye ait tum kayıtlı dokumanları getiren method
        public List<DokumanDetayModel> GetMusteriDokumanlari(int musteriKodu)
        {
            IQueryable<MusteriDokuman> docs = _MusteriContext.MusteriDokumanRepository.Filter(s => s.MusteriKodu == musteriKodu);
            IQueryable<TVMKullanicilar> users = _TVMContext.TVMKullanicilarRepository.All();

            var list = from d in docs
                       join u in users on d.TVMPersonelKodu equals u.KullaniciKodu
                       select new
                       {
                           Adi = u.Adi,
                           Soyadi = u.Soyadi,
                           Dosya = d
                       };

            List<DokumanDetayModel> model = new List<DokumanDetayModel>();

            foreach (var item in list)
            {
                DokumanDetayModel document = new DokumanDetayModel();

                document.DosyaAdi = item.Dosya.DosyaAdi;
                document.DokumanTuru = item.Dosya.DokumanTuru;
                document.DokumanURL = item.Dosya.DokumanURL;
                document.KayitTarihi = item.Dosya.KayitTarihi;
                document.SiraNo = item.Dosya.SiraNo;
                document.TvmPersonelAdi = item.Adi + " " + item.Soyadi;
                document.TVMKodu = item.Dosya.TVMKodu;
                document.TVMPersonelKodu = item.Dosya.TVMPersonelKodu;
                document.MusteriKodu = musteriKodu;

                model.Add(document);
            }

            return model;
        }


        public MusteriDokuman GetDokuman(int musteriKodu, int siraNo)
        {
            return _MusteriContext.MusteriDokumanRepository.Filter(s => s.MusteriKodu == musteriKodu && s.SiraNo == siraNo).First();
        }
        public MusteriDokuman CreateDokuman(MusteriDokuman Dokuman)
        {
            int? maxSiraNo = _MusteriContext.MusteriDokumanRepository.All().Where(s => s.MusteriKodu == Dokuman.MusteriKodu).Max(s => (int?)s.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            Dokuman.SiraNo = siraNo;
            Dokuman.KayitTarihi = TurkeyDateTime.Now;

            Dokuman = _MusteriContext.MusteriDokumanRepository.Create(Dokuman);
            _MusteriContext.Commit();
            return Dokuman;
        }
        public bool DeleteDokuman(int musteriKodu, int siraNo)
        {
            _MusteriContext.MusteriDokumanRepository.Delete(s => s.SiraNo == siraNo && s.MusteriKodu == musteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool CheckedFileName(string fileName, int musteriKodu)
        {
            List<MusteriDokuman> dokumanlar = _MusteriContext.MusteriDokumanRepository.Filter(d => d.DosyaAdi == fileName && d.MusteriKodu == musteriKodu).ToList<MusteriDokuman>();
            if (dokumanlar.Count > 0)
                return false;
            else
                return true;
        }
        public bool UpdateDokuman(MusteriDokuman dokuman)
        {
            _MusteriContext.MusteriDokumanRepository.Update(dokuman);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region MusteriNotlar Members


        //Musteriye ait tum notlari getiren method
        public List<NotModelDetay> GetMusteriNotlari(int musteriKodu)
        {
            IQueryable<MusteriNot> notlar = _MusteriContext.MusteriNotRepository.Filter(s => s.MusteriKodu == musteriKodu);
            IQueryable<TVMKullanicilar> users = _TVMContext.TVMKullanicilarRepository.All();

            var list = from n in notlar
                       join u in users on n.TVMPersonelKodu equals u.KullaniciKodu
                       select new
                       {
                           Notlar = n,
                           Adi = u.Adi,
                           Soyadi = u.Soyadi
                       };

            List<NotModelDetay> model = new List<NotModelDetay>();

            foreach (var item in list)
            {
                NotModelDetay not = new NotModelDetay();

                not.KayitTarihi = item.Notlar.KayitTarihi;
                not.Konu = item.Notlar.Konu;
                not.MusteriKodu = item.Notlar.MusteriKodu;
                not.NotAciklamasi = item.Notlar.NotAciklamasi;
                not.SiraNo = item.Notlar.SiraNo;
                not.TVMKodu = item.Notlar.TVMKodu;
                not.TvmPersonelAdi = item.Adi + " " + item.Soyadi;
                not.TVMPersonelKodu = item.Notlar.TVMPersonelKodu;

                model.Add(not);
            }

            return model;
        }
        public MusteriNot GetNot(int musteriKodu, int siraNo)
        {
            return _MusteriContext.MusteriNotRepository.Filter(s => s.MusteriKodu == musteriKodu && s.SiraNo == siraNo).First();
        }
        public MusteriNot CreateNot(MusteriNot Not)
        {
            int? maxSiraNo = _MusteriContext.MusteriNotRepository.All().Where(s => s.MusteriKodu == Not.MusteriKodu).Max(p => (int?)p.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            Not.SiraNo = siraNo;
            Not.KayitTarihi = TurkeyDateTime.Now;

            Not = _MusteriContext.MusteriNotRepository.Create(Not);
            _MusteriContext.Commit();

            return Not;
        }
        public bool DeleteNot(int musteriKodu, int siraNo)
        {
            _MusteriContext.MusteriNotRepository.Delete(s => s.SiraNo == siraNo && s.MusteriKodu == musteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool UpdateNot(MusteriNot not)
        {
            _MusteriContext.MusteriNotRepository.Update(not);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region Meslek

        public Meslek GetMeslek(int meslekKodu)
        {
            return _ParameterContext.MeslekRepository.Filter(s => s.MeslekKodu == meslekKodu).FirstOrDefault();
        }

        #endregion

        #region Extra

        public void XmlToDB()
        {
            int sayac = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\ergin_000\Desktop\musteriler.xml");

            XmlNodeList xnList = doc.SelectNodes("/Musteriler/Musteri");
            foreach (XmlNode xn in xnList)
            {
                MusteriGenelBilgiler musteri = new MusteriGenelBilgiler();
                musteri.KayitTarihi = TurkeyDateTime.Now;
                musteri.TVMKodu = 101;
                musteri.TVMKullaniciKodu = 42;

                musteri.TVMMusteriKodu = xn["TVMMusteriKodu"] != null ? xn["TVMMusteriKodu"].InnerText : "";

                string tipKodu = xn["MusteriTipKodu"] != null ? xn["MusteriTipKodu"].InnerText : "";
                if (!String.IsNullOrEmpty(tipKodu))
                    musteri.MusteriTipKodu = Convert.ToInt16(tipKodu);

                musteri.KimlikNo = xn["KimlikNo"] != null ? xn["KimlikNo"].InnerText : "";
                musteri.VergiDairesi = xn["VergiDairesi"] != null ? xn["VergiDairesi"].InnerText : "";
                musteri.AdiUnvan = xn["AdiUnvan"] != null ? xn["AdiUnvan"].InnerText : "";
                musteri.SoyadiUnvan = xn["SoyadiUnvan"] != null ? xn["SoyadiUnvan"].InnerText : "";
                musteri.Cinsiyet = xn["Cinsiyet"] != null ? xn["Cinsiyet"].InnerText : "";

                string DogumTarihi = xn["DogumTarihi"] != null ? xn["DogumTarihi"].InnerText : "";
                if (!String.IsNullOrEmpty(DogumTarihi))
                    musteri.DogumTarihi = DateTime.Parse(DogumTarihi);

                musteri.EMail = xn["EMail"] != null ? xn["EMail"].InnerText : "";
                musteri.WebUrl = xn["WebUrl"] != null ? xn["WebUrl"].InnerText : "";

                string uyruk = xn["Uyruk"] != null ? xn["Uyruk"].InnerText : "";
                if (!String.IsNullOrEmpty(uyruk))
                    musteri.Uyruk = Convert.ToInt16(uyruk);

                string meslekkodu = xn["MeslekKodu"] != null ? xn["MeslekKodu"].InnerText : "";
                if (!String.IsNullOrEmpty(meslekkodu))
                    musteri.MeslekKodu = Convert.ToInt32(meslekkodu);

                string medenidurum = xn["MedeniDurumu"] != null ? xn["MedeniDurumu"].InnerText : "";
                if (!String.IsNullOrEmpty(medenidurum))
                    musteri.MedeniDurumu = Convert.ToByte(medenidurum);

                foreach (XmlNode adresItem in xn["Adresler"])
                {
                    MusteriAdre adres = new MusteriAdre();

                    string sirano = adresItem["SiraNo"] != null ? adresItem["SiraNo"].InnerText : "";
                    if (!String.IsNullOrEmpty(sirano))
                        adres.SiraNo = Convert.ToInt32(sirano);

                    string adrestipi = adresItem["AdresTipi"] != null ? adresItem["AdresTipi"].InnerText : "";
                    if (!String.IsNullOrEmpty(adrestipi))
                        adres.AdresTipi = Convert.ToInt32(adrestipi);

                    adres.UlkeKodu = adresItem["UlkeKodu"] != null ? adresItem["UlkeKodu"].InnerText : "";
                    adres.IlKodu = adresItem["IlKodu"] != null ? adresItem["IlKodu"].InnerText : "";

                    string ilcekodu = adresItem["IlceKodu"] != null ? adresItem["IlceKodu"].InnerText : "";
                    if (!String.IsNullOrEmpty(ilcekodu))
                        adres.IlceKodu = Convert.ToInt32(ilcekodu);

                    adres.Adres = adresItem["Adres"] != null ? adresItem["Adres"].InnerText : "";
                    adres.Semt = adresItem["Semt"] != null ? adresItem["Semt"].InnerText : "";
                    adres.Mahalle = adresItem["Mahalle"] != null ? adresItem["Mahalle"].InnerText : "";
                    adres.Cadde = adresItem["Cadde"] != null ? adresItem["Cadde"].InnerText : "";
                    adres.Sokak = adresItem["Sokak"] != null ? adresItem["Sokak"].InnerText : "";
                    adres.Apartman = adresItem["Apartman"] != null ? adresItem["Apartman"].InnerText : "";
                    adres.BinaNo = adresItem["BinaNo"] != null ? adresItem["BinaNo"].InnerText : "";
                    adres.DaireNo = adresItem["DaireNo"] != null ? adresItem["DaireNo"].InnerText : "";

                    string postakodu = adresItem["PostaKodu"] != null ? adresItem["PostaKodu"].InnerText : "";
                    if (!String.IsNullOrEmpty(postakodu))
                        adres.PostaKodu = Convert.ToInt32(postakodu);

                    string vars = adresItem["Varsayilan"] != null ? adresItem["Varsayilan"].InnerText : "";
                    if (!String.IsNullOrEmpty(vars))
                        adres.Varsayilan = vars == "1" ? true : false;

                    musteri.MusteriAdres.Add(adres);
                }

                foreach (XmlNode telitem in xn["Telefonlar"])
                {
                    MusteriTelefon tel = new MusteriTelefon();

                    string sirano = telitem["SiraNo"] != null ? telitem["SiraNo"].InnerText : "";
                    if (!String.IsNullOrEmpty(sirano))
                        tel.SiraNo = Convert.ToInt32(sirano);

                    string tip = telitem["IletisimNumaraTipi"] != null ? telitem["IletisimNumaraTipi"].InnerText : "";
                    if (!String.IsNullOrEmpty(tip))
                        tel.IletisimNumaraTipi = Convert.ToInt16(tip);

                    tel.Numara = telitem["Numara"] != null ? telitem["Numara"].InnerText : "";
                    tel.NumaraSahibi = telitem["NumaraSahibi"] != null ? telitem["NumaraSahibi"].InnerText : "";

                    musteri.MusteriTelefons.Add(tel);
                }

                sayac++;
                if (sayac > 100)
                {
                    sayac = 0;
                    _MusteriContext.Commit();
                }

                _MusteriContext.MusteriGenelBilgilerRepository.Create(musteri);
            }
            _MusteriContext.Commit();
        }

        #endregion

        //Haritada Muşterileri Gösterir
        public List<MusteriHaritaOzelDetay> MusterilerimHaritaArama(MusteriharitaAramaModel arama)
        {
            List<MusteriHaritaOzelDetay> model = new List<MusteriHaritaOzelDetay>();

            IQueryable<MusteriGenelBilgiler> QmusteriList = _MusteriContext.MusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == arama.TVMKodu);

            int sayac = 0;
            switch (arama.MusteriSayisi)
            {
                case 1: sayac = 25; break;
                case 2: sayac = 50; break;
                case 3: sayac = 75; break;
                case 4: sayac = 100; break;
                case 5: sayac = 200; break;
                default: sayac = 300; break;
            }

            if (arama.MusteriTipi > 0)
                QmusteriList = QmusteriList.Where(s => s.MusteriTipKodu == arama.MusteriTipi);

            if (!String.IsNullOrEmpty(arama.Ad))
                QmusteriList = QmusteriList.Where(s => s.AdiUnvan.StartsWith(arama.Ad));

            if (!String.IsNullOrEmpty(arama.Soyad))
                QmusteriList = QmusteriList.Where(s => s.SoyadiUnvan.StartsWith(arama.Soyad));

            int eklenen = 0;
            foreach (var mus in QmusteriList)
            {
                MusteriHaritaOzelDetay mdl = new MusteriHaritaOzelDetay();

                mdl.MusteriTipi = mus.MusteriTipKodu;
                mdl.MusteriKodu = mus.MusteriKodu;
                mdl.AdiSoyadi = mus.AdiUnvan + " " + mus.SoyadiUnvan;
                mdl.Email = mus.EMail;

                MusteriTelefon cep = mus.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();

                if (cep != null)
                    mdl.Tel = "Cep : " + cep.Numara;

                MusteriAdre adres = mus.MusteriAdres.Where(s => s.Varsayilan.HasValue && s.Varsayilan.Value).FirstOrDefault();

                if (adres != null)
                {
                    mdl.Latitude = adres.Latitude;
                    mdl.Longitude = adres.Longitude;
                }
                model.Add(mdl);

                eklenen++;

                if (sayac < eklenen)
                    break;
            }



            return model;
        }

        public DataTableList PagedListTelefon(DataTableParameters<MusteriTelefon> telefonList, int musteriKodu)
        {
            IQueryable<MusteriTelefon> query = _MusteriContext.MusteriTelefonRepository.Filter(s => s.MusteriKodu == musteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.MusteriTelefonRepository.Page(query,
                                                                  telefonList.OrderByProperty,
                                                                  telefonList.IsAscendingOrder,
                                                                  telefonList.Page,
                                                                  telefonList.PageSize, out totalRowCount);
            return telefonList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListAdres(DataTableParameters<MusteriAdre> adresList, int musteriKodu)
        {
            IQueryable<MusteriAdre> query = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == musteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.MusteriAdreRepository.Page(query,
                                                                  adresList.OrderByProperty,
                                                                  adresList.IsAscendingOrder,
                                                                  adresList.Page,
                                                                  adresList.PageSize, out totalRowCount);
            return adresList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListDokuman(DataTableParameters<MusteriDokuman> dokumanList, int musteriKodu)
        {
            IQueryable<MusteriDokuman> query = _MusteriContext.MusteriDokumanRepository.Filter(s => s.MusteriKodu == musteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.MusteriDokumanRepository.Page(query,
                                                                  dokumanList.OrderByProperty,
                                                                  dokumanList.IsAscendingOrder,
                                                                  dokumanList.Page,
                                                                  dokumanList.PageSize, out totalRowCount);
            return dokumanList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListNot(DataTableParameters<MusteriNot> notList, int musteriKodu)
        {
            IQueryable<MusteriNot> query = _MusteriContext.MusteriNotRepository.Filter(s => s.MusteriKodu == musteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.MusteriNotRepository.Page(query,
                                                                  notList.OrderByProperty,
                                                                  notList.IsAscendingOrder,
                                                                  notList.Page,
                                                                  notList.PageSize, out totalRowCount);
            return notList.Prepare(query, totalRowCount);
        }

        public List<MusteriListeModelOzel> PagedList(MusteriListe arama, out int totalRowCount)
        {
            IAktifKullaniciService aktifServis = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            IQueryable<TVMDetay> tvm = _TVMContext.TVMDetayRepository.All();
            IQueryable<MusteriGenelBilgiler> musteriler;

            // ==== Kullanıcı Neosinerji tvm sine bağlıysa sınırlama yok değilse sınırlı yetki ==== //
            int aktifTVMKodu = _Aktif.TVMKodu;
            if (aktifServis.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _Aktif.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
            else
            {
                IQueryable<TVMDetay> YetkiliTvmList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == aktifTVMKodu || s.BagliOlduguTVMKodu == aktifTVMKodu);

                IQueryable<MusteriGenelBilgiler> ALLmusteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();

                musteriler = from m in ALLmusteriler
                             join tv in YetkiliTvmList on m.TVMKodu equals tv.Kodu
                             select m;
            }


            CultureInfo culture = new CultureInfo("tr-TR");
            //String olmayan alanlar...
            if (arama.MusteriKodu.HasValue)
                musteriler = musteriler.Where(w => w.MusteriKodu == arama.MusteriKodu.Value);

            //if (arama.yasBaslangic >0 && arama.yasBitis > 0)
            //    musteriler = musteriler.Where(w => w.DogumTarihi.Value.Year>= arama.yasBaslangic && w.DogumTarihi.Value.Year<= arama.yasBitis);

            if (arama.TVMKodu.HasValue)
                musteriler = musteriler.Where(w => w.TVMKodu == arama.TVMKodu.Value);

            if (arama.MusteriTipKodu.HasValue)
                musteriler = musteriler.Where(w => w.MusteriTipKodu == arama.MusteriTipKodu.Value);

            //sring alanlar...
            if (!String.IsNullOrEmpty(arama.AdiUnvan))
                musteriler = musteriler.Where(w => w.AdiUnvan.Trim().ToLower().Replace("ı", "i").StartsWith(arama.AdiUnvan.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.SoyadiUnvan))
                musteriler = musteriler.Where(w => w.SoyadiUnvan.Trim().ToLower().Replace("ı", "i").StartsWith(arama.SoyadiUnvan.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.EMail))
                musteriler = musteriler.Where(w => w.EMail.Trim().ToLower().Replace("ı", "i").StartsWith(arama.EMail.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.TVMMusteriKodu))
                musteriler = musteriler.Where(w => w.TVMMusteriKodu.Trim().ToLower().Replace("ı", "i").StartsWith(arama.TVMMusteriKodu.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.KimlikNo))
                musteriler = musteriler.Where(w => w.KimlikNo.Trim().ToLower().Replace("ı", "i").StartsWith(arama.KimlikNo.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.PasaportNo))
                musteriler = musteriler.Where(w => w.PasaportNo.Trim().ToLower().Replace("ı", "i").StartsWith(arama.PasaportNo.Trim().ToLower().Replace("ı", "i")));


            IQueryable<MusteriGenelBilgiler> query = from k in musteriler
                                                     join t in tvm on k.TVMKodu equals t.Kodu
                                                     select k;

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in musteriler
                        join t in tvm on k.TVMKodu equals t.Kodu
                        select new { k.MusteriKodu, k.TVMMusteriKodu, k.MusteriTipKodu, AdiUnvan = k.AdiUnvan + " " + k.SoyadiUnvan, k.EMail, k.Cinsiyet, k.DogumTarihi, TVMUnvani = t.Unvani })
            .OrderBy(o => o.AdiUnvan)
            .Skip(excludedRows)
            .Take(arama.PageSize)
            .ToList();

            List<MusteriListeModelOzel> listeModel = new List<MusteriListeModelOzel>();

            foreach (var item in list)
            {
                MusteriListeModelOzel model = new MusteriListeModelOzel();

                model.MusteriKodu = item.MusteriKodu;
                model.TVMMusteriKodu = item.TVMMusteriKodu;
                model.MusteriTipKodu = item.MusteriTipKodu;
                model.AdiUnvan = item.AdiUnvan;
                model.EMail = item.EMail;
                model.Cinsiyet = item.Cinsiyet;
                model.DogumTarihi = item.DogumTarihi.HasValue ? item.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";
                model.BagliOlduguTvmText = item.TVMUnvani;

                listeModel.Add(model);
            }

            return listeModel;

        }

        public List<MusteriListesiModelOzel> PagedMusteriList(MusteriListesi arama, out int totalRowCount)
        {
            IAktifKullaniciService aktifServis = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            IQueryable<TVMDetay> tvm = _TVMContext.TVMDetayRepository.All();
            IQueryable<MusteriGenelBilgiler> musteriler;

            // ==== Kullanıcı Neosinerji tvm sine bağlıysa sınırlama yok değilse sınırlı yetki ==== //
            int aktifTVMKodu = _Aktif.TVMKodu;
            if (aktifServis.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _Aktif.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                musteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();
            else
            {
                IQueryable<TVMDetay> YetkiliTvmList = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == aktifTVMKodu || s.BagliOlduguTVMKodu == aktifTVMKodu);

                IQueryable<MusteriGenelBilgiler> ALLmusteriler = _MusteriContext.MusteriGenelBilgilerRepository.All();

                musteriler = from m in ALLmusteriler
                             join tv in YetkiliTvmList on m.TVMKodu equals tv.Kodu
                             select m;
            }

            CultureInfo culture = new CultureInfo("tr-TR");
            //String olmayan alanlar...
            if (arama.MusteriKodu.HasValue)
                musteriler = musteriler.Where(w => w.MusteriKodu == arama.MusteriKodu.Value);

            if (arama.yasBaslangic > 0 && arama.yasBitis > 0)
                musteriler = musteriler.Where(w => w.DogumTarihi.Value.Year >= arama.yasBitis && w.DogumTarihi.Value.Year <= arama.yasBaslangic);
            else if (arama.yasBaslangic > 0 && arama.yasBitis == 0)
            {
                musteriler = musteriler.Where(w => w.DogumTarihi.Value.Year <= arama.yasBaslangic);
            }

            if (arama.TVMKodu.HasValue)
                musteriler = musteriler.Where(w => w.TVMKodu == arama.TVMKodu.Value);

            if (arama.MusteriTipKodu.HasValue)
                musteriler = musteriler.Where(w => w.MusteriTipKodu == arama.MusteriTipKodu.Value);

            if (arama.DogumTarihi.HasValue)
                musteriler = musteriler.Where(w => w.DogumTarihi == arama.DogumTarihi.Value);

            if (arama.MeslekKodu.HasValue)
                musteriler = musteriler.Where(w => w.MeslekKodu == arama.MeslekKodu.Value);




            //sring alanlar...
            if (!String.IsNullOrEmpty(arama.AdiUnvan))
                musteriler = musteriler.Where(w => w.AdiUnvan.Trim().ToLower().Replace("ı", "i").StartsWith(arama.AdiUnvan.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.SoyadiUnvan))
                musteriler = musteriler.Where(w => w.SoyadiUnvan.Trim().ToLower().Replace("ı", "i").StartsWith(arama.SoyadiUnvan.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.EMail))
                musteriler = musteriler.Where(w => w.EMail.Trim().ToLower().Replace("ı", "i").StartsWith(arama.EMail.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.TVMMusteriKodu))
                musteriler = musteriler.Where(w => w.TVMMusteriKodu.Trim().ToLower().Replace("ı", "i").StartsWith(arama.TVMMusteriKodu.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.KimlikNo))
                musteriler = musteriler.Where(w => w.KimlikNo.Trim().ToLower().Replace("ı", "i").StartsWith(arama.KimlikNo.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.PasaportNo))
                musteriler = musteriler.Where(w => w.PasaportNo.Trim().ToLower().Replace("ı", "i").StartsWith(arama.PasaportNo.Trim().ToLower().Replace("ı", "i")));

            if (!String.IsNullOrEmpty(arama.Cinsiyet))
                musteriler = musteriler.Where(w => w.Cinsiyet.Trim().ToLower().Replace("ı", "i").StartsWith(arama.Cinsiyet.Trim().ToLower().Replace("ı", "i")));
           else
            {
                musteriler = musteriler.Where(w => w.Cinsiyet == null || w.Cinsiyet == "");

            }










            IQueryable<MusteriGenelBilgiler> query = from k in musteriler
                                                     join t in tvm on k.TVMKodu equals t.Kodu
                                                     select k;


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;


            IQueryable<Il> iller = _UlkeContext.IlRepository.All();
            IQueryable<Ilce> ilceler = _UlkeContext.IlceRepository.All();


            var list = (from k in musteriler
                        join t in tvm on k.TVMKodu equals t.Kodu
                        select new
                        {
                            k.MusteriKodu,
                            k.TVMMusteriKodu,
                            k.MusteriTipKodu,
                            AdiUnvan = k.AdiUnvan + " " + k.SoyadiUnvan,
                            k.EMail,
                            k.Cinsiyet,
                            k.DogumTarihi,
                            TVMUnvani = t.Unvani,
                            k.MedeniDurumu,
                            k.EgitimDurumu,
                            k.MeslekKodu,
                            k.MusteriAdres,
                            k.MusteriTelefons,
                            k.KimlikNo,
                            k.TVMKullaniciKodu
                        })
                .OrderBy(o => o.AdiUnvan)
                .Skip(excludedRows)
                .Take(arama.PageSize)
                .ToList();



            List<MusteriListesiModelOzel> listesiModel = new List<MusteriListesiModelOzel>();

            foreach (var item in list)
            {
                MusteriListesiModelOzel model = new MusteriListesiModelOzel();

                model.MusteriKodu = item.MusteriKodu;
                model.MusteriTipKodu = item.MusteriTipKodu;
                model.MusteriGrupKodu = item.TVMMusteriKodu;
                model.AdiSoyadiUnvan = !String.IsNullOrEmpty(item.AdiUnvan) ? item.AdiUnvan : "";
                model.EMail = !String.IsNullOrEmpty(item.EMail) ? item.EMail : "";
                model.Cinsiyet = !String.IsNullOrEmpty(item.Cinsiyet) ? item.Cinsiyet : "";
                model.DogumTarihi = item.DogumTarihi.HasValue ? item.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";
                model.BagliOlduguTvmText = item.TVMUnvani;
                model.MedeniDurumu = item.MedeniDurumu;
                model.EgitimDurumu = item.EgitimDurumu;
                model.MeslekKodu = item.MeslekKodu;
                model.KimlikNo = item.KimlikNo;
                var MusAdres = item.MusteriAdres.Where(w => w.Varsayilan == true).FirstOrDefault();
                if (MusAdres != null)
                {
                    string ilAdi = "";
                    string ilceAdi = "";
                    foreach (var itemAdres in item.MusteriAdres.Where(w => w.Varsayilan == true))
                    {
                        if (!String.IsNullOrEmpty(itemAdres.IlKodu))
                        {
                            ilAdi = iller.Where(W => W.IlKodu == itemAdres.IlKodu).Select(s => s.IlAdi).FirstOrDefault();
                        }
                        if (itemAdres.IlceKodu.HasValue)
                        {
                            ilceAdi = ilceler.Where(W => W.IlceKodu == itemAdres.IlceKodu).Select(s => s.IlceAdi).FirstOrDefault();
                        }
                        model.IlIlce = ilAdi + "/" + ilceAdi;
                    }
                }
                else
                {
                    model.IlIlce = "";

                }

                model.Telefons = item.MusteriTelefons.ToList();
                foreach (var itemTel in model.Telefons)
                {
                    if (itemTel.IletisimNumaraTipi == IletisimNumaraTipleri.Ev)
                    {
                        model.EvTel = itemTel.Numara.Length > 11 ? itemTel.Numara : "";
                    }
                    else if (itemTel.IletisimNumaraTipi == IletisimNumaraTipleri.Cep)
                    {
                        model.CepTel = itemTel.Numara.Length > 11 ? itemTel.Numara : "";
                    }
                    else
                    {
                        model.EvTel = "";
                        model.CepTel = "";

                    }
                }

                if (model.MeslekKodu.HasValue)
                {
                    model.MeslekKoduText = _TanimService.GetMeslek(model.MeslekKodu.Value).MeslekAdi;
                }
                else
                {
                    model.MeslekKoduText = "";
                }
                int kullaniciKodu = Convert.ToInt32(item.TVMKullaniciKodu);
                var kullanici = _TVMContext.TVMKullanicilarRepository.All().Where(w => w.KullaniciKodu == kullaniciKodu).FirstOrDefault();
                if (kullanici != null)
                {
                    model.KaydedenKullanici = kullanici.Adi + " " + kullanici.Soyadi;
                }
                else model.KaydedenKullanici = "";



                listesiModel.Add(model);
            }

            return listesiModel;
        }

        public List<MusteriAdediModelOzel> PagedMusteriAdetList(MusteriAdedi arama)
        {
            List<int> tvmList = new List<int>();
            var tvmKodlari = arama.TVMKodlari.Split(',');
            foreach (var item in tvmKodlari)
            {
                tvmList.Add(Convert.ToInt32(item));
            }

            List<MusteriAdediModelOzel> adediModel = new List<MusteriAdediModelOzel>();
            MusteriAdediModelOzel model;
            

            foreach (var item in tvmList)
            {
                model = new MusteriAdediModelOzel();
                
                model.SahisFirmasiCount = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w =>w.TVMKodu==item && w.MusteriTipKodu == MusteriTipleri.SahisFirmasi).Count();
                model.TCMusteriCount = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMKodu == item && w.MusteriTipKodu == MusteriTipleri.TCMusteri).Count();
                model.TuzelMusteriCount = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMKodu == item && w.MusteriTipKodu == MusteriTipleri.TuzelMusteri).Count();
                model.YabanciMusteriCount = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMKodu == item && w.MusteriTipKodu == MusteriTipleri.YabanciMusteri).Count();
                model.MusteriToplamCount = _MusteriContext.MusteriGenelBilgilerRepository.All().Where(w => w.TVMKodu == item).Count();
                model.tvmUnvani = _TVMContext.TVMDetayRepository.All().Where(w => w.Kodu == item).Select(s => s.Unvani).FirstOrDefault();

                adediModel.Add(model);

            }

            int sahirFirmaTotal = adediModel.Sum(p => p.SahisFirmasiCount);
            int tcmusteriTotal = adediModel.Sum(p => p.TCMusteriCount);
            int tuzelMusteriTotal = adediModel.Sum(p => p.TuzelMusteriCount);

            int yabanciMusteriTotal = adediModel.Sum(p => p.YabanciMusteriCount);
            int musteriToplamTotal = adediModel.Sum(p => p.MusteriToplamCount);
            

            adediModel.Add(new MusteriAdediModelOzel() {
                tvmUnvani="Genel Toplam",
                SahisFirmasiCount=sahirFirmaTotal,
                TCMusteriCount= tcmusteriTotal,
                TuzelMusteriCount=tuzelMusteriTotal,
                YabanciMusteriCount=yabanciMusteriTotal,
                MusteriToplamCount=musteriToplamTotal
            });

            

            return adediModel;
        }

        #region Potansiyel Müşteri

        public PotansiyelMusteriGenelBilgiler GetPotansiyelMusteri(int musteriKodu)
        {
            if (_Aktif.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu && _Aktif.YetkiGrubu == NeosinerjiTVM.NeosinerjiYoneticiYetkisi)
                return _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.FindById(musteriKodu);
            else
            {
                IQueryable<PotansiyelMusteriGenelBilgiler> musteriler = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.All();
                IQueryable<TVMDetay> yetkiliTvmler = _TVMContext.TVMDetayRepository.Filter(s => s.Kodu == _Aktif.TVMKodu || s.BagliOlduguTVMKodu == _Aktif.TVMKodu);

                var musteri = (from m in musteriler
                               join t in yetkiliTvmler on m.TVMKodu equals t.Kodu
                               where m.PotansiyelMusteriKodu == musteriKodu
                               select m).FirstOrDefault();


                return musteri;

            }
        }

        public PotansiyelMusteriGenelBilgiler GetPotansiyelMusteri(string KimlikNo, int TVMKodu)
        {
            PotansiyelMusteriGenelBilgiler potansiyelMusteriGenelBilgiler = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Filter(s => s.KimlikNo == KimlikNo &&
                                                                                                              s.TVMKodu == TVMKodu).SingleOrDefault();
            return potansiyelMusteriGenelBilgiler;
        }

        public List<PotansiyelMusteriFinderOzetModel> GetPotansiyelMusteriListByTvmKodu(int TVMKodu)
        {
            var potansiyelMusteriList = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == TVMKodu)
                                                            .Select(m => new { m.PotansiyelMusteriKodu, m.AdiUnvan, m.SoyadiUnvan })
                                                            .OrderBy(m => m.PotansiyelMusteriKodu)
                                                            .ToList();

            List<PotansiyelMusteriFinderOzetModel> list = new List<PotansiyelMusteriFinderOzetModel>();
            foreach (var item in potansiyelMusteriList)
            {
                list.Add(new PotansiyelMusteriFinderOzetModel() { MusteriKodu = item.PotansiyelMusteriKodu, Adi = item.AdiUnvan, Soyadi = item.SoyadiUnvan });
            }

            return list;
        }

        public int ToplamPotansiyelMusteriSayisi(int tvmKodu)
        {
            return _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu).Count();
        }

        public int ToplamPotansiyelMusteriSayisi()
        {
            return _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.All().Count();
        }

        public List<PotansiyelMusteriGenelBilgiler> GetSon5PotansiyelMusteri(int tvmKodu)
        {
            return _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == tvmKodu).
                                                                  OrderByDescending(s => s.PotansiyelMusteriKodu).Take(5).
                                                                  ToList<PotansiyelMusteriGenelBilgiler>();
        }

        public PotansiyelMusteriGenelBilgiler CreatePotansiyelMusteri(PotansiyelMusteriGenelBilgiler musteri, PotansiyelMusteriAdre adres, PotansiyelMusteriTelefon telefon)
        {
            musteri.KayitTarihi = TurkeyDateTime.Now;
            musteri = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Create(musteri);

            if (adres != null && adres.AdresTipi.HasValue && adres.AdresTipi.Value > 0)
            {
                adres.Varsayilan = true;
                musteri.PotansiyelMusteriAdres.Add(adres);
            }

            if (telefon != null && telefon.IletisimNumaraTipi > 0 && telefon.Numara.Length == 14)
                musteri.PotansiyelMusteriTelefons.Add(telefon);

            _MusteriContext.Commit();
            return musteri;
        }

        public bool UpdatePotansiyelMusteri(PotansiyelMusteriGenelBilgiler potansiyelMusteriGenelBilgiler)
        {
            _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Update(potansiyelMusteriGenelBilgiler);
            _MusteriContext.Commit();
            return true;
        }

        public bool DeletePotansiyelMusteri(int Id)
        {
            PotansiyelMusteriGenelBilgiler musteri = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.FindById(Id);

            if (musteri != null)
            {
                _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Delete(musteri);
                _MusteriContext.Commit();

                return true;
            }

            return false;
        }

        #endregion

        #region PotansiyelMusteriAdres Members
        //Musteriye Ayit Tum Adresleri Getiren method
        public List<PotansiyelMusteriAdre> GetPotansiyelMusteriAdresleri(int potansiyelMusteriKodu)
        {
            return _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu).ToList<PotansiyelMusteriAdre>();
        }
        public PotansiyelMusteriAdre GetPotansiyelAdres(int potansiyelMusteriKodu, int siraNo)
        {
            return _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.SiraNo == siraNo).FirstOrDefault();
        }

        //public PotansiyelMusteriAdre GetPotansiyelAdres(int potansiyelMusteriKodu)
        //{
        //    return _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.Varsayilan == true).FirstOrDefault();
        //}

        public PotansiyelMusteriAdre GetDefaultPotansiyelAdres(int potansiyelMusteriKodu)
        {
            return _MusteriContext.PotansiyelMusteriAdresRepository.Find(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.Varsayilan == true);
        }
        public PotansiyelMusteriAdre CreatePotansiyelMusteriAdres(PotansiyelMusteriAdre potansiyelMusteriAdres)
        {
            int? maxSiraNo = _MusteriContext.PotansiyelMusteriAdresRepository.All().Where(f => f.PotansiyelMusteriKodu == potansiyelMusteriAdres.PotansiyelMusteriKodu).Max(m => (int?)m.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            potansiyelMusteriAdres.SiraNo = siraNo;

            if (potansiyelMusteriAdres.AdresTipi.HasValue)
            {
                if (!String.IsNullOrEmpty(potansiyelMusteriAdres.Adres))
                    potansiyelMusteriAdres.Adres = "";


                //Musteri adresi varsayılan ise diğerleri değiştiriliyor..
                List<PotansiyelMusteriAdre> adresleri = _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriAdres.PotansiyelMusteriKodu).ToList<PotansiyelMusteriAdre>();

                if (potansiyelMusteriAdres.Varsayilan == true)
                {
                    foreach (var item in adresleri)
                    {
                        item.Varsayilan = false;
                        _MusteriContext.PotansiyelMusteriAdresRepository.Update(item);
                    }
                }
                else
                {
                    if (adresleri.Count == 0)
                        potansiyelMusteriAdres.Varsayilan = true;
                }

                potansiyelMusteriAdres = _MusteriContext.PotansiyelMusteriAdresRepository.Create(potansiyelMusteriAdres);
                _MusteriContext.Commit();
            }
            return potansiyelMusteriAdres;
        }
        public bool DeletePotansiyelAdres(int potansiyelMusteriKodu, int siraNo)
        {
            //Kontrol edilecek
            PotansiyelMusteriAdre adres = _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.SiraNo == siraNo).First();
            List<PotansiyelMusteriAdre> adresleri = _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu).ToList<PotansiyelMusteriAdre>();
            if (adresleri.Count > 1)
            {
                if (adres.Varsayilan == true)
                {
                    PotansiyelMusteriAdre adr = adresleri.Where(s => s.SiraNo != adres.SiraNo).First();
                    adr.Varsayilan = true;
                    _MusteriContext.PotansiyelMusteriAdresRepository.Update(adr);
                }
                _MusteriContext.PotansiyelMusteriAdresRepository.Delete(adres);
                _MusteriContext.Commit();
                return true;
            }
            return false;
        }
        public bool UpdatePotansiyelAdres(PotansiyelMusteriAdre adres)
        {
            int adresSayisi = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == adres.PotansiyelMusteriKodu).Count();

            PotansiyelMusteriAdre irtibatAdresi = _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == adres.PotansiyelMusteriKodu &&
                                                                   s.SiraNo != adres.SiraNo &&
                                                                   s.Varsayilan == true).SingleOrDefault();

            //Adres tek oldugu icin varsayılanı false yapılamaz...
            if (adresSayisi == 1 && adres.Varsayilan == false)
            {
                adres.Varsayilan = true;
                _MusteriContext.PotansiyelMusteriAdresRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }


            //Adres sayısı birden fazla ve varsayılanı true olan başka adres yoksa adres update edilebilir fakat başka bir adres irtibat adresi yapılır..
            if (adresSayisi > 1 && irtibatAdresi != null && adres.Varsayilan == true)
            {
                irtibatAdresi.Varsayilan = false;
                _MusteriContext.PotansiyelMusteriAdresRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }

            //Adres sayısı birden fazla ve guncelllenmek istenen adres irtibat adresiyse varsayılan değeri false yapılamaz
            if (adresSayisi > 1 && irtibatAdresi == null && adres.Varsayilan == false)
            {
                MusteriAdre degisicekAdres = _MusteriContext.MusteriAdreRepository.Filter(s => s.MusteriKodu == adres.PotansiyelMusteriKodu &&
                                                                                          s.SiraNo != adres.SiraNo &&
                                                                                          s.Varsayilan == false).First();
                degisicekAdres.Varsayilan = true;
                _MusteriContext.PotansiyelMusteriAdresRepository.Update(adres);
                _MusteriContext.Commit();
                return true;
            }
            _MusteriContext.PotansiyelMusteriAdresRepository.Update(adres);
            _MusteriContext.Commit();
            return true;
        }

        #endregion

        #region Potansiyel MusteriTelefon Members

        //Musteriye ait tum kayıtlı telefonlari getiren method
        public List<PotansiyelMusteriTelefon> GetPotansiyelMusteriTelefon(int potansiyelMusteriKodu)
        {
            return _MusteriContext.PotansiyelMusteriTelefonRepository
                                  .Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu)
                                  .OrderByDescending(o => o.SiraNo)
                                  .ToList<PotansiyelMusteriTelefon>();
        }
        public PotansiyelMusteriTelefon GetPotansiyelTelefon(int potansiyelMusteriKodu, int siraNo)
        {
            return _MusteriContext.PotansiyelMusteriTelefonRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.SiraNo == siraNo).FirstOrDefault();
        }
        public PotansiyelMusteriTelefon CreatePotansiyelTelefon(PotansiyelMusteriTelefon potansiyelMusteriTelefon)
        {
            int? maxSiraNo = _MusteriContext.PotansiyelMusteriTelefonRepository.All().Where(f => f.PotansiyelMusteriKodu == potansiyelMusteriTelefon.PotansiyelMusteriKodu).Max(m => (int?)m.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            potansiyelMusteriTelefon.SiraNo = siraNo;

            potansiyelMusteriTelefon = _MusteriContext.PotansiyelMusteriTelefonRepository.Create(potansiyelMusteriTelefon);
            _MusteriContext.Commit();
            return potansiyelMusteriTelefon;
        }
        public bool DeletePotansiyelTelefon(int potansiyelMusteriKodu, int siraNo)
        {
            _MusteriContext.PotansiyelMusteriTelefonRepository.Delete(s => s.SiraNo == siraNo && s.PotansiyelMusteriKodu == potansiyelMusteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool UpdatePotansiyelTelefon(PotansiyelMusteriTelefon telefon)
        {
            _MusteriContext.PotansiyelMusteriTelefonRepository.Update(telefon);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region Potansiyel MusteriDokuman Members

        //Musteriye ait tum kayıtlı dokumanları getiren method
        public List<PotansiyelDokumanDetayModel> GetPotansiyelMusteriDokumanlari(int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriDokuman> docs = _MusteriContext.PotansiyelMusteriDokumanRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);
            IQueryable<TVMKullanicilar> users = _TVMContext.TVMKullanicilarRepository.All();

            var list = from d in docs
                       join u in users on d.TVMPersonelKodu equals u.KullaniciKodu
                       select new
                       {
                           Adi = u.Adi,
                           Soyadi = u.Soyadi,
                           Dosya = d
                       };

            List<PotansiyelDokumanDetayModel> model = new List<PotansiyelDokumanDetayModel>();

            foreach (var item in list)
            {
                PotansiyelDokumanDetayModel document = new PotansiyelDokumanDetayModel();

                document.DosyaAdi = item.Dosya.DosyaAdi;
                document.DokumanTuru = item.Dosya.DokumanTuru;
                document.DokumanURL = item.Dosya.DokumanURL;
                document.KayitTarihi = item.Dosya.KayitTarihi;
                document.SiraNo = item.Dosya.SiraNo;
                document.TvmPersonelAdi = item.Adi + " " + item.Soyadi;
                document.TVMKodu = item.Dosya.TVMKodu;
                document.TVMPersonelKodu = item.Dosya.TVMPersonelKodu;
                document.MusteriKodu = potansiyelMusteriKodu;

                model.Add(document);
            }

            return model;
        }
        public PotansiyelMusteriDokuman GetPotansiyelDokuman(int potansiyelMusteriKodu, int siraNo)
        {
            return _MusteriContext.PotansiyelMusteriDokumanRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.SiraNo == siraNo).First();
        }
        public PotansiyelMusteriDokuman CreatePotansiyelDokuman(PotansiyelMusteriDokuman Dokuman)
        {
            int? maxSiraNo = _MusteriContext.PotansiyelMusteriDokumanRepository.All().Where(s => s.PotansiyelMusteriKodu == Dokuman.PotansiyelMusteriKodu).Max(s => (int?)s.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            Dokuman.SiraNo = siraNo;

            Dokuman = _MusteriContext.PotansiyelMusteriDokumanRepository.Create(Dokuman);
            _MusteriContext.Commit();
            return Dokuman;
        }
        public bool DeletePotansiyelDokuman(int potansiyelMusteriKodu, int siraNo)
        {
            _MusteriContext.PotansiyelMusteriDokumanRepository.Delete(s => s.SiraNo == siraNo && s.PotansiyelMusteriKodu == potansiyelMusteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool PotansiyelCheckedFileName(string fileName, int potansiyelMusteriKodu)
        {
            List<PotansiyelMusteriDokuman> dokumanlar = _MusteriContext.PotansiyelMusteriDokumanRepository.Filter(d => d.DosyaAdi == fileName && d.PotansiyelMusteriKodu == potansiyelMusteriKodu).ToList<PotansiyelMusteriDokuman>();
            if (dokumanlar.Count > 0)
                return false;
            else
                return true;
        }
        public bool UpdatePotansiyelDokuman(PotansiyelMusteriDokuman dokuman)
        {
            _MusteriContext.PotansiyelMusteriDokumanRepository.Update(dokuman);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        #region Potansiyel MusteriNotlar Members


        //Musteriye ait tum notlari getiren method
        public List<PotansiyelNotModelDetay> GetPotansiyelMusteriNotlari(int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriNot> notlar = _MusteriContext.PotansiyelMusteriNotRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);
            IQueryable<TVMKullanicilar> users = _TVMContext.TVMKullanicilarRepository.All();

            var list = from n in notlar
                       join u in users on n.TVMPersonelKodu equals u.KullaniciKodu
                       select new
                       {
                           Notlar = n,
                           Adi = u.Adi,
                           Soyadi = u.Soyadi
                       };

            List<PotansiyelNotModelDetay> model = new List<PotansiyelNotModelDetay>();

            foreach (var item in list)
            {
                PotansiyelNotModelDetay not = new PotansiyelNotModelDetay();

                not.KayitTarihi = item.Notlar.KayitTarihi;
                not.Konu = item.Notlar.Konu;
                not.PotansiyelMusteriKodu = item.Notlar.PotansiyelMusteriKodu;
                not.NotAciklamasi = item.Notlar.NotAciklamasi;
                not.SiraNo = item.Notlar.SiraNo;
                not.TVMKodu = item.Notlar.TVMKodu;
                not.TvmPersonelAdi = item.Adi + " " + item.Soyadi;
                not.TVMPersonelKodu = item.Notlar.TVMPersonelKodu;

                model.Add(not);
            }

            return model;
        }
        public PotansiyelMusteriNot GetPotansiyelNot(int potansiyelMusteriKodu, int siraNo)
        {
            return _MusteriContext.PotansiyelMusteriNotRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu && s.SiraNo == siraNo).First();
        }
        public PotansiyelMusteriNot CreatePotansiyelNot(PotansiyelMusteriNot Not)
        {
            int? maxSiraNo = _MusteriContext.PotansiyelMusteriNotRepository.All().Where(s => s.PotansiyelMusteriKodu == Not.PotansiyelMusteriKodu).Max(p => (int?)p.SiraNo);

            int siraNo = maxSiraNo.HasValue ? maxSiraNo.Value + 1 : 1;

            Not.SiraNo = siraNo;

            Not = _MusteriContext.PotansiyelMusteriNotRepository.Create(Not);
            _MusteriContext.Commit();

            return Not;
        }
        public bool DeletePotansiyelNot(int potansiyelMusteriKodu, int siraNo)
        {
            _MusteriContext.PotansiyelMusteriNotRepository.Delete(s => s.SiraNo == siraNo && s.PotansiyelMusteriKodu == potansiyelMusteriKodu);
            _MusteriContext.Commit();
            return true;
        }
        public bool UpdatePotansiyelNot(PotansiyelMusteriNot not)
        {
            _MusteriContext.PotansiyelMusteriNotRepository.Update(not);
            _MusteriContext.Commit();
            return true;
        }
        #endregion

        public DataTableList PagedListPotansiyelTelefon(DataTableParameters<PotansiyelMusteriTelefon> telefonList, int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriTelefon> query = _MusteriContext.PotansiyelMusteriTelefonRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.PotansiyelMusteriTelefonRepository.Page(query,
                                                                  telefonList.OrderByProperty,
                                                                  telefonList.IsAscendingOrder,
                                                                  telefonList.Page,
                                                                  telefonList.PageSize, out totalRowCount);
            return telefonList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListPotansiyelAdres(DataTableParameters<PotansiyelMusteriAdre> adresList, int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriAdre> query = _MusteriContext.PotansiyelMusteriAdresRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.PotansiyelMusteriAdresRepository.Page(query,
                                                                  adresList.OrderByProperty,
                                                                  adresList.IsAscendingOrder,
                                                                  adresList.Page,
                                                                  adresList.PageSize, out totalRowCount);
            return adresList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListPotansiyelDokuman(DataTableParameters<PotansiyelMusteriDokuman> dokumanList, int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriDokuman> query = _MusteriContext.PotansiyelMusteriDokumanRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.PotansiyelMusteriDokumanRepository.Page(query,
                                                                  dokumanList.OrderByProperty,
                                                                  dokumanList.IsAscendingOrder,
                                                                  dokumanList.Page,
                                                                  dokumanList.PageSize, out totalRowCount);
            return dokumanList.Prepare(query, totalRowCount);
        }

        public DataTableList PagedListPotansiyelNot(DataTableParameters<PotansiyelMusteriNot> notList, int potansiyelMusteriKodu)
        {
            IQueryable<PotansiyelMusteriNot> query = _MusteriContext.PotansiyelMusteriNotRepository.Filter(s => s.PotansiyelMusteriKodu == potansiyelMusteriKodu);

            int totalRowCount = 0;
            query = _MusteriContext.PotansiyelMusteriNotRepository.Page(query,
                                                                  notList.OrderByProperty,
                                                                  notList.IsAscendingOrder,
                                                                  notList.Page,
                                                                  notList.PageSize, out totalRowCount);
            return notList.Prepare(query, totalRowCount);
        }

        public List<PotansiyelMusteriListeModelOzel> PagedListPotansiyel(PotansiyelMusteriListe arama, out int totalRowCount)
        {

            IAktifKullaniciService aktifServis = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            IQueryable<TVMDetay> tvm = _TVMContext.TVMDetayRepository.All();
            IQueryable<PotansiyelMusteriGenelBilgiler> musteriler;

            // ==== Kullanıcı Neosinerji tvm sine bağlıysa sınırlama yok değilse sınırlı yetki ==== //
            int aktifTVMKodu = aktifServis.TVMKodu;
            if (aktifTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                musteriler = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.All();
            else
                musteriler = _MusteriContext.PotansiyelMusteriGenelBilgilerRepository.Filter(s => s.TVMKodu == aktifTVMKodu);



            CultureInfo culture = new CultureInfo("tr-TR");
            //String olmayan alanlar...
            if (arama.MusteriKodu.HasValue)
                musteriler = musteriler.Where(w => w.PotansiyelMusteriKodu == arama.MusteriKodu.Value);

            if (arama.TVMKodu.HasValue)
                musteriler = musteriler.Where(w => w.TVMKodu == arama.TVMKodu.Value);

            if (arama.MusteriTipKodu.HasValue)
                musteriler = musteriler.Where(w => w.MusteriTipKodu == arama.MusteriTipKodu.Value);

            //sring alanlar...
            if (!String.IsNullOrEmpty(arama.AdiUnvan))
            {
                //string adiUnvani = arama.SoyadiUnvan.ToUpper(culture);
                musteriler = musteriler.Where(w => w.AdiUnvan.StartsWith(arama.AdiUnvan));
            }
            if (!String.IsNullOrEmpty(arama.SoyadiUnvan))
            {
                //string soyadiUnvan = arama.AdiUnvan.ToUpper(culture);
                musteriler = musteriler.Where(w => w.SoyadiUnvan.StartsWith(arama.SoyadiUnvan));
            }
            if (!String.IsNullOrEmpty(arama.EMail))
                musteriler = musteriler.Where(w => w.EMail.StartsWith(arama.EMail));

            if (!String.IsNullOrEmpty(arama.TVMMusteriKodu))
                musteriler = musteriler.Where(w => w.TVMMusteriKodu.StartsWith(arama.TVMMusteriKodu));

            if (!String.IsNullOrEmpty(arama.KimlikNo))
                musteriler = musteriler.Where(w => w.KimlikNo.StartsWith(arama.KimlikNo));

            if (!String.IsNullOrEmpty(arama.PasaportNo))
                musteriler = musteriler.Where(w => w.PasaportNo.StartsWith(arama.PasaportNo));


            IQueryable<PotansiyelMusteriGenelBilgiler> query = from k in musteriler
                                                               join t in tvm on k.TVMKodu equals t.Kodu
                                                               select k;

            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = query.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in musteriler
                        join t in tvm on k.TVMKodu equals t.Kodu
                        select new { k.PotansiyelMusteriKodu, k.TVMMusteriKodu, k.MusteriTipKodu, AdiUnvan = k.AdiUnvan + " " + k.SoyadiUnvan, k.EMail, k.Cinsiyet, k.DogumTarihi, TVMUnvani = t.Unvani })
            .OrderBy(o => o.AdiUnvan)
            .Skip(excludedRows)
            .Take(arama.PageSize)
            .ToList();

            List<PotansiyelMusteriListeModelOzel> listeModel = new List<PotansiyelMusteriListeModelOzel>();

            foreach (var item in list)
            {
                PotansiyelMusteriListeModelOzel model = new PotansiyelMusteriListeModelOzel();

                model.MusteriKodu = item.PotansiyelMusteriKodu;
                model.TVMMusteriKodu = item.TVMMusteriKodu;
                model.MusteriTipKodu = item.MusteriTipKodu;
                model.AdiUnvan = item.AdiUnvan;
                model.EMail = item.EMail;
                model.Cinsiyet = item.Cinsiyet;
                model.DogumTarihi = item.DogumTarihi.HasValue ? item.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";
                model.BagliOlduguTvmText = item.TVMUnvani;

                listeModel.Add(model);
            }

            return listeModel;
        }

    }
}
