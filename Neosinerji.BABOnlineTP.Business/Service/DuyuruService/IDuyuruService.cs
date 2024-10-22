using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IDuyuruService
    {
        #region Duyurular Members
        Duyurular CreateDuyuru(Duyurular duyuru, string[] tvmList, DuyuruDokuman dokuman);
        Duyurular GetDuyuru(int duyuruId);
        List<Duyurular> GetAllDuyuru();
        List<string> GetEkliTVMName(int duyuruId);
        Duyurular GetDuyuruAnaSayfa(int duyuruId);
        bool UpdateDuyuru(Duyurular duyuru, string[] tvmList);
        List<DuyuruProcedureModel> GetListDuyuruByTvmId(int tvmKodu);
        bool DeleteDuyuru(int DuyuruId);
        #endregion

        #region DuyuruTVM Members
        int[] GetAddedTVMList(int duyuruId);
        #endregion

        #region DuyuruDokuman Members
        List<DuyuruDokuman> GetDokumanByDuyuruId(int duyuruId);
        bool DeleteDokuman(int duyuruDokumanId);
        int GetDuyuruIdByDokumanId(int duyuruDokumanId);
        #endregion

    }
}
