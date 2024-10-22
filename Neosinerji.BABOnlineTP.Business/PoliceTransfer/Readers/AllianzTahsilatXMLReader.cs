using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class AllianzTahsilatXMLReader 
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public AllianzTahsilatXMLReader()
        { }
        public AllianzTahsilatXMLReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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

            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNodeList xnList = doc.SelectNodes("/ACENTE_TAHSILAT/KOC_SIRKET/ACENTE/TAHSILAT_LIST/TAHSILAT");
               
                    #region tahsilat
                    IPoliceService _PoliceService = DependencyResolver.Current.GetService<IPoliceService>();
                    IPoliceTransferService _PoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
                    IPoliceContext _PoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
                    string policeNumarasi = "0";
                    int? ekNo = null;
                    int polSayac = 0;
                    int varOlanKayitlar = 0;
                    foreach (XmlNode xn in xnList)
                    {
                        XmlNodeList polices = xn.ChildNodes;
                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                        var polGenel = new PoliceGenel();
                        bool odemeType = false;
                        tahsilat = new PoliceTahsilat();
                        for (int indx = 0; indx < polices.Count; indx++)
                        {
                            XmlNode polNode = polices[indx];

                            if (polNode.Name == "POLICE_NO") policeNumarasi = polNode.InnerText;
                            if (polNode.Name == "ZEYIL_NO") ekNo = Util.toInt(polNode.InnerText);

                            if (policeNumarasi != "0" && ekNo != null)
                            {
                                polGenel = _PoliceService.getTahsilatPolice(SigortaSirketiBirlikKodlari.ALLIANZSIGORTA, policeNumarasi, ekNo.Value);

                            #region Tahsilat
                            
                                        if (polGenel != null)
                                        {
                                            if (polNode.Name == "TAKSIT_VADESI") tahsilat.TaksitVadeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat2).HasValue ?
                                                Util.toDate(polNode.InnerText, Util.DateFormat2).Value : Convert.ToDateTime("01.01.0001");

                                            if (polNode.Name == "TAKSIT_SIRA_NO") tahsilat.TaksitNo = Util.toInt(polNode.InnerText);

                                            if (polNode.Name == "TAHSILAT_TARIHI") tahsilat.OdemeBelgeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat2).HasValue ?
                                                Util.toDate(polNode.InnerText, Util.DateFormat2).Value : Convert.ToDateTime("01.01.0001");

                                            if (polNode.Name == "ODEME_TIPI")
                                            {
                                                if (polNode.InnerText == "KK")
                                                {
                                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                                    odemeType = true;
                                                }
                                                else if (polNode.InnerText == "NAK")
                                                {
                                                    tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                    tahsilat.OdemeBelgeNo = "111111";
                                                    odemeType = true;
                                                }
                                            }
                                        }
                            
                            #region Tahsilat işlemi
                            
                                        if (odemeType)
                                    {
                                        if (odemeType && polNode.Name == "TUTAR")
                                        {
                                            tahsilat.TaksitTutari = Util.ToDecimal(polNode.InnerText);
                                            tahsilat.OdenenTutar = Util.ToDecimal(polNode.InnerText);
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.PoliceNo = policeNumarasi;
                                            tahsilat.ZeyilNo = ekNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.PoliceSigortaEttiren.KimlikNo) ? polGenel.PoliceSigortaEttiren.KimlikNo : polGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                            tahsilat.BrutPrim = polGenel.BrutPrim.HasValue ? polGenel.BrutPrim.Value : 0;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmkodu;
                                            var resultAyniKayitmi = polGenel.PoliceTahsilats.Where(s => s.PoliceNo == policeNumarasi
                                                   && s.ZeyilNo == ekNo.ToString()
                                                   && s.OdemTipi == tahsilat.OdemTipi
                                                   && s.TaksitNo == tahsilat.TaksitNo
                                                   && s.OdenenTutar == tahsilat.OdenenTutar
                                                   ).FirstOrDefault();
                                            if (resultAyniKayitmi == null)
                                            {
                                                polGenel.PoliceTahsilats.Add(tahsilat);
                                                _PoliceContext.PoliceGenelRepository.Update(polGenel);
                                                _PoliceContext.Commit();
                                                odemeType = false;
                                                polSayac++;
                                            }
                                            else
                                            {
                                                varOlanKayitlar++;
                                            }
                                            break;
                                        }
                                    }
                            
                            #endregion
                        }
                                #endregion
                            }
                        }

                    
             
                    this.message = polSayac + " tahsilat kaydı eklendi." + "Var olan tahsilat kayıt sayısı " + varOlanKayitlar;
                    _PoliceTransferService.setMessage(this.message);
                    return null;
                    #endregion
                
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
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
