using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;

namespace Neosinerji.BABOnlineTP.Business.TeklifYapayZeka
{
    public class TeklifYapayZekaService : ITeklifYapayZekaService
    {
        IYapayZekaContext _YapayZekaContext;
        public TeklifYapayZekaService(IYapayZekaContext yapayZekaContext)
        {
            _YapayZekaContext = yapayZekaContext;
        }

        public TeklifYapayZekaModel callAIService(TeklifYapayZekaData data)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Cinsiyet", "DTVarYok", "YasKategorisi", "AtananIl", "AtananIlce", "AtananMah", "BransKodu", "TUMBirlikKodu", "Ekno", "YenilemeNo", "TanzimTrYil", "TanzimTrAy", "TanzimTrGun", "BasTrYil", "BasTrAy", "BasTrGun", "Tanzim_BaslangicTarGunFark", "BitTrYil", "BitTrAy", "BitTrGun", "BrutPrim", "NetPrim", "Komisyon", "OdemeSekli", "Durum", "TaliKomisyonOran", "TaliKomisyon", "PlakaKodu", "Marka", "MarkaAciklama", "Model", "AracFiyati", "AracSinifi", "BuyukSehirMi", "IlNufusu", "IlceNufusu", "AdresMahalleMi", "AdresKoyMu", "MahalleKoyNufusu", "IlNufusYogunlugu", "IlceNufusYogunlugu", "IlceIleUzaklik", "MahalleIleUzaklik", "Kategori"},
                                Values = new string[,] {  { data.cinsiyet, data.DTVarYok, data.YasKategorisi, data.AtananIl, data.AtananIlce, data.AtananMah, data.BransKodu, data.TUMBirlikKodu, data.Ekno, data.YenilemeNo, data.TanzimTrYil, data.TanzimTrAy, data.TanzimTrGun, data.BasTrYil, data.BasTrAy, data.BasTrGun, data.Tanzim_BaslangicTarGunFark, data.BitTrYil, data.BitTrAy, data.BitTrGun, data.BrutPrim, data.NetPrim, data.Komisyon, data.OdemeSekli, data.Durum, data.TaliKomisyonOran, data.TaliKomisyon, data.PlakaKodu, data.Marka, data.MarkaAciklama, data.Model, data.AracFiyati, data.AracSinifi, data.BuyukSehirMi, data.IlceNufusu, data.IlceNufusu, data.AdresMahalleMi, data.AdresKoyMu, data.MahalleKoyNufusu, data.IlNufusYogunlugu, data.IlceNufusYogunlugu, data.IlceIleUzaklik, data.MahalleIleUzaklik, data.Kategori},  }
                                //Values = new string[,] {  { "value", "value", "value", "value", "value", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value", "0", "value", "value", "value", "value", "value", "value", "value", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  { "value", "value", "value", "value", "value", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value", "0", "value", "value", "value", "value", "value", "value", "value", "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "czFrX2ni5CD+bqmkpW/4pedqXOKUlIMyS9OaiuQeAZlPXm39r1nstoZH+mxr7GYUDCJH57Fn7Wt4yuRplsqTvg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/d9b10dd996634529be5854daa5772a8b/services/68dad6a1bc3149a8a9cf44333ce62850/execute?api-version=2.0&details=true");


