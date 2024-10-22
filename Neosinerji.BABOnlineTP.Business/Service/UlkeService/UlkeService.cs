using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Business.Service;

namespace Neosinerji.BABOnlineTP.Business
{
    public class UlkeService : IUlkeService
    {
        IParametreContext _Context;
        ICRContext _CRContext;

        public UlkeService(IParametreContext unitOfWork, ICRContext CRContext )
        {
            _Context = unitOfWork;
            _CRContext = CRContext;
        }

        #region Ulke
        public Ulke GetUlke(string ulkeKodu)
        {
            return _Context.UlkeRepository.FindById(ulkeKodu);
        }

        public string GetUlkeAdi(string ulkeKodu)
        {
            string result = String.Empty;

            Ulke ulke = _Context.UlkeRepository.Find(s => s.UlkeKodu == ulkeKodu);
            if (ulke != null)
                result = ulke.UlkeAdi;

            return result;
        }

        public List<Ulke> GetUlkeList()
        {
            return _Context.UlkeRepository.All().ToList<Ulke>();
        }

        public Ulke CreateUlke(Ulke ulke)
        {
            ulke = _Context.UlkeRepository.Create(ulke);
            _Context.Commit();
            return ulke;
        }

        public void UpdateUlke(Ulke ulke)
        {
            _Context.UlkeRepository.Update(ulke);
            _Context.Commit();
        }
        #endregion

        #region İl
        public Il GetIl(string ulkeKodu, string ilKodu)
        {
            return _Context.IlRepository.Find(f => f.UlkeKodu == ulkeKodu &&
                                                   f.IlKodu == ilKodu);
        }

        public string GetIlAdi(string ulkeKodu, string ilKodu)
        {
            string result = String.Empty;

            Il il = _Context.IlRepository.Filter(s => s.UlkeKodu == ulkeKodu & s.IlKodu == ilKodu).FirstOrDefault();
            if (il != null)
                result = il.IlAdi;

            return result;
        }

        public List<Il> GetIlList()
        {
            return _Context.IlRepository.All().OrderBy(o => o.IlKodu).ToList<Il>();
        }

        public List<Il> GetIlList(string ulkeKodu)
        {
            return _Context.IlRepository.Filter(f => f.UlkeKodu == ulkeKodu).OrderBy(o => o.IlKodu).ToList<Il>();
        }

        public Il CreateIl(Il il)
        {
            il = _Context.IlRepository.Create(il);
            _Context.Commit();
            return il;
        }

        public void UpdateIl(Il il)
        {
            _Context.IlRepository.Update(il);
            _Context.Commit();
        }
        #endregion

        #region İlçe
        public Ilce GetIlce(int ilceKodu)
        {
            return _Context.IlceRepository.FindById(ilceKodu);
        }

        public string GetIlceAdi(int ilceKodu)
        {
            string result = String.Empty;

            Ilce ilce = _Context.IlceRepository.Filter(s => s.IlceKodu == ilceKodu).FirstOrDefault();
            if (ilce != null)
                result = ilce.IlceAdi;

            return result;
        }

        public List<Ilce> GetIlceList()
        {
            return _Context.IlceRepository.All().ToList<Ilce>();
        }

        public List<Ilce> GetIlceList(string ulkeKodu, string ilKodu)
        {
            return _Context.IlceRepository.Filter(f => f.UlkeKodu == ulkeKodu &&
                                                       f.IlKodu == ilKodu)
                                          .OrderBy(o => o.IlceAdi)
                                          .ToList<Ilce>();
        }

        public Ilce CreateIlce(Ilce ilce)
        {
            ilce = _Context.IlceRepository.Create(ilce);
            _Context.Commit();
            return ilce;
        }

        public void UpdateIlce(Ilce ilce)
        {
            _Context.IlceRepository.Update(ilce);
            _Context.Commit();
        }

        #endregion
        public ErgoIlIlce GetErgoIlIlce(string ilAdi, string ilceAdi)
        {
            ErgoIlIlce ililce = _CRContext.ErgoIlIlceRepository.All().Where(s => s.IlAdi.Contains(ilAdi) && s.IlceAdi.Contains(ilceAdi)).FirstOrDefault();
            return ililce;
        }

    }
}
