using EurekoSigorta_Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IMusteriService
    {
        #region MusteriGenelBİlgiler members
        MusteriGenelBilgiler GetMusteri(int MusteriKodu);
        MusteriGenelBilgiler GetMusteri(string KimlikNo, int TVMKodu);
        MusteriGenelBilgiler GetMusteriTeklifFor(string KimlikNo, int TVMKodu);
        List<MusteriFinderOzetModel> GetMusteriListByTvmKodu(int TVMKodu);
        MusteriGenelBilgiler CreateMusteri(MusteriGenelBilgiler musteri, MusteriAdre adres, MusteriTelefon telefon);
        MusteriGenelBilgiler CreateMusteri(MusteriGenelBilgiler musteri);
        bool UpdateMusteri(MusteriGenelBilgiler musteriGenelBilgiler);
        bool DeleteMusteri(int Id);
        void UpdateMusteriAdi(MusteriGenelBilgiler bilgi);
        int ToplamMusteriSayisi(int tvmKodu);
        int ToplamMusteriSayisi();
        List<MusteriGenelBilgiler> GetSon5Musteri(int tvmKodu);
        void MusteriEnlemBoylamGetir();
        #endregion


        #region MusteriAdres Members
        MusteriAdre CreateMusteriAdres(MusteriAdre adres);
        MusteriAdre GetAdres(int musteriKodu, int siraNo);
        MusteriAdre GetDefaultAdres(int musteriKodu);
        List<MusteriAdre> GetMusteriAdresleri(int musteriKodu);
        bool DeleteAdres(int musteriKodu, int siraNo);
        bool UpdateAdres(MusteriAdre adres);
        //UlkeToilceListModel GetMusteriAdresleriKisa(int musteriKodu);

        ParitusAdresModel GetParitusAdres(string adres);
        #endregion


        #region MusteriTelefon Members
        List<MusteriTelefon> GetMusteriTelefon(int musteriKodu);
        MusteriTelefon GetTelefon(int musteriKodu, int siraNo);
        MusteriTelefon CreateTelefon(MusteriTelefon musteriTelefon);
        bool DeleteTelefon(int musteriKodu, int siraNo);
        bool UpdateTelefon(MusteriTelefon telefon);
        #endregion


        #region MusteriDokuman Members
        List<DokumanDetayModel> GetMusteriDokumanlari(int musteriKodu);
        MusteriDokuman GetDokuman(int musteriKodu, int siraNo);
        MusteriDokuman CreateDokuman(MusteriDokuman Dokuman);
        bool DeleteDokuman(int musteriKodu, int siraNo);
        bool UpdateDokuman(MusteriDokuman dokuman);
        bool CheckedFileName(string fileName, int musteriKodu);
        #endregion


        #region MusteriNotlar Members
        List<NotModelDetay> GetMusteriNotlari(int musteriKodu);
        MusteriNot GetNot(int musteriKodu, int siraNo);
        MusteriNot CreateNot(MusteriNot Not);
        bool DeleteNot(int musteriKodu, int siraNo);
        bool UpdateNot(MusteriNot not);
        #endregion


        #region Meslek

        Meslek GetMeslek(int meslekKodu);

        #endregion

        #region extra

        void XmlToDB();
        #endregion

        List<MusteriHaritaOzelDetay> MusterilerimHaritaArama(MusteriharitaAramaModel arama);

        DataTableList PagedListTelefon(DataTableParameters<MusteriTelefon> telefonList, int musteriKodu);
        DataTableList PagedListAdres(DataTableParameters<MusteriAdre> adresList, int musteriKodu);
        DataTableList PagedListDokuman(DataTableParameters<MusteriDokuman> dokumanList, int musteriKodu);
        DataTableList PagedListNot(DataTableParameters<MusteriNot> notList, int musteriKodu);
        List<MusteriListeModelOzel> PagedList(MusteriListe musteriListe, out int totalRowCount);
        List<MusteriListesiModelOzel> PagedMusteriList(MusteriListesi musteriListesi, out int totalRowCount);

        List<MusteriAdediModelOzel> PagedMusteriAdetList(MusteriAdedi musteriAdedi);

        #region Potansiyel Müşteri Genel Bilgiler

        PotansiyelMusteriGenelBilgiler GetPotansiyelMusteri(int musteriKodu);
        PotansiyelMusteriGenelBilgiler GetPotansiyelMusteri(string KimlikNo, int TVMKodu);
        List<PotansiyelMusteriFinderOzetModel> GetPotansiyelMusteriListByTvmKodu(int TVMKodu);
        int ToplamPotansiyelMusteriSayisi(int tvmKodu);
        int ToplamPotansiyelMusteriSayisi();
        List<PotansiyelMusteriGenelBilgiler> GetSon5PotansiyelMusteri(int tvmKodu);
        PotansiyelMusteriGenelBilgiler CreatePotansiyelMusteri(PotansiyelMusteriGenelBilgiler musteri, PotansiyelMusteriAdre adres, PotansiyelMusteriTelefon telefon);
        bool UpdatePotansiyelMusteri(PotansiyelMusteriGenelBilgiler potansiyelMusteriGenelBilgiler);
        bool DeletePotansiyelMusteri(int Id);

        #endregion

        #region Potansiyel Müşteri Adres Members
        List<PotansiyelMusteriAdre> GetPotansiyelMusteriAdresleri(int potansiyelMusteriKodu);
        PotansiyelMusteriAdre GetPotansiyelAdres(int potansiyelMusteriKodu, int siraNo);
        //PotansiyelMusteriAdre GetPotansiyelAdres(int potansiyelMusteriKodu);
        PotansiyelMusteriAdre GetDefaultPotansiyelAdres(int potansiyelMusteriKodu);
        PotansiyelMusteriAdre CreatePotansiyelMusteriAdres(PotansiyelMusteriAdre potansiyelMusteriAdres);
        bool DeletePotansiyelAdres(int potansiyelMusteriKodu, int siraNo);
        bool UpdatePotansiyelAdres(PotansiyelMusteriAdre adres);
        #endregion

        #region Potansiyel MusteriTelefon Members

        //Musteriye ait tum kayıtlı telefonlari getiren method
        List<PotansiyelMusteriTelefon> GetPotansiyelMusteriTelefon(int potansiyelMusteriKodu);
        PotansiyelMusteriTelefon GetPotansiyelTelefon(int potansiyelMusteriKodu, int siraNo);
        PotansiyelMusteriTelefon CreatePotansiyelTelefon(PotansiyelMusteriTelefon potansiyelMusteriTelefon);
        bool DeletePotansiyelTelefon(int potansiyelMusteriKodu, int siraNo);
        bool UpdatePotansiyelTelefon(PotansiyelMusteriTelefon telefon);
        #endregion

        #region Potansiyel MusteriDokuman Members

        List<PotansiyelDokumanDetayModel> GetPotansiyelMusteriDokumanlari(int potansiyelMusteriKodu);
        PotansiyelMusteriDokuman GetPotansiyelDokuman(int potansiyelMusteriKodu, int siraNo);
        PotansiyelMusteriDokuman CreatePotansiyelDokuman(PotansiyelMusteriDokuman Dokuman);
        bool DeletePotansiyelDokuman(int potansiyelMusteriKodu, int siraNo);
        bool PotansiyelCheckedFileName(string fileName, int potansiyelMusteriKodu);
        bool UpdatePotansiyelDokuman(PotansiyelMusteriDokuman dokuman);
        #endregion

        #region Potansiyel MusteriNotlar Members


        //Musteriye ait tum notlari getiren method
        List<PotansiyelNotModelDetay> GetPotansiyelMusteriNotlari(int potansiyelMusteriKodu);
        PotansiyelMusteriNot GetPotansiyelNot(int potansiyelMusteriKodu, int siraNo);
        PotansiyelMusteriNot CreatePotansiyelNot(PotansiyelMusteriNot Not);
        bool DeletePotansiyelNot(int potansiyelMusteriKodu, int siraNo);
        bool UpdatePotansiyelNot(PotansiyelMusteriNot not);
        #endregion


        DataTableList PagedListPotansiyelTelefon(DataTableParameters<PotansiyelMusteriTelefon> telefonList, int potansiyelMusteriKodu);
        DataTableList PagedListPotansiyelAdres(DataTableParameters<PotansiyelMusteriAdre> adresList, int potansiyelMusteriKodu);
        DataTableList PagedListPotansiyelDokuman(DataTableParameters<PotansiyelMusteriDokuman> dokumanList, int potansiyelMusteriKodu);
        DataTableList PagedListPotansiyelNot(DataTableParameters<PotansiyelMusteriNot> notList, int potansiyelMusteriKodu);
        List<PotansiyelMusteriListeModelOzel> PagedListPotansiyel(PotansiyelMusteriListe arama, out int totalRowCount);

    }
}
