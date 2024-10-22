using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface IWebServiceLogListService
    {
        #region WebServiceLogList Servis
        List<WebServiceLogModelOzel> PagedList(WebServiceLogListe wbsrvclgListe);
        #endregion
    }
}
