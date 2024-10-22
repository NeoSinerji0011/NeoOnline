using Neosinerji.BABOnlineTP.Business.turknippon.dask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK
{
    public interface ITURKNIPPONDask : ITeklif
    {
        ReturnAdresModel GetAdresDetay(long UAVTKodu);
        List<ListOutput> GetIlList();
        List<ListOutput> GetIlceList(int ilKodu);
        List<ListOutput> GetBeldeList(int ilceKodu);
        List<ListOutput> GetMahalleList(int beldeKodu);
        List<ListOutput> GetCaddeSokakList(int mahalleKodu);
        List<ListOutput> GetBinaList(int caddeSokakKodu);
        List<ListOutput> GetBagimsizBolumList(int binaKodu);
        PoliceBilgileri GetPoliceDetay(long PolNo, int? YenilemeNo, int? EkNo);
    }
    public class ReturnAdresModel
    {
        public int IlKodu { get; set; }
        public string IlAdi { get; set; }
        public int IlceKodu { get; set; }
        public string IlceAdi { get; set; }
        public long BeldeKodu { get; set; }
        public string BeldeAdi { get; set; }
        public int MahalleKodu { get; set; }
        public string MahalleAdi { get; set; }
        public int CaddeSokakKodu { get; set; }
        public string CaddeAdi { get; set; }
        public string SokakAdi { get; set; }
        public string SiteAdi { get; set; }
        public string ApartmanAdi { get; set; }
        public long BinaKodu { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public string Kat { get; set; }
        public string Ada { get; set; }
        public string Pafta { get; set; }
        public string Parsel { get; set; }
        public string SayfaNo { get; set; }
        public string UAVTAdresKodu { get; set; }
        public string Hata { get; set; }
    }
    public class PoliceBilgileri
    {
        public string SigortaliAdi { get; set; }
        public string SigortaliSoyadi { get; set; }
        public string AcikAdres { get; set; }
        public string Aciklama { get; set; }
        public string Hata { get; set; }
        public string DigerSirketYenilemesiMi { get; set; }
        public string BinaninToplamKatSayisi { get; set; }
        public string RizikoDepremBolgesi { get; set; }
        public string ReferansPoliceNumarasi { get; set; }
        public string DainiMurteinKrediBitisTar { get; set; }
        public string DainiMurteinBankaKodu { get; set; }
        public string DainiMurteinAdi { get; set; }
        public string DainiMurteinKrediDovizCinsi { get; set; }
        public string DainiMurteinFinansKurumu { get; set; }
        public string DainiMurteinVarMi { get; set; }
        public string DainiMurteinKrediTutari { get; set; }
        public string DainiMurteinSubeKodu { get; set; }
        public string DainiMurteinKrediSozlesmeNo { get; set; }
        public string DainiMurteinAdiUnvani { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string OncekiSirketPolNo { get; set; }
        public string OncekiSirketKodu { get; set; }
        public string OncekiZeylNo { get; set; }
        public string DaireM2 { get; set; }
        public string BinaInsaTarzi { get; set; }
        public string BinaInsaYili { get; set; }
        public string DaireKullanimSekli { get; set; }
        public string SigortaEttirenSifati { get; set; }
        public string HasarliMi { get; set; }




        public ReturnAdresModel adres { get; set; }
    }
}
