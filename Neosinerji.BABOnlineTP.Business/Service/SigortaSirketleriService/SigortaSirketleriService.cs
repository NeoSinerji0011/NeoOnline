using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business
{
    public class SigortaSirketleriService : ISigortaSirketleriService
    {
        IParametreContext _UnitOfWork;

        public SigortaSirketleriService(IParametreContext unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        #region IEPostaService Members

        public SigortaSirketleri GetSirket(string sirketKodu)
        {
            return _UnitOfWork.SigortaSirketleriRepository.FindById(sirketKodu);
        }

        public List<SigortaSirketleri> GetList(bool isUnderWriter = false)
        {
            List<SigortaSirketleri> list =new List<SigortaSirketleri>();
            if (isUnderWriter)
            {
                var templist = _UnitOfWork.SigortaSirketleriRepository.All().Where(w => w.UygulamaKodu == 3).OrderBy(s => s.SirketAdi).ToList();
                foreach (var item in templist)
                {
                    if (int.Parse(item.SirketKodu) > 500 && int.Parse(item.SirketKodu) < 600)
                    { 
                        list.Add(item);
                    }
                }
            }
            else
            {
                list = _UnitOfWork.SigortaSirketleriRepository.All().Where(w => w.UygulamaKodu != UygulamaKodlari.NeoConnect).OrderBy(s => s.SirketAdi).ToList();
            }
            return list;
        }

        public List<SigortaSirketleri> GetNeoConnectSirketListesi()
        {
            return _UnitOfWork.SigortaSirketleriRepository.All().OrderBy(s => s.SirketAdi).ToList();
        }

        public string GetSigortaSirketleriUnvan(string kodu)
        {
            string result = String.Empty;
            kodu = kodu.PadLeft(3, '0');
            SigortaSirketleri tum = _UnitOfWork.SigortaSirketleriRepository.Filter(s => s.SirketKodu == kodu).FirstOrDefault();
            if (tum != null)
                result = tum.SirketAdi;

            return result;
        }
        public SigortaSirketleri GetSigortaBilgileri(string SirketKodu)
        {
            SigortaSirketleri result = new SigortaSirketleri(); ;

            SigortaSirketleri tum = _UnitOfWork.SigortaSirketleriRepository.Filter(s => s.SirketKodu == SirketKodu).FirstOrDefault();
            if (tum != null)
            {
                if (tum.SirketAdi != null)
                    result.SirketAdi = tum.SirketAdi;
                else
                    result.SirketAdi = "";

                if (tum.VergiDairesi != null)
                    result.VergiDairesi = tum.VergiDairesi;
                else
                    result.VergiDairesi = "";

                if (tum.VergiNumarasi != null)
                    result.VergiNumarasi = tum.VergiNumarasi.Trim();
                else
                    result.VergiNumarasi = "";
            }


            return result;
        }

        public SigortaSirketleri CreateItem(SigortaSirketleri sirket)
        {
            SigortaSirketleri epsta = _UnitOfWork.SigortaSirketleriRepository.Create(sirket);
            _UnitOfWork.Commit();
            return epsta;
        }

        public bool UpdateItem(SigortaSirketleri sirket)
        {
            _UnitOfWork.SigortaSirketleriRepository.Update(sirket);
            _UnitOfWork.Commit();
            return true;
        }

        public void DeleteSirket(string sirketKodu)
        {
            _UnitOfWork.SigortaSirketleriRepository.Delete(m => m.SirketKodu == sirketKodu);
            _UnitOfWork.Commit();
        }
        #endregion

    }
}
