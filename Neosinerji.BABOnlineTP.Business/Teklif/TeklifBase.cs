using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;

using System.Threading;

namespace Neosinerji.BABOnlineTP.Business
{

    class CalcState
    {
        public CalcState(ManualResetEvent reset, string input)
        {
            Reset = reset;
            Input = input;
        }
        public ManualResetEvent Reset { get; private set; }
        public string Input { get; set; }
    }


    public class TeklifBase : ITeklifBase
    {
        protected ITeklifContext _TeklifContext;
        protected ITeklifService _TeklifService;
        protected ITVMService _TVMService;
        protected ITUMService _TUMService;
        protected IAracService _AracService;
        protected IMusteriService _MusteriService;
        protected ICRService _CRService;
        protected ILogService _LogService;
        protected IAktifKullaniciService _Aktif;
        protected IParametreContext _ParameterContext;
        protected string _RootPath = String.Empty;

        public TeklifBase()
        {
            _TeklifContext = DependencyResolver.Current.GetService<ITeklifContext>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _TUMService = DependencyResolver.Current.GetService<ITUMService>();
            _AracService = DependencyResolver.Current.GetService<IAracService>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            _CRService = DependencyResolver.Current.GetService<ICRService>();
            _LogService = DependencyResolver.Current.GetService<ILogService>();
            _Aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _ParameterContext = DependencyResolver.Current.GetService<IParametreContext>();
            _RootPath = System.Web.HttpContext.Current.Server.MapPath("/");
        }

        public TeklifBase(int teklifId)
            : this()
        {
            this._Teklif = _TeklifService.GetTeklif(teklifId);
            this._TUMTeklifler = _TeklifService.GetTeklifListe(this._Teklif.TeklifNo, this.Teklif.GenelBilgiler.TVMKodu);
        }

        public ITeklif Create(ITeklif teklif)
        {
            return _TeklifService.Create(teklif);
        }

        public virtual IsDurum Hesapla(ITeklif teklif)
        {
            teklif = _TeklifService.Create(teklif);

            _Teklif = teklif;

            IsDurum isDurum = this.IsDurumBaslat(this.Teklif);

            Task.Factory.StartNew(() =>
            {
                foreach (var t in this.TUMTeklifler)
                {
                    t.Hesapla(this.Teklif);

                    this.Create(t);
                    this.IsDurumTeklifTamamlandi(this.Teklif.GenelBilgiler.TeklifId, t);
                }

                this.IsDurumTamamlandi();
            });

            return isDurum;
        }

        public virtual void Policelestir(ITeklif teklif, Odeme odeme)
        {

        }

        public virtual void CreatePDF()
        {

        }

        public virtual void EPostaGonder(string DigerAdSoyad, string DigerEmail)
        {
        }

        public virtual void TSSEPostaGonder(string teklifPDF, string DigerAdSoyad, string DigerEmail)
        {
        }

        public void AddUretimMerkezi(int tumKodu)
        {
            this.UretimMerkezleri.Add(tumKodu);
        }

        public void AddOdemePlani(int odemePlaniAlternatifKodu)
        {
            this.OdemePlaniKodlari.Add(odemePlaniAlternatifKodu);
        }

        public void AddTeklif(ITeklif teklif)
        {
            this.TUMTeklifler.Add(teklif);
        }

        public IsDurum GetIsDurum()
        {
            return _TeklifContext.IsDurumRepository.Find(f => f.IsTipi == Common.IsTipleri.Teklif && f.ReferansId == this.Teklif.GenelBilgiler.TeklifId);
        }

        private List<int> _UretimMerkezleri;
        public List<int> UretimMerkezleri
        {
            get
            {
                if (_UretimMerkezleri == null)
                    _UretimMerkezleri = new List<int>();

                return _UretimMerkezleri;
            }
        }

        private List<int> _OdemePlaniKodlari;
        public List<int> OdemePlaniKodlari
        {
            get
            {
                if (_OdemePlaniKodlari == null)
                    _OdemePlaniKodlari = new List<int>();

                return _OdemePlaniKodlari;
            }
        }

        private ITeklif _Teklif;
        public ITeklif Teklif
        {
            get
            {
                return _Teklif;
            }
            set
            {
                _Teklif = value;
            }
        }

        private List<ITeklif> _TUMTeklifler;
        public List<ITeklif> TUMTeklifler
        {
            get
            {
                if (_TUMTeklifler == null)
                    _TUMTeklifler = new List<ITeklif>();

                return _TUMTeklifler;
            }
            set
            {
                _TUMTeklifler = value;
            }
        }

