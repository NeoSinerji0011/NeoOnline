using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceMuhasebe
{

    public class PoliceEntity
    {
        IPoliceService _PoliceService;
        ITVMService _TVMService;
        public Int32 PoliceId { get; set; }

        public String PoliceHash { get; set; }
        public String PoliceNumarasi { get; set; }
        public Nullable<int> EkNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public String SigortaSirketKodu { get; set; }
        public Int32? UrunKodu { get; set; }
        public String UrunAdi { get; set; }
        public AcenteEntity Acente { get; set; }
        public SigortaliEntity Sigortali { get; set; }
        public SigortaEttirenEntity SigortaEttiren { get; set; }
        public Decimal? NetPrim { get; set; }
        public Decimal? ToplamVergi { get; set; }
        public Decimal? BrutPrim { get; set; }
        public Decimal? DovizKuru { get; set; }
        public Decimal? ToplamKomisyon { get; set; }
        public VergiEntity Vergiler { get; set; }
        public DateTime? TanzimTarihi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public Int32 TaksitSayisi { get; set; }
        public Int32? OdemeSekli { get; set; }
        //public Int32 OdemeTipi { get; set; }
        public String ParaBrimi { get; set; }
        public Int32? TaliAcenteKodu { get; set; }
        public Int32? UretimTaliAcenteKodu { get; set; }
        public String TaliAcenteUnvani { get; set; } //taliacente unvanı
        public String UretimTaliAcenteUnvani { get; set; }
        public Decimal? TaliAcenteKomisyon { get; set; }
        public OdemeEntity[] Odemeler { get; set; }
        public AracEntity Plaka { get; set; }
        public string TVMMusteriKodu { get; set; }

        public PoliceEntity()
        {

        }

        public PoliceEntity(PoliceGenel policeGenel)
        {
            if (policeGenel != null)
            {
                this.PoliceNumarasi = policeGenel.PoliceNumarasi;
                this.SigortaSirketKodu = policeGenel.TUMBirlikKodu;
                this.EkNo = policeGenel.EkNo;
                this.YenilemeNo = policeGenel.YenilemeNo;

                this.BaslangicTarihi = policeGenel.BaslangicTarihi;
                this.BitisTarihi = policeGenel.BitisTarihi;
                this.BrutPrim = policeGenel.BrutPrim;
                this.NetPrim = policeGenel.NetPrim;
                if (policeGenel.ParaBirimi=="YTL")
                {
                    policeGenel.ParaBirimi = "TL";
                }
                this.DovizKuru = policeGenel.DovizKur;
                this.ParaBrimi = policeGenel.ParaBirimi;
                this.PoliceId = policeGenel.PoliceId;
                this.PoliceNumarasi = policeGenel.PoliceNumarasi;
                this.SigortaEttiren = new SigortaEttirenEntity(policeGenel.PoliceSigortaEttiren);
                this.Sigortali = new SigortaliEntity(policeGenel.PoliceSigortali);
                this.TanzimTarihi = policeGenel.TanzimTarihi;
                this.ToplamVergi = policeGenel.ToplamVergi;
                this.ToplamKomisyon = policeGenel.Komisyon;
                this.Plaka = new AracEntity(policeGenel.PoliceArac);
                this.TaliAcenteKodu = policeGenel.TaliAcenteKodu;
                var taliacentekodu = 0;
                if (policeGenel.TaliAcenteKodu != null)
                {
                    taliacentekodu = policeGenel.TaliAcenteKodu.Value;
                }
                this.TaliAcenteUnvani = this.getAltAcenteunvan(taliacentekodu);
                this.TaliAcenteKomisyon = policeGenel.TaliKomisyon;
                this.UretimTaliAcenteKodu = policeGenel.UretimTaliAcenteKodu;
                var uertimtaliacentekodu = 0;
                if (policeGenel.UretimTaliAcenteKodu != null)
                {
                    uertimtaliacentekodu = policeGenel.UretimTaliAcenteKodu.Value;
                }
                this.UretimTaliAcenteUnvani = this.getAltAcenteunvan(uertimtaliacentekodu);
              //  this.UretimTaliAcenteUnvani = policeGenel.TVMDetay.Unvani;
                this.TVMMusteriKodu = this.creteMusteriGrupAdi(policeGenel);
                this.UrunKodu = policeGenel.BransKodu;
                this.UrunAdi = policeGenel.BransAdi;

                this.OdemeSekli = policeGenel.OdemeSekli;
                //this.OdemeTipi = OdemeTipleri.KrediKarti;
                this.Odemeler = this.createOdemeEntityArray(policeGenel);
                this.TaksitSayisi = this.Odemeler.Length;
                this.PoliceHash = policeGenel.HashCode;

                this.Vergiler = this.createVergiEntity(policeGenel);


            }
        }

        private OdemeEntity[] createOdemeEntityArray(PoliceGenel policeGenel)
        {
            List<OdemeEntity> odemeEntityList = new List<OdemeEntity>();
            List<PoliceOdemePlani> odemePlaniList = (List<PoliceOdemePlani>)policeGenel.PoliceOdemePlanis;
            if (odemePlaniList != null)
            {
                foreach (PoliceOdemePlani odemePlani in odemePlaniList)
                {
                    OdemeEntity entity = new OdemeEntity(odemePlani);
                    odemeEntityList.Add(entity);
                }
            }
            return odemeEntityList.ToArray();
        }

        private string creteMusteriGrupAdi(PoliceGenel policeGenel)
        {
            string ret = "";
            _PoliceService = DependencyResolver.Current.GetService<IPoliceService>();
            var musteriDetay = _PoliceService.getMusteri( policeGenel.PoliceSigortaEttiren.KimlikNo,  policeGenel.TVMKodu.Value);
            if (musteriDetay!=null)
            {
                ret = musteriDetay.TVMMusteriKodu;
            } 
            
            return ret;
        }

        private VergiEntity createVergiEntity(PoliceGenel policeGenel)
        {
            VergiEntity vergiEntity = new VergiEntity();
            List<PoliceVergi> policeVergiList = (List<PoliceVergi>)policeGenel.PoliceVergis;
            foreach (PoliceVergi policeVergi in policeVergiList)
            {
                if (policeVergi.VergiKodu == 1) //Trafik Hizmetleri Gelistirme Fonu
                {
                    vergiEntity.TrafikHizmetleriGelistirmeFonu = policeVergi.VergiTutari;
                }
                else if (policeVergi.VergiKodu == 2) //Gider Vergisi
                {
                    vergiEntity.GiderVergisi = policeVergi.VergiTutari;
                }
                else if (policeVergi.VergiKodu == 3) // Garanti Fonu
                {
                    vergiEntity.GarantiFonu = policeVergi.VergiTutari;
                }
                else if (policeVergi.VergiKodu == 4) // Yangin Sigorta Vergisi
                {
                    vergiEntity.YanginSigortaVergisi = policeVergi.VergiTutari;
                }
            }
            return vergiEntity;
        }

        private string getAltAcenteunvan(int altAcenteKodu)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            string acenteUnvan = "";
            var altTvmDetay = _TVMService.GetDetay(altAcenteKodu);
            if (altTvmDetay != null)
            {
                acenteUnvan = altTvmDetay.Unvani;
            }

            return acenteUnvan;
        }

    }
}
