using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITUMService
    {
        #region TUM Service
        DataTableList PagedList(TUMListe tumListe);

        List<TUMDetay> GetListTUMDetay();
        List<TUMDetay> GetNeoConnectTUMList();
        TUMDetay GetDetay(int kodu);
        TUMDetay CreateDetay(TUMDetay detay);
        void UpdateDetay(TUMDetay detay);
        string GetTumUnvan(int kodu);

        int GetTUMKodu(string TUMBirlikKodu);
        string GetTUMVknKodu(string TUMBirlikKodu);

        #endregion

        #region Bağlantı Service
        TUMIPBaglanti GetTUMIPBaglanti(int siraNo, int tumKodu);
        List<TUMIPBaglanti> GetListIPBaglanti();
        List<TUMIPBaglanti> GetListIPBaglanti(int tumKodu);
        TUMIPBaglanti CreateIPBaglanti(TUMIPBaglanti baglanti);
        bool UpdateItem(TUMIPBaglanti baglanti);
        void DeleteBaglanti(int baglantiKodu, int tumKodu);

        DataTableList PagedListIPBaglanti(DataTableParameters<TUMIPBaglanti> baglantiList, int tumKodu);
        #endregion

        #region Notlar Service
        TUMNotlar GetTUMNot(int siraNo, int tumKodu);
        List<TUMNotlar> GetListNotlar();
        List<TUMNotlar> GetListNotlar(int tvmKodu);
        TUMNotlar CreateNot(TUMNotlar not);
        bool UpdateItem(TUMNotlar not);
        void DeleteNot(int NotKodu, int tumKodu);

        DataTableList PagedListNot(DataTableParameters<TUMNotlar> notList, int tumKodu);
        #endregion

        #region Dokumanlar Service
        TUMDokumanlar GetTUMDokuman(int siraNo, int tumKodu);
        List<TUMDokumanlar> GetListDokumanlar();
        List<TUMDokumanlar> GetListDokumanlar(int tumKodu);
        TUMDokumanlar CreateDokuman(TUMDokumanlar dokuman);
        bool UpdateItem(TUMDokumanlar dokuman);
        void DeleteDokuman(int dokumanKodu, int tumKodu);
        int GetTumDokumanSiraNo(int tumKodu);
        bool CheckedFileName(string fileName);

        DataTableList PagedListDokuman(DataTableParameters<TUMDokumanlar> dokumanList, int tumKodu);
        #endregion

        #region BankaHesaplari Service
        TUMBankaHesaplari GetTUMBankaHesap(int SiraNo, int tumKodu);
        List<TUMBankaHesaplari> GetListTUMBankaHesaplari();
        List<TUMBankaHesaplari> GetListTUMBankaHesaplari(int tumKodu);
        TUMBankaHesaplari CreateTUMBankaHesap(TUMBankaHesaplari bankahesap);
        bool UpdateBankaHesap(TUMBankaHesaplari bankahesap);
        void DeleteTUMBankaHesap(int SiraNo, int tumKodu);

        DataTableList PagedListTUMBankaHesap(DataTableParameters<TUMBankaHesaplari> bankahesapList, int tumKodu);
        #endregion

        #region IletisimYetkilileri Service
        TUMIletisimYetkilileri GetTUMIletisimYetkili(int SiraNo, int tumKodu);
        List<TUMIletisimYetkilileri> GetListTUMIletisimYetkilileri();
        List<TUMIletisimYetkilileri> GetListTUMIletisimYetkilileri(int tumKodu);
        TUMIletisimYetkilileri CreateTUMIletisimYetkili(TUMIletisimYetkilileri iletisimyetkili);
        bool UpdateIletisimYetkili(TUMIletisimYetkilileri iletisimYetkili);
        void DeleteTUMIletisimYetkili(int SiraNo, int tumKodu);

        DataTableList PagedListTUMIletisimYetkili(DataTableParameters<TUMIletisimYetkilileri> iletisimYetkilileriList, int tumKodu);
        #endregion

        #region Urunleri Service
        TUMUrunleri GetTUMUrun(string urunKodu, int tumKodu);
        TUMUrunleri GetTUMUrun(string urunKodu, int tumKodu, int BabOnlineUrunKodu);
        List<TUMUrunleri> GetListUrunler();
        List<TUMUrunleri> GetListUrunler(int tumKodu);
        TUMUrunleri CreateUrun(TUMUrunleri urun);
        bool UpdateItem(TUMUrunleri urun);
        void DeleteUrun(string tumUrunKodu, int tumKodu, int babOnlineUrunKodu);

        DataTableList PagedListUrun(DataTableParameters<TUMUrunleri> urunleriList, int tumKodu);
        #endregion
    }
}