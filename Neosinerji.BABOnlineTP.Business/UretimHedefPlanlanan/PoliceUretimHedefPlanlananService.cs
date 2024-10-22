using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Neosinerji.BABOnlineTP.Business.UretimHedefPlanlanan
{
    public class PoliceUretimHedefPlanlananService : IPoliceUretimHedefPlanlananService
    {
        IKomisyonContext _KomisyonContext;
        ITVMContext _TVMContext;

        public PoliceUretimHedefPlanlananService(IKomisyonContext komisyonContext, ITVMContext tvmContext)
        {
            _KomisyonContext = komisyonContext;
            _TVMContext = tvmContext;
        }

        public List<PoliceUretimHedefPlanlanan> GetPoliceUretimHedefPlanlananListe(int acenteKodu, int Yil)
        {

            // return _KomisyonContext.PoliceUretimHedefPlanlananRepository.Filter(s => s.TVMKodu == acenteKodu && s.Donem == Yil).ToList();

            int bagliOldugutvmKod = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == acenteKodu).BagliOlduguTVMKodu;

            if (bagliOldugutvmKod!=-9999)
            {
                return _KomisyonContext.PoliceUretimHedefPlanlananRepository.All().Where(s => s.Donem == Yil && (s.TVMKodu == acenteKodu || s.TVMKoduTali == acenteKodu)).ToList();
              
            }
            else
            {
                return _KomisyonContext.PoliceUretimHedefPlanlananRepository.All().Where(s => s.Donem == Yil && s.TVMKodu == acenteKodu && s.TVMKoduTali==null).ToList();
            }
           
            

        }

        public bool CreatePoliceUretimHedefPlanlanan(PoliceUretimHedefPlanlanan model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceUretimHedefPlanlananRepository.Create(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public PoliceUretimHedefPlanlanan GetPoliceUretimHedefPlan(int id)
        {
            return _KomisyonContext.PoliceUretimHedefPlanlananRepository.FindById(id);
        }

        public bool UpdatePoliceUretimHedefPlanlanan(PoliceUretimHedefPlanlanan model)
        {
            bool result;
            try
            {
                _KomisyonContext.PoliceUretimHedefPlanlananRepository.Update(model);
                _KomisyonContext.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public PoliceUretimHedefPlanlanan PoliceUretimHedefPlaniVarmi(int Year, int AcenteTVMKodu)
        {
            return _KomisyonContext.PoliceUretimHedefPlanlananRepository.Find(s => s.Donem == Year && s.TVMKoduTali == AcenteTVMKodu);

        }

        public List<PoliceUretimHedefPlanlanan> GetPoliceUretimHedefPlaniAramaEkrani(int tvmKodu, DateTime basTarih)
        {

            var yetkiliTvmler = _TVMContext.TVMDetayRepository.All().Where(s => s.Kodu == tvmKodu || s.BagliOlduguTVMKodu == tvmKodu).Select(t => t.Kodu).ToArray();

            return _KomisyonContext.PoliceUretimHedefPlanlananRepository.All().Where(s => s.KayitTarihi == basTarih && yetkiliTvmler.Contains(s.TVMKoduTali ?? 0)).ToList();

        }

       

    }
}
