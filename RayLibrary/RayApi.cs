using RaySigorta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RayWebService
{
    public class RayApi
    {
        public string AcenteNo { get; set; }
        public string IpNo { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }

        public RayApi(string AcenteNo, string IpNo, string KullaniciAdi, string Sifre)
        {
            this.AcenteNo = AcenteNo; // "5893"
            this.IpNo = IpNo; // "88.247.127.91"
            this.KullaniciAdi = KullaniciAdi; //"A5893001"
            this.Sifre = Sifre; // "Poyraz.1211"
        }
        public List<PoliceDetayOutputEntity> PoliceListe(DateTime OnayTarih)
        {
            List<RaySigorta.PoliceDetayOutputEntity> PoliceListe = new List<RaySigorta.PoliceDetayOutputEntity>();
            PoliceListesiOutputEntity sonuc = null;
            var logfile2 = AppDomain.CurrentDomain.BaseDirectory + @"\Files\errorlog.json";
            try
            {

                RaySigorta.PoliceListesiInputEntity policeListesiInputEntity = new RaySigorta.PoliceListesiInputEntity();
                policeListesiInputEntity.AcenteNo = this.AcenteNo;
                policeListesiInputEntity.OnayTarih = OnayTarih;
                RaySigorta.ServisKullaniciBilgileriNesne servisKullaniciBilgileriNesne = new RaySigorta.ServisKullaniciBilgileriNesne();
                servisKullaniciBilgileriNesne.IpNo = this.IpNo;
                servisKullaniciBilgileriNesne.KullaniciAdi = this.KullaniciAdi;
                servisKullaniciBilgileriNesne.Sifre = this.Sifre;
                servisKullaniciBilgileriNesne.Ortam = RaySigorta.ConstsLokasyon.Canli;
                servisKullaniciBilgileriNesne.ReferansNo = "1234567";
                policeListesiInputEntity.ServisKullaniciBilgileri = servisKullaniciBilgileriNesne;

                RaySigorta.TransferClient transferClient = new RaySigorta.TransferClient(RaySigorta.TransferClient.EndpointConfiguration.BasicHttpBinding_ITransfer, "https://apigw.raysigorta.com.tr/Transfer/Transfer.svc");
                var res1 = transferClient.OpenAsync();
                res1.Wait();
                var qwe = res1.Status;
                var res = transferClient.PoliceListeAsync(policeListesiInputEntity);

                res.Wait();
                sonuc = res.Result;
                var logfile = AppDomain.CurrentDomain.BaseDirectory + @"\Files\log.json";
                if (sonuc != null)
                    if (!sonuc.Basarilimi)
                        File.WriteAllText(logfile, this.IpNo + Environment.NewLine + sonuc.HataMesaji + Environment.NewLine + DateTime.Now);

                transferClient.CloseAsync().Wait();
            }
            catch (Exception ex)
            {
                File.WriteAllText(logfile2, ex.Message + Environment.NewLine + ex.StackTrace);
            }

            if (sonuc != null)
            {
                if (sonuc.Policeler != null)
                {
                    try
                    {
                        foreach (var item in sonuc.Policeler)
                        {
                            RaySigorta.PoliceDetayOutputEntity policeDetay = PoliceDetayRequest(Convert.ToInt32(item.PoliceNo.ToString()), item.UrunNo.ToString(), item.ZeyilNo, item.YenilemeNo);
                            PoliceListe.Add(policeDetay);
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logfile2, ex.Message + "----" + Environment.NewLine + ex.StackTrace);

                    }
                }
            }

            return PoliceListe;

        }
        public RaySigorta.PoliceDetayOutputEntity PoliceDetayRequest(int PoliceNo, string UrunKodu, int ZeyilNo, int YenilemeNo)
        {
            RaySigorta.PoliceDetayInputEntity policeDetayInputEntity = new RaySigorta.PoliceDetayInputEntity();
            policeDetayInputEntity.AcenteNo = this.AcenteNo;
            policeDetayInputEntity.PoliceNo = PoliceNo;
            policeDetayInputEntity.UrunKodu = UrunKodu;
            policeDetayInputEntity.ZeyilNo = ZeyilNo;
            policeDetayInputEntity.YenilemeNo = YenilemeNo;

            RaySigorta.ServisKullaniciBilgileriNesne servisKullaniciBilgileriNesne = new RaySigorta.ServisKullaniciBilgileriNesne();
            servisKullaniciBilgileriNesne.IpNo = this.IpNo;
            servisKullaniciBilgileriNesne.KullaniciAdi = this.KullaniciAdi;
            servisKullaniciBilgileriNesne.Sifre = this.Sifre;
            servisKullaniciBilgileriNesne.Ortam = RaySigorta.ConstsLokasyon.Canli;
            servisKullaniciBilgileriNesne.ReferansNo = "1234567";
            policeDetayInputEntity.ServisKullaniciBilgileri = servisKullaniciBilgileriNesne;

            RaySigorta.TransferClient transferClient = new RaySigorta.TransferClient(RaySigorta.TransferClient.EndpointConfiguration.BasicHttpBinding_ITransfer, "https://apigw.raysigorta.com.tr/Transfer/Transfer.svc");
            var res1 = transferClient.OpenAsync();
            res1.Wait();
            var qwe = res1.Status;
            var res = transferClient.PoliceDetayAsync(policeDetayInputEntity);
            res.Wait();
            var logfile = AppDomain.CurrentDomain.BaseDirectory + @"\Files\log.json";
            if (res.Result != null)
                try
                {
                    if (!res.Result.Basarilimi)
                        File.AppendAllText(logfile, this.IpNo + Environment.NewLine + res.Result.HataMesaji + Environment.NewLine + DateTime.Now);
                }
                catch (Exception)
                { }


            RaySigorta.PoliceDetayOutputEntity sonuc = res.Result;
            transferClient.CloseAsync().Wait();
            return sonuc;
        }
    }
}
