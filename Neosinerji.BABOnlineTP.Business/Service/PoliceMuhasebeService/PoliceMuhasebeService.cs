using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceMuhasebe;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Service.PoliceMuhasebeService
{
    public class PoliceMuhasebeService : IPoliceMuhasebeService
    {
        IPoliceContext policeContext;
        ITVMContext tvmContext;
        ITUMContext tumContext;
        public PoliceMuhasebeService(IPoliceContext policeContext, ITVMContext tvmContext, ITUMContext tumContext)
        {
            this.policeContext = policeContext;
            this.tvmContext = tvmContext;
            this.tumContext = tumContext;
        }

        public List<PoliceGenel> getPoliceMuhasebeList()
        {
            return policeContext.PoliceGenelRepository.All().ToList<PoliceGenel>();
        }


        public PoliceEntity createEntityFromModel(PoliceGenel PoliceGenel, TVMDetay tvmDetay)
        {
            PoliceEntity entity = new PoliceEntity(PoliceGenel);
            //entity.SigortaEttiren = new SigortaEttirenEntity(PoliceGenel.PoliceSigortaEttiren);
            //this.Sigortali = new SigortaliEntity(policeGenel.PoliceSigortali);
            //PoliceEntity entity = new PoliceEntity();

            //TUMDetay tumDetay = tumContext.TUMDetayRepository.FindById(PoliceGenel.TUMKodu);

            //TVMDetay tvmDetay = tvmContext.TVMDetayRepository.Find(t => t.Kodu == PoliceGenel.TVMKodu);

            //if (tumDetay != null)
            //{
            //        entity.SigortaSirketKodu = tumDetay.BirlikKodu;
            //    int sirketKodu = int.Parse(tumDetay.BirlikKodu);
            //    TVMAcentelikleri tvmAcentelik = tvmContext.TVMAcentelikleriRepository.Find(t => t.TVMKodu == PoliceGenel.TVMKodu && t.SigortaSirketKodu == sirketKodu);
            //    String urunKodu = PoliceGenel.UrunKodu.ToString();
            //    TUMUrunleri tumUrun = tumContext.TUMUrunleriRepository.Find(t => t.TUMKodu == tumDetay.Kodu && t.TUMUrunKodu.Equals(urunKodu));
            //    entity.UrunAdi = tumUrun.TUMUrunAdi;
            //}

            //entity.Acente = new AcenteEntity(tvmDetay, tumDetay);

            entity.Acente = new AcenteEntity(tvmDetay);
            return entity;
        }

        public List<PoliceEntity> createEntityListFromModelList(List<PoliceGenel> PoliceGenelList, TVMDetay tvmDetay)
        {
            List<PoliceEntity> returnList = new List<PoliceEntity>();

            foreach (PoliceGenel policeGenel in PoliceGenelList)
            {
                PoliceEntity entity = createEntityFromModel(policeGenel, tvmDetay);
                returnList.Add(entity);
            }
            return returnList;
        }


        public PoliceKeyEntity createKeyEntityFromModel(PoliceGenel PoliceGenel)
        {
            PoliceKeyEntity keyEntity = new PoliceKeyEntity();
            keyEntity.PoliceNo = PoliceGenel.PoliceId;
            keyEntity.PoliceHash = PoliceGenel.HashCode;
            return keyEntity;
        }

        public List<PoliceKeyEntity> createKeyEntityListFromModelList(List<PoliceGenel> PoliceGenelList)
        {
            List<PoliceKeyEntity> returnList = new List<PoliceKeyEntity>();
            foreach (PoliceGenel policeGenel in PoliceGenelList)
            {
                PoliceKeyEntity keyEntity = this.createKeyEntityFromModel(policeGenel);
                returnList.Add(keyEntity);
            }
            return returnList;
        }

        public PoliceGenel getPoliceByIdHash(int id, string hash)
        {
            return policeContext.PoliceGenelRepository.Find(t => t.PoliceId == id);
        }

        public PoliceGenel getPoliceByKey(string SigortaSirketKodu, string PoliceNo, int YenilemeNo, int EkNo)
        {
            return policeContext.PoliceGenelRepository.Find(t => t.TUMBirlikKodu == SigortaSirketKodu && t.PoliceNumarasi == PoliceNo && t.YenilemeNo == YenilemeNo && t.EkNo == EkNo);
        }

        public List<PoliceGenel> getPoliceMuhasebeList(int acenteKodu, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            return policeContext.PoliceGenelRepository.Filter(t => t.TVMKodu == acenteKodu && t.TanzimTarihi >= baslangicTarihi && t.TanzimTarihi <= bitisTarihi).ToList();
        }




        //***************


        //***************
    }
}
