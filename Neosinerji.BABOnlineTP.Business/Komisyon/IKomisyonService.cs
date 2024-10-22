using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Komisyon
{
    public interface IKomisyonService
    {
        List<TaliAcenteKomisyonOrani> TaliAcenteKomisyonListesi(List<TVMModel> taliAcenteler, List<TVMModel> disKaynak, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime gecerlilikBaslangicTarihi, int sayfa, int adet, out int toplam);

        List<TaliAcenteKomisyonOrani> TaliAcenteKademeliKomisyonListesi(List<TVMModel> taliAcenteler, List<TVMModel> disKaynak, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, int gecerliYil);

        bool TaliAcenteKomisyonListesiOlustur(List<TVMModel> taliAcenteler, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime gecerlilikBaslangicTarihi, decimal oran);

        bool TaliAcenteKademeliKomisyonListesiOlustur(List<TVMModel> taliAcenteler, int taliDisKaynakKodu, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, int gecerliYil, List<KomisyonKademeModel> kademeModel);

        TaliAcenteKomisyonOrani TaliAcenteKomisyonGuncelle(int komisyonOranId, DateTime gecerlilikBaslangicTarihi, decimal oran, out bool guncellendiMi);

        TaliAcenteKomisyonOrani TaliAcenteKademeliKomisyonGuncelle(int komisyonOranId, int gecerliYil, List<KomisyonKademeModel> kademeModel, out bool guncellendiMi);

        List<TaliAcenteKomisyonOrani> TaliAcenteKademeliKomisyonListesi(int komisyonOranId);

        List<PoliceGenel> TaliAcentePoliceGenelListesi(TaliKomisyonHesaplamaPoliceDurumu durum, List<TVMModel> taliAcenteler, List<TVMModel> disKaynaklar, List<BransModel> branslar, List<SigortaSirketiModel> sigortaSirketleri, DateTime tarihBaslangic, DateTime tarihBitis, PrimTipleri iptalZeylTahakkuk);

        decimal TaliAcenteKomisyonOrani(int taliTVMKodu, int bransKodu, string sigortaSirketKodu, DateTime gecerlilikTarihi, string policeNo);

        PoliceGenel TeklifGenelKomisyonGuncelle(int teklifGenelId, int? taliTVMKodu, int oncekiTaliKodu, decimal komisyonOrani, decimal komisyon, out bool guncellendiMi);

        //List<TaliAcenteKomisyonOrani> GetTaliAcenteKomisyon(int anaTVMKodu, int taliTVMKodu, string TUMBirlikKodu, int bransKodu);
        List<TaliAcenteKomisyonOrani> GetTaliAcenteKomisyon(int anaTVMKodu, int taliTVMKodu, int? disKaynakKodu, string TUMBirlikKodu, int bransKodu);

        decimal PoliceTVMGerceklesen(int tvmKoduTali, int donem, int bransKodu);

        bool PoliceUretimHedefGerceklesenGuncelle(PoliceGenel police, int oncekiTaliKodu);
    }
}
