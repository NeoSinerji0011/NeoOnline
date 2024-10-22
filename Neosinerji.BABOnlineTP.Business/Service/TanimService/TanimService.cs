using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class TanimService : ITanimService
    {

        IParametreContext _UnitOfWork;
        public TanimService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #region Meslek Service Members
        public List<Meslek> GetListMeslek()
        {
            return _UnitOfWork.MeslekRepository.All().ToList<Meslek>();
        }

        public List<Meslek> GetListMeslek(string startsWith)
        {
            return _UnitOfWork.MeslekRepository.Filter(f => f.MeslekAdi.StartsWith(startsWith)).ToList<Meslek>();
        }

        public Meslek GetMeslek(int meslekKodu)
        {
            Meslek meslek = _UnitOfWork.MeslekRepository.FindById(meslekKodu);
            return meslek;
        }

        #endregion

        #region Tanim Service Members

        public List<GenelTanimlar> GetListTanimlar()
        {
            return _UnitOfWork.GenelTanimlarRepository.All().ToList<GenelTanimlar>();
        }

        public List<GenelTanimlar> GetListTanimlar(string tanimTipi)
        {
            return _UnitOfWork.GenelTanimlarRepository.Filter(s => s.TanimTipi == tanimTipi).ToList<GenelTanimlar>();
        }

        public List<GenelTanimlar> GetListAltSektor(string anaSektorKodu)
        {
            int anaKodu = 0;
            if (int.TryParse(anaSektorKodu, out anaKodu))
            {
                List<GenelTanimlar> altSektor = new List<GenelTanimlar>();
                var list = _UnitOfWork.GenelTanimlarRepository.Filter(s => s.TanimTipi == "FaaliyetAltSektor");

                foreach (var item in list)
                {
                    if (Convert.ToInt32(item.TanimId) > (anaKodu - 1) && Convert.ToInt32(item.TanimId) < (anaKodu + 200))
                    {
                        altSektor.Add(item);
                    }
                }

                //List<GenelTanimlar> altSektor = _UnitOfWork.GenelTanimlarRepository
                //                                .Filter(s => Convert.ToInt32(s.TanimId) <= anaKodu ||
                //                                             Convert.ToInt32(s.TanimId) >= anaKodu & s.TanimTipi == "FaaliyetAltSektor")
                //                                .ToList<GenelTanimlar>();
                return altSektor;
            }
            else return null;
        }

        public GenelTanimlar GetTanim(string tanimTipi, string tanimID)
        {
            return _UnitOfWork.GenelTanimlarRepository.FindById(new object[] { tanimTipi, tanimID });
        }

        public GenelTanimlar GetTanimByAciklama(string tanimTipi, string aciklama)
        {
            GenelTanimlar tanim = _UnitOfWork.GenelTanimlarRepository.Filter(s => s.TanimTipi == tanimTipi && s.Aciklama == aciklama).FirstOrDefault();
            return tanim;
        }

        public Tuple<string, string> Get_AnaAlt_SektorKodu(string anaSektorAciklama, string altSektorAciklama)
        {
            if (!String.IsNullOrEmpty(anaSektorAciklama) && !String.IsNullOrEmpty(altSektorAciklama))
            {
                GenelTanimlar anaSektor = _UnitOfWork.GenelTanimlarRepository.Filter(s => s.Aciklama == anaSektorAciklama
                                                                                     && s.TanimTipi == "FaaliyetAnaSektor").FirstOrDefault();
                if (anaSektor != null)
                {
                    GenelTanimlar altSektor = _UnitOfWork.GenelTanimlarRepository.Filter(s => s.Aciklama == altSektorAciklama
                                                                                        && s.TanimTipi == "FaaliyetAltSektor").FirstOrDefault();

                    if (altSektor != null)
                    {
                        return Tuple.Create(anaSektor.TanimId, altSektor.TanimId);
                    }
                    else return Tuple.Create("", "");
                }
                else return Tuple.Create("", "");
            }
            else
                return Tuple.Create("", "");
        }


        #endregion

    }
}
