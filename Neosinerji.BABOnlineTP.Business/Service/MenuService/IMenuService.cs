using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IMenuService
    {
        List<AnaMenu> GetAnaMenuListYetkili();
        List<AnaMenu> GetAnaMenuList();
        AnaMenu GetAnaMenu(int Id);
        AnaMenu CreateAnaMenu(AnaMenu anaMenu);
        bool UpdateAnaMenu(AnaMenu anaMenu);
        bool CheckAltMenu(int AnaMenuKodu);
        bool DeleteAnaMenu(int AnaMenuKodu);

        #region IAltMenuService  Members

        List<AltMenu> GetAltMenuListYetkili();
        List<AltMenu> GetAltMenuList();
        List<AltMenu> GetAltMenuList(int AnaMenuKodu);
        AltMenu GetAltMenu(int altMenuKodu, int anaMenuKodu);
        AltMenu CreateAltMenu(AltMenu altMenu);
        bool UpdateAltMenu(AltMenu altMenu);
        DataTableList PagedListAltMenuList(DataTableParameters<AltMenu> altMenuList, int anaMenuKodu);
        bool CheckAltMenuSekme(int altmenekodu);
        bool DeleteAltMenu(int anamenukodu, int altmenukodu);
        #endregion


        #region IAltMenuSekmeService Members

        List<AltMenuSekme> GetALtMenuSekmeListYetkili();
        List<AltMenuSekme> GetALtMenuSekmeList();
        List<AltMenuSekme> GetALtMenuSekmeList(int altMenuKodu, int anaMenuKodu);
        AltMenuSekme GetAltMenuSekme(int altMenuKodu, int sekmeKodu);
        AltMenuSekme CreateALtMenuSekme(AltMenuSekme altMenuSekme);
        bool UpdateAltMenuSekme(AltMenuSekme altmenuSekme);
        bool DeleteAltMenuSekme(int sekmeKodu);

        #endregion


        #region IMenuIslemSErvice Members

        List<MenuIslem> GetListMenuIslem();
        MenuIslem GetMenuIslem(int islemKodu);
        MenuIslem CreateMenuIslem(MenuIslem menuIslem);
        bool UpdateMenuIslem(MenuIslem menuIslem);
        bool DeleteMenuIslem(int islemKodu);

        #endregion


    }
}
