using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business;

namespace Neosinerji.BABOnlineTP.Business
{
       
    public class TVMProjeKoduSQLs
    {
        ITVMContext _TVMContext;
        IKullaniciService _KullaniciService;
        public TVMProjeKoduSQLs(ITVMContext tvmContext,
                                IKullaniciService kullanici)
        {
            _TVMContext = tvmContext;
            _KullaniciService = kullanici;
        }
       
        public string ToplamKullaniciSayisi(int tvmKodu)
        {
             return "0";
        }
        public int DuzenlenenTeklifSayisi(string projeKodu, int urunKodu, int TeklifDurumKodu)
        {
            return 1;
        }
    }
}
