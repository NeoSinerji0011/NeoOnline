using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business
{
    public class AracService : IAracService
    {
        IAracContext _AracContext;
        ICRContext _CRContext;

        public AracService(IAracContext aracContext, ICRContext cRContext)
        {
            _AracContext = aracContext;
            _CRContext = cRContext;
        }

        #region AracKullanimSekli
        public AracKullanimSekli GetAracKullanimSekli(short kullanimSekliKodu)
        {
            return _AracContext.AracKullanimSekliRepository.FindById(kullanimSekliKodu);
        }

        public List<AracKullanimSekli> GetAracKullanimSekliList()
        {
            return _AracContext.AracKullanimSekliRepository.All().ToList<AracKullanimSekli>();
        }
        #endregion

        #region AracKullanimTarzi

        public AracKullanimTarzi GetAracKullanimTarzi(string kullanimTarziKodu, string kod2)
        {
            return _AracContext.AracKullanimTarziRepository.FindById(new object[] { kullanimTarziKodu, kod2 });
        }

        public List<AracKullanimTarzi> GetAracKullanimTarziList()
        {
            return _AracContext.AracKullanimTarziRepository.Filter(f => f.Durum == 1).ToList<AracKullanimTarzi>();
        }

        public List<AracKullanimTarzi> GetAracKullanimTarziList(short kullanimSekliKodu)
        {
            return _AracContext.AracKullanimTarziRepository.Filter(f => f.Durum == 1 && f.KullanimSekliKodu == kullanimSekliKodu).ToList<AracKullanimTarzi>();
        }

        public List<AracKullanimTarziServisModel> GetAracKullanimTarziTeklif(short kullanimSekliKodu)
        {
            return _AracContext.AracKullanimTarziRepository.Filter(f => f.Durum == 1 && f.KullanimSekliKodu == kullanimSekliKodu)
                                                           .Select(t => new AracKullanimTarziServisModel() { Kod = t.KullanimTarziKodu + "-" + t.Kod2, KullanimTarzi = t.KullanimTarzi })
                                                           .ToList<AracKullanimTarziServisModel>();
        }
        #endregion

        #region AracMarka
        public AracMarka GetAracMarka(string markaKodu)
        {
            return _AracContext.AracMarkaRepository.FindById(markaKodu);
        }

        public List<AracMarka> GetAracMarkaList(string kullanimTarziKodu)
        {
            IQueryable<AracTip> tipler = _AracContext.AracTipRepository.Filter(f => f.KullanimSekli1 == kullanimTarziKodu ||
                                                                                    f.KullanimSekli2 == kullanimTarziKodu ||
                                                                                    f.KullanimSekli3 == kullanimTarziKodu ||
                                                                                    f.KullanimSekli4 == kullanimTarziKodu);
            IQueryable<AracMarka> markalar = _AracContext.AracMarkaRepository.All();

            var list = (from m in markalar
                        join t in tipler on m.MarkaKodu equals t.MarkaKodu
                        select m).Distinct().OrderBy(o => o.MarkaAdi);

            return list.ToList<AracMarka>();
        }

        public List<AracMarka> GetAracMarkaList(string[] kullanimTarziKodlari)
        {
            IQueryable<AracTip> tipler = _AracContext.AracTipRepository.All();

            var filteredTipler = from t in tipler
                                 where kullanimTarziKodlari.Contains(t.KullanimSekli1) ||
                                       kullanimTarziKodlari.Contains(t.KullanimSekli2) ||
                                       kullanimTarziKodlari.Contains(t.KullanimSekli3) ||
                                       kullanimTarziKodlari.Contains(t.KullanimSekli4)
                                 select t;

            IQueryable<AracMarka> markalar = _AracContext.AracMarkaRepository.All();

            var list = (from m in markalar
                        join t in filteredTipler on m.MarkaKodu equals t.MarkaKodu
                        select m).Distinct().OrderBy(o => o.MarkaAdi);

            return list.ToList<AracMarka>();
        }

        public List<AracMarka> GetAracMarkaList()
        {
            return _AracContext.AracMarkaRepository.All().OrderBy(o => o.MarkaAdi).ToList();
        }
        #endregion

        #region AracTip
        public AracTip GetAracTip(string markaKodu, string tipKodu)
        {
            return _AracContext.AracTipRepository.FindById(new object[] { markaKodu, tipKodu });
        }

        public List<AracTip> GetAracTipList()
        {
            return _AracContext.AracTipRepository.All().ToList();
        }

        public List<AracTip> GetAracTipList(string markaKodu)
        {
            return _AracContext.AracTipRepository.Filter(m => m.MarkaKodu == markaKodu).OrderBy(o => o.TipAdi).ToList();
        }

        public List<AracTip> GetAracTipList(string kullanimTarziKodu, string markaKodu)
        {
            return _AracContext.AracTipRepository.Filter(f => f.MarkaKodu == markaKodu &&
                                                             (f.KullanimSekli1 == kullanimTarziKodu ||
                                                              f.KullanimSekli2 == kullanimTarziKodu ||
                                                              f.KullanimSekli3 == kullanimTarziKodu ||
                                                              f.KullanimSekli4 == kullanimTarziKodu)).ToList<AracTip>();
        }

        public List<AracTip> GetAracTipList(string kullanimTarziKodu, string markaKodu, int model)
        {
            IQueryable<AracTip> tipler = _AracContext.AracTipRepository.All();
            IQueryable<AracModel> modeller = _AracContext.AracModelRepository.All();

            var tip = (from t in tipler
                       join m in modeller on new { t.MarkaKodu, t.TipKodu } equals new { m.MarkaKodu, m.TipKodu }
                       where t.MarkaKodu == markaKodu
                             && (t.KullanimSekli1 == kullanimTarziKodu ||
                                 t.KullanimSekli2 == kullanimTarziKodu ||
                                 t.KullanimSekli3 == kullanimTarziKodu ||
                                 t.KullanimSekli4 == kullanimTarziKodu)
                             && m.Model == model
                       select t).Distinct().OrderBy(o => o.TipAdi);

            return tip.ToList<AracTip>();
        }

        public List<AracTip> GetAracTipList(string[] kullanimTarziKodlari, string markaKodu, int model)
        {
            IQueryable<AracTip> tipler = _AracContext.AracTipRepository.All();
            IQueryable<AracModel> modeller = _AracContext.AracModelRepository.All();

            var tip = (from t in tipler
                       join m in modeller on new { t.MarkaKodu, t.TipKodu } equals new { m.MarkaKodu, m.TipKodu }
                       where t.MarkaKodu == markaKodu
                             && (kullanimTarziKodlari.Contains(t.KullanimSekli1) ||
                                 kullanimTarziKodlari.Contains(t.KullanimSekli2) ||
                                 kullanimTarziKodlari.Contains(t.KullanimSekli3) ||
                                 kullanimTarziKodlari.Contains(t.KullanimSekli4))
                             && m.Model == model
                       select t).Distinct().OrderBy(o => o.TipAdi);

            return tip.ToList<AracTip>();
        }

        #endregion

        #region AracModel
        public List<AracModel> GetAracModelList(string markaKodu, string tipKodu)
        {
            return _AracContext.AracModelRepository.Filter(f => f.MarkaKodu == markaKodu && f.TipKodu == tipKodu).ToList<AracModel>();
        }

        public AracModel GetAracModel(string markaKodu, string tipKodu, int model)
        {
            return _AracContext.AracModelRepository.Find(f => f.MarkaKodu == markaKodu && f.TipKodu == tipKodu && f.Model == model);
        }
        #endregion

        public decimal GetAracDeger(string markaKodu, string tipkodu, int model)
        {
            AracModel aracModel = _AracContext.AracModelRepository.Filter(s => s.MarkaKodu == markaKodu && s.Model == model && s.TipKodu == tipkodu).FirstOrDefault();
            if (aracModel != null && aracModel.Fiyat.HasValue)
            {
                return aracModel.Fiyat.Value;
            }
            else return 0;
        }

        public short GetAracKisiSayisi(string markaKodu, string tipKodu)
        {
            AracTip aracTip = _AracContext.AracTipRepository.Filter(s => s.MarkaKodu == markaKodu && s.TipKodu == tipKodu).FirstOrDefault();
            if (aracTip != null && aracTip.KisiSayisi.HasValue)
            { return aracTip.KisiSayisi.Value; }
            else return 0;
        }

        #region Karsilastirmali Ortam

        public MapfreToHdiPlakaSorgu KarsilastirmaliPlakaSorgu_FillMapfre(PoliceSorguTrafikResponse response)
        {
            MapfreToHdiPlakaSorgu model = new MapfreToHdiPlakaSorgu();

            try
            {
                #region Set Model

                DateTime eskiPoliceTarih = response.trafikHasar.hasarBilgi.Where(w => w.PoliceBitisTarihi < DateTime.Today.AddDays(30))
                                                                          .DefaultIfEmpty(new TrafikHasarBilgi() { polBitisTarihi = "01/01/1900" })
                                                                          .Max(m => m.PoliceBitisTarihi);

                DateTime maxPoliceTarih = response.trafikHasar.hasarBilgi.Max(m => m.PoliceBitisTarihi);
                TrafikHasarBilgi eskiPoliceBilgi = response.trafikHasar.hasarBilgi.FirstOrDefault(w => w.PoliceBitisTarihi == eskiPoliceTarih);
                TrafikHasarBilgi bilgi = response.trafikHasar.hasarBilgi.FirstOrDefault(w => w.PoliceBitisTarihi == maxPoliceTarih);

                #endregion

                #region Eski Poliçe

                if (eskiPoliceBilgi != null)
                {
                    model.EskiPoliceSigortaSirkedKodu = eskiPoliceBilgi.sirketKodu;
                    model.EskiPoliceAcenteKod = eskiPoliceBilgi.acenteKod;
                    model.EskiPoliceNo = eskiPoliceBilgi.policeNo;
                    model.EskiPoliceYenilemeNo = eskiPoliceBilgi.yenilemeNo;
                    if (!String.IsNullOrEmpty(eskiPoliceBilgi.polBitisTarihi))
                    {
                        DateTime policeBitis = MapfreSorguResponse.ToDateTime(eskiPoliceBilgi.polBitisTarihi);

                        if (policeBitis < TurkeyDateTime.Today)
                            model.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                        else
                            model.YeniPoliceBaslangicTarih = policeBitis.ToString("dd.MM.yyyy");
                    }
                }
                else
                {
                    model.YeniPoliceBaslangicTarih = TurkeyDateTime.Today.ToString("dd.MM.yyyy");
                }

                model.PoliceBitisTarihi = maxPoliceTarih.ToString("dd.MM.yyy");

                #endregion

                #region Bilgi

                if (bilgi != null)
                {
                    int modelYili = 0;
                    int.TryParse(bilgi.modelYili, out modelYili);

                    model.AracModelYili = bilgi.modelYili;
                    model.AracTescilTarih = MapfreSorguResponse.ToDateTime(bilgi.tescilTarihi).ToString("dd.MM.yyyy");
                    model.AracSasiNo = bilgi.sasiNo;
                    model.AracMotorNo = bilgi.motorNo;
                    model.motorGucu = bilgi.motorGucu;
                    model.silindirHacmi = bilgi.silindirHacmi;
                    model.belgeNo = bilgi.belgeNo;
                    if (!String.IsNullOrEmpty(bilgi.belgeTarihi))
                    {
                        model.belgeTarihi = MapfreSorguResponse.ToDateTime(bilgi.belgeTarihi).ToString("dd.MM.yyyy");
                    }                   

                    #region Arac Tarife Grup Kodu

                    if (!String.IsNullOrEmpty(bilgi.aracTarifeGrupKod))
                    {
                        CR_AracGrup aracGrup = _CRContext.CR_AracGrupRepository.Filter(s => s.TarifeKodu == bilgi.aracTarifeGrupKod &&
                                                                                            s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();

                        if (aracGrup != null)
                        {
                            AracKullanimTarzi tarzi = _AracContext.AracKullanimTarziRepository.Filter(s => s.Kod2 == aracGrup.Kod2 &&
                                                                                                           s.KullanimTarziKodu == aracGrup.KullanimTarziKodu)
                                                                                              .FirstOrDefault();
                            if (tarzi != null)
                            {
                                if (tarzi.KullanimSekliKodu.HasValue)
                                {
                                    model.AracMarkaKodu = bilgi.marka;
                                    model.AracTipKodu = bilgi.model.TrimStart('0');
                                    model.AracKullanimSekli = tarzi.KullanimSekliKodu.ToString();
                                    model.AracKullanimTarzi = String.Format("{0}-{1}", tarzi.KullanimTarziKodu, tarzi.Kod2);

                                    // LİST
                                    model.TarzList = GetAracKullanimTarziTeklif(tarzi.KullanimSekliKodu.Value);
                                    model.Markalar = GetAracMarkaList(tarzi.KullanimTarziKodu);
                                    model.Tipler = GetAracTipList(tarzi.KullanimTarziKodu, model.AracMarkaKodu, modelYili);
                                }
                            }
                        }
                    }

                    model.HasarsizlikKademe = bilgi.uygulanmasiGerekenBasamakKodu;
                    model.UygulanmisHasarsizlikKademe = bilgi.uygulanmisBasamakKodu;
                    if (model.HasarsizlikKademe == "1")
                    {
                        model.HasarsizlikInd = "0";
                        model.HasarsizlikSur = "40";
                    }
                    else if (model.HasarsizlikKademe == "2")
                    {
                        model.HasarsizlikInd = "0";
                        model.HasarsizlikSur = "20";
                    }
                    else if (model.HasarsizlikKademe == "3")
                    {
                        model.HasarsizlikInd = "0";
                        model.HasarsizlikSur = "10";
                    }
                    else if (model.HasarsizlikKademe == "4")
                    {
                        model.HasarsizlikInd = "0";
                        model.HasarsizlikSur = "0";
                    }
                    else if (model.HasarsizlikKademe == "5")
                    {
                        model.HasarsizlikInd = "10";
                        model.HasarsizlikSur = "0";
                    }
                    else if (model.HasarsizlikKademe == "6")
                    {
                        model.HasarsizlikInd = "15";
                        model.HasarsizlikSur = "0";
                    }
                    else if (model.HasarsizlikKademe == "7")
                    {
                        model.HasarsizlikInd = "20";
                        model.HasarsizlikSur = "0";
                    }
                    #endregion
                }

                #endregion
            }
            catch (Exception)
            {
            }

            return model;
        }

        public MapfreToHdiPlakaSorgu KarsilastirmaliPlakaSorgu_FillMapfreKasko(TramerSorguPoliceValue bilgi)
        {
            MapfreToHdiPlakaSorgu model = new MapfreToHdiPlakaSorgu();

            try
            {
                if (bilgi != null)
                {
                    #region AracGrupKodu

                    if (!String.IsNullOrEmpty(bilgi.aracTarifeGrupKodu))
                    {
                        int modelYili = 0;
                        int.TryParse(bilgi.modelYili, out modelYili);

                        CR_AracGrup aracGrup = _CRContext.CR_AracGrupRepository.Filter(s => s.TarifeKodu == bilgi.aracTarifeGrupKodu &&
                                                                                            s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();

                        if (aracGrup != null)
                        {
                            AracKullanimTarzi tarzi = _AracContext.AracKullanimTarziRepository.Filter(s => s.Kod2 == aracGrup.Kod2 &&
                                                                                                           s.KullanimTarziKodu == aracGrup.KullanimTarziKodu)
                                                                                               .FirstOrDefault();
                            if (tarzi != null && tarzi.KullanimSekliKodu.HasValue)
                            {
                                model.AracMarkaKodu = bilgi.aracMarkaKodu;
                                model.AracKullanimSekli = tarzi.KullanimSekliKodu.ToString();
                                model.AracKullanimTarzi = String.Format("{0}-{1}", tarzi.KullanimTarziKodu, tarzi.Kod2);

                                // LİST
                                model.TarzList = GetAracKullanimTarziTeklif(tarzi.KullanimSekliKodu.Value);
                                model.Markalar = GetAracMarkaList(tarzi.KullanimTarziKodu);
                                model.Tipler = GetAracTipList(tarzi.KullanimTarziKodu, model.AracMarkaKodu, modelYili);
                            }

                        }
                    }

                    #endregion
                }
            }
            catch (Exception)
            {
            }

            return model;
        }

        #endregion
    }
}

