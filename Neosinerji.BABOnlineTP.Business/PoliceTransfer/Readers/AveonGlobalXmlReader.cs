using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    public class AveonGlobalXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        public AveonGlobalXmlReader()
        { }

        public AveonGlobalXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
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
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            int carpan = 1;
            string pasaportno = "";
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;


            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));

            }



            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);
                var tanimliOdemeTipleri = _TVMService.GetListTanımliBransOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.AVEONGLOBAL);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;
                decimal dovizKuru = 1;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        carpan = 1;
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];

                        #region Genel Bilgiler

                        XmlNodeList gnlb = polNode["Alanlar"].ChildNodes;

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlb[gnlbidx];

                            if (gnlnode.Name == "PoliceNo") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                            if (gnlnode.Name == "UrunKodu")
                            {
                                tumUrunKodu = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "UrunGrubu")
                            {
                                tumUrunAdi = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "ZeyilNo") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                            if (gnlnode.Name == "TecditNo") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                            if (gnlnode.Name == "ZeyilAdi") police.GenelBilgiler.ZeyilAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "TanzimTarihi")
                                police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText.Replace('.', '/'), Util.DateFormat1);
                            if (gnlnode.Name == "VadeBaslangic") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                            if (gnlnode.Name == "VadeBitis") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText);
                            if (gnlnode.Name == "BrutPrim")
                            {
                                gnlnode.InnerText = gnlnode.InnerText.Replace(',', ' ').Replace('.', ',').Replace(' ', '.');
                                police.GenelBilgiler.BrutPrim = Convert.ToDecimal(gnlnode.InnerText);
                                polBrutprimim = police.GenelBilgiler.BrutPrim;

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
                            if (gnlnode.Name == "DovizKuru")
                            {
                                dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                                    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                    police.GenelBilgiler.DovizKur = dovizKuru;
                                }
                            }
                            if (gnlnode.Name == "KayitTipi")
                            {
                                if (gnlnode.InnerText == "I")
                                {
                                    carpan = -1;
                                    police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                                    foreach (var item in police.GenelBilgiler.PoliceVergis)
                                    {
                                        item.VergiTutari = item.VergiTutari * carpan;
                                    }
                                }

                            }
                            if (gnlnode.Name == "NetPrim")
                            {
                                police.GenelBilgiler.NetPrim = Util.ToDecimal(gnlnode.InnerText) * carpan;
                                polNet = police.GenelBilgiler.NetPrim;
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                                    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                }
                            }
                            if (gnlnode.Name == "Komisyon")
                            {
                                police.GenelBilgiler.Komisyon = Util.ToDecimal(gnlnode.InnerText) * carpan;
                                polKomisyon = police.GenelBilgiler.Komisyon;
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                                    police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                }
                            }
                            //PoliceGenelBrans PoliceBransEslestirRi = new PoliceGenelBrans();
                            //PoliceBransEslestirRi = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                            //police.GenelBilgiler.BransAdi = PoliceBransEslestirRi.BransAdi;
                            //police.GenelBilgiler.BransKodu = PoliceBransEslestirRi.BransKodu;
                            if (police.GenelBilgiler.BransKodu != 1 || police.GenelBilgiler.BransKodu != 2)
                            {
                                if (gnlnode.Name == "RizikoEvIlce") police.GenelBilgiler.PoliceRizikoAdresi.Ilce = gnlnode.InnerText;
                                if (gnlnode.Name == "RizikoEvIli") police.GenelBilgiler.PoliceRizikoAdresi.Il = gnlnode.InnerText;
                                if (gnlnode.Name == "RizikoEvKapiNo") police.GenelBilgiler.PoliceRizikoAdresi.BinaKodu = gnlnode.InnerText;
                                if (gnlnode.Name == "RizikoEvSokak") police.GenelBilgiler.PoliceRizikoAdresi.Sokak = gnlnode.InnerText;
                                if (gnlnode.Name == "RizikoEvMahalle") police.GenelBilgiler.PoliceRizikoAdresi.Mahalle = gnlnode.InnerText;
                            }

                            // Sigorta ettiren - Musteri
                            #region Sigorta ettiren - Musteri
                            if (gnlnode.Name == "MusteriTcKimlikNo") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = gnlnode.InnerText;
                            }
                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            if (gnlnode.Name == "MusteriAdi") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriSoyadi") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvIli") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvIlce") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvMahalle") police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvSokak") police.GenelBilgiler.PoliceSigortaEttiren.Sokak = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvKapiNo") police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = gnlnode.InnerText;

                            if (gnlnode.Name == "MusteriCepTelefonNo") police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEvTelefonNo") police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriEPosta") police.GenelBilgiler.PoliceSigortaEttiren.EMail = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriCinsiyet") police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriDogumTarihi") police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                            #endregion
                            // Sigortalı
                            #region Sigortalı
                            if (gnlnode.Name == "SigortaliPasaportNo")
                            {
                                pasaportno = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "SigortaliEPosta") police.GenelBilgiler.PoliceSigortali.EMail = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliTcKimlikNo") police.GenelBilgiler.PoliceSigortali.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = gnlnode.InnerText;
                                sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortali.KimlikNo == null)
                                {
                                    police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;

                                }
                            }
                            if (gnlnode.Name == "SigortaliAdi")
                            {
                                police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "SigortaliSoyadi") police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvIli") police.GenelBilgiler.PoliceSigortali.IlAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvIlce") police.GenelBilgiler.PoliceSigortali.IlceAdi = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvMahalle") police.GenelBilgiler.PoliceSigortali.Mahalle = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvSokak") police.GenelBilgiler.PoliceSigortali.Sokak = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvKapiNo") police.GenelBilgiler.PoliceSigortali.BinaNo = gnlnode.InnerText;


                            if (gnlnode.Name == "SigortaliCepTelefonNo") police.GenelBilgiler.PoliceSigortali.MobilTelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliEvTelefonNo") police.GenelBilgiler.PoliceSigortali.TelefonNo = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliCinsiyet") police.GenelBilgiler.PoliceSigortali.Cinsiyet = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliDogumTarihi") police.GenelBilgiler.PoliceSigortali.DogumTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);

                            if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) &&
                                !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo)
                                && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == ""
                                && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == ""
                                && police.GenelBilgiler.PoliceSigortali.KimlikNo == ""
                                && police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == ""
                                && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == null
                                && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == null
                                && police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null
                                && police.GenelBilgiler.PoliceSigortali.KimlikNo == null)
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;
                            }

                            else if (pasaportno != "" && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == null)
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = pasaportno;

                            }
                            else if (pasaportno != "" && police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortali.KimlikNo == null)
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;

                            }
                            else if (pasaportno != "" && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "")
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = pasaportno;

                            }
                            else if (pasaportno != "" && police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortali.KimlikNo == "")
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;

                            }
                            #endregion
                            #region Araç Bilgileri

                            if (gnlnode.Name == "AracYolcuKoltukSayisi" && !String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.KoltukSayisi = Convert.ToInt32(gnlnode.InnerText);
                            }
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
                            decimal topVergi = 0;
                            if (gnlnode.Name == "THGF")
                            {
                                PoliceVergi thgf = new PoliceVergi();
                                thgf.VergiKodu = 1;
                                thgf.VergiTutari = 1;
                                topVergi += Util.ToDecimal(gnlnode.InnerText) * carpan;
                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                            }
                            if (gnlnode.Name == "GiderVergisi")
                            {
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                gv.VergiTutari = 1;
                                topVergi += Util.ToDecimal(gnlnode.InnerText) * carpan;
                                police.GenelBilgiler.PoliceVergis.Add(gv);
                            }
                            if (gnlnode.Name == "GHP")
                            {
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                gf.VergiTutari = 1;
                                topVergi += Util.ToDecimal(gnlnode.InnerText) * carpan;
                                police.GenelBilgiler.PoliceVergis.Add(gf);
                            }
                            if (gnlnode.Name == "YSV")
                            {
                                PoliceVergi ysv = new PoliceVergi();
                                ysv.VergiKodu = 4;
                                ysv.VergiTutari = 1;
                                topVergi += Util.ToDecimal(gnlnode.InnerText) * carpan;
                                police.GenelBilgiler.PoliceVergis.Add(ysv);
                            }

                            police.GenelBilgiler.ToplamVergi = topVergi;

                            #endregion

                            // Odeme Sekli
                            if (gnlnode.Name == "odeme-kodu")
                            {
                                if (Util.toInt(gnlnode.InnerText) == 10)
                                {
                                    police.GenelBilgiler.OdemeSekli = 1; //Pesin
                                }
                                else
                                {
                                    police.GenelBilgiler.OdemeSekli = 2; //vadeli
                                }
                            }
                        }

                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AVEONGLOBAL;

                        #endregion

                        #region Odeme Planı
                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestirDoga(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);
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
                        XmlNode tk = polNode["Odemeler"];
                        XmlNodeList tks = tk.ChildNodes;


                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            odm.VadeTarihi = Util.toDate(elm["Vade"].InnerText);
                            if (elm["KapananTutar"].InnerText != null)
                            {
                                odm.TaksitTutari = Convert.ToDecimal(elm["KapananTutar"].InnerText.Replace(',', ' ').Replace('.', ',').Replace(' ', '.'));
                            }
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Convert.ToDecimal(elm["KapananTutar"].InnerText.Replace(',', ' ').Replace('.', ',').Replace(' ', '.'));
                                odm.DovizliTaksitTutari = Math.Round(Convert.ToDecimal(elm["KapananTutar"].InnerText.Replace(',', ' ').Replace('.', ',').Replace(' ', '.')) / police.GenelBilgiler.DovizKur.Value, 2);
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
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.AVEONGLOBAL, police.GenelBilgiler.BransKodu.Value);
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
                                            tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                    //tahsilat.OdemeBelgeNo = "111111";
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
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                if (odmm.TaksitTutari != 0)
                                {
                                    police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                }
                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.AVEONGLOBAL, police.GenelBilgiler.BransKodu.Value);
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
                                                tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }

                            }
                        }

                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
                        #endregion

                        policeler.Add(police);

                    }
                }

                #endregion
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

        string tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas = null, PoliceGenel police = null)
        {
            foreach (var item in neoOnline_TahsilatKapatmas)
            {
                if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                {
                    return item.Kart_No.Trim();
                }
            }
            return "";
        }

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}