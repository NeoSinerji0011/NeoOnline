using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ICRService
    {
        #region Araç ek soru
        List<CR_AracEkSoru> GetAracEkSoru(int tumKodu, string soruTipi);
        #endregion

        #region Araç Kullanım Tarzı
        List<CR_KullanimTarzi> GetKullanimTarzlari(int tumKodu);
        List<AracKullanimTarziServisModel> GetKullanimTarzi(int tumKodu);
        CR_KullanimTarzi GetKullanimTarzi(int tumKodu, string kullanimTarziKodu, string kod2);
        #endregion

        #region Araç Grup Kodu
        List<CR_AracGrup> GetAracGruplari(int tumKodu);
        List<AracKullanimTarziServisModel> GetAracGrupKodlari(int tumKodu);
        CR_AracGrup GetAracGrupKodu(int tumKodu, string kullanimTarziKodu, string kod2);
        #endregion

        #region İl İlçe
        CR_IlIlce GetIlIlceByCr(int tumKodu, string crIlKodu, string crIlceKodu);
        #endregion

        #region Ülke
        List<CR_Ulke> GetUlkeler(int tumKodu);
        CR_Ulke GetUlke(int tumKodu, string ulkeKodu);
        #endregion

        #region Tescil İl İlçe
        CR_TescilIlIlce GetTescilIlIlce(int tumKodu, string ilKodu, string ilceKodu);
        List<KeyValueItem<string, string>> GetTescilIlList();
        List<KeyValueItem<string, string>> GetTescilIlceList(string ilKodu);
        #endregion

        #region Trafik IMM & FK
        CR_TrafikIMM GetTrafikIMM(int tumKodu, short kademe);
        CR_TrafikFK GetTrafikFK(int tumKodu, short kademe);
        List<KeyValueItem<short, string>> GetTrafikIMM();
        List<KeyValueItem<short, string>> GetTrafikFK();

        TrafikIMM GetTrafikIMMBedel(int id, string kullanimTarziKod, string kod2);
        CR_TrafikIMM GetCRTrafikIMMBedel(int TUMKodu, decimal? BedeniSahis, decimal? Kombine, string kullanimTarziKod, string kod2);

        TrafikFK GetTrafikFKBedel(int id, string kullanimTarziKod, string kod2);
        CR_TrafikFK GetCRTrafikFKBedel(int TUMKodu, decimal? Vefat, decimal? Tedavi, decimal? Kombine, string kullanimTarziKod, string kod2);

        List<KeyValueItem<string, string>> GetTrafikIMMListe(string kullanimtarzikodu, string kod2);
        List<KeyValueItem<string, string>> GetTrafikFKListe(string kullanimtarzikodu, string kod2);

        bool IMMKombineManeviDahiMi(int id);
        #endregion

        #region Kasko IMM & FK
        CR_KaskoIMM GetKaskoIMM(int tumKodu, short kademe, string kullanimtarzikodu);
        CR_KaskoFK GetKaskoFK(int tumKodu, short kademe, string kullanimtarzikodu);
        List<KeyValueItem<string, string>> GetKaskoFKList(int tumkodu, string kullanimtarzikodu, string kod2);
        List<KeyValueItem<string, string>> GetKaskoIMMList(int tumkodu, string kullanimtarzikodu, string kod2);

        KaskoIMM GetKaskoIMMBedel(int id, string kullanimTarziKod, string kod2);
        CR_KaskoIMM GetCRKaskoIMMBedel(int TUMKodu, decimal? BedeniSahis, decimal? Kombine, string kullanimTarziKod, string kod2);

        KaskoFK GetKaskoFKBedel(int id, string kullanimTarziKod, string kod2);
        CR_KaskoFK GetCRKaskoFKBedel(int TUMKodu, decimal? Vefat, decimal? Tedavi, decimal? Kombine, string kullanimTarziKod, string kod2);

        List<KeyValueItem<string, string>> GetKaskoIMMListe(string kullanimtarzikodu, string kod2);
        List<KeyValueItem<string, string>> GetKaskoFKListe(string kullanimtarzikodu, string kod2);
        #endregion

        #region Kasko AMS (MAPFRE)
        CR_KaskoAMS GetKaskoAMS(int tumKodu, int tvmKodu, string amsKodu);
        List<KeyValueItem<string, string>> GetKaskoAMSList(int tumKodu, int tvmKodu, string kullanimtarzikodu, string kod2);
        #endregion

        #region Kasko İkame Türü (MAPFRE)
        CR_KaskoIkameTuru GetKaskoIkameTuru(int tumKodu, string tarifeKodu, string ikameTuruKodu);
        List<KeyValueItem<string, string>> GetKaskoIkameTuruList(int tumKodu, string tarifeKodu);
        #endregion

        #region Kasko Dain-i Murtein (MAPFRE) 
        List<CR_KaskoDM> GetKaskoDMListe(int tumKodu, int kurumTipi);
        CR_KaskoDM GetKaskoDM(int tumKodu, int kurumTipi, string kurumKodu);
        CR_KaskoDM GetKaskoDMAd(int tumKodu, string kurumAdi);
        #endregion

        #region TUM Müşteri No
        void InsertTUMMusteri(int tumKodu, int musteriKodu, string tumMusteriNo);
        string GetTUMMusteriKodu(int tumKodu, int musteriKodu);
        #endregion

        #region Dask

        List<DaskSubeler> GetListDaskSubeler(int KurumKodu);
        DaskSubeler GetDaskSube(int KurumKodu, int SubeKodu);
        List<DaskKurumlar> GetListDaskKurumlar();
        DaskKurumlar GetDaskKurum(int KurumKodu);
        List<DaskBelde> GetListDaskBeldeler(int IlKodu, int IlceKodu);
        List<DaskIlce> GetListDaskIlceler(int IlKodu);
        List<DaskIl> GetListDaskIller();
        DaskIl GetDaskIl(int IlKodu);
        DaskIlce GetDaskIlce(int IlceKodu);
        DaskIlce GetDaskIlce(int Ilkodu, string IlceAdi);
        DaskBelde GetDaskBelde(int BeldeKodu);
        DaskBelde GetDaskBelde(int IlKodu, int IlceKodu, string BeldeAdi);

        #endregion

        #region Kredili hayat çarpan tablosu
        CR_KrediHayatCarpan GetKrediHayatCarpan(int tumKodu, string tipKodu, int musteriYas, int krediVade);
        #endregion

        #region Seyehat Sağlık

        List<UlkeKodlari> GetSeyehatUlkeleri(bool schengenMi);
        UlkeKodlari GetSeyehatUlkesi(string ulkeKodu);
        decimal GetPrimTutari(int SigortaliYasi, int SeyehatGunSayisi, bool schengenMi, byte? planKodu);

        #endregion

        #region Konut

        List<BelediyeIl> GetListBelediyeIl();
        List<Belediye> GetListBelediye(int IlKodu);
        Belediye GetBelediye(int IlKodu, int BelediyeKodu);
        BelediyeIl GetBelediyeIl(int IlKodu);
        List<DepremMuafiyet> GetListDepremMuafiyet();
        DepremMuafiyet GetDepremMuafiyet(int kod, string Value);
        string GetDepremMuafiyetText(int kod, string Value);

        #endregion

        #region Is YEri

        List<Istigal> GetListIstigal();
        Istigal GetIstigal(int kod);

        #endregion
    }
}
