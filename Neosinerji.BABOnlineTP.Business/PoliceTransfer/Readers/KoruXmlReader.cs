using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class KoruXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public KoruXmlReader()
        { }

        public KoruXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            decimal dovizKuru = 1;
            int carpan = 1; //tahakkuk - iptal
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(filePath);

                //doc.Normalize();

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        int kontrol = 0;
                        carpan = 1;
                        decimal verTop = 0;
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];
                        //  string ekno = "156446540115";

                        #region Genel Bilgiler

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        XmlNode iptal = polNode.SelectSingleNode("Alanlar/KayitTipi");
                        if (iptal != null)
                        {
                            if (iptal.InnerText == "I")
                            {
                                carpan = -1;
                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                                police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * carpan;
                                police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * carpan;
                                police.GenelBilgiler.ToplamVergi = police.GenelBilgiler.ToplamVergi * carpan;
                                foreach (var item in police.GenelBilgiler.PoliceVergis)
                                {
                                    item.VergiTutari = item.VergiTutari * carpan;
                                }
                                foreach (var item in police.GenelBilgiler.PoliceOdemePlanis)
                                {
                                    item.TaksitTutari = item.TaksitTutari * carpan;
                                }
                            }
                        }

                        XmlNodeList gnlb = polNode["Alanlar"].ChildNodes;

                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlb[gnlbidx];

                            if (gnlnode.Name == "PoliceNo") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                            if (gnlnode.Name == "UrunKodu") tumUrunKodu = gnlnode.InnerText;
                            if (gnlnode.Name == "ZeyilNo") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                            if (gnlnode.Name == "TecditNo") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                            if (gnlnode.Name == "ZeyilAdi") police.GenelBilgiler.ZeyilAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "EGMTescilBelgeSeriKod") police.GenelBilgiler.PoliceArac.TescilSeriKod = gnlnode.InnerText;
                            if (gnlnode.Name == "EGMTescilBelgeSeriNo") police.GenelBilgiler.PoliceArac.TescilSeriNo = gnlnode.InnerText;
                            if (gnlnode.Name == "BrutPrim")
                            {
                                string brut = gnlnode.InnerText;
                                if (gnlnode.InnerText.Contains(","))
                                {
                                    // (1,532.45) gibi gelen string i  düzeltmek için 
                                    brut = brut.Remove(brut.IndexOf(","), 1);
                                }
                                police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(brut);
                                polBrutprimim = police.GenelBilgiler.BrutPrim;
                            }
                            if (gnlnode.Name == "DovizKuru")
                            {
                                dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                police.GenelBilgiler.DovizKur = dovizKuru;
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                                    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                }
                            }
                            if (gnlnode.Name == "DovizTipi")
                            {
                                if (gnlnode.InnerText == "1")
                                {
                                    police.GenelBilgiler.ParaBirimi = "USD";
                                }
                                if (gnlnode.InnerText == "3")
                                {
                                    police.GenelBilgiler.ParaBirimi = "AUD";
                                }
                                if (gnlnode.InnerText == "11")
                                {
                                    police.GenelBilgiler.ParaBirimi = "SEK";
                                }
                                if (gnlnode.InnerText == "12")
                                {
                                    police.GenelBilgiler.ParaBirimi = "CHF";
                                }
                                if (gnlnode.InnerText == "14")
                                {
                                    police.GenelBilgiler.ParaBirimi = "JPY";
                                }
                                if (gnlnode.InnerText == "15")
                                {
                                    police.GenelBilgiler.ParaBirimi = "CAD";
                                }
                                if (gnlnode.InnerText == "16")
                                {
                                    police.GenelBilgiler.ParaBirimi = "KWD";
                                }
                                if (gnlnode.InnerText == "17")
                                {
                                    police.GenelBilgiler.ParaBirimi = "NOK";
                                }
                                if (gnlnode.InnerText == "18")
                                {
                                    police.GenelBilgiler.ParaBirimi = "GBP";
                                }
                                if (gnlnode.InnerText == "19")
                                {
                                    police.GenelBilgiler.ParaBirimi = "SAR";
                                }
                                if (gnlnode.InnerText == "20")
                                {
                                    police.GenelBilgiler.ParaBirimi = "EUR";
                                }
                                if (gnlnode.InnerText == "99")
                                {
                                    police.GenelBilgiler.ParaBirimi = "TL";
                                }
                            }
                            if (gnlnode.Name == "NetPrim")
                            {
                                police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                polNet = police.GenelBilgiler.NetPrim;
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                                    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                    dovizKuru = 0;
                                }
                            }
                            if (gnlnode.Name == "Komisyon")
                            {
                                police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
                                polKomisyon = police.GenelBilgiler.Komisyon;
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                                    police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                }
                            }
                            if (gnlnode.Name == "PoliceBaslangicTarihi" && gnlnode.InnerText != "")
                            {
                                police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                                kontrol = 1;
                            }
                            if (gnlnode.Name == "PoliceGirisTarihi" && kontrol == 0)
                            {
                                police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "TanzimTarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText);
                            if (gnlnode.Name == "VadeBitis") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText);
                            #region Araç Bilgileri

                            if (gnlnode.Name == "AracKisiSayisi" && !String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.KoltukSayisi = Convert.ToInt32(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "AracKodu")
                            {
                                police.GenelBilgiler.PoliceArac.Marka = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracKullanimSekli")
                            {
                                police.GenelBilgiler.PoliceArac.KullanimSekli = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracMarka")
                            {
                                police.GenelBilgiler.PoliceArac.MarkaAciklama = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracModelYili" && !String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "AracMotorGucu")
                            {
                                police.GenelBilgiler.PoliceArac.MotorGucu = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracMotorNo")
                            {
                                police.GenelBilgiler.PoliceArac.MotorNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracPlaka")
                            {
                                police.GenelBilgiler.PoliceArac.PlakaNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracPlakaIlKodu")
                            {
                                police.GenelBilgiler.PoliceArac.PlakaKodu = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracRenk")
                            {
                                police.GenelBilgiler.PoliceArac.Renk = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracSasiNo")
                            {
                                police.GenelBilgiler.PoliceArac.SasiNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracSilindirHacmi")
                            {
                                police.GenelBilgiler.PoliceArac.SilindirHacmi = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracTarz")
                            {
                                police.GenelBilgiler.PoliceArac.KullanimTarzi = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "AracTipi")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiAciklama = gnlnode.InnerText;
                            }
                            #endregion

                            #region Vergiler

                            if (gnlnode.Name == "GHP")
                            {
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                gf.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                verTop += carpan * Util.ToDecimal(gnlnode.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(gf);
                            }

                            if (gnlnode.Name == "GiderVergisi")
                            {
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                gv.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                verTop += carpan * Util.ToDecimal(gnlnode.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(gv);
                            }

                            if (gnlnode.Name == "THGF")
                            {
                                PoliceVergi thgf = new PoliceVergi();
                                thgf.VergiKodu = 1;
                                thgf.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                verTop += carpan * Util.ToDecimal(gnlnode.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                            }

                            if (gnlnode.Name == "YSV")
                            {
                                PoliceVergi ysv = new PoliceVergi();
                                ysv.VergiKodu = 4;
                                ysv.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                verTop += carpan * Util.ToDecimal(gnlnode.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(ysv);
                            }

                            #endregion
                            if (police.GenelBilgiler.BransKodu != 1 || police.GenelBilgiler.BransKodu != 2)
                            {
                                if (police.GenelBilgiler.BransKodu != null)
                                {
                                    if (gnlnode.Name == "RizikoEvIlce") police.GenelBilgiler.PoliceRizikoAdresi.Ilce = gnlnode.InnerText;
                                    if (gnlnode.Name == "RizikoEvIli") police.GenelBilgiler.PoliceRizikoAdresi.Il = gnlnode.InnerText;
                                    if (gnlnode.Name == "RizikoEvKapiNo") police.GenelBilgiler.PoliceRizikoAdresi.BinaKodu = gnlnode.InnerText;
                                    if (gnlnode.Name == "RizikoEvSokak") police.GenelBilgiler.PoliceRizikoAdresi.Sokak = gnlnode.InnerText;
                                    if (gnlnode.Name == "RizikoEvMahalle") police.GenelBilgiler.PoliceRizikoAdresi.Mahalle = gnlnode.InnerText;
                                }
                            }
                            // Sigorta ettiren - Musteri

                            if (gnlnode.Name == "MusteriAdi") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriSoyadi") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriTcKimlikNo") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = gnlnode.InnerText;
                            }
                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            if (gnlnode.Name == "MusteriEvIli") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvIlce") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvMahalle") police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvSokak") police.GenelBilgiler.PoliceSigortaEttiren.Sokak = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvKapiNo")
                            {
                                if (gnlnode.InnerText.Length < 20)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = gnlnode.InnerText;
                                }
                            }
                            if (gnlnode.Name == "MusteriCepTelefonNo") police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvTelefonNo") police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEPosta") police.GenelBilgiler.PoliceSigortaEttiren.EMail = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriCinsiyet") police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriDogumTarihi") police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);

                            //sigortalı
                            string pasaportno = "";
                            if (gnlnode.Name == "SigortaliAdi") police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliSoyadi") police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliTcKimlikNo") police.GenelBilgiler.PoliceSigortali.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = gnlnode.InnerText;
                            }
                            sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            if (gnlnode.Name == "SigortaliEvIli") police.GenelBilgiler.PoliceSigortali.IlAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvIlce") police.GenelBilgiler.PoliceSigortali.IlceAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvMahalle") police.GenelBilgiler.PoliceSigortali.Mahalle = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvSokak") police.GenelBilgiler.PoliceSigortali.Sokak = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvKapiNo")
                            {
                                if (gnlnode.InnerText.Length < 20)
                                {
                                    police.GenelBilgiler.PoliceSigortali.BinaNo = gnlnode.InnerText;
                                }
                            }
                            if (gnlnode.Name == "SigortaliCepTelefonNo") police.GenelBilgiler.PoliceSigortali.MobilTelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvTelefonNo") police.GenelBilgiler.PoliceSigortali.TelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliPasaportNo") pasaportno = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEPosta") police.GenelBilgiler.PoliceSigortali.EMail = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliCinsiyet") police.GenelBilgiler.PoliceSigortali.Cinsiyet = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliDogumTarihi") police.GenelBilgiler.PoliceSigortali.DogumTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                            if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) &&
                                String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;
                            }
                        }

                        #region Odeme Planı

                        XmlNode tk = polNode["Odemeler"];
                        XmlNodeList tks = tk.ChildNodes;
                        XmlNode tkOpl = polNode["Opl"];
                        XmlNode tkTaksitArr = tkOpl["TaksitArr"];
                        XmlNodeList tksTaksitArr = tkTaksitArr.ChildNodes;
                        DateTime? zeyilBaslangicTarihi = DateTime.Now;
                        for (int indx = 0; indx < tks.Count; indx++)
                        {

                            XmlNode elm = tks.Item(indx);
                            XmlNode elmTaksitArr = tksTaksitArr.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            if (iptal.InnerText == "I")
                            {
                                if (indx < tkTaksitArr.ChildNodes.Count)
                                    zeyilBaslangicTarihi = Util.toDate(elmTaksitArr["VadeTarihi"].InnerText);
                                odm.VadeTarihi = zeyilBaslangicTarihi.Value;
                                zeyilBaslangicTarihi = zeyilBaslangicTarihi.Value.AddMonths(1);
                            }
                            else if (iptal.InnerText == "K")
                            {
                                if (indx < tkTaksitArr.ChildNodes.Count)
                                    zeyilBaslangicTarihi = Util.toDate(elmTaksitArr["VadeTarihi"].InnerText);
                                odm.VadeTarihi = zeyilBaslangicTarihi.Value;
                                zeyilBaslangicTarihi = zeyilBaslangicTarihi.Value.AddMonths(1);
                            }
                            else
                            {
                                try
                                {
                                    odm.VadeTarihi = Util.toDate(elmTaksitArr["VadeTarihi"].InnerText);  
                                }
                                catch (Exception)
                                {
                                    odm.VadeTarihi = zeyilBaslangicTarihi.Value;
                                    zeyilBaslangicTarihi = zeyilBaslangicTarihi.Value.AddMonths(1);
                                }

                            }
                            string brut = elm["EvrakTutari"].InnerText;
                            if (brut.Contains(","))
                            {
                                // (1,532.45) gibi gelen string i  düzeltmek için 
                                brut = brut.Remove(brut.IndexOf(","), 1);
                            }
                            odm.TaksitTutari = Util.ToDecimal(brut);
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                string kapanan = elm["EvrakTutari"].InnerText;
                                if (kapanan.Contains(","))
                                {
                                    // (1,532.45) gibi gelen string i  düzeltmek için 
                                    kapanan = kapanan.Remove(kapanan.IndexOf(","), 1);
                                }
                                odm.TaksitTutari = Util.ToDecimal(kapanan);
                                odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(kapanan) / police.GenelBilgiler.DovizKur.Value, 2);
                            }
                            if (!String.IsNullOrEmpty(elm["HesapCek"].InnerText))
                            {
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            else
                            {
                                odm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            if (odm.TaksitTutari != 0)
                            {
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi
                            PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                            PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                            police.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                            police.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.KORUSIGORTA, police.GenelBilgiler.BransKodu.Value);
                            if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                            {
                                int otoOdeSayac = 0;
                                foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                {
                                    if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                    {
                                        otoOdeSayac++;
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();

                                        tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                        odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                        if (tahsilat.OdemTipi == 1)
                                        {
                                            tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                        }
                                        else
                                        {
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari;
                                        }
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(elm["HesapCek"].InnerText))
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = elm["HesapCek"].InnerText;
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.Value;
                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                else
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.Havale;
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    // tahsilat.OdemeBelgeNo = "111111";
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = 0;
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.Value;
                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                            }

                            #endregion

                        }
                        if (tks.Count == 0)
                        {

                            PoliceOdemePlani odmm = new PoliceOdemePlani();

                            if (odmm.TaksitTutari == null && police.GenelBilgiler.BrutPrim.Value != 0)
                            {
                                odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                {
                                    odmm.TaksitTutari = police.GenelBilgiler.BrutPrim.Value;
                                    odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                }
                                if (odmm.VadeTarihi == null)
                                {
                                    odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                }
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                odmm.TaksitNo = 1;
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                int? branskodu = null;
                                if (police.GenelBilgiler.BransKodu.HasValue)
                                {
                                    branskodu = police.GenelBilgiler.BransKodu.Value;
                                }
                                else
                                {
                                    branskodu = null;
                                }
                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.KORUSIGORTA, branskodu);
                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                {
                                    int otoOdeSayac = 0;
                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                    {
                                        if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                        {
                                            otoOdeSayac++;
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();

                                            tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                            odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                            if (tahsilat.OdemTipi == 1)
                                            {
                                                tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                tahsilat.KalanTaksitTutari = 0;
                                                tahsilat.OdemeBelgeNo = "111111****1111";
                                                tahsilat.OtomatikTahsilatiKkMi = 1;
                                            }
                                            else
                                            {
                                                tahsilat.OdenenTutar = 0;
                                                tahsilat.KalanTaksitTutari = odmm.TaksitTutari;
                                            }
                                            tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odmm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                            tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                            tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (branskodu == 1 || branskodu == 2)
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = "111111****1111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //  tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }

                            }

                        }

                        #endregion

                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
                        //police.Genel.TaliAcenteKodu = "";
                        //police.Genel.HashCode = "";
                        police.GenelBilgiler.ToplamVergi = verTop;
                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.KORUSIGORTA;
                        #endregion

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

                        if (tumUrunAdi == null)
                        {
                            police.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                        }
                        else
                        {
                            police.GenelBilgiler.TUMUrunAdi = tumUrunAdi;
                        }

                        if (tumUrunKodu == null)
                        {
                            police.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                        }
                        else
                        {
                            police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                        }

                        police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                        police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;
                        policeler.Add(police);



                    }
                }
            }
            catch (Exception ex)
            {
                this.message = ex.ToString();
                policeler = null;
            }
            return policeler;
        }

        public string getMessage()
        {
            return this.message;
        }

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