        #region İş Durumu
        private object _DurumToken = new object();
        private IsDurum _Durum;
        protected IsDurum IsDurumBaslat(ITeklif teklif)
        {
            _Durum = new IsDurum();
            _Durum.Guid = System.Guid.NewGuid().ToString("N");
            _Durum.IsTipi = Common.IsTipleri.Teklif;
            _Durum.ReferansId = teklif.GenelBilgiler.TeklifId;
            _Durum.IsSayi = (byte)this.TUMTeklifler.Count;
            _Durum.Tamamlanan = 0;
            _Durum.Baslangic = TurkeyDateTime.Now;
            _Durum.Durumu = IsDurumTipleri.Basladi;

            _Durum = _TeklifContext.IsDurumRepository.Create(_Durum);

            foreach (var item in this.TUMTeklifler)
            {
                IsDurumDetay detay = new IsDurumDetay();
                detay.TUMKodu = item.TUMKodu;
                detay.OdemePlaniAlternatifKodu = item.GenelBilgiler.OdemePlaniAlternatifKodu;
                detay.Durumu = IsDurumTipleri.Basladi;
                detay.Baslangic = TurkeyDateTime.Now;
                detay.HataMesaji = String.Empty;
                detay.BilgiMesaji = String.Empty;

                _Durum.IsDurumDetays.Add(detay);
            }

            _TeklifContext.Commit();

            return _Durum;
        }

        protected void IsDurumTeklifTamamlandi(int teklifId, ITeklif tumTeklif)
        {
            try
            {
                lock (_DurumToken)
                {
                    _Durum = _TeklifContext.IsDurumRepository.Find(f => f.IsTipi == Common.IsTipleri.Teklif &&
                                                                   f.ReferansId == teklifId);
                    _Durum.Tamamlanan++;
                    _TeklifContext.IsDurumRepository.Update(_Durum);

                    var durumDetay = _Durum.IsDurumDetays.FirstOrDefault(f => f.TUMKodu == tumTeklif.GenelBilgiler.TUMKodu &&
                                                                              f.OdemePlaniAlternatifKodu == tumTeklif.GenelBilgiler.OdemePlaniAlternatifKodu);

                    if (durumDetay != null)
                    {
                        durumDetay.Durumu = IsDurumTipleri.Tamamlandi;
                        durumDetay.ReferansId = tumTeklif.GenelBilgiler.TeklifId;
                        durumDetay.Bitis = TurkeyDateTime.Now;

                        if (tumTeklif.Hatalar.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (var hata in tumTeklif.Hatalar)
                            {
                                sb.Append(hata);
                                sb.Append("|");
                                sb.AppendLine();
                            }
                            durumDetay.HataMesaji = sb.ToString();
                            sb.Clear();
                        }

                        if (tumTeklif.BilgiMesajlari.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (var msj in tumTeklif.BilgiMesajlari)
                            {
                                sb.Append(msj);
                                sb.Append("|");
                                sb.AppendLine();
                            }
                            durumDetay.BilgiMesaji = sb.ToString();
                            sb.Clear();
                        }
                    }

                    _TeklifContext.Commit();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dex)
            {
                System.Data.SqlClient.SqlException ex = dex.InnerException.InnerException as System.Data.SqlClient.SqlException;
                foreach (System.Data.SqlClient.SqlError item in ex.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(item.Message);
                    System.Diagnostics.Debug.WriteLine(item.Procedure);
                }
                throw;
            }
        }

        protected void IsDurumTamamlandi()
        {
            if (_Durum == null)
                return;

            lock (_DurumToken)
            {
                int tamamlanmayanlar = _TeklifContext.IsDurumDetayRepository.All().Where(w => w.IsId == _Durum.IsId && w.Durumu == IsDurumTipleri.Basladi).Count();

                if (tamamlanmayanlar == 0)
                {
                    _Durum.Durumu = IsDurumTipleri.Tamamlandi;
                    _Durum.Bitis = TurkeyDateTime.Now;
                    _TeklifContext.IsDurumRepository.Update(_Durum);

                    _TeklifContext.Commit();
                }
            }
        }
        #endregion

        private ITeklifContext GetNewContext()
        {
            IDbContextFactory contextFactory = DependencyResolver.Current.GetService<IDbContextFactory>();
            ITeklifContext teklifContext = DependencyResolver.Current.GetService<ITeklifContext>();
            DbContext dbContext = contextFactory.CreateNewContext();
            teklifContext.SetDbContext(dbContext);

            return teklifContext;
        }

       
    }
}