                var myContent = JsonConvert.SerializeObject(scoreRequest);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.PostAsync("", byteContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(result);
                    AIRespone response1 = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<AIRespone>(result);
                    var fdate = json["Results"];
                    AIRespone rs = fdate.ToObject<AIRespone>();
                    Dictionary<string, AIRespone> Results = fdate.ToObject<Dictionary<string, AIRespone>>();
                    TeklifYapayZekaModel model = new TeklifYapayZekaModel();
                    AIRespone aiResponse = Results.Values.Where(t => t.type.Equals("table")).FirstOrDefault();
                    model.score1 = float.Parse(aiResponse.value.Values[0, 37], System.Globalization.CultureInfo.InvariantCulture); 
                                  
                    model.score2 = float.Parse(aiResponse.value.Values[0, 38], System.Globalization.CultureInfo.InvariantCulture);

                    model.score3 = float.Parse(aiResponse.value.Values[0, 39], System.Globalization.CultureInfo.InvariantCulture);

                    model.score4 = float.Parse(aiResponse.value.Values[0, 40], System.Globalization.CultureInfo.InvariantCulture);

                    model.score5 = float.Parse(aiResponse.value.Values[0, 41], System.Globalization.CultureInfo.InvariantCulture);

                    model.score6 = float.Parse(aiResponse.value.Values[0, 42], System.Globalization.CultureInfo.InvariantCulture);

                    model.score7 = float.Parse(aiResponse.value.Values[0, 43], System.Globalization.CultureInfo.InvariantCulture);

                    model.score8 = float.Parse(aiResponse.value.Values[0, 44], System.Globalization.CultureInfo.InvariantCulture);

                    model.score9 = float.Parse(aiResponse.value.Values[0, 45], System.Globalization.CultureInfo.InvariantCulture);

                    model.score10 = float.Parse(aiResponse.value.Values[0, 46], System.Globalization.CultureInfo.InvariantCulture);

                    model.score11 = float.Parse(aiResponse.value.Values[0, 47], System.Globalization.CultureInfo.InvariantCulture);

                    model.score12 = float.Parse(aiResponse.value.Values[0, 48], System.Globalization.CultureInfo.InvariantCulture);

                    model.score13 = float.Parse(aiResponse.value.Values[0, 49], System.Globalization.CultureInfo.InvariantCulture);

                    model.score14 = float.Parse(aiResponse.value.Values[0, 50], System.Globalization.CultureInfo.InvariantCulture);

                    model.score15 = float.Parse(aiResponse.value.Values[0, 51], System.Globalization.CultureInfo.InvariantCulture); 
                    return model;
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));
                    Console.WriteLine(response.Headers.ToString());
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseContent);

                }
            }
            return null;
        }
        public string buyukSehirMi(string il)
        {
            //var result = "0";
            il = il.ToLower().Replace('ı', 'i');
            var result = _YapayZekaContext.YZ_BuyuksehirNufusRepository.Filter(w => w.Buyuksehir.ToLower().Trim() == il).FirstOrDefault();
            if (result != null)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        public Tuple<string, string> ilNufusVeYogunluk (string il)
        {
            il = il.ToLower().Replace('ı', 'i');
            var result = _YapayZekaContext.YZ_IlNufusRepository.Filter(w => w.IlAdi.ToLower().Trim() == il).FirstOrDefault();
            string nufus = "0";
            string yogunluk = "0";
            if(result!=null){
                nufus = result.ToplamNufus.ToString();
                yogunluk = result.NufusYogunluk.ToString();
            }
            return Tuple.Create(nufus, yogunluk);
        }
        public Tuple<string, string,string> ilceNufusVeYogunluk(string il,string ilce)
        {
            il=il.ToLower().Replace('ı', 'i');
            ilce = ilce.ToLower().Replace('ı', 'i');
            var result = _YapayZekaContext.YZ_IlceNufusRepository.Filter(w => w.IlAdi.ToLower()== il && w.IlceAdi.ToLower() == ilce).FirstOrDefault();
            string nufus = "0";
            string yogunluk = "0";
            string ilIleUzaklik = "0";
            if (result != null)
            {
                nufus = result.ToplamNufus.ToString();
                yogunluk = result.NufusYogunluk.ToString();
                ilIleUzaklik = result.IleUzaklik.ToString();
            }
            return Tuple.Create(nufus, yogunluk, ilIleUzaklik);
        }
    }

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class ResponseTable
    {
        public string[] ColumnNames { get; set; }
        public string[] ColumnTypes { get; set; }
        public string[,] Values { get; set; }
    }


    public class AIRespone
    {
        public string type { get; set; }
        public ResponseTable value { get; set; }
    }

    public class AIResponseResul
    {
        Dictionary<string, ResponseTable> Results { get; set; }
    }
}
