using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database;
using Microsoft.Practices.Unity;
using System.Web.Mvc;
using System.Net.Mail;
using System.Globalization;


namespace Neosinerji.BABOnlineTP.Business
{
    public class MusteriADL
    {
        IAktifKullaniciService _AktifKullanici;
        [Dependency]
        ITVMContext _TvmContext { get; set; }
        [Dependency]
        IMusteriContext _MusteriContext { get; set; }
        [Dependency]
        IParametreContext _ParameterContext { get; set; }
        [Dependency]
        ITanimService _TanimService { get; set; }

        public MusteriADL(IAktifKullaniciService aktifKullanici)
        {
            _AktifKullanici = aktifKullanici;
        }

        #region Conts
        private const string MusteriTipiGercek = "GERÇEK";
        private const string MusteriTipiFirma = "FİRMA";
        private const string MusteriTipiTuzel = "TÜZEL";
        private const string MusteriTipiYabanci = "YABANCI";


        private const string NumaraTipi_Is = "İŞ";
        private const string NumaraTipi_Cep = "CEP";
        private const string NumaraTipi_Ev = "EV";
        private const string NumaraTipi_Fax = "FAX";
        private const string NumaraTipi_Diğer = "DİĞER";


        private const string EgitimDurumu_Ilkogretim = "İLKÖĞRETİM";
        private const string EgitimDurumu_Lise = "LİSE";
        private const string EgitimDurumu_OnLisans = "ÖN LİSANS";
        private const string EgitimDurumu_Lisans = "LİSANS";
        private const string EgitimDurumu_YuksekLisans = "YÜKSEK LİSANS";
        private const string EgitimDurumu_Doktora = "DOKTORA";
        private const string EgitimDurumu_Ogrenci = "ÖĞRENCİ";


        private const string MedeniDurum_Evli = "EVLİ";
        private const string MedeniDurum_Bekar = "BEKAR";
        private const string MedeniDurum_Dul = "DUL";
        private const string MedeniDurum_Bosanmis = "BOŞANMIŞ";

        private const string Cinsiyet_Erkek = "E";
        private const string Cinsiyet_Kadın = "K";

        private const string AdresTipi_Ev = "EV";
        private const string AdresTipi_Is = "İŞ";
        private const string AdresTipi_Diger = "DİĞER";

        List<Ulke> ulkeler;
        List<Il> iller;
        List<Ilce> ilceler;
        #endregion

