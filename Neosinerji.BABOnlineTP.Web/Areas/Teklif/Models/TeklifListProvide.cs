using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class TeklifProvider
    {

       
        public static List<SelectListItem> KrediKartiAylar()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "01", Text = "01" },
                new SelectListItem() { Value = "02", Text = "02" },
                new SelectListItem() { Value = "03", Text = "03" },
                new SelectListItem() { Value = "04", Text = "04" },
                new SelectListItem() { Value = "05", Text = "05" },
                new SelectListItem() { Value = "06", Text = "06" },
                new SelectListItem() { Value = "07", Text = "07" },
                new SelectListItem() { Value = "08", Text = "08" },
                new SelectListItem() { Value = "09", Text = "09" },
                new SelectListItem() { Value = "10", Text = "10" },
                new SelectListItem() { Value = "11", Text = "11" },
                new SelectListItem() { Value = "12", Text = "12" },
            });

            return list;
        }

        public static List<SelectListItem> KrediKartiYillar()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "2013", Text = "2013" },
                new SelectListItem() { Value = "2014", Text = "2014" },
                new SelectListItem() { Value = "2015", Text = "2015" },
                new SelectListItem() { Value = "2016", Text = "2016" },
                new SelectListItem() { Value = "2017", Text = "2017" },
                new SelectListItem() { Value = "2018", Text = "2018" },
                new SelectListItem() { Value = "2019", Text = "2019" },
                new SelectListItem() { Value = "2020", Text = "2020" },
                new SelectListItem() { Value = "2021", Text = "2021" },
                new SelectListItem() { Value = "2022", Text = "2022" },
                new SelectListItem() { Value = "2023", Text = "2023" },
                new SelectListItem() { Value = "2024", Text = "2024" },
            });

            return list;
        }

        public static List<SelectListItem> OdemeTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                //new SelectListItem(){Text="Blokeli Kredi Kart",Value="6"},
                  new SelectListItem(){Text=babonline.Credit_Card,Value="2"},
                new SelectListItem(){Text=babonline.Cash,Value="1"},
                 //new SelectListItem(){Text=babonline.Transfer,Value="3"}
            });

            return list;
        }
        public static List<SelectListItem> OdemeTipleriReasuror()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="Blokeli Kredi Kart",Value="6"},
                new SelectListItem(){Text=babonline.Credit_Card,Value="2"},
                new SelectListItem(){Text=babonline.Cash,Value="1"},
                new SelectListItem(){Text=babonline.Credit_Card + " ECB",Value="20"},
                new SelectListItem(){Text=babonline.Credit_Card + " " + babonline.InsuranceCompany,Value="21"}, // Sigorta Şirketi
                new SelectListItem(){Text=babonline.Transfer+" ECB",Value="22"},
                new SelectListItem(){Text=babonline.Transfer + " " + babonline.InsuranceCompany,Value="23"}, // Sigorta Şirketi
                new SelectListItem(){Text=babonline.Transfer,Value="3"}
            });

            return list;
        }
        public static List<SelectListItem> TSSOdemeTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Credit_Card,Value="2"}
            });

            return list;
        }

        public static List<SelectListItem> DaskKurumTipleri()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text=babonline.Bank,Value="1"},
                    new SelectListItem(){ Text=babonline.FinancialInstitution,Value="2"}
            });

            return Items;
        }

        public static List<SelectListItem> DaskDovizKodlari()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text=babonline.TL_TurkishLiras, Value="1"},
                    new SelectListItem(){ Text=babonline.USD_AmericanDolar, Value="2"},
                    new SelectListItem(){ Text=babonline.EUR_Euro, Value="3"}
            });

            return Items;
        }

        public static List<SelectListItem> DaskBinaYapiTazrlari()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.SteelConcreteFramework},
                new SelectListItem(){ Value="2", Text=babonline.BrickMasonry},
                new SelectListItem(){ Value="3", Text=babonline.Other}
            });

            return Items;
        }

        public static List<SelectListItem> KonutBinaYapiTazrlari()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.SteelConcreteFramework},
                new SelectListItem(){ Value="2", Text=babonline.BrickMasonry},
                new SelectListItem(){ Value="3", Text=babonline.HALF_ASI_STONE}
            });

            return Items;
        }

        public static List<SelectListItem> BosKalmaSureleri()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.Month1},
                new SelectListItem(){ Value="2", Text=babonline.Month2},
                new SelectListItem(){ Value="3", Text=babonline.Month3},
                new SelectListItem(){ Value="4", Text=babonline.Month4},
                new SelectListItem(){ Value="5", Text=babonline.Month5},
                new SelectListItem(){ Value="6", Text=babonline.Month6},
                new SelectListItem(){ Value="7", Text=babonline.Month7},
                new SelectListItem(){ Value="8", Text=babonline.Month8},
                new SelectListItem(){ Value="9", Text=babonline.Month9},
                new SelectListItem(){ Value="10", Text=babonline.Month10},
                new SelectListItem(){ Value="11", Text=babonline.Month11}
            });

            return Items;
        }

        public static List<SelectListItem> CatiTipleri()
        {
            List<SelectListItem> Items = new List<SelectListItem>();
            //ÇELİK KONS. HARİÇ BÜTÜN BİN.
            //ÇELİK KONS. VE DİĞERLERİ 
            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.STEEL_CONSTRUCTION_THOUSANDS_EXCEPT_ALL},
                new SelectListItem(){ Value="2", Text=babonline.STEEL_CONSTRUCTION_AND_OTHERS},

            });

            return Items;
        }

        public static List<SelectListItem> KatTipleri()
        {
            List<SelectListItem> Items = new List<SelectListItem>();
            //BODRUM KAT  
            //ZEMİN KAT   
            //DİĞER KATLAR
            //ÜST KAT     
            //TÜM KATLAR  

            Items.AddRange(new SelectListItem[]{
               new SelectListItem(){ Value="1", Text=babonline.TheBasementFloor},
                new SelectListItem(){ Value="2", Text=babonline.GroundFloor},
                new SelectListItem(){ Value="3", Text=babonline.OtherFloors},
                new SelectListItem(){ Value="4", Text=babonline.Upstairs},
                new SelectListItem(){ Value="5", Text=babonline.AllFloors}

            });

            return Items;
        }

        public static List<SelectListItem> DaskBinaKatSayisi()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.BetweenFloor_01_04},
                new SelectListItem(){ Value="2", Text=babonline.BetweenFloor_05_07},
                new SelectListItem(){ Value="3", Text=babonline.BetweenFloor_08_19},
                new SelectListItem(){ Value="4", Text=babonline.Over20}
            });

            return Items;
        }

        public static List<SelectListItem> DaskBinaInsaYili()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.Preview_1975},
                new SelectListItem(){ Value="2", Text="1976 - 1996"},
                new SelectListItem(){ Value="3", Text="1997 - 1999"},
                new SelectListItem(){ Value="4", Text="2000 - 2006"},
                new SelectListItem(){ Value="5", Text=babonline.Post_2007}
            });

            return Items;
        }

        public static List<SelectListItem> DaskBinaKullanimSekli()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.Dwelling},
                new SelectListItem(){ Value="2", Text=babonline.Bureau},
                new SelectListItem(){ Value="3", Text=babonline.Commercial},
                new SelectListItem(){ Value="4", Text=babonline.Other}
            });
            return Items;
        }

        public static List<SelectListItem> DaskBinaHasarDurumu()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.Undamaged},
                new SelectListItem(){ Value="2", Text=babonline.LessDamaged},
                new SelectListItem(){ Value="3", Text=babonline.ModerateDamaged
                },
            });
            return Items;
        }

        public static List<SelectListItem> Dask_S_EttirenSifati()
        {
            List<SelectListItem> Items = new List<SelectListItem>();

            Items.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="1", Text=babonline.Owner},
                new SelectListItem(){ Value="2", Text=babonline.Tenant},
                new SelectListItem(){ Value="3", Text=babonline.InfifaRightsOwner},
                new SelectListItem(){ Value="4", Text=babonline.Manager},
                new SelectListItem(){ Value="5", Text=babonline.Relative},
                new SelectListItem(){ Value="6", Text=babonline.DainiLossPayee},
                new SelectListItem(){ Value="7", Text=babonline.Other}
            });
            return Items;
        }

        public static List<SelectListItem> UlkeTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="1",Text=babonline.Schengen},
                        new SelectListItem(){ Value="2",Text=babonline.Other}
            });

            return list;
        }

        public static List<SelectListItem> BireyTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="1",Text=babonline.Father},
                        new SelectListItem(){ Value="2",Text=babonline.Mother},
                        new SelectListItem(){ Value="3",Text=babonline.Child}
            });

            return list;
        }

        public static List<SelectListItem> SeyehatPlanlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="1",Text=babonline.Silver},
                        new SelectListItem(){ Value="2",Text=babonline.Gold},
                        new SelectListItem(){ Value="3",Text=babonline.Platinum}
            });

            return list;
        }

        public static List<SelectListItem> KimlikTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="1",Text=babonline.TC_Number},
                        new SelectListItem(){ Value="2",Text=babonline.Passport},
            });

            return list;
        }

        public static List<SelectListItem> Uyruklar()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="0",Text="TC"},
                        new SelectListItem(){ Value="1",Text=babonline.Foreign},
            });

            return list;
        }

        public static List<SelectListItem> HanAptFabTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                        new SelectListItem(){ Value="1",Text=babonline.Apartment},
                        new SelectListItem(){ Value="2",Text=babonline.inn},
                        new SelectListItem(){ Value="3",Text=babonline.Bazaar},
                        new SelectListItem(){ Value="4",Text=babonline.Passage},
                        new SelectListItem(){ Value="5",Text=babonline.Other},
            });

            return list;
        }

        public static List<SelectListItem> HesaplamaSecenegiTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Yıllık Prim Belirterek"},
                new SelectListItem(){ Value="2", Text="Süre Sonu Birikim Tutarı Belirterek"},

            });

            return list;
        }

        public static List<SelectListItem> GelirVergisiTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="15.00%"},
                new SelectListItem(){ Value="2", Text="20.00%"},
                new SelectListItem(){ Value="3", Text="27.00%"},
                new SelectListItem(){ Value="4", Text="35.00%"},
                new SelectListItem(){ Value="5", Text="Beyan Edilmemiştir."},
            });

            return list;
        }

        public static List<SelectListItem> ParaBirimleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1" ,Text="EUR"},
                new SelectListItem(){ Value="2", Text="USD"},
            });

            return list;
        }

        public static List<SelectListItem> PrimDonemleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Aylık"},
                new SelectListItem(){ Value="2", Text="3 Aylık"},
                new SelectListItem(){ Value="3", Text="6 Aylık"},
                new SelectListItem(){ Value="4", Text="Yıllık"},
            });

            return list;
        }

        // Turuncu Elma hesaplama Seçeneği
        public static List<SelectListItem> TuruncuElmaHesaplamaSecenegiTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Ana Teminat"},
                new SelectListItem(){ Value="2", Text="Yıllık Prim"},

            });

            return list;
        }

        //Prim İadeli Liste Seçenekleri
        public static List<SelectListItem> PrimIadeliHesaplamaSecenegiTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Yillik Prim Belirterek"},
                new SelectListItem(){ Value="2", Text="Vefat Teminatı Belirterek"},

            });

            return list;
        }

        public static List<SelectListItem> PrimIadeliSureSecenegiTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="12"},
                new SelectListItem(){ Value="2", Text="15"},
                new SelectListItem(){ Value="3", Text="18"},
                new SelectListItem(){ Value="4", Text="24"},
                new SelectListItem(){ Value="5", Text="30"},

            });

            return list;
        }

        //OdemeGuvence Liste Seçenekleri
        public static List<SelectListItem> ParaBirimleriTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="EUR"},
                new SelectListItem(){ Value="2", Text="USD"},
                new SelectListItem(){ Value="3", Text="TL"},

            });

            return list;
        }
        //OdemeGuvence Liste Seçenekleri
        public static List<SelectListItem> ParaBirimTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="TL", Text="TL"},
                new SelectListItem(){ Value="USD", Text="USD"},
                new SelectListItem(){ Value="EUR", Text="EUR"},
                new SelectListItem(){ Value="GBP", Text="GBP"},
                new SelectListItem(){ Value="AED", Text="AED"},

            });

            return list;
        }

        public static List<SelectListItem> EkTeminatTipleriAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Var"},
                new SelectListItem(){ Value="2", Text="Yok"},


            });

            return list;
        }

        //PrimIadeli2 Liste Seçenekleri
        public static List<SelectListItem> YillikPrimTutarlariAEGON()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="480"},
                new SelectListItem(){ Value="2", Text="960"},
                new SelectListItem(){ Value="3", Text="1440"},

            });

            return list;
        }

        //Metlife
        public static List<SelectListItem> MeslekTipleriMetlife()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){ Value="", Text=babonline.PleaseSelect},
                new SelectListItem(){ Value="1", Text="Yazılım Müdürü"},
                new SelectListItem(){ Value="2", Text="Yazılım Uzmanı"},
                new SelectListItem(){ Value="3", Text="Danışman"},

            });

            return list;
        }

        public static List<SelectListItem> KritikHastalikSureSecenegiTipleri()
        {

            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},

                new SelectListItem(){ Value="1", Text="1"},
                new SelectListItem(){ Value="2", Text="2"},
                new SelectListItem(){ Value="3", Text="3"},
                new SelectListItem(){ Value="4", Text="4"},
                new SelectListItem(){ Value="5", Text="5"},
                new SelectListItem(){ Value="6", Text="6"},
                new SelectListItem(){ Value="7", Text="7"},
                new SelectListItem(){ Value="8", Text="8"},
                new SelectListItem(){ Value="9", Text="9"},
                new SelectListItem(){ Value="10", Text="10"},
                new SelectListItem(){ Value="11", Text="11"},
                new SelectListItem(){ Value="12", Text="12"},
            });

            return list;
        }

        public static List<SelectListItem> KritikHastalikTeminatTutarlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[]{
            new SelectListItem(){ Value="", Text=babonline.PleaseSelect},

                new SelectListItem(){ Value="1", Text="20.000"},
                new SelectListItem(){ Value="2", Text="50.000"},
                new SelectListItem(){ Value="3", Text="75.000"},
                new SelectListItem(){ Value="4", Text="Diğer"},

            });

            return list;
        }

        public static List<SelectListItem> KritikHastalikOdemeTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="Hesap",Value="1"},
                new SelectListItem(){Text=babonline.Credit_Card,Value="2"},
                new SelectListItem(){Text="Diğer Banka Kredi Kartı",Value="3"}
            });

            return list;
        }

        public static List<SelectListItem> GroupamaMeslekGrupKodlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "DOKTOR/DOKTOR EMEKLİSİ" },
                new SelectListItem() { Value = "2", Text = "DİŞ HEKİMİ/DİŞ HEKİMİ EMEKLİSİ" },
                new SelectListItem() { Value = "3", Text = "ECZACI/ECZACI EMEKLİSİ" },
                new SelectListItem() { Value = "4", Text = "MÜHENDİS/MÜHENDİS EMEKLİSİ" },
                new SelectListItem() { Value = "5", Text = "MİMAR/MİMAR EMEKLİSİ" },
                new SelectListItem() { Value = "6", Text = "ÖĞRETMEN ÜYESİ/ ÖĞRETMEN ÜYESİ EMEKLİSİ" },
                new SelectListItem() { Value = "7", Text = "ÖĞRETMEN/ÖĞRETMEN EMEKLİSİ" },
                new SelectListItem() { Value = "8", Text = "DEVLET MEMURU/DEVLET MEMURU EMEKLİSİ" },
                new SelectListItem() { Value = "9", Text = "T.S.K. SUBAY/ T.S.K. SUBAY EMEKLİSİ" },
                new SelectListItem() { Value = "10", Text = "T.S.K. ASTSUBAY/ T.S.K. ASTSUBAY EMEKLİSİ" },
                new SelectListItem() { Value = "11", Text = "ANONİM ŞİRKET ÇALIŞANI" },
                new SelectListItem() { Value = "14", Text = "VETERİNER/VETERİNER EMEKLİSİ" },
                new SelectListItem() { Value = "15", Text = "UZMAN ÇAVUŞ/UZMAN ÇAVUŞ EMEKLİSİ" },
            });

            return list;
        }

        public static List<SelectListItem> GroupamaYakinlikDereceleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "EŞLER ARASI GEÇİŞ" },
                new SelectListItem() { Value = "2", Text = "ŞİRKET ORTAĞINDAN ŞİRKETE GEÇİŞ" },
                new SelectListItem() { Value = "3", Text = "ESKİ ARAÇ SATILDI" },
                new SelectListItem() { Value = "4", Text = "SÜRE AŞIMI" },
            });

            return list;
        }
        public static List<SelectListItem> GroupamaRizikoFiyatlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },   });
            int sayac = 0;
            for (int i = 1; i < 51; i++)
            {
                sayac += 5;
                list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = i.ToString(), Text = " % "+  sayac.ToString() },
            });
            }
            return list;
        }

        public static List<SelectListItem> GroupamaYHIMSSecimleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "YHİMS MADDİ / BEDENİ AYRIMLI" },
                new SelectListItem() { Value = "2", Text = "YHİMS YOK" },
                new SelectListItem() { Value = "3", Text = "YHİMS MADDİ / BEDENİ TEK LİMİT" },
            });

            return list;
        }

        public static List<SelectListItem> GulfIkameTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = GULF_IkameTurleri.YediGun },
                new SelectListItem() { Value = "2", Text = GULF_IkameTurleri.OndortGun },
                new SelectListItem() { Value = "3", Text = GULF_IkameTurleri.OtuzGunServisParcaDahil},
            });

            return list;
        }

        public static List<SelectListItem> GulfYakitTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "LPG/Hibrid" },
                new SelectListItem() { Value = "2", Text = "Elektrik" },
                new SelectListItem() { Value = "3", Text = "Benzin/Dizel"},
            });

            return list;
        }
        public static List<SelectListItem> GulfHukuksalKorumaBedelleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "5,000" },
                new SelectListItem() { Value = "2", Text = "10,000" }
            });

            return list;
        }

        public static List<SelectListItem> ErgoMeslekGrupKodlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "156613", Text = "AKADEMİSYEN" },
                new SelectListItem() { Value = "156616", Text = "A.Ş ÇALIŞANI" },
                new SelectListItem() { Value = "156615", Text = "DEVLET MEMURU" },
                new SelectListItem() { Value = "156620", Text = "DOKTOR" },
                new SelectListItem() { Value = "156614", Text = "ÖĞRETMEN" },
                new SelectListItem() { Value = "156617", Text = "SUBAY-ASTSUBAY" }
            });

            return list;
        }

        public static List<SelectListItem> AxaHayatTeminatLimitleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "5.000" },
                new SelectListItem() { Value = "2", Text = "7.000" }
            });

            return list;
        }
        public static List<SelectListItem> AxaAsistansHizmetleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "T", Text = "Turasist" },
                new SelectListItem() { Value = "I", Text = "IPA" }
            });

            return list;
        }
        public static List<SelectListItem> AxaIkameSecimleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "3", Text = "3 GÜN" },
                new SelectListItem() { Value = "7", Text = "7 GÜN" },
                new SelectListItem() { Value = "10", Text = "10 GÜN" },
                new SelectListItem() { Value = "15", Text = "15 GÜN" },
                new SelectListItem() { Value = "IIS", Text = "15 GÜN LÜKS ARAÇ" }
            });

            return list;
        }

        public static List<SelectListItem> AxaOnarimSecimleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "Mavi Ürünün/Orjinal Parça" },
                new SelectListItem() { Value = "2", Text = "Lacivert Ürünün/Orjinal Parça" }
            });

            return list;
        }

        public static List<SelectListItem> AxaYeniDegerKlozlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "0", Text = "Hayir" },
                new SelectListItem() { Value = "2", Text = "Evet (Sadece sıfır yaş araçlar için geçerlidir.)" },
                new SelectListItem() { Value = "1", Text = "Evet (Sadece sıfır yaş araçlar için geçerlidir.)" }
            });

            return list;
        }
        public static List<SelectListItem> AxaDepremSelKoasuranslari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "0", Text = "Yoktur" },
                new SelectListItem() { Value = "2", Text = "%25 Koasürans" }
            });

            return list;
        }
        public static List<SelectListItem> AxaMuafiyetTutarlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "500 USD" },
                new SelectListItem() { Value = "2", Text = "750 USD" },
                new SelectListItem() { Value = "3", Text = "1.000 USD" },
                new SelectListItem() { Value = "4", Text = "MUAFİYETSİZ" }
            });

            return list;
        }
        public static List<SelectListItem> AxaSorumlulukLimitleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
               new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "5.000 TL" },
                new SelectListItem() { Value = "2", Text = "Hayır" },
                new SelectListItem() { Value = "3", Text = "10.000 TL" },
                new SelectListItem() { Value = "4", Text = "15.000 TL" },
                new SelectListItem() { Value = "5", Text = "20.000 TL" },
                new SelectListItem() { Value = "6", Text = "30.000 TL" },
                new SelectListItem() { Value = "7", Text = "40.000 TL" },
                new SelectListItem() { Value = "8", Text = "50.000 TL" },
            });

            return list;
        }

        public static List<SelectListItem> ErgoServisTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
               new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                new SelectListItem() { Value = "1", Text = "TÜM SERVİSLER" },
                new SelectListItem() { Value = "2", Text = "TÜM ANLAŞMALI SERVİSLER" },
                new SelectListItem() { Value = "3", Text = "SADECE ANLAŞMALI ÖZEL SERVİSLER" },
                new SelectListItem() { Value = "4", Text = "(%2 Muafiyet) TÜM ANLAŞMALI SERVİSLER" },
            });

            return list;
        }

        public static List<SelectListItem> TurkNipponKaskoServisTurleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = "TNS Anlaşmalı Servis" },
                new SelectListItem() { Value = "2", Text = "Tüm Servisler" }
            });
            return list;
        }
        public static List<SelectListItem> TurkNipponMuafiyetTutarlari()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "YOK" },
                new SelectListItem() { Value = "1", Text = "250 TL" },
                new SelectListItem() { Value = "2", Text = "500 TL" },
                new SelectListItem() { Value = "3", Text = "750 TL" },
                new SelectListItem() { Value = "4", Text = "10.000 TL" },
                new SelectListItem() { Value = "6", Text = "15.000 TL" }
            });
            return list;
        }
    }
       
}
