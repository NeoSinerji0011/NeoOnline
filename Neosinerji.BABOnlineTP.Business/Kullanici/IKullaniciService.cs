using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IKullaniciService
    {
        #region TVMKullanicilar
        List<KullaniciListeModel> PagedList(KullaniciArama arama, out int totalRowCount);
        List<TVMKullanicilar> GetListKullanicilar();
        List<KullaniciModelForList> GetTVMKullanicilari(int tvmKodu);
        List<KullaniciModelForList> GetListTVMKullanicilari(int tvmKodu);
        List<KullaniciModelForList> GetListAktifTVMKullanicilari(int tvmKodu);
        List<KullaniciOzetModel> GetKullaniciOzet(int tvmKodu);
        TVMKullanicilar GetKullaniciByEmail(string email);
        TVMKullanicilar GetKullanici(int kullaniciKodu);
        TVMKullanicilar GetKullaniciPublic(int kullaniciKodu);
        TVMKullanicilar GetKullaniciByTCKN(string tckn);
        TVMKullanicilar GetKullaniciByPartajNO(string partaj);
        TVMKullanicilar CreateKullanici(TVMKullanicilar kullanici);
        TVMKullanicilar GetKullaniciYetkisiz(int kullaniciKodu);
        TVMKullanicilar GetMapfreKullanici(string kullaniciKodu);
        List<KullaniciModelForList> GetListKullanicilarTeklifAra();
        bool CreateSifreTarihcesi(TVMKullaniciSifreTarihcesi sifreTarihi);
        void UpdateKullanici(TVMKullanicilar kullanici);
        bool RecoverPassword(TVMKullanicilar kullanici);
        bool RecoverPasswordMapfre(TVMKullanicilar kullanici);
        bool ResetPassword(string token);
        bool KullaniciEkleTest(string tckn);

        TVMKullanicilar GetKullaniciEmailKontorl(string EmailKontrolKodu);
        #endregion

        #region TVMKullaniciAtama
        void CreateKullaniciAtama(TVMKullanicilar kullanici, int? yeniDepartmanKodu);
        #endregion

        #region TVMKullaniciDurumTarihcesi
        void CreateKullaniciDurum(TVMKullanicilar kullanici, byte durum);
        #endregion

        AcenteKullanicilariModel GetListAcenteKullanicilari();
        bool KullaniciYetkiKontrolu(int id);


        //Mapfre Kullanıcı
        MapfreKullanici CreateMapfreKullanici(MapfreKullanici MapfreKullanici);
        //MapfreKullanici MapfreKullaniciGet(int KullaniciId);
        //void UpdateMapfreKullanici(MapfreKullanici kullanici);
        List<MapfreKullaniciListeModel> MapfrePagedList(MapfreKullaniciArama arama, out int totalRowCount);
        void MapfreKulaniciOlustur(int mapfeKullaniciId, string sifre);
        TVMKullanicilar GetMapfreKullaniciByKullaniciAdi(string KullaniciAdi);
        bool MapfreKullaniciEkleTest(string kullaniciAdi);
        string KullaniciSifreSuresiKontrol(int aktifTvmKodu, int aktifKullaniciKodu);

        bool KullaniciVarmi(string tckn, string email);
    }
}