        public bool MusterileriKaydet(string dosyaAdresi)
        {
            ExcelMusteriListModel model = ProcessFile(dosyaAdresi);

            foreach (var item in model.HatasizKayitlar)
            {
                MusteriGenelBilgiler musteri = new MusteriGenelBilgiler();

                #region Musteri Genel Bilgliri ekleniyor.

                if (item.MusteriTipi == MusteriTipiGercek)
                    musteri.MusteriTipKodu = 1;
                if (item.MusteriTipi == MusteriTipiTuzel)
                    musteri.MusteriTipKodu = 2;
                if (item.MusteriTipi == MusteriTipiFirma)
                    musteri.MusteriTipKodu = 3;
                if (item.MusteriTipi == MusteriTipiYabanci)
                    musteri.MusteriTipKodu = 4;

                musteri.TVMKodu = _AktifKullanici.TVMKodu;
                musteri.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;
                musteri.TVMMusteriKodu = item.TVMMusteriKodu;
                musteri.KimlikNo = item.KimlikNo;

                if (item.AdiUnvan.Length > 49)
                    item.AdiUnvan = item.AdiUnvan.Substring(0, 49);
                musteri.AdiUnvan = item.AdiUnvan;
                if (!String.IsNullOrEmpty(item.SoyadiUnvan))
                    musteri.SoyadiUnvan = item.SoyadiUnvan;
                else
                    musteri.SoyadiUnvan = ".";

                if (!String.IsNullOrEmpty(item.EMail))
                    musteri.EMail = item.EMail;

                musteri.WebUrl = item.WebUrl;
                musteri.KayitTarihi = TurkeyDateTime.Now;

                if (item.MusteriTipi == MusteriTipiGercek)
                {
                    musteri.Uyruk = 0;
                }

                else if (item.MusteriTipi == MusteriTipiFirma || item.MusteriTipi == MusteriTipiTuzel)
                {
                    if (!String.IsNullOrEmpty(item.VergiDairesi))
                        musteri.VergiDairesi = item.VergiDairesi;

                    if (!String.IsNullOrEmpty(item.Uyruk))
                        musteri.Uyruk = (short)(item.Uyruk == "TC" ? 0 : 1);

                    //Müşteri tipi tüzel olanlara ozel eklentiler
                    //if (item.MusteriTipi == MusteriTipiTuzel)
                    //{
                    //    //Ana ve alt sektor kaydediliyor
                    //    if (!String.IsNullOrEmpty(item.FaaliyetAnaSektor) && !String.IsNullOrEmpty(item.FaaliyetAltSektor))
                    //    {
                    //        var sonuc = Faaliyet_AnaAlt_SektorGetir(item.FaaliyetAnaSektor, item.FaaliyetAltSektor);
                    //        musteri.FaaliyetGosterdigiAnaSektor = sonuc.Item1;
                    //        musteri.FaaliyetGosterdigiAltSektor = sonuc.Item2;
                    //    }

                    //    //Faaliyet olcegi kaydediliyor
                    //    if (!String.IsNullOrEmpty(item.FaaliyetOlcegi))
                    //        musteri.FaaliyetOlcegi_ = FaaliyetOlcegiGetir(item.FaaliyetOlcegi);

                    //    //Sabit Varlık Bİlgisi Ekleniyor
                    //    if (!String.IsNullOrEmpty(item.SabitVarlikBilgisi))
                    //        musteri.SabitVarlikBilgisi = SabitVarlikBilgisiGetir(item.SabitVarlikBilgisi);

                    //    //Ciro bilgisi ekleniyor
                    //    if (!String.IsNullOrEmpty(item.CiroBilgisi))
                    //        musteri.CiroBilgisi = CiroBilgisiGetir(item.CiroBilgisi);
                    //}
                }

                else if (item.MusteriTipi == MusteriTipiYabanci)
                {
                    if (!String.IsNullOrEmpty(item.PasaportNo))
                    {
                        musteri.PasaportNo = item.PasaportNo;
                        double d = double.Parse(item.PasaportGecerlilikTarihi);
                        musteri.PasaportGecerlilikBitisTarihi = DateTime.FromOADate(d);
                    }
                    musteri.Uyruk = 1;
                }

                if (item.MusteriTipi == MusteriTipiGercek || item.MusteriTipi == MusteriTipiYabanci)
                {
                    if (!String.IsNullOrEmpty(item.Cinsiyet))
                        musteri.Cinsiyet = item.Cinsiyet;

                    if (!String.IsNullOrEmpty(item.DogumTarihi))
                    {
                        DateTime dt = DateTime.MinValue;
                        if (DateTime.TryParseExact(item.DogumTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                            musteri.DogumTarihi = dt;
                        else
                            musteri.DogumTarihi = dt;
                    }

                    if (!String.IsNullOrEmpty(item.EgitimDurumu))
                    {
                        short kodu = (short)EgitimDurumuGetir(item.EgitimDurumu);
                        if (kodu > 0)
                            musteri.EgitimDurumu = kodu;
                    }


                    if (!String.IsNullOrEmpty(item.Meslek))
                    {
                        int kodu = MeslekKoduGetir(item.Meslek);
                        if (kodu > 0)
                            musteri.MeslekKodu = kodu;
                    }

                    if (!String.IsNullOrEmpty(item.MedeniDurumu))
                    {
                        byte kodu = (byte)MedeniDurumKoduGetir(item.MedeniDurumu);
                        if (kodu > 0)
                            musteri.MedeniDurumu = kodu;
                    }

                }
                #endregion

                #region Adres Bilgileri Ekleniyor.


                MusteriAdre adres = new MusteriAdre();
                adres.SiraNo = 0;
                adres.Varsayilan = true;

                if (!String.IsNullOrEmpty(item.AdresTipi))
                    adres.AdresTipi = AdresTipiKoduGetir(item.AdresTipi);
                else
                    adres.AdresTipi = 10;

                if (!String.IsNullOrEmpty(item.UlkeKodu))
                    adres.UlkeKodu = UlkeKoduGetir(item.UlkeKodu);
                else
                    adres.UlkeKodu = "000";

                if (!String.IsNullOrEmpty(item.IlKodu))
                    adres.IlKodu = IlKoduGetri(item.IlKodu);
                else
                    adres.IlKodu = "999";

                if (!String.IsNullOrEmpty(item.IlceKodu))
                    adres.IlceKodu = IlceKoduGetir(item.IlceKodu);
                else
                    adres.IlceKodu = 977;

                if (!String.IsNullOrEmpty(item.Adres))
                    adres.Adres = item.Adres;
                else
                    adres.Adres = "DİĞER";

                if (!String.IsNullOrEmpty(item.Semt))
                    adres.Semt = item.Semt;

                if (!String.IsNullOrEmpty(item.Mahalle))
                    adres.Mahalle = item.Mahalle;
                else
                    adres.Mahalle = ".";

                if (!String.IsNullOrEmpty(item.Cadde))
                    adres.Cadde = item.Cadde;
                else
                    adres.Cadde = ".";

                if (!String.IsNullOrEmpty(item.Sokak))
                    adres.Sokak = item.Sokak;
                else
                    adres.Sokak = ".";

                if (!String.IsNullOrEmpty(item.Apartman))
                    adres.Apartman = item.Apartman;
                else
                    adres.Apartman = ".";

                if (!String.IsNullOrEmpty(item.BinaNo))
                    adres.BinaNo = item.BinaNo;
                else
                    adres.BinaNo = ".";

                if (!String.IsNullOrEmpty(item.DaireNo))
                    adres.DaireNo = item.DaireNo;
                else
                    adres.DaireNo = ".";

                if (!String.IsNullOrEmpty(item.HanAptFab))
                    adres.HanAptFab = item.HanAptFab;

                int postakod;

                if (!String.IsNullOrEmpty(item.PostaKodu))
                {
                    if (int.TryParse(item.PostaKodu, out postakod))
                        adres.PostaKodu = postakod;
                    else
                        adres.PostaKodu = 0;
                }
                else
                    adres.PostaKodu = 00000;

                if (!String.IsNullOrEmpty(item.Diger))
                    adres.Diger = item.Diger;



                #endregion

                #region Telefon Bilgileri Ekleniyor

                MusteriTelefon telefon = new MusteriTelefon();

                telefon.SiraNo = 0;

                if (!String.IsNullOrEmpty(item.IletisimNumaraTipi))
                    telefon.IletisimNumaraTipi = (short)IletisimNumaraTipiGetir(item.IletisimNumaraTipi);
                else
                    telefon.IletisimNumaraTipi = 15;

                if (!String.IsNullOrEmpty(item.Numara) && !String.IsNullOrEmpty(item.AlanKodu))
                    telefon.Numara = item.TelefonUlkeKodu + "" + item.AlanKodu + "" + item.Numara;
                else
                    telefon.Numara = "90-111-1111111";

                if (!String.IsNullOrEmpty(item.NumaraSahibi))
                    telefon.NumaraSahibi = item.NumaraSahibi;


                musteri.MusteriTelefons.Add(telefon);


                if (!String.IsNullOrEmpty(item.IletisimNumaraTipi_2) && !String.IsNullOrEmpty(item.TelefonUlkeKodu_2)
                && !String.IsNullOrEmpty(item.AlanKodu_2) && !String.IsNullOrEmpty(item.Numara_2))
                {
                    MusteriTelefon tel2 = new MusteriTelefon();
                    tel2.SiraNo = 1;
                    tel2.IletisimNumaraTipi = (short)IletisimNumaraTipiGetir(item.IletisimNumaraTipi_2);
                    tel2.Numara = item.TelefonUlkeKodu_2 + "" + item.AlanKodu_2 + "" + item.Numara_2;

                    musteri.MusteriTelefons.Add(tel2);
                }


                if (!String.IsNullOrEmpty(item.IletisimNumaraTipi_3) && !String.IsNullOrEmpty(item.TelefonUlkeKodu_3)
                && !String.IsNullOrEmpty(item.AlanKodu_3) && !String.IsNullOrEmpty(item.Numara_3))
                {
                    MusteriTelefon tel3 = new MusteriTelefon();
                    tel3.SiraNo = 2;
                    tel3.IletisimNumaraTipi = (short)IletisimNumaraTipiGetir(item.IletisimNumaraTipi_3);
                    tel3.Numara = item.TelefonUlkeKodu_3 + "" + item.AlanKodu_3 + "" + item.Numara_3;

                    musteri.MusteriTelefons.Add(tel3);
                }


                #endregion
                var kod = musteri.TVMKodu;
                var perkod = musteri.TVMKullaniciKodu;

                MusteriNot musteriNot = new MusteriNot();
                musteriNot.Konu = "";
                musteriNot.KayitTarihi = TurkeyDateTime.Now;
                musteriNot.TVMKodu = kod;
                musteriNot.TVMPersonelKodu = perkod;





                if (!String.IsNullOrEmpty(item.Not))
                {

                    musteriNot.NotAciklamasi = item.Not;
                }
                else
                {
                    musteriNot.NotAciklamasi = "";
                }


                //_MusteriContext.MusteriNotRepository.Create(musteriNot)

                musteri.MusteriAdres.Add(adres);
                musteri.MusteriNots.Add(musteriNot);

                _MusteriContext.MusteriGenelBilgilerRepository.Create(musteri);


            }
            _MusteriContext.Commit();
            return true;
        }

        private DataTable GetTable()
        {
            DataTable dt = new DataTable();

            DataColumn[] Coll = new DataColumn[45];

            Coll[0] = new DataColumn("Müşteri Tipi *", typeof(string));
            Coll[1] = new DataColumn("TVM Müşteri Kodu", typeof(string));
            Coll[2] = new DataColumn("Kimlik No *", typeof(string));
            Coll[3] = new DataColumn("Pasaport No", typeof(string));
            Coll[4] = new DataColumn("Pasaport Geçerlilik Bitiş Tarihi", typeof(string));
            Coll[5] = new DataColumn("Vergi Dairesi", typeof(string));
            Coll[6] = new DataColumn("Adı / Ünvanı *", typeof(string));
            Coll[7] = new DataColumn("Soyadı / Ünvanı *", typeof(string));
            Coll[8] = new DataColumn("Cinsiyet", typeof(string));
            Coll[9] = new DataColumn("Doğum Tarihi", typeof(string));
            Coll[10] = new DataColumn("Email *", typeof(string));
            Coll[11] = new DataColumn("Web URL", typeof(string));
            Coll[12] = new DataColumn("Uyruk *", typeof(string));
            Coll[13] = new DataColumn("Eğitim Durumu", typeof(string));

            Coll[14] = new DataColumn("Meslek", typeof(string));
            Coll[15] = new DataColumn("Medeni Durumu", typeof(string));
            Coll[16] = new DataColumn("Adres Tipi *", typeof(string));
            Coll[17] = new DataColumn("Ulke *", typeof(string));
            Coll[18] = new DataColumn("İl*", typeof(string));
            Coll[19] = new DataColumn("İlçe *", typeof(string));
            Coll[20] = new DataColumn("Adres *", typeof(string));
            Coll[21] = new DataColumn("Semt", typeof(string));
            Coll[22] = new DataColumn("Mahalle *", typeof(string));
            Coll[23] = new DataColumn("Cadde *", typeof(string));
            Coll[24] = new DataColumn("Sokak *", typeof(string));
            Coll[25] = new DataColumn("Apartman *", typeof(string));
            Coll[26] = new DataColumn("Bina No *", typeof(string));


            Coll[27] = new DataColumn("Daire No *", typeof(string));
            Coll[28] = new DataColumn("Han/Apt/Fab", typeof(string));
            Coll[29] = new DataColumn("Posta Kodu *", typeof(string));
            Coll[30] = new DataColumn("Diğer", typeof(string));
            Coll[31] = new DataColumn("Numara Tipi", typeof(string));
            Coll[32] = new DataColumn("Ülke Kodu *", typeof(string));
            Coll[33] = new DataColumn("Alan Kodu *", typeof(string));
            Coll[34] = new DataColumn("Numara *", typeof(string));
            Coll[35] = new DataColumn("Numara Sahibi *", typeof(string));


            Coll[36] = new DataColumn("Numara Tipi_2", typeof(string));
            Coll[37] = new DataColumn("Ülke Kodu_2", typeof(string));
            Coll[38] = new DataColumn("Alan Kodu_2", typeof(string));
            Coll[39] = new DataColumn("Numara_2", typeof(string));

            Coll[40] = new DataColumn("Numara Tipi_3", typeof(string));
            Coll[41] = new DataColumn("Ülke Kodu_3", typeof(string));
            Coll[42] = new DataColumn("Alan Kodu_3", typeof(string));
            Coll[43] = new DataColumn("Numara_3", typeof(string));
            Coll[44] = new DataColumn("Not", typeof(string));

            dt.Columns.AddRange(Coll);

            return dt;
        }

        public ExcelMusteriListModel ProcessFile(string mFilePath)
        {
            XL2007 xl = null;

            xl = new XL2007(mFilePath, 2);
            ExcelMusteriListModel model = new ExcelMusteriListModel();

            if (xl.Open())
            {
                //Ulkeler ve iller getiriliyor.
                ulkeler = new List<Ulke>();
                iller = new List<Il>();
                ilceler = new List<Ilce>();

                _ParameterContext = DependencyResolver.Current.GetService<IParametreContext>();
                _TvmContext = DependencyResolver.Current.GetService<ITVMContext>();
                _MusteriContext = DependencyResolver.Current.GetService<IMusteriContext>();
                _TanimService = DependencyResolver.Current.GetService<ITanimService>();
                ulkeler = _ParameterContext.UlkeRepository.All().ToList<Ulke>();
                iller = _ParameterContext.IlRepository.All().ToList<Il>();
                ilceler = _ParameterContext.IlceRepository.All().ToList<Ilce>();

                DataTable dt = GetTable();

                model.HataliKayitlar = new List<ExcelMusteriModel>();
                model.HatasizKayitlar = new List<ExcelMusteriModel>();
                xl.Fill(dt);
                xl.Dispose();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        ExcelMusteriModel mdl = new ExcelMusteriModel();

                        mdl.MusteriTipi = item[0].ToString();
                        mdl.TVMMusteriKodu = item[1].ToString();
                        mdl.KimlikNo = item[2].ToString();
                        mdl.PasaportNo = item[3].ToString();
                        mdl.PasaportGecerlilikTarihi = item[4].ToString();
                        mdl.VergiDairesi = item[5].ToString();
                        mdl.AdiUnvan = item[6].ToString();
                        mdl.SoyadiUnvan = item[7].ToString();
                        mdl.Cinsiyet = item[8].ToString();
                        mdl.DogumTarihi = item[9].ToString();
                        mdl.EMail = item[10].ToString();
                        mdl.WebUrl = item[11].ToString();
                        mdl.Uyruk = item[12].ToString();
                        mdl.EgitimDurumu = item[13].ToString();
                        mdl.Meslek = item[14].ToString();
                        mdl.MedeniDurumu = item[15].ToString();
                        mdl.AdresTipi = item[16].ToString();
                        mdl.UlkeKodu = item[17].ToString();
                        mdl.IlKodu = item[18].ToString();
                        mdl.IlceKodu = item[19].ToString();
                        mdl.Adres = item[20].ToString();
                        mdl.Semt = item[21].ToString();
                        mdl.Mahalle = item[22].ToString();
                        mdl.Cadde = item[23].ToString();
                        mdl.Sokak = item[24].ToString();
                        mdl.Apartman = item[25].ToString();
                        mdl.BinaNo = item[26].ToString();
                        mdl.DaireNo = item[27].ToString();
                        mdl.HanAptFab = item[28].ToString();
                        mdl.PostaKodu = item[29].ToString();
                        mdl.Diger = item[30].ToString();
                        mdl.IletisimNumaraTipi = item[31].ToString();
                        mdl.TelefonUlkeKodu = item[32].ToString();
                        mdl.AlanKodu = item[33].ToString();
                        mdl.Numara = item[34].ToString();
                        mdl.NumaraSahibi = item[35].ToString();

                        mdl.IletisimNumaraTipi_2 = item[36].ToString();
                        mdl.TelefonUlkeKodu_2 = item[37].ToString();
                        mdl.AlanKodu_2 = item[38].ToString();
                        mdl.Numara_2 = item[39].ToString();

                        mdl.IletisimNumaraTipi_3 = item[40].ToString();
                        mdl.TelefonUlkeKodu_3 = item[41].ToString();
                        mdl.AlanKodu_3 = item[42].ToString();
                        mdl.Numara_3 = item[43].ToString();
                        mdl.Not = item[44].ToString();


                        mdl = ModelKontrol(mdl);

                        if (mdl.HataMesaj == "")
                        {
                            int sayac = 0;
                            foreach (DataRow item2 in dt.Rows)
                            {
                                if (item2[2].ToString() == mdl.KimlikNo)
                                    sayac++;
                            }
                            if (sayac == 1)
                                model.HatasizKayitlar.Add(mdl);
                            else
                            {
                                mdl.HataMesaj += "Bu kimlik numaralı kayıt tekrarlı.";
                                mdl.hatalist = mdl.HataMesaj.Split('.');
                                model.HataliKayitlar.Add(mdl);
                            }
                        }
                        else
                        {
                            mdl.hatalist = mdl.HataMesaj.Split('.');
                            model.HataliKayitlar.Add(mdl);
                        }
                    }
                }
            }
            return model;
        }

