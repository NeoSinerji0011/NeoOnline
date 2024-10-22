using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public interface ICommonService
    {
        string GetDosyaTipAciklama(byte tipKodu);
        byte GetFileType(string extension);
        int GunFarkikBul(DateTime dt1, DateTime dt2);
        List<SelectListItem> DonemAralikListesi();
    }
}
