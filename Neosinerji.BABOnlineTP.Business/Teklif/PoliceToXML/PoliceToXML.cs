using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Neosinerji.BABOnlineTP.Business
{
    public class PoliceToXML : IPoliceToXML
    {
        ILogService _Log;
        ITVMService _TVMService;
        IUlkeService _UlkeService;
        ITUMService _TUMService;
        ITeklifService _TeklifService;

        public PoliceToXML()
        {
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _TUMService = DependencyResolver.Current.GetService<ITUMService>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
        }

        public void SendPoliceToMuhasebe(ITeklif teklif)
        {
            try
            {
                if (teklif.GenelBilgiler != null && teklif.GenelBilgiler.TeklifDurumKodu == TeklifDurumlari.Police)
                {
                    TVMDetay tvm = teklif.GenelBilgiler.TVMDetay;

                    if (tvm != null && tvm.MuhasebeEntegrasyon.HasValue && tvm.MuhasebeEntegrasyon.Value)
                    {
                        XElement element = XelementParse(teklif, tvm);

                        //Muhasebeye gönderilen xml kaydediliyor.
                        teklif.BeginLog(element.ToString(), WebServisIstekTipleri.Muhasebe);

                        string id = SendPoliceToEKobiApi(element);

                        //Servis id gönderirse web servis cevabı olarak kaydediyoruz.
                        if (!String.IsNullOrEmpty(id))
                        {
                            teklif.AddWebServisCevap(WebServisCevaplar.PoliceMuhasebeId, id);
                            teklif.EndLog(id, true);
                        }
                        else
                        {
                            teklif.EndLog("ID bilgisi null.", false);
                        }

                        teklif.GenelBilgiler.WEBServisLogs = teklif.Log;
                        teklif.GenelBilgiler.TeklifWebServisCevaps = teklif.WebServisCevaplar;
                        _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                teklif.EndLog("Servise ulaşırken bir hata oluştu", false);
                teklif.GenelBilgiler.WEBServisLogs = teklif.Log;
                teklif.GenelBilgiler.TeklifWebServisCevaps = teklif.WebServisCevaplar;
                _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);
            }
        }

        private string SendPoliceToEKobiApi(XElement xml)
        {
            string returnId = String.Empty;
            IKonfigurasyonService _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();

            string muhasebeURL = _KonfigurasyonService.GetKonfigDeger(Konfig.MuhasebeURL);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(muhasebeURL);

            request.ContentType = "text/json";
            request.Method = "POST";

            string Json = JsonConvert.SerializeXNode(xml);

            //Json = Json.Replace("\\", "").Replace("\"", "");

            byte[] bytes = Encoding.UTF8.GetBytes(Json);

            request.ContentLength = bytes.Length;

            using (Stream putStream = request.GetRequestStream())
            {
                putStream.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream data = response.GetResponseStream();
                StreamReader reader = new StreamReader(data);

                string resultText = reader.ReadToEnd();

                dynamic result = JsonConvert.DeserializeObject(resultText);
                returnId = result._id;
            }

            return returnId;
        }

        public XElement XelementParse(ITeklif teklif, TVMDetay tvm)
        {
            TeklifGenel teklifGenel = teklif.GenelBilgiler;
            TeklifSigortali teklifSigortali = teklifGenel.TeklifSigortalis.FirstOrDefault();
            TeklifSigortaEttiren teklifSigortaEttiren = teklifGenel.TeklifSigortaEttirens.FirstOrDefault();

            MusteriGenelBilgiler sigortali = new MusteriGenelBilgiler();
            MusteriTelefon sigortaliTel = new MusteriTelefon();
            MusteriAdre sigortaliAdres = new MusteriAdre();

            MusteriGenelBilgiler sigortaEttiren = new MusteriGenelBilgiler();
            MusteriTelefon sigortaEttirenTel = new MusteriTelefon();
            MusteriAdre sigortaEttirenAdres = new MusteriAdre();


            if (teklifSigortali != null)
                sigortali = teklifSigortali.MusteriGenelBilgiler;

            if (sigortali.MusteriTelefons.Count() > 0)
                sigortaliTel = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();

            if (sigortali.MusteriAdres.Count() > 0)
                sigortaliAdres = sigortali.MusteriAdres.Where(s => s.Varsayilan == true).FirstOrDefault();



            if (teklifSigortaEttiren != null)
                sigortaEttiren = teklifSigortaEttiren.MusteriGenelBilgiler;

            if (sigortali.MusteriTelefons.Count() > 0)
                sigortaEttirenTel = sigortaEttiren.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();

            if (sigortali.MusteriAdres.Count() > 0)
                sigortaEttirenAdres = sigortaEttiren.MusteriAdres.Where(s => s.Varsayilan == true).FirstOrDefault();


            XElement element = new XElement("Police",
                                            new XElement("TeklifNo", teklifGenel.TeklifNo),
                                            new XElement("PoliceNo", teklifGenel.TUMPoliceNo),

                                            //Urun Bilgileri
                                            new XElement("Urun",
                                                         new XElement("UrunKodu", teklifGenel.UrunKodu),
                                                         new XElement("UrunAdi", UrunKodlari.GetUrunAdi(teklifGenel.UrunKodu))),

                                            //Acente Bilgileri
                                            new XElement("Acente",
                                                         new XElement("AcenteKodu", tvm.Kodu),
                                                         new XElement("AcenteVergiNo", tvm.VergiNumarasi),
                                                         new XElement("AcenteLevhaNo", ""),
                                                         new XElement("Unvan", tvm.Unvani),
                                                         new XElement("TelefonNo", tvm.Telefon),
                                                         new XElement("PartajNo", GetTVMPartajNo(tvm.Kodu, teklifGenel.TUMKodu)),
                                                         new XElement("Adres",
                                                                      new XElement("Il", _UlkeService.GetIlAdi("TUR", tvm.IlKodu)),
                                                                      new XElement("Ilce", _UlkeService.GetIlceAdi(tvm.IlceKodu.HasValue ?
                                                                                                (int)tvm.IlceKodu : 0))),
                                                                      new XElement("Adres", tvm.Adres)),

                                            //Sigorta Şirketi Bilgileri
                                            new XElement("SigortaSirketi",
                                                         new XElement("SigortaSirketKodu", teklifGenel.TUMKodu),
                                                         new XElement("VergiNumarasi", GetTUMVergiNumarasi(teklifGenel.TUMKodu)),
                                                         new XElement("Unvani", _TUMService.GetTumUnvan(teklifGenel.TUMKodu))),

                                            //Sigortali
                                            new XElement("Sigortali",
                                                         new XElement("MusteriTipi", MusteriTipleri.MusteriTipi(sigortali.MusteriTipKodu)),
                                                         new XElement("VergiNoTCKimlikNo",!String.IsNullOrEmpty(sigortali.KimlikNo) ? sigortali.KimlikNo:""),
                                                         new XElement("AdSoyadUnvani", sigortali.AdiUnvan + " " + sigortali.SoyadiUnvan),
                                                         new XElement("TelefonNo", (IletisimNumaraTipleri.IletisimNumaraTipi((byte)sigortaliTel.IletisimNumaraTipi)
                                                                      + " " + sigortaliTel.Numara)),
                                                         new XElement("eMail", sigortali.EMail),
                                                         new XElement("Adres",
                                                                      new XElement("Il", _UlkeService.GetIlAdi("TUR", sigortaliAdres.IlKodu)),
                                                                      new XElement("Ilce", _UlkeService.GetIlceAdi(sigortaliAdres.IlceKodu.HasValue ?
                                                                                            teklifGenel.TVMDetay.IlceKodu.Value : 0)),
                                                                      new XElement("Adres", sigortaliAdres.Adres))),
                //Sİgorta Ettiren   
                                            new XElement("SigortaEttiren",
                                                         new XElement("MusteriTipi", MusteriTipleri.MusteriTipi(sigortaEttiren.MusteriTipKodu)),
                                                         new XElement("VergiNoTCKimlikNo", !String.IsNullOrEmpty(sigortaEttiren.KimlikNo) ? sigortaEttiren.KimlikNo : ""),
                                                         new XElement("AdSoyadUnvani", sigortaEttiren.AdiUnvan + " " + sigortaEttiren.SoyadiUnvan),
                                                         new XElement("TelefonNo", (IletisimNumaraTipleri.IletisimNumaraTipi((byte)sigortaEttirenTel.IletisimNumaraTipi)
                                                                      + " " + sigortaEttirenTel.Numara)),
                                                         new XElement("eMail", sigortaEttiren.EMail),
                                                         new XElement("Adres",
                                                                      new XElement("Il", _UlkeService.GetIlAdi("TUR", sigortaEttirenAdres.IlKodu)),
                                                                      new XElement("Ilce", _UlkeService.GetIlceAdi(sigortaEttirenAdres.IlceKodu.HasValue ?
                                                                                            teklifGenel.TVMDetay.IlceKodu.Value : 0)),
                                                                      new XElement("Adres", sigortaEttirenAdres.Adres))),

                                            new XElement("NetPrim", teklifGenel.NetPrim),
                                            new XElement("ToplamVergi", teklifGenel.ToplamVergi),
                                            new XElement("BrutPrim", teklifGenel.BrutPrim),
                                            new XElement("ToplamKomisyon", teklifGenel.ToplamKomisyon),
                                            new XElement("TanzimTarihi", teklifGenel.TanzimTarihi.ToString("dd.MM.yyyy")),
                                            new XElement("BaslangicTarihi", teklifGenel.BaslamaTarihi.ToString("dd.MM.yyyy")),
                                            new XElement("BitisTarihi", teklifGenel.BitisTarihi.ToString("dd.MM.yyyy")),
                                            new XElement("TaksitSayisi", teklifGenel.TaksitSayisi),
                                            new XElement("OdemeSekli", OdemeSekilleri.OdemeSekli(teklifGenel.OdemeSekli)),
                                            new XElement("OdemeTipi", OdemeTipleri.OdemeTipi(teklifGenel.OdemeTipi))
                                             );


            XElement odeme = new XElement("Odeme");

            foreach (var item in teklifGenel.TeklifOdemePlanis)
            {
                XElement taksit = new XElement("Taksit",
                                              new XElement("TaksitNo", item.TaksitNo),
                                              new XElement("VadeTarihi", item.VadeTarihi.HasValue ? item.VadeTarihi.Value.ToString("dd.MM.yyyy") : ""),
                                              new XElement("TaksitTutari", item.TaksitTutari),
                                              new XElement("OdemeTipi", OdemeTipleri.OdemeTipi(item.OdemeTipi)));
                odeme.Add(taksit);
            }

            element.Add(odeme);

            return element;
        }

        public string GetTVMPartajNo(int tvmKodu, int tumKodu)
        {
            string result = String.Empty;

            TVMWebServisKullanicilari kullanici = _TVMService.GetTVMWebServisKullanicilari(tvmKodu, tumKodu);
            if (kullanici != null)
                result = kullanici.PartajNo_;

            return result;
        }

        public string GetTUMVergiNumarasi(int tumKodu)
        {
            string result = String.Empty;

            TUMDetay tum = _TUMService.GetDetay(tumKodu);
            if (tum != null)
                result = tum.VergiNumarasi;

            return result;
        }
    }
}