        private ExcelMusteriModel ModelKontrol(ExcelMusteriModel model)
        {
            if (model != null)
            {
                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMKullaniciKodu = _AktifKullanici.KullaniciKodu;

                //MusteriTİpi kontrol ediliyor.
                model.HataMesaj = MusteriTipiKontrol(model.MusteriTipi);

                //Müşteri Tipi Hatalı girilmişse diğer kontroller yapılmıyor (çünkü tüm kontroller müşteri tipine göre yapılmalı)
                if (model.HataMesaj != "")
                    return model;

                //Adi Unvanı kontrol
                //model.HataMesaj += AdiUnvaniKontrol(model.AdiUnvan, model.SoyadiUnvan);

                if (model.MusteriTipi == MusteriTipiGercek || model.MusteriTipi == MusteriTipiYabanci)
                {
                    //kimlik numarası kontrol ediliyor.
                    model.HataMesaj += KimlikNoKontrol(model.KimlikNo, model.MusteriTipi, model);

                    //Doğum Tarihi kontrol Ediliyor.
                    model.HataMesaj += DogumTarihiKontrol(model.DogumTarihi);

                    //Eğitim Durumu Kontrol
                    model.HataMesaj += EgitimDurumuKontrol(model.EgitimDurumu);
                    //Medeni durum
                    model.HataMesaj += MedeniDurumKontrol(model.MedeniDurumu);
                    //Cinsiyet Alanı kontrol ediliyor.
                    //model.HataMesaj += CinsiyetKontrol(model.Cinsiyet);

                }
                else if (model.MusteriTipi == MusteriTipiTuzel || model.MusteriTipi == MusteriTipiFirma)
                {
                    //kimlik numarası kontrol ediliyor.
                    model.HataMesaj += KimlikNoKontrol(model.KimlikNo, model.MusteriTipi, model);

                    //if (model.MusteriTipi == MusteriTipiTuzel)
                    //{
                    //    //Faaliyet ana ve alt sektor kontrol
                    //    model.HataMesaj += Faaliyet_AnaAlt_Sektor(model.FaaliyetAnaSektor, model.FaaliyetAltSektor);

                    //    //Faaliyet ölçeği
                    //    model.HataMesaj += FaaliyetOlcegi(model.FaaliyetOlcegi);

                    //    //Sabit Varlık Bilgisi
                    //    model.HataMesaj += SabitVarlikBilgisi(model.SabitVarlikBilgisi);

                    //    //Ciro Bilgisi
                    //    model.HataMesaj += CiroBilgisi(model.CiroBilgisi);
                    //}
                }
                //---------------------------ORTAK KONTROLLER------------------------------------

                model.HataMesaj += UlkeIlIlceKontrol(model.UlkeKodu, model.IlKodu, model.IlceKodu);
                model.HataMesaj += PostaKoduKontrol(model.PostaKodu);
                model.HataMesaj += NumaraTipiKontrol(model.IletisimNumaraTipi);
                model.HataMesaj += AlanKoduKntrol(model.AlanKodu);
                model.HataMesaj += NumaraKontrol(model.Numara);
                model.HataMesaj += AdresKontrol(model.Adres, model.Mahalle, model.Cadde, /*model.Sokak,*/ model.Apartman, model.BinaNo, model.DaireNo);

                if (!EmailKontrol(model.EMail))
                    model.HataMesaj += "Email adresi hatalı formatta.";

            }
            return model;
        }

