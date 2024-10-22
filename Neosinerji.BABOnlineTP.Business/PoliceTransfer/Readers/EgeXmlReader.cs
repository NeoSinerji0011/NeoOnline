using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class EgeXmlReader : IPoliceTransferReader
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        public EgeXmlReader()
        { }

        public EgeXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;

            int carpan = 1; //tahakkuk - iptal
            string tumUrunAdi = null;
            string tumUrunKodu = null;


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
                            if (gnlnode.Name == "BrutPrim")
                            {
                                string brut = gnlnode.InnerText;
                                if (gnlnode.InnerText.Contains(","))
                                {
                                    // (1,532.45) gibi gelen string i  düzeltmek için 
                                    brut = brut.Remove(brut.IndexOf(","), 1);
                                }
                                police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(brut);
                            }
                            if (gnlnode.Name == "NetPrim") police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                            if (gnlnode.Name == "Komisyon") police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
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



                            //vergiler
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


                            // Sigorta ettiren - Musteri

                            if (gnlnode.Name == "MusteriAdi") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriTcKimlikNo") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = gnlnode.InnerText;
                            }

                            //sigortalı
                            if (gnlnode.Name == "SigortaliAdi") police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "SigortaliSoyadi") police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriTcKimlikNo") police.GenelBilgiler.PoliceSigortali.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "MusteriVergiNo" && gnlnode.InnerText.Length == 10)
                            {
                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = gnlnode.InnerText;
                            }
                        }

                        #region Odeme Planı

                        XmlNode tk = polNode["Odemeler"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            odm.VadeTarihi = Util.toDate(elm["Vade"].InnerText);
                            odm.TaksitTutari = Util.ToDecimal(elm["EvrakTutari"].InnerText);

                            police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                        }

                        #endregion

                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
                        //police.Genel.TaliAcenteKodu = "";
                        //police.Genel.HashCode = "";
                        police.GenelBilgiler.ToplamVergi = verTop;
                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.EGESIGORTA;
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
