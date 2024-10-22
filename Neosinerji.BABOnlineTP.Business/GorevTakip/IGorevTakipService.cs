using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.GorevTakip
{
    public interface IGorevTakipService
    {
        List<IsTipleri> GetIsTipleri();
        List<TalepKanallari> GetTalepKanallari();
        AtananIsler AddAtananIs(AtananIsler IsItem);
        bool UpdateAtananIs(AtananIsler IsItem);
        AtananIsler getIsDetay(int IsId);

        MusteriBilgi GetMusteriBilgi(string KimlikNo, int TVMKodu);
        //   List<IslerimProcedureModel> IslerimPagedList(IslerimListe arama, out int totalRowCount);
        List<IslerimProcedureModel> IslerimPagedList();
        string DurumRenk(byte durum);
        string OncelikSeviyesiRenk(byte oncelikSeviye);
        List<AtananIslerProcedureModel> GorevDagilimRaporuPagedList(AtananIslerimListe arama, out int totalRowCount);

        #region Dökümanlar
        IQueryable<AtananIsDokumanlar> GetListDokumanlar(int isId);
        AtananIsDokumanlar CreateDokuman(AtananIsDokumanlar dokuman);
        AtananIsDokumanlar GetTVMDokuman(int siraNo, int isId);
        void DeleteDokuman(int dokumanKodu, int isId);
        #endregion

        #region Notlar
        IQueryable<AtananIsNotlar> GetListNotlar(int isId);
        AtananIsNotlar CreateNot(AtananIsNotlar not);
        void DeleteNot(int notId);

        #endregion
    }
    public class MusteriBilgi
    {
        public string Adi { get; set; }
        public string Soyadi { get; set; }
    }

}