        #region Kontrol Metodları
        private string MusteriTipiKontrol(string musteriTipi)
        {
            //musteriTipi = musteriTipi.ToLower();

            if (musteriTipi == MusteriTipiGercek || musteriTipi == MusteriTipiTuzel || musteriTipi == MusteriTipiFirma || musteriTipi == MusteriTipiYabanci)
                return "";
            else
                return "Müşteri Tipi Hatalı.";
        }
        private string KimlikNoKontrol(string kimlikNo, string musteriTipi, ExcelMusteriModel mdl = null)
        {
            int TVMKodu = _AktifKullanici.TVMKodu;
            MusteriGenelBilgiler mstr = _MusteriContext.MusteriGenelBilgilerRepository.Find(s => s.KimlikNo == kimlikNo && s.TVMKodu == TVMKodu);

            if (kimlikNo.Length == 11 && (musteriTipi == MusteriTipiGercek || musteriTipi == MusteriTipiYabanci))
            {
                if (mstr == null)
                {
                    foreach (var item in kimlikNo)
                    {
                        if (!Char.IsDigit(item)) return "Kimlik Numarası rakamlardan oluşmalıdır.";
                    }
                    return "";
                }
                else
                {
                    foreach (var item in mstr.MusteriTelefons)
                    {
                        #region Telefon Bilgileri Ekleniyor
                        MusteriTelefon musteriTelefon = item;
                        if (musteriTelefon.IletisimNumaraTipi == (short)IletisimNumaraTipiGetir(mdl.IletisimNumaraTipi))
                        {
                            if (!String.IsNullOrEmpty(mdl.Numara) && !String.IsNullOrEmpty(mdl.AlanKodu))
                                musteriTelefon.Numara = mdl.TelefonUlkeKodu + "" + mdl.AlanKodu + "" + mdl.Numara;

                            if (!String.IsNullOrEmpty(musteriTelefon.NumaraSahibi))
                                musteriTelefon.NumaraSahibi = mdl.NumaraSahibi;

                            _MusteriContext.MusteriTelefonRepository.Update(musteriTelefon);
                            _MusteriContext.Commit();
                            break;
                        }


                        #endregion

                    }
                    return "Bu müşteri daha önce kaydedilmiş.";
                }

            }
            else if (kimlikNo.Length == 10 && (musteriTipi == MusteriTipiTuzel || musteriTipi == MusteriTipiFirma))
            {
                if (mstr == null)
                {
                    foreach (var item in kimlikNo)
                    {
                        if (!Char.IsDigit(item)) return "Kimlik Numarası rakamlardan oluşmalıdır.";
                    }
                    return "";
                }
                else return "Bu müşteri daha önce kaydedilmiş.";
            }
            else
                return "Kimlik Numarası hatalı.";
        }
        //private string AdiUnvaniKontrol(string ad, string soyad)
        //{
        //    if (ad.Length > 0 && soyad.Length > 0)
        //    {
        //        //foreach (var item in ad)
        //        //{
        //        //    if (!Char.IsLetter(item)) return "Ad alanında hatalı veri girilmiş.";
        //        //}
        //        //foreach (var item in soyad)
        //        //{
        //        //    if (!Char.IsLetter(item)) return "Soyad alanında hatalı veri girilmiş.";
        //        //}
        //        return "";
        //    }
        //    else
        //        return "Ad ve soyad boş geçilemez.";
        //}
        //private string CinsiyetKontrol(string cinsiyet)
        //{
        //    if (cinsiyet.Length > 0 && (cinsiyet == Cinsiyet_Erkek || cinsiyet == Cinsiyet_Kadın)) return "";
        //    else if (cinsiyet.Length == 0) return "Cinsiyet girmelisiniz.";
        //    else return "Cinsiyet hatalı";
        //}
        private bool EmailKontrol(string email)
        {
            try
            {
                MailAddress mail = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private string DogumTarihiKontrol(string dogumTarihi)
        {
            if (!String.IsNullOrEmpty(dogumTarihi))
                return "";
            else return "Lütfen doğum tarihi giriniz.";
        }
        private string MeslekKontrol(string meslek)
        {
            if (!String.IsNullOrEmpty(meslek))
            {
                Meslek mslk = _ParameterContext.MeslekRepository.Find(s => s.MeslekAdi == meslek);
                if (mslk == null)
                    return "Meslek hatalı.";
            }
            return "";
        }
        private string EgitimDurumuKontrol(string egitimDurumu)
        {
            if (String.IsNullOrEmpty(egitimDurumu))
                return "";
            else if (egitimDurumu == EgitimDurumu_Ilkogretim || egitimDurumu == EgitimDurumu_Lise || egitimDurumu == EgitimDurumu_OnLisans ||
                     egitimDurumu == EgitimDurumu_Lisans || egitimDurumu == EgitimDurumu_Doktora || egitimDurumu == EgitimDurumu_Ogrenci ||
                     egitimDurumu == EgitimDurumu_YuksekLisans)
            {
                return "";
            }
            else
                return "Eğitim durumu hatalı";

        }
        private string MedeniDurumKontrol(string medeniDurum)
        {
            if (!String.IsNullOrEmpty(medeniDurum))
            {
                if (medeniDurum == MedeniDurum_Evli || medeniDurum == MedeniDurum_Bekar || medeniDurum == MedeniDurum_Dul || medeniDurum == MedeniDurum_Bosanmis)
                    return "";
                else return "Medeni durum hatalı.";
            }
            else return "";

        }
        private string UlkeIlIlceKontrol(string ulke, string il, string ilce)
        {
            if (String.IsNullOrEmpty(ulke)) return "";
            else if (String.IsNullOrEmpty(il)) return "";
            else if (String.IsNullOrEmpty(ilce)) return "";
            else
            {
                //ulke = ulke.ToUpper();
                //il = il.Replace('İ', 'I');
                ilce = ilce.Replace('i', 'İ').ToUpper();
                Ulke uke = ulkeler.Find(s => s.UlkeAdi == ulke);
                if (ulke == null) return "Ülke hatalı.";
                else
                {
                    Il Il = iller.Find(s => s.IlAdi == il && s.UlkeKodu == uke.UlkeKodu);
                    if (Il == null) return "il hatalı.";
                    else
                    {
                        Ilce Ilce = ilceler.Find(s => s.IlceAdi == ilce && s.IlKodu == Il.IlKodu);
                        if (Ilce == null) return "ilçe hatalı.";
                    }
                }
                if (ulke != null && ilce != null && il != null) return "";
                else return "Hatalı adres.";
            }
        }
        private string PostaKoduKontrol(string postaKodu)
        {
            if (String.IsNullOrEmpty(postaKodu)) return "Posta kodu girilmeli.";
            else if (postaKodu.Length == 5)
            {
                foreach (var item in postaKodu)
                {
                    if (!Char.IsDigit(item)) return "Posta kodu rakamlardan oluşmalı.";
                }
                return "";
            }
            else return "Posta kodu 5 rakamdan oluşmalı.";
        }
        private string NumaraTipiKontrol(string numaraTipi)
        {
            if (String.IsNullOrEmpty(numaraTipi)) return "";//return "Numara tipi belirtiniz.";
            // numaraTipi = numaraTipi.ToLower();

            if (numaraTipi == NumaraTipi_Cep || numaraTipi == NumaraTipi_Diğer ||
                     numaraTipi == NumaraTipi_Ev || numaraTipi == NumaraTipi_Fax || numaraTipi == NumaraTipi_Is) return "";
            else
                return "Numara tipi hatalı.";
        }
        private string AlanKoduKntrol(string numaraalankodu)
        {
            if (String.IsNullOrEmpty(numaraalankodu)) return ""; // return "Alan Kodu girmelisiniz.";
            else if (numaraalankodu.Length == 3) return "";
            else return "Alan Kodu hatalı. Alan Kodu formatı  212 veya 532  şeklinde olmalı";
        }
        private string NumaraKontrol(string numara)
        {
            if (String.IsNullOrEmpty(numara)) return "";// return "Numara girmelisiniz.";
            else if (numara.Length == 7) return "";
            else return "Numara hatalı. Numara formatı  4447474  şeklinde olmalı";
        }
        private string AdresKontrol(string adres, string mahalle, string cadde, /*string sokak,*/ string apartman, string binano, string daireno)
        {
            if (String.IsNullOrEmpty(adres)) return "Adres girmelisiniz";
            if (String.IsNullOrEmpty(mahalle)) return "Mahalle girmelisiniz.";
            if (String.IsNullOrEmpty(cadde)) return "Cadde girmelisiniz.";
            //if (String.IsNullOrEmpty(sokak)) return "Sokak girmelisiniz.";
            if (String.IsNullOrEmpty(apartman)) return "Apartman girmelisiniz.";
            if (String.IsNullOrEmpty(binano)) return "Bina no girmelisiniz.";
            if (String.IsNullOrEmpty(daireno)) return "Daire no girmelisiniz.";
            else return "";
        }

        //Tüzel kişi için
        //private string Faaliyet_AnaAlt_Sektor(string anasektor, string altsektor)
        //{
        //    List<GenelTanimlar> anasektorler = _TanimService.GetListTanimlar("FaaliyetAnaSektor");
        //    if (!String.IsNullOrEmpty(anasektor))
        //    {
        //        int sayac = 0;
        //        foreach (var ana in anasektorler)
        //        {
        //            if (ana.Aciklama == anasektor)
        //            {
        //                if (!String.IsNullOrEmpty(altsektor))
        //                {
        //                    List<GenelTanimlar> altsektorler = _TanimService.GetListAltSektor(ana.TanimId);
        //                    int altsayac = 0;
        //                    foreach (var alt in altsektorler)
        //                        if (alt.Aciklama == altsektor)
        //                            altsayac++;

        //                    if (altsayac != 0) return "";
        //                    else return "Alt sektör hatalı.";
        //                }
        //                else return "Lütfen alt sektör giriniz.";
        //            }
        //        }
        //        if (sayac != 0)
        //            return "";
        //        else
        //            return "Ana sektör hatalı.";
        //    }
        //    else return "Lütfen ana sektör giriniz.";
        //}
        //private string FaaliyetOlcegi(string faaliyetolcegi)
        //{
        //    if (!String.IsNullOrEmpty(faaliyetolcegi))
        //    {
        //        List<GenelTanimlar> faliyetOlcekleri = _TanimService.GetListTanimlar("FaaliyetOlcegi");
        //        int sayac = 0;
        //        foreach (var item in faliyetOlcekleri)
        //        {
        //            if (item.Aciklama == faaliyetolcegi)
        //                sayac++;
        //        }
        //        if (sayac != 0) return "";
        //        else return "Faaliyet ölçeği hatalı.";
        //    }
        //    else return "Lütfen faaliyet ölçeği giriniz.";
        //}
        //private string SabitVarlikBilgisi(string sabitvarlik)
        //{
        //    if (!String.IsNullOrEmpty(sabitvarlik))
        //    {
        //        int sayac = 0;
        //        List<GenelTanimlar> sabitvarliklar = _TanimService.GetListTanimlar("SabitVarlik");
        //        foreach (var item in sabitvarliklar)
        //        {
        //            if (item.Aciklama == sabitvarlik)
        //                sayac++;
        //        }
        //        if (sayac != 0) return "";
        //        else return "Sabit varlık hatalı.";
        //    }
        //    else return "Lütfen sabit varlık giriniz.";
        //}
        //private string CiroBilgisi(string cirobilgisi)
        //{
        //    if (!String.IsNullOrEmpty(cirobilgisi))
        //    {
        //        int sayac = 0;
        //        List<GenelTanimlar> cirobilgileri = _TanimService.GetListTanimlar("CiroBilgisi");
        //        foreach (var item in cirobilgileri)
        //        {
        //            if (item.Aciklama == cirobilgisi)
        //                sayac++;
        //        }
        //        if (sayac != 0) return "";
        //        else return "Ciro bilgisi hatalı";
        //    }
        //    else return "Lütfen ciro bilgisi giriniz.";
        //}
        #endregion

        #region Eklenecek bilgilerin Databasedeki kodları getiriliyor

        private int EgitimDurumuGetir(string egitimDurumuText)
        {
            if (egitimDurumuText == EgitimDurumu_Ilkogretim) return 1;
            else if (egitimDurumuText == EgitimDurumu_Lise) return 2;
            else if (egitimDurumuText == EgitimDurumu_OnLisans) return 3;
            else if (egitimDurumuText == EgitimDurumu_Lisans) return 4;
            else if (egitimDurumuText == EgitimDurumu_YuksekLisans) return 5;
            else if (egitimDurumuText == EgitimDurumu_Doktora) return 6;
            else if (egitimDurumuText == EgitimDurumu_Ogrenci) return 7;
            else return 0;
        }
        private int MeslekKoduGetir(string meslekText)
        {
            if (!String.IsNullOrEmpty(meslekText))
            {
                int meslekKodu = _ParameterContext.MeslekRepository.Find(s => s.MeslekAdi == meslekText).MeslekKodu;
                return meslekKodu;
            }
            else
                return 0;
        }
        private int MedeniDurumKoduGetir(string medeniDurumText)
        {
            if (medeniDurumText == MedeniDurum_Evli) return 1;
            else if (medeniDurumText == MedeniDurum_Bekar) return 2;
            else if (medeniDurumText == MedeniDurum_Dul) return 3;
            else if (medeniDurumText == MedeniDurum_Bosanmis) return 4;
            else return 0;
        }
        private int AdresTipiKoduGetir(string adresTipiText)
        {
            if (adresTipiText == AdresTipi_Ev) return 8;
            else if (adresTipiText == AdresTipi_Is) return 9;
            else if (adresTipiText == AdresTipi_Diger) return 10;
            else return 0;
        }
        private string UlkeKoduGetir(string ulkeAdi)
        {
            if (!String.IsNullOrEmpty(ulkeAdi))
            {
                Ulke uke = _ParameterContext.UlkeRepository.Filter(s => s.UlkeAdi == ulkeAdi).FirstOrDefault();
                if (uke != null)
                { return uke.UlkeKodu; }
                else
                { return "000"; }
            }
            else return "";
        }
        private string IlKoduGetri(string ilAdi)
        {
            if (!String.IsNullOrEmpty(ilAdi))
            {
                // ilAdi = ilAdi.Replace('İ', 'I');
                Il il = _ParameterContext.IlRepository.Find(s => s.IlAdi == ilAdi);
                if (il == null) return "999";
                else return il.IlKodu;
            }

            else return "";
        }
        private int IlceKoduGetir(string ilceAdi)
        {
            if (!String.IsNullOrEmpty(ilceAdi))
            {
                //ilceAdi = ilceAdi.Replace('i', 'İ').ToUpper();
                Ilce ilce = _ParameterContext.IlceRepository.Find(s => s.IlceAdi == ilceAdi);
                if (ilce == null) return 977;
                else return ilce.IlceKodu;
            }
            else return 0;
        }
        private int IletisimNumaraTipiGetir(string iletisimNumaraTipi)
        {
            if (!String.IsNullOrEmpty(iletisimNumaraTipi))
            {
                if (iletisimNumaraTipi == NumaraTipi_Cep) return 11;
                else if (iletisimNumaraTipi == NumaraTipi_Is) return 12;
                else if (iletisimNumaraTipi == NumaraTipi_Ev) return 13;
                else if (iletisimNumaraTipi == NumaraTipi_Fax) return 14;
                else if (iletisimNumaraTipi == NumaraTipi_Diğer) return 15;
                else return 0;
            }
            else return 0;
        }

        #region Tüzel ek

        //Tüzel müşteri  için
        //private Tuple<string, string> Faaliyet_AnaAlt_SektorGetir(string anaSektor, string altSektor)
        //{
        //    if (!String.IsNullOrEmpty(anaSektor))
        //    {
        //        var sonuc = _TanimService.Get_AnaAlt_SektorKodu(anaSektor, altSektor);
        //        return Tuple.Create(sonuc.Item1, sonuc.Item2);
        //    }
        //    else return Tuple.Create("", "");
        //}
        //private string FaaliyetOlcegiGetir(string faaliyetOlcegi)
        //{
        //    if (!String.IsNullOrEmpty(faaliyetOlcegi))
        //    {
        //        GenelTanimlar sonuc = _TanimService.GetTanimByAciklama("FaaliyetOlcegi", faaliyetOlcegi);
        //        return sonuc.TanimId;
        //    }
        //    else return "";
        //}
        //private string SabitVarlikBilgisiGetir(string sabitVarlikBilgisi)
        //{
        //    if (!String.IsNullOrEmpty(sabitVarlikBilgisi))
        //    {
        //        GenelTanimlar tanim = _TanimService.GetTanimByAciklama("SabitVarlik", sabitVarlikBilgisi);
        //        return tanim.TanimId;
        //    }
        //    else return "";
        //}
        //private string CiroBilgisiGetir(string ciroBilgisi)
        //{
        //    if (!String.IsNullOrEmpty(ciroBilgisi))
        //    {
        //        GenelTanimlar tanim = _TanimService.GetTanimByAciklama("CiroBilgisi", ciroBilgisi);
        //        return tanim.TanimId;
        //    }
        //    else return "";
        //}

        #endregion

        #endregion
    }

    public class ExcelMusteriListModel
    {
        public List<ExcelMusteriModel> HatasizKayitlar { get; set; }
        public List<ExcelMusteriModel> HataliKayitlar { get; set; }
        public string DosyaAdresi { get; set; }
    }

    public class ExcelMusteriModel
    {
        public string HataMesaj { get; set; }
        public string[] hatalist { get; set; }

        //Genel Bilgiler
        public string MusteriTipi { get; set; }
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public string TVMMusteriKodu { get; set; }
        public string KimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string PasaportGecerlilikTarihi { get; set; }
        public string VergiDairesi { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string Cinsiyet { get; set; }
        public string DogumTarihi { get; set; }
        public string EMail { get; set; }
        public string WebUrl { get; set; }
        public string Uyruk { get; set; }
        public string EgitimDurumu { get; set; }
        public string Meslek { get; set; }
        public string MedeniDurumu { get; set; }

        //Adres
        public string AdresTipi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public string IlceKodu { get; set; }
        public string Adres { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public string HanAptFab { get; set; }
        public string PostaKodu { get; set; }
        public string Diger { get; set; }

        //Numara 
        public string IletisimNumaraTipi { get; set; }
        public string TelefonUlkeKodu { get; set; }
        public string AlanKodu { get; set; }
        public string Numara { get; set; }
        public string NumaraSahibi { get; set; }



        //Numara 2
        public string IletisimNumaraTipi_2 { get; set; }
        public string TelefonUlkeKodu_2 { get; set; }
        public string AlanKodu_2 { get; set; }
        public string Numara_2 { get; set; }

        //Numara 3
        public string IletisimNumaraTipi_3 { get; set; }
        public string TelefonUlkeKodu_3 { get; set; }
        public string AlanKodu_3 { get; set; }
        public string Numara_3 { get; set; }
        public string Not { get; set; }

        //Tüzel Müşteri 
        //public string FaaliyetAnaSektor { get; set; }
        //public string FaaliyetAltSektor { get; set; }
        //public string FaaliyetOlcegi { get; set; }
        //public string SabitVarlikBilgisi { get; set; }
        //public string CiroBilgisi { get; set; }
    }
}
