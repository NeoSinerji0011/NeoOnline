using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ITanimService
    {

        #region Meslek Service Members
        List<Meslek> GetListMeslek();
        List<Meslek> GetListMeslek(string startsWith);
        Meslek GetMeslek(int meslekKodu);
        #endregion

        #region Tanim Service Members
        List<GenelTanimlar> GetListTanimlar();
        List<GenelTanimlar> GetListTanimlar(string tanimTipi);
        GenelTanimlar GetTanim(string tanimTipi, string tanimID);
        List<GenelTanimlar> GetListAltSektor(string anaSektorKodu);
        Tuple<string, string> Get_AnaAlt_SektorKodu(string anaSektorAciklama, string altSektorAciklama);
        GenelTanimlar GetTanimByAciklama(string tanimTipi, string aciklama);
        #endregion

    }
}
