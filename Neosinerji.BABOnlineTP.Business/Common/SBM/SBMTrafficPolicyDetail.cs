using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.Common.SBM
{
    public class SBMTrafficPolicyDetail
    {
        #region Police Bilgileri
        public string SbmTramerNo;
        public string AcenteNo;
        public string YenilemeNo;
        public string EkTuru;
        public string TCKimlikNo;
        public string PasaportNo;
        public string GuncellemeTarihi;
        public string BitisTarihi;
        public string EkBaslangicTarihi;
        public string SistemTarihi;
        public string TahakkukIptal;

        public string SigortaSirketi;
        public string PoliceNo;
        public string PoliceEkNo;
        public string Sigortali;
        public string VergiKimlikNo;
        public string OlusturmaTarihi;
        public string BaslangicTarihi;
        public string TanzimTarihi;
        public string EkBitisTarihi;
        public string SistemSaati;
        public string HavuzaDahil;
        #endregion

        #region Sigortali Bilgileri
        public string Adres;
        public string IkametIlce;
        public string IkametIl;
        #endregion

        #region Onceki Police Bilgileri
        public string OncekiSigortaSirketi;
        public string OncekiPoliceNo;
        public string OncekiAcenteNo;
        public string OncekiYenilemeNo;
        #endregion

        #region Arac Bilgileri
        public string AracTarifeGrupKodu;
        public string SasiNo;
        public string ModelYili;
        public string AracTipi;
        public string YolcuSayisi;
        public string ImalatYeri;
        public string SilindirHacmi;
        public string YukKapasitesi;
        public string AyaktaYolcuAdedi;

        public string Plaka;
        public string MotorNo;
        public string AracMarkasi;
        public string TescilTarihi;
        public string KullanimSekli;
        public string Renk;
        public string MotorGucu;
        public string TrafigeCikisTarihi;
        #endregion

        #region Teminat ve Prim Bilgileri
        public string AracBasinaMaddiTeminat;
        public string KisiBasinaTedaviTeminati;
        public string KisiBasinaOlumSakatlikTeminati;
        public string TemelTarifePrimi;
        public string GiderVergisi;
        public string TrafikHizmetleriGelistirmeFonu;
        public string HavuzPrimi;

        public string KazaBasinaMaddiTeminat;
        public string KazaBasinaTedaviTeminati;
        public string KazaBasinaOlumSakatlikTeminati;
        public string NetPrim;
        public string GarantiFon;
        public string BrutPrim;
        #endregion

        #region Indirim / Surprim Bilgileri
        public string HasarsizlikIndirimi;
        public string HasarlılıkSurprim;
        public string TarifeBasamakKodu;
        public string GecikmedenDolayiSurprim;
        public string ZKYTMSIndirim;
        #endregion

        #region Önceki Poliçe Hasar Belge Bilgileri
        public string TramerBelgeNo;
        public string TramerBelgeTarihi;
        #endregion

        //#region Mevcut Poliçe Hasar Belge Bilgileri

        //#endregion

    }
}
