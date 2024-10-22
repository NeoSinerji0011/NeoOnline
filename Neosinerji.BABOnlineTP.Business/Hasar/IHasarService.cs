using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IHasarService
    {
        IQueryable<HasarNotlari> GetListNotlar(int hasarId);
        IQueryable<HasarBankaHesaplari> GetListBankaHesaplari(int hasarId);
        IQueryable<HasarIletisimYetkilileri> GetListIletisimYetkilileri(int hasarId);
        IQueryable<HasarEksperIslemleri> GetListEksperIslemleri(int hasarId);
        List<HasarZorunluEvrakListesi> GetListEvraklar();
        List<HasarEksperListesi> GetListEksperler();

        bool CreateHasarGenelBilgiler(HasarGenelBilgiler hasar);
        HasarGenelBilgiler GetHasarDetay(int policeId);
        bool UpdateHasarGenelBilgiler(HasarGenelBilgiler hasar);

        bool CreateHasarEksper(HasarEksperIslemleri eksper);
        bool UpdateHasarEksper(HasarEksperIslemleri eksper);
        string GetEksperAdi(int eksperKodu);
        HasarEksperIslemleri GetHasarEksperIslem(int eksperId);

        bool CreateHasarEvrak(HasarZorunluEvraklari evrak);
        void DeleteHasarEvrak(int evrakId);
        List<HasarZorunluEvraklari> GetHasarEvraklari(int hasarId);
        string GetEvrakAdi(int evrakkodu);

        bool CreateHasarNotlar(HasarNotlari not);
        bool DeleteNot(int notId);

        bool CreateHasarBankaHesap(HasarBankaHesaplari bankaHesap);
        bool CreateHasarIletisimYetkilileri(HasarIletisimYetkilileri iletisimYetkili);
        HasarBankaHesaplari GetBankaHesap(int hesapId);
        bool UpdateBankaHesap(HasarBankaHesaplari bankaHesap);
        void DeleteHasarBankaHesap(int bankaHesapId);

        HasarIletisimYetkilileri GetTVMIletisimYetkili(int iletisimId);
        bool UpdateHasarIletisimYetkili(HasarIletisimYetkilileri iletisimYetkili);
        HasarIletisimYetkilileri GetIletisimYetkilileriDetay(int iletisimId);
        void DeleteHasarIletisimYetkili(int iletisimId);

        List<HasarAsistansFirmalari> GetListAsistansFirmalari();
        List<HasarAnlasmaliServisler> GetListAnlasmaliServisler();
    }
}
