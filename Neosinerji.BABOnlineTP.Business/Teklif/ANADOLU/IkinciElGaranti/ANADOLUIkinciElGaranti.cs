using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUIkinciElGaranti : Teklif, IANADOLUIkinciElGaranti
    {
        IParametreContext _ParameterContext;
        ITeklifService _TeklifService;


        [InjectionConstructor]
        public ANADOLUIkinciElGaranti()
            : base()
        {
            _ParameterContext = DependencyResolver.Current.GetService<IParametreContext>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.ANADOLU;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                DateTime bitisTarihi = TurkeyDateTime.Now;
                decimal brutPrim = 0;
                decimal IkinciElGarantiliUzatmaTeminati = 0;

                //  int modelyili = Convert.ToInt32(teklif.ReadSoru(IkinciElGarantiSorular.ModelYili, "0"));
                byte periyod = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.PoliceSuresi, "0"));
                byte silindirHacmi = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.SilindirHacmi, "0"));
                byte teminatTuru = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.TeminatTuru, "0"));


                string teminat = String.Empty;
                switch (teminatTuru)
                {
                    case TeminatTurleri.Normal: teminat = "Normal"; break;
                    case TeminatTurleri.Yildiz_3: teminat = "Star_3"; break;
                    case TeminatTurleri.Yildiz_5: teminat = "Star_5"; break;
                }

                El2Garanti_HesapCetveli cetvel = _ParameterContext.El2Garanti_HesapCetveliRepository.Filter(s => s.Periyod == periyod &&
                                                                                        s.TeminatTuru == teminat &&
                                                                                        s.MotorHacmi == silindirHacmi).FirstOrDefault();

                if (cetvel != null)
                {
                    brutPrim = (cetvel.Mandatory_1 * KurBilgileri.Euro);
                    IkinciElGarantiliUzatmaTeminati = (cetvel.TeminatTutari * KurBilgileri.Euro);
                }
                else
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;

                    this.EndLog("Grilen kriterlerle teklif hesaplanamıyor.", false);
                    this.AddHata("Grilen kriterlerle teklif hesaplanamıyor.");
                    return;
                }

                #region Genel bilgiler

                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = TurkeyDateTime.Now;
                this.GenelBilgiler.BitisTarihi = bitisTarihi;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = brutPrim;
                this.GenelBilgiler.NetPrim = brutPrim;

                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = 0;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi ?? 1;

                #endregion


                #region Teminatlar

                this.AddTeminat(IkinciElGarantiTeminatlar.IkinciElGarantiliUzatma, IkinciElGarantiliUzatmaTeminati, 0, 0, 0, 0);

                #endregion

                #region Vergiler

                // Vergi Yoktur.

                #endregion

                #region Ödeme Planı

                if (this.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    if (this.GenelBilgiler.TaksitSayisi > 1)
                        this.AddOdemePlaniALL(teklif);
                }
                else
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, teklif.GenelBilgiler.OdemeTipi ?? 0);

                #endregion

            }
            catch (Exception ex)
            {
                #region Hata Log

                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            try
            {
                ITeklif anaTeklif = _TeklifService.GetAnaTeklif(this.TeklifNo, this.GenelBilgiler.TVMKodu);
                Random rnd = new Random();
                int policeNo = rnd.Next((int.MaxValue / 2), int.MaxValue);

                this.GenelBilgiler.TUMPoliceNo = policeNo.ToString();
                this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                this.GenelBilgiler.TanzimTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BaslamaTarihi = anaTeklif.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = anaTeklif.GenelBilgiler.BitisTarihi;

                //Muhasebe aktarımı
                //this.SendMuhasebe();

                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);


                //Poliçe yazılıyor
                //IkinciElGarantiTeklif ikinciElGaranti = new IkinciElGarantiTeklif(this.GenelBilgiler.TeklifId);
                //ikinciElGaranti.CreatePolicePDF(this);
            }
            catch (Exception ex)
            {
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public override void PolicePDF()
        {
            //base.PolicePDF();
        }
    }
}
