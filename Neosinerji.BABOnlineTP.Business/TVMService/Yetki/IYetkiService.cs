using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IYetkiService
    {

        #region YetkiGrubu Members
        string GetYetkiGrupAdi(int yetkiGrupKodu);
        TVMYetkiGruplari GetYetkiGrup(int yetkiGrupKodu);
        List<YetkiListeServiceModel> GetListYetkiGrup(int? kullaniciKodu);
        List<YetkiListeServiceModel> GetListYetkiGrupByTVMKodu(int tvmKodu);
        List<TVMYetkiGruplari> GetListYetkiGrup(int TVMkodu);
        TVMYetkiGruplari CreateYetkiGrubu(TVMYetkiGruplari yetkiGrubu);
        void UpdateYetkiGrubu(TVMYetkiGruplari yetkiGrubu);
        bool CheckAuthorityYetkiGrup(int yetkiGrupKodu);
        #endregion

        #region YetkiGrupYetki Members
        List<TVMYetkiGrupYetkileri> GetListYetkiGrupYetki(int yetkiGrupkodu);
        TVMYetkiGrupYetkileri CreateYetkiGrupYetkileri(TVMYetkiGrupYetkileri yetkiGrupYetkileri);
        void UpdateYetkiGrupYetkileri(TVMYetkiGrupYetkileri yetkiGrupYetkileri);
        #endregion
        List<TVMYetkiGrupSablonu> GetListTVMYetkiGrupSablonu(int yetkiGrupkodu);
        TVMYetkiGruplari GetYetkiGrupKontrolu(int yetkiGrupKodu, int tvmkodu);

    }
}
