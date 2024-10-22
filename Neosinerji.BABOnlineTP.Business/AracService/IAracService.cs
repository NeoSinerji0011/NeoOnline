using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.MAPFRE;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IAracService
    {
        #region AracKullanimSekli
        AracKullanimSekli GetAracKullanimSekli(short kullanimSekliKodu);
        List<AracKullanimSekli> GetAracKullanimSekliList();
        #endregion

        #region AracKullanimTarzi
        AracKullanimTarzi GetAracKullanimTarzi(string kullanimTarziKodu, string kod2);
        List<AracKullanimTarzi> GetAracKullanimTarziList();
        List<AracKullanimTarzi> GetAracKullanimTarziList(short kullanimSekliKodu);
        List<AracKullanimTarziServisModel> GetAracKullanimTarziTeklif(short kullanimSekliKodu);
        #endregion

        #region AracMarka
        AracMarka GetAracMarka(string markaKodu);
        List<AracMarka> GetAracMarkaList(string kullanimTarziKodu);
        List<AracMarka> GetAracMarkaList(string[] kullanimTarziKodlari);
        List<AracMarka> GetAracMarkaList();
        #endregion

        #region AracTip
        AracTip GetAracTip(string markaKodu, string tipKodu);
        List<AracTip> GetAracTipList();
        List<AracTip> GetAracTipList(string markaKodu);
        List<AracTip> GetAracTipList(string kullanimTarziKodu, string markaKodu);
        List<AracTip> GetAracTipList(string kullanimTarziKodu, string markaKodu, int model);
        List<AracTip> GetAracTipList(string[] kullanimTarziKodlari, string markaKodu, int model);
        #endregion

        #region AracModel
        List<AracModel> GetAracModelList(string markaKodu, string tipKodu);
        AracModel GetAracModel(string markaKodu, string tipKodu, int model);
        #endregion

        short GetAracKisiSayisi(string markaKodu, string tipKodu);
        decimal GetAracDeger(string markaKodu, string tipkodu, int model);


        MapfreToHdiPlakaSorgu KarsilastirmaliPlakaSorgu_FillMapfre(PoliceSorguTrafikResponse response);
        MapfreToHdiPlakaSorgu KarsilastirmaliPlakaSorgu_FillMapfreKasko(TramerSorguPoliceValue bilgi);
    }
}
