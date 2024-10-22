using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{
    public interface ITaliAcenteTransferService
    {
        TVMDetay GetDetay(int kodu);
        List<TVMYetkiGruplari> GetListTVYetkiGruplari(int id);
        List<TVMYetkiGrupYetkileri> GetListTVYetkiGrupYetkileri(List<TVMYetkiGruplari> yetkiGrupKodu);
        List<TVMKullanicilar> GetListKullanicilar(int tvmKodu);
        List<TVMDepartmanlar> GetListDepartmanlar(int tvmKodu);
        List<TVMBolgeleri> GetListBolgeler(int tvmKodu);
        List<TVMWebServisKullanicilari> GetListTVMWebServisKullanicilari(int tvmKodu);
        List<TVMUrunYetkileri> GetListTVMUrunYetkileri(int id);

        List<TaliAcenteExcel> getTaliler(string path, int tvmKodu);
        void Add(List<TaliAcente> taliler);
        void taliYetkiAdd(List<TVMYetkiGrupYetkileri> taliYetkileri);
        void taliWebServisKullaniciAdd(List<TVMWebServisKullanicilari> taliWebServis);
        void taliDepartmanlarAdd(List<TVMDepartmanlar> taliDepartmanlar);
        void taliBolgelerAdd(List<TVMBolgeleri> taliBolgeler);
        bool tckVarMi(string tc);
        bool TVMKoduVarMi(int tvmKodu);
        void taliKullaniciAdd(TVMKullanicilar taliKullanici);
         TVMDetay GetSonTvm(int tvmKodu);
        int GetSonTvmKodu(int tvmKodu);
        int getBasariliKayitlar();
        int getBasarisizKayitlar();
        List<TVMYetkiGrupYetkileri> GetListTVYetkiGrupYetkileri(int yetkiGrupKodu);

        TVMYetkiGruplari GetTVYetkiGrup(int yetkiKodu);
        TVMYetkiGruplari GetTVYetkiGruplari(int id);
    }
}
