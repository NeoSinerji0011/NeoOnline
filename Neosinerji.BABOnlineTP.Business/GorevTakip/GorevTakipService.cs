using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GorevTakip
{
    public class GorevTakipService : IGorevTakipService
    {
        IPoliceContext _policeContext;
        IMusteriService _musteriService;
        IGorevTakipContext _gorevTakipContext;
        IAktifKullaniciService _AktifKullanici;        

        public GorevTakipService(IPoliceContext policeContext, IMusteriService musteriService, IGorevTakipContext gorevTakipContext, IAktifKullaniciService aktifKullaniciService)
        {
            _policeContext = policeContext;
            _musteriService = musteriService;
            _gorevTakipContext = gorevTakipContext;
            _AktifKullanici = aktifKullaniciService;
        }

        #region Add-Update-Get
        public AtananIsler AddAtananIs(AtananIsler IsItem)
        {
            try
            {
                IsItem = _gorevTakipContext.AtananIslerRepository.Create(IsItem);
                _gorevTakipContext.Commit();
                return IsItem;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public bool UpdateAtananIs(AtananIsler IsItem)
        {
            bool result = false;
            try
            {
                _gorevTakipContext.AtananIslerRepository.Update(IsItem);
                _gorevTakipContext.Commit();
                result = true;
                return result;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public AtananIsler getIsDetay(int IsId)
        {
            AtananIsler isDetay = new AtananIsler();
            isDetay = _gorevTakipContext.AtananIslerRepository.FindById(IsId);
            return isDetay;
        }
        public MusteriBilgi GetMusteriBilgi(string KimlikNo, int TVMKodu)
        {
            MusteriBilgi musteriBilgi = new MusteriBilgi();
            var musteriDetay = _musteriService.GetMusteri(KimlikNo, TVMKodu);
            if (musteriDetay != null)
            {
                musteriBilgi.Adi = musteriDetay.AdiUnvan;
                musteriBilgi.Soyadi = musteriDetay.SoyadiUnvan;
            }
            return musteriBilgi;
        }
        
        #endregion

        #region Gorev Dağılım ve İşlerim Raporu
        public List<IslerimProcedureModel> IslerimPagedList()
        {
            string durumListe = IsDurumTip.Beklemede + "," + IsDurumTip.DevamEdiyor;

            List<IslerimProcedureModel> raporsonuc = _gorevTakipContext.SP_Islerim(_AktifKullanici.TVMKodu, _AktifKullanici.KullaniciKodu, durumListe);
            return raporsonuc.ToList();
        }      

        public List<AtananIslerProcedureModel> GorevDagilimRaporuPagedList(AtananIslerimListe arama, out int totalRowCount)
        {

            List<AtananIslerProcedureModel> raporsonuc = _gorevTakipContext.GorevDagilimRaporu(arama.IsBaslangicTarihi,
                                                                                    arama.IsBasBitisTarihi,
                                                                                    arama.TvmKodu,
                                                                                    arama.KullaniciList,
                                                                                    arama.IsTipi,
                                                                                    arama.Durum,
                                                                                    arama.OncelikSeviyesi
                                                                                    );


            if (arama.PageSize <= 0) arama.PageSize = 10;

            totalRowCount = raporsonuc.Count();

            if (totalRowCount <= arama.PageSize || arama.Page <= 0) arama.Page = 1;

            int excludedRows = (arama.Page - 1) * arama.PageSize;

            var list = (from k in raporsonuc
                        select k).Skip(excludedRows)
                                 .Take(arama.PageSize)
                                 .ToList();

            return list.ToList<AtananIslerProcedureModel>();
        }

        public List<IsTipleri> GetIsTipleri()
        {
            List<IsTipleri> isTipleri = new List<IsTipleri>();
            isTipleri = _gorevTakipContext.IsTipleriRepository.All().Where(w => w.Durum == 1).ToList();
            return isTipleri;
        }
        public List<TalepKanallari> GetTalepKanallari()
        {
            List<TalepKanallari> talepKanallari = new List<TalepKanallari>();
            talepKanallari = _gorevTakipContext.TalepKanallariRepository.All().Where(w => w.Durum == 1).ToList();
            return talepKanallari;
        }
        public string DurumRenk(byte durum)
        {
            string renk = "";
            switch (durum)
            {
                case 1: renk = "#ff80ff"; break;
                case 2: renk = "#ff9933"; break;
                case 3: renk = "#FF7F7F"; break;
                case 4: renk = "#66b3ff"; break;
                case 5: renk = "#ff0000"; break;
                default: break;
            }
            return renk;
        }
        public string OncelikSeviyesiRenk(byte oncelikSeviye)
        {
            string renk = "";
            switch (oncelikSeviye)
            {
                case 1: renk = "#ff0000"; break;
                case 2: renk = "#993300"; break;
                case 3: renk = "#009900"; break;
                default: break;
            }
            return renk;
        }
        #endregion 

        #region Dökümanlar
        public IQueryable<AtananIsDokumanlar> GetListDokumanlar(int isId)
        {
            return _gorevTakipContext.AtananIsDokumanlarRepository.Filter(s => s.IsId == isId);
        }

        public AtananIsDokumanlar CreateDokuman(AtananIsDokumanlar dokuman)
        {
            AtananIsDokumanlar dkmn = _gorevTakipContext.AtananIsDokumanlarRepository.Create(dokuman);
            _gorevTakipContext.Commit();
            return dkmn;
        }
        public AtananIsDokumanlar GetTVMDokuman(int siraNo, int isId)
        {
            AtananIsDokumanlar dokuman = _gorevTakipContext.AtananIsDokumanlarRepository.Find(m => m.SiraNo == siraNo && m.IsId == isId);
            return dokuman;
        }
        public void DeleteDokuman(int dokumanKodu, int isId)
        {
            _gorevTakipContext.AtananIsDokumanlarRepository.Delete(m => m.SiraNo == dokumanKodu && m.IsId == isId);
            _gorevTakipContext.Commit();
        }
        #endregion

        #region Notlar
        public IQueryable<AtananIsNotlar> GetListNotlar(int isId)
        {
            return _gorevTakipContext.AtananIsNotlarRepository.Filter(s => s.IsId == isId).OrderByDescending(s=>s.KayitTarihi);
        }
        public AtananIsNotlar CreateNot(AtananIsNotlar not)
        {
            AtananIsNotlar isNot = _gorevTakipContext.AtananIsNotlarRepository.Create(not);
            _gorevTakipContext.Commit();
            return isNot;
        }
        public void DeleteNot(int notId)
        {
            _gorevTakipContext.AtananIsNotlarRepository.Delete(m => m.NotId == notId);
            _gorevTakipContext.Commit();
        }
        #endregion
        public static class IsDurumTip
        {
            public const string Beklemede = "1";
            public const string DevamEdiyor = "2";
        }

    }
}
