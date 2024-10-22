using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Factory
{
    public static class AutoPoliceTransferReaderFactory
    {
        public static IHDIOtomatikPoliceTransferReader createHDIReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi,string TahsilatDosyaYolu="")
        {
            return new HDIOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
        }
        public static IMAPFREOtomatikPoliceTransferReader createMapfreReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "")
        {
            return new MapfreOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
        }
        public static IMAPFREDASKOtomtikPoliceTranfer createMapfreDaskReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            return new MAPFREDASKOtomtikPoliceTranfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
        }
        public static IMAPFREOtomatikTahsilatPoliceTransferReader createTahsilatMapfreReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            return new MAPFREOtomatikTahsilatPoliceTransferReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
        }
        public static IAXAOtomatikPoliceTransfer createAXAReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            return new AXAOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
        }
        public static IETHICAOtomatikPoliceTransfer createETHICAReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "")
        {
            return new ETHICAOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
        }
        public static IDOGAOtomatikPoliceTransfer createDOGAReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "")
        {
            return new DOGAOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
        }
        public static IRaySigortaOtomatikPoliceTransfer createRAYReader(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "", string partajNo = "")
        {
            return new RaySigortaOtomatikPoliceTransfer(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu, partajNo);
        }
    }
}
