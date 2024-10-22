using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class HasarService : IHasarService
    {
        IPoliceContext _PoliceContext;
        public HasarService(IPoliceContext policeContext)
        {
            _PoliceContext = policeContext;
        }
        public IQueryable<HasarNotlari> GetListNotlar(int hasarId)
        {
            return _PoliceContext.HasarNotlariRepository.Filter(s => s.HasarId == hasarId).OrderByDescending(s=>s.NotId);
        }
        public IQueryable<HasarBankaHesaplari> GetListBankaHesaplari(int hasarId)
        {
            return _PoliceContext.HasarBankaHesaplariRepository.Filter(s => s.HasarId == hasarId).OrderByDescending(s => s.BankaHesapId);
        }
        public IQueryable<HasarIletisimYetkilileri> GetListIletisimYetkilileri(int hasarId)
        {
            return _PoliceContext.HasarIletisimYetkilileriRepository.Filter(s => s.HasarId == hasarId).OrderByDescending(s => s.IletisimYetkiliId);
        }
        public IQueryable<HasarEksperIslemleri> GetListEksperIslemleri(int hasarId)
        {
            return _PoliceContext.HasarEksperIslemleriRepository.Filter(s => s.HasarId == hasarId).OrderByDescending(s => s.EksperId);
        }
        public List<HasarZorunluEvrakListesi> GetListEvraklar()
        {
            return _PoliceContext.HasarZorunluEvrakListesiRepository.All().Where(s => s.Durum == 1).ToList();
        }
        public List<HasarEksperListesi> GetListEksperler()
        {
            return _PoliceContext.HasarEksperListesiRepository.All().Where(s => s.Durumu == 1).ToList();
        }

        public HasarGenelBilgiler GetHasarDetay(int policeId)
        {
            return _PoliceContext.HasarGenelBilgilerRepository.All().Where(s => s.PoliceId == policeId).FirstOrDefault();
        }

        public bool CreateHasarGenelBilgiler(HasarGenelBilgiler hasar)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarGenelBilgilerRepository.Create(hasar);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public bool UpdateHasarGenelBilgiler(HasarGenelBilgiler hasar)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarGenelBilgilerRepository.Update(hasar);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public bool CreateHasarEksper(HasarEksperIslemleri eksper)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarEksperIslemleriRepository.Create(eksper);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public bool UpdateHasarEksper(HasarEksperIslemleri eksper)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarEksperIslemleriRepository.Update(eksper);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public HasarEksperIslemleri GetHasarEksperIslem(int eksperId)
        {
            HasarEksperIslemleri hasarEksperIslem = new HasarEksperIslemleri();
            hasarEksperIslem = _PoliceContext.HasarEksperIslemleriRepository.All().Where(s => s.EksperId == eksperId).FirstOrDefault();

            return hasarEksperIslem;

        }
        
        public string GetEksperAdi(int eksperKodu)
        {
            string EksperUnvani = "";
            var eksper = _PoliceContext.HasarEksperListesiRepository.All().Where(s => s.EksperKodu == eksperKodu).FirstOrDefault();
            if (eksper != null)
            {
                EksperUnvani = eksper.EksperAdSoyadUnvan;
            }
            return EksperUnvani;
        }

        public bool CreateHasarEvrak(HasarZorunluEvraklari evrak)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarZorunluEvraklariRepository.Create(evrak);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public List<HasarZorunluEvraklari> GetHasarEvraklari(int hasarId)
        {
            return _PoliceContext.HasarZorunluEvraklariRepository.All().Where(s => s.HasarId == hasarId).ToList();
        }

        public string GetEvrakAdi(int evrakkodu)
        {
            string Evrak = "";
            var evrakAdi = _PoliceContext.HasarZorunluEvrakListesiRepository.All().Where(s => s.EvrakKodu == evrakkodu).FirstOrDefault();
            if (evrakAdi != null)
            {
                Evrak = evrakAdi.EvrakAdi;
            }
            return Evrak;
        }

        public void DeleteHasarEvrak(int evrakId)
        {
            _PoliceContext.HasarZorunluEvraklariRepository.Delete(m => m.EvrakId == evrakId);
            _PoliceContext.Commit();
        }

        public bool CreateHasarNotlar(HasarNotlari not)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarNotlariRepository.Create(not);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public bool DeleteNot(int notId)
        {
            _PoliceContext.HasarNotlariRepository.Delete(s => s.NotId == notId);
            _PoliceContext.Commit();
            return true;
        }

        public bool CreateHasarBankaHesap(HasarBankaHesaplari bankaHesap)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarBankaHesaplariRepository.Create(bankaHesap);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public bool CreateHasarIletisimYetkilileri(HasarIletisimYetkilileri iletisimYetkili)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarIletisimYetkilileriRepository.Create(iletisimYetkili);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public HasarBankaHesaplari GetBankaHesap(int hesapId)
        {
            HasarBankaHesaplari Hesap = new HasarBankaHesaplari();
            Hesap = _PoliceContext.HasarBankaHesaplariRepository.All().Where(s => s.BankaHesapId == hesapId).FirstOrDefault();

            return Hesap;

        }

        public bool UpdateBankaHesap(HasarBankaHesaplari bankaHesap)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarBankaHesaplariRepository.Update(bankaHesap);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }
        public void DeleteHasarBankaHesap(int bankaHesapId)
        {
            _PoliceContext.HasarBankaHesaplariRepository.Delete(m => m.BankaHesapId == bankaHesapId);
            _PoliceContext.Commit();
        }
        public HasarIletisimYetkilileri GetTVMIletisimYetkili(int iletisimId)
        {
            HasarIletisimYetkilileri iletisimYetkili = new HasarIletisimYetkilileri();
            iletisimYetkili = _PoliceContext.HasarIletisimYetkilileriRepository.All().Where(s => s.IletisimYetkiliId == iletisimId).FirstOrDefault();

            return iletisimYetkili;

        }
      
        public bool UpdateHasarIletisimYetkili(HasarIletisimYetkilileri iletisimYetkili)
        {
            bool basariliMi = false;
            try
            {
                _PoliceContext.HasarIletisimYetkilileriRepository.Update(iletisimYetkili);
                _PoliceContext.Commit();
                basariliMi = true;
                return basariliMi;
            }
            catch (Exception)
            {
                basariliMi = false;
                return basariliMi;
                throw;
            }

        }

        public HasarIletisimYetkilileri GetIletisimYetkilileriDetay(int iletisimId)
        {
            return _PoliceContext.HasarIletisimYetkilileriRepository.All().Where(s => s.IletisimYetkiliId == iletisimId).FirstOrDefault();
        }
        public void DeleteHasarIletisimYetkili(int iletisimId)
        {
            _PoliceContext.HasarIletisimYetkilileriRepository.Delete(m => m.IletisimYetkiliId == iletisimId);
            _PoliceContext.Commit();
        }

        public List<HasarAsistansFirmalari> GetListAsistansFirmalari()
        {
            return _PoliceContext.HasarAsistansFirmalariRepository.All().Where(s => s.Durumu == 1).ToList();
        }

        public List<HasarAnlasmaliServisler> GetListAnlasmaliServisler()
        {
            return _PoliceContext.HasarAnlasmaliServislerRepository.All().Where(s => s.Durumu == 1).ToList();
        }
    }
}
