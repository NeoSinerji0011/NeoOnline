using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Common;


namespace Neosinerji.BABOnlineTP.Business
{
    public class CRService : ICRService
    {
        ICRContext _CRContext;
        IParametreContext _ParametreContext;

        public CRService(ICRContext CRContext, IParametreContext parameterContext)
        {
            _CRContext = CRContext;
            _ParametreContext = parameterContext;
        }

        #region Araç ek soru
        public List<CR_AracEkSoru> GetAracEkSoru(int tumKodu, string soruTipi)
        {
            return _CRContext.CR_AracEkSoruRepository.Filter(f => f.TUMKodu == tumKodu && f.SoruTipi == soruTipi).ToList<CR_AracEkSoru>();
        }
        #endregion

        #region Araç Kullanım Tarzı
        public List<CR_KullanimTarzi> GetKullanimTarzlari(int tumKodu)
        {
            return _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == tumKodu)
                                                        .OrderBy(o => o.TarifeKodu)
                                                        .ToList<CR_KullanimTarzi>();
        }

        public List<AracKullanimTarziServisModel> GetKullanimTarzi(int tumKodu)
        {
            return _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == tumKodu)
                                                        .OrderBy(o => o.TarifeKodu)
                                                        .Select(t => new AracKullanimTarziServisModel() { Kod = t.KullanimTarziKodu + "-" + t.Kod2, KullanimTarzi = t.Aciklama })
                                                        .ToList<AracKullanimTarziServisModel>();
        }

        public CR_KullanimTarzi GetKullanimTarzi(int tumKodu, string kullanimTarziKodu, string kod2)
        {
            return _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == tumKodu && f.KullanimTarziKodu == kullanimTarziKodu && f.Kod2 == kod2).FirstOrDefault();
        }
        #endregion

        #region Araç Grup Kodu
        public List<CR_AracGrup> GetAracGruplari(int tumKodu)
        {
            return _CRContext.CR_AracGrupRepository.Filter(f => f.TUMKodu == tumKodu)
                                                    .OrderBy(o => o.TarifeKodu)
                                                    .ToList<CR_AracGrup>();
        }

        public List<AracKullanimTarziServisModel> GetAracGrupKodlari(int tumKodu)
        {
            return _CRContext.CR_AracGrupRepository.Filter(f => f.TUMKodu == tumKodu)
                                                    .OrderBy(o => o.TarifeKodu)
                                                    .Select(t => new AracKullanimTarziServisModel() { Kod = t.KullanimTarziKodu + "-" + t.Kod2, KullanimTarzi = t.Aciklama })
                                                    .ToList<AracKullanimTarziServisModel>();
        }

        public CR_AracGrup GetAracGrupKodu(int tumKodu, string kullanimTarziKodu, string kod2)
        {
            return _CRContext.CR_AracGrupRepository.Filter(f => f.TUMKodu == tumKodu && f.KullanimTarziKodu == kullanimTarziKodu && f.Kod2 == kod2).FirstOrDefault();
        }
        #endregion

        #region İl İlçe
        public CR_IlIlce GetIlIlceByCr(int tumKodu, string crIlKodu, string crIlceKodu)
        {
            return _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == tumKodu && f.CRIlKodu == crIlKodu && f.CRIlceKodu == crIlceKodu).FirstOrDefault();
        }
        #endregion

        #region Ülke
        public List<CR_Ulke> GetUlkeler(int tumKodu)
        {
            return _CRContext.CR_UlkeRepository.Filter(f => f.TUMKodu == tumKodu).ToList<CR_Ulke>();
        }

        public CR_Ulke GetUlke(int tumKodu, string ulkeKodu)
        {
            return _CRContext.CR_UlkeRepository.All().FirstOrDefault(f => f.TUMKodu == tumKodu && f.UlkeKodu == ulkeKodu);
        }
        #endregion

        #region Tescil İl İlçe
        public CR_TescilIlIlce GetTescilIlIlce(int tumKodu, string ilKodu, string ilceKodu)
        {
            return _CRContext.CR_TescilIlIlceRepository.FindById(new object[] { tumKodu, ilKodu, ilceKodu });
        }

        public List<KeyValueItem<string, string>> GetTescilIlList()
        {
            List<KeyValueItem<string, string>> iller = _CRContext.CR_TescilIlIlceRepository
                                                                 .All()
                                                                 .Select(s => new KeyValueItem<string, string> { Key = s.IlKodu, Value = s.TescilIlAdi })
                                                                 .Distinct()
                                                                 .OrderBy(o => o.Key)
                                                                 .ToList();

            return iller;
        }

        public List<KeyValueItem<string, string>> GetTescilIlceList(string ilKodu)
        {
            List<KeyValueItem<string, string>> ilceler = _CRContext.CR_TescilIlIlceRepository
                                                                   .Filter(f => f.IlKodu == ilKodu)
                                                                   .Select(s => new KeyValueItem<string, string> { Key = s.IlceKodu, Value = s.TescilIlceAdi })
                                                                   .OrderBy(o => o.Value)
                                                                   .ToList();
            return ilceler;
        }

        #endregion

        #region Trafik IMM & FK

        public CR_TrafikIMM GetTrafikIMM(int tumKodu, short kademe)
        {
            return _CRContext.CR_TrafikIMMRepository.FindById(new object[] { tumKodu, kademe });
        }

        public CR_TrafikFK GetTrafikFK(int tumKodu, short kademe)
        {
            return _CRContext.CR_TrafikFKRepository.FindById(new object[] { tumKodu, kademe });
        }

        public List<KeyValueItem<short, string>> GetTrafikIMM()
        {
            List<KeyValueItem<short, string>> imm = _CRContext.CR_TrafikIMMRepository
                                                              .All()
                                                              .Select(s => new KeyValueItem<short, string> { Key = s.Kademe, Value = s.Text })
                                                              .OrderBy(o => o.Key)
                                                              .ToList();

            return imm;
        }

        public List<KeyValueItem<short, string>> GetTrafikFK()
        {
            List<KeyValueItem<short, string>> fk = _CRContext.CR_TrafikFKRepository
                                                             .All()
                                                             .Select(s => new KeyValueItem<short, string> { Key = s.Kademe, Value = s.Text })
                                                             .OrderBy(o => o.Key)
                                                             .ToList();

            return fk;
        }

        #endregion

        #region Kasko IMM & FK
        public CR_KaskoIMM GetKaskoIMM(int tumKodu, short kademe, string kullanimtarzikodu)
        {
            string[] parts = kullanimtarzikodu.Split('-');
            string kullanimT = parts[0];
            string kod2 = parts[1];
            string STRkademe = kademe.ToString();
            return _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == tumKodu &&
                                                           s.Kademe == STRkademe &&
                                                           s.KullanimTarziKodu == kullanimT &&
                                                           s.Kod2 == kod2).FirstOrDefault();
        }

        public CR_KaskoFK GetKaskoFK(int tumKodu, short kademe, string kullanimtarzikodu)
        {
            string[] parts = kullanimtarzikodu.Split('-');
            string kullanimT = parts[0];
            string kod2 = parts[1];
            string STRkademe = kademe.ToString();
            return _CRContext.CR_KaskoFKRepository.Filter(s => s.TUMKodu == tumKodu &&
                                                           s.Kademe == STRkademe &&
                                                           s.KullanimTarziKodu == kullanimT &&
                                                           s.Kod2 == kod2).FirstOrDefault();
        }

        public List<KeyValueItem<string, string>> GetKaskoIMMList(int tumkodu, string kullanimtarzikodu, string kod2)
        {
            List<KeyValueItem<string, string>> imm = _CRContext.CR_KaskoIMMRepository
                                                             .Filter(s => s.TUMKodu == tumkodu && s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                                             .Select(s => new KeyValueItem<string, string> { Key = s.Kademe, Value = s.Text })
                                                             .OrderBy(o => o.Key)
                                                             .ToList();

            return imm;
        }

        public List<KeyValueItem<string, string>> GetKaskoFKList(int tumkodu, string kullanimtarzikodu, string kod2)
        {
            List<KeyValueItem<string, string>> fk = _CRContext.CR_KaskoFKRepository
                                                             .Filter(s => s.TUMKodu == tumkodu && s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                                             .Select(s => new KeyValueItem<string, string> { Key = s.Kademe, Value = s.Text })
                                                             .OrderBy(o => o.Key)
                                                             .ToList();

            return fk;
        }
        #endregion

        #region Trafik IMM & FK //TrafikIMM, TrafikFK

        public List<KeyValueItem<string, string>> GetTrafikIMMListe(string kullanimtarzikodu, string kod2)
        {

            var immList = _CRContext.TrafikIMMRepository
                                    .Filter(s => s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                    .Select(s => new { Key = s.Id, Value = s.Text, s.BedeniSahis })
                                    .OrderBy(o => o.BedeniSahis)
                                    .ToList();
            List<KeyValueItem<string, string>> imm = new List<KeyValueItem<string, string>>();
            foreach (var item in immList)
            {
                imm.Add(new KeyValueItem<string, string>(item.Key.ToString(), item.Value));

            }

            return imm;
        }

        public List<KeyValueItem<string, string>> GetTrafikFKListe(string kullanimtarzikodu, string kod2)
        {
            var fkList = _CRContext.TrafikFKRepository
                                   .Filter(s => s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                   .Select(s => new { Key = s.Id, Value = s.Text, s.Vefat })
                                   .OrderBy(o => o.Vefat)
                                   .ToList();
            List<KeyValueItem<string, string>> fk = new List<KeyValueItem<string, string>>();
            foreach (var item in fkList)
            {
                fk.Add(new KeyValueItem<string, string>(item.Key.ToString(), item.Value));

            }
            return fk;
        }

        public TrafikIMM GetTrafikIMMBedel(int id, string kullanimTarziKod, string kod2)
        {
            return _CRContext.TrafikIMMRepository.Filter(s => s.Id == id && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).FirstOrDefault();

        }

        public CR_TrafikIMM GetCRTrafikIMMBedel(int TUMKodu, decimal? BedeniSahis, decimal? Kombine, string kullanimTarziKod, string kod2)
        {
            if (BedeniSahis > 0)
            {
                var imm = _CRContext.CR_TrafikIMMRepository
                      .Filter(s => s.TUMKodu == TUMKodu && (s.BedeniSahis == BedeniSahis || s.BedeniSahis < BedeniSahis) && s.BedeniSahis > 0 && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                      .OrderByDescending(s => s.BedeniSahis).FirstOrDefault();


                if (imm != null)
                {
                    return imm;
                }
                else
                {
                    return _CRContext.CR_TrafikIMMRepository
                    .Filter(s => s.TUMKodu == TUMKodu && (s.BedeniSahis == BedeniSahis || s.BedeniSahis > BedeniSahis) && s.BedeniSahis > 0 && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                    .OrderBy(s => s.BedeniSahis).FirstOrDefault();
                }

            }
            else
            {
                if (TUMKodu == TeklifUretimMerkezleri.HDI)
                {

                    var immKombine = _CRContext.CR_TrafikIMMRepository
                                                        .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                         .OrderByDescending(s => s.Kademe).FirstOrDefault();

                    if (immKombine != null)
                    {
                        return immKombine;
                    }
                    else
                    {
                        return _CRContext.CR_TrafikIMMRepository
                                                            .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                             .OrderBy(s => s.Kademe).FirstOrDefault();
                    }
                }

                else
                {
                    var immKombine = _CRContext.CR_TrafikIMMRepository.Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                        .OrderByDescending(s => s.Kombine)
                        .FirstOrDefault();
                    if (immKombine != null)
                    {
                        return immKombine;
                    }
                    else
                    {
                        return _CRContext.CR_TrafikIMMRepository.Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                        .OrderBy(s => s.Kombine)
                        .FirstOrDefault();
                    }
                }
            }

        }

        public TrafikFK GetTrafikFKBedel(int id, string kullanimTarziKod, string kod2)
        {
            return _CRContext.TrafikFKRepository.Filter(s => s.Id == id && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).FirstOrDefault();

        }

        public CR_TrafikFK GetCRTrafikFKBedel(int TUMKodu, decimal? Vefat, decimal? Tedavi, decimal? Kombine, string kullanimTarziKod, string kod2)
        {
            if (Vefat > 0)
            {
                var fk = _CRContext.CR_TrafikFKRepository
                    .Filter(s => s.TumKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat < Vefat) && s.Vefat > 0 && (s.Tedavi == Tedavi || s.Tedavi < Tedavi) && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                    .OrderByDescending(s => s.Tedavi).FirstOrDefault();

                if (fk != null)
                {
                    return fk;
                }
                else
                {
                    return _CRContext.CR_TrafikFKRepository
                       .Filter(s => s.TumKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat > Vefat) && s.Vefat > 0 && (s.Tedavi == Tedavi || s.Tedavi > Tedavi) && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                       .OrderBy(s => s.Tedavi).FirstOrDefault();
                }
            }
            else
            {
                var fkKombine = _CRContext.CR_TrafikFKRepository
                    .Filter(s => s.TumKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.Vefat == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                    .OrderByDescending(s => s.Kombine).FirstOrDefault();
                if (fkKombine != null)
                {
                    return fkKombine;
                }
                else
                {
                    return _CRContext.CR_TrafikFKRepository
                    .Filter(s => s.TumKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.Vefat == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                    .OrderBy(s => s.Kombine).FirstOrDefault();
                }
            }

        }


        #endregion

        #region Kasko IMM & FK //KaskoIMM, KaskoFK

        public List<KeyValueItem<string, string>> GetKaskoIMMListe(string kullanimtarzikodu, string kod2)
        {
            var immList = _CRContext.KaskoIMMRepository
                                    .Filter(s => s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                    .Select(s => new { Key = s.Id, Value = s.Text, s.BedeniSahis })
                                    .OrderBy(o => o.BedeniSahis)
                                    .ToList();
            List<KeyValueItem<string, string>> imm = new List<KeyValueItem<string, string>>();
            foreach (var item in immList)
            {
                imm.Add(new KeyValueItem<string, string>(item.Key.ToString(), item.Value));


            }
            return imm;
        }

        public List<KeyValueItem<string, string>> GetKaskoFKListe(string kullanimtarzikodu, string kod2)
        {
            var fkList = _CRContext.KaskoFKRepository
                                   .Filter(s => s.KullanimTarziKodu == kullanimtarzikodu && s.Kod2 == kod2)
                                   .Select(s => new { Key = s.Id, Value = s.Text, s.Vefat })
                                   .OrderBy(o => o.Vefat)
                                   .ToList();
            List<KeyValueItem<string, string>> fk = new List<KeyValueItem<string, string>>();
            foreach (var item in fkList)
            {
                fk.Add(new KeyValueItem<string, string>(item.Key.ToString(), item.Value));

            }
            return fk;
        }

        public KaskoIMM GetKaskoIMMBedel(int id, string kullanimTarziKod, string kod2)
        {
            return _CRContext.KaskoIMMRepository.Filter(s => s.Id == id && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).FirstOrDefault();

        }
        public bool IMMKombineManeviDahiMi(int id)
        {
            var kayit= _CRContext.KaskoIMMRepository.Filter(s => s.Id == id).FirstOrDefault();
            bool result = false;
            if (kayit!=null)
            {
                if (kayit.Text.Contains("Manevi Dahil"))
                {
                    result = true;
                }
            }
            return result;
        }

        public CR_KaskoIMM GetCRKaskoIMMBedel(int TUMKodu, decimal? BedeniSahis, decimal? Kombine, string kullanimTarziKod, string kod2)
        {
            if (BedeniSahis > 0)
            {
                var imm = _CRContext.CR_KaskoIMMRepository
                                                        .Filter(s => s.TUMKodu == TUMKodu && (s.BedeniSahis == BedeniSahis || s.BedeniSahis < BedeniSahis) && s.BedeniSahis > 0 && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                        .OrderByDescending(s => s.BedeniSahis).FirstOrDefault();
                if (imm != null)
                {
                    return imm;
                }
                else
                {
                    return _CRContext.CR_KaskoIMMRepository
                                                            .Filter(s => s.TUMKodu == TUMKodu && (s.BedeniSahis == BedeniSahis || s.BedeniSahis > BedeniSahis) && s.BedeniSahis > 0 && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                            .OrderBy(s => s.BedeniSahis).FirstOrDefault();
                }
            }
            else
            {
                if (TUMKodu == TeklifUretimMerkezleri.HDI)
                {

                    var immKombine = _CRContext.CR_KaskoIMMRepository
                                                        .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                         .OrderByDescending(s => s.Kademe).FirstOrDefault();

                    if (immKombine != null)
                    {
                        return immKombine;
                    }
                    else
                    {
                        return _CRContext.CR_KaskoIMMRepository
                                                            .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                             .OrderBy(s => s.Kademe).FirstOrDefault();
                    }
                }
                else
                {

                    var immKombine = _CRContext.CR_KaskoIMMRepository
                                                           .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                             .OrderByDescending(s => s.Kombine).FirstOrDefault();
                    if (immKombine != null)
                    {
                        return immKombine;
                    }
                    else
                    {
                        return _CRContext.CR_KaskoIMMRepository
                                                               .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.BedeniSahis == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2)
                                                                 .OrderBy(s => s.Kombine).FirstOrDefault();
                    }
                }
            }

        }

        public KaskoFK GetKaskoFKBedel(int id, string kullanimTarziKod, string kod2)
        {
            return _CRContext.KaskoFKRepository.Filter(s => s.Id == id && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).FirstOrDefault();

        }

        public CR_KaskoFK GetCRKaskoFKBedel(int TUMKodu, decimal? Vefat, decimal? Tedavi, decimal? Kombine, string kullanimTarziKod, string kod2)
        {
            if (Vefat > 0)
            {
                CR_KaskoFK fk = new CR_KaskoFK();
                if (TUMKodu!=TeklifUretimMerkezleri.GULF)
                {
                    fk = _CRContext.CR_KaskoFKRepository
                                                                        .Filter(s => s.TUMKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat < Vefat) && s.Vefat > 0 && (s.Tedavi == Tedavi || s.Tedavi < Tedavi) && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderByDescending(s => s.Tedavi)
                                                                        .FirstOrDefault();
                }
                else
                {
                   fk= _CRContext.CR_KaskoFKRepository
                                                                      .Filter(s => s.TUMKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat < Vefat) && s.Vefat > 0 && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderByDescending(s => s.Vefat)
                                                                      .FirstOrDefault();
                }               

                if (fk != null)
                {
                    return fk;
                }
                else
                {
                    if (TUMKodu != TeklifUretimMerkezleri.GULF)
                    {
                        fk = _CRContext.CR_KaskoFKRepository
                                                     .Filter(s => s.TUMKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat > Vefat) && s.Vefat > 0 && (s.Tedavi == Tedavi || s.Tedavi > Tedavi) && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderBy(s => s.Tedavi)
                                                     .FirstOrDefault();
                    }
                    else
                    {
                        fk = _CRContext.CR_KaskoFKRepository
                                                                            .Filter(s => s.TUMKodu == TUMKodu && (s.Vefat == Vefat || s.Vefat > Vefat) && s.Vefat > 0  && s.Kombine == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderBy(s => s.Vefat)
                                                                            .FirstOrDefault();
                    }
                    return fk;
                }
            }
            else
            {
                var fkKombine = _CRContext.CR_KaskoFKRepository
                                                .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine < Kombine) && s.Kombine > 0 && s.Vefat == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderByDescending(s => s.Kombine)
                                                .FirstOrDefault();

                if (fkKombine != null)
                {
                    return fkKombine;
                }
                else
                {
                    fkKombine = _CRContext.CR_KaskoFKRepository
                                               .Filter(s => s.TUMKodu == TUMKodu && (s.Kombine == Kombine || s.Kombine > Kombine) && s.Kombine > 0 && s.Vefat == 0 && s.KullanimTarziKodu == kullanimTarziKod && s.Kod2 == kod2).OrderBy(s => s.Kombine)
                                               .FirstOrDefault();
                    return fkKombine;
                }
            }
        }

        #endregion

        #region Kasko AMS (MAPFRE)
        public CR_KaskoAMS GetKaskoAMS(int tumKodu, int tvmKodu, string amsKodu)
        {
            return _CRContext.CR_KaskoAMSRepository.Filter(f => f.TUMKodu == tumKodu &&
                                                                (f.TVMKodu == tvmKodu || f.TVMKodu == 999999) &&
                                                                f.AMSKodu == amsKodu)
                                                    .FirstOrDefault();
        }

        public List<KeyValueItem<string, string>> GetKaskoAMSList(int tumKodu, int tvmKodu, string kullanimtarzikodu, string kod2)
        {
            CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.All()
                                                                                  .Where(w => w.TUMKodu == tumKodu &&
                                                                                              w.KullanimTarziKodu == kullanimtarzikodu &&
                                                                                              w.Kod2 == kod2)
                                                                                  .FirstOrDefault();

            if (kullanimTarzi != null)
            {
                string tarifeKodu = kullanimTarzi.TarifeKodu;

                List<KeyValueItem<string, string>> ams = _CRContext.CR_KaskoAMSRepository.All()
                                                                                         .Where(w => w.TUMKodu == tumKodu &&
                                                                                                     (w.TVMKodu == tvmKodu || w.TVMKodu == 999999) &&
                                                                                                     w.KullanimTarziKodu == tarifeKodu)
                                                                                         .Select(s => new KeyValueItem<string, string> { Key = s.AMSKodu, Value = s.Aciklama })
                                                                                         .OrderBy(o => o.Key)
                                                                                         .ToList();

                return ams;
            }

            return null;
        }
        #endregion

        #region Kasko İkame Türü (MAPFRE)
        public CR_KaskoIkameTuru GetKaskoIkameTuru(int tumKodu, string tarifeKodu, string ikameTuruKodu)
        {
            return _CRContext.CR_KaskoIkameTuruRepository.All().FirstOrDefault(f => f.TUMKodu == tumKodu && f.TarifeKodu == tarifeKodu && f.IkameTuruKodu == ikameTuruKodu);
        }

        public List<KeyValueItem<string, string>> GetKaskoIkameTuruList(int tumKodu, string tarifeKodu)
        {
            return _CRContext.CR_KaskoIkameTuruRepository.All()
                                                         .Where(w => w.TUMKodu == tumKodu &&
                                                                     w.TarifeKodu == tarifeKodu)
                                                         .Select(s => new KeyValueItem<string, string> { Key = s.IkameTuruKodu, Value = s.IkameTuru })
                                                         .OrderBy(o => o.Key)
                                                         .ToList();
        }
        #endregion

        #region Kasko Dain-i Murtein (MAPFRE)
        public List<CR_KaskoDM> GetKaskoDMListe(int tumKodu, int kurumTipi)
        {
            return _CRContext.CR_KaskoDMRepository.All()
                                                  .Where(w => w.TUMKodu == tumKodu && w.KurumTipi == kurumTipi)
                                                  .OrderBy(o => o.KurumAdi)
                                                  .ToList();
        }

        public CR_KaskoDM GetKaskoDM(int tumKodu, int kurumTipi, string kurumKodu)
        {
            return _CRContext.CR_KaskoDMRepository.All()
                                                  .FirstOrDefault(f => f.TUMKodu == tumKodu &&
                                                                       f.KurumTipi == kurumTipi &&
                                                                       f.KurumKodu == kurumKodu);
        }

        public CR_KaskoDM GetKaskoDMAd(int tumKodu, string kurumAdi)
        {
            return _CRContext.CR_KaskoDMRepository.All()
                                                  .FirstOrDefault(f => f.TUMKodu == tumKodu &&
                                                                       f.KurumAdi == kurumAdi);
        }
        #endregion

        #region TUM Müşteri No
        public void InsertTUMMusteri(int tumKodu, int musteriKodu, string tumMusteriNo)
        {
            CR_TUMMusteri tumMusteri = new CR_TUMMusteri();
            tumMusteri.TUMKodu = tumKodu;
            tumMusteri.MusteriKodu = musteriKodu;
            tumMusteri.TUMMusteriKodu = tumMusteriNo;

            tumMusteri = _CRContext.CR_TUMMusteriRepository.Create(tumMusteri);
            _CRContext.Commit();
        }

        public string GetTUMMusteriKodu(int tumKodu, int musteriKodu)
        {
            string MUSTERI_NO = String.Empty;

            CR_TUMMusteri tumMusteri = _CRContext.CR_TUMMusteriRepository.FindById(new object[] { tumKodu, musteriKodu });
            if (tumMusteri != null)
            {
                MUSTERI_NO = tumMusteri.TUMMusteriKodu;
            }

            return MUSTERI_NO;
        }
        #endregion

        #region Dask

        public List<DaskKurumlar> GetListDaskKurumlar()
        {
            return _ParametreContext.DaskKurumlarRepository.All().OrderBy(s => s.KurumAdi).ToList<DaskKurumlar>();
        }

        public DaskKurumlar GetDaskKurum(int KurumKodu)
        {
            return _ParametreContext.DaskKurumlarRepository.Filter(s => s.KurumKodu == KurumKodu).FirstOrDefault();
        }

        public List<DaskSubeler> GetListDaskSubeler(int KurumKodu)
        {
            return _ParametreContext.DaskSubelerRepository.Filter(s => s.KurumKodu == KurumKodu).OrderBy(s => s.SubeAdi).ToList<DaskSubeler>();
        }

        public DaskSubeler GetDaskSube(int KurumKodu, int SubeKodu)
        { return _ParametreContext.DaskSubelerRepository.Filter(s => s.KurumKodu == KurumKodu & s.SubeKodu == SubeKodu).FirstOrDefault(); }

        public List<DaskIl> GetListDaskIller()
        {
            return _ParametreContext.DaskIlRepository.All().ToList<DaskIl>();
        }

        public List<DaskIlce> GetListDaskIlceler(int IlKodu)
        {
            return _ParametreContext.DaskIlceRepository.Filter(s => s.IlKodu == IlKodu).ToList<DaskIlce>();
        }

        public DaskIl GetDaskIl(int IlKodu)
        {
            return _ParametreContext.DaskIlRepository.Filter(s => s.IlKodu == IlKodu).FirstOrDefault();
        }

        public DaskIlce GetDaskIlce(int IlceKodu)
        {
            return _ParametreContext.DaskIlceRepository.Filter(s => s.IlceKodu == IlceKodu).FirstOrDefault();
        }

        public DaskIlce GetDaskIlce(int Ilkodu, string IlceAdi)
        {
            return _ParametreContext.DaskIlceRepository.Filter(s => s.IlceAdi == IlceAdi & s.IlKodu == Ilkodu).FirstOrDefault();
        }

        public DaskBelde GetDaskBelde(int BeldeKodu)
        {
            return _ParametreContext.DaskBeldeRepository.Filter(s => s.BeldeKodu == BeldeKodu).FirstOrDefault();
        }

        public DaskBelde GetDaskBelde(int IlKodu, int IlceKodu, string BeldeAdi)
        {
            return _ParametreContext.DaskBeldeRepository.Filter(s => s.BeldeAdi == BeldeAdi &
                                                                     s.IlKodu == IlKodu &
                                                                     s.IlceKodu == IlceKodu).FirstOrDefault();
        }

        public List<DaskBelde> GetListDaskBeldeler(int IlKodu, int IlceKodu)
        {
            return _ParametreContext.DaskBeldeRepository.Filter(s => s.IlKodu == IlKodu & s.IlceKodu == IlceKodu).ToList<DaskBelde>();
        }

        #endregion

        #region Kredili hayat çarpan tablosu
        public CR_KrediHayatCarpan GetKrediHayatCarpan(int tumKodu, string tipKodu, int musteriYas, int krediVade)
        {
            return _CRContext.CR_KrediHayatCarpanRepository.Filter(f => f.TUMKodu == tumKodu && f.TipKodu == tipKodu &&
                                                                    f.MusteriYas == musteriYas && f.KrediVade == krediVade)
                                                           .FirstOrDefault();
        }
        #endregion

        #region Seyehat Sağlık

        public List<UlkeKodlari> GetSeyehatUlkeleri(bool schengenMi)
        {
            if (schengenMi)
                return _ParametreContext.UlkeKodlariRepository.Filter(s => s.SchengenMi == 1 & s.Garanti == 1).OrderBy(s => s.UlkeAdi).ToList<UlkeKodlari>();
            else
                return _ParametreContext.UlkeKodlariRepository.Filter(s => s.SchengenMi == 0 & s.Garanti == 1).OrderBy(s => s.UlkeAdi).ToList<UlkeKodlari>();
        }

        public UlkeKodlari GetSeyehatUlkesi(string ulkeKodu)
        {
            return _ParametreContext.UlkeKodlariRepository.Filter(s => s.UlkeKodu == ulkeKodu).FirstOrDefault();
        }

        public decimal GetPrimTutari(int SigortaliYasi, int SeyehatGunSayisi, bool schengenMi, byte? planKodu)
        {
            decimal prim = 0;

            if (schengenMi)
            {
                SchengenUlkeOranlari primOrani = _ParametreContext.SchengenUlkeOranlariRepository.Filter(s => s.Yas1 <= SigortaliYasi & s.Yas2 >= SigortaliYasi &
                                                                                                              s.Gun1 <= SeyehatGunSayisi & s.Gun2 >= SeyehatGunSayisi)
                                                                                                                                                    .FirstOrDefault();
                if (primOrani != null)
                {
                    prim = prim + primOrani.Oran;
                }
            }
            else
            {
                if (planKodu.HasValue)
                {
                    DigerUlkeOranlari primOrani = _ParametreContext.DigerUlkeOranlariRepository.Filter(s => s.Gun1 <= SeyehatGunSayisi &
                                                                                                  s.Gun2 >= SeyehatGunSayisi &
                                                                                                  s.PlanTipi == planKodu).FirstOrDefault();


                    if (primOrani != null)
                        prim = prim + primOrani.Oran;
                    if (SigortaliYasi > 66 & SigortaliYasi < 71)
                        prim = prim + ((prim / 100) * 75);

                    if (SigortaliYasi > 70 & SigortaliYasi < 76)
                        prim = prim + ((prim / 100) * 150);


                    /*Yaş 1	Yaş 2	Oran
                        0,5	65	0
                        66	70	75
                        71	75	150
                    */
                }
            }

            return prim;
        }

        #endregion

        #region Konut

        public List<BelediyeIl> GetListBelediyeIl()
        {
            return _ParametreContext.BelediyeIlRepository.All().ToList<BelediyeIl>();
        }

        public List<Belediye> GetListBelediye(int IlKodu)
        {
            return _ParametreContext.BelediyeRepository.Filter(s => s.IlKodu == IlKodu).OrderBy(s => s.BelediyeAdi).ToList<Belediye>();
        }

        public Belediye GetBelediye(int IlKodu, int BelediyeKodu)
        {
            return _ParametreContext.BelediyeRepository.Filter(s => s.IlKodu == IlKodu & s.BelediyeKodu == BelediyeKodu).FirstOrDefault();
        }

        public BelediyeIl GetBelediyeIl(int IlKodu)
        {
            return _ParametreContext.BelediyeIlRepository.Filter(s => s.IlKodu == IlKodu).FirstOrDefault();
        }

        public List<DepremMuafiyet> GetListDepremMuafiyet()
        {
            return _ParametreContext.DepremMuafiyetRepository.All().ToList<DepremMuafiyet>();
        }

        public DepremMuafiyet GetDepremMuafiyet(int kod, string Value)
        {
            string[] array = Value.Split('-');
            if (array.Length == 2)
            {
                int yazlikKislik = Convert.ToInt32(array[0]);
                int kademe = Convert.ToInt32(array[1]);
                return _ParametreContext.DepremMuafiyetRepository.Filter(s => s.TeminatKodu == kod &
                                                                              s.YazlikKislik == yazlikKislik &&
                                                                              s.Kademe == kademe).FirstOrDefault();
            }
            return null;
        }

        public string GetDepremMuafiyetText(int kod, string Value)
        {
            string[] array = Value.Split('-');
            if (array.Length == 2)
            {
                int yazlikKislik = Convert.ToInt32(array[0]);
                int kademe = Convert.ToInt32(array[1]);

                DepremMuafiyet muafiyet = _ParametreContext.DepremMuafiyetRepository.Filter(s => s.TeminatKodu == kod &
                                                                                              s.YazlikKislik == yazlikKislik &&
                                                                                              s.Kademe == kademe).FirstOrDefault();

                string result = String.Empty;

                if (muafiyet != null)
                {
                    result = muafiyet.YazlikKislik == 2 ? ResourceHelper.GetString("Summery") : ResourceHelper.GetString("WinterWeight");
                    result += " " + muafiyet.Kademe;
                }
                return result;
            }
            return String.Empty;
        }

        #endregion

        #region Is YEri

        public List<Istigal> GetListIstigal()
        {
            return _ParametreContext.IstigalRepository.All().OrderBy(s => s.Aciklama).ToList<Istigal>();
        }
        public Istigal GetIstigal(int kodu)
        {
            return _ParametreContext.IstigalRepository.Filter(s => s.Kod == kodu).FirstOrDefault();
        }

        #endregion
    }
}
