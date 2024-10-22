using Neosinerji.BABOnlineTP.Business.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Business.Paritus;

namespace Neosinerji.BABOnlineTP.Business
{
    public class ParitusService : IParitusService
    {
        IKonfigurasyonService _KonfigurasyonService;

        public ParitusService()
        {
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
        }

        public ParitusAdresModel GetParitusAdres(ParitusAdresSorgulamaRequest adresModel)
        {
            ParitusAdresModel model = new ParitusAdresModel();

            if (adresModel != null)
            {
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.Paritus_ServiceURL);
                string apiKey = _KonfigurasyonService.GetKonfigDeger(Konfig.Paritus_ApiKey);


                string URL = serviceURL + "?id=1&searchuavt=true";

                if (!String.IsNullOrEmpty(adresModel.address))
                    URL += "&address=" + adresModel.address;

                if (!String.IsNullOrEmpty(adresModel.city))
                    URL += "&city=" + adresModel.city;

                if (!String.IsNullOrEmpty(adresModel.town))
                    URL += "&town=" + adresModel.town;

                if (!String.IsNullOrEmpty(adresModel.district))
                    URL += "&district=" + adresModel.district;

                if (!String.IsNullOrEmpty(adresModel.zipcode))
                    URL += "&zipcode=" + adresModel.zipcode;

                if (!String.IsNullOrEmpty(adresModel.street))
                    URL += "&street=" + adresModel.street;



                URL += "&apikey=" + apiKey;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

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
                        model.IlKodu = response.parsedAddress.cityCode;

                        if (!String.IsNullOrEmpty(response.parsedAddress.townCode))
                            model.IlceKodu = Convert.ToInt32(response.parsedAddress.townCode);


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
                        model.uavtBuildingCode = response.parsedAddress.uavtBuildingCode;
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
    }
}
