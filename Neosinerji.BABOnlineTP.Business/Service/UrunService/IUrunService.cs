using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IUrunService
    {
        #region IUrunService Members

        Urun GetUrun(int urunKodu);
        List<Urun> GetListUrun();
        List<UrunServiceModel> GetList();
        Urun CreateItem(Urun urun);
        bool UpdateItem(Urun urun);

        #endregion


        #region IUrunSoruService Members
        List<UrunSoruServiceModel> GetUrunSorulari(int urunKodu);
        void AddSoru(int urunKodu, int soruKodu);
        void DeleteSoru(int urunKodu, int soruKodu);

        #endregion


        #region IUrunTeminatService Members
        List<UrunTeminatServiceModel> GetUrunTeminatlari(int urunKodu);
        void AddTeminat(int urunKodu, int teminatKodu);
        void DeleteTeminat(int urunKodu, int teminatKodu);
        #endregion


        #region IUrunVergiService

        List<UrunVergiServiceModel> GetUrunVergileri(int urunKodu);
        void AddVergi(int urunKodu, int vergiKodu);
        void DeleteVergi(int urunKodu, int vergiKodu);

        #endregion

    }
}
