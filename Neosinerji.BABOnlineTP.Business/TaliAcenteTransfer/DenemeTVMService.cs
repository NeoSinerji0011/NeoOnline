using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer
{
    public class DenemeTVMService : IDenemeTVMService
    {
        ITVMContext _TVMContext;
        IParametreContext _UnitOfWork;
        IAktifKullaniciService _AktifKullanici;

        public DenemeTVMService(ITVMContext tvmContext, IParametreContext unitOfWork, IAktifKullaniciService aktifKullanici)
        {
            _TVMContext = tvmContext;
            _UnitOfWork = unitOfWork;
            _AktifKullanici = aktifKullanici;
        }

        public bool AddDenemeTVM(TVMDenemeSurum tvmDeneme)
        {
            try
            {
                bool kayitVarMi = this.KayitVarMi(tvmDeneme.TVMDetay.Email);
                if (!kayitVarMi)
                {
                    _TVMContext.TVMDetayRepository.Create(tvmDeneme.TVMDetay);
                    _TVMContext.Commit();

                    return true;
                }
                else return false;


            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool KayitVarMi(string email)
        {
            TVMDetay kayit = _TVMContext.TVMDetayRepository.Find(s => s.Email == email);
            if (kayit != null) return true;
            else return false;
        }
    }
}
